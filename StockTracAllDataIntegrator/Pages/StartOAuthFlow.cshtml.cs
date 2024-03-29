using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Serilog;

namespace StockTracAllDataIntegrator.Pages
{
    // Razor Page Model to initiate the OAuth flow
    public class StartOAuthFlowModel : PageModel
    {
        private readonly string clientId;
        private readonly string redirectUri;
        private readonly string authorizationEndpoint;

        public StartOAuthFlowModel(IConfiguration configuration)
        {
            clientId = configuration["OAuth:ClientId"];
            redirectUri = configuration["OAuth:RedirectUri"];
            authorizationEndpoint = configuration["OAuth:AuthorizationEndpoint"];
        }

        public IActionResult OnGet()
        {
            Log.Information($"Application Base Directory: {AppDomain.CurrentDomain.BaseDirectory}");

            var url = $"{authorizationEndpoint}?client_id={clientId}&scope=read&response_type=code&redirect_uri={Uri.EscapeDataString(redirectUri)}";
            return Redirect(url);
        }
    }
}
