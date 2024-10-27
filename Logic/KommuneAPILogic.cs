using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Interface;
using System.Collections.Generic;
using Models.Models;

namespace Logic
{
    public class KartverketAPILogic : IKartverketAPILogic
    {
        private readonly HttpClient _httpClient;

        public KartverketAPILogic(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        // Henter kommunedata fra Kartverkets API
        public async Task<List<Kommune>> GetKommunerAsync()
        {
            var response = await _httpClient.GetAsync("https://api.kartverket.no/kommuneinfo/v1/kommuner");
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<List<Kommune>>(content); // Returner som Liste av Kommune-objekter
        }
    }
}
