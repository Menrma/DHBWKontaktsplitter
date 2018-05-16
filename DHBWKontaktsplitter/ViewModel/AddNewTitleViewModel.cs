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
        private string _eingabeTitle = string.Empty;
        #endregion

        public AddNewTitleViewModel()
        {
            SaveCommand = new RelayCommand(SaveCommandExecute, SaveCommandCanExecute);
            CancelCommand = new RelayCommand(CancelCommandExecute);
        }

        #region Dependency Properties

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
            var selectCommand = DBQuery.CreateSqlParameterTitle(EingabeTitle, false);
            var currentTitles = DatabaseHelper.CheckDatabase(selectCommand);
            if(currentTitles.Rows.Count > 0)
            {
                var text = DatabaseHelper.GetNotificationText(4);
                MessageBox.Show(text, "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
            {
                var insertCommand = DBQuery.CreateSqlParameterTitle(EingabeTitle, true);
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
            return EingabeTitle.Length > 0;
        }
        #endregion

        #region Methods

        private void _reset()
        {
            EingabeTitle = string.Empty;
        }

        #endregion
    }
}
