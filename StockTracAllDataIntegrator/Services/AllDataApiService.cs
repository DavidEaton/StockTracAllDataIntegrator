using System.Net.Http.Headers;

namespace StockTracAllDataIntegrator.Services
{
    public class AllDataApiService : IAllDataApiService
    {
        private readonly HttpClient _httpClient;

        public AllDataApiService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<string> GetCarComponentsAsync(string accessToken, int carId, int componentId, bool flatten)
        {
            var requestUri = $"https://api-beta.alldata.com/ADAG/api/dws/ADConnect/v5/carids/{carId}/components/{componentId}?flatten={flatten}";

            _httpClient.DefaultRequestHeaders.Accept.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/hal+json"));
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            var response = await _httpClient.GetAsync(requestUri);
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            return content;
        }
    }
}
