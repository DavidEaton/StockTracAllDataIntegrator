using Microsoft.AspNetCore.Mvc.RazorPages;
using Serilog;

namespace StockTracAllDataIntegrator.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;

        public IndexModel(ILogger<IndexModel> logger)
        {
            _logger = logger;
            Log.Information($"Application Base Directory: {AppDomain.CurrentDomain.BaseDirectory}");
            _logger.LogDebug($"Application Base Directory: {AppDomain.CurrentDomain.BaseDirectory}");
        }

        public void OnGet()
        {

        }
    }
}
