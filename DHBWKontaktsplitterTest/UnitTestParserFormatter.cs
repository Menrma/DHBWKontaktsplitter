using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DHBWKontaktsplitter;
using DHBWKontaktsplitter.Model;

namespace DHBWKontaktsplitterTest
{
    [TestClass]
    public class UnitTestParserFormatter
    {
        [TestMethod]
        public void FormatterTestName1()
        {
            ContactModel model = new ContactModel();

            model.AnredeText = "frau";
            model.BriefanredeText = "Sehr geehrte";
            model.Vorname = "sandra";
            model.Nachname = "berger";
            model.GeschlechtText = "W";
            model.TitelList.Add(new TitleModel {Title = "dr."});

            var result = Formatter.DoFormat(model);

            Assert.AreEqual("Frau", result.AnredeText);
            Assert.AreEqual("Sehr geehrte", result.BriefanredeText);
            Assert.AreEqual("Dr.", result.AllTitles);
            Assert.AreEqual("Weiblich", result.GeschlechtText);
            Assert.AreEqual("Sandra", result.Vorname);
            Assert.AreEqual("Berger", result.Nachname);
        }

        [TestMethod]
        public void FormatterTestName2()
        {
            ContactModel model = new ContactModel();

            model.AnredeText = "herr";
            model.BriefanredeText = "Sehr geehrter";
            model.Vorname = "winfried";
            model.Nachname = "russwurm,";
            model.GeschlechtText = "M";
            model.TitelList.Add(new TitleModel { Title = "dr." });

            var result = Formatter.DoFormat(model);

            Assert.AreEqual("Herr", result.AnredeText);
            Assert.AreEqual("Sehr geehrter", result.BriefanredeText);
            Assert.AreEqual("Dr.", result.AllTitles);
            Assert.AreEqual("Männlich", result.GeschlechtText);
            Assert.AreEqual("Winfried", result.Vorname);
            Assert.AreEqual("Russwurm", result.Nachname);
        }
    }
}