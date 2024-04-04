using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using StockTracAllDataIntegrator.Models;
using StockTracAllDataIntegrator.Services;
using System.Text.Json;

namespace StockTracAllDataIntegrator.Pages
{
    public class CallbackModel : PageModel
    {
        private readonly ITokenService tokenService;
        private readonly IConfiguration configuration;
        private readonly IAllDataApiService _allDataApiService;
        private readonly ILogger<CallbackModel> _logger;

        public bool TokenExchangeSuccess { get; private set; }
        public string AccessToken { get; set; }
        public CarComponentsModel? CarComponents { get; set; }

        public CallbackModel(ITokenService tokenService, IConfiguration configuration, IAllDataApiService allDataApiService, ILogger<CallbackModel> logger)
        {
            this.tokenService = tokenService;
            this.configuration = configuration;
            _allDataApiService = allDataApiService;
            _logger = logger;
        }

        public async Task<IActionResult> OnGetAsync(string code)
        {
            if (!string.IsNullOrEmpty(code))
            {
                try
                {
                    var redirectUri = configuration["OAuth:RedirectUri"];
                    var accessTokenCookie = Request.Cookies["Access-Token"];

                    AccessToken = await tokenService.ExchangeAuthorizationCodeForToken(code, redirectUri, accessTokenCookie) ?? "No Token Found";
                    TokenExchangeSuccess = AccessToken != "No Token Found";

                    if (TokenExchangeSuccess)
                    {
                        var carComponents = await _allDataApiService.GetCarComponentsAsync(AccessToken, 57900, 1, true);

                        var options = new JsonSerializerOptions
                        {
                            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                            PropertyNameCaseInsensitive = true,
                        };

                        CarComponents = JsonSerializer.Deserialize<CarComponentsModel>(carComponents, options);
                    }

                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error exchanging authorization code for token.");
                    TokenExchangeSuccess = false;
                    // TODO: Handle the error
                }
            }
            else
            {
                _logger.LogError("Code parameter is missing from the query string.");
                TokenExchangeSuccess = false;
                // TODO: Handle the error
            }

            if (TokenExchangeSuccess)
            {
                return RedirectToPage($"/ApiResultsDisplay?accessToken={AccessToken}");
            }
            else
            {
                return RedirectToPage("/Error");
            }
        }
    }
}
