using System.Threading.Tasks;
using DevelopmentInProgress.MarketView.Interface.Strategy;

namespace DevelopmentInProgress.TradeServer.StrategyRunner.WebHost
{
    public interface IStrategyRunner
    {
        Task<Strategy> RunAsync(Strategy strategy, string localPath);
    }
}
