using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Dka.Net5.BasicConsoleApp.Services
{
    public class ServiceB : BackgroundService, ICustomHostedService
    {
        private readonly ILogger<ServiceB> _logger;
        private readonly IHostApplicationLifetime _hostApplicationLifetime;

        private CancellationToken _cancellationToken;
        private CancellationTokenSource _cancellationTokenSource;

        public CancellationToken CancellationToken => _cancellationTokenSource.Token;

        public ServiceB(
            ILogger<ServiceB> logger,
            IHostApplicationLifetime hostApplicationLifetime)
        {
            _logger = logger;
            _hostApplicationLifetime = hostApplicationLifetime;
        }

        protected override Task ExecuteAsync(CancellationToken cancellationToken)
        {
            Init(cancellationToken);
            return Task.CompletedTask;
        }
        
        private void Init(CancellationToken cancellationToken)
        {
            _cancellationToken = cancellationToken; 
            _cancellationToken.Register(() => _logger.LogInformation("{ServiceName} cancellation token triggered", nameof(ServiceB)));
            
            _hostApplicationLifetime.ApplicationStarted.Register(HostStarted);
            _hostApplicationLifetime.ApplicationStopping.Register(HostStopping);
            _hostApplicationLifetime.ApplicationStopped.Register(HostStopped);

            _cancellationTokenSource = new CancellationTokenSource();
        }

        private void HostStarted()
        {
            Task.Run(DoWork);
        }

        private void HostStopping()
        {
            _cancellationTokenSource.Cancel();
        }
        
        private void HostStopped()
        {
            
        }        
        
        private Task DoWork()
        {
            _logger.LogInformation("{ServiceName} finished work", nameof(ServiceB));
            _cancellationTokenSource.Cancel();
            return Task.CompletedTask;
        }
    }
}