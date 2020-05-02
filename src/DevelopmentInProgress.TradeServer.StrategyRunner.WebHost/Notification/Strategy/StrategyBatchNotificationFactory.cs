using DevelopmentInProgress.TradeView.Interface.Strategy;
using Microsoft.Extensions.Logging;

namespace DevelopmentInProgress.TradeServer.StrategyRunner.WebHost.Notification.Strategy
{
    public class StrategyBatchNotificationFactory : BatchNotificationFactory<StrategyNotification>
    {
        private readonly IStrategyNotificationPublisher notificationPublisher;
        private readonly ILoggerFactory loggerFactory;

        public StrategyBatchNotificationFactory(IStrategyNotificationPublisher notificationPublisher, ILoggerFactory loggerFactory)
        {
            this.notificationPublisher = notificationPublisher;
            this.loggerFactory = loggerFactory;
        }

        public override IBatchNotification<StrategyNotification> GetBatchNotifier(BatchNotificationType batchNotifierType)
        {
            switch (batchNotifierType)
            {
                case BatchNotificationType.StrategyLogger:
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

                case BatchNotificationType.StrategyParameterUpdatePublisher:
                    return new StrategyBatchParameterUpdatePublisher(notificationPublisher);
            }

            return null;
        }
    }
}
