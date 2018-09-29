using System.Threading.Tasks;
using DevelopmentInProgress.MarketView.Interface.Strategy;

namespace DevelopmentInProgress.TradeServer.StrategyEngine
{
    public interface IStrategyRunner
    {
        Task<Strategy> RunAsync(Strategy strategy, string localPath);
    }
}
