using DHBWKontaktsplitter.Framework;
using DHBWKontaktsplitter.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

        }

        private void SaveZuordnungCommandExecute(object obj)
        {

        }
        #endregion

        #region Methods
        #endregion
    }

    public class ZuordnungModel
    {
        public string EntryText { get; set; }
        public string LabelText { get; set; }
        public ObservableCollection<string> DropDownEntries { get; set; } = new ObservableCollection<string>(StaticHelper.BestandteileList);
        public string SelectedDropDownEntry { get; set; } = string.Empty;
    }
}