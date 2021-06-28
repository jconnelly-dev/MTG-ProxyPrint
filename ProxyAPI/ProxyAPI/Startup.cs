using System;
using System.IO;
using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using ProxyAPI.Security;
using MagicConsumer;
using MagicConsumer.WizardsAPI;
using Microsoft.AspNetCore.Http.Features;
using Serilog;
using Serilog.Events;

namespace ProxyAPI
{
    public class Startup
    {
        #region Members.
        private readonly string _apiName;
        private readonly int _apiVersionMajor;
        private readonly int _apiVersionMinor;
        private readonly string _apiVersion;
        private readonly IWebHostEnvironment _env;
        public IConfiguration _configuration { get; }
        #endregion

        #region Constructors.
        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            _env = env;
            _configuration = configuration;

            _apiName = "ProxyAPI";
            _apiVersionMajor = 1;
            _apiVersionMinor = 0;
            _apiVersion = $"{1}.{0}";
        }
        #endregion

        /*
         * ASP.NET Core framework will automatically invoke this method first at runtime.
         *  This method should be used to define global functionality (services) that needs to be  
         *  configured and shared to other parts of the application (i.e. database). The framework
         *  uses a built-in Dependency Injection container (IServiceCollection services) to register
         *  all application dependencies.
         */
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();

            services.AddSwaggerGen(config =>
            {
                config.SwaggerDoc(_apiVersion, new OpenApiInfo { Title = _apiName, Version = _apiVersion });

                config.OperationFilter<AddSwaggerAuthHeaderParams>();

                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                config.IncludeXmlComments(xmlPath);
            });

            services.AddApiVersioning(config =>
            {
                config.DefaultApiVersion = new ApiVersion(_apiVersionMajor, _apiVersionMinor);
                config.AssumeDefaultVersionWhenUnspecified = false;
                config.ReportApiVersions = true;
                config.ApiVersionReader = new UrlSegmentApiVersionReader();
            });

            // Add 3rd party serilog logger configured based on appsettings section.
            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(_configuration)
                .CreateLogger();

            // Get the settings for this project from the corresponding appsettings file sections.
            string solutionProfile = _env.IsProduction()
                ? AppOptions.Release
                : AppOptions.Development;
            IConfigurationSection projectSection = _configuration.GetSection(solutionProfile);
            IConfigurationSection consumerSection = _configuration.GetSection(ConsumerOptions.SectionName);

            // Use 'options pattern' to map the corresponding sections to objects that we can then log and validate.
            AppOptions configs = projectSection.Get<AppOptions>();
            ConsumerOptions consumerConfigs = consumerSection.Get<ConsumerOptions>();
            LogInitConfigurations(configs, consumerConfigs, solutionProfile);
            configs.Validate();
            consumerConfigs.Validate();           

            // Set the file upload size limit to an absolute max of 256 MB.
            services.Configure<FormOptions>(options =>
            {
                options.MultipartBodyLengthLimit = configs.MaxUploadFileByteSize;
            });

            // Add singletons to all controllers through dependency injection.
            IMagicConsumer consumer = new WizardsConsumer(consumerConfigs, configs.InputPath);
            services.AddSingleton(consumer);
            services.AddSingleton(Log.Logger);
            services.AddSingleton(configs);
        }

        private void LogInitConfigurations(AppOptions appConfigs, ConsumerOptions extConfigs, string solutionProfile)
        {
            Log.Logger.Write(LogEventLevel.Information, "--------------------------------------------------");
            Log.Logger.Write(LogEventLevel.Information, "Initializing API...");
            Log.Logger.Write(LogEventLevel.Verbose, "Logging Verbose Enabled...");
            Log.Logger.Write(LogEventLevel.Debug, "Logging Debug Enabled...");
            Log.Logger.Write(LogEventLevel.Information, "Logging Information Enabled...");
            Log.Logger.Write(LogEventLevel.Warning, "Logging Warning Enabled...");
            Log.Logger.Write(LogEventLevel.Error, "Logging Error Enabled...");
            Log.Logger.Write(LogEventLevel.Fatal, "Logging Fatal Enabled...");
            Log.Logger.Write(LogEventLevel.Information, $"ProjectName={_env.ApplicationName}");
            Log.Logger.Write(LogEventLevel.Information, $"ProjectVersion={_apiVersion}");
            Log.Logger.Write(LogEventLevel.Information, $"MachineName={Environment.MachineName}");
            Log.Logger.Write(LogEventLevel.Information, $"EnvironmentName={_env.EnvironmentName}");
            Log.Logger.Write(LogEventLevel.Information, $"SolutionProfile={solutionProfile}");
            Log.Logger.Write(LogEventLevel.Information, "Application Configurations ->");
            Log.Logger.Write(LogEventLevel.Information, $"InputPath={appConfigs.InputPath}");
            Log.Logger.Write(LogEventLevel.Information, $"OutputPath={appConfigs.OutputPath}");
            Log.Logger.Write(LogEventLevel.Information, $"DownloadPath={appConfigs.DownloadPath}");
            Log.Logger.Write(LogEventLevel.Information, $"MaxUploadFileByteSize={appConfigs.MaxUploadFileByteSize}");
            Log.Logger.Write(LogEventLevel.Information, "Consumer Configurations ->");
            Log.Logger.Write(LogEventLevel.Information, $"Domain={extConfigs.Domain}");
            Log.Logger.Write(LogEventLevel.Information, $"Version={extConfigs.Version}");
            Log.Logger.Write(LogEventLevel.Information, $"Resource={extConfigs.Resource}");
            Log.Logger.Write(LogEventLevel.Information, $"RequestTimeoutSeconds={extConfigs.RequestTimeoutSeconds}");
            Log.Logger.Write(LogEventLevel.Information, "--------------------------------------------------");
        }

        /*
         * ASP.NET Core framework will automatically invoke this method at runtime after ConfigureServices.
         *  This method should be used to configure the HTTP request pipeline. The pipeline will determine how the 
         *  application will respond to an HTTP request and consists of ordered Middleware that executes on any
         *  incoming requests (can include: authentication, autherization, static files, mvc middleware, and more).
         *  IWebHostEnvironment can be used to check specific environment conditions.
         *  IApplicationBuilder is used to configure the HTTP request pipeline of ASP.NET Core.
         */
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.Use(async (context, next) =>
            {
                // X-FRAME-OPTIONS is a web header that can be used to allow or deny a page to be iframed. 
                //  Adding this to each response header will help protect against clickjacking attempts.
                context.Response.Headers.Add("X-Frame-Options", "SAMEORIGIN");
                await next();
            });

            if (env.IsProduction())
            {
                app.UseExceptionHandler("/Error");
            }
            else
            {
                app.UseDeveloperExceptionPage();
            }

            // Use serilog framework instead of builtin microsoft builtin logging.
            app.UseSerilogRequestLogging();

            app.UseSwagger();
            app.UseSwaggerUI(config =>
            {
                config.SwaggerEndpoint($"/swagger/{_apiVersion}/swagger.json", $"{_apiName} {_apiVersion}");
            });

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
