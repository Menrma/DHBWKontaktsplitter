using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DHBWKontaktsplitter;

namespace DHBWKontaktsplitterTest
{
    [TestClass]
    public class UnitTestParser
    {
        [TestMethod]
        public void ParserTestName1()
        {
            Parser p = new Parser();

            var result = p.ExecuteInput("Frau Sandra Berger");

            Assert.AreEqual("frau", result.Contact.AnredeText);
            Assert.AreEqual("Sehr geehrte", result.Contact.BriefanredeText);
            Assert.AreEqual(null, result.Contact.AllTitles);
            Assert.AreEqual("W", result.Contact.GeschlechtText);
            Assert.AreEqual("sandra", result.Contact.Vorname);
            Assert.AreEqual("berger", result.Contact.Nachname);
        }

        [TestMethod]
        public void ParserTestName2()
        {
            Parser p = new Parser();

            var result = p.ExecuteInput("Herr Dr. Sandro Gutmensch");

            Assert.AreEqual("herr", result.Contact.AnredeText);
            Assert.AreEqual("Sehr geehrter", result.Contact.BriefanredeText);
            Assert.AreEqual(null, result.Contact.AllTitles);
            Assert.AreEqual(1, result.Contact.TitelList.Count);
            Assert.AreEqual("M", result.Contact.GeschlechtText);
            Assert.AreEqual("sandro", result.Contact.Vorname);
            Assert.AreEqual("gutmensch", result.Contact.Nachname);
        }

        [TestMethod]
        public void ParserTestName3()
        {
            Parser p = new Parser();

            var result = p.ExecuteInput("Dr. Russwurm, Winfried");
            
            Assert.AreEqual(null, result.Contact.AllTitles);
            Assert.AreEqual(1, result.Contact.TitelList.Count);
            Assert.AreEqual("winfried", result.Contact.Vorname);
            Assert.AreEqual("russwurm,", result.Contact.Nachname);
        }

        [TestMethod]
        public void ParserTestName4()
        {
            Parser p = new Parser();

            var result = p.ExecuteInput("Estobar y Gonzales");

            Assert.AreEqual("estobar", result.Contact.Vorname);
            Assert.AreEqual("y gonzales", result.Contact.Nachname);
        }

        [TestMethod]
        public void ParserTestName5()
        {
            Parser p = new Parser();

            var result = p.ExecuteInput("Herr Dipl. Ing. Max von Müller");

            Assert.AreEqual("herr", result.Contact.AnredeText);
            Assert.AreEqual("Sehr geehrter", result.Contact.BriefanredeText);
            Assert.AreEqual("M", result.Contact.GeschlechtText);
            Assert.AreEqual("max", result.Contact.Vorname);
            Assert.AreEqual("von müller", result.Contact.Nachname);
        }

        [TestMethod]
        public void ParserTestName6()
        {
            Parser p = new Parser();

            var result = p.ExecuteInput("Sandra Berger");
            
            Assert.AreEqual("Sehr geehrte Damen und Herren", result.Contact.BriefanredeText);
            Assert.AreEqual("KA", result.Contact.GeschlechtText);
            Assert.AreEqual("sandra", result.Contact.Vorname);
            Assert.AreEqual("berger", result.Contact.Nachname);
        }

    }
}