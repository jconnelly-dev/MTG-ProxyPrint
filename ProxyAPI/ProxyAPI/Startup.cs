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

namespace ProxyAPI
{
    public class Startup
    {
        #region Members.
        private readonly string _apiName;
        private readonly int _apiVersionMajor;
        private readonly int _apiVersionMinor;
        private readonly string _apiVersion;
        public IConfiguration Configuration { get; }
        #endregion

        #region Constructors.
        public Startup(IConfiguration configuration)
        {
            _apiName = "ProxyAPI";
            _apiVersionMajor = 1;
            _apiVersionMinor = 0;
            _apiVersion = $"{1}.{0}";
            Configuration = configuration;
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

            // Set the file upload size limit to an absolute max of 256 MB.
            services.Configure<FormOptions>(options =>
            {
                options.MultipartBodyLengthLimit = 268435456;
            });

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

            // Through dependency injection, instantiate a singleton of our API configs and pass to each service (i.e. each controller).
            ProxyConfigs singletonConfigs = null;
            services.AddSingleton<ProxyConfigs>((container) =>
            {
                ILogger<ProxyConfigs> logger = container.GetRequiredService<ILogger<ProxyConfigs>>();
                singletonConfigs = new ProxyConfigs(Configuration, logger, _apiName, _apiVersion, Environment.MachineName);
                return singletonConfigs;
            });

            // Add singleton of our consumer (of external magic APIs) to all controllers through dependency injection.
            services.AddSingleton<IMagicConsumer>((container) =>
            {
                ILogger<IMagicConsumer> logger = container.GetRequiredService<ILogger<IMagicConsumer>>();
                IMagicConsumer singletonConsumer = new WizardsConsumer(
                    downloadPath: singletonConfigs.DownloadPath,
                    apiDomain: singletonConfigs.ApiDomain,
                    apiVersion: singletonConfigs.ApiVersion,
                    apiResource: singletonConfigs.ApiResource,
                    apiRequestTimeout: singletonConfigs.ApiRequestTimeout);
                return singletonConsumer;
            });
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

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseSwagger();
            app.UseSwaggerUI(config =>
            {
                config.SwaggerEndpoint($"/swagger/{_apiVersion}/swagger.json", $"{_apiName} {_apiVersion}");
            });

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
