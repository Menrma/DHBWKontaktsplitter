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
    public class InformationBase
    {
        private DataTable _anredeTable;

        public Tuple<int, string> GetAnrede(List<string> input)
        {
            var anredeParameter = DBQuery.CreateSqlParameterAnrede(input);
            var anredeTable = DatabaseHelper.CheckDatabase(anredeParameter);

            if (anredeTable.Rows.Count == 0) return new Tuple<int, string>(0, string.Empty);

            _anredeTable = anredeTable;

            int anredeId = 0;
            int.TryParse(DatabaseHelper.GetFirstFromDatabaseResult(anredeTable, "ID"), out anredeId);
            var anredeText = DatabaseHelper.GetFirstFromDatabaseResult(anredeTable, "ANREDE");

            return new Tuple<int, string>(anredeId, anredeText);
        }

        public int GetAnredeIdFromAnredeTable()
        {
            if (_anredeTable == null) return 0;

            int anredeId = 0;
            int.TryParse(DatabaseHelper.GetFirstFromDatabaseResult(_anredeTable, "ID"), out anredeId);
            return anredeId;
        }

        public int GetLanguFromAnredeTable()
        {
            if (_anredeTable == null) return 1;

            int languId = 0;
            int.TryParse(DatabaseHelper.GetFirstFromDatabaseResult(_anredeTable, "SPRACHE_ID"), out languId);

            return languId;
        }

        public int GetGeschlechtFromAnredeTable()
        {
            if (_anredeTable == null) return 4;

            int gId = 0;
            int.TryParse(DatabaseHelper.GetFirstFromDatabaseResult(_anredeTable, "GESCHLECHT_ID"), out gId);

            return gId;
        }

        public Tuple<int, string> GetBriefanrede(int languId, int gId)
        {
            var briefAnredeParameter = DBQuery.CreateSqlParameterBriefanrede(languId, gId);
            var bAnredeTable = DatabaseHelper.CheckDatabase(briefAnredeParameter);

            if (bAnredeTable.Rows.Count == 0) return new Tuple<int, string>(0, string.Empty);

            int brAnredeId = 0;
            int.TryParse(DatabaseHelper.GetFirstFromDatabaseResult(bAnredeTable, "ID"), out brAnredeId);
            var anredeText = DatabaseHelper.GetFirstFromDatabaseResult(bAnredeTable, "Wert");

            return new Tuple<int, string>(brAnredeId, anredeText);
        }

        public Tuple<int, string> GetGeschlecht(int gId)
        {
            var geschlechtParameter = DBQuery.CreateSqlParameteGeschlecht(gId);
            var geschlechtTable = DatabaseHelper.CheckDatabase(geschlechtParameter);

            int gIdDB = 0;
            int.TryParse(DatabaseHelper.GetFirstFromDatabaseResult(geschlechtTable, "ID"), out gIdDB);
            var geschlechtText = DatabaseHelper.GetFirstFromDatabaseResult(geschlechtTable, "Wert");
            return new Tuple<int, string>(gIdDB, geschlechtText);
        }

        public Tuple<List<TitleModel>, List<string>> GetAllTitles(List<string> input)
        {
            if (input.Count == 0) return null;

            var tmpList = new List<string>(input);
            var returnList = new List<TitleModel>();
            var deleteList = new List<string>();

            foreach (var inputEntry in tmpList)
            {
                var titlesParameter = DBQuery.CreateSqlParameterSearchTitle(inputEntry);
                var titleTable = DatabaseHelper.CheckDatabase(titlesParameter);
                if (titleTable.Rows.Count > 0)
                {
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
