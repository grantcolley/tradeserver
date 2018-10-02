using DevelopmentInProgress.MarketView.Interface.Strategy;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DevelopmentInProgress.TradeServer.StrategyRunner.WebHost.Notification.Publishing
{
    public class NotificationPublisher : INotificationPublisher
    {
        private readonly INotificationPublisherContext notificationPublisherContext;

        public NotificationPublisher(INotificationPublisherContext notificationPublisherContext)
        {
            this.notificationPublisherContext = notificationPublisherContext;
        }

        public async Task PublishNotificationsAsync(IEnumerable<StrategyNotification> notifications)
        {
            var notifyGroups = notifications.GroupBy(n => n.Name);
            foreach (var group in notifyGroups)
            {
                await notificationPublisherContext.PublishNotificationsAsync(group.Key, group);
            }
        }

        public async Task PublishTradesAsync(IEnumerable<StrategyNotification> notifications)
        {
            var notifyGroups = notifications.GroupBy(n => n.Name);
            foreach (var group in notifyGroups)
            {
                await notificationPublisherContext.PublishTradesAsync(group.Key, group);
            }
        }

        public async Task PublishOrderBookAsync(IEnumerable<StrategyNotification> notifications)
        {
            var notifyGroups = notifications.GroupBy(n => n.Name);
            foreach (var group in notifyGroups)
            {
                await notificationPublisherContext.PublishOrderBookAsync(group.Key, group);
            }
        }

        public async Task PublishAccountInfoAsync(IEnumerable<StrategyNotification> notifications)
        {
            var notifyGroups = notifications.GroupBy(n => n.Name);
            foreach (var group in notifyGroups)
            {
                await notificationPublisherContext.PublishAccountInfoAsync(group.Key, group);
            }
        }
    }
}
