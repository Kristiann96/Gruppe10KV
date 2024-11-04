using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Interface;
using System.Collections.Generic;
using LogicInterfaces;
using Models.Models;
using Microsoft.Extensions.Logging;

namespace Logic
{
    public class KartverketAPILogic : IKartverketAPILogic
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<KartverketAPILogic> _logger;

        public KartverketAPILogic(HttpClient httpClient, ILogger<KartverketAPILogic> logger)
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

    }
    
}