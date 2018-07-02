using DevelopmentInProgress.MarketView.Interface.TradeStrategy;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace DevelopmentInProgress.TradeServer.StrategyEngine.Notification.Publishing
{
    public class StrategyPublisher : BatchNotification<IEnumerable<StrategyNotification>>, IBatchNotification<IEnumerable<StrategyNotification>>
    {
        private readonly INotificationPublisher notificationPublisher;

        public StrategyPublisher(INotificationPublisher notificationPublisher)
        {
            this.notificationPublisher = notificationPublisher;

            Start();
        }

        public override async Task NotifyAsync(IEnumerable<IEnumerable<StrategyNotification>> notifications, CancellationToken cancellationToken)
        {
            foreach (var strategyNotifications in notifications)
            {
                await notificationPublisher.PublishAsync(strategyNotifications);
            }
        }
    }
}
