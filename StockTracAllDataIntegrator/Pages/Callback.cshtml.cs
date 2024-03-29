using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using StockTracAllDataIntegrator.Services;

namespace StockTracAllDataIntegrator.Pages
{
    public class CallbackModel : PageModel
    {
        private readonly ITokenService tokenService;
        private readonly IConfiguration configuration;
        public bool TokenExchangeSuccess { get; private set; }
        public string AccessToken { get; set; }

        public CallbackModel(ITokenService tokenService, IConfiguration configuration)
        {
            this.tokenService = tokenService;
            this.configuration = configuration;
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
                return RedirectToPage("/TokenDisplay", new { AccessToken });
            }
            else
            {
                return RedirectToPage("/Error");
            }
        }

    }
}
