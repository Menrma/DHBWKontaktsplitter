using DHBWKontaktsplitter.Framework;
using DHBWKontaktsplitter.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DHBWKontaktsplitter
{
    /// <summary>
    /// Parser Klasse
    /// </summary>
    public class Parser
    {
        private ExecutionModel _execModel;
        private InformationBase _informationBase = new InformationBase();

        /// <summary>
        /// Methode welche die Eingabe verarbeitet
        /// </summary>
        /// <param name="input">Benutzer-Eingabe</param>
        /// <returns></returns>
        public ExecutionModel ExecuteInput(string input)
        {
            _execModel = new ExecutionModel();
            //Input alles klein
            input = input.ToLower();
            //Überprüfen ob der Input leer oder größer als 100 Zeichen lang ist
            _checkInputLength(input);
            //Bei Fehler Verarbeitung stoppen
            if (_execModel.HasError) return _execModel;

            //Eingabe an jedem Leerzeichen splitten
            var inputSplitted = input.Split(' ').ToList<string>().FindAll(x => !string.IsNullOrEmpty(x));

            int languId = 0;
            int gId = 0;

            //'Anrede' überprüfen
            //Überprüfen ob díe Eingabe eine Anrede enthält
            var anredeTuple = _informationBase.GetAnrede(inputSplitted);
            //Text + Id der Anrede, falls vorhanden, speichern
            _execModel.Contact.AnredeId = anredeTuple.Item1;
            _execModel.Contact.AnredeText = anredeTuple.Item2;
            //Erkannte Anrede aus der gesplitteten Liste entfernen
            _removeMatchedParameterFromList(inputSplitted, _execModel.Contact.AnredeText);
            //Versuchen über die Anrede die Sprache zu ermitteln
            languId = _informationBase.GetLanguFromAnredeTable();
            //Versuchen über die Anrede das Geschlecht zu ermitteln
            gId = _informationBase.GetGeschlechtFromAnredeTable();

            //'Briefanrede' ermitteln
            //Anhand der Sprache- und Geschlecht-Id die Briefanrede ermitteln
            var brAnredeTuple = _informationBase.GetBriefanrede(languId, gId);
            //Text + Id, falls vorhanden, speichern
            _execModel.Contact.BriefanredeId = brAnredeTuple.Item1;
            _execModel.Contact.BriefanredeText = brAnredeTuple.Item2;

            //'Geschlecht' ermitteln
            //Text + Id des anhand der Geschlecht-Id ermitteln und speichern
            var geschlechtTuple = _informationBase.GetGeschlecht(gId);
            _execModel.Contact.GeschlechtId = geschlechtTuple.Item1;
            _execModel.Contact.GeschlechtText = geschlechtTuple.Item2;

            //'Titel' überprüfen
            //Überprüfen ob sich im verbliebenen Input ein Titel befindet
            var t = _informationBase.GetAllTitles(inputSplitted);
            //Ermittelte Titel, falls vorhanden, speichern
            _execModel.Contact.TitelList = t.Item1;
            //In Item2 stehen die erkannten Titel
            if(t.Item2.Count > 0)
            {
                //Erkannte Titel aus der gesplitteten Eingabe entfernen
                foreach (var inputEntry in t.Item2)
                {
                    _removeMatchedParameterFromList(inputSplitted, inputEntry);
                }
            }

            //'Vorname' überprüfen
            _execModel.Contact.Vorname = _getFirstNameFromList(inputSplitted);

            //'Nachname' überprüfen
            _execModel.Contact.Nachname = _getLastNameFromList(inputSplitted, _execModel.Contact.Vorname);
            //Vorname aus der Eingabe entfernen
            _removeMatchedParameterFromList(inputSplitted, _execModel.Contact.Vorname);

            //Überprüfen ob der Nachname aus mehreren Teilen besteht
            var nachnameSplit = _execModel.Contact.Nachname.Split(' ');
            //Jeden einzelnen Teil aus der Eingabe entfernen
            foreach(var entry in nachnameSplit)
            {
                _removeMatchedParameterFromList(inputSplitted, entry);
            }

            //Verbliebene Bestandteile der Eingabe in das Rückgabe-Model speichern
            _execModel.SplittedInput = inputSplitted;

            return _execModel;
        }

        /// <summary>
        /// Methode zu Überpfüng der Länge des Inputs
        /// </summary>
        /// <param name="input">Benutzer-Eingabe</param>
        private void _checkInputLength(string input)
        {
            //Fehler wenn Eingabe > 100 Zeichen
            if (input.Length > 100)
            {
                //Parameter setzen und passende ErrorId zurückgeben
                _execModel.HasError = true;
                _execModel.ErrorId = 1;
            }
            //Fehler wenn Eingabe leer
            else if (string.IsNullOrEmpty(input))
            {
                //Parameter setzen und passende ErrorId zurückgeben
                _execModel.HasError = true;
                _execModel.ErrorId = 2;
            }
        }

        /// <summary>
        /// Methode für das Entfernen einer String-Value aus der gesplitteten Input-Liste
        /// </summary>
        /// <param name="list">Benutzer-Eingabe als gesplittete Liste</param>
        /// <param name="value">Zu entfernendes String-Value</param>
        private void _removeMatchedParameterFromList(List<string> list, string value)
        {
            if (string.IsNullOrEmpty(value)) return;
            int idx = list.IndexOf(value.Trim());
            if (idx >= 0) list.RemoveAt(idx);
        }

        /// <summary>
        /// Methode für das Extrahieren des Vornamnes aus der Input-Liste
        /// </summary>
        /// <param name="list">Liste mit der gesplitteten Eingabe</param>
        /// <returns>Extrahierter Nachname als String</returns>
        private string _getFirstNameFromList(List<string> list)
        {
            //Überprüfen ob die Liste lediglich einen Wert enthält
            if (list.Count == 1)
            {
                //Id der Error-Meldung zurückgegeben
                _execModel.NotificationId = 3; // No 'Vorname'
                return string.Empty; //is lastname
            }

            //Erkennen des Vornamens anhand definierter Regeln
            bool nextEntry = false;
            foreach (var entry in list)
            {
                if (entry.Contains(",")) nextEntry = true;
                else if (nextEntry) return entry;
                else if (string.Compare(entry.Last().ToString(), ".") != 0) return entry;
            }
            _execModel.NotificationId = 3; // No 'Vorname'
            return string.Empty;
        }

        /// <summary>
        /// Methode für das Extrahieren des Nachnamens aus der Input-Liste
        /// </summary>
        /// <param name="list">Liste mit der gesplitteten Eingabe</param>
        /// <param name="vname">Erkannter Vorname als String</param>
        /// <returns>Extrahierter Nachname als String</returns>
        private string _getLastNameFromList(List<string> list, string vname)
        {
            //Befindet sich lediglich ein Wert in der Liste,
            //handelt es sich per Definition um den Nachnamen
            if (list.Count == 1) return list.ElementAt(0);

            //Nachname anhand definierter Regeln erkennen
            var lastName = string.Empty;
            foreach(var entry in list)
            {
                if (entry.Contains(",")) return entry;
            }

            if (string.IsNullOrEmpty(vname) && string.IsNullOrEmpty(lastName)) return string.Empty;

            int vnameIdx = list.IndexOf(vname);
            if(vnameIdx >= 0)
            {
                var res = list.GetRange(vnameIdx + 1, list.Count - 1 - vnameIdx);
                if (res.Count == 0) return string.Empty;
                lastName = string.Join(" ", res);
            } 

            return lastName;
        }
    }
}
