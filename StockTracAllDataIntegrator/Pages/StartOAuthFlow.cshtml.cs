using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

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
            // Generate the URL for the ALLDATA authorization endpoint
            var state = Guid.NewGuid().ToString(); // Generate a unique state value for CSRF protection
            var scope = "read"; // Specify the scope of access you are requesting
            var responseType = "code"; // We want an authorization code response

            // Construct the authorization URL
            var authorizationUrl = $"{authorizationEndpoint}?response_type={responseType}&client_id={clientId}&scope={scope}&redirect_uri={Uri.EscapeDataString(redirectUri)}&state={state}";

            // Redirect the user to the ALLDATA authorization endpoint
            return Redirect(authorizationUrl);
        }

    }
}
