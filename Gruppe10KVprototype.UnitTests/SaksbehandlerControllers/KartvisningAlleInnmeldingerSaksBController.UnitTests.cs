using Gruppe10KVprototype.Controllers.SaksbehandlerControllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Moq;
using LogicInterfaces;
using Models.Models;
using ViewModels;
using Interface;

namespace Controller.UnitTests
{
    [TestClass]
    public class KartvisningAlleInnmeldingerSaksBControllerTests
    {
        private Mock<IGeometriRepository> _geometriRepositoryMock;
        private Mock<IKommuneAPILogic> _kommuneAPILogicMock;
        private KartvisningAlleInnmeldingerSaksBController _controller;
        private Mock<ITempDataDictionary> _tempDataMock;

        [TestInitialize]
        public void Initialize()
        {
            _geometriRepositoryMock = new Mock<IGeometriRepository>();
            _kommuneAPILogicMock = new Mock<IKommuneAPILogic>();
            _tempDataMock = new Mock<ITempDataDictionary>();

            _controller = new KartvisningAlleInnmeldingerSaksBController(
                _geometriRepositoryMock.Object,
                _kommuneAPILogicMock.Object);

            // Initialize TempData
            _controller.TempData = _tempDataMock.Object;
        }

        #region KartvisningAlleInnmeldingerSaksB Tests

        [TestMethod]
        [Description("Tester at KartvisningAlleInnmeldingerSaksB henter data og returnerer korrekt ViewModel")]
        public async Task KartvisningAlleInnmeldingerSaksB_ReturnsViewWithCorrectData()
        {
            // Arrange
            var geometriData = new List<Geometri>
            {
                new Geometri { GeometriGeoJson = "{ \"type\": \"Point\", \"coordinates\": [10.0, 60.0] }" }
            };
            var kommunerData = new List<Kommune>
            {
                new Kommune { Kommunenavn = "Oslo", Kommunenummer = "0301" }
            };

            _geometriRepositoryMock.Setup(x => x.GetAllGeometriAsync())
                .ReturnsAsync(geometriData);
            _kommuneAPILogicMock.Setup(x => x.GetKommunerAsync())
                .ReturnsAsync(kommunerData);

            // Act
            IActionResult actionResult = await _controller.KartvisningAlleInnmeldingerSaksB();

            // Assert
            Assert.IsNotNull(actionResult);
            Assert.IsInstanceOfType(actionResult, typeof(ViewResult));

            var viewResult = (ViewResult)actionResult;
            Assert.IsNotNull(viewResult.Model);
            Assert.IsInstanceOfType(viewResult.Model, typeof(KartvisningAlleInnmeldingerSaksBViewModel));

            var viewModel = (KartvisningAlleInnmeldingerSaksBViewModel)viewResult.Model;
            Assert.AreEqual(1, viewModel.GeometriData.Count());
            Assert.AreEqual(1, viewModel.KommunerData.Count());
        }

        #endregion

        #region GetKommunenummer Tests

        [TestMethod]
        [Description("Tester at GetKommunenummer henter kommunenummer og navn, og redirecter til korrekt action")]
        public async Task GetKommunenummer_RedirectsToCorrectActionWithTempData()
        {
            // Arrange
            var kommune = new Kommune { Kommunenavn = "Oslo", Kommunenummer = "0301" };
            _kommuneAPILogicMock.Setup(x => x.GetKommuneByCoordinatesAsync(It.IsAny<double>(), It.IsAny<double>()))
                .ReturnsAsync(kommune);

            // Setup TempData expectations
            var tempData = new Dictionary<string, object>();
            _tempDataMock.Setup(td => td[It.IsAny<string>()])
                .Returns((string key) => tempData.ContainsKey(key) ? tempData[key] : null);
            _tempDataMock.Setup(td => td.Add(It.IsAny<string>(), It.IsAny<object>()))
                .Callback<string, object>((key, value) => tempData[key] = value);

            // Act
            IActionResult actionResult = await _controller.GetKommunenummer(1, 60.0, 10.0);

            // Assert
            Assert.IsNotNull(actionResult);
            Assert.IsInstanceOfType(actionResult, typeof(RedirectToActionResult));

            var redirectResult = (RedirectToActionResult)actionResult;
            Assert.AreEqual("KartvisningEnInnmeldingSaksB", redirectResult.ActionName);
            Assert.AreEqual("KartvisningEnInnmeldingSaksB", redirectResult.ControllerName);

            // Verify TempData was set correctly
            _tempDataMock.VerifySet(td => td["Kommunenummer"] = "0301");
            _tempDataMock.VerifySet(td => td["Kommunenavn"] = "Oslo");
        }

        #endregion
    }
}