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
    /// <summary>
    /// ViewModel der AddTitleView
    /// </summary>
    public class AddNewTitleViewModel : ViewModelBase
    {
        #region Properties
        private string _eingabeTitle = string.Empty;
        #endregion

        public AddNewTitleViewModel()
        {
            //Commands der Oberfläche registrieren
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

        /// <summary>
        /// Methode die beim Drücken des 'Abbrechen'-Buttons ausgeführt wird
        /// </summary>
        /// <param name="obj">Fenster-Objekt</param>
        private void CancelCommandExecute(object obj)
        {
            //Fenster schließen
            ((Window)obj).Close();
        }

        /// <summary>
        /// Methode die biem Drücken des 'Speichern'-Buttons ausgeführt wird
        /// </summary>
        /// <param name="obj"></param>
        private void SaveCommandExecute(object obj)
        {
            //Eingegeben Titel formatieren
            EingabeTitle = Formatter.FormatNewTitle(EingabeTitle);
            //SQL-Command für das Ermitteln von Titeln erstellen
            var selectCommand = DBQuery.CreateSqlParameterTitle(EingabeTitle, false);
            //Titel aus der Datenbank abrufen
            var currentTitles = DatabaseHelper.CheckDatabase(selectCommand);
            if(currentTitles.Rows.Count > 0)
            {
                //Titel existiert bereits in der Datenbank
                //Fehler-Text aus der Datenbank lesen
                var text = DatabaseHelper.GetNotificationText(4);
                //Fehler als MessageBox auf der Benutzeroberfläche anzeigen
                MessageBox.Show(text, "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
            {
                //Titel ist in der Datenbank noch nicht vorhanden
                //SQL-Command für das Einfügen des Titels erstellen
                var insertCommand = DBQuery.CreateSqlParameterTitle(EingabeTitle, true);
                //SQL-Command ausführen
                int resCount = DatabaseHelper.InsertDatabase(insertCommand);

                //Anzahl der geschriebenen Datensätze prüfen
                if (resCount == 1)
                {
                    //Genau ein Datensatz wurde geschrieben
                    //Erfolgsmeldung auf der Benutzeroberfläche anzeigen
                    var text = DatabaseHelper.GetNotificationText(5);
                    MessageBox.Show(text, "Gespeichert", MessageBoxButton.OK, MessageBoxImage.Information);
                    //Eingabe zurücksetzen
                    _reset();
                }
                else
                {
                    //Datensatz konnte nicht in die Datenbank geschrieben
                    //Fehler auf der Benutzeroberlfäche anzeigen
                    var text = DatabaseHelper.GetNotificationText(6);
                    MessageBox.Show(text, "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        /// <summary>
        /// Methode die überprüft ob der 'Speichern'-Button gedrückt werden kann
        /// </summary>
        /// <param name="obj"></param>
        /// <returns>True, wenn Länge der Eingabe größer als 0, false sonst</returns>
        private bool SaveCommandCanExecute(object obj)
        {
            return EingabeTitle.Length > 0;
        }
        #endregion

        #region Methods

        /// <summary>
        /// Methode zum Zurücksetzen der Eingabe in einen initialen Zustand
        /// </summary>
        private void _reset()
        {
            EingabeTitle = string.Empty;
        }

        #endregion
    }
}
