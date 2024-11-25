using Interface;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Models.Entities;
using ViewModels;
using Gruppe10KVprototype.Controllers.InnmelderControllers;
using Models.Models;

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
        }

        [TestMethod]
        [Description("Tester at GET-metoden henter aktive geometri-data og returnerer ViewResult")]
        public async Task BidraTilKartForbedring_HenterGeometri_ReturnererView()
        {
            // Arrange - Mock geometri data
            var mockGeometriData = new List<Geometri>
            {
                new Geometri { GeometriId = 1, GeometriGeoJson = "{\"type\":\"Point\",\"coordinates\":[10.0,60.0]}" },
                new Geometri { GeometriId = 2, GeometriGeoJson = "{\"type\":\"Point\",\"coordinates\":[11.0,61.0]}" }
            };

            _mockGeometriRepository
                .Setup(repo => repo.GetAktiveGeometriMedInnmeldingAsync())
                .ReturnsAsync(new List<(Geometri, InnmeldingModel)>
                {
                    (new Geometri { GeometriId = 1, GeometriGeoJson = "{\"type\":\"Point\",\"coordinates\":[10.0,60.0]}" },
                        new InnmeldingModel { /* Fyll ut egenskaper for modellen */ }),
                    (new Geometri { GeometriId = 2, GeometriGeoJson = "{\"type\":\"Point\",\"coordinates\":[11.0,61.0]}" },
                        new InnmeldingModel { /* Fyll ut egenskaper for modellen */ })
                });


            // Act - Kaller GET-metoden
            var resultat = await _controller.BidraTilKartForbedring() as ViewResult;

            // Assert - Verifiserer ViewResult og ViewModel
            Assert.IsNotNull(resultat);
            var viewModel = resultat.Model as BidraTilKartForbedringViewModel;
            Assert.IsNotNull(viewModel);
            Assert.AreEqual(2, viewModel.GeometriData.Count());
        }
        
        [TestMethod]
        [Description("Tester at POST-metoden returnerer BadRequest når modellen er ugyldig")]
        public async Task LagreVurdering_UgyldigModell_ReturnererBadRequest()
        {
            // Arrange - Ugyldig vurdering (uten kommentar)
            var vurdering = new VurderingModel { VurderingId = 1, Kommentar = "", InnmelderId = 1 };

            _controller.ModelState.AddModelError("Kommentar", "Kommentar er påkrevd.");

            // Act - Kaller POST-metoden
            var resultat = await _controller.LagreVurdering(vurdering) as BadRequestObjectResult;

            // Assert - Verifiserer BadRequestResult
            Assert.IsNotNull(resultat);
            Assert.AreEqual(400, resultat.StatusCode);
        }

        [TestMethod]
        [Description("Tester at POST-metoden returnerer feil ved lagringsproblemer")]
        public async Task LagreVurdering_Lagringsfeil_ReturnererServerError()
        {
            // Arrange - Gyldig vurdering, men repository kaster feil
            var vurdering = new VurderingModel { VurderingId = 1, Kommentar = "Test", InnmelderId = 1 };

            _mockVurderingRepository
                .Setup(repo => repo.LeggTilVurderingAsync(vurdering))
                .ThrowsAsync(new System.Exception("Databasefeil"));

            // Act - Kaller POST-metoden
            var resultat = await _controller.LagreVurdering(vurdering) as ObjectResult;

            // Assert - Verifiserer at StatusCode 500 returneres
            Assert.IsNotNull(resultat);
            Assert.AreEqual(500, resultat.StatusCode);
            Assert.AreEqual("Det oppstod en feil ved lagring av vurderingen.", resultat.Value);
        }
    }
}
