using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using StockTracAllDataIntegrator.Models;
using System.Text.Json;

namespace StockTracAllDataIntegrator.Pages
{
    public class ApiResultsModel : PageModel
    {
        [BindProperty(SupportsGet = true)]
        public string? AccessToken { get; set; }

        [BindProperty(SupportsGet = true)]
        public CarComponentsModel CarComponents { get; set; } = new CarComponentsModel();

        public void OnGet()
        {
            var carComponentsJson = TempData["CarComponents"] as string;
            if (!string.IsNullOrEmpty(carComponentsJson))
            {
                CarComponents = JsonSerializer.Deserialize<CarComponentsModel>(carComponentsJson);
            }
        }

    }
}
