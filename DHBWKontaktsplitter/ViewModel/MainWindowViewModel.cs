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
        private ContactModel _contactModel = new ContactModel();;
        private ExecutionModel execModel;
        #endregion

        public MainWindowViewModel()
        {
            EnterCommand = new RelayCommand(EnterCommandExecute);
            AddNewTitleCommand = new RelayCommand(AddNewTitleCommandExecute);
            EditContactCommand = new RelayCommand(EditContactCommandExecute);
        }

        #region Dependency Properties
        public string Eingabe
        {
            get { return _eingabe; }
            set { _eingabe = value; }
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

        #endregion

        #region Commands
        public ICommand EnterCommand { get; set; }
        public ICommand AddNewTitleCommand { get; set; }
        public ICommand EditContactCommand { get; set; }

        private void EnterCommandExecute(object obj)
        {
            Contact = new ContactModel();
            execModel = new ExecutionModel();
            execModel = _parser.ExecuteInput(Eingabe);

            if (execModel.HasError)
            {
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
                execModel.Contact.TitelList.ForEach(x => execModel.Contact.AllTitles += x.Title + " ");
                execModel.Contact.AllTitles = execModel.Contact.AllTitles?.Trim();
                Contact = execModel.Contact;
            }

            if(execModel.SplittedInput.Count > 0)
            {
                // there are not matched items
                var list = _createZuordnungList(execModel);
                var view = new ManuelleZuordnungView();
                var viewModel = new ManuelleZuordnungViewModel();
                viewModel.InputList = list;
                view.DataContext = viewModel;
                view.Show();
            }

        }

        private void AddNewTitleCommandExecute(object obj)
        {
            new AddNewTitleView().Show();
        }


        private void EditContactCommandExecute(object obj)
        {
            //new ManuelleZuordnungView().Show();
            var list = _createZuordnungList(execModel, true);
            var view = new ManuelleZuordnungView();
            var viewModel = new ManuelleZuordnungViewModel();
            viewModel.InputList = list;
            viewModel.TextBoxIsEnabled = true;
            viewModel.NewTitleButtonVisibility = Visibility.Visible;
            view.DataContext = viewModel;
            view.Show();
        }

        #endregion

        #region Methods
        private List<ZuordnungModel> _createZuordnungList(ExecutionModel execModel, bool changeContact = false)
        {
            var list = new List<ZuordnungModel>();
            if (changeContact)
            {
                list.Add(new ZuordnungModel { LabelText = StaticHelper.Anrede, EntryText = execModel.Contact.AnredeText, SelectedDropDownEntry = StaticHelper.Anrede });
                list.Add(new ZuordnungModel { LabelText = StaticHelper.Briefanrede, EntryText = execModel.Contact.BriefanredeText, SelectedDropDownEntry = StaticHelper.Briefanrede });
                //list.Add(new ZuordnungModel { LabelText = StaticHelper.Titel, EntryText = execModel.Contact.TitelText });
                list.Add(new ZuordnungModel { LabelText = StaticHelper.Geschlecht, EntryText = execModel.Contact.GeschlechtText, SelectedDropDownEntry = StaticHelper.Geschlecht });
                list.Add(new ZuordnungModel { LabelText = StaticHelper.Vorname, EntryText = execModel.Contact.Vorname, SelectedDropDownEntry = StaticHelper.Vorname });
                list.Add(new ZuordnungModel { LabelText = StaticHelper.Nachname, EntryText = execModel.Contact.Nachname, SelectedDropDownEntry = StaticHelper.Nachname });
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

        #endregion
    }
}
