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
        private readonly CookieContainer _cookieContainer;

        public TokenService(HttpClient httpClient, IConfiguration configuration, ILogger<TokenService> logger)
        {
            _logger = logger;
            _cookieContainer = new CookieContainer();

            var handler = new HttpClientHandler
            {
                UseCookies = true,
                CookieContainer = _cookieContainer
            };

            _httpClient = new HttpClient(handler)
            {
                BaseAddress = new Uri(configuration["OAuth:ApiBaseAddress"])
            };

            _clientId = configuration["OAuth:ClientId"];
            _clientSecret = configuration["OAuth:ClientSecret"];
            _obtainTokenEndpoint = configuration["OAuth:ObtainTokenEndpoint"];
        }

        public async Task<string> ExchangeAuthorizationCodeForToken(string authorizationCode, string redirectUri, string accessTokenCookie)
        {
            var clientCredentials = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{_clientId}:{_clientSecret}"));
            var requestUri = new Uri(_httpClient.BaseAddress, _obtainTokenEndpoint);

            if (!string.IsNullOrEmpty(accessTokenCookie))
            {
                var cookieValue = ExtractTokenValue(accessTokenCookie);
                var cookie = new Cookie("Access-Token", cookieValue)
                {
                    Domain = requestUri.Host,
                    Secure = requestUri.Scheme == Uri.UriSchemeHttps,
                    HttpOnly = true,
                    Path = "/",
                    Expires = ExtractTokenExpiry(accessTokenCookie)
                };

                _cookieContainer.Add(requestUri, cookie);
            }

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

            // TODO: Wrap in retry logic: https://docs.microsoft.com/en-us/dotnet/architecture/microservices/implement-resilient-applications/implement-http-call-retries-exponential-backoff-polly
            var response = await _httpClient.SendAsync(requestMessage);
            var responseContent = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError($"Failed to exchange authorization code for token. Status: {response.StatusCode}");
                return null;
            }

            var tokenResponse = JsonSerializer.Deserialize<TokenResponse>(responseContent);

            // TODO: Error handling
            if (tokenResponse?.AccessToken is null)
            {
                _logger.LogError("No access token in the response.");
                return null;
            }
            return tokenResponse?.AccessToken;
        }
        private string ExtractTokenValue(string cookieHeader)
        {
            var parts = cookieHeader.Split(';');
            var valuePart = parts.FirstOrDefault(part => part.StartsWith("Access-Token="));
            if (valuePart != null)
            {
                // Assuming the name and value are separated by an equals sign
                return valuePart.Split('=')[1];
            }
            return string.Empty;
        }

        private DateTime ExtractTokenExpiry(string cookieHeader)
        {
            var parts = cookieHeader.Split(';');
            var expiryPart = parts.FirstOrDefault(part => part.TrimStart().StartsWith("Expires="));
            if (expiryPart != null)
            {
                var expiryString = expiryPart.Split('=')[1];
                if (DateTime.TryParse(expiryString, out var expiry))
                {
                    return expiry;
                }
            }
            return DateTime.MinValue; // or some default value
        }
    }
}
