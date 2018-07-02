using DevelopmentInProgress.MarketView.Interface.TradeStrategy;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace DevelopmentInProgress.TradeServer.StrategyEngine.Notification
{
    public class StrategyNotifier : BatchNotification<StrategyNotification>, IBatchNotification<StrategyNotification>
    {
        IBatchNotification<IEnumerable<StrategyNotification>> logger;
        IBatchNotification<IEnumerable<StrategyNotification>> publisher;

        public StrategyNotifier(IBatchNotificationFactory<IEnumerable<StrategyNotification>> strategyBatchNotificationListFactory)
        {
            logger = strategyBatchNotificationListFactory.GetBatchNotifier(BatchNotificationType.StrategyLogger);
            publisher = strategyBatchNotificationListFactory.GetBatchNotifier(BatchNotificationType.StrategyPublisher);

            Start();
        }

        public override async Task NotifyAsync(IEnumerable<StrategyNotification> notifications, CancellationToken cancellationToken)
        {
            // TODO: Consider Serializing the publisher list here...
            publisher.AddNotification(new List<StrategyNotification>(notifications));

            logger.AddNotification(new List<StrategyNotification>(notifications));
        }
    }
}
