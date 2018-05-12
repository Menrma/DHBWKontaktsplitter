using DHBWKontaktsplitter.Framework;
using DHBWKontaktsplitter.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace DHBWKontaktsplitter.ViewModel
{
    public class AddNewTitleViewModel : ViewModelBase
    {
        #region Properties
        private ObservableCollection<SpracheModel> _languList = new ObservableCollection<SpracheModel>();
        private SpracheModel _selectedLanguItem;
        private string _eingabeTitle = String.Empty;
        #endregion

        public AddNewTitleViewModel()
        {
            SaveCommand = new RelayCommand(SaveCommandExecute, SaveCommandCanExecute);
            CancelCommand = new RelayCommand(CancelCommandExecute);
            _init();
        }

        #region Dependency Properties
        public ObservableCollection<SpracheModel> LanguList
        {
            get { return _languList; }
            set
            {
                _languList = value;
                OnPropertyChanged("LanguList");
            }
        }

        public SpracheModel SelectedLanguItem
        {
            get { return _selectedLanguItem; }
            set
            {
                _selectedLanguItem = value;
                OnPropertyChanged("SelectedLanguItem");
            }
        }

        public string EingabeTitle
        {
            get { return _eingabeTitle; }
            set
            {
                _eingabeTitle = value;
                OnPropertyChanged("EingabeTitle");
            }
        }
        #endregion

        #region Commands
        public ICommand SaveCommand { get; set; }
        public ICommand CancelCommand { get; set; }


        private void CancelCommandExecute(object obj)
        {
            ((Window)obj).Close();
        }

        private void SaveCommandExecute(object obj)
        {
            EingabeTitle = Formatter.FormatNewTitle(EingabeTitle);
            var selectCommand = _createSqlParameteTitle(SelectedLanguItem.ID, EingabeTitle, false);
            var currentTitles = DatabaseHelper.CheckDatabase(selectCommand);
            if(currentTitles.Rows.Count > 0)
            {
                var text = DatabaseHelper.GetNotificationText(4);
                MessageBox.Show(text, "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
            {
                var insertCommand = _createSqlParameteTitle(SelectedLanguItem.ID, EingabeTitle, true);
                int resCount = DatabaseHelper.InsertDatabase(insertCommand);

                if (resCount == 1)
                {
                    var text = DatabaseHelper.GetNotificationText(5);
                    MessageBox.Show(text, "Gespeichert", MessageBoxButton.OK, MessageBoxImage.Information);
                    _reset();
                }
                else
                {
                    var text = DatabaseHelper.GetNotificationText(6);
                    MessageBox.Show(text, "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }

        }

        private bool SaveCommandCanExecute(object obj)
        {
            return EingabeTitle.Length > 0 && SelectedLanguItem != null;
        }
        #endregion

        #region Methods
        private void _init()
        {
            var langTable = DatabaseHelper.CheckDatabase(new SQLiteCommand(StaticHelper.GetAllLang));
            LanguList = _convertTableToList(langTable);
        }

        private void _reset()
        {
            SelectedLanguItem = null;
            EingabeTitle = String.Empty;
        }
        
        private ObservableCollection<SpracheModel> _convertTableToList(DataTable table)
        {
            var liste = new ObservableCollection<SpracheModel>();
            foreach(DataRow row in table.AsEnumerable())
            {
                liste.Add(new SpracheModel()
                {
                    ID = int.Parse(row[0].ToString()),
                    Text = row[1].ToString()
                });
            }
            return liste;
        }

        private SQLiteCommand _createSqlParameteTitle(int languId, string text, bool isInsert)
        {
            SQLiteCommand cmd = new SQLiteCommand();
            if (isInsert)
                cmd.CommandText = StaticHelper.InsertTitel;
            else
                cmd.CommandText = StaticHelper.GetTitel;

            cmd.Parameters.AddWithValue("@spracheID", languId);
            cmd.Parameters.AddWithValue("@title", text);
            return cmd;
        }
        #endregion
    }
}
