using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using ViewModels;

namespace Gruppe10KVprototype.Tests.Controllers
{
    [TestClass]
    public class KartfeilSkjemaBekreftelseControllerTester
    {
        private KartfeilSkjemaBekreftelseController _kontroller;

        [TestInitialize]
        public void Oppsett()
        {
            _kontroller = new KartfeilSkjemaBekreftelseController();
        }

        [TestMethod]
        [Description("Tester at bekreftelse med tom geometri omdirigerer til kartfeil-siden")]
        public void BekreftSkjema_TomGeometri_RedirectTilKartfeilSide()
        {
            // Arrange - Setter opp ViewModel med tom geometri
            var modell = new KartfeilSkjemaViewModel { GeometriGeoJson = string.Empty };

            // Act - Kaller kontroller-metoden
            var resultat = _kontroller.KartfeilSkjemaBekreftelse(modell) as RedirectToActionResult;

            // Assert - Verifiserer redirect til riktig side
            Assert.IsNotNull(resultat);
            Assert.AreEqual("KartfeilMarkering", resultat.ActionName);
            Assert.AreEqual("KartfeilMarkering", resultat.ControllerName);
        }


        [TestMethod]
        [Description("Tester at bekreftelse med gyldig geometri returnerer riktig view")]
        public void BekreftSkjema_GyldigGeometri_ReturnererView()
        {
            // Arrange - Setter opp ViewModel med gyldig GeoJSON
            var gyldigGeoJson = "{\"type\":\"Point\",\"coordinates\":[10.0,60.0]}";
            var modell = new KartfeilSkjemaViewModel { GeometriGeoJson = gyldigGeoJson };

            // Act - Kaller kontroller-metoden
            var resultat = _kontroller.KartfeilSkjemaBekreftelse(modell) as ViewResult;

            // Assert - Verifiserer at riktig view returneres med modellen
            Assert.IsNotNull(resultat);
            Assert.AreSame(modell, resultat.Model);
        }
    }
}