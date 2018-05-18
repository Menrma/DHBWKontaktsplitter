using DHBWKontaktsplitter.Framework;
using DHBWKontaktsplitter.Model;
using DHBWKontaktsplitter.View;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace DHBWKontaktsplitter.ViewModel
{
    /// <summary>
    /// ViewModel der MainWindowView
    /// </summary>
    public class MainWindowViewModel : ViewModelBase
    {
        #region Properties
        private string _eingabe = String.Empty;
        private Parser _parser = new Parser();
        private ContactModel _contactModel = new ContactModel();
        private ExecutionModel execModel = new ExecutionModel();
        private bool _editButtonIsEnabled = false;
        #endregion

        public MainWindowViewModel()
        {
            //Commands der Oberfläche registrieren
            EnterCommand = new RelayCommand(EnterCommandExecute);
            AddNewTitleCommand = new RelayCommand(AddNewTitleCommandExecute);
            EditContactCommand = new RelayCommand(EditContactCommandExecute);
            SaveContactCommand = new RelayCommand(SaveContactCommandExecute);
        }

        #region Dependency Properties
        public string Eingabe
        {
            get { return _eingabe; }
            set
            {
                _eingabe = value;
                OnPropertyChanged("Eingabe");
            }
        }

        public ContactModel Contact
        {
            get { return _contactModel; }
            set
            {
                _contactModel = value;
                OnPropertyChanged("Contact");
            }
        }

        public bool EditButtonIsEnabled
        {
            get { return _editButtonIsEnabled; }
            set
            {
                _editButtonIsEnabled = value;
                OnPropertyChanged("EditButtonIsEnabled");
            }
        }

        #endregion

        #region Commands
        public ICommand EnterCommand { get; set; }
        public ICommand AddNewTitleCommand { get; set; }
        public ICommand EditContactCommand { get; set; }
        public ICommand SaveContactCommand { get; set; }

        /// <summary>
        /// Methode die beim Drücken der Enter-Taste ausgeführt wird
        /// </summary>
        /// <param name="obj"></param>
        private void EnterCommandExecute(object obj)
        {
            //Parser aufrufen
            execModel = _parser.ExecuteInput(Eingabe);

            //Prüfen ob während der Verarbeitung Fehler aufgetreten sind
            if (execModel.HasError)
            {
                //Ändern-Button nicht auswählbar
                EditButtonIsEnabled = false;
                //Text anhand der ErrorId aus der Datenbank lesen
                var text = DatabaseHelper.GetNotificationText(execModel.ErrorId);
                //Fehler in einer MessageBox anzeigen
                MessageBox.Show(text, "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
            {
                //Während der Verabeitung sind keine Fehler aufgetreten
                //Prüfen ob eine Benachrichtigung angezeigt werden muss
                if (execModel.NotificationId != 0)
                {
                    //Text anhand der NotificationId aus der Datenbank lesen
                    var text = DatabaseHelper.GetNotificationText(execModel.NotificationId);
                    //Benachrichtigung in einer MessageBox anzeigen
                    MessageBox.Show(text, "Warnung", MessageBoxButton.OK, MessageBoxImage.Warning);
                }

                //Formatierer aufrufen
                var formattedContact = Formatter.DoFormat(execModel.Contact);
                //Ändern-Button auswählbar machen
                EditButtonIsEnabled = true;
            }

            //Prüfen ob die Eingabe nicht zuordenbare Bestandteile enthält
            if(execModel.SplittedInput.Count > 0)
            {
                //Nicht zuordenbare Inhalt vorhanden
                //Weiteres Fenster für die manuelle Zuordnung öffnen
                var editList = _openManuelleZuordnung(false, Visibility.Hidden);
                //Manuell zugeordnete Inhalte in das Model mappen
                _mapZurodnungToModel(editList);
            }
            else
            {
                //Alle Inhalte konnten zugeordnet werden
                //Anzeigen des Ergebnisses auf der Benutzeroberfläche
                Contact = execModel.Contact;
            }
        }

        /// <summary>
        /// Methode die beim Drücken des 'Neuer-Titel'-Buttons ausgeführt wird
        /// </summary>
        /// <param name="obj"></param>
        private void AddNewTitleCommandExecute(object obj)
        {
            //Fenster zum Anlegen eines neuen Titels öffnen
            new AddNewTitleView().Show();
        }

        /// <summary>
        /// Methode die beim Drücken des 'Kontakt ändern'-Buttons ausgeführt wird
        /// </summary>
        /// <param name="obj"></param>
        private void EditContactCommandExecute(object obj)
        {
            //Fenster für die manuelle Zuordnung öffnen
            var editList = _openManuelleZuordnung(true, Visibility.Visible);
            //Manuell zugeordnete Werte in das Model mappen
            _mapZurodnungToModel(editList);
        }

        /// <summary>
        /// Methode die beim Drücken des 'Speichern'-Buttons ausgeführt wird
        /// </summary>
        /// <param name="obj"></param>
        private void SaveContactCommandExecute(object obj)
        {
            //Methode zum Speichern des Models aufrufen
            int res = _saveContact();

            //Überprüfen ob das Speichern funktioniert hat
            //Fehler- oder Benachrichtigungs-Text aus der Datenbank lesen
            var text = DatabaseHelper.GetNotificationText(res);
            if (res != 11)
            {
                //Fehlermeldung in einer MessageBox anzeigen
                MessageBox.Show(text, "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
            {
                //Benachrichtigung in einer MessageBox anzeigen
                MessageBox.Show(text, "Gespeichert", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        #endregion

        #region Methods
        /// <summary>
        /// Methode zum Erstellen der Liste für die manuelle Zuordnung
        /// </summary>
        /// <param name="execModel">Ergebnis des Parse-Vorgangs</param>
        /// <param name="changeContact">Wert der angibt, ob der Kontakt geändert wird (true)</param>
        /// <returns></returns>
        private List<ZuordnungModel> _createZuordnungList(ExecutionModel execModel, bool changeContact = false)
        {
            var list = new List<ZuordnungModel>();
            if (changeContact)
            {
                //Kontakt wird geändert
                //Alle Bestandteile der Anrede in Liste speichern, um diese auf der Benutzeroberfläche anzuzeigen
                list.Add(new ZuordnungModel { EntryText = execModel.Contact.AnredeText, SelectedDropDownEntry = StaticHelper.Anrede });
                list.Add(new ZuordnungModel { EntryText = execModel.Contact.BriefanredeText, SelectedDropDownEntry = StaticHelper.Briefanrede });

                foreach(var entry in execModel.Contact.TitelList)
                {
                    list.Add(new ZuordnungModel { EntryText = entry.Title, SelectedDropDownEntry = StaticHelper.Titel });
                }

                //list.Add(new ZuordnungModel { LabelText = StaticHelper.Titel, EntryText = execModel.Contact.TitelText });
                list.Add(new ZuordnungModel {  EntryText = execModel.Contact.GeschlechtText, SelectedDropDownEntry = StaticHelper.Geschlecht });
                list.Add(new ZuordnungModel { EntryText = execModel.Contact.Vorname, SelectedDropDownEntry = StaticHelper.Vorname });
                list.Add(new ZuordnungModel { EntryText = execModel.Contact.Nachname, SelectedDropDownEntry = StaticHelper.Nachname });
            }
            else
            {
                //Kontakt wird nicht bearbeitet
                //Es liegen nicht zuordenbare Inhalte vor
                //Lediglich diese in die Liste speichern
                foreach (var entry in execModel.SplittedInput)
                {
                    list.Add(new ZuordnungModel { EntryText = entry });
                }
            }

            return list;
        }

        /// <summary>
        /// Methode zum öffnen des Fensters für die manuelle Zuordnung
        /// </summary>
        /// <param name="textBoxIsEnabled">Paramter der angibt, ob auf dem zu öffnenden Fenster der die Textboxen bearbeitbar sind</param>
        /// <param name="newTitleButtonVisibility">Paramter der angibt, ob auf dem zu öffnenden Fenster weitere Einträge hinzugefügt werden können</param>
        /// <returns></returns>
        private List<ZuordnungModel> _openManuelleZuordnung(bool textBoxIsEnabled, Visibility newTitleButtonVisibility)
        {
            bool changeContact = false;
            if (textBoxIsEnabled) changeContact = true;
            //Liste mit Einräge erstellen, die auf der Benutzeroberfläche angezeigt werden sollen
            var list = _createZuordnungList(execModel, changeContact);
            var view = new ManuelleZuordnungView();
            var viewModel = new ManuelleZuordnungViewModel();
            //ViewModel bekommt die Liste mit den darzustellenden Elementen
            viewModel.InputList = list;
            //Parameter setzen
            viewModel.TextBoxIsEnabled = textBoxIsEnabled;
            viewModel.NewTitleButtonVisibility = newTitleButtonVisibility;
            view.DataContext = viewModel;
            //Bentzeroberfläche öffnen
            view.ShowDialog();

            //Alle Zuordnungen, welche auf der Benutzeroberfläche vorgenommen wurden, zurückgeben
            return viewModel.InputListObservable.ToList();
        }

        /// <summary>
        /// Methode zum Synchronisieren der manuellen Zuordnung mit dem Model
        /// </summary>
        /// <param name="zuordnungList">Liste mit Zuordnungen</param>
        private void _mapZurodnungToModel(List<ZuordnungModel> zuordnungList)
        {
            if (zuordnungList.Count == 0) return;

            var tmpContact = execModel.Contact;
            
            //Über Liste der Zuordnungen loopen
            foreach(var entry in zuordnungList)
            {
                //Prüfen ob Anrede
                if(entry.SelectedDropDownEntry == StaticHelper.Anrede)
                {
                    tmpContact.AnredeText = entry.EntryText;
                }
                //Prüfen ob Briefanrede
                else if(entry.SelectedDropDownEntry == StaticHelper.Briefanrede)
                {
                    tmpContact.BriefanredeText = entry.EntryText;
                }
                //Prüfen ob Titel
                else if(entry.SelectedDropDownEntry == StaticHelper.Titel)
                {
                    var result = execModel.Contact.TitelList.Find(x => x.Title == entry.EntryText);
                    if (result != null) continue;

                    tmpContact.AllTitles += " " + entry.EntryText;
                    tmpContact.TitelList.Add(new TitleModel
                    {
                        Title = entry.EntryText
                    });
                }
                //Pürfen ob Geschlecht
                else if(entry.SelectedDropDownEntry == StaticHelper.Geschlecht)
                {
                    execModel.Contact.GeschlechtText = entry.EntryText;
                }
                //Pürfen ob Vorname
                else if(entry.SelectedDropDownEntry == StaticHelper.Vorname && execModel.Contact.Vorname != entry.EntryText)
                {
                    tmpContact.Vorname += entry.EntryText;
                }
                //Prüfen ob Nachname
                else if(entry.SelectedDropDownEntry == StaticHelper.Nachname && execModel.Contact.Nachname != entry.EntryText)
                {
                    tmpContact.Nachname += " " + entry.EntryText;
                }

                //Gemapptes Model auf der Benutzeroberfläche anzeigen
                Contact = tmpContact;
            }
        }

        private int _saveContact()
        {
            int res = Validator.ValidateContact(Contact);
            if (res != 0)
            {
                return res;
            }
            else
            {
                //Beginn Speicher-Vorgang...
                try
                {
                    var insertContactParameter = DBQuery.CreateSqlParameterSaveContact(Contact);
                    var cnt = DatabaseHelper.InsertDatabase(insertContactParameter);

                    var getLastContactParamter = DBQuery.CreateSqlParameterLastContact();
                    var id = DatabaseHelper.CheckDatabase(getLastContactParamter);

                    int contactId = 0;
                    int.TryParse(id.Rows[0][0].ToString(), out contactId);

                    var toInsert = new List<TitleModel>();
                    //Alle noch nicht in der Datenbank vorhandene Titel ermitteln
                    var newTitles = Contact.TitelList.FindAll(x => x.Title_ID == 0);
                    foreach (var entry in newTitles)
                    {
                        //Insert neuen Titel
                        var insertTitleCommand = DBQuery.CreateSqlParameterTitle(entry.Title, true);
                        int resCount = DatabaseHelper.InsertDatabase(insertTitleCommand);

                        //Hinzugefügten Titel aus der Datenbank ermitteln
                        var selectCommand = DBQuery.CreateSqlParameterTitle(entry.Title, false);
                        var currentTitles = DatabaseHelper.CheckDatabase(selectCommand);

                        //Neu hinzugfügten Titel in Liste schreiben
                        toInsert.Add(new TitleModel
                        {
                            Title_ID = int.Parse(currentTitles.Rows[0][0].ToString()),
                            Title = currentTitles.Rows[0][1].ToString()
                        });
                    }

                    //Alle bereits in der Datenbank vorhandenen Titel ermitteln
                    var existingTitles = Contact.TitelList.FindAll(x => x.Title_ID != 0);
                    toInsert.AddRange(existingTitles);

                    //Über alle dem Kontakt zugeordneten Titel loopen und diese in die Datenbank schreiben
                    foreach (var entry in toInsert)
                    {
                        var insertTitleContactParameter = DBQuery.CreateSqlParameterSaveTitle(contactId, entry.Title_ID);
                        var resTitleContact = DatabaseHelper.InsertDatabase(insertTitleContactParameter);
                    }

                }
                catch (Exception ex)
                {
                    //Fehler während der Verarbeitung
                    return 10;
                }

                return 11;
            }
        }

        #endregion
    }
}
