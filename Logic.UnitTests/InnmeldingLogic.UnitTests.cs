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

namespace InnmeldingLogic.UnitTests
{
    [TestClass]
    public class InnmeldingLogicTests
    {
        private Mock<ITransaksjonsRepository> _transaksjonsRepositoryMock;
        private Mock<IGeometriRepository> _geometriRepositoryMock;
        private Logic.InnmeldingLogic _logic;

        [TestInitialize]
        public void Initialize()
        {
            _transaksjonsRepositoryMock = new Mock<ITransaksjonsRepository>();
            _geometriRepositoryMock = new Mock<IGeometriRepository>();
            _logic = new Logic.InnmeldingLogic(_transaksjonsRepositoryMock.Object, _geometriRepositoryMock.Object);
        }

        [TestMethod]
        [Description("Tester validering av e-postadresser ved innmelding")]
        [DataRow("test@example.com", true)]
        [DataRow("invalid-email", false)]
        [DataRow("", false)]
        public async Task ValidereOgLagreNyInnmelding_SjekkUlikeEpostFormater_KasterException(string epost,
            bool shouldBeValid)
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
        [Description("Tester at tom tittel gir feilmelding")]
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
        [Description("Tester at tittel over 100 tegn gir feilmelding")]
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
        [Description("Tester at tom beskrivelse gir feilmelding")]
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
        [Description("Tester validering av geometri-data med ulike GeoJSON-formater")]
        [DataRow("{ \"type\": \"Point\", \"coordinates\": [10.0, 60.0] }", true, "Gyldig Point GeoJSON")]
        [DataRow("{ \"type\": \"Feature\", \"geometry\": { \"type\": \"Point\", \"coordinates\": [10.0, 60.0] } }", 
            true, "Gyldig Feature GeoJSON")]
        [DataRow("{ \"type\": \"Invalid\", \"coordinates\": [10.0, 60.0] }", false, "Ugyldig type")]
        [DataRow("{ \"coordinates\": [10.0, 60.0] }", false, "Manglende type")]
        [DataRow("Invalid JSON", false, "Ugyldig JSON-format")]
        [DataRow(null, false, "Null GeoJSON")]
        [DataRow("", false, "Tom GeoJSON")]
        public async Task ValidereGeometriData_MedUlikeFormater_ValidererKorrekt(
            string geoJson, 
            bool shouldBeValid,
            string testScenario)
        {
            // Arrange
            var innmeldingId = 1;
            var geometri = new Geometri { GeometriGeoJson = geoJson };

            _geometriRepositoryMock.Setup(x => x.GetGeometriByInnmeldingIdAsync(innmeldingId))
                .ReturnsAsync(new Geometri());

            if (!shouldBeValid)
            {
                // Act & Assert
                var exception = await Assert.ThrowsExceptionAsync<ForretningsRegelExceptionModel>(
                    async () => await _logic.ValidereGeometriDataForOppdatering(innmeldingId, geometri));
        
                Assert.IsNotNull(exception, $"Scenario '{testScenario}' skulle kaste en exception");
            }
            else
            {
                // Act
                var result = await _logic.ValidereGeometriDataForOppdatering(innmeldingId, geometri);

                // Assert
                Assert.IsTrue(result, $"Scenario '{testScenario}' skulle returnere true");
                _geometriRepositoryMock.Verify(x => x.GetGeometriByInnmeldingIdAsync(innmeldingId), 
                    Times.Once, 
                    "Repository skulle kalles én gang for gyldig data");
            }
        }
        
        [TestMethod]
        [Description("Tester at databasefeil ved lagring håndteres korrekt")]
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

}