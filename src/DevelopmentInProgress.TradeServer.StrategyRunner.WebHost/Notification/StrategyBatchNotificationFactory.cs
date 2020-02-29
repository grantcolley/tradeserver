using DevelopmentInProgress.TradeServer.StrategyRunner.WebHost.Notification.Publishing;
using DevelopmentInProgress.TradeView.Interface.Strategy;
using Microsoft.Extensions.Logging;

namespace DevelopmentInProgress.TradeServer.StrategyRunner.WebHost.Notification
{
    public class StrategyBatchNotificationFactory : BatchNotificationFactory<StrategyNotification>
    {
        private readonly INotificationPublisher notificationPublisher;
        private readonly ILoggerFactory loggerFactory;

        public StrategyBatchNotificationFactory(INotificationPublisher notificationPublisher, ILoggerFactory loggerFactory)
        {
            this.notificationPublisher = notificationPublisher;
            this.loggerFactory = loggerFactory;
        }

        public override IBatchNotification<StrategyNotification> GetBatchNotifier(BatchNotificationType batchNotifierType)
        {
            switch (batchNotifierType)
            {
                case BatchNotificationType.StrategyRunnerLogger:
                    return new StrategyBatchLogger(loggerFactory);

                case BatchNotificationType.StrategyAccountInfoPublisher:
                    return new StrategyBatchAccountInfoPublisher(notificationPublisher);

                case BatchNotificationType.StrategyCustomNotificationPublisher:
                    return new StrategyBatchCustomNotificationPublisher(notificationPublisher);

                case BatchNotificationType.StrategyNotificationPublisher:
                    return new StrategyBatchNotificationPublisher(notificationPublisher);

                case BatchNotificationType.StrategyOrderBookPublisher:
                    return new StrategyBatchOrderBookPublisher(notificationPublisher);

                case BatchNotificationType.StrategyTradePublisher:
                    return new StrategyBatchTradePublisher(notificationPublisher);

                case BatchNotificationType.StrategyCandlesticksPublisher:
                    return new StrategyBatchCandlesticksPublisher(notificationPublisher);

                case BatchNotificationType.StrategyStatisticsPublisher:
                    return new StrategyBatchStatisticsPublisher(notificationPublisher);
            }

            return null;
        }
    }
}
