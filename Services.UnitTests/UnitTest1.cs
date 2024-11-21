using Interface;
using LogicInterfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Models.Entities;
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
        private OppdatereInnmeldingService _service = null!;

        [TestInitialize]
        public void TestInitialize()
        {
            // Oppretter mock-objekter for å simulere avhengigheter
            _mockInnmeldingRepo = new Mock<IInnmeldingRepository>();
            _mockInnmeldingLogic = new Mock<IInnmeldingLogic>();

            // Oppretter service-instans med mock-avhengigheter
            _service = new OppdatereInnmeldingService(
                _mockInnmeldingRepo.Object,
                null!, // Null for andre avhengigheter som ikke brukes i denne testen
                null!,
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
    }
}
