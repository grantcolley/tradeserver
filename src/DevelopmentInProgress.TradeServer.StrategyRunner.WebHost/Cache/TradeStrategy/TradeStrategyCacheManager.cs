using System.Collections.Concurrent;
using System.Threading.Tasks;
using DevelopmentInProgress.TradeServer.StrategyRunner.WebHost.Notification;
using DevelopmentInProgress.TradeView.Interface.Server;
using DevelopmentInProgress.TradeView.Interface.Strategy;

namespace DevelopmentInProgress.TradeServer.StrategyRunner.WebHost.Cache.TradeStrategy
{
    public class TradeStrategyCacheManager : ITradeStrategyCacheManager
    {
        private IBatchNotification<ServerNotification> serverBatchNotificationPublisher;
        private ConcurrentDictionary<string, ITradeStrategy> tradeStrategies;

        public TradeStrategyCacheManager(IBatchNotification<ServerNotification> serverBatchNotificationPublisher)
        {
            this.serverBatchNotificationPublisher = serverBatchNotificationPublisher;
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

        public async Task StopStrategy(string strategyName, string parameters)
        {
            if (tradeStrategies.TryGetValue(strategyName, out ITradeStrategy tradeStrategy))
            {
                await tradeStrategy.TryStopStrategy(parameters);
            }
        }

        public async Task UpdateStrategy(string strategyName, string parameters)
        {
            if (tradeStrategies.TryGetValue(strategyName, out ITradeStrategy tradeStrategy))
            {
                await tradeStrategy.TryUpdateStrategyAsync(parameters);
            }
        }
    }
}
