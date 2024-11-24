using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Collections.Generic;
using System.Threading.Tasks;
using ViewModels;
using Interfaces;
using Models.Entities;
using Models.Models;
using LogicInterfaces;
using Gruppe10KVprototype.Controllers.SaksbehandlerControllers;
using Interface;

namespace Gruppe10KVprototype.Tests.Controllers.SaksbehandlerControllers
{
    [TestClass]
    public class BehandleInnmeldingSaksBControllerTests
    {
        private Mock<IGeometriRepository> _mockGeometriRepository = null!;
        private Mock<IEnumLogic> _mockEnumLogic = null!;
        private Mock<IDataSammenstillingSaksBRepository> _mockDataSammenstillingRepository = null!;
        private Mock<IInnmeldingRepository> _mockInnmeldingRepository = null!;
        private Mock<ISaksbehandlerRepository> _mockSaksbehandlerRepository = null!;
        private BehandleInnmeldingSaksBController _controller = null!;

        [TestInitialize]
        public void SettOpp()
        {
            // Initialiserer mock-objektene
            _mockGeometriRepository = new Mock<IGeometriRepository>();
            _mockEnumLogic = new Mock<IEnumLogic>();
            _mockDataSammenstillingRepository = new Mock<IDataSammenstillingSaksBRepository>();
            _mockInnmeldingRepository = new Mock<IInnmeldingRepository>();
            _mockSaksbehandlerRepository = new Mock<ISaksbehandlerRepository>();

            // Oppretter controller med mock-objektene
            _controller = new BehandleInnmeldingSaksBController(
                _mockGeometriRepository.Object,
                _mockEnumLogic.Object,
                _mockDataSammenstillingRepository.Object,
                _mockInnmeldingRepository.Object,
                _mockSaksbehandlerRepository.Object
            );
        }

        [TestMethod]
        [Description("Tester at BehandleInnmeldingSaksB returnerer korrekt view og view-model når innmelding eksisterer.")]
        public async Task BehandleInnmeldingSaksB_SkalReturnereView_NarInnmeldingEksisterer()
        {
            // Arrange
            int innmeldingId = 1;
            var mockInnmelding = new InnmeldingModel();
            var mockPerson = new PersonModel();
            var mockInnmelder = new InnmelderModel();
            var mockSaksbehandler = new SaksbehandlerModel();
            var mockGeometri = new Geometri();

            _mockDataSammenstillingRepository.Setup(repo => repo.GetInnmeldingMedDetaljerAsync(innmeldingId))
                .ReturnsAsync((mockInnmelding, mockPerson, mockInnmelder, mockSaksbehandler));
            _mockGeometriRepository.Setup(repo => repo.GetGeometriByInnmeldingIdAsync(innmeldingId))
                .ReturnsAsync(mockGeometri);
            _mockEnumLogic.Setup(logic => logic.GetFormattedStatusEnumValuesAsync())
                .ReturnsAsync(new List<string> { "Status1", "Status2" });

            // Act
            var resultat = await _controller.BehandleInnmeldingSaksB(innmeldingId);

            // Assert
            var viewResultat = resultat as ViewResult;
            Assert.IsNotNull(viewResultat, "Forventet ViewResult.");
            Assert.IsInstanceOfType(viewResultat!.Model, typeof(BehandleInnmeldingSaksBViewModel),
                "Forventet at model er av typen BehandleInnmeldingSaksBViewModel.");
        }

        [TestMethod]
        [Description("Tester at BehandleInnmeldingSaksB redirecter til oversikt når innmelding ikke finnes.")]
        public async Task BehandleInnmeldingSaksB_SkalRedirecte_NarInnmeldingIkkeFinnes()
        {
            // Arrange
            int innmeldingId = 999;
            _mockDataSammenstillingRepository.Setup(repo => repo.GetInnmeldingMedDetaljerAsync(innmeldingId))
                .ReturnsAsync((default(InnmeldingModel), default(PersonModel), default(InnmelderModel), default(SaksbehandlerModel)));

            // Act
            var resultat = await _controller.BehandleInnmeldingSaksB(innmeldingId);

            // Assert
            var redirectResultat = resultat as RedirectToActionResult;
            Assert.IsNotNull(redirectResultat, "Forventet RedirectToActionResult.");
            Assert.AreEqual("OversiktAlleInnmeldingerSaksB", redirectResultat!.ActionName,
                "Forventet redirect til 'OversiktAlleInnmeldingerSaksB'.");
        }

        [TestMethod]
        [Description("Tester at OppdateringAvInnmeldingSaksB oppdaterer korrekt og returnerer success-melding.")]
        public async Task OppdateringAvInnmeldingSaksB_SkalRedirecteMedSuccess_NarOppdateringLykkes()
        {
            // Arrange
            int innmeldingId = 1;
            var viewModel = new BehandleInnmeldingSaksBViewModel
            {
                InnmeldingModel = new InnmeldingModel
                {
                    Status = "Status",
                    Prioritet = "Prioritet",
                    KartType = "KartType"
                }
            };

            // Mock the EnumLogic conversion logic
            _mockEnumLogic.Setup(logic => logic.ConvertToDbFormat(It.IsAny<string>()))
                .Returns((string s) => $"Db{s}");

            // Mock the repository update method
            _mockInnmeldingRepository.Setup(repo => repo.OppdatereEnumSaksBAsync(It.IsAny<int>(), It.IsAny<InnmeldingModel>()))
                .ReturnsAsync(true);

            // Act
            var resultat = await _controller.OppdateringAvInnmeldingSaksB(innmeldingId, viewModel);

            // Assert
            var redirectResultat = resultat as RedirectToActionResult;
            Assert.IsNotNull(redirectResultat, "Forventet RedirectToActionResult.");
            Assert.AreEqual("BehandleInnmeldingSaksB", redirectResultat!.ActionName,
                "Forventet redirect til 'BehandleInnmeldingSaksB'.");

            _mockEnumLogic.Verify(logic => logic.ConvertToDbFormat(It.IsAny<string>()), Times.Exactly(3),
                "Forventet at ConvertToDbFormat kalles tre ganger.");
            _mockInnmeldingRepository.Verify(repo => repo.OppdatereEnumSaksBAsync(innmeldingId, viewModel.InnmeldingModel), Times.Once,
                "Forventet at OppdatereEnumSaksBAsync kalles én gang.");
        }
    }
}
