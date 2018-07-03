using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using DevelopmentInProgress.TradeServer.StrategyEngine.ExchangeService;
using DevelopmentInProgress.TradeServer.StrategyEngine.Notification;
using DevelopmentInProgress.TradeServer.StrategyEngine.Utilities;
using DevelopmentInProgress.MarketView.Interface.Interfaces;
using DevelopmentInProgress.MarketView.Interface.TradeStrategy;
using DevelopmentInProgress.TradeServer.StrategyEngine.Cache;

namespace DevelopmentInProgress.TradeServer.StrategyEngine
{
    public class StrategyRunner : IStrategyRunner
    {
        private IBatchNotification<StrategyNotification> strategyNotifier;
        private IExchangeServiceFactory<IExchangeService> exchangeServiceFactory;
        private ISymbolsCacheManager symbolsCacheManager;

        public StrategyRunner(IBatchNotificationFactory<StrategyNotification> batchNotificationFactory, ISymbolsCacheManager symbolsCacheManager)
        {
            this.symbolsCacheManager = symbolsCacheManager;
            strategyNotifier = batchNotificationFactory.GetBatchNotifier(BatchNotificationType.StrategyNotifier);
        }

        public async Task<Strategy> RunAsync(Strategy strategy, string localPath)
        {
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

            ((ITradeStrategy)obj).TradeStrategyNotificationEvent += TradeStrategyNotificationEvent;

            try
            {
                strategy.Status = StrategyStatus.Running;

                Notify(NotificationLevel.Information, NotificationEventId.RunStrategyAsync, strategy, $"Running {strategy.TargetType}");

                symbolsCacheManager.Subscribe(strategy, obj);

                var result = await obj.RunAsync(strategy);

                symbolsCacheManager.Unsubscribe(strategy, obj);
            }
            catch(Exception)
            {
                throw;
            }
            finally
            {
                ((ITradeStrategy)obj).TradeStrategyNotificationEvent -= TradeStrategyNotificationEvent;
            }

            return strategy;
        }

        private void TradeStrategyNotificationEvent(object sender, TradeStrategyNotificationEventArgs e)
        {
            strategyNotifier.AddNotification(e.StrategyNotification);
        }

        private void Notify(NotificationLevel notificationLevel, int notificationEvent, Strategy strategy, string message = "")
        {
            var strategyNotification = strategy.GetNotification(notificationLevel, notificationEvent, message);
            strategyNotifier.AddNotification(strategyNotification);
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
