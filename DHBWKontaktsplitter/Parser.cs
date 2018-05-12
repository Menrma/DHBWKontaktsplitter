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
        private ExecutionModel _execModel = new ExecutionModel();

        public ExecutionModel ExecuteInput(string input)
        {
            input = input.ToLower();
            //Check input length less than 100 characters or empty
            _checkInputLength(input);
            if (_execModel.HasError) return _execModel;

            //Split input at space
            var inputSplitted = input.Split(' ').ToList<string>();

            //Check 'Anrede'
            //Create SQLiteParameter for 'Anrede'
            var anredeParameter = _createSqlParameterAnrede(inputSplitted);
            var anredeTable = DatabaseHelper.CheckDatabase(anredeParameter);
            int languId = 0;

            if (anredeTable.Rows.Count > 0)
            {
                _execModel.Contact.AnredeText = DatabaseHelper.GetFirstFromDatabaseResult(anredeTable, "ANREDE");
                _removeMatchedParameterFromList(inputSplitted, _execModel.Contact.AnredeText);
                languId = int.Parse(DatabaseHelper.GetFirstFromDatabaseResult(anredeTable, "SPRACHE_ID"));
                int gId = int.Parse(DatabaseHelper.GetFirstFromDatabaseResult(anredeTable, "GESCHLECHT_ID"));

                if (!string.IsNullOrEmpty(_execModel.Contact.AnredeText) && languId != 0 && gId != 0)
                {
                    //Get 'Briefanrede'
                    var briefAnredeParameter = _createSqlParameterBriefanrede(languId, gId);
                    var bAnredeTable = DatabaseHelper.CheckDatabase(briefAnredeParameter);
                    _execModel.Contact.BriefanredeText = DatabaseHelper.GetFirstFromDatabaseResult(bAnredeTable, "WERT");

                    //Get'Geschlecht'
                    var geschlechtParameter = _createSqlParameteGeschlecht(gId);
                    var geschlechtTable = DatabaseHelper.CheckDatabase(geschlechtParameter);
                    _execModel.Contact.GeschlechtText = DatabaseHelper.GetFirstFromDatabaseResult(geschlechtTable, "WERT");
                }
            }
            else
            {

            }

            //Check 'Title'
            //TODO => Zweiten Paramter ändern
            if (inputSplitted.Count > 0)
            {
                var tmpList = new List<string>(inputSplitted);

                foreach(var inputEntry in tmpList)
                {
                    var titlesParameter = _createSqlParameterTitle(languId, inputEntry);
                    var titleTable = DatabaseHelper.CheckDatabase(titlesParameter);
                    if (titleTable.Rows.Count > 0)
                    {
                        var row = titleTable.Rows[0];
                        _execModel.Contact.TitelList.Add(new TitleModel
                        {
                            Title_ID = int.Parse(row[0].ToString()),
                            Sprache_ID = int.Parse(row[1].ToString()),
                            Title = row[2].ToString()
                        });
                        _removeMatchedParameterFromList(inputSplitted, row[2].ToString());
                    }
                }
            }

            //Check 'Vorname'
            _execModel.Contact.Vorname = _getFirstNameFromList(inputSplitted);
            _removeMatchedParameterFromList(inputSplitted, _execModel.Contact.Vorname);

            //Check 'Nachname'
            _execModel.Contact.Nachname = _getLastNameFromList(inputSplitted);
            _removeMatchedParameterFromList(inputSplitted, _execModel.Contact.Nachname);

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

        private string _getLastNameFromList(List<string> list)
        {
            if (list.Count == 1) return list.ElementAt(0);

            bool nextEntry = false;
            var lastName = string.Empty;
            foreach(var entry in list)
            {
                if (entry.Contains(",")) return entry;
                else if (string.Compare(entry.Last().ToString(), ".") == 0) nextEntry = true;
                else if (nextEntry) lastName += entry + " ";
            }
            return lastName;
        }

        private SQLiteCommand _createSqlParameterAnrede(List<string> inputList)
        {
            string sqlExtension = string.Empty;
            SQLiteCommand cmd = new SQLiteCommand();

            sqlExtension = "IN ({0})";
            var paramList = new List<string>();
            //Check splitted input for 'Anrede'
            int indexer = 0;
            foreach(var singleInput in inputList)
            {
                var param = "@param" + indexer.ToString();
                paramList.Add(param);
                cmd.Parameters.AddWithValue(param, singleInput);
                indexer++;
            }
            sqlExtension = string.Format(sqlExtension, string.Join(",", paramList));
            cmd.CommandText = string.Format(StaticHelper.CheckAnrede, sqlExtension);
            return cmd;
        }

        private SQLiteCommand _createSqlParameterBriefanrede(int spracheId, int geschlechtId)
        {
            SQLiteCommand cmd = new SQLiteCommand();
            cmd.CommandText = StaticHelper.CheckBriefanrede;
            cmd.Parameters.AddWithValue("@spracheId", spracheId);
            cmd.Parameters.AddWithValue("@geschlechtID", geschlechtId);
            return cmd;
        }

        private SQLiteCommand _createSqlParameteGeschlecht(int geschlechtId)
        {
            SQLiteCommand cmd = new SQLiteCommand();
            cmd.CommandText = StaticHelper.CheckGeschlecht;
            cmd.Parameters.AddWithValue("@gId", geschlechtId);
            return cmd;
        }

        private SQLiteCommand _createSqlParameterTitle(int languId, string title)
        {
            SQLiteCommand cmd = new SQLiteCommand();
            if (languId == 0) languId = 1; // DE is default

            cmd.Parameters.AddWithValue("@spracheId", languId);
            cmd.Parameters.AddWithValue("@title", "%" + title + "%");
            cmd.CommandText = StaticHelper.GetTitel;
            return cmd;
        }
    }
}
