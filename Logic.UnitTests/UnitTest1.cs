using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Text.Json;
using System.Threading.Tasks;
using Models.Entities;
using Models.Models;
using Models.Exceptions;
using Interface;
using Logic;

[TestClass]
public class InnmeldingLogicTests
{
    private Mock<ITransaksjonsRepository> _transaksjonsRepositoryMock;
    private Mock<IGeometriRepository> _geometriRepositoryMock;
    private InnmeldingLogic _logic;

    [TestInitialize]
    public void Initialize()
    {
        _transaksjonsRepositoryMock = new Mock<ITransaksjonsRepository>();
        _geometriRepositoryMock = new Mock<IGeometriRepository>();
        _logic = new InnmeldingLogic(_transaksjonsRepositoryMock.Object, _geometriRepositoryMock.Object);
    }

    [TestMethod]
    [DataRow("test@example.com", true)]
    [DataRow("invalid-email", false)]
    [DataRow("", false)]
    public async Task ValidereOgLagreNyInnmelding_SjekkUlikeEpostFormater(string epost, bool shouldBeValid)
    {
        // Arrange
        var innmelding = new InnmeldingModel { Tittel = "Test", Beskrivelse = "Test" };
        var geometri = new Geometri { GeometriGeoJson = "{ \"type\": \"Point\", \"coordinates\": [10.0, 60.0] }" };

        if (!shouldBeValid)
        {
            // Act & Assert
            await Assert.ThrowsExceptionAsync<ForretningsRegelExceptionModel>(async () => 
                await _logic.ValidereOgLagreNyInnmelding(innmelding, geometri, epost));
        }
        else
        {
            // Arrange
            _transaksjonsRepositoryMock.Setup(x => x.LagreKomplettInnmeldingAsync(
                    It.IsAny<string>(), It.IsAny<InnmeldingModel>(), It.IsAny<Geometri>()))
                .ReturnsAsync(true);

            // Act
            var result = await _logic.ValidereOgLagreNyInnmelding(innmelding, geometri, epost);

            // Assert
            Assert.IsTrue(result);
        }
    }
    
    [TestMethod]
    [DataRow("")]
    [DataRow(" ")]
    public async Task ValiderInnmeldingData_UgyldigTittel_KasterException(string tittel)
    {
        // Arrange
        var innmelding = new InnmeldingModel { Tittel = tittel, Beskrivelse = "Test" };

        // Act & Assert
        await Assert.ThrowsExceptionAsync<ForretningsRegelExceptionModel>(async () => 
            await _logic.ValiderInnmeldingData(innmelding));
    }

    [TestMethod]
    public async Task ValiderInnmeldingData_TittelForLang_KasterException()
    {
        // Arrange
        var innmelding = new InnmeldingModel 
        { 
            Tittel = new string('a', 101),
            Beskrivelse = "Test" 
        };

        // Act & Assert
        await Assert.ThrowsExceptionAsync<ForretningsRegelExceptionModel>(async () => 
            await _logic.ValiderInnmeldingData(innmelding));
    }

    [TestMethod]
    [DataRow("")]
    [DataRow(" ")]
    public async Task ValiderInnmeldingData_UgyldigBeskrivelse_KasterException(string beskrivelse)
    {
        // Arrange
        var innmelding = new InnmeldingModel { Tittel = "Test", Beskrivelse = beskrivelse };

        // Act & Assert
        await Assert.ThrowsExceptionAsync<ForretningsRegelExceptionModel>(async () => 
            await _logic.ValiderInnmeldingData(innmelding));
    }
    
    [TestMethod]
    [DataRow("{ \"type\": \"Point\", \"coordinates\": [10.0, 60.0] }", true)]
    [DataRow("{ \"type\": \"Feature\", \"geometry\": { \"type\": \"Point\", \"coordinates\": [10.0, 60.0] } }", true)]
    [DataRow("{ \"type\": \"Invalid\", \"coordinates\": [10.0, 60.0] }", false)]
    [DataRow("{ \"coordinates\": [10.0, 60.0] }", false)]
    [DataRow("Invalid JSON", false)]
    public async Task ValidereGeometriDataForOppdatering_SjekkUlikeGeoJsonFormater(string geoJson, bool shouldBeValid)
    {
        // Arrange
        var innmeldingId = 1;
        var geometri = new Geometri { GeometriGeoJson = geoJson };

        _geometriRepositoryMock.Setup(x => x.GetGeometriByInnmeldingIdAsync(innmeldingId))
            .ReturnsAsync(new Geometri());

        if (!shouldBeValid)
        {
            // Act & Assert
            await Assert.ThrowsExceptionAsync<ForretningsRegelExceptionModel>(async () => 
                await _logic.ValidereGeometriDataForOppdatering(innmeldingId, geometri));
        }
        else
        {
            // Act
            var result = await _logic.ValidereGeometriDataForOppdatering(innmeldingId, geometri);

            // Assert
            Assert.IsTrue(result);
        }
    }

    [TestMethod]
    public async Task ValidereOgLagreNyInnmelding_LagringsfeilerIRepository_KasterException()
    {
        // Arrange
        var innmelding = new InnmeldingModel { Tittel = "Test", Beskrivelse = "Test" };
        var geometri = new Geometri { GeometriGeoJson = "{ \"type\": \"Point\", \"coordinates\": [10.0, 60.0] }" };
        var epost = "test@example.com";

        _transaksjonsRepositoryMock.Setup(x => x.LagreKomplettInnmeldingAsync(
                It.IsAny<string>(), It.IsAny<InnmeldingModel>(), It.IsAny<Geometri>()))
            .ThrowsAsync(new Exception("Database error"));

        // Act & Assert
        await Assert.ThrowsExceptionAsync<ForretningsRegelExceptionModel>(async () => 
            await _logic.ValidereOgLagreNyInnmelding(innmelding, geometri, epost));
    }
}
