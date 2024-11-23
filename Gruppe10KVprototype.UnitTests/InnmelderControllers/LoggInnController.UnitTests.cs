/*using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;
using Gruppe10KVprototype.Controllers.InnmelderControllers;
using ViewModels;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Gruppe10KVprototype.Tests.Controllers.InnmelderControllers
{
    [TestClass]
    public class LoggInnControllerTests
    {
        private Mock<SignInManager<IdentityUser>> _mockSignInManager;
        private Mock<ILogger<LoggInnController>> _mockLogger;
        private LoggInnController _controller;

        [TestInitialize]
        public void TestInitialize()
        {
            // Mock dependencies for SignInManager<IdentityUser>
            var mockUserStore = new Mock<IUserStore<IdentityUser>>();  // Mock the IUserStore
            var mockOptions = new Mock<IOptions<IdentityOptions>>();  // Mock the IOptions<IdentityOptions>
            var mockLoggerFactory = new Mock<ILoggerFactory>();  // Mock the ILoggerFactory
            var mockSignInManagerLogger = new Mock<ILogger<SignInManager<IdentityUser>>>();  // Mock the logger for SignInManager

            // Create a mock of SignInManager<IdentityUser> using the required dependencies
            _mockSignInManager = new Mock<SignInManager<IdentityUser>>(
                mockUserStore.Object,  // Pass the mocked IUserStore
                mockLoggerFactory.Object,  // Pass the mocked ILoggerFactory
                mockOptions.Object,  // Pass the mocked IOptions<IdentityOptions>
                null,  // IUserConfirmation<IdentityUser> - null for unit tests
                null,  // IAuthenticationSchemeProvider - null for unit tests
                null,  // IEmailSender - null for unit tests
                mockSignInManagerLogger.Object  // Pass the mocked ILogger<SignInManager<IdentityUser>>
            );

            // Mock PasswordSignInAsync to simulate failed login
            _mockSignInManager.Setup(m => m.PasswordSignInAsync(It.IsAny<string>(), It.IsAny<string>(), false, false))
                .ReturnsAsync(Microsoft.AspNetCore.Identity.SignInResult.Failed);  // Use the fully qualified SignInResult.Failed

            // Create the controller with the mocked SignInManager and ILoggerFactory
            _controller = new LoggInnController(_mockSignInManager.Object, mockLoggerFactory.Object);
        }





        [TestMethod]
        [Description("Tester at LoggInn-metoden returnerer korrekt view ved ugyldig modell")]
        public void LoggInn_InvalidModel_ReturnsView()
        {
            // Arrange - Create an invalid model (e.g., missing required fields)
            var model = new LoggInnViewModel
            {
                Email = "",
                Password = ""
            };

            // Add error to ModelState to simulate invalid model state
            _controller.ModelState.AddModelError("Email", "Email is required");

            // Act - Call LoggInn method
            var result = _controller.LoggInn(model).Result as ViewResult;

            // Assert - Verify that the result is a ViewResult (the view is returned when the model is invalid)
            Assert.IsNotNull(result);
            Assert.AreEqual("LoggInn", result.ViewName);
        }

        [TestMethod]
        [Description("Tester at LoggInn-metoden returnerer redirect ved vellykket innlogging")]
        public async Task LoggInn_SuccessfulLogin_RedirectsToLandingsSide()
        {
            // Arrange - Create a valid model
            var model = new LoggInnViewModel
            {
                Email = "user@example.com",
                Password = "Password123"
            };

            // Mock the SignInManager behavior for a successful login
            _mockSignInManager.Setup(m => m.PasswordSignInAsync(model.Email, model.Password, false, false))
                .ReturnsAsync(Microsoft.AspNetCore.Identity.SignInResult.Success); // Fully qualified SignInResult

            // Act - Call LoggInn method
            var result = await _controller.LoggInn(model) as RedirectToActionResult;

            // Assert - Verify that the result is a redirect to "LandingsSide" action
            Assert.IsNotNull(result);
            Assert.AreEqual("LandingsSide", result.ActionName);
            Assert.AreEqual("LandingsSide", result.ControllerName);
        }

        [TestMethod]
        [Description("Tester at LoggInn-metoden returnerer View ved feil brukernavn eller passord")]
        public async Task LoggInn_FailedLogin_ReturnsViewWithError()
        {
            // Arrange - Create a valid model
            var model = new LoggInnViewModel
            {
                Email = "user@example.com",
                Password = "WrongPassword"
            };

            // Mock the SignInManager behavior for a failed login
            _mockSignInManager.Setup(m => m.PasswordSignInAsync(model.Email, model.Password, false, false))
                .ReturnsAsync(Microsoft.AspNetCore.Identity.SignInResult.Failed); // Fully qualified SignInResult

            // Act - Call LoggInn method
            var result = await _controller.LoggInn(model) as ViewResult;

            // Assert - Verify that the result is a ViewResult (the view is returned on failure)
            Assert.IsNotNull(result);
            Assert.AreEqual("LoggInn", result.ViewName);

            // Assert - Verify that the error message is added to ModelState
            Assert.IsTrue(_controller.ModelState.ContainsKey(string.Empty));
            Assert.AreEqual("Feil brukernavn eller passord", _controller.ModelState[string.Empty].Errors[0].ErrorMessage);
        }
    }
}*/
