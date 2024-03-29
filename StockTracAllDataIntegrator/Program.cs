using Serilog;
using StockTracAllDataIntegrator.Services;

namespace StockTracAllDataIntegrator
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var logFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Logs", "StockTracAllDataIntegrator-.txt");

            // Ensure the Logs directory exists
            var logDirectory = Path.GetDirectoryName(logFilePath);
            if (!Directory.Exists(logDirectory))
            {
                Directory.CreateDirectory(logDirectory);
            }

            // Configure Serilog first
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.Console()
                .WriteTo.File(logFilePath,
                              rollingInterval: RollingInterval.Day,
                              outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}")
                .CreateLogger();

            Log.Information($"Application Base Directory: {AppDomain.CurrentDomain.BaseDirectory}");
            Log.Information($"Logging to: {logFilePath}");


            // Log.Logger should be set before creating the builder so that we can log from there
            try
            {
                Log.Information("Starting web host");

                var builder = WebApplication.CreateBuilder(args);

                // Add services to the container with the new Logger
                builder.Host.UseSerilog(); // This will use the Serilog settings you've configured above

                // Add services to the container.
                builder.Services.AddRazorPages();
                builder.Services.AddHttpClient<ITokenService, TokenService>();

                var app = builder.Build();

                // Configure the HTTP request pipeline.
                if (!app.Environment.IsDevelopment())
                {
                    app.UseExceptionHandler("/Error");
                    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                    app.UseHsts();
                }

                app.UseHttpsRedirection();
                app.UseStaticFiles();

                app.UseRouting();

                app.UseAuthorization();

                app.MapRazorPages();

                // Change the default route
                app.MapGet("/", () => Results.Redirect("/StartOAuthFlow"));

                app.Run();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Host terminated unexpectedly");
            }
            finally
            {
                Log.CloseAndFlush(); // Ensure to flush and stop logging on shutdown
            }
        }
    }
}
