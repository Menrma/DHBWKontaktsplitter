using DHBWKontaktsplitter.Framework;
using DHBWKontaktsplitter.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace DHBWKontaktsplitter.ViewModel
{
    /// <summary>
    /// ViewModel der ManuelleZuordnungView
    /// </summary>
    public class ManuelleZuordnungViewModel : ViewModelBase
    {
        #region Properties
        private List<ZuordnungModel> _inputList = new List<ZuordnungModel>();
        #endregion

        public ManuelleZuordnungViewModel()
        {
            //Commands der Oberfläche registrieren
            AddNewTitleCommand = new RelayCommand(AddNewTitleCommandExecute);
            SaveZuordnungCommand = new RelayCommand(SaveZuordnungCommandExecute);
        }

        #region Dependency Properties
        public ObservableCollection<ZuordnungModel> InputListObservable { get; set; } = new ObservableCollection<ZuordnungModel>();

        public List<ZuordnungModel> InputList
        {
            get { return _inputList; }
            set
            {
                _inputList = value;
                InputListObservable = new ObservableCollection<ZuordnungModel>(_inputList);
                OnPropertyChanged("InputListObservable");
            }
        }

        public ContactModel ContectModel { get; set; } = new ContactModel();

        public Visibility NewTitleButtonVisibility { get; set; } = Visibility.Hidden;

        public bool TextBoxIsEnabled { get; set; } = false;
        #endregion

        #region Commands
        public ICommand AddNewTitleCommand { get; set; }
        public ICommand SaveZuordnungCommand { get; set; }

        /// <summary>
        /// Methode die beim Drücken des 'Weiterer Eintrag'-Buttons aufgerufen wird
        /// </summary>
        /// <param name="obj"></param>
        private void AddNewTitleCommandExecute(object obj)
        {
            //Initialen Eintrag auf der Benutzeroberfläche anzeigen
            InputListObservable.Add(new ZuordnungModel());
        }

        /// <summary>
        /// Methode die beim Drücken des 'Zurodnen'-Buttons ausgeführt wird
        /// </summary>
        /// <param name="obj"></param>
        private void SaveZuordnungCommandExecute(object obj)
        {
            //Lernfunktion für Titel
            //Alle Elemente vom Typ Titel von der Oberfläche ermitteln und versuchen in die Datenbank zu speichern
            var titles = InputListObservable.ToList().FindAll(x => x.SelectedDropDownEntry == StaticHelper.Titel);
            //Loop über jeden Titel..
            foreach (var entry in titles)
            {
                try
                {
                    //SQL-Command für das Einfügen vrobereiten
                    var titleInsertCommand = DBQuery.CreateSqlParameterSearchTitle(entry.EntryText.ToLower().Trim());
                    //SQL-Command ausführen und Titel in die Datenbank schreiben
                    int resCount = DatabaseHelper.InsertDatabase(titleInsertCommand);
                }
                catch(Exception ex)
                {
                    //Titel ist bereits in Datenbank
                    Console.WriteLine(ex.ToString());
                }
            }

            //Nach erfolgreicher Verarbeitung ==> Fenster schließen
            ((Window)obj).Close();
        }
        #endregion
    }
}