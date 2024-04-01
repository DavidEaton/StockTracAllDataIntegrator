using System.Net;
using System.Text;
using System.Text.Json;

namespace StockTracAllDataIntegrator.Services
{
    public class TokenService : ITokenService
    {
        private readonly ILogger<TokenService> _logger;
        private readonly HttpClient _httpClient;
        private readonly string _clientId;
        private readonly string _clientSecret;
        private readonly string _obtainTokenEndpoint;

        public TokenService(HttpClient httpClient, IConfiguration configuration, ILogger<TokenService> logger)
        {
            _logger = logger;
            _httpClient = httpClient;
            _clientId = configuration["OAuth:ClientId"];
            _clientSecret = configuration["OAuth:ClientSecret"];
            _obtainTokenEndpoint = configuration["OAuth:ObtainTokenEndpoint"];
        }

        public async Task<string> ExchangeAuthorizationCodeForToken(string authorizationCode, string redirectUri)
        {
            var clientCredentials = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{_clientId}:{_clientSecret}"));
            using var requestMessage = new HttpRequestMessage(HttpMethod.Post, _obtainTokenEndpoint)
            {
                Content = new FormUrlEncodedContent(new Dictionary<string, string>
                {
                    ["grant_type"] = "authorization_code",
                    ["code"] = authorizationCode,
                    ["redirect_uri"] = redirectUri,
                    ["client_id"] = _clientId,
                    ["scope"] = "read"
                }),
                Headers =
                {
                    { HttpRequestHeader.Authorization.ToString(), $"Basic {clientCredentials}" }
                }
            };


            // Wrap in retry logic: https://docs.microsoft.com/en-us/dotnet/architecture/microservices/implement-resilient-applications/implement-http-call-retries-exponential-backoff-polly
            var response = await _httpClient.SendAsync(requestMessage);
            var responseContent = await response.Content.ReadAsStringAsync();

            _logger.LogInformation($"Response Status Code: {response.StatusCode}");
            _logger.LogInformation($"Response Content: {responseContent}");

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError($"Failed to exchange authorization code for token. Status: {response.StatusCode}");
                return null;
                //throw new ApplicationException($"Failed to exchange authorization code for token. Status: {response.StatusCode}");
            }

            var tokenResponse = JsonSerializer.Deserialize<TokenResponse>(responseContent);
            if (tokenResponse?.AccessToken == null)
            {
                _logger.LogError("No access token in the response.");
                return null;
                //throw new InvalidOperationException("No access token in the response.");
            }

            _logger.LogInformation("Access token obtained successfully.");
            return tokenResponse.AccessToken;
        }
    }

}
