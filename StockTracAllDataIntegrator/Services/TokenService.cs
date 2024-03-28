using System.Text.Json;

namespace StockTracAllDataIntegrator.Services
{
    public class TokenService : ITokenService
    {
        private readonly ILogger<TokenService> _logger;
        private readonly HttpClient _httpClient;
        private readonly string _clientId;
        private readonly string _clientSecret;

        public TokenService(HttpClient httpClient, IConfiguration configuration, ILogger<TokenService> logger)
        {
            _logger = logger;
            _httpClient = httpClient;
            _clientId = configuration["OAuth:ClientId"];
            _clientSecret = configuration["OAuth:ClientSecret"];
        }

        public async Task<string> ExchangeAuthorizationCodeForToken(string authorizationCode, string redirectUri)
        {
            var requestContent = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                ["grant_type"] = "authorization_code",
                ["code"] = authorizationCode,
                ["redirect_uri"] = redirectUri,
                ["client_id"] = _clientId,
                ["client_secret"] = _clientSecret
            });

            var response = await _httpClient.PostAsync("https://api.alldata.com/ADAG/oauth/token", requestContent);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError($"Failed to exchange authorization code for token. Status: {response.StatusCode}");
                throw new ApplicationException($"Failed to exchange authorization code for token. Status: {response.StatusCode}");
            }

            var responseContent = await response.Content.ReadAsStringAsync();
            var tokenResponse = JsonSerializer.Deserialize<TokenResponse>(responseContent);
            if (tokenResponse?.AccessToken == null)
            {
                _logger.LogError("No access token in the response.");
                throw new InvalidOperationException("No access token in the response.");
            }

            return tokenResponse.AccessToken;
        }
    }

}
