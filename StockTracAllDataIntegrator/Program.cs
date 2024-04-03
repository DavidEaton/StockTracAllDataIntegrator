using Serilog;
using Serilog.Events;
using Serilog.Formatting.Json;
using Serilog.Sinks.SystemConsole.Themes;
using StockTracAllDataIntegrator.Services;

namespace StockTracAllDataIntegrator
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var logFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\..\Logs\StockTracAllDataIntegrator-log-.json");

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                .MinimumLevel.Override("Microsoft.Hosting.Lifetime", LogEventLevel.Information)
                .MinimumLevel.Override("System", LogEventLevel.Warning)
                .MinimumLevel.Override("Microsoft.AspNetCore.Authentication", LogEventLevel.Information)
                .Enrich.FromLogContext()
                .WriteTo.File(new JsonFormatter(), logFilePath, rollingInterval: RollingInterval.Day)
                .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level}] {SourceContext}{NewLine}{Message:lj}{NewLine}{Exception}{NewLine}", theme: AnsiConsoleTheme.Code)
                .CreateLogger();

            Log.Information($"Application log File Path: {logFilePath}");

            try
            {
                Log.Information("Starting web host");

                var builder = WebApplication.CreateBuilder(args);

                builder.Host.UseSerilog();

                // Add services to the container.
                builder.Services.AddRazorPages();
                builder.Services.AddHttpClient();
                builder.Services.AddHttpClient<ITokenService, TokenService>();
                builder.Services.AddHttpClient<IAllDataApiService, AllDataApiService>();

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
                app.MapGet("/", () => Results.Redirect("/LoginModel"));

                app.Run();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Host terminated unexpectedly");
            }
            finally
            {
                // Ensure to flush and stop logging on shutdown
                Log.CloseAndFlush();
            }
        }
    }
}
