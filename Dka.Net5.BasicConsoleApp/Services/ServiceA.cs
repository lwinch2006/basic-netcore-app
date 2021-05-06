using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Dka.Net5.BasicConsoleApp.Services
{
    public class ServiceA : BackgroundService
    {
        private readonly ILogger<ServiceA> _logger;
        
        public ServiceA(ILogger<ServiceA> logger)
        {
            _logger = logger;
        }
        
        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            cancellationToken.Register(() => _logger.LogInformation("{ServiceName} cancellation token triggered", nameof(ServiceA)));
            await Run();
            _logger.LogInformation("{ServiceName} finished work", nameof(ServiceA));
        }
        
        private Task Run()
        {
            return Task.CompletedTask;
        }        
    }
}