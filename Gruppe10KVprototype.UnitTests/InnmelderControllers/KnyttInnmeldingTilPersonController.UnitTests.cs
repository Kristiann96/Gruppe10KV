using LogicInterfaces;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Models.Entities;
using Models.Exceptions;
using ViewModels;
using Gruppe10KVprototype.Controllers.InnmelderControllers;
using Models.Models;

namespace Gruppe10KVprototype.Tests.Controllers
{
    [TestClass]
    public class KnyttInnmeldingTilPersonControllerTester
    {
        private KnyttInnmeldingTilPersonController _controller;
        private Mock<IInnmeldingLogic> _mockInnmeldingLogic;

        [TestInitialize]
        public void SetUp()
        {
            _mockInnmeldingLogic = new Mock<IInnmeldingLogic>();
            _controller = new KnyttInnmeldingTilPersonController(_mockInnmeldingLogic.Object);
        }

        [TestMethod]
        [Description("Tester at GET-metoden KnyttInnmeldingTilPerson returnerer redirect når required fields er tomme eller null")]
        public void KnyttInnmeldingTilPerson_FieldsMissing_Redirigerer()
        {
            var model = new KnyttInnmeldingTilPersonViewModel
            {
                GeometriGeoJson = null,
                Tittel = "",
                Beskrivelse = ""
            };

            var result = _controller.KnyttInnmeldingTilPerson(model) as RedirectToActionResult;

            Assert.IsNotNull(result);
            Assert.AreEqual("KartfeilMarkering", result.ActionName);
            Assert.AreEqual("KartfeilMarkering", result.ControllerName);
        }

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

            var result = _controller.KnyttInnmeldingTilPerson(model) as ViewResult;

            Assert.IsNotNull(result);
            var returnedModel = result.Model as KnyttInnmeldingTilPersonViewModel;
            Assert.IsNotNull(returnedModel);
            Assert.AreEqual(model.GeometriGeoJson, returnedModel.GeometriGeoJson);
            Assert.AreEqual(model.Tittel, returnedModel.Tittel);
            Assert.AreEqual(model.Beskrivelse, returnedModel.Beskrivelse);
        }

        [TestMethod]
        [Description("Tester at POST-metoden LagreKnyttInnmeldingTilPerson returnerer View ved ugyldig modell")]
        public async Task LagreKnyttInnmeldingTilPerson_InvalidModel_ReturnererView()
        {
            // Arrange - Lag en ugyldig modell
            var model = new KnyttInnmeldingTilPersonViewModel
            {
                GeometriGeoJson = "{\"type\":\"Point\",\"coordinates\":[10.0,60.0]}",
                Tittel = "", // Tittel er tom, som gir en valideringsfeil
                Beskrivelse = "", // Beskrivelse er tom, men dette er ikke en valideringsfeil i denne testen
                Epost = "test@example.com"
            };
    
            // Legg til en modelfeil for "Tittel" (krav for at modellen skal være gyldig)
            _controller.ModelState.AddModelError("Tittel", "Tittel er påkrevd");

            // Act - Kall på POST-metoden asynkront
            var result = await _controller.LagreKnyttInnmeldingTilPerson(model) as ViewResult;

            // Assert - Verifiser at resultatet er et ViewResult
            Assert.IsNotNull(result);
    
            // Verifiser at det returnerte ViewModel er av riktig type og inneholder de samme verdiene
            var returnedModel = result.Model as KnyttInnmeldingTilPersonViewModel;
            Assert.IsNotNull(returnedModel);
            Assert.AreEqual(model.Tittel, returnedModel.Tittel);
            Assert.AreEqual(model.Beskrivelse, returnedModel.Beskrivelse);
            Assert.AreEqual(model.Epost, returnedModel.Epost);
        }


        [TestMethod]
        [Description("Tester at POST-metoden LagreKnyttInnmeldingTilPerson returnerer LandingsSide ved gyldig modell")]
        public async Task LagreKnyttInnmeldingTilPerson_ValidModel_RedirigererTilLandingsSide()
        {
            // Arrange - Lag en gyldig modell
            var model = new KnyttInnmeldingTilPersonViewModel
            {
                GeometriGeoJson = "{\"type\":\"Point\",\"coordinates\":[10.0,60.0]}",
                Tittel = "Test Tittel",
                Beskrivelse = "Test Beskrivelse",
                ErNodEtatKritisk = true, // Prioritet skal være "høy"
                Epost = "test@example.com"
            };

            _mockInnmeldingLogic
                .Setup(m => m.ValidereOgLagreNyInnmelding(It.IsAny<InnmeldingModel>(), It.IsAny<Geometri>(), It.IsAny<string>()))
                .ReturnsAsync(true); // Simulerer en vellykket lagring

            // Act - Kaller POST-metoden med en gyldig modell
            var result = await _controller.LagreKnyttInnmeldingTilPerson(model) as RedirectToActionResult;

            // Assert - Verifiserer at det er en omdirigering til LandingsSide
            Assert.IsNotNull(result);
            Assert.AreEqual("LandingsSide", result.ActionName);
            Assert.AreEqual("LandingsSide", result.ControllerName);
        }


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
                .Setup(m => m.ValidereOgLagreNyInnmelding(It.IsAny<InnmeldingModel>(), It.IsAny<Geometri>(), It.IsAny<string>()))
                .Throws(new ForretningsRegelExceptionModel("Forretningsregel-feil"));

            var result = await _controller.LagreKnyttInnmeldingTilPerson(model) as ViewResult;

            Assert.IsNotNull(result);
            var returnedModel = result.Model as KnyttInnmeldingTilPersonViewModel;
            Assert.IsNotNull(returnedModel);
            Assert.AreEqual(model, returnedModel);
            Assert.IsTrue(_controller.ModelState.ContainsKey(""));
            Assert.AreEqual("Forretningsregel-feil", _controller.ModelState[""].Errors[0].ErrorMessage);
        }

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
                .Setup(m => m.ValidereOgLagreNyInnmelding(It.IsAny<InnmeldingModel>(), It.IsAny<Geometri>(), It.IsAny<string>()))
                .Throws(new Exception("Uventet feil"));

            var result = await _controller.LagreKnyttInnmeldingTilPerson(model) as ViewResult;

            Assert.IsNotNull(result);
            var returnedModel = result.Model as KnyttInnmeldingTilPersonViewModel;
            Assert.IsNotNull(returnedModel);
            Assert.AreEqual(model, returnedModel);
        }
    }
}