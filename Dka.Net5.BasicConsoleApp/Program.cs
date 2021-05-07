using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Dka.Net5.BasicConsoleApp.Core.Logic;
using Dka.Net5.BasicConsoleApp.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Scrutor;
using Serilog;

namespace Dka.Net5.BasicConsoleApp
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development";
            
            var host = new HostBuilder()
                .UseEnvironment(environment)
                .ConfigureAppConfiguration(ConfigureConfiguration)
                .ConfigureServices(ConfigureServices)
                .ConfigureLogging(ConfigureLogging)
                .UseConsoleLifetime()
                .Build();

            var logger = host.Services.GetService<ILogger<Program>>();
            var hostedServices = host.Services.GetService<IEnumerable<IHostedService>>()?.Select(t => (ICustomHostedService)t) ?? Enumerable.Empty<ICustomHostedService>();
            
            using (host)
            {
                logger.LogInformation("Program started {DateTime} UTC", DateTime.UtcNow);

                await host.StartAsync();
                WaitHostedServicesCompleted(hostedServices);
                await host.StopAsync();

                logger.LogInformation("Program finished {DateTime} UTC", DateTime.UtcNow);
            }
        }
        
        private static void ConfigureConfiguration(HostBuilderContext hostBuilderContext, IConfigurationBuilder configurationBuilder)
        {
            configurationBuilder.SetBasePath(Directory.GetCurrentDirectory());
            configurationBuilder.AddEnvironmentVariables();
            configurationBuilder.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
            configurationBuilder.AddJsonFile($"appsettings.{hostBuilderContext.HostingEnvironment}.json", optional: true, reloadOnChange: true);            
        }
        
        private static void ConfigureServices(IServiceCollection services)
        {
            services.Scan(scan =>
            {
                scan
                    .FromAssemblyOf<ISampleLogicA>().AddClasses().UsingRegistrationStrategy(RegistrationStrategy.Skip).AsImplementedInterfaces().WithScopedLifetime()
                    .FromAssemblyOf<SampleLogicA>().AddClasses().UsingRegistrationStrategy(RegistrationStrategy.Skip).AsSelf().WithScopedLifetime();
            });

            services.AddHostedService<ServiceA>();
            services.AddHostedService<ServiceB>();
        }

        private static void ConfigureLogging(HostBuilderContext hostBuilderContext, ILoggingBuilder loggingBuilder)
        {
            var logger = new LoggerConfiguration()
                .Enrich.WithProperty("AppName", "Basic console app")
                .Enrich.WithProperty("Solution", "Dka.Net5.BasicConsoleApp")
                .Enrich.WithProperty("EnvironmentName", hostBuilderContext.HostingEnvironment)
                .MinimumLevel.Information()
                .WriteTo.Console()
                .WriteTo.File("BasicConsoleApp.log")
                .CreateLogger();

            loggingBuilder.AddSerilog(logger);
        }

        private static void WaitHostedServicesCompleted(IEnumerable<ICustomHostedService> hostedServices)
        {
            Task.WaitAll(hostedServices.Select(t => Task.Run(() =>
            {
                while (!t.CancellationToken.IsCancellationRequested)
                { }
            })).ToArray());
        }
    }
}