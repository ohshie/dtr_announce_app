using System.Drawing;
using Dtr_Announce_App.Business;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using PhotinoNET;
using Serilog;

namespace Dtr_Announce_App
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            var app = ConfigureServices(args);

            app.UseCors("CorsPolicy");

            app.MapControllers();
            
            app.UseSwagger();
            app.UseSwaggerUI();
            
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

            builder.Services.AddTransient<VkVideoUrlFetcher>();
            builder.Services.AddTransient<DateConverter>();
            builder.Services.AddTransient<AnnounceCreator>();
            builder.Services.AddTransient<AnnouncePoster>();
            
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Information()
                .Enrich.FromLogContext()
                .WriteTo.Console()
                .CreateLogger();

            builder.Services.AddControllers();
            
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy",
                    configurePolicy => configurePolicy.WithOrigins("file://", "http://localhost:5173")
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        .AllowCredentials());
            });

            builder.Services.AddHttpClient();
            builder.Services.AddSerilog();
            
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            
            return builder.Build();
        }
    }
}
