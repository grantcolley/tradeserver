using DevelopmentInProgress.MarketView.Interface.TradeStrategy;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DevelopmentInProgress.TradeServer.StrategyEngine.Notification.Publishing
{
    public class NotificationPublisher : INotificationPublisher
    {
        private readonly INotificationPublisherContext notificationHub;

        public NotificationPublisher(INotificationPublisherContext notificationHub)
        {
            this.notificationHub = notificationHub;
        }

        public async Task PublishAsync(IEnumerable<StrategyNotification> notifications)
        {
            var notifyGroups = notifications.GroupBy(n => n.Name);
            foreach (var group in notifyGroups)
            {
                await notificationHub.NotifyAsync(group.Key, group);
            }
        }
    }
}
