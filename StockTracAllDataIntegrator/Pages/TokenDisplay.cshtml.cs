using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace StockTracAllDataIntegrator.Pages
{
    public class TokenDisplayModel : PageModel
    {
        [BindProperty(SupportsGet = true)]
        public string AccessToken { get; set; }

        [BindProperty(SupportsGet = true)]
        public string CarComponents { get; set; }

        public void OnGet()
        {
        }
    }
}
