using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Interface;
using Interfaces;
using LogicInterfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Models.Entities;
using Models.Models;
using ViewModels;

namespace Controller.UnitTests
{
    [TestClass]
    public class OversiktAlleInnmeldingerSaksBController_UnitTests
    {
        private readonly Mock<IDataSammenstillingSaksBRepository> _mockDataSammenstillingsRepo;
        private readonly Mock<IGeometriRepository> _mockGeometriRepo;
        private readonly Mock<IKommuneAPILogic> _mockKommuneAPILogic;
        private readonly Mock<IEnumLogic> _mockEnumLogic;
        private readonly OversiktAlleInnmeldingerSaksBController _controller;
        private readonly List<(InnmeldingModel, PersonModel, Geometri, GjesteinnmelderModel, InnmelderModel)> _testData;

        public OversiktAlleInnmeldingerSaksBController_UnitTests()
        {
            _mockDataSammenstillingsRepo = new Mock<IDataSammenstillingSaksBRepository>();
            _mockGeometriRepo = new Mock<IGeometriRepository>();
            _mockKommuneAPILogic = new Mock<IKommuneAPILogic>();
            _mockEnumLogic = new Mock<IEnumLogic>();
            _testData = new List<(InnmeldingModel, PersonModel, Geometri, GjesteinnmelderModel, InnmelderModel)>
            {
                (
                    new InnmeldingModel
                    {
                        InnmeldingId = 1, Tittel = "Innmelding 1", Status = "ny", Prioritet = "høy",
                        SisteEndring = DateTime.Now
                    },
                    new PersonModel { Fornavn = "Ola", Etternavn = "Nordmann" },
                    new Geometri { GeometriGeoJson = "{}" },
                    new GjesteinnmelderModel { Epost = "gjest@example.com" },
                    new InnmelderModel { Epost = "innmelder@example.com" }
                ),
                (
                    new InnmeldingModel
                    {
                        InnmeldingId = 2, Tittel = "Innmelding 2", Status = "under_behandling", Prioritet = "lav",
                        SisteEndring = DateTime.Now.AddDays(-7)
                    },
                    new PersonModel { Fornavn = "Kari", Etternavn = "Hansen" },
                    new Geometri { GeometriGeoJson = "{}" },
                    new GjesteinnmelderModel { Epost = "gjest2@example.com" },
                    new InnmelderModel { Epost = "innmelder2@example.com" }
                )
            };

            _controller = new OversiktAlleInnmeldingerSaksBController(
                _mockDataSammenstillingsRepo.Object,
                _mockGeometriRepo.Object,
                _mockKommuneAPILogic.Object,
                _mockEnumLogic.Object
            );
        }

        [TestMethod]
        [Description("Verifiserer at controller-metoden returnerer:" +
                     "\n1. Et ViewResult" +
                     "\n2. En not-null ViewModel" +
                     "\n3. Korrekt antall innmeldinger i ViewModelen")]
        public async Task OversiktAlleInnmeldingerSaksB_NårInnmeldingFinnes_ReturnererForventetViewModel()
        {
            // Arrange
            _mockDataSammenstillingsRepo.Setup(x =>
                    x.GetOversiktAlleInnmeldingerSaksBAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>()))
                .ReturnsAsync((_testData, 2));

            // Act
            var result = await _controller.OversiktAlleInnmeldingerSaksB(1, 10, "");

            // Assert
            Assert.IsInstanceOfType(result, typeof(ViewResult));
            var viewResult = (ViewResult)result;
    
            var viewModel = viewResult.Model as OversiktAlleInnmeldingerSaksBViewModel;
            Assert.IsNotNull(viewModel);
            Assert.AreEqual(2, viewModel.InnmeldingId.Count());
        }

        [TestMethod]
        [Description("Verifiserer at enum-verdier blir korrekt formatert for visning:" +
                     "\n1. Status 'ny' blir til 'Ny'" +
                     "\n2. Status 'under_behandling' blir til 'Under behandling'" +
                     "\n3. Prioritet 'høy' blir til 'Høy'" +
                     "\n4. Prioritet 'lav' blir til 'Lav'" +
                     "\n5. Sjekker at antall status- og prioritetsverdier matcher testdataen" +
                     "\n6. Bekrefter at rekkefølgen på de formaterte verdiene er korrekt")]
        public async Task OversiktAlleInnmeldingerSaksB_FormattererEnumVerdierKorrekt()
        {
            // Arrange
            _mockDataSammenstillingsRepo.Setup(x =>
                    x.GetOversiktAlleInnmeldingerSaksBAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>()))
                .ReturnsAsync((_testData, 2));

            _mockEnumLogic.Setup(x => x.ConvertToDisplayFormat("ny"))
                .Returns(Logic.EnumFormatter.ToNormalText("ny"));
            _mockEnumLogic.Setup(x => x.ConvertToDisplayFormat("under_behandling"))
                .Returns(Logic.EnumFormatter.ToNormalText("under_behandling"));
            _mockEnumLogic.Setup(x => x.ConvertToDisplayFormat("høy"))
                .Returns(Logic.EnumFormatter.ToNormalText("høy"));
            _mockEnumLogic.Setup(x => x.ConvertToDisplayFormat("lav"))
                .Returns(Logic.EnumFormatter.ToNormalText("lav"));

            // Act
            var result = await _controller.OversiktAlleInnmeldingerSaksB(1, 10, "");
            var viewModel = (result as ViewResult).Model as OversiktAlleInnmeldingerSaksBViewModel;

            // Assert
            Assert.IsNotNull(viewModel);
            Assert.AreEqual(2, viewModel.Status.Count(), "Status count mismatch");
            Assert.IsTrue(viewModel.Status.SequenceEqual(new[] { "Ny", "Under behandling" }), "Status mismatch");
            Assert.IsTrue(viewModel.Prioritet.SequenceEqual(new[] { "Høy", "Lav" }), "Prioritet mismatch");
        }

        [TestMethod]
        [Description("Verifiserer at geometri blir korrekt hentet for innmeldinger:" +
                     "\n- Henter geometri én gang for hver innmelding" +
                     "\n- Henter geometri med korrekt innmeldingId")]
        public async Task OversiktAlleInnmeldingerSaksB_HenterGeometriDataKorrekt()
        {
            // Arrange
            _mockDataSammenstillingsRepo.Setup(x =>
                    x.GetOversiktAlleInnmeldingerSaksBAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>()))
                .ReturnsAsync((_testData, 2));

            // Act
            var result = await _controller.OversiktAlleInnmeldingerSaksB(1, 10, "");

            // Assert
            // Verifiser at metoden blir kalt nøyaktig én gang for hver innmelding
            _mockGeometriRepo.Verify(x => x.GetGeometriByInnmeldingIdAsync(1), Times.Once());
            _mockGeometriRepo.Verify(x => x.GetGeometriByInnmeldingIdAsync(2), Times.Once());
        }
    }
}