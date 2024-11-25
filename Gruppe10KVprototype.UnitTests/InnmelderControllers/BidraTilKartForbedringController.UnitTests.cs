using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Gruppe10KVprototype.Controllers.InnmelderControllers;
using Interface;
using Microsoft.AspNetCore.Authorization;
using Models.Entities;
using Models.Models;
using ViewModels;

namespace Controller.UnitTests
{
    [TestClass]
    public class BidraTilKartForbedringControllerTester
    {
        private Mock<IGeometriRepository> _mockGeometriRepository;
        private Mock<IVurderingRepository> _mockVurderingRepository;
        private BidraTilKartForbedringController _controller;

        [TestInitialize]
        public void Oppsett()
        {
            _mockGeometriRepository = new Mock<IGeometriRepository>();
            _mockVurderingRepository = new Mock<IVurderingRepository>();
            _controller = new BidraTilKartForbedringController(
                _mockGeometriRepository.Object,
                _mockVurderingRepository.Object
            );

            // Setup controller context med authentication
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Role, "Innmelder"),
            }, "mock"));

            _controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext { User = user }
            };
        }

        #region GET Tester

        [TestMethod]
        [Description("Tester at GET-metoden henter aktive geometri-data og returnerer view med data")]
        public async Task BidraTilKartForbedring_HenterAktiveGeometrier_ReturnererViewMedData()
        {
            // Arrange
            var mockData = new List<(Geometri, InnmeldingModel)>
            {
                (new Geometri
                    {
                        GeometriId = 1,
                        GeometriGeoJson = "{\"type\":\"Point\",\"coordinates\":[10.0,60.0]}"
                    },
                    new InnmeldingModel
                    {
                        InnmeldingId = 1,
                        Status = "Aktiv"
                    }),
                (new Geometri
                    {
                        GeometriId = 2,
                        GeometriGeoJson = "{\"type\":\"Point\",\"coordinates\":[11.0,61.0]}"
                    },
                    new InnmeldingModel
                    {
                        InnmeldingId = 2,
                        Status = "Aktiv"
                    })
            };

            _mockGeometriRepository
                .Setup(repo => repo.GetAktiveGeometriMedInnmeldingAsync())
                .ReturnsAsync(mockData);

            // Act
            var resultat = await _controller.BidraTilKartForbedring() as ViewResult;

            // Assert
            Assert.IsNotNull(resultat);
            var viewModel = resultat.Model as BidraTilKartForbedringViewModel;
            Assert.IsNotNull(viewModel);
            Assert.AreEqual(2, viewModel.GeometriData.Count());
            _mockGeometriRepository.Verify(r => r.GetAktiveGeometriMedInnmeldingAsync(), Times.Once);
        }

        [TestMethod]
        [Description("Tester at GET-metoden håndterer tom liste")]
        public async Task BidraTilKartForbedring_IngenAktiveGeometrier_ReturnererViewMedTomListe()
        {
            // Arrange
            _mockGeometriRepository
                .Setup(repo => repo.GetAktiveGeometriMedInnmeldingAsync())
                .ReturnsAsync(new List<(Geometri, InnmeldingModel)>());

            // Act
            var resultat = await _controller.BidraTilKartForbedring() as ViewResult;

            // Assert
            Assert.IsNotNull(resultat);
            var viewModel = resultat.Model as BidraTilKartForbedringViewModel;
            Assert.IsNotNull(viewModel);
            Assert.IsFalse(viewModel.GeometriData.Any());
            _mockGeometriRepository.Verify(r => r.GetAktiveGeometriMedInnmeldingAsync(), Times.Once);
        }

        [TestMethod]
        [Description("Tester at GET-metoden håndterer databasefeil")]
        public async Task BidraTilKartForbedring_DatabaseFeil_KasterException()
        {
            // Arrange
            _mockGeometriRepository
                .Setup(repo => repo.GetAktiveGeometriMedInnmeldingAsync())
                .ThrowsAsync(new Exception("Databasefeil"));

            // Act & Assert
            await Assert.ThrowsExceptionAsync<Exception>(() =>
                _controller.BidraTilKartForbedring());
        }

        #endregion

        #region POST Tester

        [TestMethod]
        [Description("Tester at POST-metoden lagrer gyldig vurdering og returnerer suksessmelding")]
        public async Task LagreVurdering_GyldigVurdering_ReturnererSuksess()
        {
            // Arrange
            var vurdering = new VurderingModel
            {
                VurderingId = 1,
                Kommentar = "Test vurdering",
                InnmelderId = 1
            };

            _mockVurderingRepository
                .Setup(repo => repo.LeggTilVurderingAsync(vurdering))
                .ReturnsAsync(1); // Returnerer Task<int> med verdi 1 (f.eks. antall påvirkede rader)

            // Act
            var resultat = await _controller.LagreVurdering(vurdering) as JsonResult;

            // Assert
            Assert.IsNotNull(resultat);
            Assert.IsNotNull(resultat.Value);

            // Konverter til string og sjekk innholdet
            var jsonString = System.Text.Json.JsonSerializer.Serialize(resultat.Value);
            Assert.IsTrue(jsonString.Contains("\"success\":true"));
            Assert.IsTrue(jsonString.Contains("\"Message\":\"Takk for ditt bidrag!\""));

            _mockVurderingRepository.Verify(r => r.LeggTilVurderingAsync(vurdering), Times.Once);
        }

        [TestMethod]
        [Description("Tester at POST-metoden returnerer BadRequest ved ugyldig modell")]
        public async Task LagreVurdering_UgyldigModell_ReturnererBadRequest()
        {
            // Arrange
            var vurdering = new VurderingModel(); // Tom modell
            _controller.ModelState.AddModelError("Kommentar", "Kommentar er påkrevd");

            // Act
            var resultat = await _controller.LagreVurdering(vurdering) as BadRequestObjectResult;

            // Assert
            Assert.IsNotNull(resultat);
            Assert.AreEqual(400, resultat.StatusCode);
            _mockVurderingRepository.Verify(r => r.LeggTilVurderingAsync(It.IsAny<VurderingModel>()), Times.Never);
        }

        [TestMethod]
        [Description("Tester at POST-metoden returnerer feilmelding ved lagringsfeil")]
        public async Task LagreVurdering_DatabaseFeil_ReturnererServerError()
        {
            // Arrange
            var vurdering = new VurderingModel
            {
                VurderingId = 1,
                Kommentar = "Test",
                InnmelderId = 1
            };

            _mockVurderingRepository
                .Setup(repo => repo.LeggTilVurderingAsync(vurdering))
                .ThrowsAsync(new Exception("Databasefeil"));

            // Act
            var resultat = await _controller.LagreVurdering(vurdering) as ObjectResult;

            // Assert
            Assert.IsNotNull(resultat);
            Assert.AreEqual(500, resultat.StatusCode);
            Assert.AreEqual("Det oppstod en feil ved lagring av vurderingen.", resultat.Value);
            _mockVurderingRepository.Verify(r => r.LeggTilVurderingAsync(vurdering), Times.Once);
        }

        #endregion

        #region Autorisering og Sikkerhet

        [TestMethod]
        [Description("Tester at controller er beskyttet med Innmelder-rolle")]
        public void Controller_ErBeskyttetMedInnmelderRolle()
        {
            // Arrange & Act
            var authorizeAttribute = typeof(BidraTilKartForbedringController)
                .GetCustomAttributes(typeof(AuthorizeAttribute), true)
                .FirstOrDefault() as AuthorizeAttribute;

            // Assert
            Assert.IsNotNull(authorizeAttribute);
            Assert.AreEqual("Innmelder", authorizeAttribute.Roles);
        }

        [TestMethod]
        [Description("Tester at controller har AntiForgeryToken validering")]
        public void Controller_HarAutoValidateAntiforgeryTokenAttribute()
        {
            // Arrange & Act
            var attributes = typeof(BidraTilKartForbedringController)
                .GetCustomAttributes(typeof(AutoValidateAntiforgeryTokenAttribute), true);

            // Assert
            Assert.IsTrue(attributes.Any());
        }

        #endregion

        #region Detaljert GeometriData Testing

        [TestMethod]
        [Description("Tester at geometridata inneholder korrekte verdier og format")]
        public async Task BidraTilKartForbedring_GeometriData_HarKorrekteVerdierOgFormat()
        {
            // Arrange
            var mockData = new List<(Geometri, InnmeldingModel)>
            {
                (new Geometri
                    {
                        GeometriId = 1,
                        GeometriGeoJson = "{\"type\":\"Point\",\"coordinates\":[10.0,60.0]}",
                        InnmeldingId = 1
                    },
                    new InnmeldingModel
                    {
                        InnmeldingId = 1,
                        Status = "Aktiv",
                        Tittel = "Test Innmelding",
                        Beskrivelse = "Test Beskrivelse"
                    })
            };

            _mockGeometriRepository
                .Setup(repo => repo.GetAktiveGeometriMedInnmeldingAsync())
                .ReturnsAsync(mockData);

            // Act
            var resultat = await _controller.BidraTilKartForbedring() as ViewResult;

            // Assert
            Assert.IsNotNull(resultat);
            var viewModel = resultat.Model as BidraTilKartForbedringViewModel;
            Assert.IsNotNull(viewModel);

            var (geometri, innmelding) = viewModel.GeometriData.First();

            // Sjekk geometri
            Assert.AreEqual(1, geometri.GeometriId);
            Assert.AreEqual(1, geometri.InnmeldingId);

            // Sjekk GeoJSON format
            var geoJson = System.Text.Json.JsonDocument.Parse(geometri.GeometriGeoJson);
            Assert.AreEqual("Point", geoJson.RootElement.GetProperty("type").GetString());
            Assert.AreEqual(10.0, geoJson.RootElement.GetProperty("coordinates")[0].GetDouble());
            Assert.AreEqual(60.0, geoJson.RootElement.GetProperty("coordinates")[1].GetDouble());

            // Sjekk innmelding
            Assert.AreEqual(1, innmelding.InnmeldingId);
            Assert.AreEqual("Aktiv", innmelding.Status);
            Assert.AreEqual("Test Innmelding", innmelding.Tittel);
            Assert.AreEqual("Test Beskrivelse", innmelding.Beskrivelse);
        }

        [TestMethod]
        [Description("Tester at ugyldig GeoJSON format håndteres")]
        public async Task BidraTilKartForbedring_UgyldigGeoJsonFormat_ReturnererView()
        {
            // Arrange
            var mockData = new List<(Geometri, InnmeldingModel)>
            {
                (new Geometri
                    {
                        GeometriId = 1,
                        GeometriGeoJson = "{\"type\":\"InvalidType\",\"coordinates\":[]}",
                        InnmeldingId = 1
                    },
                    new InnmeldingModel
                    {
                        InnmeldingId = 1,
                        Status = "Aktiv"
                    })
            };

            _mockGeometriRepository
                .Setup(repo => repo.GetAktiveGeometriMedInnmeldingAsync())
                .ReturnsAsync(mockData);

            // Act
            var resultat = await _controller.BidraTilKartForbedring() as ViewResult;

            // Assert
            Assert.IsNotNull(resultat);
            var viewModel = resultat.Model as BidraTilKartForbedringViewModel;
            Assert.IsNotNull(viewModel);
            Assert.IsTrue(viewModel.GeometriData.Any());
        }

        #endregion

        #region Validering av Vurdering

        [TestMethod]
        [Description("Tester at vurdering valideres for påkrevde felt")]
        public async Task LagreVurdering_ManglendePåkrevdeFelt_ReturnererValideringsfeil()
        {
            // Arrange
            var vurdering = new VurderingModel(); // Tom modell

            // Legg kun til valideringsfeil for felt som faktisk er påkrevd
            _controller.ModelState.AddModelError("Kommentar", "Kommentar er påkrevd");
            _controller.ModelState.AddModelError("InnmelderId", "InnmelderId er påkrevd");

            // Act
            var resultat = await _controller.LagreVurdering(vurdering) as BadRequestObjectResult;

            // Assert
            Assert.IsNotNull(resultat);
            Assert.AreEqual(400, resultat.StatusCode);
            _mockVurderingRepository.Verify(r => 
                r.LeggTilVurderingAsync(It.IsAny<VurderingModel>()), Times.Never);
        }

        [TestMethod]
        [Description("Tester at vurdering valideres for kommentarfelt")]
        public async Task LagreVurdering_UgyldigKommentar_ReturnererValideringsfeil()
        {
            // Arrange
            var vurdering = new VurderingModel 
            { 
                VurderingId = 1,
                InnmelderId = 1,
                Kommentar = null  // eller "" hvis det er det som valideres
            };

            _controller.ModelState.AddModelError("Kommentar", "Kommentar er påkrevd");

            // Act
            var resultat = await _controller.LagreVurdering(vurdering) as BadRequestObjectResult;

            // Assert
            Assert.IsNotNull(resultat);
            Assert.AreEqual(400, resultat.StatusCode);
            _mockVurderingRepository.Verify(r => 
                r.LeggTilVurderingAsync(It.IsAny<VurderingModel>()), Times.Never);
        }

        #endregion
    }
}