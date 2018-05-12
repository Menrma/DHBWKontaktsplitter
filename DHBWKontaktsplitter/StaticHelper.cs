using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DHBWKontaktsplitter
{
    public static class StaticHelper
    {
        public static readonly string ConnectionString = "Data Source = Database/Kontaktsplitter.db; Version = 3;";
        public static readonly string CheckAnrede = "SELECT * FROM ANREDE WHERE ANREDE {0}";
        public static readonly string CheckBriefanrede = "SELECT * FROM BRIEFANREDE WHERE SPRACHE_ID = @spracheId AND GESCHLECHT_ID = @geschlechtID";
        public static readonly string CheckGeschlecht = "SELECT * FROM GESCHLECHT WHERE ID = @gId";
        public static readonly string GetNotification = "SELECT * FROM BENACHRICHTIGUNG WHERE ID = @id";
        public static readonly string InsertTitel = "INSERT INTO TITEL (SPRACHE_ID, TITEL) VALUES (@spracheID, @title)";
        public static readonly string GetTitel = "SELECT * FROM TITEL WHERE SPRACHE_ID = @spracheID AND TITEL LIKE @title";
        public static readonly string GetAllLang = "SELECT * FROM Sprachen";
        public static readonly string Anrede = "Anrede";
        public static readonly string Briefanrede = "Briefanrede";
        public static readonly string Titel = "Titel";
        public static readonly string Geschlecht = "Geschlecht";
        public static readonly string Vorname = "Vorname";
        public static readonly string Nachname = "Nachname";
        public static readonly List<string> BestandteileList = new List<string> { Anrede, Briefanrede, Titel, Geschlecht, Vorname, Nachname };
    }
}