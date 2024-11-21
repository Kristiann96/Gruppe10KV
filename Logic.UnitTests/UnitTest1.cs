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
}
