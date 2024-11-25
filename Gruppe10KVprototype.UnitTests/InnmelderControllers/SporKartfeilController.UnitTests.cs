using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Interface;
using LogicInterfaces;
using Models.Models;
using Gruppe10KVprototype.Controllers.InnmelderControllers;
using System.Text.Json;

namespace Gruppe10KVprototype.Tests.Controllers
{
    [TestClass]
    public class SporKartfeilControllerTests
    {
        private Mock<IGeometriRepository> _mockGeometriRepository;
        private Mock<IKommuneAPILogic> _mockKommuneAPILogic;
        private SporKartfeilController _controller;

        [TestInitialize]
        public void Setup()
        {
            _mockGeometriRepository = new Mock<IGeometriRepository>();
            _mockKommuneAPILogic = new Mock<IKommuneAPILogic>();
            _controller = new SporKartfeilController(
                _mockGeometriRepository.Object,
                _mockKommuneAPILogic.Object
            );
        }

        [TestMethod]
        [Description("Verifiserer at kontrolleren håndterer en tom GeoJSON-streng ved å returnere en BadRequest med korrekt feilmelding")]
        public void A_BehandleGeoJson_GirFeilmelding_NaarGeoJsonErTom()
        {
            // Arrange
            var geometri = new Geometri { GeometriGeoJson = string.Empty };

            // Act
            var result = _controller.GaaTilSkjema(geometri);

            // Assert
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
            var badRequestResult = (BadRequestObjectResult)result;
            Assert.AreEqual("Ingen sporet rute funnet", badRequestResult.Value);
        }

        [TestMethod]
        [Description("Verifiserer at kontrolleren håndterer en ugyldig GeoJSON-streng ved å returnere en BadRequest med korrekt feilmelding")]
        public void B_BehandleGeoJson_GirFeilmelding_NaarGeoJsonErUgyldig()
        {
            // Arrange
            var geometri = new Geometri { GeometriGeoJson = "ugyldig json" };

            // Act
            var result = _controller.GaaTilSkjema(geometri);

            // Assert
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
            var badRequestResult = (BadRequestObjectResult)result;
            Assert.AreEqual("Ugyldig sporingsdata", badRequestResult.Value);
        }

        [TestMethod]
        [Description("Verifiserer at kontrolleren håndterer en null GeoJSON-verdi ved å returnere en BadRequest med korrekt feilmelding")]
        public void C_BehandleGeoJson_GirFeilmelding_NaarGeoJsonErNull()
        {
            // Arrange
            var geometri = new Geometri { GeometriGeoJson = null };

            // Act
            var result = _controller.GaaTilSkjema(geometri);

            // Assert
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
            var badRequestResult = (BadRequestObjectResult)result;
            Assert.AreEqual("Ingen sporet rute funnet", badRequestResult.Value);
        }

        [TestMethod]
        [Description("Verifiserer at kontrolleren videresender til riktig action med korrekte parametere når en gyldig GeoJSON blir sendt inn")]
        public void D_BehandleGeoJson_OmdirigererTilSkjema_NaarGeoJsonErGyldig()
        {
            // Arrange
            var validGeoJson = "{\"type\":\"Feature\",\"geometry\":{\"type\":\"Point\",\"coordinates\":[125.6,10.1]}}";
            var geometri = new Geometri { GeometriGeoJson = validGeoJson };

            // Act
            var result = _controller.GaaTilSkjema(geometri);

            // Assert
            Assert.IsInstanceOfType(result, typeof(RedirectToActionResult));
            var redirectResult = (RedirectToActionResult)result;
            Assert.AreEqual("KartfeilSkjema", redirectResult.ActionName);
            Assert.AreEqual("KartfeilSkjema", redirectResult.ControllerName);
            Assert.IsNotNull(redirectResult.RouteValues);
            Assert.AreEqual(validGeoJson, redirectResult.RouteValues["geoJson"]);
        }

        [TestMethod]
        [Description("Verifiserer at SporKartfeil-action returnerer korrekt visning")]
        public void E_SporKartfeil_ReturnerKorrektVisning()
        {
            // Act
            var result = _controller.SporKartfeil();

            // Assert
            Assert.IsInstanceOfType(result, typeof(ViewResult));
        }
    }
}