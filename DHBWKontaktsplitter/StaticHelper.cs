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
        public static readonly string CheckAnrede = "SELECT * FROM ANREDE WHERE Anrede {0}";
        public static readonly string CheckBriefanrede = "SELECT * FROM BRIEFANREDE WHERE SPRACHE_ID = @spracheId AND GESCHLECHT_ID = @geschlechtID";
        public static readonly string CheckGeschlecht = "SELECT * FROM GESCHLECHT WHERE ID = @gId";
        public static readonly string GetNotification = "SELECT * FROM BENACHRICHTIGUNG WHERE ID = @id";
        public static readonly string InsertTitel = "INSERT INTO TITEL (TITEL) VALUES (@title)";
        public static readonly string GetTitel = "SELECT * FROM TITEL WHERE TITEL LIKE @title";
        public static readonly string GetAllLang = "SELECT * FROM Sprachen";
        public static readonly string InserContact = "INSERT INTO Kontakt (Anrede_ID, Briefanrede_ID, Geschlecht_ID, Vorname, Nachname) VALUES (@anredeId, @brAnredeId, @gId, @vname, @nname)";
        public static readonly string InsertTitelKontakt = "INSERT INTO Titel_Kontakt (Kontakt_ID, Titel_ID) VALUES (@contactId, @titelId)";
        public static readonly string GetLastContact = "SELECT MAX(ID) FROM Kontakt";
        public static readonly string Anrede = "Anrede";
        public static readonly string Briefanrede = "Briefanrede";
        public static readonly string Titel = "Titel";
        public static readonly string Geschlecht = "Geschlecht";
        public static readonly string Vorname = "Vorname";
        public static readonly string Nachname = "Nachname";
        public static readonly List<string> BestandteileList = new List<string> { Anrede, Briefanrede, Titel, Geschlecht, Vorname, Nachname };
        public const string GeschlechtWeibl = "Weiblich";
        public const string GeschlechtMannl = "Männlich";
        public const string GeschlechtKA = "Keine Angabe";
    }
}