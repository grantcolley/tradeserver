using DevelopmentInProgress.TradeServer.StrategyExecution.WebHost.Cache.TradeStrategy;
using DevelopmentInProgress.TradeServer.StrategyExecution.WebHost.Notification.Strategy;
using DevelopmentInProgress.TradeView.Core.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Threading;

namespace DevelopmentInProgress.TradeServer.StrategyExecution.WebHost.Notification.Server
{
    public class ServerManager : IServerManager, IDisposable
    {
        private IServerMonitor serverMonitor;
        private IBatchNotification<ServerNotification> serverBatchNotificationPublisher;
        private ITradeStrategyCacheManager tradeStrategyCacheManager;
        private StrategyNotificationHub strategyNotificationHub;
        private ServerNotificationHub serverNotificationHub;
        private SemaphoreSlim notificationSemaphoreSlim = new SemaphoreSlim(1, 1);
        private List<IDisposable> disposables;
        private bool disposed;

        public ServerManager(IServerMonitor serverMonitor,
            IBatchNotification<ServerNotification> serverBatchNotificationPublisher,
            ITradeStrategyCacheManager tradeStrategyCacheManager,
            StrategyNotificationHub strategyNotificationHub,
            ServerNotificationHub serverNotificationHub)
        {
            this.serverMonitor = serverMonitor;
            this.serverBatchNotificationPublisher = serverBatchNotificationPublisher;
            this.tradeStrategyCacheManager = tradeStrategyCacheManager;
            this.strategyNotificationHub = strategyNotificationHub;
            this.serverNotificationHub = serverNotificationHub;

            disposables = new List<IDisposable>();

            ObserverTradeStrategyCacheManager();
            ObserverStrategyNotificationHub();
            ObserverServerNotificationHub();
        }

        public IServerMonitor ServerMonitor { get { return serverMonitor; } }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
            {
                return;
            }

            if (disposing)
            {
                foreach (var disposable in disposables)
                {
                    disposable.Dispose();
                }

                notificationSemaphoreSlim.Dispose();
            }

            disposed = true;
        }

        private void ObserverTradeStrategyCacheManager()
        {
            var tradeStrategyCacheManagerObservable = Observable.FromEventPattern<ServerNotificationEventArgs>(
                eventHandler => tradeStrategyCacheManager.ServerNotification += eventHandler,
                eventHandler => tradeStrategyCacheManager.ServerNotification -= eventHandler)
                .Select(eventPattern => eventPattern.EventArgs);

            var tradeStrategyCacheManagerSubscription = tradeStrategyCacheManagerObservable.Subscribe(args =>
            {
                OnNotification();
            });

            disposables.Add(tradeStrategyCacheManagerSubscription);
        }

        private void ObserverStrategyNotificationHub()
        {
            var strategyNotificationHubObservable = Observable.FromEventPattern<ServerNotificationEventArgs>(
                eventHandler => strategyNotificationHub.ServerNotification += eventHandler,
                eventHandler => strategyNotificationHub.ServerNotification -= eventHandler)
                .Select(eventPattern => eventPattern.EventArgs);

            var strategyNotificationHubObservableSubscription = strategyNotificationHubObservable.Subscribe(args =>
            {
                OnNotification();
            });

            disposables.Add(strategyNotificationHubObservableSubscription);
        }

        private void ObserverServerNotificationHub()
        {
            var serverNotificationHubObservable = Observable.FromEventPattern<ServerNotificationEventArgs>(
                eventHandler => serverNotificationHub.ServerNotification += eventHandler,
                eventHandler => serverNotificationHub.ServerNotification -= eventHandler)
                .Select(eventPattern => eventPattern.EventArgs);

            var serverNotificationHubObservableSubscription = serverNotificationHubObservable.Subscribe(args =>
            {
                OnNotification();
            });

            disposables.Add(serverNotificationHubObservableSubscription);
        }

        private void OnNotification()
        {
            notificationSemaphoreSlim.Wait();

            try
            {
                List<ServerStrategy> strategyServers = new List<ServerStrategy>();

                var strategies = tradeStrategyCacheManager.GetStrategies();
                var serverInfo = strategyNotificationHub.GetServerInfo();

                Func<TradeView.Core.TradeStrategy.Strategy, Socket.Messages.ChannelInfo, ServerStrategy> f = (s, c) =>
                {
                    var serverStrategy = new ServerStrategy
                    {
                        Strategy = s
                    };

                    serverStrategy.Connections.AddRange(c.Connections.Select(conn => new ServerStrategyConnection
                    {
                        Connection = conn.Name
                    }));

                    return serverStrategy;
                };

                var serverStrategies = (from s in strategies
                                        join c in serverInfo.Channels on s.Name equals c.Name
                                        select f(s, c)).ToList();

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
