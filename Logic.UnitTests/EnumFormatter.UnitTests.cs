using Microsoft.VisualStudio.TestTools.UnitTesting;
using Logic;

namespace Logic.Tests
{
    [TestClass]
    public class EnumFormatterTests
    {
        [TestMethod]
        [Description("ToNormalText should convert underscore-separated uppercase text to proper case")]
        public void ToNormalText_WithValidEnumValue_ReturnsFormattedString()
        {
            // Arrange
            var input = "UNDER_BEHANDLING";

            // Act
            var result = EnumFormatter.ToNormalText(input);

            // Assert
            Assert.AreEqual("Under behandling", result);
        }

        [TestMethod]
        [Description("ToNormalText should handle single word input")]
        public void ToNormalText_WithSingleWord_ReturnsCapitalizedWord()
        {
            // Arrange
            var input = "AKTIV";

            // Act
            var result = EnumFormatter.ToNormalText(input);

            // Assert
            Assert.AreEqual("Aktiv", result);
        }

        [TestMethod]
        [Description("ToNormalText should return empty string for null input")]
        public void ToNormalText_WithNull_ReturnsEmptyString()
        {
            // Act
            var result = EnumFormatter.ToNormalText(null);

            // Assert
            Assert.AreEqual(string.Empty, result);
        }

        [TestMethod]
        [Description("ToNormalText should return empty string for empty input")]
        public void ToNormalText_WithEmptyString_ReturnsEmptyString()
        {
            // Act
            var result = EnumFormatter.ToNormalText("");

            // Assert
            Assert.AreEqual(string.Empty, result);
        }

        [TestMethod]
        [Description("ToNormalText should handle multiple underscores")]
        public void ToNormalText_WithMultipleUnderscores_ReturnsFormattedString()
        {
            // Arrange
            var input = "VERY_LONG_ENUM_VALUE";

            // Act
            var result = EnumFormatter.ToNormalText(input);

            // Assert
            Assert.AreEqual("Very long enum value", result);
        }

        [TestMethod]
        [Description("ToNormalText should handle input with trailing underscore")]
        public void ToNormalText_WithTrailingUnderscore_ReturnsFormattedString()
        {
            // Arrange
            var input = "STATUS_ACTIVE_";

            // Act
            var result = EnumFormatter.ToNormalText(input);

            // Assert
            Assert.AreEqual("Status active", result);
        }
    }

    [TestClass]
    public class StringExtensionsTests
    {
        [TestMethod]
        [Description("ReplaceFirstCharToUpper should capitalize first character")]
        public void ReplaceFirstCharToUpper_WithLowercaseString_CapitalizesFirstChar()
        {
            // Arrange
            var input = "test";

            // Act
            var result = input.ReplaceFirstCharToUpper();

            // Assert
            Assert.AreEqual("Test", result);
        }

        [TestMethod]
        [Description("ReplaceFirstCharToUpper should handle single character")]
        public void ReplaceFirstCharToUpper_WithSingleChar_ReturnsCapitalizedChar()
        {
            // Arrange
            var input = "a";

            // Act
            var result = input.ReplaceFirstCharToUpper();

            // Assert
            Assert.AreEqual("A", result);
        }

        [TestMethod]
        [Description("ReplaceFirstCharToUpper should return null for null input")]
        public void ReplaceFirstCharToUpper_WithNull_ReturnsNull()
        {
            // Arrange
            string input = null;

            // Act
            var result = input.ReplaceFirstCharToUpper();

            // Assert
            Assert.IsNull(result);
        }

        [TestMethod]
        [Description("ReplaceFirstCharToUpper should return empty string for empty input")]
        public void ReplaceFirstCharToUpper_WithEmptyString_ReturnsEmptyString()
        {
            // Arrange
            var input = "";

            // Act
            var result = input.ReplaceFirstCharToUpper();

            // Assert
            Assert.AreEqual(string.Empty, result);
        }

        [TestMethod]
        [Description("ReplaceFirstCharToUpper should maintain case of other characters")]
        public void ReplaceFirstCharToUpper_WithMixedCase_OnlyChangesFirstChar()
        {
            // Arrange
            var input = "testINGString";

            // Act
            var result = input.ReplaceFirstCharToUpper();

            // Assert
            Assert.AreEqual("TestINGString", result);
        }
    }
}