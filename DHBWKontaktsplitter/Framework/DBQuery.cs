using DHBWKontaktsplitter.Model;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DHBWKontaktsplitter.Framework
{
    public static class DBQuery
    {
        public static SQLiteCommand CreateSqlParameterAnrede(List<string> inputList)
        {
            string sqlExtension = string.Empty;
            SQLiteCommand cmd = new SQLiteCommand();

            sqlExtension = "IN ({0})";
            var paramList = new List<string>();
            //Check splitted input for 'Anrede'
            int indexer = 0;
            foreach (var singleInput in inputList)
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

        public static SQLiteCommand CreateSqlParameterBriefanrede(int spracheId, int geschlechtId)
        {
            SQLiteCommand cmd = new SQLiteCommand();
            cmd.CommandText = StaticHelper.CheckBriefanrede;
            cmd.Parameters.AddWithValue("@spracheId", spracheId);
            cmd.Parameters.AddWithValue("@geschlechtID", geschlechtId);
            return cmd;
        }

        public static SQLiteCommand CreateSqlParameteGeschlecht(int geschlechtId)
        {
            SQLiteCommand cmd = new SQLiteCommand();
            cmd.CommandText = StaticHelper.CheckGeschlecht;
            cmd.Parameters.AddWithValue("@gId", geschlechtId);
            return cmd;
        }

        public static SQLiteCommand CreateSqlParameterSearchTitle(string title)
        {
            SQLiteCommand cmd = new SQLiteCommand();

            var titleSplitted = title.Split('-');
            if(titleSplitted.Length == 0)
            {
                cmd.Parameters.AddWithValue("@title", Formatter.FormatNewTitle(title));
            }
            else
            {
                var concatTitle = "";
                foreach(var singleTitle in titleSplitted)
                {
                    concatTitle += "%" + singleTitle + "%";
                }
                cmd.Parameters.AddWithValue("@title", concatTitle);
            }
            
            cmd.CommandText = StaticHelper.GetTitel;
            return cmd;
        }

        public static SQLiteCommand CreateSqlParameterTitle(string text, bool isInsert)
        {
            SQLiteCommand cmd = new SQLiteCommand();
            if (isInsert)
                cmd.CommandText = StaticHelper.InsertTitel;
            else
                cmd.CommandText = StaticHelper.GetTitel;

            cmd.Parameters.AddWithValue("@title", text);
            return cmd;
        }

        public static SQLiteCommand CreateSqlParameterSaveContact(ContactModel contact)
        {
            SQLiteCommand cmd = new SQLiteCommand();

            cmd.CommandText = StaticHelper.InserContact;
 
            cmd.Parameters.AddWithValue("@anredeId", contact.AnredeId);
            cmd.Parameters.AddWithValue("@brAnredeId", contact.BriefanredeId);
            cmd.Parameters.AddWithValue("@gId", contact.GeschlechtId);
            cmd.Parameters.AddWithValue("@vname", contact.Vorname);
            cmd.Parameters.AddWithValue("@nname", contact.Nachname);

            return cmd;
        }

        public static SQLiteCommand CreateSqlParameterLastContact()
        {
            SQLiteCommand cmd = new SQLiteCommand();
            cmd.CommandText = StaticHelper.GetLastContact;
            return cmd;
        }

        public static SQLiteCommand CreateSqlParameterSaveTitle(int contactId, int titleId)
        {
            SQLiteCommand cmd = new SQLiteCommand();

            cmd.CommandText = StaticHelper.InsertTitelKontakt;

            cmd.Parameters.AddWithValue("@contactId",contactId);
            cmd.Parameters.AddWithValue("@titelId", titleId);

            return cmd;
        }
    }
}
