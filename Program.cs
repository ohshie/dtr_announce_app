using System.Drawing;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using PhotinoNET;
using Serilog;

namespace HelloPhotino.NET
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            var app = ConfigureServices(args);

            app.UseCors("CorsPolicy");

            app.MapControllers();

            app.RunAsync();
            
            // Window title declared here for visibility
            string windowTitle = "DTR announce helper";
            
            var window = new PhotinoWindow()
                .SetTitle(windowTitle)
                // Resize to a percentage of the main monitor work area
                .SetUseOsDefaultSize(false)
                .SetSize(new Size(600, 400))
                .Center()
                .SetResizable(true)

                .Load("wwwroot/index.html"); // Can be used with relative path strings or "new URI()" instance to load a website.

            window.WaitForClose(); // Starts the application event loop
        }

        private static WebApplication ConfigureServices(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Information()
                .Enrich.FromLogContext()
                .WriteTo.Console()
                .CreateLogger();

            builder.Services.AddControllers();
            
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy",
                    configurePolicy => configurePolicy.WithOrigins("http://localhost:5000")
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        .AllowCredentials());
            });

            builder.Services.AddHttpClient();
            builder.Services.AddSerilog();
            
            return builder.Build();
        }
    }
}
