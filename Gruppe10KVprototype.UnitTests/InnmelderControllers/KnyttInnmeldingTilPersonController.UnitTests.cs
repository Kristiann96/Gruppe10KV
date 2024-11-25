using System;
using LogicInterfaces;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Models.Entities;
using Models.Exceptions;
using Models.Models;
using ViewModels;
using Gruppe10KVprototype.Controllers.InnmelderControllers;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Controller.UnitTests
{
    [TestClass]
    public class KnyttInnmeldingTilPersonControllerTester
    {
        private KnyttInnmeldingTilPersonController _controller;
        private Mock<IInnmeldingLogic> _mockInnmeldingLogic;
        private Mock<IHttpContextAccessor> _mockHttpContextAccessor;

        [TestInitialize]
        public void SetUp()
        {
            _mockInnmeldingLogic = new Mock<IInnmeldingLogic>();
            _controller = new KnyttInnmeldingTilPersonController(_mockInnmeldingLogic.Object);

            // Mock HttpContext to simulate a logged-in user
            var claims = new[]
            {
                new Claim(ClaimTypes.Name, "testuser@example.com") // Simulate logged-in user
            };
            var identity = new ClaimsIdentity(claims, "TestAuthentication");
            var principal = new ClaimsPrincipal(identity);

            _controller.ControllerContext.HttpContext = new DefaultHttpContext
            {
                User = principal
            };
        }

        // Tester at GET-metoden KnyttInnmeldingTilPerson returnerer redirect når required fields er tomme eller null
        [TestMethod]
        [Description(
            "Tester at GET-metoden KnyttInnmeldingTilPerson returnerer redirect når required fields er tomme eller null")]
        public void KnyttInnmeldingTilPerson_FieldsMissing_Redirigerer()
        {
            var model = new KnyttInnmeldingTilPersonViewModel
            {
                GeometriGeoJson = null,
                Tittel = "",
                Beskrivelse = ""
            };

            var result = _controller.KnyttInnmeldingTilPerson(model).Result as RedirectToActionResult;

            Assert.IsNotNull(result);
            Assert.AreEqual("KartfeilMarkering", result.ActionName);
            Assert.AreEqual("KartfeilMarkering", result.ControllerName);
        }

        // Tester at GET-metoden KnyttInnmeldingTilPerson returnerer View med riktig modell
        [TestMethod]
        [Description("Tester at GET-metoden KnyttInnmeldingTilPerson returnerer View med riktig modell")]
        public void KnyttInnmeldingTilPerson_FieldsProvided_ReturnererViewMedModel()
        {
            var model = new KnyttInnmeldingTilPersonViewModel
            {
                GeometriGeoJson = "{\"type\":\"Point\",\"coordinates\":[10.0,60.0]}",
                Tittel = "Test Tittel",
                Beskrivelse = "Test Beskrivelse"
            };

            var result = _controller.KnyttInnmeldingTilPerson(model).Result as ViewResult;

            Assert.IsNotNull(result);
            var returnedModel = result.Model as KnyttInnmeldingTilPersonViewModel;
            Assert.IsNotNull(returnedModel);
            Assert.AreEqual(model.GeometriGeoJson, returnedModel.GeometriGeoJson);
            Assert.AreEqual(model.Tittel, returnedModel.Tittel);
            Assert.AreEqual(model.Beskrivelse, returnedModel.Beskrivelse);
        }

        // Tester at POST-metoden LagreKnyttInnmeldingTilPerson returnerer View ved ugyldig modell
        [TestMethod]
        [Description("Tester at POST-metoden LagreKnyttInnmeldingTilPerson returnerer View ved ugyldig modell")]
        public async Task LagreKnyttInnmeldingTilPerson_InvalidModel_ReturnererView()
        {
            var model = new KnyttInnmeldingTilPersonViewModel
            {
                GeometriGeoJson = "{\"type\":\"Point\",\"coordinates\":[10.0,60.0]}",
                Tittel = "", // Tittel er tom, som gir en valideringsfeil
                Beskrivelse = "",
                Epost = "test@example.com"
            };

            _controller.ModelState.AddModelError("Tittel", "Tittel er påkrevd");

            var result = await _controller.LagreKnyttInnmeldingTilPerson(model) as ViewResult;

            Assert.IsNotNull(result);
            var returnedModel = result.Model as KnyttInnmeldingTilPersonViewModel;
            Assert.IsNotNull(returnedModel);
            Assert.AreEqual(model.Tittel, returnedModel.Tittel);
            Assert.AreEqual(model.Beskrivelse, returnedModel.Beskrivelse);
            Assert.AreEqual(model.Epost, returnedModel.Epost);
        }

        // Tester at POST-metoden LagreKnyttInnmeldingTilPerson returnerer LandingsSide ved gyldig modell
        [TestMethod]
        [Description("Tester at POST-metoden LagreKnyttInnmeldingTilPerson returnerer LandingsSide ved gyldig modell")]
        public async Task LagreKnyttInnmeldingTilPerson_ValidModel_RedirigererTilLandingsSide()
        {
            var model = new KnyttInnmeldingTilPersonViewModel
            {
                GeometriGeoJson = "{\"type\":\"Point\",\"coordinates\":[10.0,60.0]}",
                Tittel = "Test Tittel",
                Beskrivelse = "Test Beskrivelse",
                ErNodEtatKritisk = true, // Prioritet skal være "høy"
                Epost = "test@example.com"
            };

            _mockInnmeldingLogic
                .Setup(m => m.ValidereOgLagreNyInnmelding(It.IsAny<InnmeldingModel>(), It.IsAny<Geometri>(),
                    It.IsAny<string>(), It.IsAny<bool>()))
                .ReturnsAsync(true);

            var result = await _controller.LagreKnyttInnmeldingTilPerson(model) as RedirectToActionResult;

            Assert.IsNotNull(result);
            Assert.AreEqual("LandingsSide", result.ActionName);
            Assert.AreEqual("LandingsSide", result.ControllerName);
        }

        // Tester at POST-metoden LagreKnyttInnmeldingTilPerson håndterer ForretningsRegelExceptionModel
        [TestMethod]
        [Description("Tester at POST-metoden LagreKnyttInnmeldingTilPerson håndterer ForretningsRegelExceptionModel")]
        public async Task LagreKnyttInnmeldingTilPerson_ForretningsRegelException_HåndtererFeil()
        {
            var model = new KnyttInnmeldingTilPersonViewModel
            {
                GeometriGeoJson = "{\"type\":\"Point\",\"coordinates\":[10.0,60.0]}",
                Tittel = "Test Tittel",
                Beskrivelse = "Test Beskrivelse",
                Epost = "test@example.com"
            };

            _mockInnmeldingLogic
                .Setup(m => m.ValidereOgLagreNyInnmelding(It.IsAny<InnmeldingModel>(), It.IsAny<Geometri>(),
                    It.IsAny<string>(), It.IsAny<bool>()))
                .Throws(new ForretningsRegelExceptionModel("Forretningsregel-feil"));

            var result = await _controller.LagreKnyttInnmeldingTilPerson(model) as ViewResult;

            Assert.IsNotNull(result);
            var returnedModel = result.Model as KnyttInnmeldingTilPersonViewModel;
            Assert.IsNotNull(returnedModel);
            Assert.AreEqual(model, returnedModel);
            Assert.IsTrue(_controller.ModelState.ContainsKey(""));
            Assert.AreEqual("Forretningsregel-feil", _controller.ModelState[""].Errors[0].ErrorMessage);
        }

        // Tester at POST-metoden LagreKnyttInnmeldingTilPerson håndterer uventet feil
        [TestMethod]
        [Description("Tester at POST-metoden LagreKnyttInnmeldingTilPerson håndterer uventet feil")]
        public async Task LagreKnyttInnmeldingTilPerson_UnexpectedException_HåndtererFeil()
        {
            var model = new KnyttInnmeldingTilPersonViewModel
            {
                GeometriGeoJson = "{\"type\":\"Point\",\"coordinates\":[10.0,60.0]}",
                Tittel = "Test Tittel",
                Beskrivelse = "Test Beskrivelse",
                Epost = "test@example.com"
            };

            _mockInnmeldingLogic
                .Setup(m => m.ValidereOgLagreNyInnmelding(It.IsAny<InnmeldingModel>(), It.IsAny<Geometri>(),
                    It.IsAny<string>(), It.IsAny<bool>()))
                .Throws(new Exception("Uventet feil"));

            var result = await _controller.LagreKnyttInnmeldingTilPerson(model) as ViewResult;

            Assert.IsNotNull(result);
            var returnedModel = result.Model as KnyttInnmeldingTilPersonViewModel;
            Assert.IsNotNull(returnedModel);
            Assert.AreEqual(model, returnedModel);
        }
    }
}