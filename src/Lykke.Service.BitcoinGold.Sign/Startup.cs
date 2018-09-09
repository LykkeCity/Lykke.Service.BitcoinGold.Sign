using System;
using System.Threading.Tasks;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Lykke.BitcoinGold.Sign.Services;
using Lykke.Common.ApiLibrary.Middleware;
using Lykke.Common.ApiLibrary.Swagger;
using Lykke.Logs;
using Lykke.Service.BitcoinGold.Sign.Core.Services;
using Lykke.Service.BitcoinGold.Sign.Core.Settings;
using Lykke.Service.BitcoinGold.Sign.Modules;
using Lykke.SettingsReader;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Lykke.Service.BitcoinGold.Sign
{
    public class Startup
    {
        public IHostingEnvironment Environment { get; }
        public IContainer ApplicationContainer { get; private set; }
        public IConfigurationRoot Configuration { get; }

        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddEnvironmentVariables();
            Configuration = builder.Build();

            Environment = env;
        }

        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            services.AddMvc()
                .AddJsonOptions(options =>
                {
                    options.SerializerSettings.ContractResolver =
                        new Newtonsoft.Json.Serialization.DefaultContractResolver();
                });

            services.AddSwaggerGen(options =>
            {
                options.DefaultLykkeConfiguration("v1", "BitcoinGold.Service.Sign");
            });
            services.AddEmptyLykkeLogging();


            var builder = new ContainerBuilder();
            var appSettings = Configuration.LoadSettings<AppSettings>();
            var modules = new Module[]
            {
                    new SignInModule(appSettings.Nested(x => x.BitcoinGoldSign)),
                    new ServiceModule(appSettings.Nested(x=>x.BitcoinGoldSign))
            };

            foreach (var module in modules)
            {
                builder.RegisterModule(module);
            }

            builder.Populate(services);
            ApplicationContainer = builder.Build();

            return new AutofacServiceProvider(ApplicationContainer);
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IApplicationLifetime appLifetime)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseLykkeMiddleware(ex => new { Message = "Technical problem" });

            app.UseMvc();
            app.UseSwagger();
            app.UseSwaggerUI(x =>
            {
                x.RoutePrefix = "swagger/ui";
                x.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
            });
            app.UseStaticFiles();

            appLifetime.ApplicationStarted.Register(() => StartApplication().GetAwaiter().GetResult());
            appLifetime.ApplicationStopping.Register(() => StopApplication().GetAwaiter().GetResult());
            appLifetime.ApplicationStopped.Register(CleanUp);
        }

        private async Task StartApplication()
        {
            await ApplicationContainer.Resolve<IStartupManager>().StartAsync();
        }

        private async Task StopApplication()
        {
            await ApplicationContainer.Resolve<IShutdownManager>().StopAsync();
        }

        private void CleanUp()
        {
            ApplicationContainer.Dispose();
        }
    }
}
