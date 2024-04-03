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

        public async Task<IActionResult> OnGetAsync(string code, string state)
        {
            _logger.LogInformation("Callback endpoint hit.");
            _logger.LogInformation("Headers: {@Headers}", Request.Headers);
            _logger.LogInformation("Cookies: {@Cookies}", Request.Cookies);

            if (!string.IsNullOrEmpty(code))
            {
                try
                {
                    var redirectUri = configuration["OAuth:RedirectUri"];
                    var accessTokenCookie = Request.Cookies["Access-Token"];

                    if (string.IsNullOrEmpty(accessTokenCookie))
                    {
                        // Handle the missing cookie scenario
                        // This could involve redirecting the user to log in again or showing an error message
                    }
                    else
                    {
                        AccessToken = await tokenService.ExchangeAuthorizationCodeForToken(code, redirectUri, accessTokenCookie) ?? "No Token Found";
                        TokenExchangeSuccess = !string.IsNullOrEmpty(AccessToken);

                        if (TokenExchangeSuccess)
                        {
                            // Your existing logic for using the access token
                            // ...
                        }
                    }

                    TokenExchangeSuccess = true;

                    // call https://api-beta.alldata.com/ADAG/api/dws/ADConnect/v5/carids/57900/components/1?flatten=true with the access token to get the data
                    // Implement AllDataApiService to call the API and return the data
                    var carComponents = await _allDataApiService.GetCarComponentsAsync(AccessToken, 57900, 1, true);
                    CarComponents = JsonSerializer.Deserialize<CarComponentsModel>(carComponents);

                }
                catch (Exception ex)
                {
                    // Log the exception
                    TokenExchangeSuccess = false;
                    // Handle the error, possibly return an error page or log the details
                }
            }
            else
            {
                // Log the error, the code parameter is missing from the query string
                TokenExchangeSuccess = false;
                // Handle the error
            }

            if (TokenExchangeSuccess)
            {
                return RedirectToPage("/TokenDisplay", new { AccessToken, CarComponents });
            }
            else
            {
                return RedirectToPage("/Error");
            }
        }
    }
}
