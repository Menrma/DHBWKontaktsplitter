using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DHBWKontaktsplitter.Model
{
    /// <summary>
    /// Model, welches einen Kontakt repräsentiert
    /// </summary>
    public class ContactModel
    {
        public string Eingabe { get; set; }
        public int AnredeId { get; set; }
        public string AnredeText { get; set; }
        public int BriefanredeId { get; set; }
        public string BriefanredeText { get; set; }
        public List<TitleModel> TitelList { get; set; } = new List<TitleModel>();
        public string AllTitles { get; set; }
        public int GeschlechtId { get; set; }
        public string GeschlechtText { get; set; }
        public string Vorname { get; set; }
        public string Nachname { get; set; }
    }
}
