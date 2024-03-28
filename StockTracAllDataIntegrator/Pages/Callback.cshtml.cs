using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using StockTracAllDataIntegrator.Services;

namespace StockTracAllDataIntegrator.Pages
{
    public class CallbackModel : PageModel
    {
        private readonly ITokenService tokenService;
        private readonly IConfiguration configuration;
        public string AccessToken { get; set; }

        public CallbackModel(ITokenService tokenService, IConfiguration configuration)
        {
            this.tokenService = tokenService;
            this.configuration = configuration;
        }

        public async Task<IActionResult> OnGet(string code, string state)
        {
            var redirectUri = configuration["OAuth:RedirectUri"];

            // Exchange code for token
            AccessToken = await tokenService.ExchangeAuthorizationCodeForToken(code, redirectUri);

            // Pass the token back to the C++ application
            // Here you need to implement the logic to pass the access token back to the C++ app.
            // This could be done by setting the token in a location the C++ app can access it, like a database or cache.

            return Page();
        }

    }
}
