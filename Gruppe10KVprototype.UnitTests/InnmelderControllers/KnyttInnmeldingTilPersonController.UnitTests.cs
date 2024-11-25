using System.Security.Claims;
using Gruppe10KVprototype.Controllers.InnmelderControllers;
using LogicInterfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Models.Entities;
using Models.Models;
using Moq;
using ViewModels;

[TestClass]
public class KnyttInnmeldingTilPersonControllerTester
{
    private KnyttInnmeldingTilPersonController _kontroller;
    private Mock<IInnmeldingLogic> _innmeldingLogicMock;

    [TestInitialize]
    public void Oppsett()
    {
        _innmeldingLogicMock = new Mock<IInnmeldingLogic>();
        _kontroller = new KnyttInnmeldingTilPersonController(_innmeldingLogicMock.Object);
    }

    private void SimulerInnloggetBruker(string epost = "test@example.com")
    {
        var claims = new[] { new Claim(ClaimTypes.Name, epost) };
        var identity = new ClaimsIdentity(claims, "TestAuthentication");
        var principal = new ClaimsPrincipal(identity);

        _kontroller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = principal }
        };
    }

    private void SimulerIkkeInnloggetBruker()
    {
        _kontroller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext()
        };
    }

    #region Sikkerhet
    [TestMethod]
    [Description("Tester at controller har AntiForgeryToken beskyttelse")]
    public void Controller_HarAntiForgeryTokenBeskyttelse()
    {
        // Arrange & Act
        var attributes = typeof(KnyttInnmeldingTilPersonController)
            .GetCustomAttributes(typeof(AutoValidateAntiforgeryTokenAttribute), true);

        // Assert
        Assert.IsTrue(attributes.Any());
    }
    #endregion

    #region GET KnyttInnmeldingTilPerson Tester
    [TestMethod]
    [Description("Tester automatisk lagring for innlogget bruker")]
    public async Task KnyttInnmeldingTilPerson_InnloggetBruker_LagreAutomatisk()
    {
        // Arrange
        SimulerInnloggetBruker("test@example.com");
        var model = new KnyttInnmeldingTilPersonViewModel
        {
            GeometriGeoJson = "{\"type\":\"Point\",\"coordinates\":[10.0,60.0]}",
            Tittel = "Test",
            Beskrivelse = "Test"
        };

        _innmeldingLogicMock
            .Setup(x => x.ValidereOgLagreNyInnmelding(
                It.IsAny<InnmeldingModel>(),
                It.IsAny<Geometri>(),
                "test@example.com",
                true))
            .ReturnsAsync(true);

        // Act
        var resultat = await _kontroller.KnyttInnmeldingTilPerson(model) as RedirectToActionResult;

        // Assert
        Assert.IsNotNull(resultat);
        Assert.AreEqual("LandingsSide", resultat.ActionName);
        Assert.AreEqual("LandingsSide", resultat.ControllerName);
    }

    [TestMethod]
    [Description("Tester at ikke-innlogget bruker får se skjema")]
    public async Task KnyttInnmeldingTilPerson_IkkeInnloggetBruker_ViserSkjema()
    {
        // Arrange
        SimulerIkkeInnloggetBruker();
        var model = new KnyttInnmeldingTilPersonViewModel
        {
            GeometriGeoJson = "{\"type\":\"Point\",\"coordinates\":[10.0,60.0]}",
            Tittel = "Test",
            Beskrivelse = "Test"
        };

        // Act
        var resultat = await _kontroller.KnyttInnmeldingTilPerson(model) as ViewResult;

        // Assert
        Assert.IsNotNull(resultat);
        Assert.IsNull(resultat.ViewName); // Standard view
        var returnertModel = resultat.Model as KnyttInnmeldingTilPersonViewModel;
        Assert.IsNotNull(returnertModel);
        Assert.IsNull(returnertModel.Epost);
    }
    #endregion

    #region POST LagreKnyttInnmeldingTilPerson Tester
    [TestMethod]
    [Description("Tester at innlogget bruker sendes til LandingsSide ved vellykket lagring")]
    public async Task LagreKnyttInnmeldingTilPerson_InnloggetBruker_RedirectTilLandingsSide()
    {
        // Arrange
        SimulerInnloggetBruker();
        var model = new KnyttInnmeldingTilPersonViewModel
        {
            GeometriGeoJson = "{\"type\":\"Point\",\"coordinates\":[10.0,60.0]}",
            Tittel = "Test",
            Beskrivelse = "Test",
            ErNodEtatKritisk = true
        };

        _innmeldingLogicMock
            .Setup(x => x.ValidereOgLagreNyInnmelding(
                It.IsAny<InnmeldingModel>(),
                It.IsAny<Geometri>(),
                It.IsAny<string>(),
                true))
            .ReturnsAsync(true);

        // Act
        var resultat = await _kontroller.LagreKnyttInnmeldingTilPerson(model) as RedirectToActionResult;

        // Assert
        Assert.IsNotNull(resultat);
        Assert.AreEqual("LandingsSide", resultat.ActionName);
        Assert.AreEqual("LandingsSide", resultat.ControllerName);
    }

    [TestMethod]
    [Description("Tester at ikke-innlogget bruker sendes til Home ved vellykket lagring")]
    public async Task LagreKnyttInnmeldingTilPerson_IkkeInnloggetBruker_RedirectTilHome()
    {
        // Arrange
        SimulerIkkeInnloggetBruker();
        var model = new KnyttInnmeldingTilPersonViewModel
        {
            GeometriGeoJson = "{\"type\":\"Point\",\"coordinates\":[10.0,60.0]}",
            Tittel = "Test",
            Beskrivelse = "Test",
            Epost = "test@example.com"
        };

        _innmeldingLogicMock
            .Setup(x => x.ValidereOgLagreNyInnmelding(
                It.IsAny<InnmeldingModel>(),
                It.IsAny<Geometri>(),
                It.IsAny<string>(),
                false))
            .ReturnsAsync(true);

        // Act
        var resultat = await _kontroller.LagreKnyttInnmeldingTilPerson(model) as RedirectToActionResult;

        // Assert
        Assert.IsNotNull(resultat);
        Assert.AreEqual("Index", resultat.ActionName);
        Assert.AreEqual("Home", resultat.ControllerName);
    }

    [TestMethod]
    [Description("Tester at feilet lagring returnerer feilmelding")]
    public async Task LagreKnyttInnmeldingTilPerson_LagreIkkeVellykket_ReturnererFeilmelding()
    {
        // Arrange
        SimulerInnloggetBruker();
        var model = new KnyttInnmeldingTilPersonViewModel
        {
            GeometriGeoJson = "{\"type\":\"Point\",\"coordinates\":[10.0,60.0]}",
            Tittel = "Test",
            Beskrivelse = "Test"
        };

        _innmeldingLogicMock
            .Setup(x => x.ValidereOgLagreNyInnmelding(
                It.IsAny<InnmeldingModel>(),
                It.IsAny<Geometri>(),
                It.IsAny<string>(),
                true))
            .ReturnsAsync(false);

        // Act
        var resultat = await _kontroller.LagreKnyttInnmeldingTilPerson(model) as ViewResult;

        // Assert
        Assert.IsNotNull(resultat);
        Assert.AreEqual("KnyttInnmeldingTilPerson", resultat.ViewName);
        Assert.IsTrue(_kontroller.ModelState.ContainsKey(""));
        Assert.AreEqual(
            "Kunne ikke lagre innmeldingen. Vennligst prøv igjen.", 
            _kontroller.ModelState[""].Errors[0].ErrorMessage);
    }
    #endregion
}