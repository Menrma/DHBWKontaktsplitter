using DHBWKontaktsplitter;
using DHBWKontaktsplitter.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DHBWKontaktsplitterTest
{
    [TestClass]
    public class UnitTestValidator
    {
        [TestMethod]
        public void ValidatorSuccess()
        {
            ContactModel contact = new ContactModel();

            contact.AnredeText = "herr";
            contact.BriefanredeText = "Sehr geehrter";
            contact.GeschlechtText = "Männlich";

            var result = Validator.ValidateContact(contact);

            Assert.AreEqual(0, result);
        }

        [TestMethod]
        public void ValidatorSuccess2()
        {
            ContactModel contact = new ContactModel();

            contact.AnredeText = "frau";
            contact.BriefanredeText = "Sehr geehrte";
            contact.GeschlechtText = "Weiblich";

            var result = Validator.ValidateContact(contact);

            Assert.AreEqual(0, result);
        }

        [TestMethod]
        public void ValidatorFail()
        {
            ContactModel contact = new ContactModel();

            contact.AnredeText = "her";
            contact.BriefanredeText = "Sehr geehrter";
            contact.GeschlechtText = "Männlich";

            var result = Validator.ValidateContact(contact);

            Assert.AreEqual(7, result); //Anrede fehlerhaft
        }

        [TestMethod]
        public void ValidatorFail2()
        {
            ContactModel contact = new ContactModel();

            contact.AnredeText = "frau";
            contact.BriefanredeText = "Sehr geehrteee";
            contact.GeschlechtText = "Weiblich";

            var result = Validator.ValidateContact(contact);

            Assert.AreEqual(8, result); //Briefanrede fehlerhaft
        }

        [TestMethod]
        public void ValidatorFail3()
        {
            ContactModel contact = new ContactModel();

            contact.AnredeText = "frau";
            contact.BriefanredeText = "Sehr geehrte";
            contact.GeschlechtText = "Weiblichhh";

            var result = Validator.ValidateContact(contact);

            Assert.AreEqual(9, result); //Geschlecht-Text fehlerhaft
        }
    }
}
