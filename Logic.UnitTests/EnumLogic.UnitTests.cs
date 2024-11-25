using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Interface;
using LogicInterfaces;

namespace Logic.Tests
{
    [TestClass]
    public class EnumLogicTests
    {
        private Mock<IInnmeldingRepository> _innmeldingRepositoryMock;
        private EnumLogic _enumLogic;

        // Test data
        private const string RAW_STATUS_VALUES = "('ny','ikke_påbegynt','under_behandling')";
        private const string RAW_PRIORITET_VALUES = "('lav','normal','høy')";
        private const string RAW_KART_TYPE_VALUES = "('standard','topografisk','ortofoto')";
        private const string RAW_INNMELDER_TYPE_VALUES = "('privat','offentlig','kommersiell')";

        [TestInitialize]
        public void Initialize()
        {
            _innmeldingRepositoryMock = new Mock<IInnmeldingRepository>();
            _enumLogic = new EnumLogic(_innmeldingRepositoryMock.Object);

            // Setup default repository responses
            _innmeldingRepositoryMock.Setup(x => x.GetStatusEnumValuesAsync())
                .ReturnsAsync(RAW_STATUS_VALUES);
            _innmeldingRepositoryMock.Setup(x => x.GetPrioritetEnumValuesAsync())
                .ReturnsAsync(RAW_PRIORITET_VALUES);
            _innmeldingRepositoryMock.Setup(x => x.GetKartTypeEnumValuesAsync())
                .ReturnsAsync(RAW_KART_TYPE_VALUES);
            _innmeldingRepositoryMock.Setup(x => x.GetInnmelderTypeEnumValuesAsync())
                .ReturnsAsync(RAW_INNMELDER_TYPE_VALUES);
        }

        #region Format Conversion Tests
        [TestMethod]
        [Description("Tester konvertering fra visningsformat til databaseformat")]
        [DataRow("Under behandling", "under_behandling")]
        [DataRow("Ikke påbegynt", "ikke_påbegynt")]
        [DataRow("Ny", "ny")]
        [DataRow("", "")]
        [DataRow(null, "")]
        public void ConvertToDbFormat_ConvertsCorrectly(string input, string expected)
        {
            // Act
            var result = _enumLogic.ConvertToDbFormat(input);

            // Assert
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        [Description("Tester konvertering fra databaseformat til visningsformat")]
        [DataRow("under_behandling", "Under behandling")]
        [DataRow("ikke_påbegynt", "Ikke påbegynt")]
        [DataRow("ny", "Ny")]
        [DataRow("", "")]
        [DataRow(null, "")]
        public void ConvertToDisplayFormat_ConvertsCorrectly(string input, string expected)
        {
            // Act
            var result = _enumLogic.ConvertToDisplayFormat(input);

            // Assert
            Assert.AreEqual(expected, result);
        }
        #endregion

        #region Get Formatted Values Tests
        [TestMethod]
        public async Task GetFormattedStatusEnumValuesAsync_ReturnsFormattedValues()
        {
            // Act
            var result = await _enumLogic.GetFormattedStatusEnumValuesAsync();

            // Assert
            CollectionAssert.AreEqual(
                new[] { "Ny", "Ikke påbegynt", "Under behandling" },
                result.ToList()
            );
            _innmeldingRepositoryMock.Verify(x => x.GetStatusEnumValuesAsync(), Times.Once);
        }

        [TestMethod]
        public async Task GetFormattedPrioritetEnumValuesAsync_ReturnsFormattedValues()
        {
            // Act
            var result = await _enumLogic.GetFormattedPrioritetEnumValuesAsync();

            // Assert
            CollectionAssert.AreEqual(
                new[] { "Lav", "Normal", "Høy" },
                result.ToList()
            );
            _innmeldingRepositoryMock.Verify(x => x.GetPrioritetEnumValuesAsync(), Times.Once);
        }

        [TestMethod]
        public async Task GetFormattedKartTypeEnumValuesAsync_ReturnsFormattedValues()
        {
            // Act
            var result = await _enumLogic.GetFormattedKartTypeEnumValuesAsync();

            // Assert
            CollectionAssert.AreEqual(
                new[] { "Standard", "Topografisk", "Ortofoto" },
                result.ToList()
            );
            _innmeldingRepositoryMock.Verify(x => x.GetKartTypeEnumValuesAsync(), Times.Once);
        }

        [TestMethod]
        public async Task GetFormattedInnmelderTypeEnumValuesAsync_ReturnsFormattedValues()
        {
            // Act
            var result = await _enumLogic.GetFormattedInnmelderTypeEnumValuesAsync();

            // Assert
            CollectionAssert.AreEqual(
                new[] { "Privat", "Offentlig", "Kommersiell" },
                result.ToList()
            );
            _innmeldingRepositoryMock.Verify(x => x.GetInnmelderTypeEnumValuesAsync(), Times.Once);
        }
        #endregion

        #region Validation Tests
        [TestMethod]
        [Description("Tester validering av status-verdier i forskjellige formater")]
        [DataRow("under_behandling", true)]
        [DataRow("Under behandling", true)]
        [DataRow("invalid_status", false)]
        [DataRow("", false)]
        [DataRow(null, false)]
        public async Task ValidateStatusValueAsync_ValidatesCorrectly(string input, bool expected)
        {
            // Act
            var result = await _enumLogic.ValidateStatusValueAsync(input);

            // Assert
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        [Description("Tester validering av prioritet-verdier")]
        [DataRow("høy", true)]
        [DataRow("Høy", true)]
        [DataRow("invalid_priority", false)]
        [DataRow("", false)]
        [DataRow(null, false)]
        public async Task ValidatePrioritetValueAsync_ValidatesCorrectly(string input, bool expected)
        {
            // Act
            var result = await _enumLogic.ValidatePrioritetValueAsync(input);

            // Assert
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        [Description("Tester validering av karttype-verdier")]
        [DataRow("standard", true)]
        [DataRow("Standard", true)]
        [DataRow("invalid_type", false)]
        [DataRow("", false)]
        [DataRow(null, false)]
        public async Task ValidateKartTypeValueAsync_ValidatesCorrectly(string input, bool expected)
        {
            // Act
            var result = await _enumLogic.ValidateKartTypeValueAsync(input);

            // Assert
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        [Description("Tester validering av innmeldertype-verdier")]
        [DataRow("privat", true)]
        [DataRow("Privat", true)]
        [DataRow("invalid_type", false)]
        [DataRow("", false)]
        [DataRow(null, false)]
        public async Task ValidateInnmelderTypeValueAsync_ValidatesCorrectly(string input, bool expected)
        {
            // Act
            var result = await _enumLogic.ValidateInnmelderTypeValueAsync(input);

            // Assert
            Assert.AreEqual(expected, result);
        }
        #endregion

        #region Error Handling Tests
        [TestMethod]
        public async Task GetFormattedStatusEnumValuesAsync_HandlesEmptyDatabaseResponse()
        {
            // Arrange
            _innmeldingRepositoryMock.Setup(x => x.GetStatusEnumValuesAsync())
                .ReturnsAsync(string.Empty);

            // Act
            var result = await _enumLogic.GetFormattedStatusEnumValuesAsync();

            // Assert
            Assert.IsFalse(result.Any());
        }

        [TestMethod]
        public async Task GetFormattedStatusEnumValuesAsync_HandlesNullDatabaseResponse()
        {
            // Arrange
            _innmeldingRepositoryMock.Setup(x => x.GetStatusEnumValuesAsync())
                .ReturnsAsync((string)null);

            // Act
            var result = await _enumLogic.GetFormattedStatusEnumValuesAsync();

            // Assert
            Assert.IsFalse(result.Any());
        }

        [TestMethod]
        [Description("Tester håndtering av ulike ugyldige formater")]
        public async Task GetFormattedStatusEnumValuesAsync_HandlesInvalidFormat()
        {
            // Arrange
            _innmeldingRepositoryMock.Setup(x => x.GetStatusEnumValuesAsync())
                .ReturnsAsync("('invalid','format','string')");

            // Act
            var result = await _enumLogic.GetFormattedStatusEnumValuesAsync();

            // Assert
            CollectionAssert.AreEqual(
                new[] { "Invalid", "Format", "String" },
                result.ToList()
            );
        }
        #endregion

        #region Repository Interaction Tests
        [TestMethod]
        public async Task EnumMethods_CallRepositoryOnce()
        {
            // Act
            await _enumLogic.GetFormattedStatusEnumValuesAsync();
            await _enumLogic.ValidateStatusValueAsync("ny");

            // Assert
            _innmeldingRepositoryMock.Verify(x => x.GetStatusEnumValuesAsync(), Times.Exactly(2));
        }
        #endregion
    }
}