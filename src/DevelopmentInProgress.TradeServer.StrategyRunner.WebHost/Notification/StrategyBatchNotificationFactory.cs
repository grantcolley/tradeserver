using DevelopmentInProgress.TradeServer.StrategyRunner.WebHost.Notification.Logging;
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
                    return new StrategyRunnerLogger(loggerFactory);

                case BatchNotificationType.StrategyAccountInfoPublisher:
                    return new StrategyAccountInfoPublisher(notificationPublisher);

                case BatchNotificationType.StrategyCustomNotificationPublisher:
                    return new StrategyCustomNotificationPublisher(notificationPublisher);

                case BatchNotificationType.StrategyNotificationPublisher:
                    return new StrategyNotificationPublisher(notificationPublisher);

                case BatchNotificationType.StrategyOrderBookPublisher:
                    return new StrategyOrderBookPublisher(notificationPublisher);

                case BatchNotificationType.StrategyTradePublisher:
                    return new StrategyTradePublisher(notificationPublisher);

                case BatchNotificationType.StrategyCandlesticksPublisher:
                    return new StrategyCandlesticksPublisher(notificationPublisher);

                case BatchNotificationType.StrategyStatisticsPublisher:
                    return new StrategyStatisticsPublisher(notificationPublisher);
            }

            return null;
        }
    }
}
