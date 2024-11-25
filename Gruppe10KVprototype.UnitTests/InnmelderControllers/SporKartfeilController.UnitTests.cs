using Microsoft.AspNetCore.Mvc;
using Moq;
using Interface;
using LogicInterfaces;
using Models.Models;
using Gruppe10KVprototype.Controllers.InnmelderControllers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Controller.UnitTests
{
    [TestClass]
    public class SporKartfeilControllerTester
    {
        private Mock<IGeometriRepository> _geometriRepositoryMock;
        private Mock<IKommuneAPILogic> _kommuneApiLogicMock;
        private SporKartfeilController _kontroller;

        [TestInitialize]
        public void Oppsett()
        {
            _geometriRepositoryMock = new Mock<IGeometriRepository>();
            _kommuneApiLogicMock = new Mock<IKommuneAPILogic>();
            _kontroller = new SporKartfeilController(
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
            var attributes = typeof(SporKartfeilController)
                .GetCustomAttributes(typeof(AutoValidateAntiforgeryTokenAttribute), true);

            // Assert
            Assert.IsTrue(attributes.Any(), "Controller mangler AutoValidateAntiforgeryToken attributt");
        }

        [TestMethod]
        [Description("Tester at GaaTilSkjema har ValidateAntiForgeryToken attributt")]
        public void GaaTilSkjema_HarValidateAntiForgeryTokenAttributt()
        {
            // Arrange & Act
            var methodInfo = typeof(SporKartfeilController).GetMethod("GaaTilSkjema");
            var attributes = methodInfo.GetCustomAttributes(typeof(ValidateAntiForgeryTokenAttribute), true);

            // Assert
            Assert.IsTrue(attributes.Any(), "GaaTilSkjema mangler ValidateAntiForgeryToken attributt");
        }

        [TestMethod]
        [Description("Tester at GaaTilSkjema har HttpPost attributt")]
        public void GaaTilSkjema_HarHttpPostAttributt()
        {
            // Arrange & Act
            var methodInfo = typeof(SporKartfeilController).GetMethod("GaaTilSkjema");
            var attributes = methodInfo.GetCustomAttributes(typeof(HttpPostAttribute), true);

            // Assert
            Assert.IsTrue(attributes.Any(), "GaaTilSkjema mangler HttpPost attributt");
        }

        [TestMethod]
        [Description("Tester at GaaTilSkjema har FromBody attributt på parameteren")]
        public void GaaTilSkjema_HarFromBodyAttributt()
        {
            // Arrange & Act
            var parameter = typeof(SporKartfeilController)
                .GetMethod("GaaTilSkjema")
                .GetParameters()
                .First();
            
            var attribute = parameter.GetCustomAttributes(typeof(FromBodyAttribute), true);

            // Assert
            Assert.IsTrue(attribute.Any(), "Geometri parameter mangler FromBody attributt");
        }
        #endregion

        #region SporKartfeil Tester
        [TestMethod]
        [Description("Tester at SporKartfeil returnerer standard view")]
        public void SporKartfeil_ReturnererStandardView()
        {
            // Act
            var resultat = _kontroller.SporKartfeil() as ViewResult;

            // Assert
            Assert.IsNotNull(resultat);
            Assert.IsNull(resultat.ViewName); // Bekrefter at standard viewnavn brukes
        }
        #endregion

        #region GaaTilSkjema Tester
        [TestMethod]
        [Description("Tester håndtering av tom GeoJSON")]
        public void GaaTilSkjema_TomGeoJson_ReturnererBadRequest()
        {
            // Arrange
            var geometri = new Geometri { GeometriGeoJson = string.Empty };

            // Act
            var resultat = _kontroller.GaaTilSkjema(geometri) as BadRequestObjectResult;

            // Assert
            Assert.IsNotNull(resultat);
            Assert.AreEqual("Ingen sporet rute funnet", resultat.Value);
        }

        [TestMethod]
        [Description("Tester håndtering av ugyldig GeoJSON")]
        public void GaaTilSkjema_UgyldigGeoJson_ReturnererBadRequest()
        {
            // Arrange
            var geometri = new Geometri { GeometriGeoJson = "ugyldig json" };

            // Act
            var resultat = _kontroller.GaaTilSkjema(geometri) as BadRequestObjectResult;

            // Assert
            Assert.IsNotNull(resultat);
            Assert.AreEqual("Ugyldig sporingsdata", resultat.Value);
        }

        [TestMethod]
        [Description("Tester håndtering av null GeoJSON")]
        public void GaaTilSkjema_NullGeoJson_ReturnererBadRequest()
        {
            // Arrange
            var geometri = new Geometri { GeometriGeoJson = null };

            // Act
            var resultat = _kontroller.GaaTilSkjema(geometri) as BadRequestObjectResult;

            // Assert
            Assert.IsNotNull(resultat);
            Assert.AreEqual("Ingen sporet rute funnet", resultat.Value);
        }

        [TestMethod]
        [Description("Tester videresending med gyldig GeoJSON")]
        public void GaaTilSkjema_GyldigGeoJson_RedirectTilSkjema()
        {
            // Arrange
            var gyldigGeoJson = "{\"type\":\"Feature\",\"geometry\":{\"type\":\"Point\",\"coordinates\":[125.6,10.1]}}";
            var geometri = new Geometri { GeometriGeoJson = gyldigGeoJson };

            // Act
            var resultat = _kontroller.GaaTilSkjema(geometri) as RedirectToActionResult;

            // Assert
            Assert.IsNotNull(resultat);
            Assert.AreEqual("KartfeilSkjema", resultat.ActionName);
            Assert.AreEqual("KartfeilSkjema", resultat.ControllerName);
            Assert.IsNotNull(resultat.RouteValues);
            Assert.AreEqual(gyldigGeoJson, resultat.RouteValues["geoJson"]);
        }

        [TestMethod]
        [Description("Tester at GaaTilSkjema håndterer kompleks GeoJSON")]
        public void GaaTilSkjema_KompleksGeoJson_RedirectTilSkjema()
        {
            // Arrange
            var kompleksGeoJson = @"{
                ""type"": ""Feature"",
                ""geometry"": {
                    ""type"": ""LineString"",
                    ""coordinates"": [
                        [10.0, 60.0],
                        [11.0, 61.0],
                        [12.0, 62.0]
                    ]
                }
            }";
            var geometri = new Geometri { GeometriGeoJson = kompleksGeoJson };

            // Act
            var resultat = _kontroller.GaaTilSkjema(geometri) as RedirectToActionResult;

            // Assert
            Assert.IsNotNull(resultat);
            Assert.AreEqual("KartfeilSkjema", resultat.ActionName);
            Assert.AreEqual("KartfeilSkjema", resultat.ControllerName);
            Assert.IsNotNull(resultat.RouteValues);
            Assert.AreEqual(kompleksGeoJson, resultat.RouteValues["geoJson"]);
        }
        #endregion
    }
}