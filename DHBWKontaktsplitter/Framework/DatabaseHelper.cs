using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DHBWKontaktsplitter.Framework
{
    public static class DatabaseHelper
    {
        public static DataTable CheckDatabase(SQLiteCommand cmd)
        {
            var result = new DataTable();
            //Check database for relevant informations
            using (var dbConnection = new SQLiteConnection(StaticHelper.ConnectionString))
            {
                dbConnection.Open();
                cmd.Connection = dbConnection;

                using (var reader = cmd.ExecuteReader())
                {
                    result.Load(reader);
                }
                dbConnection.Close();
                return result;
            }
        }

        public static int InsertDatabase(SQLiteCommand cmd)
        {
            // Insert in Database
            using (var dbConnection = new SQLiteConnection(StaticHelper.ConnectionString))
            {
                dbConnection.Open();
                cmd.Connection = dbConnection;

                int resCount = cmd.ExecuteNonQuery();
                dbConnection.Close();
                return resCount;
            }
        }

        public static string GetFirstFromDatabaseResult(DataTable table, string columnName)
        {
            if (table.Rows.Count > 0)
            {
                return table.Rows[0][columnName].ToString();
            }
            else
            {
                return string.Empty;
            }
        }

        private static SQLiteCommand _createSqlParameteError(int errorId)
        {
            SQLiteCommand cmd = new SQLiteCommand();
            cmd.CommandText = StaticHelper.GetNotification;
            cmd.Parameters.AddWithValue("@id", errorId);
            return cmd;
        }

        public static string GetNotificationText(int notificationId)
        {
            var errorCommand = _createSqlParameteError(notificationId);
            var errorTable = CheckDatabase(errorCommand);
            return GetFirstFromDatabaseResult(errorTable, "TEXT");
        }
    }
}
