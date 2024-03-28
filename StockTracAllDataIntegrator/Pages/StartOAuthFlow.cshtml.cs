using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace StockTracAllDataIntegrator.Pages
{
    // Razor Page Model to initiate the OAuth flow
    public class StartOAuthFlowModel : PageModel
    {
        private readonly string clientId;
        private readonly string redirectUri;

        public StartOAuthFlowModel(IConfiguration configuration)
        {
            clientId = configuration["OAuth:ClientId"];
            redirectUri = configuration["OAuth:RedirectUri"];
        }

        public IActionResult OnGet()
        {
            var authorizationEndpoint = $"https://my.alldata.com/#/authorization";
            var url = $"{authorizationEndpoint}?client_id={clientId}&scope=read&response_type=code&redirect_uri={Uri.EscapeDataString(redirectUri)}";
            return Redirect(url);
        }
    }
}
