using Microsoft.AspNetCore.Mvc;

namespace Gruppe10KVprototype.Tests.Controllers
{
    [TestClass]
    public class KartfeilSkjemaControllerTester
    {
        private KartfeilSkjemaController _controller;

        [TestInitialize]
        public void SetUp()
        {
            _controller = new KartfeilSkjemaController();
        }

        [TestMethod]
        [Description("Tester at GET-metoden KartfeilSkjema returnerer redirect når geoJson er tom eller null")]
        public void KartfeilSkjema_GeoJsonNullOrEmpty_Redirigerer()
        {
            // Act - Kaller GET-metoden med null
            var result = _controller.KartfeilSkjema(null) as RedirectToActionResult;

            // Assert - Verifiserer at det er en omdirigering til KartfeilMarkering
            Assert.IsNotNull(result);
            Assert.AreEqual("KartfeilMarkering", result.ActionName);
            Assert.AreEqual("KartfeilMarkering", result.ControllerName);
        }

        [TestMethod]
        [Description("Tester at GET-metoden KartfeilSkjema returnerer View med riktig ViewModel")]
        public void KartfeilSkjema_GeoJsonProvided_ReturnererViewMedViewModel()
        {
            // Arrange
            var geoJson = "{\"type\":\"Point\",\"coordinates\":[10.0,60.0]}";

            // Act - Kaller GET-metoden med geoJson
            var result = _controller.KartfeilSkjema(geoJson) as ViewResult;

            // Assert - Verifiserer at resultatet er en ViewResult med riktig ViewModel
            Assert.IsNotNull(result);
            var viewModel = result.Model as KartfeilSkjemaViewModel;
            Assert.IsNotNull(viewModel);
            Assert.AreEqual(geoJson, viewModel.GeometriGeoJson);
        }

        [TestMethod]
        [Description("Tester at POST-metoden GaaTilBekreftelse returnerer bekreftelses-skjema når modell er gyldig")]
        public void GaaTilBekreftelse_ValidModel_ReturnererBekreftelseView()
        {
            // Arrange - Lag en gyldig KartfeilSkjemaViewModel
            var model = new KartfeilSkjemaViewModel
            {
                GeometriGeoJson = "{\"type\":\"Point\",\"coordinates\":[10.0,60.0]}",
                Tittel = "Test Tittel",
                Beskrivelse = "Test Beskrivelse",
                ErNodEtatKritisk = true // Prioritet skal være "høy"
            };

            // Act - Kaller POST-metoden
            var result = _controller.GaaTilBekreftelse(model) as ViewResult;

            // Assert - Verifiserer at resultatet er ViewResult og at modellen er den samme
            Assert.IsNotNull(result);
            var returnedModel = result.Model as KartfeilSkjemaViewModel;
            Assert.IsNotNull(returnedModel);
            Assert.AreEqual("høy", returnedModel.Prioritet);
        }

        [TestMethod]
        [Description("Tester at POST-metoden GaaTilBekreftelse returnerer samme skjema når modellen er ugyldig")]
        public void GaaTilBekreftelse_InvalidModel_ReturnererKartfeilSkjemaView()
        {
            // Arrange - Lag en ugyldig KartfeilSkjemaViewModel
            var model = new KartfeilSkjemaViewModel
            {
                GeometriGeoJson = "{\"type\":\"Point\",\"coordinates\":[10.0,60.0]}",
                Tittel = "",
                Beskrivelse = "", // Tittel og Beskrivelse er påkrevd
                ErNodEtatKritisk = false
            };
            _controller.ModelState.AddModelError("Tittel", "Tittel er påkrevd");
            _controller.ModelState.AddModelError("Beskrivelse", "Beskrivelse er påkrevd");

            // Act - Kaller POST-metoden med en ugyldig modell
            var result = _controller.GaaTilBekreftelse(model) as ViewResult;

            // Assert - Verifiserer at det returneres tilbake til KartfeilSkjema-skjemaet med ugyldig modell
            Assert.IsNotNull(result);
            var returnedModel = result.Model as KartfeilSkjemaViewModel;
            Assert.IsNotNull(returnedModel);
            Assert.AreEqual(model, returnedModel);
        }
    }
}
