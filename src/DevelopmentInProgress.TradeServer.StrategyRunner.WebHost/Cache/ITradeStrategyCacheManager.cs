using DevelopmentInProgress.TradeView.Interface.Strategy;

namespace DevelopmentInProgress.TradeServer.StrategyRunner.WebHost.Cache
{
    public interface ITradeStrategyCacheManager
    {
        bool TryAddTradeStrategy(string strategyName, ITradeStrategy tradeStrategy);
        bool TryRemoveTradeStrategy(string strategyName, out ITradeStrategy tradeStrategy);
        bool TryGetTradeStrategy(string strategyName, out ITradeStrategy tradeStrategy);
    }
}