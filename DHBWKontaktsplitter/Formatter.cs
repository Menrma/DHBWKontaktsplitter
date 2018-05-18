using DHBWKontaktsplitter.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DHBWKontaktsplitter
{
    /// <summary>
    /// Klasse für die Formatierung
    /// </summary>
    public static class Formatter
    {
        /// <summary>
        /// Methode für die Formatierung eines Models vom Typ Contact
        /// </summary>
        /// <param name="contact">Vom Parser erstelltes Model</param>
        /// <returns></returns>
        public static ContactModel DoFormat(ContactModel contact)
        {
            if (!string.IsNullOrEmpty(contact.AnredeText))
            {
                contact.AnredeText = _uppercaseFirstChar(contact.AnredeText);
            }
            if (!string.IsNullOrEmpty(contact.GeschlechtText))
            {
                contact.GeschlechtText = _convertGeschlechtToText(contact.GeschlechtText);
            }
            if (contact.TitelList.Count > 0)
            {
                foreach(var title in contact.TitelList)
                {
                    title.Title = _uppercaseFirstChar(title.Title);
                    title.Title = title.Title.Trim();
                    contact.AllTitles += title.Title + " ";
                }
                contact.AllTitles = contact.AllTitles?.Trim();
            }
            if (!string.IsNullOrEmpty(contact.Vorname))
            {
                contact.Vorname = _uppercaseFirstChar(contact.Vorname);
                contact.Vorname = contact.Vorname.Trim();
            }
            if (!string.IsNullOrEmpty(contact.Nachname))
            {
                contact.Nachname = _uppercaseFirstChar(contact.Nachname);
                contact.Nachname = _removeSpecialCharacters(contact.Nachname);
                contact.Nachname = contact.Nachname.Trim();
            }

            return contact;
        }

        /// <summary>
        /// Methode für das Formatieren eines neu eingegebenen Artikels
        /// </summary>
        /// <param name="title">Titel</param>
        /// <returns></returns>
        public static string FormatNewTitle(string title)
        {
            title = title.ToLower();
            if (title.Last() != '.') title += ".";
            return title;
        }

        /// <summary>
        /// Methode, welche das erste Zeichen einer Eingabe Groß schreibt
        /// </summary>
        /// <param name="value"></param>
        /// <returns>Eingabe mit erstem Zeichen groß</returns>
        private static string _uppercaseFirstChar(string value)
        {
            var valueArray = value.Trim().Split(' ');
            if (valueArray.Length == 1)
                return value.First().ToString().ToUpper() + value.Substring(1);
            else
            {
                string result = string.Empty;
                for (int i = 0; i < valueArray.Length; i++)
                {
                    if (i < valueArray.Length - 1) result += valueArray[i] + " ";
                    else result += valueArray[i].First().ToString().ToUpper() + valueArray[i].Substring(1);
                }
                return result;
            }
        }

        /// <summary>
        /// Methode welche ein Komma aus einer Eingabe entfernt
        /// </summary>
        /// <param name="value">Eingabe ohne Komma</param>
        /// <returns></returns>
        private static string _removeSpecialCharacters(string value)
        {
            if (value.Contains(",")) return value.Substring(0, value.IndexOf(","));
            return value;
        }

        /// <summary>
        /// Methode für die Umwandlung eines Geschlechts-Kürzels in einen definierten Text
        /// </summary>
        /// <param name="geschlecht">Geschlecht-Kürzel (W,M,KA)</param>
        /// <returns></returns>
        private static string _convertGeschlechtToText(string geschlecht)
        {
            switch (geschlecht)
            {
                case "W":
                    return StaticHelper.GeschlechtWeibl;
                case "M":
                    return StaticHelper.GeschlechtMannl;
                case "X":
                    //???
                    break;
            }
            return StaticHelper.GeschlechtKA;
        }

        /// <summary>
        /// Methode für die Umwandlung eines Geschlecht-Texts in das entsprechende Kürzel
        /// </summary>
        /// <param name="text">Geschlecht als Text</param>
        /// <returns></returns>
        public static string ConvertTextToGeschlecht(string text)
        {
            switch (text)
            {
                case StaticHelper.GeschlechtWeibl:
                    return "W";
                case StaticHelper.GeschlechtMannl:
                    return "M";
                case StaticHelper.GeschlechtKA:
                    return "KA";
            }
            return string.Empty;
        }
    }
}
