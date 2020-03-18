using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DevelopmentInProgress.TradeServer.StrategyRunner.WebHost.Notification;
using DevelopmentInProgress.TradeServer.StrategyRunner.WebHost.Notification.Strategy;
using DevelopmentInProgress.TradeView.Interface.Server;
using DevelopmentInProgress.TradeView.Interface.Strategy;

namespace DevelopmentInProgress.TradeServer.StrategyRunner.WebHost.Cache.TradeStrategy
{
    public class TradeStrategyCacheManager : ITradeStrategyCacheManager
    {
        private IServerMonitor serverMonitor;
        private IBatchNotification<ServerNotification> serverBatchNotificationPublisher;
        private StrategyNotificationHub strategyNotificationHub;
        private ConcurrentDictionary<string, ITradeStrategy> tradeStrategies;
        private SemaphoreSlim notificationSemaphoreSlim = new SemaphoreSlim(1, 1);

        public TradeStrategyCacheManager(
            IServerMonitor serverMonitor, 
            IBatchNotification<ServerNotification> serverBatchNotificationPublisher,
            StrategyNotificationHub strategyNotificationHub)
        {
            this.serverMonitor = serverMonitor;
            this.strategyNotificationHub = strategyNotificationHub;
            this.serverBatchNotificationPublisher = serverBatchNotificationPublisher;
            tradeStrategies = new ConcurrentDictionary<string, ITradeStrategy>();
        }

        public bool TryGetTradeStrategy(string strategyName, out ITradeStrategy tradeStrategy)
        {
            return tradeStrategies.TryGetValue(strategyName, out tradeStrategy);
        }

        public bool TryAddTradeStrategy(string strategyName, ITradeStrategy tradeStrategy)
        {
            if(tradeStrategies.TryAdd(strategyName, tradeStrategy))
            {
                ServerNotification();
                return true;
            }

            return false;
        }

        public bool TryRemoveTradeStrategy(string strategyName, out ITradeStrategy tradeStrategy)
        {
            if (tradeStrategies.TryRemove(strategyName, out tradeStrategy))
            {
                ServerNotification();
                return true;
            }

            return false;
        }

        public async Task StopStrategy(string strategyName, string parameters)
        {
            if (tradeStrategies.TryGetValue(strategyName, out ITradeStrategy tradeStrategy))
            {
                await tradeStrategy.TryStopStrategy(parameters);

                ServerNotification();
            }
        }

        public async Task UpdateStrategy(string strategyName, string parameters)
        {
            if (tradeStrategies.TryGetValue(strategyName, out ITradeStrategy tradeStrategy))
            {
                await tradeStrategy.TryUpdateStrategyAsync(parameters);

                ServerNotification();
            }
        }

        public void ServerNotification()
        {
            notificationSemaphoreSlim.Wait();

            try
            {
                List<ServerStrategy> strategyServers = new List<ServerStrategy>();

                var strategies = tradeStrategies.Values.Select(s => s.Strategy).ToList();
                var serverInfo = strategyNotificationHub.GetServerInfo();

                var serverStrategies = (from s in strategies
                                                 join c in serverInfo.Channels on s.Name equals c.Name
                                                 select new ServerStrategy
                                                 {
                                                     Strategy = s,
                                                     Connections = new List<ServerStrategyConnection>(
                                                         c.Connections.Select(conn => new ServerStrategyConnection 
                                                         {
                                                             Connection = conn.Name 
                                                         }))
                                                 }).ToList();

                var serverNotification = serverMonitor.GetServerNotification(serverStrategies);

                serverBatchNotificationPublisher.AddNotification(serverNotification);
            }
            finally
            {
                notificationSemaphoreSlim.Release();
            }
        }
    }
}
