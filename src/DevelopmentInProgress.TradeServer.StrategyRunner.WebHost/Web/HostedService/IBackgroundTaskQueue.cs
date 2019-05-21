using System;
using System.Threading;
using System.Threading.Tasks;

namespace DevelopmentInProgress.TradeServer.StrategyRunner.WebHost.Web.HostedService
{
    public interface IBackgroundTaskQueue
    {
        void QueueBackgroundWorkItem(Func<CancellationToken, Task> workItem);
        Task<Func<CancellationToken, Task>> DequeueAsync(CancellationToken cancellationToken);
    }
}
