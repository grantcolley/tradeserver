using DevelopmentInProgress.TradeServer.StrategyEngine.Notification.Logging;
using DevelopmentInProgress.TradeServer.StrategyEngine.Notification.Publishing;
using DevelopmentInProgress.MarketView.Interface.Strategy;
using Microsoft.Extensions.Logging;

namespace DevelopmentInProgress.TradeServer.StrategyEngine.Notification
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
                case BatchNotificationType.StrategyEngineLogger:
                    return new StrategyEngineLogger(loggerFactory);

                case BatchNotificationType.StrategyAccountInfoPublisher:
                    return new StrategyAccountInfoPublisher(notificationPublisher);

                case BatchNotificationType.StrategyNotificationPublisher:
                    return new StrategyNotificationPublisher(notificationPublisher);

                case BatchNotificationType.StrategyOrderBookPublisher:
                    return new StrategyOrderBookPublisher(notificationPublisher);

                case BatchNotificationType.StrategyTradePublisher:
                    return new StrategyTradePublisher(notificationPublisher);
            }

            return null;
        }
    }
}
