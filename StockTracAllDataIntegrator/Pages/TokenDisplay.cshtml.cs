using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace StockTracAllDataIntegrator.Pages
{
    public class TokenDisplayModel : PageModel
    {
        [BindProperty(SupportsGet = true)]
        public string AccessToken { get; set; }

        public void OnGet()
        {
            // The access token is now available in the AccessToken property
        }
    }
}
