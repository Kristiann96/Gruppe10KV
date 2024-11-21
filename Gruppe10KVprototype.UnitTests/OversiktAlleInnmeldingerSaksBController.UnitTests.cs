using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Interface;
using Interfaces;
using LogicInterfaces;
using Models.Entities;
using Models.Models;
using ViewModels;
using Gruppe10KVprototype.Controllers.SaksbehandlerControllers;

namespace Gruppe10KVprototype.UnitTests;

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
                new InnmeldingModel { InnmeldingId = 1, Tittel = "Innmelding 1", Status = "ny", Prioritet = "høy", SisteEndring = DateTime.Now },
                new PersonModel { Fornavn = "Ola", Etternavn = "Nordmann" },
                new Geometri { GeometriGeoJson = "{}" },
                new GjesteinnmelderModel { Epost = "gjest@example.com" },
                new InnmelderModel { Epost = "innmelder@example.com" }
            ),
            (
                new InnmeldingModel { InnmeldingId = 2, Tittel = "Innmelding 2", Status = "under_behandling", Prioritet = "lav", SisteEndring = DateTime.Now.AddDays(-7) },
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
    [Description("Verifiserer at controller-metoden returnerer:"+
                 "\n1. Et ViewResult" +
                 "\n2. Riktig view-navn ('OversiktAlleInnmeldingerSaksB')" +
                 "\n3. En not-null ViewModel" +
                 "\n4. Korrekt antall innmeldinger i ViewModelen")]
    public async Task OversiktAlleInnmeldingerSaksB_NårInnmeldingFinnes_ReturnererForventetViewModel()
    {
        // Arrange
        _mockDataSammenstillingsRepo.Setup(x => x.GetOversiktAlleInnmeldingerSaksBAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>()))
            .ReturnsAsync((_testData, 2));

        // Act
        var result = await _controller.OversiktAlleInnmeldingerSaksB(1, 10, "");

        // Assert
        Assert.IsInstanceOfType(result, typeof(ViewResult));
        var viewResult = (ViewResult)result;
        Assert.AreEqual("OversiktAlleInnmeldingerSaksB", viewResult.ViewName);
        
        var viewModel = viewResult.Model as OversiktAlleInnmeldingerSaksBViewModel;
        Assert.IsNotNull(viewModel);
        Assert.AreEqual(2, viewModel.InnmeldingId.Count());
    }
}