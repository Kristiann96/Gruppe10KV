using Interface;
using LogicInterfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Models.Entities;
using Models.Exceptions;
using Moq;
using ViewModels;

namespace Services.UnitTests;

[TestClass]
public class OppdatereInnmeldingServiceTests
{
    private Mock<IInnmeldingRepository> _mockInnmeldingRepo = null!;
    private Mock<IGeometriRepository> _mockGeometriRepo = null!;
    private Mock<ITransaksjonsRepository> _mockTransaksjonsRepo = null!;
    private Mock<IInnmeldingLogic> _mockInnmeldingLogic = null!;
    private OppdatereInnmeldingService _service = null!;

    [TestInitialize]
    public void TestInitialize()
    {
        _mockInnmeldingRepo = new Mock<IInnmeldingRepository>();
        _mockGeometriRepo = new Mock<IGeometriRepository>();
        _mockTransaksjonsRepo = new Mock<ITransaksjonsRepository>();
        _mockInnmeldingLogic = new Mock<IInnmeldingLogic>();
        _service = new OppdatereInnmeldingService(
            _mockInnmeldingRepo.Object,
            _mockGeometriRepo.Object,
            _mockTransaksjonsRepo.Object,
            _mockInnmeldingLogic.Object
        );
    }

    [TestMethod]
    [Description("Verifiserer at riktig exception type kastes fra logikklaget og at repository ikke kalles")]
    public async Task OppdatereInnmelding_NaarValideringFeilerILogikk_SkalIkkeKallePaaRepository()
    {
        // Arrange
        var testViewModel = new OppdatereInnmeldingViewModel
        {
            InnmeldingId = 1,
            Tittel = "Test",
            Beskrivelse = "Test"
        };

        _mockInnmeldingLogic.Setup(x => x.ValiderInnmeldingData(It.IsAny<InnmeldingModel>()))
            .ThrowsAsync(new ForretningsRegelExceptionModel("Data er ikke gyldig"));

        // Act & Assert
        await Assert.ThrowsExceptionAsync<ForretningsRegelExceptionModel>(() =>
            _service.OppdatereInnmeldingAsync(testViewModel));

        _mockInnmeldingRepo.Verify(r => r.OppdatereInnmeldingAsync(It.IsAny<InnmeldingModel>()), 
            Times.Never());
    }
}