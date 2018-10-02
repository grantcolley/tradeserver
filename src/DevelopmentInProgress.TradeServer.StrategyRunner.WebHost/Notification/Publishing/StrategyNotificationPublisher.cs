using DevelopmentInProgress.MarketView.Interface.Strategy;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace DevelopmentInProgress.TradeServer.StrategyRunner.WebHost.Notification.Publishing
{
    public class StrategyNotificationPublisher : BatchNotification<StrategyNotification>, IBatchNotification<StrategyNotification>
    {
        private readonly INotificationPublisher notificationPublisher;

        public StrategyNotificationPublisher(INotificationPublisher notificationPublisher)
        {
            this.notificationPublisher = notificationPublisher;

            Start();
        }

        public override async Task NotifyAsync(IEnumerable<StrategyNotification> notifications, CancellationToken cancellationToken)
        {
            await notificationPublisher.PublishNotificationsAsync(notifications);
        }
    }
}
