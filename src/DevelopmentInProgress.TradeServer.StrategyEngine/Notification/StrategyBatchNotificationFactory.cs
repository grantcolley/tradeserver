using DevelopmentInProgress.MarketView.Interface.TradeStrategy;
using System.Collections.Generic;

namespace DevelopmentInProgress.TradeServer.StrategyEngine.Notification
{
    public class StrategyBatchNotificationFactory : BatchNotificationFactory<StrategyNotification>
    {
        private IBatchNotificationFactory<IEnumerable<StrategyNotification>> strategyBatchNotificationListFactory;

        public StrategyBatchNotificationFactory(IBatchNotificationFactory<IEnumerable<StrategyNotification>> strategyBatchNotificationListFactory)
        {
            this.strategyBatchNotificationListFactory = strategyBatchNotificationListFactory;
        }

        public override IBatchNotification<StrategyNotification> GetBatchNotifier(BatchNotificationType batchNotifierType)
        {
            switch (batchNotifierType)
            {
                case BatchNotificationType.StrategyNotifier:
                    return new StrategyNotifier(strategyBatchNotificationListFactory);
            }

            return null;
        }
    }
}
