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
    public class Parser
    {
        private ExecutionModel _execModel;
        private InformationBase _informationBase = new InformationBase();

        public ExecutionModel ExecuteInput(string input)
        {
            _execModel = new ExecutionModel();
            input = input.ToLower();
            //Check input length less than 100 characters or empty
            _checkInputLength(input);
            if (_execModel.HasError) return _execModel;

            //Split input at space
            var inputSplitted = input.Split(' ').ToList<string>().FindAll(x => !string.IsNullOrEmpty(x));
            Console.WriteLine(inputSplitted.Count);

            int languId = 0;
            int gId = 0;

            //Check 'Anrede'
            var anredeTuple = _informationBase.GetAnrede(inputSplitted);
            _execModel.Contact.AnredeId = anredeTuple.Item1;
            _execModel.Contact.AnredeText = anredeTuple.Item2;
            _removeMatchedParameterFromList(inputSplitted, _execModel.Contact.AnredeText);
            languId = _informationBase.GetLanguFromAnredeTable();
            gId = _informationBase.GetGeschlechtFromAnredeTable();

            //Get 'Briefanrede'
            var brAnredeTuple = _informationBase.GetBriefanrede(languId, gId);
            _execModel.Contact.BriefanredeId = brAnredeTuple.Item1;
            _execModel.Contact.BriefanredeText = brAnredeTuple.Item2;

            //Get'Geschlecht'
            var geschlechtTuple = _informationBase.GetGeschlecht(gId);
            _execModel.Contact.GeschlechtId = geschlechtTuple.Item1;
            _execModel.Contact.GeschlechtText = geschlechtTuple.Item2;

            //Check 'Title'
            var t = _informationBase.GetAllTitles(inputSplitted);
            _execModel.Contact.TitelList = t.Item1;
            if(t.Item2.Count > 0)
            {
                foreach (var inputEntry in t.Item2)
                {
                    _removeMatchedParameterFromList(inputSplitted, inputEntry);
                }
            }

            //Check 'Vorname'
            _execModel.Contact.Vorname = _getFirstNameFromList(inputSplitted);

            //Check 'Nachname'
            _execModel.Contact.Nachname = _getLastNameFromList(inputSplitted, _execModel.Contact.Vorname);
            _removeMatchedParameterFromList(inputSplitted, _execModel.Contact.Vorname);

            var nachnameSplit = _execModel.Contact.Nachname.Split(' ');
            foreach(var entry in nachnameSplit)
            {
                _removeMatchedParameterFromList(inputSplitted, entry);
            }

            _execModel.SplittedInput = inputSplitted;

            return _execModel;
        }

        private void _checkInputLength(string input)
        {
            if (input.Length > 100)
            {
                _execModel.HasError = true;
                _execModel.ErrorId = 1;
            }
            else if (string.IsNullOrEmpty(input))
            {
                _execModel.HasError = true;
                _execModel.ErrorId = 2;
            }
        }

        private void _removeMatchedParameterFromList(List<string> list, string value)
        {
            if (string.IsNullOrEmpty(value)) return;
            int idx = list.IndexOf(value.Trim());
            if (idx >= 0) list.RemoveAt(idx);
        }

        private string _getFirstNameFromList(List<string> list)
        {
            if (list.Count == 1)
            {
                _execModel.NotificationId = 3; // No 'Vorname'
                return string.Empty; //is lastname
            }
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

        private string _getLastNameFromList(List<string> list, string vname)
        {
            if (list.Count == 1) return list.ElementAt(0);

            //bool nextEntry = false;
            var lastName = string.Empty;
            foreach(var entry in list)
            {
                if (entry.Contains(",")) return entry;
                //else if (string.Compare(entry.Last().ToString(), ".") == 0) nextEntry = true;
                //else if (nextEntry) lastName += entry + " ";
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
