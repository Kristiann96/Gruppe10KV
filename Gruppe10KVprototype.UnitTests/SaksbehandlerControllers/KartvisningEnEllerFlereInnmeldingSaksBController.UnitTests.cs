using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Moq;
using Interface;
using Interfaces;
using LogicInterfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Models.Models;
using Models.Entities;
using ViewModels;

namespace Controller.UnitTests
{
    [TestClass]
    public class KartvisningEnEllerFlereInnmeldingSaksBControllerTests
    {
        private Mock<IGeometriRepository> _geometriRepositoryMock;
        private Mock<IDataSammenstillingSaksBRepository> _dataSammenstillingsRepoMock;
        private Mock<IEnumLogic> _enumLogicMock;
        private Mock<IVurderingRepository> _vurderingRepositoryMock;
        private KartvisningEnEllerFlereInnmeldingSaksBController _controller;
        private Mock<ITempDataDictionary> _tempDataMock;

        [TestInitialize]
        public void Initialize()
        {
            _geometriRepositoryMock = new Mock<IGeometriRepository>();
            _dataSammenstillingsRepoMock = new Mock<IDataSammenstillingSaksBRepository>();
            _enumLogicMock = new Mock<IEnumLogic>();
            _vurderingRepositoryMock = new Mock<IVurderingRepository>();
            _tempDataMock = new Mock<ITempDataDictionary>();

            _controller = new KartvisningEnEllerFlereInnmeldingSaksBController(
                _geometriRepositoryMock.Object,
                _dataSammenstillingsRepoMock.Object,
                _enumLogicMock.Object,
                _vurderingRepositoryMock.Object);

            _controller.TempData = _tempDataMock.Object;
        }

        [TestMethod]
        [Description("Tester at KartvisningEnEllerFlereInnmeldingSaksB returnerer korrekt ViewModel med en innmelding")]
        public async Task KartvisningEnEllerFlereInnmeldingSaksB_WithSingleId_ReturnsViewWithCorrectData()
        {
            // Arrange
            int innmeldingId = 1;
            var innmelding = new InnmeldingModel
            {
                InnmeldingId = innmeldingId,
                Status = "UNDER_BEHANDLING",
                Prioritet = "HOY",
                KartType = "KART_TYPE_1",
                SisteEndring = DateTime.Now
            };
            var person = new PersonModel { PersonId = 1, Fornavn = "Test", Etternavn = "Testesen" };
            var innmelder = new InnmelderModel { InnmelderId = 1, InnmelderType = "PRIVAT" };
            var saksbehandler = new SaksbehandlerModel { SaksbehandlerId = 1 };
            var geometri = new Geometri { GeometriGeoJson = "test", InnmeldingId = innmeldingId };
            var kommentarer = new List<string> { "Test kommentar" };

            _dataSammenstillingsRepoMock.Setup(x => x.GetInnmeldingMedDetaljerAsync(innmeldingId))
                .ReturnsAsync((innmelding, person, innmelder, saksbehandler));

            _geometriRepositoryMock.Setup(x => x.GetGeometriByInnmeldingIdAsync(innmeldingId))
                .ReturnsAsync(geometri);

            _vurderingRepositoryMock.Setup(x => x.HentAntallVurderingerAsync(innmeldingId))
                .ReturnsAsync((2, 1)); // 2 bekreftelser, 1 avkreftelse

            _vurderingRepositoryMock.Setup(x => x.HentKommentarerForInnmeldingAsync(innmeldingId))
                .ReturnsAsync(kommentarer);

            _enumLogicMock.Setup(x => x.ConvertToDisplayFormat(It.IsAny<string>()))
                .Returns<string>(s => $"Formatert_{s}");

            // Act
            IActionResult actionResult = await _controller.KartvisningEnEllerFlereInnmeldingSaksB(innmeldingId, null);

            // Assert
            Assert.IsNotNull(actionResult);
            Assert.IsInstanceOfType(actionResult, typeof(ViewResult));

            var viewResult = (ViewResult)actionResult;
            Assert.IsNotNull(viewResult.Model);
            Assert.IsInstanceOfType(viewResult.Model, typeof(KartvisningEnEllerFlereInnmeldingSaksBViewModel));

            var viewModel = (KartvisningEnEllerFlereInnmeldingSaksBViewModel)viewResult.Model;
            Assert.AreEqual(1, viewModel.AlleInnmeldinger.Count);

            var innmeldingDetaljer = viewModel.AlleInnmeldinger.First();
            Assert.AreEqual(innmeldingId, innmeldingDetaljer.Innmelding.InnmeldingId);
            Assert.AreEqual("Formatert_UNDER_BEHANDLING", innmeldingDetaljer.Innmelding.Status);
            Assert.AreEqual("Formatert_HOY", innmeldingDetaljer.Innmelding.Prioritet);
            Assert.AreEqual("Formatert_KART_TYPE_1", innmeldingDetaljer.Innmelding.KartType);
            Assert.AreEqual(2, innmeldingDetaljer.AntallBekreftelser);
            Assert.AreEqual(1, innmeldingDetaljer.AntallAvkreftelser);
            Assert.AreEqual(1, innmeldingDetaljer.Kommentarer.Count());
            Assert.AreEqual("Test Testesen", innmeldingDetaljer.SaksbehandlerNavn);
        }

        [TestMethod]
        [Description("Tester at KartvisningEnEllerFlereInnmeldingSaksB returnerer korrekt ViewModel med flere innmeldinger")]
        public async Task KartvisningEnEllerFlereInnmeldingSaksB_WithMultipleIds_ReturnsViewWithCorrectData()
        {
            // Arrange
            var innmeldingIds = "1,2";
            var innmelding1 = new InnmeldingModel
            {
                InnmeldingId = 1,
                Status = "UNDER_BEHANDLING",
                SisteEndring = DateTime.Now
            };
            var innmelding2 = new InnmeldingModel
            {
                InnmeldingId = 2,
                Status = "FERDIG",
                SisteEndring = DateTime.Now
            };
            var person = new PersonModel { PersonId = 1, Fornavn = "Test", Etternavn = "Testesen" };
            var innmelder = new InnmelderModel { InnmelderId = 1 };
            var saksbehandler = new SaksbehandlerModel { SaksbehandlerId = 1 };
            var geometri = new Geometri { GeometriGeoJson = "test", InnmeldingId = 1 };

            _dataSammenstillingsRepoMock.Setup(x => x.GetInnmeldingMedDetaljerAsync(1))
                .ReturnsAsync((innmelding1, person, innmelder, saksbehandler));
            _dataSammenstillingsRepoMock.Setup(x => x.GetInnmeldingMedDetaljerAsync(2))
                .ReturnsAsync((innmelding2, person, innmelder, saksbehandler));

            _geometriRepositoryMock.Setup(x => x.GetGeometriByInnmeldingIdAsync(It.IsAny<int>()))
                .ReturnsAsync(geometri);

            _vurderingRepositoryMock.Setup(x => x.HentAntallVurderingerAsync(It.IsAny<int>()))
                .ReturnsAsync((1, 0));

            _vurderingRepositoryMock.Setup(x => x.HentKommentarerForInnmeldingAsync(It.IsAny<int>()))
                .ReturnsAsync(new List<string>());

            _enumLogicMock.Setup(x => x.ConvertToDisplayFormat(It.IsAny<string>()))
                .Returns<string>(s => $"Formatert_{s}");

            // Act
            IActionResult actionResult = await _controller.KartvisningEnEllerFlereInnmeldingSaksB(null, innmeldingIds);

            // Assert
            Assert.IsNotNull(actionResult);
            Assert.IsInstanceOfType(actionResult, typeof(ViewResult));

            var viewResult = (ViewResult)actionResult;
            var viewModel = (KartvisningEnEllerFlereInnmeldingSaksBViewModel)viewResult.Model;
            Assert.AreEqual(2, viewModel.AlleInnmeldinger.Count);
        }

        [TestMethod]
        [Description("Tester at KartvisningEnEllerFlereInnmeldingSaksB returnerer NotFound når ingen innmeldinger finnes")]
        public async Task KartvisningEnEllerFlereInnmeldingSaksB_WithNonExistentId_ReturnsNotFound()
        {
            // Arrange
            int innmeldingId = 999;
            _dataSammenstillingsRepoMock.Setup(x => x.GetInnmeldingMedDetaljerAsync(innmeldingId))
                .ReturnsAsync((null, null, null, null));

            // Act
            IActionResult actionResult = await _controller.KartvisningEnEllerFlereInnmeldingSaksB(innmeldingId, null);

            // Assert
            Assert.IsNotNull(actionResult);
            Assert.IsInstanceOfType(actionResult, typeof(NotFoundObjectResult));

            var notFoundResult = (NotFoundObjectResult)actionResult;
            Assert.AreEqual("Ingen innmeldinger funnet", notFoundResult.Value);
        }
    }
}