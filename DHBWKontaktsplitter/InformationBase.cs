using DHBWKontaktsplitter.Framework;
using DHBWKontaktsplitter.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DHBWKontaktsplitter
{
    /// <summary>
    /// Klasse für das Ermitteln von Daten aus der Datenbank
    /// </summary>
    public class InformationBase
    {
        private DataTable _anredeTable;

        /// <summary>
        /// Methode die prüft ob ein Bestandteil der gesplitteten Benutzer-Eingabe eine Anrede ist
        /// </summary>
        /// <param name="input">Benutzer-Eingabe als gesplittete Liste</param>
        /// <returns>Tuple bestehend aus int (Id) und string (Anrede als Text)</returns>
        public Tuple<int, string> GetAnrede(List<string> input)
        {
            //SQL-Parameter vorbereiten und Datenbank abfragen
            var anredeParameter = DBQuery.CreateSqlParameterAnrede(input);
            var anredeTable = DatabaseHelper.CheckDatabase(anredeParameter);
            _anredeTable = anredeTable;

            //Ein leeres Tuple zurückgegeben
            if (anredeTable.Rows.Count == 0) return new Tuple<int, string>(0, string.Empty);

            //Die ermittelte Anrede-Id und die Anrede als Text aus der Rückgabe der Datenbank ermitteln
            int anredeId = 0;
            int.TryParse(DatabaseHelper.GetFirstFromDatabaseResult(anredeTable, "ID"), out anredeId);
            var anredeText = DatabaseHelper.GetFirstFromDatabaseResult(anredeTable, "ANREDE");

            return new Tuple<int, string>(anredeId, anredeText);
        }

        /// <summary>
        /// Methode für das Ermitteln der Anrede-Id
        /// </summary>
        /// <returns>Id der zuletzt ermittelten Anrede</returns>
        public int GetAnredeIdFromAnredeTable()
        {
            if (_anredeTable == null) return 0;

            int anredeId = 0;
            int.TryParse(DatabaseHelper.GetFirstFromDatabaseResult(_anredeTable, "ID"), out anredeId);
            return anredeId;
        }

        /// <summary>
        /// Methode für die Ermittlung der Sprach-Id der letzten Anrede
        /// </summary>
        /// <returns>Sprach-Id</returns>
        public int GetLanguFromAnredeTable()
        {
            //Wenn bisher keine Anrede erkannt wurde, dann Default-Wert 1 (DE) nehmen
            if (_anredeTable == null || _anredeTable.Rows.Count == 0) return 1;

            //Der Anrede zugeordnete Sprache nehmen
            int languId = 0;
            int.TryParse(DatabaseHelper.GetFirstFromDatabaseResult(_anredeTable, "SPRACHE_ID"), out languId);

            //Sprache-Id zurückgegeben
            return languId;
        }

        /// <summary>
        /// Methode für die Ermittluing der Geschlecht-Id der letzten Anrede
        /// </summary>
        /// <returns>Geschlecht-Id</returns>
        public int GetGeschlechtFromAnredeTable()
        {
            //Wenn bisher keine Anrede erkannt wurde, dann Default-Wert 4 (keine Angabe) nehmen
            if (_anredeTable == null || _anredeTable.Rows.Count == 0) return 4;

            //Der Anrede zugeordnetes Geschlecht nehmen
            int gId = 0;
            int.TryParse(DatabaseHelper.GetFirstFromDatabaseResult(_anredeTable, "GESCHLECHT_ID"), out gId);

            //Geschlecht-Id zurückgeben
            return gId;
        }

        /// <summary>
        /// Methode für die Ermittlung der Briefanrede
        /// </summary>
        /// <param name="languId">Sprach-Id</param>
        /// <param name="gId">Geschlecht-Id</param>
        /// <returns></returns>
        public Tuple<int, string> GetBriefanrede(int languId, int gId)
        {
            //Default-Werte setzen falls noch nicht geschehene
            if (languId == 0) languId = 1;
            if (gId == 0) gId = 4;

            //Ermittlung und Speicherung der Briefanrede aus der Datenbank
            var briefAnredeParameter = DBQuery.CreateSqlParameterBriefanrede(languId, gId);
            var bAnredeTable = DatabaseHelper.CheckDatabase(briefAnredeParameter);

            if (bAnredeTable.Rows.Count == 0) return new Tuple<int, string>(0, string.Empty);

            //Ermittelte Werte in ein Tuple speichern und zurückgeben
            int brAnredeId = 0;
            int.TryParse(DatabaseHelper.GetFirstFromDatabaseResult(bAnredeTable, "ID"), out brAnredeId);
            var anredeText = DatabaseHelper.GetFirstFromDatabaseResult(bAnredeTable, "Wert");

            return new Tuple<int, string>(brAnredeId, anredeText);
        }

        /// <summary>
        /// Methode für die Ermittlung eines Geschlecht-Texts zu einer Geschlecht-Id
        /// </summary>
        /// <param name="gId">Geschlecht-Id</param>
        /// <returns></returns>
        public Tuple<int, string> GetGeschlecht(int gId)
        {
            //Ermittlung und Speicherung des Geschlechts aus der Datenbank
            var geschlechtParameter = DBQuery.CreateSqlParameteGeschlecht(gId);
            var geschlechtTable = DatabaseHelper.CheckDatabase(geschlechtParameter);

            //Ermittelte Werte in ein Tuple speichern und zurückgegeben
            int gIdDB = 0;
            int.TryParse(DatabaseHelper.GetFirstFromDatabaseResult(geschlechtTable, "ID"), out gIdDB);
            var geschlechtText = DatabaseHelper.GetFirstFromDatabaseResult(geschlechtTable, "Wert");
            return new Tuple<int, string>(gIdDB, geschlechtText);
        }

        /// <summary>
        /// Methode für die Ermittlung der Titel aus der Eingabe
        /// </summary>
        /// <param name="input">Eingabe als gesplittete Liste</param>
        /// <returns></returns>
        public Tuple<List<TitleModel>, List<string>> GetAllTitles(List<string> input)
        {
            if (input.Count == 0) return null;

            var tmpList = new List<string>(input);
            var returnList = new List<TitleModel>();
            var deleteList = new List<string>();

            //Über jeden Eintrag in der Liste loopen und überprüfen, ob es sich um einen Titel handelt
            foreach (var inputEntry in tmpList)
            {
                var titlesParameter = DBQuery.CreateSqlParameterSearchTitle(inputEntry);
                var titleTable = DatabaseHelper.CheckDatabase(titlesParameter);
                //Überprüfen ob aktueller Eintrag ein Titel ist
                if (titleTable.Rows.Count > 0)
                {
                    //Titel wurde erkannt ==> Speichern
                    var row = titleTable.Rows[0];
                    returnList.Add(new TitleModel
                    {
                        Title_ID = int.Parse(row[0].ToString()),
                        Title = row[1].ToString()
                    });
                    deleteList.Add(inputEntry);
                }
            }

            return new Tuple<List<TitleModel>, List<string>>(returnList, deleteList); ;
        }
    }
}
