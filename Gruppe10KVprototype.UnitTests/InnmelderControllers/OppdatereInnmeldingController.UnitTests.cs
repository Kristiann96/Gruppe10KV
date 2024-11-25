using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Moq;
using ViewModels;
using ServicesInterfaces;
using Gruppe10KVprototype.Controllers.InnmelderControllers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Controller.UnitTests
{
    [TestClass]
    public class OppdatereInnmeldingControllerTests
    {
        private Mock<IOppdatereInnmeldingService> _serviceMock;
        private OppdatereInnmeldingController _controller;

        [TestInitialize]
        public void Setup()
        {
            _serviceMock = new Mock<IOppdatereInnmeldingService>();
            _controller = new OppdatereInnmeldingController(_serviceMock.Object);
        }
 
        [TestMethod]
        [Description("Tester vellykket henting av innmelding for oppdatering")]
        public async Task OppdatereInnmelding_ValidId_ReturnsViewWithModel()
        {
            // Arrange
            int innmeldingId = 1;
            var expectedModel = new OppdatereInnmeldingViewModel();
            _serviceMock.Setup(s => s.HentInnmeldingForOppdateringAsync(innmeldingId))
                .Returns(Task.FromResult(expectedModel));

            // Act
            var result = await _controller.OppdatereInnmelding(innmeldingId);

            // Assert
            Assert.IsInstanceOfType(result, typeof(ViewResult));
            var viewResult = (ViewResult)result;
            Assert.AreEqual(expectedModel, viewResult.Model);
        }
    }
}