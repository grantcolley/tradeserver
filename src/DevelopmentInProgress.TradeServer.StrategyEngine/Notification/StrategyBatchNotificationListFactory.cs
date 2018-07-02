using DevelopmentInProgress.TradeServer.StrategyEngine.Notification.Logging;
using DevelopmentInProgress.TradeServer.StrategyEngine.Notification.Publishing;
using DevelopmentInProgress.MarketView.Interface.TradeStrategy;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;

namespace DevelopmentInProgress.TradeServer.StrategyEngine.Notification
{
    public class StrategyBatchNotificationListFactory : BatchNotificationFactory<IEnumerable<StrategyNotification>>
    {
        private readonly INotificationPublisher notificationPublisher;
        private readonly ILoggerFactory loggerFactory;

        public StrategyBatchNotificationListFactory(INotificationPublisher notificationPublisher, ILoggerFactory loggerFactory)
        {
            this.notificationPublisher = notificationPublisher;
            this.loggerFactory = loggerFactory;
        }

        public override IBatchNotification<IEnumerable<StrategyNotification>> GetBatchNotifier(BatchNotificationType batchNotifierType)
        {
            switch (batchNotifierType)
            {
                case BatchNotificationType.StrategyLogger:
                    return new StrategyLogger(loggerFactory);

                case BatchNotificationType.StrategyPublisher:
                    return new StrategyPublisher(notificationPublisher);
            }

            return null;
        }
    }
}
