using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using StockTracAllDataIntegrator.Services;

namespace StockTracAllDataIntegrator.Pages
{
    public class CallbackModel : PageModel
    {
        private readonly ITokenService tokenService;
        private readonly IConfiguration configuration;
        private readonly IAllDataApiService _allDataApiService;

        public bool TokenExchangeSuccess { get; private set; }
        public string AccessToken { get; set; }
        public string CarComponents { get; set; }

        public CallbackModel(ITokenService tokenService, IConfiguration configuration, IAllDataApiService allDataApiService)
        {
            this.tokenService = tokenService;
            this.configuration = configuration;
            _allDataApiService = allDataApiService;
        }

        public async Task<IActionResult> OnGetAsync(string code, string state)
        {
            if (!string.IsNullOrEmpty(code))
            {
                try
                {
                    var redirectUri = configuration["OAuth:RedirectUri"];
                    AccessToken = await tokenService.ExchangeAuthorizationCodeForToken(code, redirectUri) ?? "No Token Found";
                    TokenExchangeSuccess = true;

                    // Instead of returning a page, you could also redirect to a different page
                    // or even back to the C++ app if it's listening for a custom URI scheme or on a local HTTP server
                    // e.g., return Redirect("customapp://token/" + AccessToken);

                    // call https://api-beta.alldata.com/ADAG/api/dws/ADConnect/v5/carids/57900/components/1?flatten=true with the access token to get the data
                    // Implement AllDataApiService to call the API and return the data
                    CarComponents = await _allDataApiService.GetCarComponentsAsync(AccessToken, 57900, 1, true);
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
