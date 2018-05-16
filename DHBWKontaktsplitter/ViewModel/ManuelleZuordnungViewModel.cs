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
    public class ManuelleZuordnungViewModel : ViewModelBase
    {
        #region Properties
        private List<ZuordnungModel> _inputList = new List<ZuordnungModel>();
        #endregion

        public ManuelleZuordnungViewModel()
        {
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

        private void AddNewTitleCommandExecute(object obj)
        {
            InputListObservable.Add(new ZuordnungModel());
        }

        private void SaveZuordnungCommandExecute(object obj)
        {
            //Lernfunktion für Titel
            //Alle Titel von der Oberfläche ermitteln und versuchen in die Datenbank zu speichern
            var titles = InputListObservable.ToList().FindAll(x => x.SelectedDropDownEntry == StaticHelper.Titel);
            foreach(var entry in titles)
            {
                try
                {
                    //TODO Erster Parameter weg
                    var titleInsertCommand = DBQuery.CreateSqlParameterSearchTitle(entry.EntryText.ToLower().Trim());
                    int resCount = DatabaseHelper.InsertDatabase(titleInsertCommand);
                }
                catch(Exception ex)
                {
                    //Titel ist bereits in Datenbank
                    Console.WriteLine(ex.ToString());
                }
            }

            ((Window)obj).Close();
        }
        #endregion

        #region Methods

        #endregion
    }
}