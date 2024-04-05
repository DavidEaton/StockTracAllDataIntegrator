using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using StockTracAllDataIntegrator.Models;
using StockTracAllDataIntegrator.Services;

namespace StockTracAllDataIntegrator.Pages
{
    public class CallbackModel : PageModel
    {
        private readonly ITokenService tokenService;
        private readonly IConfiguration configuration;
        private readonly IAllDataApiService _allDataApiService;
        private readonly ILogger<CallbackModel> _logger;

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
            if (string.IsNullOrEmpty(code))
            {
                _logger.LogError("Authorization code is missing from the callback.");
                return RedirectToPage("/Error");
            }

            try
            {
                var redirectUri = configuration["OAuth:RedirectUri"];
                var accessTokenCookie = Request.Cookies["Access-Token"];

                AccessToken = await tokenService.ExchangeAuthorizationCodeForToken(code, redirectUri, accessTokenCookie) ?? "No Token Found";

                if (AccessToken != "No Token Found")
                {
                    var carComponentsJson = await _allDataApiService.GetCarComponentsAsync(AccessToken, 57900, 1, true);

                    if (!string.IsNullOrEmpty(carComponentsJson))
                    {
                        TempData["CarComponents"] = carComponentsJson;
                    }
                    else
                    {
                        _logger.LogError("No car components data was returned from the service.");
                        return RedirectToPage("/Error");
                    }

                    return RedirectToPage("/ApiResults", new { accessToken = AccessToken });
                }
                else
                {
                    _logger.LogError("No valid access token found.");
                    return RedirectToPage("/Error");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error exchanging authorization code for token.");
                return RedirectToPage("/Error");
            }
        }
    }
}