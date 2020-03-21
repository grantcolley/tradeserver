using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DevelopmentInProgress.TradeServer.StrategyRunner.WebHost.Notification.Server;
using DevelopmentInProgress.TradeView.Interface.Strategy;

namespace DevelopmentInProgress.TradeServer.StrategyRunner.WebHost.Cache.TradeStrategy
{
    public class TradeStrategyCacheManager : ServerNotificationBase, ITradeStrategyCacheManager
    {
        private ConcurrentDictionary<string, ITradeStrategy> tradeStrategies;

        public TradeStrategyCacheManager()
        {
            tradeStrategies = new ConcurrentDictionary<string, ITradeStrategy>();
        }

        public List<Strategy> GetStrategies()
        {
            return tradeStrategies.Values.Select(s => s.Strategy).ToList();
        }

        public bool TryGetTradeStrategy(string strategyName, out ITradeStrategy tradeStrategy)
        {
            return tradeStrategies.TryGetValue(strategyName, out tradeStrategy);
        }

        public bool TryAddTradeStrategy(string strategyName, ITradeStrategy tradeStrategy)
        {
            if(tradeStrategies.TryAdd(strategyName, tradeStrategy))
            {
                OnServerNotification();
                return true;
            }

            return false;
        }

        public bool TryRemoveTradeStrategy(string strategyName, out ITradeStrategy tradeStrategy)
        {
            if (tradeStrategies.TryRemove(strategyName, out tradeStrategy))
            {
                OnServerNotification();
                return true;
            }

            return false;
        }

        public async Task StopStrategy(string strategyName, string parameters)
        {
            if (tradeStrategies.TryGetValue(strategyName, out ITradeStrategy tradeStrategy))
            {
                await tradeStrategy.TryStopStrategy(parameters);

                OnServerNotification();
            }
        }

        public async Task UpdateStrategy(string strategyName, string parameters)
        {
            if (tradeStrategies.TryGetValue(strategyName, out ITradeStrategy tradeStrategy))
            {
                await tradeStrategy.TryUpdateStrategyAsync(parameters);

                OnServerNotification();
            }
        }
    }
}
