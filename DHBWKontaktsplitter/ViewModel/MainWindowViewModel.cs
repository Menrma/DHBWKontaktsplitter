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
                execModel.Contact.TitelList.ForEach(x => execModel.Contact.AllTitles += x.Title + " ");
                execModel.Contact.AllTitles = execModel.Contact.AllTitles?.Trim();
                EditButtonIsEnabled = true;
            }

            if(execModel.SplittedInput.Count > 0)
            {
                // there are not matched items
                var editList = _openManuelleZuordnung(false, Visibility.Hidden);
                _mapZurodnungToModel(editList);
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
            throw new NotImplementedException();
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

        #endregion
    }
}
