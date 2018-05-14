using DHBWKontaktsplitter.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DHBWKontaktsplitter
{
    public static class Formatter
    {
        public static ContactModel DoFormat(ContactModel contact)
        {
            if (!string.IsNullOrEmpty(contact.AnredeText))
            {
                contact.AnredeText = _uppercaseFirstChar(contact.AnredeText);
            }
            if (!string.IsNullOrEmpty(contact.GeschlechtText))
            {
                switch (contact.GeschlechtText)
                {
                    case "W":
                        contact.GeschlechtText = "Weiblich";
                        break;
                    case "M":
                        contact.GeschlechtText = "Männlich";
                        break;
                    case "X":
                        //???
                        break;
                    case "KA":
                        contact.GeschlechtText = "Keine Angabe";
                        break;
                }
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

        public static string FormatNewTitle(string title)
        {
            title = title.ToLower();
            if (title.Last() != '.') title += ".";
            return title;
        }

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

        private static string _removeSpecialCharacters(string value)
        {
            if (value.Contains(",")) return value.Substring(0, value.IndexOf(","));
            return value;
        }
    }
}
