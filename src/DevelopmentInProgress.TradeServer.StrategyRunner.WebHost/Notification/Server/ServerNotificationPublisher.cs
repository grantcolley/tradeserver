using DevelopmentInProgress.TradeView.Core.Server;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DevelopmentInProgress.TradeServer.StrategyRunner.WebHost.Notification.Server
{
    public class ServerNotificationPublisher : IServerNotificationPublisher
    {
        private readonly IServerNotificationPublisherContext notificationPublisherContext;

        public ServerNotificationPublisher(IServerNotificationPublisherContext notificationPublisherContext)
        {
            this.notificationPublisherContext = notificationPublisherContext;
        }

        public async Task PublishNotificationsAsync(IEnumerable<ServerNotification> notifications)
        {
            await notificationPublisherContext.PublishNotificationsAsync(notifications);
        }
    }
}
