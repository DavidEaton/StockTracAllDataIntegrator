using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace StockTracAllDataIntegrator.Pages
{
    public class LoginModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;
        private readonly ILogger<LoginModel> _logger;

        public LoginModel(IHttpClientFactory httpClientFactory, IConfiguration configuration, ILogger<LoginModel> logger)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
            _logger = logger;
        }

        [BindProperty]
        public string Username { get; set; }

        [BindProperty]
        public string Password { get; set; }

        public IActionResult OnGet()
        {
            _logger.LogInformation("Login page accessed via GET request.");
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            _logger.LogInformation("Login page accessed via POST request.");

            var loginUrl = _configuration["OAuth:LoginEndpoint"];

            var httpClient = _httpClientFactory.CreateClient();
            var loginContent = new StringContent($"{{\"username\":\"{Username}\",\"password\":\"{Password}\"}}", System.Text.Encoding.UTF8, "application/json");

            var response = await httpClient.PostAsync(loginUrl, loginContent);

            if (response.IsSuccessStatusCode)
            {
                var cookieName = "Access-Token";
                string? accessToken = GetCookie(response, cookieName);

                AppendCookie(cookieName, accessToken);

                cookieName = "Access-Token-Refresh";
                string? refreshToken = GetCookie(response, cookieName);

                AppendCookie(cookieName, refreshToken);

                if (!string.IsNullOrEmpty(accessToken) && !string.IsNullOrEmpty(refreshToken))
                {
                    return RedirectToPage("StartOAuthFlow");
                }
            }

            // Handle failure: show an error message or redirect to a failure page
            ModelState.AddModelError(string.Empty, "Login failed. Please check your username and password.");
            return Page();
        }

        private void AppendCookie(string cookieName, string? accessToken)
        {
            if (!string.IsNullOrEmpty(accessToken))
            {
                var accessTokenValue = ExtractTokenValue(accessToken);
                Response.Cookies.Append(cookieName, accessTokenValue, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    Domain = ".alldata.com",
                    Path = "/",
                    Expires = GetCookieExpiry(accessToken)
                });
            }
        }

        private static string? GetCookie(HttpResponseMessage response, string cookieName)
        {
            return response.Headers.GetValues("Set-Cookie")
                .FirstOrDefault(cookie => cookie
                .StartsWith(cookieName));
        }

        // Utility methods to extract token value and expiry from the Set-Cookie header
        private string ExtractTokenValue(string cookie)
        {
            // Assuming the token is the first value in the cookie string
            return cookie.Split(';').FirstOrDefault()?.Split('=').LastOrDefault() ?? string.Empty;
        }

        private DateTimeOffset? GetCookieExpiry(string cookie)
        {
            var expiryString = cookie.Split(';').FirstOrDefault(c => c.TrimStart().StartsWith("Expires="))?.Split('=').LastOrDefault();
            if (DateTimeOffset.TryParse(expiryString, out var expiry))
            {
                return expiry;
            }
            return null; // Or set a default expiry if needed
        }

    }
}
