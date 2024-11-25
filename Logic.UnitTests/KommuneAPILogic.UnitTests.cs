using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;
using Newtonsoft.Json;
using System.Net;
using Models.Models;
using System.Globalization;
using System.Text.Json;

namespace Logic.Tests
{
    [TestClass]
    public class KommuneAPILogicTests
    {
        private Mock<HttpMessageHandler> _httpMessageHandlerMock;
        private HttpClient _httpClient;
        private Mock<ILogger<KommuneAPILogic>> _loggerMock;
        private KommuneAPILogic _kommuneAPILogic;

        [TestInitialize]
        public void Initialize()
        {
            _httpMessageHandlerMock = new Mock<HttpMessageHandler>();
            _httpClient = new HttpClient(_httpMessageHandlerMock.Object);
            _loggerMock = new Mock<ILogger<KommuneAPILogic>>();
            _kommuneAPILogic = new KommuneAPILogic(_httpClient, _loggerMock.Object);
        }

        [TestMethod]
        [Description("Tester henting av alle kommuner fra API")]
        public async Task HentKommuner_VellykketAPIKall_ReturnererKommuneListe()
        {
            // Arrange
            var kommuner = new List<Kommune>
            {
                new Kommune { Kommunenavn = "Oslo", Kommunenummer = "0301" },
                new Kommune { Kommunenavn = "Bergen", Kommunenummer = "4601" }
            };

            SetupHttpMockResponse(
                "https://ws.geonorge.no/kommuneinfo/v1/kommuner",
                JsonConvert.SerializeObject(kommuner),
                HttpStatusCode.OK
            );

            // Act
            var result = await _kommuneAPILogic.GetKommunerAsync();

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Count);
            Assert.AreEqual("Oslo", result[0].Kommunenavn);
            Assert.AreEqual("0301", result[0].Kommunenummer);
        }

        [TestMethod]
        [Description("Tester at henting av kommuneliste kaster exception når API-kallet feiler")]
        [ExpectedException(typeof(HttpRequestException))]
        public async Task HentKommuner_APIKallFeil_KasterException()
        {
            // Arrange
            SetupHttpMockResponse(
                "https://ws.geonorge.no/kommuneinfo/v1/kommuner",
                "Error",
                HttpStatusCode.InternalServerError
            );

            // Act
            await _kommuneAPILogic.GetKommunerAsync();
        }

        [TestMethod]
        [Description("Tester henting av kommune basert på koordinater")]
        public async Task HentKommuneFraKoordinater_GyldigeKoordinater_ReturnererRiktigKommune()
        {
            // Arrange
            var kommune = new Kommune { Kommunenavn = "Oslo", Kommunenummer = "0301" };
            var lat = 59.911491;
            var lng = 10.757933;

            var expectedUrl = string.Format(
                CultureInfo.InvariantCulture,
                "https://ws.geonorge.no/kommuneinfo/v1/punkt?nord={0:F6}&ost={1:F6}&koordsys=4258",
                lat,
                lng
            );

            SetupHttpMockResponse(
                expectedUrl,
                JsonConvert.SerializeObject(kommune),
                HttpStatusCode.OK
            );

            // Act
            var result = await _kommuneAPILogic.GetKommuneByCoordinatesAsync(lat, lng);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("Oslo", result.Kommunenavn);
            Assert.AreEqual("0301", result.Kommunenummer);
        }

        [TestMethod]
        [Description("Tester håndtering av geometridata fra database")]
        public async Task HentKommuneStringFraGeometri_GyldigGeometri_ReturnererKommuneString()
        {
            // Arrange
            var geometriJson = @"{
                ""type"": ""Point"",
                ""coordinates"": [10.757933, 59.911491]
            }";

            Console.WriteLine($"Input GeoJSON: {geometriJson}");

            var geometri = new Geometri
            {
                GeometriId = 1,
                InnmeldingId = 1,
                GeometriGeoJson = geometriJson
            };

            var kommune = new Kommune { Kommunenavn = "Oslo", Kommunenummer = "0301" };

            // Parse coordinates
            using (var doc = JsonDocument.Parse(geometriJson))
            {
                var coordinates = doc.RootElement.GetProperty("coordinates");
                var lng = coordinates[0].GetDouble();
                var lat = coordinates[1].GetDouble();
                Console.WriteLine($"Parsed coordinates: lat={lat}, lng={lng}");

                var expectedUrl = string.Format(
                    CultureInfo.InvariantCulture,
                    "https://api.kartverket.no/kommuneinfo/v1/punkt?nord={0:F6}&ost={1:F6}&koordsys=4258",
                    lat,
                    lng
                );

                Console.WriteLine($"Expected URL: {expectedUrl}");

                SetupHttpMockResponse(
                    expectedUrl,
                    JsonConvert.SerializeObject(kommune),
                    HttpStatusCode.OK
                );
            }

            // Act
            var result = await _kommuneAPILogic.GetKommuneStringFromGeometri(geometri);

            // Assert
            Assert.AreEqual("Oslo (0301)", result);
        }

        [TestMethod]
        [Description("Tester håndtering av manglende geometridata")]
        public async Task HentKommuneStringFraGeometri_NullGeometri_ReturnererIkkeTilgjengelig()
        {
            // Act
            var result = await _kommuneAPILogic.GetKommuneStringFromGeometri(null);

            // Assert
            Assert.AreEqual("Ikke tilgjengelig", result);
        }

        [TestMethod]
        [Description("Tester håndtering av ugyldig GeoJSON")]
        public async Task HentKommuneStringFraGeometri_UgyldigGeoJson_ReturnererIkkeTilgjengelig()
        {
            // Arrange
            var geometri = new Geometri
            {
                GeometriId = 1,
                InnmeldingId = 1,
                GeometriGeoJson = "ugyldig json"
            };

            // Act
            var result = await _kommuneAPILogic.GetKommuneStringFromGeometri(geometri);

            // Assert
            Assert.AreEqual("Ikke tilgjengelig", result);
        }

        [TestMethod]
        [Description("Tester håndtering av API-feil ved geometri til kommune konvertering")]
        public async Task HentKommuneStringFraGeometri_APIFeil_ReturnererIkkeTilgjengelig()
        {
            // Arrange
            var geometriJson = @"{
                ""type"": ""Point"",
                ""coordinates"": [10.757933, 59.911491]
            }";

            var geometri = new Geometri
            {
                GeometriId = 1,
                InnmeldingId = 1,
                GeometriGeoJson = geometriJson
            };

            using (var doc = JsonDocument.Parse(geometriJson))
            {
                var coordinates = doc.RootElement.GetProperty("coordinates");
                var lng = coordinates[0].GetDouble();
                var lat = coordinates[1].GetDouble();

                var expectedUrl = string.Format(
                    CultureInfo.InvariantCulture,
                    "https://api.kartverket.no/kommuneinfo/v1/punkt?nord={0:F6}&ost={1:F6}&koordsys=4258",
                    lat,
                    lng
                );

                SetupHttpMockResponse(
                    expectedUrl,
                    "Error",
                    HttpStatusCode.InternalServerError
                );
            }

            // Act
            var result = await _kommuneAPILogic.GetKommuneStringFromGeometri(geometri);

            // Assert
            Assert.AreEqual("Ikke tilgjengelig", result);
        }

        private void SetupHttpMockResponse(string expectedUrl, string content, HttpStatusCode statusCode)
        {
            Console.WriteLine($"Setting up mock for URL: {expectedUrl}");

            _httpMessageHandlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(request =>
                        request.Method == HttpMethod.Get &&
                        request.RequestUri.ToString().Equals(expectedUrl, StringComparison.OrdinalIgnoreCase)),
                    ItExpr.IsAny<CancellationToken>()
                )
                .Callback<HttpRequestMessage, CancellationToken>((request, token) =>
                {
                    Console.WriteLine($"Request URL: {request.RequestUri}");
                })
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = statusCode,
                    Content = new StringContent(content)
                })
                .Verifiable();
        }
    }
}