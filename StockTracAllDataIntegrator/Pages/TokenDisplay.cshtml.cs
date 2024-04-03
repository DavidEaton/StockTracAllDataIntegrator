using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using StockTracAllDataIntegrator.Models;

namespace StockTracAllDataIntegrator.Pages
{
    public class TokenDisplayModel : PageModel
    {
        [BindProperty(SupportsGet = true)]
        public string AccessToken { get; set; }

        [BindProperty(SupportsGet = true)]
        public CarComponentsModel CarComponents { get; set; } = new CarComponentsModel();

        public void OnGet()
        {
        }
    }
}
