using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using DevelopmentInProgress.TradeServer.StrategyRunner.WebHost.Notification;
using DevelopmentInProgress.TradeServer.StrategyRunner.WebHost.Utilities;
using DevelopmentInProgress.MarketView.Interface.Strategy;
using DevelopmentInProgress.TradeServer.StrategyRunner.WebHost.Cache;
using System.Threading;

namespace DevelopmentInProgress.TradeServer.StrategyRunner.WebHost
{
    public class StrategyRunner : IStrategyRunner
    {
        private IBatchNotification<StrategyNotification> strategyRunnerLogger;
        private IBatchNotification<StrategyNotification> strategyAccountInfoPublisher;
        private IBatchNotification<StrategyNotification> strategyNotificationPublisher;
        private IBatchNotification<StrategyNotification> strategyOrderBookPublisher;
        private IBatchNotification<StrategyNotification> strategyTradePublisher;
        private ISubscriptionsCacheManager symbolsCacheManager;

        private CancellationToken cancellationToken;

        public StrategyRunner(IBatchNotificationFactory<StrategyNotification> batchNotificationFactory, ISubscriptionsCacheManager symbolsCacheManager)
        {
            this.symbolsCacheManager = symbolsCacheManager;

            strategyRunnerLogger = batchNotificationFactory.GetBatchNotifier(BatchNotificationType.StrategyRunnerLogger);
            strategyAccountInfoPublisher = batchNotificationFactory.GetBatchNotifier(BatchNotificationType.StrategyAccountInfoPublisher);
            strategyNotificationPublisher = batchNotificationFactory.GetBatchNotifier(BatchNotificationType.StrategyNotificationPublisher);
            strategyOrderBookPublisher = batchNotificationFactory.GetBatchNotifier(BatchNotificationType.StrategyOrderBookPublisher);
            strategyTradePublisher = batchNotificationFactory.GetBatchNotifier(BatchNotificationType.StrategyTradePublisher);
        }

        public async Task<Strategy> RunAsync(Strategy strategy, string localPath, CancellationToken cancellationToken)
        {
            this.cancellationToken = cancellationToken;

            try
            {
                strategy.Status = StrategyStatus.Initialising;

                Notify(NotificationLevel.Information, NotificationEventId.RunAsync, strategy, "Initialising strategy");

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
            if (string.IsNullOrWhiteSpace(strategy.TargetAssembly))
            {
                Notify(NotificationLevel.Error, NotificationEventId.RunStrategyAsync, strategy, "No TargetAssembly");
                return strategy;
            }

            if (string.IsNullOrWhiteSpace(strategy.TargetType))
            {
                Notify(NotificationLevel.Error, NotificationEventId.RunStrategyAsync, strategy, "No TargetType");
                return strategy;
            }

            Notify(NotificationLevel.Information, NotificationEventId.RunStrategyAsync, strategy, $"Loading {strategy.TargetType}");

            var dependencies = GetAssemblies(localPath);

            var assemblyLoader = new AssemblyLoader(localPath, dependencies);
            var assembly = assemblyLoader.LoadFromMemoryStream(Path.Combine(localPath, strategy.TargetAssembly));
            var type = assembly.GetType(strategy.TargetType);
            dynamic obj = Activator.CreateInstance(type);

            ((ITradeStrategy)obj).StrategyAccountInfoEvent += StrategyAccountInfoEvent;
            ((ITradeStrategy)obj).StrategyNotificationEvent += StrategyNotificationEvent;
            ((ITradeStrategy)obj).StrategyOrderBookEvent += StrategyOrderBookEvent;
            ((ITradeStrategy)obj).StrategyTradeEvent += StrategyTradeEvent;

            try
            {
                strategy.Status = StrategyStatus.Running;

                Notify(NotificationLevel.Information, NotificationEventId.RunStrategyAsync, strategy, $"Running {strategy.TargetType}");

                await symbolsCacheManager.Subscribe(strategy, obj);

                var result = await obj.RunAsync(strategy, cancellationToken);
            }
            catch(Exception ex)
            {
                throw;
            }
            finally
            {
                symbolsCacheManager.Unsubscribe(strategy, obj);
                ((ITradeStrategy)obj).StrategyAccountInfoEvent -= StrategyAccountInfoEvent;
                ((ITradeStrategy)obj).StrategyNotificationEvent -= StrategyNotificationEvent;
                ((ITradeStrategy)obj).StrategyOrderBookEvent -= StrategyOrderBookEvent;
                ((ITradeStrategy)obj).StrategyTradeEvent -= StrategyTradeEvent;
            }

            return strategy;
        }

        private void StrategyAccountInfoEvent(object sender, TradeStrategyNotificationEventArgs e)
        {
            strategyAccountInfoPublisher.AddNotification(e.StrategyNotification);
        }

        private void StrategyNotificationEvent(object sender, TradeStrategyNotificationEventArgs e)
        {
            strategyNotificationPublisher.AddNotification(e.StrategyNotification);
        }

        private void StrategyOrderBookEvent(object sender, TradeStrategyNotificationEventArgs e)
        {
            strategyOrderBookPublisher.AddNotification(e.StrategyNotification);
        }

        private void StrategyTradeEvent(object sender, TradeStrategyNotificationEventArgs e)
        {
            strategyTradePublisher.AddNotification(e.StrategyNotification);
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
