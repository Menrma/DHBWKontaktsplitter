using DHBWKontaktsplitter.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DHBWKontaktsplitter
{
    public static class Validator
    {
        public static int ValidateContact(ContactModel contact)
        {
            InformationBase informationBase = new InformationBase();
            int languId = 0;
            int gId = 0;

            if (!string.IsNullOrEmpty(contact.AnredeText))
            {
                var anredeTupel = informationBase.GetAnrede(new List<string> { contact.AnredeText.ToLower() });
                contact.AnredeId = anredeTupel.Item1;
                var anrede = anredeTupel.Item2;
                if (string.IsNullOrEmpty(anrede)) return 7; //Anrede fehlerhaft
            }

            if (!string.IsNullOrEmpty(contact.BriefanredeText))
            {
                languId = informationBase.GetLanguFromAnredeTable();
                gId = informationBase.GetGeschlechtFromAnredeTable();
                var brAnredeTuple = informationBase.GetBriefanrede(languId, gId);
                contact.BriefanredeId = brAnredeTuple.Item1;
                string brAnrede = brAnredeTuple.Item2;
                if (contact.BriefanredeText.ToLower().Trim() != brAnrede.ToLower().Trim()) return 8;
            }

            if (!string.IsNullOrEmpty(contact.GeschlechtText))
            {
                string geschlContact = Formatter.ConvertTextToGeschlecht(contact.GeschlechtText);
                var geschlTuple = informationBase.GetGeschlecht(gId);

                contact.GeschlechtId = geschlTuple.Item1;
                var geschl = geschlTuple.Item2;

                if (geschlContact.ToLower().Trim() != geschl.ToLower().Trim()) return 9;
            }
            return 0;
        }

    }
}
