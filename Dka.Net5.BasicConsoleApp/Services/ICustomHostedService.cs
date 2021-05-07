using System.Threading;

namespace Dka.Net5.BasicConsoleApp.Services
{
    public interface ICustomHostedService
    {
        CancellationToken CancellationToken { get; }
    }
}