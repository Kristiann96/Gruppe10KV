using Microsoft.AspNetCore.Mvc;
using Gruppe10KVprototype.Controllers.InnmelderControllers;

namespace Controller.UnitTests
{
    [TestClass]
    public class LandingsSideControllerTests
    {
        private LandingsSideController _controller;

        [TestInitialize]
        public void TestInitialize()
        {
            // Arrange - Initialize the controller before each test
            _controller = new LandingsSideController();
        }

        [TestMethod]
        [Description("Tester at LandingsSide-metoden returnerer korrekt View")]
        public void LandingsSide_ReturnererView()
        {
            // Act - Call the LandingsSide action method
            var result = _controller.LandingsSide() as ViewResult;

            // Assert - Verify that the result is a ViewResult
            Assert.IsNotNull(result, "The result should be a ViewResult.");

            // Verify that the correct view is returned (should be the default view)
            Assert.IsNull(result.ViewName, "The ViewName should be null, using the default view.");
        }
    }
}