using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using Newtonsoft.Json;
using ViewModels;
using Models.Models;
using Models.Entities;

namespace ViewModels.UnitTests
{
    [TestClass]
    public class BidraTilKartForbedringViewModelTests
    {
        /// <summary>
        /// Tester at metoden returnerer en tom JSON-array ("[]") når GeometriData er null.
        /// Dette er viktig for å sikre at metoden håndterer nullverdier uten å kaste unntak.
        /// </summary>
        [TestMethod]
        public void GetGeometriDataAsJson_ShouldReturnEmptyArray_WhenGeometriDataIsNull()
        {
            // Arrange: Opprett en ViewModel der GeometriData er null.
            var viewModel = new BidraTilKartForbedringViewModel
            {
                GeometriData = null
            };

            // Act: Kall metoden for å generere JSON-data.
            var jsonResult = viewModel.GetGeometriDataAsJson();

            // Assert: Sjekk at resultatet er en tom JSON-array.
            Assert.AreEqual("[]", jsonResult);
        }

        /// <summary>
        /// Tester at GeometriData serialiseres riktig til JSON.
        /// Sjekker om transformasjonen fra Geometri- og Innmelding-modellene til JSON fungerer som forventet.
        /// </summary>
        [TestMethod]
        public void GetGeometriDataAsJson_ShouldSerializeCorrectly()
        {
            // Arrange: Opprett en ViewModel med eksempeldata.
            var viewModel = new BidraTilKartForbedringViewModel
            {
                GeometriData = new List<(Geometri, InnmeldingModel)>
                {
                    (
                        new Geometri
                        {
                            GeometriId = 1,
                            InnmeldingId = 10,
                            GeometriGeoJson = "{\"type\": \"Point\", \"coordinates\": [10, 20]}"
                        },
                        new InnmeldingModel
                        {
                            Tittel = "Innmelding 1"
                        }
                    ),
                    (
                        new Geometri
                        {
                            GeometriId = 2,
                            InnmeldingId = 20,
                            GeometriGeoJson = "{\"type\": \"Point\", \"coordinates\": [30, 40]}"
                        },
                        new InnmeldingModel
                        {
                            Tittel = "Innmelding 2"
                        }
                    )
                }
            };

            // Act: Generer JSON fra GeometriData.
            var jsonResult = viewModel.GetGeometriDataAsJson();

            // Expected: Definer forventet JSON-struktur.
            var expectedJson = JsonConvert.SerializeObject(new[]
            {
                new
                {
                    GeometriId = 1,
                    InnmeldingId = 10,
                    GeometriGeoJson = "{\"type\": \"Point\", \"coordinates\": [10, 20]}",
                    Tittel = "Innmelding 1"
                },
                new
                {
                    GeometriId = 2,
                    InnmeldingId = 20,
                    GeometriGeoJson = "{\"type\": \"Point\", \"coordinates\": [30, 40]}",
                    Tittel = "Innmelding 2"
                }
            });

            // Assert: Sjekk at generert JSON matcher forventet resultat.
            Assert.AreEqual(expectedJson, jsonResult);
        }

        /// <summary>
        /// Tester at metoden håndterer en tom liste uten å kaste unntak
        /// og returnerer en tom JSON-array.
        /// </summary>
        [TestMethod]
        public void GetGeometriDataAsJson_ShouldHandleEmptyGeometriData()
        {
            // Arrange: Opprett en ViewModel med en tom liste for GeometriData.
            var viewModel = new BidraTilKartForbedringViewModel
            {
                GeometriData = new List<(Geometri, InnmeldingModel)>()
            };

            // Act: Generer JSON fra GeometriData.
            var jsonResult = viewModel.GetGeometriDataAsJson();

            // Assert: Sjekk at resultatet er en tom JSON-array.
            Assert.AreEqual("[]", jsonResult);
        }
    }
}
