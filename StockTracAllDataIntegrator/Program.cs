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

                var AccessToken = "eyJhbGciOiJIUzUxMiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiJqYW53QHN0b2NrdHJhYy5jb20iLCJ1c2VyX25hbWUiOiJqYW53QHN0b2NrdHJhYy5jb20iLCJpcCI6Ijk3LjExMi41LjE4MCIsImFtciI6ImFsbGRhdGE6b2F1dGg6YXV0aG9yaXphdGlvbl9jb2RlIiwiaXNzIjoiYXBpLWJldGEuYWxsZGF0YS5jb20iLCJ0eXBlIjoiRVhURVJOQUwiLCJjbGllbnRfaWQiOiIyYWM5NDI0MC0wMjg1LTQ1ZDMtOTZjOS1lYzYyZGEzOGFlMDQiLCJjbGllbnRfc2l0ZV9pZCI6MTQyNjkwLCJhY3IiOiJhY20iLCJ0ZW1wX3Bhc3MiOiJmYWxzZSIsInVzZXJfaWQiOjE3NTYyNSwic2NvcGUiOlsicmVhZCJdLCJzaXRlX2lkIjoxNDI2OTAsImV4cCI6MTcxMjI4NzczNSwiaWF0IjoxNzEyMjQ0NTM1LCJqdGkiOiIzMzI2ZWRiMC1lYTU4LTQxMTAtYTkxMS1jMGU1Zjc1YWI1MDkifQ.168ksXHZSsfPM2GOxpNUuNxmt3HOm-HXkVMSYzDDtC9TY8m1gcGvB-6jh85xquxsebqKz6_yRg10sfgR7wKy_A";

                // Change the default route
                app.MapGet("/", () => Results.Redirect("/Login"));
                //app.MapGet("/", () => Results.Redirect($"/ApiResultsDisplay?accessToken={AccessToken}"));

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
