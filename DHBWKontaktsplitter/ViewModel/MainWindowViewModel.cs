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

        private void EnterCommandExecute(object obj)
        {
            execModel = _parser.ExecuteInput(Eingabe);

            if (execModel.HasError)
            {
                EditButtonIsEnabled = false;
                var text = DatabaseHelper.GetNotificationText(execModel.ErrorId);
                MessageBox.Show(text, "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
            {
                 if (execModel.NotificationId != 0)
                {
                    var text = DatabaseHelper.GetNotificationText(execModel.NotificationId);
                    MessageBox.Show(text, "Warnung", MessageBoxButton.OK, MessageBoxImage.Warning);
                }

                var formattedContact = Formatter.DoFormat(execModel.Contact);
                EditButtonIsEnabled = true;
            }

            if(execModel.SplittedInput.Count > 0)
            {
                // there are not matched items
                var editList = _openManuelleZuordnung(false, Visibility.Hidden);
                _mapZurodnungToModel(editList);
            }
            else
            {
                Contact = execModel.Contact;
            }
        }

        private void AddNewTitleCommandExecute(object obj)
        {
            new AddNewTitleView().Show();
        }

        private void EditContactCommandExecute(object obj)
        {
            var editList = _openManuelleZuordnung(true, Visibility.Visible);
            _mapZurodnungToModel(editList);
        }

        private void SaveContactCommandExecute(object obj)
        {
            int res = _saveContact();

            var text = DatabaseHelper.GetNotificationText(res);
            if (res != 11)
            {
                MessageBox.Show(text, "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
            {
                MessageBox.Show(text, "Gespeichert", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        #endregion

        #region Methods
        private List<ZuordnungModel> _createZuordnungList(ExecutionModel execModel, bool changeContact = false)
        {
            var list = new List<ZuordnungModel>();
            if (changeContact)
            {
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
                foreach (var entry in execModel.SplittedInput)
                {
                    list.Add(new ZuordnungModel { EntryText = entry });
                }
            }

            return list;
        }

        private List<ZuordnungModel> _openManuelleZuordnung(bool textBoxIsEnabled, Visibility newTitleButtonVisibility)
        {
            bool changeContact = false;
            if (textBoxIsEnabled) changeContact = true;
            var list = _createZuordnungList(execModel, changeContact);
            var view = new ManuelleZuordnungView();
            var viewModel = new ManuelleZuordnungViewModel();
            viewModel.InputList = list;
            viewModel.TextBoxIsEnabled = textBoxIsEnabled;
            viewModel.NewTitleButtonVisibility = newTitleButtonVisibility;
            view.DataContext = viewModel;
            view.ShowDialog();

            return viewModel.InputListObservable.ToList();
        }

        private void _mapZurodnungToModel(List<ZuordnungModel> zuordnungList)
        {
            if (zuordnungList.Count == 0) return;

            var tmpContact = execModel.Contact;

            foreach(var entry in zuordnungList)
            {
                if(entry.SelectedDropDownEntry == StaticHelper.Anrede)
                {
                    tmpContact.AnredeText = entry.EntryText;
                }
                else if(entry.SelectedDropDownEntry == StaticHelper.Briefanrede)
                {
                    tmpContact.BriefanredeText = entry.EntryText;
                }
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
                else if(entry.SelectedDropDownEntry == StaticHelper.Geschlecht)
                {
                    execModel.Contact.GeschlechtText = entry.EntryText;
                }
                else if(entry.SelectedDropDownEntry == StaticHelper.Vorname && execModel.Contact.Vorname != entry.EntryText)
                {
                    tmpContact.Vorname += entry.EntryText;
                }
                else if(entry.SelectedDropDownEntry == StaticHelper.Nachname && execModel.Contact.Nachname != entry.EntryText)
                {
                    tmpContact.Nachname += " " + entry.EntryText;
                }

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
