using System.Threading;
using System.Threading.Tasks;
using DevelopmentInProgress.TradeView.Interface.Strategy;

namespace DevelopmentInProgress.TradeServer.StrategyRunner.WebHost
{
    public interface IStrategyRunner
    {
        Task<Strategy> RunAsync(Strategy strategy, string localPath, CancellationToken cancellationToken);
    }
}
