using Interface;
using LogicInterfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Models.Entities;
using Models.Models;
using Moq;
using ViewModels;
using Models.Exceptions;

namespace Services.UnitTests
{
    [TestClass]
    public class OppdatereInnmeldingServiceTests
    {
        private Mock<IInnmeldingRepository> _mockInnmeldingRepo = null!;
        private Mock<IInnmeldingLogic> _mockInnmeldingLogic = null!;
        private Mock<IGeometriRepository> _mockGeometriRepo = null!;
        private Mock<ITransaksjonsRepository> _mockTransaksjonsRepo = null!;
        private OppdatereInnmeldingService _service = null!;

        [TestInitialize]
        public void TestInitialize()
        {
            // Oppretter mock-objekter for alle avhengigheter
            _mockInnmeldingRepo = new Mock<IInnmeldingRepository>();
            _mockInnmeldingLogic = new Mock<IInnmeldingLogic>();
            _mockGeometriRepo = new Mock<IGeometriRepository>();
            _mockTransaksjonsRepo = new Mock<ITransaksjonsRepository>();

            // Oppretter service-instans med alle mock-avhengigheter
            _service = new OppdatereInnmeldingService(
                _mockInnmeldingRepo.Object,
                _mockGeometriRepo.Object,
                _mockTransaksjonsRepo.Object,
                _mockInnmeldingLogic.Object
            );
        }

        [TestMethod]
        [Description("Sikrer at OppdatereInnmeldingAsync ikke kaller repository når validering feiler")]
        public async Task OppdatereInnmeldingAsync_ValideringFeiler_SkalIkkeKalleRepository()
        {
            // Arrange - Oppretter testdata
            var testViewModel = new OppdatereInnmeldingViewModel
            {
                InnmeldingId = 1,
                Tittel = "Testtittel",
                Beskrivelse = "Testbeskrivelse"
            };

            // Simulerer at valideringslogikken kaster ForretningsRegelExceptionModel
            _mockInnmeldingLogic
                .Setup(x => x.ValiderInnmeldingData(It.IsAny<InnmeldingModel>()))
                .ThrowsAsync(new ForretningsRegelExceptionModel("Validering feilet"));

            // Act & Assert - Verifiserer at unntaket kastes
            var exception = await Assert.ThrowsExceptionAsync<ForretningsRegelExceptionModel>(() => 
                _service.OppdatereInnmeldingAsync(testViewModel));
            
            // Verifiserer feilmeldingen for ekstra trygghet
            Assert.AreEqual("Validering feilet", exception.Message);

            // Verifiserer at repository-metoden aldri blir kalt
            _mockInnmeldingRepo.Verify(
                x => x.OppdatereInnmeldingAsync(It.IsAny<InnmeldingModel>()), 
                Times.Never()
            );

            // Ekstra verifisering for å sikre at det ikke er uforventede kall
            _mockInnmeldingLogic.Verify(
                x => x.ValiderInnmeldingData(It.IsAny<InnmeldingModel>()), 
                Times.Once() // Bekrefter at validering kun ble kalt én gang
            );
        }
        
        [TestMethod]
        [Description("Sikrer at HentInnmeldingForOppdateringAsync faktisk returnerer riktig viewmodel når innmelding eksisterer")]
        public async Task HentInnmeldingForOppdateringAsync_NårInnmeldingEksisterer_ReturViewModel()
        {
            // Arrange
            var testInnmelding = new InnmeldingModel { Tittel = "Test", Beskrivelse = "Beskrivelse" };
            var testGeometri = new Geometri { GeometriGeoJson = "{}" };

            _mockInnmeldingRepo.Setup(x => x.GetInnmeldingAsync(It.IsAny<int>()))
                .ReturnsAsync(new List<InnmeldingModel> { testInnmelding });
            _mockGeometriRepo.Setup(x => x.GetGeometriByInnmeldingIdAsync(It.IsAny<int>()))
                .ReturnsAsync(testGeometri);

            // Act
            var result = await _service.HentInnmeldingForOppdateringAsync(1);

            // Assert
            Assert.AreEqual("Test", result.Tittel);
            Assert.AreEqual("Beskrivelse", result.Beskrivelse);
            Assert.AreEqual("{}", result.GeometriGeoJson);
        }
        
        [TestMethod]
        [Description("Sikrer at HentInnmeldingForOppdateringAsync faktisk kaster KeyNotFoundException når innmelding ikke eksisterer")]
        public async Task HentInnmeldingForOppdateringAsync_NårInnmeldingIkkeEksisterer_KasterKeyNotFoundException()
        {
            // Arrange
            _mockInnmeldingRepo.Setup(x => x.GetInnmeldingAsync(It.IsAny<int>()))
                .ReturnsAsync(new List<InnmeldingModel>());

            // Act & Assert
            var exception = await Assert.ThrowsExceptionAsync<KeyNotFoundException>(() =>
                _service.HentInnmeldingForOppdateringAsync(1));

            Assert.AreEqual("Innmelding med id 1 ble ikke funnet", exception.Message);
        }
        
        [TestMethod]
        [Description("Sikrer at HentInnmeldingForOppdateringAsync håndterer unntak kastet av innmelding repository")]
        public async Task HentInnmeldingForOppdateringAsync_NårInnmeldingRepoKasterException_HåndtererException()
        {
            // Arrange
            _mockInnmeldingRepo.Setup(x => x.GetInnmeldingAsync(It.IsAny<int>()))
                .ThrowsAsync(new Exception("Database error"));

            // Act & Assert
            var exception = await Assert.ThrowsExceptionAsync<Exception>(() =>
                _service.HentInnmeldingForOppdateringAsync(1));

            Assert.AreEqual("Database error", exception.Message);
        }

        [TestMethod]
        [Description("Sikrer at SlettInnmeldingAsync sletter innmelding når den eksisterer")]
        public async Task SlettInnmeldingAsync_NårInnmeldingEksisterer_SletterOgReturnereSant()
        {
            // Arrange
            var innmeldingId = 1;
            var testInnmelding = new InnmeldingModel { InnmeldingId = innmeldingId };
            
            _mockInnmeldingRepo
                .Setup(x => x.GetInnmeldingAsync(innmeldingId))
                .ReturnsAsync(new List<InnmeldingModel> { testInnmelding });
            
            _mockTransaksjonsRepo
                .Setup(x => x.SlettInnmeldingMedTilhorendeDataAsync(innmeldingId))
                .ReturnsAsync(true);

            // Act
            var result = await _service.SlettInnmeldingAsync(innmeldingId);

            // Assert
            Assert.IsTrue(result);
            _mockInnmeldingRepo.Verify(x => x.GetInnmeldingAsync(innmeldingId), Times.Once);
            _mockTransaksjonsRepo.Verify(
                x => x.SlettInnmeldingMedTilhorendeDataAsync(innmeldingId), 
                Times.Once
            );
        }

        [TestMethod]
        [Description("Sikrer at SlettInnmeldingAsync kaster KeyNotFoundException når innmelding ikke eksisterer")]
        public async Task SlettInnmeldingAsync_NårInnmeldingIkkeEksisterer_KasterKeyNotFoundException()
        {
            // Arrange
            var innmeldingId = 1;
    
            _mockInnmeldingRepo
                .Setup(x => x.GetInnmeldingAsync(innmeldingId))
                .ReturnsAsync(new List<InnmeldingModel>());

            // Act & Assert
            var exception = await Assert.ThrowsExceptionAsync<KeyNotFoundException>(() =>
                _service.SlettInnmeldingAsync(innmeldingId));

            Assert.AreEqual($"Innmelding med id {innmeldingId} ble ikke funnet", exception.Message);
            _mockInnmeldingRepo.Verify(x => x.GetInnmeldingAsync(innmeldingId), Times.Once);
            _mockTransaksjonsRepo.Verify(
                x => x.SlettInnmeldingMedTilhorendeDataAsync(innmeldingId), 
                Times.Never
            );
        }
    }
}