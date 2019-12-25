using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using DevelopmentInProgress.TradeServer.StrategyRunner.WebHost.Notification;
using DevelopmentInProgress.TradeServer.StrategyRunner.WebHost.Utilities;
using DevelopmentInProgress.TradeView.Interface.Strategy;
using DevelopmentInProgress.TradeServer.StrategyRunner.WebHost.Cache;
using System.Threading;

namespace DevelopmentInProgress.TradeServer.StrategyRunner.WebHost
{
    public class StrategyRunner : IStrategyRunner
    {
        private IBatchNotification<StrategyNotification> strategyRunnerLogger;
        private IBatchNotification<StrategyNotification> strategyAccountInfoPublisher;
        private IBatchNotification<StrategyNotification> strategyCustomNotificationPublisher;
        private IBatchNotification<StrategyNotification> strategyNotificationPublisher;
        private IBatchNotification<StrategyNotification> strategyOrderBookPublisher;
        private IBatchNotification<StrategyNotification> strategyTradePublisher;
        private IBatchNotification<StrategyNotification> strategyStatisticsPublisher;
        private IBatchNotification<StrategyNotification> strategyCandlesticksPublisher;
        private ISubscriptionsCacheManager subscriptionsCacheManager;
        private ITradeStrategyCacheManager tradeStrategyCacheManager;

        private CancellationToken cancellationToken;

        public StrategyRunner(
            IBatchNotificationFactory<StrategyNotification> batchNotificationFactory, 
            ISubscriptionsCacheManager subscriptionsCacheManager, 
            ITradeStrategyCacheManager tradeStrategyCacheManager)
        {
            this.subscriptionsCacheManager = subscriptionsCacheManager;
            this.tradeStrategyCacheManager = tradeStrategyCacheManager;

            strategyRunnerLogger = batchNotificationFactory.GetBatchNotifier(BatchNotificationType.StrategyRunnerLogger);
            strategyAccountInfoPublisher = batchNotificationFactory.GetBatchNotifier(BatchNotificationType.StrategyAccountInfoPublisher);
            strategyCustomNotificationPublisher = batchNotificationFactory.GetBatchNotifier(BatchNotificationType.StrategyCustomNotificationPublisher);
            strategyNotificationPublisher = batchNotificationFactory.GetBatchNotifier(BatchNotificationType.StrategyNotificationPublisher);
            strategyOrderBookPublisher = batchNotificationFactory.GetBatchNotifier(BatchNotificationType.StrategyOrderBookPublisher);
            strategyTradePublisher = batchNotificationFactory.GetBatchNotifier(BatchNotificationType.StrategyTradePublisher);
            strategyStatisticsPublisher = batchNotificationFactory.GetBatchNotifier(BatchNotificationType.StrategyStatisticsPublisher);
            strategyCandlesticksPublisher = batchNotificationFactory.GetBatchNotifier(BatchNotificationType.StrategyCandlesticksPublisher);
        }

        public async Task<Strategy> RunAsync(Strategy strategy, string localPath, CancellationToken cancellationToken)
        {
            this.cancellationToken = cancellationToken;

            try
            {
                strategy.Status = StrategyStatus.Initialising;

                Notify(NotificationLevel.Information, NotificationEventId.RunAsync, strategy, "Initialising strategy");

                if (string.IsNullOrWhiteSpace(strategy.TargetAssembly))
                {
                    Notify(NotificationLevel.Error, NotificationEventId.RunAsync, strategy, "No TargetAssembly");
                    return strategy;
                }

                if (string.IsNullOrWhiteSpace(strategy.TargetType))
                {
                    Notify(NotificationLevel.Error, NotificationEventId.RunAsync, strategy, "No TargetType");
                    return strategy;
                }

                return await RunStrategyAsync(strategy, localPath).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                Notify(NotificationLevel.Error, NotificationEventId.RunAsync, strategy, ex.ToString());
                throw;
            }
        }

        internal async Task<Strategy> RunStrategyAsync(Strategy strategy, string localPath)
        {
            ITradeStrategy tradeStrategy = null;

            try
            {
                Notify(NotificationLevel.Information, NotificationEventId.RunStrategyAsync, strategy, $"Loading {strategy.Name}");

                var dependencies = GetAssemblies(localPath);

                var assemblyLoader = new AssemblyLoader(localPath, dependencies);
                var assembly = assemblyLoader.LoadFromMemoryStream(Path.Combine(localPath, strategy.TargetAssembly));
                var type = assembly.GetType(strategy.TargetType);
                dynamic obj = Activator.CreateInstance(type);

                tradeStrategy = (ITradeStrategy)obj;

                tradeStrategy.StrategyNotificationEvent += StrategyNotificationEvent;
                tradeStrategy.StrategyAccountInfoEvent += StrategyAccountInfoEvent;
                tradeStrategy.StrategyOrderBookEvent += StrategyOrderBookEvent;
                tradeStrategy.StrategyTradeEvent += StrategyTradeEvent;
                tradeStrategy.StrategyStatisticsEvent += StrategyStatisticsEvent;
                tradeStrategy.StrategyCandlesticksEvent += StrategyCandlesticksEvent;
                tradeStrategy.StrategyCustomNotificationEvent += StrategyCustomNotificationEvent;

                strategy.Status = StrategyStatus.Running;

                if(tradeStrategyCacheManager.TryAddTradeStrategy(strategy.Name, tradeStrategy))
                {
                    Notify(NotificationLevel.Information, NotificationEventId.RunStrategyAsync, strategy, $"Subscribing {strategy.Name}");

                    await subscriptionsCacheManager.Subscribe(strategy, tradeStrategy).ConfigureAwait(false);

                    Notify(NotificationLevel.Information, NotificationEventId.RunStrategyAsync, strategy, $"Running {strategy.Name}");

                    var result = await tradeStrategy.RunAsync(strategy, cancellationToken).ConfigureAwait(false);

                    if(!tradeStrategyCacheManager.TryRemoveTradeStrategy(strategy.Name, out ITradeStrategy ts))
                    {
                        Notify(NotificationLevel.Error, NotificationEventId.RunStrategyAsync, strategy, $"Failed to remove {strategy.Name} from the cache manager.");
                    }
                }
                else
                {
                    Notify(NotificationLevel.Error, NotificationEventId.RunStrategyAsync, strategy, $"Failed to add {strategy.Name} to the cache manager.");
                }
            }
            finally
            {
                if(tradeStrategy != null)
                {
                    subscriptionsCacheManager.Unsubscribe(strategy, tradeStrategy);

                    tradeStrategy.StrategyNotificationEvent -= StrategyNotificationEvent;
                    tradeStrategy.StrategyAccountInfoEvent -= StrategyAccountInfoEvent;
                    tradeStrategy.StrategyOrderBookEvent -= StrategyOrderBookEvent;
                    tradeStrategy.StrategyTradeEvent -= StrategyTradeEvent;
                    tradeStrategy.StrategyCustomNotificationEvent -= StrategyCustomNotificationEvent;
                }

                // TODO: Unload target assembly and it's dependencies from memory and delete them.
            }

            return strategy;
        }

        private void StrategyNotificationEvent(object sender, StrategyNotificationEventArgs e)
        {
            strategyNotificationPublisher.AddNotification(e.StrategyNotification);
        }

        private void StrategyAccountInfoEvent(object sender, StrategyNotificationEventArgs e)
        {
            strategyAccountInfoPublisher.AddNotification(e.StrategyNotification);
        }

        private void StrategyOrderBookEvent(object sender, StrategyNotificationEventArgs e)
        {
            strategyOrderBookPublisher.AddNotification(e.StrategyNotification);
        }

        private void StrategyTradeEvent(object sender, StrategyNotificationEventArgs e)
        {
            strategyTradePublisher.AddNotification(e.StrategyNotification);
        }

        private void StrategyStatisticsEvent(object sender, StrategyNotificationEventArgs e)
        {
            strategyStatisticsPublisher.AddNotification(e.StrategyNotification);
        }

        private void StrategyCandlesticksEvent(object sender, StrategyNotificationEventArgs e)
        {
            strategyCandlesticksPublisher.AddNotification(e.StrategyNotification);
        }

        private void StrategyCustomNotificationEvent(object sender, StrategyNotificationEventArgs e)
        {
            strategyCustomNotificationPublisher.AddNotification(e.StrategyNotification);
        }

        private void Notify(NotificationLevel notificationLevel, int notificationEvent, Strategy strategy, string message = "")
        {
            var strategyNotification = strategy.GetNotification(notificationLevel, notificationEvent, message);
            strategyNotificationPublisher.AddNotification(strategyNotification);
            strategyRunnerLogger.AddNotification(strategyNotification);
        }

        private IList<string> GetAssemblies(string localPath)
        {
            var dependencies = new List<string>();
            var files = Directory.GetFiles(localPath); 

            foreach (string filePath in files)
            {
                var filePathSplit = filePath.Split('\\');
                var fileName = filePathSplit[filePathSplit.Length - 1];
                var name = fileName.Substring(0, fileName.LastIndexOf('.'));
                dependencies.Add(name);
            }

            return dependencies;
        }
    }
}
