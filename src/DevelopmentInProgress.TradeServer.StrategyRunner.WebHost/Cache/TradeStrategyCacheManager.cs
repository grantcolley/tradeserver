using System.Collections.Concurrent;
using DevelopmentInProgress.TradeView.Interface.Strategy;

namespace DevelopmentInProgress.TradeServer.StrategyRunner.WebHost.Cache
{
    public class TradeStrategyCacheManager : ITradeStrategyCacheManager
    {
        private ConcurrentDictionary<string, ITradeStrategy> tradeStrategies;

        public TradeStrategyCacheManager()
        {
            tradeStrategies = new ConcurrentDictionary<string, ITradeStrategy>();
        }

        public bool TryGetTradeStrategy(string strategyName, out ITradeStrategy tradeStrategy)
        {
            return tradeStrategies.TryGetValue(strategyName, out tradeStrategy);
        }

        public bool TryAddTradeStrategy(string strategyName, ITradeStrategy tradeStrategy)
        {
            return tradeStrategies.TryAdd(strategyName, tradeStrategy);
        }

        public bool TryRemoveTradeStrategy(string strategyName, out ITradeStrategy tradeStrategy)
        {
            return tradeStrategies.TryRemove(strategyName, out tradeStrategy);
        }
    }
}
