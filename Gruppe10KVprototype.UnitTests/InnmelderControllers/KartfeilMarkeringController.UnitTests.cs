using Microsoft.AspNetCore.Mvc;
using Moq;
using Models.Models;
using Interface;
using LogicInterfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Controller.UnitTests
{
    [TestClass]
    public class KartfeilMarkeringControllerTester
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

        #region Sikkerhet
        [TestMethod]
        [Description("Tester at controller har AntiForgeryToken beskyttelse")]
        public void Controller_HarAntiForgeryTokenBeskyttelse()
        {
            // Arrange & Act
            var controllerAttributes = typeof(KartfeilMarkeringController)
                .GetCustomAttributes(typeof(AutoValidateAntiforgeryTokenAttribute), true);
            
            var methodAttributes = typeof(KartfeilMarkeringController)
                .GetMethod("GaaTilSkjema")
                .GetCustomAttributes(typeof(ValidateAntiForgeryTokenAttribute), true);

            // Assert
            Assert.IsTrue(controllerAttributes.Any(), "Controller mangler AutoValidateAntiforgeryToken attributt");
            Assert.IsTrue(methodAttributes.Any(), "GaaTilSkjema mangler ValidateAntiForgeryToken attributt");
        }
    
        [TestMethod]
        [Description("Tester at metoden har HttpPost attributt")]
        public void GaaTilSkjema_HarHttpPostAttributt()
        {
            // Arrange & Act
            var attributes = typeof(KartfeilMarkeringController)
                .GetMethod("GaaTilSkjema")
                .GetCustomAttributes(typeof(HttpPostAttribute), true);

            // Assert
            Assert.IsTrue(attributes.Any());
        }

        [TestMethod]
        [Description("Tester at metoden har FromBody attributt på parameteren")]
        public void GaaTilSkjema_HarFromBodyAttributt()
        {
            // Arrange & Act
            var parameter = typeof(KartfeilMarkeringController)
                .GetMethod("GaaTilSkjema")
                .GetParameters()
                .First();
            
            var attribute = parameter.GetCustomAttributes(typeof(FromBodyAttribute), true);

            // Assert
            Assert.IsTrue(attribute.Any());
        }
        #endregion

        #region KartfeilMarkering Tester
        [TestMethod]
        [Description("Tester at visning av kartfeilmarkering-siden fungerer")]
        public void KartfeilMarkering_ReturnererViewResult()
        {
            // Act
            var resultat = _kontroller.KartfeilMarkering();

            // Assert
            Assert.IsInstanceOfType(resultat, typeof(ViewResult));
        }

        [TestMethod]
        [Description("Tester at KartfeilMarkering returnerer standard view")]
        public void KartfeilMarkering_ReturnererStandardView()
        {
            // Act
            var resultat = _kontroller.KartfeilMarkering() as ViewResult;

            // Assert
            Assert.IsNotNull(resultat);
            Assert.IsNull(resultat.ViewName); // Bekrefter at standard viewnavn brukes
        }
        #endregion

        #region GaaTilSkjema Tester
        [TestMethod]
        [Description("Tester at gyldig geometri-data gir videresending til riktig skjema")]
        public void GaaTilSkjema_GyldigGeometri_RedirectTilSkjema()
        {
            // Arrange
            var gyldigGeoJson = "{\"type\":\"Point\",\"coordinates\":[10.0,60.0]}";
            var geometri = new Geometri { GeometriGeoJson = gyldigGeoJson };

            // Act
            var resultat = _kontroller.GaaTilSkjema(geometri) as RedirectToActionResult;

            // Assert
            Assert.IsNotNull(resultat);
            Assert.AreEqual("KartfeilSkjema", resultat.ActionName);
            Assert.AreEqual("KartfeilSkjema", resultat.ControllerName);
            Assert.AreEqual(gyldigGeoJson, resultat.RouteValues["geoJson"]);
        }

        [TestMethod]
        [Description("Tester at tom geometri gir feilmelding og BadRequest")]
        public void GaaTilSkjema_TomGeometri_BadRequest()
        {
            // Arrange
            var geometri = new Geometri { GeometriGeoJson = "" };

            // Act
            var resultat = _kontroller.GaaTilSkjema(geometri) as BadRequestObjectResult;

            // Assert
            Assert.IsNotNull(resultat);
            Assert.AreEqual("Ingen geometri er tegnet på kartet", resultat.Value);
        }

        [TestMethod]
        [Description("Tester at ugyldig JSON-data i geometrien gir feilmelding og BadRequest")]
        public void GaaTilSkjema_UgyldigGeometri_BadRequest()
        {
            // Arrange
            var ugyldigGeoJson = "{ invalid json }";
            var geometri = new Geometri { GeometriGeoJson = ugyldigGeoJson };

            // Act
            var resultat = _kontroller.GaaTilSkjema(geometri) as BadRequestObjectResult;

            // Assert
            Assert.IsNotNull(resultat);
            Assert.AreEqual("Ugyldig geometri-data", resultat.Value);
        }

        [TestMethod]
        [Description("Tester at gyldig geometri med kompleks GeoJSON håndteres korrekt")]
        public void GaaTilSkjema_KompleksGyldigGeometri_RedirectTilSkjema()
        {
            // Arrange
            var kompleksGeoJson = @"{
                ""type"": ""Polygon"",
                ""coordinates"": [
                    [[10.0, 60.0], [11.0, 60.0], [11.0, 61.0], [10.0, 61.0], [10.0, 60.0]]
                ]
            }";
            var geometri = new Geometri { GeometriGeoJson = kompleksGeoJson };

            // Act
            var resultat = _kontroller.GaaTilSkjema(geometri) as RedirectToActionResult;

            // Assert
            Assert.IsNotNull(resultat);
            Assert.AreEqual("KartfeilSkjema", resultat.ActionName);
            Assert.AreEqual("KartfeilSkjema", resultat.ControllerName);
            Assert.AreEqual(kompleksGeoJson, resultat.RouteValues["geoJson"]);
        }

        [TestMethod]
        [Description("Tester håndtering av geometri med ugyldig GeoJSON struktur")]
        public void GaaTilSkjema_UgyldigGeoJsonStruktur_RedirectTilSkjema()
        {
            // Arrange - GeoJSON med feil struktur men gyldig JSON
            var ugyldigGeoJson = @"{
                ""type"": ""UkjentType"",
                ""koordinater"": [10.0, 60.0]
            }";
            var geometri = new Geometri { GeometriGeoJson = ugyldigGeoJson };

            // Act
            var resultat = _kontroller.GaaTilSkjema(geometri) as RedirectToActionResult;

            // Assert - Selv om strukturen er feil, er det fortsatt gyldig JSON
            Assert.IsNotNull(resultat);
            Assert.AreEqual("KartfeilSkjema", resultat.ActionName);
            Assert.AreEqual(ugyldigGeoJson, resultat.RouteValues["geoJson"]);
        }
        #endregion
    }
}