using DHBWKontaktsplitter.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace DHBWKontaktsplitter.ViewModel
{
    public class MainWindowViewModel
    {
        #region Properties
        private string _eingabe = String.Empty;
        #endregion

        public MainWindowViewModel()
        {
            EnterCommand = new RelayCommand(EnterCommandExecute);
        }

        #region Dependency Properties
        public string Eingabe
        {
            get { return _eingabe; }
            set { _eingabe = value; }
        }
        #endregion

        #region Commands
        public ICommand EnterCommand { get; set; }

        private void EnterCommandExecute(object obj)
        {

        }

        #endregion
    }
}
