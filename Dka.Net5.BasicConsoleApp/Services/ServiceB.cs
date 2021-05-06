using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Dka.Net5.BasicConsoleApp.Services
{
    public class ServiceB : BackgroundService
    {
        private readonly ILogger<ServiceB> _logger;
        
        public ServiceB(ILogger<ServiceB> logger)
        {
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            cancellationToken.Register(() => _logger.LogInformation("{ServiceName} cancellation token triggered", nameof(ServiceB)));
            await Run();
            _logger.LogInformation("{ServiceName} finished work", nameof(ServiceB));
        }

        private Task Run()
        {
            return Task.CompletedTask;
        }
    }
}