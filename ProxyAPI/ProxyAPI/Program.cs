using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProxyAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            /*
             * A .NET Generic Host in ASP.NET Core (IHostBuilder) will encapsulate this application's resources:
             *  Dependency injection, Logging, Configuration, and IHostedService implementations. The single Host
             *  is also used for lifetime managment: control over the app startup and graceful shutdown.
             * 
             * This .NET Core project begins as a console application (Main). We then start by creating a WebHost 
             *  w/default configurations, initialize by building, then starting the WebHost as an ASP.NET Core application.
             */
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureLogging((hostingContext, logging) =>
                {
                    // Define logging provider and configurations.
                    logging.ClearProviders();
                    logging.AddConfiguration(hostingContext.Configuration.GetSection("Logging"));
                    logging.AddConsole();
                    logging.AddDebug();
                    logging.AddEventSourceLogger();
                })
                .ConfigureAppConfiguration((hostingContext, configuration) =>
                {
                    configuration.Sources.Clear();
                    IHostEnvironment env = hostingContext.HostingEnvironment;
                    configuration
                        .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                        .AddJsonFile($"appsettings.{env.EnvironmentName}.json", true, true);
                })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    // Define Startup class as the source for configuring ASP.NET Core application.
                    webBuilder.UseStartup<Startup>();
                });
    }
}
