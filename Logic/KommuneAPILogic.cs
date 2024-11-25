using System.Text.Json;
using LogicInterfaces;
using Microsoft.Extensions.Logging;
using Models.Models;
using Newtonsoft.Json;

namespace Logic;

public class KommuneAPILogic : IKommuneAPILogic
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<KommuneAPILogic> _logger;

    public KommuneAPILogic(HttpClient httpClient, ILogger<KommuneAPILogic> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<List<Kommune>> GetKommunerAsync()
    {
        var response = await _httpClient.GetAsync("https://ws.geonorge.no/kommuneinfo/v1/kommuner");
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();
        _logger.LogInformation($"API Response: {content.Substring(0, Math.Min(500, content.Length))}");
        return JsonConvert.DeserializeObject<List<Kommune>>(content) ?? new List<Kommune>();
    }

    public async Task<Kommune> GetKommuneByCoordinatesAsync(double lat, double lng)
    {
        try
        {
            string formattedUrl = string.Format(
                System.Globalization.CultureInfo.InvariantCulture,
                "https://ws.geonorge.no/kommuneinfo/v1/punkt?nord={0:F6}&ost={1:F6}&koordsys=4258",
                lat,
                lng
            );
            var response = await _httpClient.GetAsync(formattedUrl);
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            _logger.LogInformation($"API Response: {content}");
            return JsonConvert.DeserializeObject<Kommune>(content);
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError($"API request failed: {ex.Message}");
            throw;
        }
    }

    public async Task<string> GetKommuneStringFromGeometri(Geometri geometri)
    {
        try
        {
            if (geometri == null || string.IsNullOrEmpty(geometri.GeometriGeoJson))
                return "Ikke tilgjengelig";

            // Parse GeoJSON to get coordinates
            var geoJsonDoc = JsonDocument.Parse(geometri.GeometriGeoJson);
            var coordinates = GetFirstCoordinate(geoJsonDoc.RootElement);

            if (coordinates == null)
                return "Ikke tilgjengelig";

            // Use /punkt endpoint with coordinates
            string formattedUrl = string.Format(
                System.Globalization.CultureInfo.InvariantCulture,
                "https://api.kartverket.no/kommuneinfo/v1/punkt?nord={0:F6}&ost={1:F6}&koordsys=4258",
                coordinates.Value.lat,
                coordinates.Value.lng
            );

            var response = await _httpClient.GetAsync(formattedUrl);
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            _logger.LogInformation($"API Response: {content}");

            var kommune = JsonConvert.DeserializeObject<Kommune>(content);
            return $"{kommune.Kommunenavn} ({kommune.Kommunenummer})";
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError($"API request failed: {ex.Message}");
            return "Ikke tilgjengelig";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Feil ved henting av kommune for geometri");
            return "Ikke tilgjengelig";
        }
    }

    private (double lat, double lng)? GetFirstCoordinate(JsonElement element)
    {
        try
        {
            JsonElement coordinates;
            if (element.TryGetProperty("type", out var typeProperty))
            {
                if (typeProperty.GetString() == "Feature")
                {
                    coordinates = element.GetProperty("geometry").GetProperty("coordinates");
                }
                else
                {
                    coordinates = element.GetProperty("coordinates");
                }

                // Håndter forskjellige geometrityper
                switch (typeProperty.GetString())
                {
                    case "Point":
                        return (coordinates[1].GetDouble(), coordinates[0].GetDouble());
                    case "LineString":
                        return (coordinates[0][1].GetDouble(), coordinates[0][0].GetDouble());
                    case "Polygon":
                        return (coordinates[0][0][1].GetDouble(), coordinates[0][0][0].GetDouble());
                    default:
                        return null;
                }
            }
            return null;
        }
        catch
        {
            return null;
        }
    }
}