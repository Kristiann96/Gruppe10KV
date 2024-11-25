using Microsoft.AspNetCore.Mvc;

[TestClass]
public class KartfeilSkjemaBekreftelseControllerTester
{
    private KartfeilSkjemaBekreftelseController _kontroller;

    [TestInitialize]
    public void Oppsett()
    {
        _kontroller = new KartfeilSkjemaBekreftelseController();
    }

    #region Sikkerhet
    [TestMethod]
    [Description("Tester at controller har AntiForgeryToken beskyttelse")]
    public void Controller_HarAntiForgeryTokenBeskyttelse()
    {
        // Arrange & Act
        var attributes = typeof(KartfeilSkjemaBekreftelseController)
            .GetCustomAttributes(typeof(AutoValidateAntiforgeryTokenAttribute), true);

        // Assert
        Assert.IsTrue(attributes.Any(), "Controller mangler AutoValidateAntiforgeryToken attributt");
    }
    #endregion

    #region KartfeilSkjemaBekreftelse Tester
    [TestMethod]
    [Description("Tester at bekreftelse med tom geometri omdirigerer til kartfeil-siden")]
    public void KartfeilSkjemaBekreftelse_TomGeometri_RedirectTilKartfeilSide()
    {
        // Arrange
        var modell = new KartfeilSkjemaViewModel { GeometriGeoJson = string.Empty };

        // Act
        var resultat = _kontroller.KartfeilSkjemaBekreftelse(modell) as RedirectToActionResult;

        // Assert
        Assert.IsNotNull(resultat);
        Assert.AreEqual("KartfeilMarkering", resultat.ActionName);
        Assert.AreEqual("KartfeilMarkering", resultat.ControllerName);
    }

    [TestMethod]
    [Description("Tester at bekreftelse med null geometri omdirigerer til kartfeil-siden")]
    public void KartfeilSkjemaBekreftelse_NullGeometri_RedirectTilKartfeilSide()
    {
        // Arrange
        var modell = new KartfeilSkjemaViewModel { GeometriGeoJson = null };

        // Act
        var resultat = _kontroller.KartfeilSkjemaBekreftelse(modell) as RedirectToActionResult;

        // Assert
        Assert.IsNotNull(resultat);
        Assert.AreEqual("KartfeilMarkering", resultat.ActionName);
        Assert.AreEqual("KartfeilMarkering", resultat.ControllerName);
    }

    [TestMethod]
    [Description("Tester at bekreftelse med gyldig geometri returnerer riktig view")]
    public void KartfeilSkjemaBekreftelse_GyldigGeometri_ReturnererView()
    {
        // Arrange
        var gyldigGeoJson = "{\"type\":\"Point\",\"coordinates\":[10.0,60.0]}";
        var modell = new KartfeilSkjemaViewModel { GeometriGeoJson = gyldigGeoJson };

        // Act
        var resultat = _kontroller.KartfeilSkjemaBekreftelse(modell) as ViewResult;

        // Assert
        Assert.IsNotNull(resultat);
        Assert.AreSame(modell, resultat.Model);
    }

    [TestMethod]
    [Description("Tester at bekreftelse med kompleks geometri returnerer riktig view")]
    public void KartfeilSkjemaBekreftelse_KompleksGeometri_ReturnererView()
    {
        // Arrange
        var kompleksGeoJson = @"{
            ""type"": ""Polygon"",
            ""coordinates"": [
                [[10.0, 60.0], [11.0, 60.0], [11.0, 61.0], [10.0, 61.0], [10.0, 60.0]]
            ]
        }";
        var modell = new KartfeilSkjemaViewModel { GeometriGeoJson = kompleksGeoJson };

        // Act
        var resultat = _kontroller.KartfeilSkjemaBekreftelse(modell) as ViewResult;

        // Assert
        Assert.IsNotNull(resultat);
        Assert.AreSame(modell, resultat.Model);
    }

    [TestMethod]
    [Description("Tester at view returneres med standard viewnavn")]
    public void KartfeilSkjemaBekreftelse_GyldigModell_ReturnererStandardView()
    {
        // Arrange
        var modell = new KartfeilSkjemaViewModel 
        { 
            GeometriGeoJson = "{\"type\":\"Point\",\"coordinates\":[10.0,60.0]}" 
        };

        // Act
        var resultat = _kontroller.KartfeilSkjemaBekreftelse(modell) as ViewResult;

        // Assert
        Assert.IsNotNull(resultat);
        Assert.IsNull(resultat.ViewName); // Bekrefter at standard viewnavn brukes
    }
    #endregion
}