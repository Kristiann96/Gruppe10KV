using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ViewModels;

namespace Controller.UnitTests
{
    [TestClass]
    public class KartfeilSkjemaControllerTester
    {
        private KartfeilSkjemaController _kontroller;

        [TestInitialize]
        public void Oppsett()
        {
            _kontroller = new KartfeilSkjemaController();
        }

        #region Sikkerhet
        [TestMethod]
        [Description("Tester at controller har AntiForgeryToken beskyttelse")]
        public void Controller_HarAntiForgeryTokenBeskyttelse()
        {
            // Arrange & Act
            var attributes = typeof(KartfeilSkjemaController)
                .GetCustomAttributes(typeof(AutoValidateAntiforgeryTokenAttribute), true);

            // Assert
            Assert.IsTrue(attributes.Any(), "Controller mangler AutoValidateAntiforgeryToken attributt");
        }

        [TestMethod]
        [Description("Tester at GaaTilBekreftelse har HttpPost attributt")]
        public void GaaTilBekreftelse_HarHttpPostAttributt()
        {
            // Arrange & Act
            var attributes = typeof(KartfeilSkjemaController)
                .GetMethod("GaaTilBekreftelse")
                .GetCustomAttributes(typeof(HttpPostAttribute), true);

            // Assert
            Assert.IsTrue(attributes.Any());
        }
        #endregion

        #region KartfeilSkjema GET Tester
        [TestMethod]
        [Description("Tester at GET-metoden returnerer redirect når geoJson er null")]
        public void KartfeilSkjema_NullGeoJson_RedirectTilKartfeilMarkering()
        {
            // Act
            var resultat = _kontroller.KartfeilSkjema(null) as RedirectToActionResult;

            // Assert
            Assert.IsNotNull(resultat);
            Assert.AreEqual("KartfeilMarkering", resultat.ActionName);
            Assert.AreEqual("KartfeilMarkering", resultat.ControllerName);
        }

        [TestMethod]
        [Description("Tester at GET-metoden returnerer redirect når geoJson er tom")]
        public void KartfeilSkjema_TomGeoJson_RedirectTilKartfeilMarkering()
        {
            // Act
            var resultat = _kontroller.KartfeilSkjema("") as RedirectToActionResult;

            // Assert
            Assert.IsNotNull(resultat);
            Assert.AreEqual("KartfeilMarkering", resultat.ActionName);
            Assert.AreEqual("KartfeilMarkering", resultat.ControllerName);
        }

        [TestMethod]
        [Description("Tester at GET-metoden returnerer view med korrekt initialisert ViewModel")]
        public void KartfeilSkjema_GyldigGeoJson_ReturnererInitialisertViewModel()
        {
            // Arrange
            var geoJson = "{\"type\":\"Point\",\"coordinates\":[10.0,60.0]}";

            // Act
            var resultat = _kontroller.KartfeilSkjema(geoJson) as ViewResult;

            // Assert
            Assert.IsNotNull(resultat);
            var viewModel = resultat.Model as KartfeilSkjemaViewModel;
            Assert.IsNotNull(viewModel);
            Assert.AreEqual(geoJson, viewModel.GeometriGeoJson);
            Assert.AreEqual("", viewModel.Tittel);
            Assert.AreEqual("", viewModel.Beskrivelse);
            Assert.IsFalse(viewModel.ErNodEtatKritisk);
            Assert.IsNull(resultat.ViewName); // Bekrefter standard view
        }
        #endregion

        #region GaaTilBekreftelse POST Tester
        [TestMethod]
        [Description("Tester at POST-metoden setter høy prioritet når ErNodEtatKritisk er true")]
        public void GaaTilBekreftelse_ErNodEtatKritisk_SetterHoyPrioritet()
        {
            // Arrange
            var model = new KartfeilSkjemaViewModel
            {
                GeometriGeoJson = "{\"type\":\"Point\",\"coordinates\":[10.0,60.0]}",
                Tittel = "Test",
                Beskrivelse = "Test",
                ErNodEtatKritisk = true
            };

            // Act
            var resultat = _kontroller.GaaTilBekreftelse(model) as ViewResult;

            // Assert
            Assert.IsNotNull(resultat);
            var returnertModel = resultat.Model as KartfeilSkjemaViewModel;
            Assert.IsNotNull(returnertModel);
            Assert.AreEqual("høy", returnertModel.Prioritet);
            Assert.AreEqual("KartfeilSkjema", resultat.ViewName);
        }

        [TestMethod]
        [Description("Tester at POST-metoden setter ikke_vurdert prioritet når ErNodEtatKritisk er false")]
        public void GaaTilBekreftelse_IkkeNodEtatKritisk_SetterIkkeVurdertPrioritet()
        {
            // Arrange
            var model = new KartfeilSkjemaViewModel
            {
                GeometriGeoJson = "{\"type\":\"Point\",\"coordinates\":[10.0,60.0]}",
                Tittel = "Test",
                Beskrivelse = "Test",
                ErNodEtatKritisk = false
            };

            // Act
            var resultat = _kontroller.GaaTilBekreftelse(model) as ViewResult;

            // Assert
            Assert.IsNotNull(resultat);
            var returnertModel = resultat.Model as KartfeilSkjemaViewModel;
            Assert.IsNotNull(returnertModel);
            Assert.AreEqual("ikke_vurdert", returnertModel.Prioritet);
            Assert.AreEqual("KartfeilSkjema", resultat.ViewName);
        }

        [TestMethod]
        [Description("Tester at POST-metoden returnerer samme view ved ugyldig modell")]
        public void GaaTilBekreftelse_UgyldigModel_ReturnererSammeView()
        {
            // Arrange
            var model = new KartfeilSkjemaViewModel
            {
                GeometriGeoJson = "{\"type\":\"Point\",\"coordinates\":[10.0,60.0]}",
                Tittel = "", // Påkrevd felt
                Beskrivelse = "", // Påkrevd felt
                ErNodEtatKritisk = false
            };
            _kontroller.ModelState.AddModelError("Tittel", "Tittel er påkrevd");
            _kontroller.ModelState.AddModelError("Beskrivelse", "Beskrivelse er påkrevd");

            // Act
            var resultat = _kontroller.GaaTilBekreftelse(model) as ViewResult;

            // Assert
            Assert.IsNotNull(resultat);
            Assert.AreEqual("KartfeilSkjema", resultat.ViewName);
            var returnertModel = resultat.Model as KartfeilSkjemaViewModel;
            Assert.IsNotNull(returnertModel);
            Assert.AreSame(model, returnertModel);
            Assert.IsFalse(_kontroller.ModelState.IsValid);
        }
        #endregion
    }
}