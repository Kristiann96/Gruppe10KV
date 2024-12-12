using Moq;
using Models.Entities;
using Models.Models;
using Models.Exceptions;
using Interface;

namespace Logic.UnitTests
{
    [TestClass]
    public class InnmeldingLogicTests
    {
        private Mock<ITransaksjonsRepository> _transaksjonsRepositoryMock;
        private Mock<IGeometriRepository> _geometriRepositoryMock;
        private InnmeldingLogic _logic;
        private const int TEST_INNMELDING_ID = 1;
        private const string VALID_GEOJSON = "{ \"type\": \"Point\", \"coordinates\": [10.0, 60.0] }";
        private const string INVALID_COORDINATES_GEOJSON = "{ \"type\": \"Point\", \"coordinates\": [0.0, 0.0] }";

        [TestInitialize]
        public void Initialize()
        {
            _transaksjonsRepositoryMock = new Mock<ITransaksjonsRepository>();
            _geometriRepositoryMock = new Mock<IGeometriRepository>();
            _logic = new InnmeldingLogic(
                _transaksjonsRepositoryMock.Object,
                _geometriRepositoryMock.Object);
        }

        #region ValidereOgLagreNyInnmelding Tests

        [TestMethod]
        [Description("Tester vellykket lagring av ny innmelding for innlogget bruker")]
        public async Task ValidereOgLagreNyInnmelding_InnloggetBruker_LagresSuksessfullt()
        {
            // Arrange
            var innmelding = CreateValidInnmelding();
            var geometri = CreateValidGeometri();
            var epost = "test@example.com";
            var erLoggetInn = true;

            _transaksjonsRepositoryMock.Setup(x => x.LagreKomplettInnmeldingInnloggetAsync(
                    It.IsAny<string>(), It.IsAny<InnmeldingModel>(), It.IsAny<Geometri>()))
                .ReturnsAsync(true);

            // Act
            var result = await _logic.ValidereOgLagreNyInnmelding(innmelding, geometri, epost, erLoggetInn);

            // Assert
            Assert.IsTrue(result);
            _transaksjonsRepositoryMock.Verify(x => x.LagreKomplettInnmeldingInnloggetAsync(
                epost, innmelding, geometri), Times.Once);
        }

        [TestMethod]
        [Description("Tester vellykket lagring av ny innmelding for ikke-innlogget bruker")]
        public async Task ValidereOgLagreNyInnmelding_IkkeInnloggetBruker_LagresSuksessfullt()
        {
            // Arrange
            var innmelding = CreateValidInnmelding();
            var geometri = CreateValidGeometri();
            var epost = "test@example.com";
            var erLoggetInn = false;

            _transaksjonsRepositoryMock.Setup(x => x.LagreKomplettInnmeldingAsync(
                    It.IsAny<int>(), It.IsAny<InnmeldingModel>(), It.IsAny<Geometri>()))
                .ReturnsAsync(true);

            // Act
            var result = await _logic.ValidereOgLagreNyInnmelding(innmelding, geometri, epost, erLoggetInn);

            // Assert
            Assert.IsTrue(result);
            _transaksjonsRepositoryMock.Verify(x => x.LagreKomplettInnmeldingAsync(
                epost, innmelding, geometri), Times.Once);
        }

        [TestMethod]
        [Description("Tester validering av e-postadresser")]
        [DataRow("test@example.com", true, true, DisplayName = "Gyldig e-post, innlogget")]
        [DataRow("test@example.com", false, true, DisplayName = "Gyldig e-post, ikke innlogget")]
        [DataRow("invalid-email", true, false, DisplayName = "Ugyldig e-post")]
        [DataRow("", true, false, DisplayName = "Tom e-post")]
        [DataRow(null, true, false, DisplayName = "Null e-post")]
        public async Task ValidereOgLagreNyInnmelding_ValiderEpost_ValidererKorrekt(
            string epost, bool erLoggetInn, bool shouldBeValid)
        {
            // Arrange
            var innmelding = CreateValidInnmelding();
            var geometri = CreateValidGeometri();

            if (shouldBeValid)
            {
                var setupMethod = erLoggetInn
                    ? _transaksjonsRepositoryMock.Setup(x => x.LagreKomplettInnmeldingInnloggetAsync(
                        It.IsAny<string>(), It.IsAny<InnmeldingModel>(), It.IsAny<Geometri>()))
                    : _transaksjonsRepositoryMock.Setup(x => x.LagreKomplettInnmeldingAsync(
                        It.IsAny<string>(), It.IsAny<InnmeldingModel>(), It.IsAny<Geometri>()));

                setupMethod.ReturnsAsync(true);

                // Act
                var result = await _logic.ValidereOgLagreNyInnmelding(
                    innmelding, geometri, epost, erLoggetInn);

                // Assert
                Assert.IsTrue(result);
            }
            else
            {
                // Act & Assert
                await Assert.ThrowsExceptionAsync<ForretningsRegelExceptionModel>(
                    async () => await _logic.ValidereOgLagreNyInnmelding(
                        innmelding, geometri, epost, erLoggetInn));
            }
        }

        #endregion

        #region ValiderInnmeldingData Tests

        [TestMethod]
        [Description("Tester validering av innmelding tittel")]
        [DataRow("", DisplayName = "Tom tittel")]
        [DataRow(" ", DisplayName = "Kun mellomrom")]
        [DataRow(null, DisplayName = "Null tittel")]
        public async Task ValiderInnmeldingData_UgyldigTittel_KasterException(string tittel)
        {
            // Arrange
            var innmelding = CreateValidInnmelding();
            innmelding.Tittel = tittel;

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
            var innmelding = CreateValidInnmelding();
            innmelding.Tittel = new string('a', 101);

            // Act & Assert
            var exception = await Assert.ThrowsExceptionAsync<ForretningsRegelExceptionModel>(
                async () => await _logic.ValiderInnmeldingData(innmelding));

            Assert.AreEqual("Tittel kan ikke være lengre enn 100 tegn", exception.Message);
        }

        #endregion

        #region ValidereGeometriData Tests

        [TestMethod]
        [Description("Tester validering av geometri med ugyldige koordinater")]
        public async Task ValidereGeometriDataForOppdatering_UgyldigeKoordinater_KasterException()
        {
            // Arrange
            var geometri = new Geometri { GeometriGeoJson = INVALID_COORDINATES_GEOJSON };
            _geometriRepositoryMock.Setup(x => x.GetGeometriByInnmeldingIdAsync(TEST_INNMELDING_ID))
                .ReturnsAsync(new Geometri());

            // Act & Assert
            var exception = await Assert.ThrowsExceptionAsync<ForretningsRegelExceptionModel>(
                async () => await _logic.ValidereGeometriDataForOppdatering(TEST_INNMELDING_ID, geometri));

            Assert.IsTrue(exception.Message.Contains("Koordinater må være innenfor Norge"));
        }

        [TestMethod]
        [Description("Tester validering av ugyldig GeoJSON format")]
        public async Task ValidereGeometriDataForOppdatering_UgyldigGeoJson_KasterException()
        {
            // Arrange
            var geometri = new Geometri { GeometriGeoJson = "invalid json" };
            _geometriRepositoryMock.Setup(x => x.GetGeometriByInnmeldingIdAsync(TEST_INNMELDING_ID))
                .ReturnsAsync(new Geometri());

            // Act & Assert
            var exception = await Assert.ThrowsExceptionAsync<ForretningsRegelExceptionModel>(
                async () => await _logic.ValidereGeometriDataForOppdatering(TEST_INNMELDING_ID, geometri));

            Assert.AreEqual("Ugyldig geometriformat. Vennligst prøv igjen.", exception.Message);
        }

        #endregion

        #region Helper Methods

        private InnmeldingModel CreateValidInnmelding()
        {
            return new InnmeldingModel
            {
                Tittel = "Test innmelding",
                Beskrivelse = "Test beskrivelse",
                Status = "NY",
                Prioritet = "NORMAL",
                KartType = "STANDARD",
                Innmeldingstidspunkt = DateTime.Now,
                SisteEndring = DateTime.Now
            };
        }

        private Geometri CreateValidGeometri()
        {
            return new Geometri
            {
                GeometriGeoJson = VALID_GEOJSON,
                InnmeldingId = TEST_INNMELDING_ID
            };
        }

        #endregion
    }
}