using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
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
        private const int TEST_INNMELDING_ID = 1;
        private const string VALID_GEOJSON = "{ \"type\": \"Point\", \"coordinates\": [10.0, 60.0] }";

        [TestInitialize]
        public void Initialize()
        {
            _transaksjonsRepositoryMock = new Mock<ITransaksjonsRepository>();
            _geometriRepositoryMock = new Mock<IGeometriRepository>();
            _logic = new Logic.InnmeldingLogic(_transaksjonsRepositoryMock.Object, _geometriRepositoryMock.Object);
        }

        #region ValidereOgLagreNyInnmelding Tests

        [TestMethod]
        [Description("Tester vellykket lagring av ny innmelding")]
        public async Task ValidereOgLagreNyInnmelding_GyldigeVerdier_LagresSuksessfullt()
        {
            // Arrange
            var innmelding = CreateValidInnmelding();
            var geometri = CreateValidGeometri();
            var epost = "test@example.com";

            _transaksjonsRepositoryMock.Setup(x => x.LagreKomplettInnmeldingAsync(
                    It.IsAny<string>(), It.IsAny<InnmeldingModel>(), It.IsAny<Geometri>()))
                .ReturnsAsync(true);

            // Act
            var result = await _logic.ValidereOgLagreNyInnmelding(innmelding, geometri, epost);

            // Assert
            Assert.IsTrue(result);
            _transaksjonsRepositoryMock.Verify(x => x.LagreKomplettInnmeldingAsync(
                epost, innmelding, geometri), Times.Once);
        }

        [TestMethod]
        [Description("Tester validering av e-postadresser")]
        [DataRow("test@example.com", true, DisplayName = "Gyldig e-post")]
        [DataRow("invalid-email", false, DisplayName = "Ugyldig e-post format")]
        [DataRow("", false, DisplayName = "Tom e-post")]
        public async Task ValidereOgLagreNyInnmelding_ValiderEpost_ValidererKorrekt(string epost, bool shouldBeValid)
        {
            // Arrange
            var innmelding = CreateValidInnmelding();
            var geometri = CreateValidGeometri();

            if (shouldBeValid)
            {
                _transaksjonsRepositoryMock.Setup(x => x.LagreKomplettInnmeldingAsync(
                        It.IsAny<string>(), It.IsAny<InnmeldingModel>(), It.IsAny<Geometri>()))
                    .ReturnsAsync(true);

                // Act
                var result = await _logic.ValidereOgLagreNyInnmelding(innmelding, geometri, epost);

                // Assert
                Assert.IsTrue(result);
            }
            else
            {
                // Act & Assert
                var exception = await Assert.ThrowsExceptionAsync<ForretningsRegelExceptionModel>(
                    async () => await _logic.ValidereOgLagreNyInnmelding(innmelding, geometri, epost));
                Assert.AreEqual("Ugyldig epost-format", exception.Message);
            }
        }

        [TestMethod]
        [Description("Tester at databasefeil ved lagring håndteres korrekt")]
        public async Task ValidereOgLagreNyInnmelding_DatabaseFeil_KasterForretningsRegelException()
        {
            // Arrange
            var innmelding = CreateValidInnmelding();
            var geometri = CreateValidGeometri();
            var epost = "test@example.com";

            _transaksjonsRepositoryMock.Setup(x => x.LagreKomplettInnmeldingAsync(
                    It.IsAny<string>(), It.IsAny<InnmeldingModel>(), It.IsAny<Geometri>()))
                .ThrowsAsync(new Exception("Database error"));

            // Act & Assert
            var exception = await Assert.ThrowsExceptionAsync<ForretningsRegelExceptionModel>(
                async () => await _logic.ValidereOgLagreNyInnmelding(innmelding, geometri, epost));
            
            Assert.AreEqual("Kunne ikke lagre innmeldingen. Vennligst prøv igjen senere.", exception.Message);
        }

        #endregion

        #region InnmeldingModel Validation Tests

        [TestMethod]
        [Description("Tester validering av tittel")]
        [DataRow("", DisplayName = "Tom tittel")]
        [DataRow(" ", DisplayName = "Kun mellomrom")]
        public async Task ValiderInnmeldingData_UgyldigTittel_KasterException(string tittel)
        {
            // Arrange
            var innmelding = new InnmeldingModel { Tittel = tittel, Beskrivelse = "Test" };

            // Act & Assert
            var exception = await Assert.ThrowsExceptionAsync<ForretningsRegelExceptionModel>(
                async () => await _logic.ValiderInnmeldingData(innmelding));
            
            Assert.AreEqual("Tittel må fylles ut", exception.Message);
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
            var exception = await Assert.ThrowsExceptionAsync<ForretningsRegelExceptionModel>(
                async () => await _logic.ValiderInnmeldingData(innmelding));
            
            Assert.AreEqual("Tittel kan ikke være lengre enn 100 tegn", exception.Message);
        }

        [TestMethod]
        [Description("Tester validering av beskrivelse")]
        [DataRow("", DisplayName = "Tom beskrivelse")]
        [DataRow(" ", DisplayName = "Kun mellomrom")]
        public async Task ValiderInnmeldingData_UgyldigBeskrivelse_KasterException(string beskrivelse)
        {
            // Arrange
            var innmelding = new InnmeldingModel { Tittel = "Test", Beskrivelse = beskrivelse };

            // Act & Assert
            var exception = await Assert.ThrowsExceptionAsync<ForretningsRegelExceptionModel>(
                async () => await _logic.ValiderInnmeldingData(innmelding));
            
            Assert.AreEqual("Beskrivelse må fylles ut", exception.Message);
        }

        #endregion

        #region Geometri Validation Tests

        [TestMethod]
        [Description("Tester at gyldig Point GeoJSON valideres korrekt")]
        public async Task ValidereGeometriData_GyldigPointGeoJson_ValidererUtenFeil()
        {
            // Arrange
            var geometri = new Geometri { GeometriGeoJson = VALID_GEOJSON };
            SetupGeometriRepository();

            // Act
            var result = await _logic.ValidereGeometriDataForOppdatering(TEST_INNMELDING_ID, geometri);

            // Assert
            Assert.IsTrue(result);
            VerifyRepositoryCall();
        }

        [TestMethod]
        [Description("Tester at gyldig Feature GeoJSON valideres korrekt")]
        public async Task ValidereGeometriData_GyldigFeatureGeoJson_ValidererUtenFeil()
        {
            // Arrange
            var geometri = new Geometri 
            { 
                GeometriGeoJson = "{ \"type\": \"Feature\", \"geometry\": { \"type\": \"Point\", \"coordinates\": [10.0, 60.0] } }" 
            };
            SetupGeometriRepository();

            // Act
            var result = await _logic.ValidereGeometriDataForOppdatering(TEST_INNMELDING_ID, geometri);

            // Assert
            Assert.IsTrue(result);
            VerifyRepositoryCall();
        }

        [TestMethod]
        [Description("Tester at ugyldig geometritype gir forventet feilmelding")]
        public async Task ValidereGeometriData_UgyldigGeometriType_KasterForretningsRegelException()
        {
            // Arrange
            var geometri = new Geometri 
            { 
                GeometriGeoJson = "{ \"type\": \"Invalid\", \"coordinates\": [10.0, 60.0] }" 
            };
            SetupGeometriRepository();

            // Act & Assert
            var exception = await Assert.ThrowsExceptionAsync<ForretningsRegelExceptionModel>(
                async () => await _logic.ValidereGeometriDataForOppdatering(TEST_INNMELDING_ID, geometri));
            
            Assert.IsTrue(exception.Message.Contains("ikke støttet"));
        }

        [TestMethod]
        [Description("Tester håndtering av null eller tom GeoJSON")]
        [DataRow(null, DisplayName = "Null GeoJSON")]
        [DataRow("", DisplayName = "Tom GeoJSON")]
        public async Task ValidereGeometriData_NullEllerTomGeoJson_KasterForretningsRegelException(string geoJson)
        {
            // Arrange
            var geometri = new Geometri { GeometriGeoJson = geoJson };
            SetupGeometriRepository();

            // Act & Assert
            var exception = await Assert.ThrowsExceptionAsync<ForretningsRegelExceptionModel>(
                async () => await _logic.ValidereGeometriDataForOppdatering(TEST_INNMELDING_ID, geometri));
            
            Assert.AreEqual("GeoJSON data mangler", exception.Message);
        }

        #endregion

        #region Helper Methods

        private InnmeldingModel CreateValidInnmelding()
        {
            return new InnmeldingModel 
            { 
                Tittel = "Test", 
                Beskrivelse = "Test beskrivelse" 
            };
        }

        private Geometri CreateValidGeometri()
        {
            return new Geometri 
            { 
                GeometriGeoJson = VALID_GEOJSON 
            };
        }

        private void SetupGeometriRepository()
        {
            _geometriRepositoryMock.Setup(x => x.GetGeometriByInnmeldingIdAsync(TEST_INNMELDING_ID))
                .ReturnsAsync(new Geometri());
        }

        private void VerifyRepositoryCall()
        {
            _geometriRepositoryMock.Verify(
                x => x.GetGeometriByInnmeldingIdAsync(TEST_INNMELDING_ID),
                Times.Once,
                "Repository skulle kalles én gang for gyldig data");
        }

        #endregion
    }
}