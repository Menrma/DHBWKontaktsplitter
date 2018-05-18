using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DHBWKontaktsplitter.Framework
{
    /// <summary>
    /// Klasse für die Kommunikation mit der Datenbank
    /// </summary>
    public static class DatabaseHelper
    {
        /// <summary>
        /// Methode welche SELECT-SQL-Anweisungen an die Datenbank sendet
        /// </summary>
        /// <param name="cmd">SELECT-SQLiteCommand</param>
        /// <returns></returns>
        public static DataTable CheckDatabase(SQLiteCommand cmd)
        {
            var result = new DataTable();
            //Verbindung zur Datenbank öffnen
            using (var dbConnection = new SQLiteConnection(StaticHelper.ConnectionString))
            {
                dbConnection.Open();
                cmd.Connection = dbConnection;

                using (var reader = cmd.ExecuteReader())
                {
                    //Rückgabe der Anfrage in eine DataTable laden
                    result.Load(reader);
                }
                dbConnection.Close();
                return result;
            }
        }

        /// <summary>
        /// Methode welche INSERT-SQL-Anweisungen an die Datenbank sendet
        /// </summary>
        /// <param name="cmd">INSERT-SQListeCommand</param>
        /// <returns></returns>
        public static int InsertDatabase(SQLiteCommand cmd)
        {
            //Verbindung zur Datenbank öffnen
            using (var dbConnection = new SQLiteConnection(StaticHelper.ConnectionString))
            {
                dbConnection.Open();
                cmd.Connection = dbConnection;

                int resCount = cmd.ExecuteNonQuery();
                dbConnection.Close();
                //Anzahl der tatsächlich geschriebenen Datensätze zurückgegeben
                return resCount;
            }
        }

        /// <summary>
        /// Methode für die Ermittlung des ersten Resultats aus der Datenbank
        /// </summary>
        /// <param name="table">Zu überprüfende DataTable</param>
        /// <param name="columnName">Name der zu überprüfenden Spalte</param>
        /// <returns></returns>
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

        /// <summary>
        /// Methode für das Erstellen eines SQLiteCommands für das Ermitteln eines Fehlertextes
        /// </summary>
        /// <param name="errorId">Id des Errors</param>
        /// <returns></returns>
        private static SQLiteCommand _createSqlParameteError(int errorId)
        {
            SQLiteCommand cmd = new SQLiteCommand();
            cmd.CommandText = StaticHelper.GetNotification;
            cmd.Parameters.AddWithValue("@id", errorId);
            return cmd;
        }

        /// <summary>
        /// Methode für das Erstellen eines SQLiteCommands für das Ermitteln des Textes einer Benachrichtigung
        /// </summary>
        /// <param name="notificationId">Id der Benachrichtigung</param>
        /// <returns></returns>
        public static string GetNotificationText(int notificationId)
        {
            var errorCommand = _createSqlParameteError(notificationId);
            var errorTable = CheckDatabase(errorCommand);
            return GetFirstFromDatabaseResult(errorTable, "TEXT");
        }
    }
}
