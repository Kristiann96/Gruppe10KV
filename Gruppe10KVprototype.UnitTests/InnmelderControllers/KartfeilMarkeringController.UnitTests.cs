using Microsoft.AspNetCore.Mvc;
using Moq;
using Models.Models;
using Interface;
using LogicInterfaces;

namespace Controller.UnitTests
{
    [TestClass]
    public class KartfeilControllerTester
    {
        private Mock<IGeometriRepository> _geometriRepositoryMock;
        private Mock<IKommuneAPILogic> _kommuneApiLogicMock;
        private KartfeilMarkeringController _kontroller;

        [TestInitialize]
        public void Oppsett()
        {
            _geometriRepositoryMock = new Mock<IGeometriRepository>();
            _kommuneApiLogicMock = new Mock<IKommuneAPILogic>();
            _kontroller = new KartfeilMarkeringController(
                _geometriRepositoryMock.Object,
                _kommuneApiLogicMock.Object
            );
        }

        [TestMethod]
        [Description("Tester at visning av kartfeil-siden fungerer")]
        public void VisKartfeilSide_ReturnererViewResult()
        {
            // Act - Kaller kontroller-metoden
            var resultat = _kontroller.KartfeilMarkering();

            // Assert - Sjekker at vi får tilbake en ViewResult
            Assert.IsInstanceOfType(resultat, typeof(ViewResult));
        }

        [TestMethod]
        [Description("Tester at gyldig geometri-data gir videresending til riktig skjema")]
        public void HåndterGeometri_GyldigGeometri_RedirectTilSkjema()
        {
            // Arrange - Setter opp testdata med gyldig GeoJSON
            var gyldigGeoJson = "{\"type\":\"Point\",\"coordinates\":[10.0,60.0]}";
            var geometri = new Geometri { GeometriGeoJson = gyldigGeoJson };

            // Act - Sender inn geometrien til kontrolleren
            var resultat = _kontroller.GaaTilSkjema(geometri) as RedirectToActionResult;

            // Assert - Verifiserer redirect og parametere
            Assert.IsNotNull(resultat);
            Assert.AreEqual("KartfeilSkjema", resultat.ActionName);
            Assert.AreEqual("KartfeilSkjema", resultat.ControllerName);
            Assert.AreEqual(gyldigGeoJson, resultat.RouteValues["geoJson"]);
        }

        [TestMethod]
        [Description("Tester at tom geometri gir feilmelding og BadRequest")]
        public void HåndterGeometri_TomGeometri_BadRequest()
        {
            // Arrange - Setter opp testdata med tom geometri
            var geometri = new Geometri { GeometriGeoJson = "" };

            // Act - Kaller kontroller-metoden
            var resultat = _kontroller.GaaTilSkjema(geometri) as BadRequestObjectResult;

            // Assert - Sjekker feilmelding og respons-type
            Assert.IsNotNull(resultat);
            Assert.AreEqual("Ingen geometri er tegnet på kartet", resultat.Value);
        }

        [TestMethod]
        [Description("Tester at ugyldig JSON-data i geometrien gir feilmelding og BadRequest")]
        public void HåndterGeometri_UgyldigGeometri_BadRequest()
        {
            // Arrange - Setter opp testdata med ugyldig JSON
            var ugyldigGeoJson = "{ invalid json }";
            var geometri = new Geometri { GeometriGeoJson = ugyldigGeoJson };

            // Act - Tester med ugyldig geometri
            var resultat = _kontroller.GaaTilSkjema(geometri) as BadRequestObjectResult;

            // Assert - Verifiserer feilhåndtering
            Assert.IsNotNull(resultat);
            Assert.AreEqual("Ugyldig geometri-data", resultat.Value);
        }
    }
}