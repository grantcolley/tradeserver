using DevelopmentInProgress.TradeView.Core.Strategy;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DevelopmentInProgress.TradeServer.StrategyRunner.WebHost.Notification.Strategy
{
    public class StrategyNotificationPublisher : IStrategyNotificationPublisher
    {
        private readonly IStrategyNotificationPublisherContext notificationPublisherContext;

        public StrategyNotificationPublisher(IStrategyNotificationPublisherContext notificationPublisherContext)
        {
            this.notificationPublisherContext = notificationPublisherContext;
        }

        public async Task PublishCustomNotificationsAsync(string methodName, IEnumerable<StrategyNotification> notifications)
        {
            var notifyGroups = notifications.GroupBy(n => n.Name);
            foreach (var group in notifyGroups)
            {
                await notificationPublisherContext.PublishCustomNotificationsAsync(group.Key, methodName, group);
            }
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

        public async Task PublishCandlesticksAsync(IEnumerable<StrategyNotification> notifications)
        {
            var notifyGroups = notifications.GroupBy(n => n.Name);
            foreach (var group in notifyGroups)
            {
                await notificationPublisherContext.PublishCandlesticksAsync(group.Key, group);
            }
        }

        public async Task PublishStatisticsAsync(IEnumerable<StrategyNotification> notifications)
        {
            var notifyGroups = notifications.GroupBy(n => n.Name);
            foreach (var group in notifyGroups)
            {
                await notificationPublisherContext.PublishStatisticsAsync(group.Key, group);
            }
        }

        public async Task PublishParameterUpdateAsync(IEnumerable<StrategyNotification> notifications)
        {
            var notifyGroups = notifications.GroupBy(n => n.Name);
            foreach (var group in notifyGroups)
            {
                await notificationPublisherContext.PublishParameterUpdateAsync(group.Key, group);
            }
        }
    }
}
