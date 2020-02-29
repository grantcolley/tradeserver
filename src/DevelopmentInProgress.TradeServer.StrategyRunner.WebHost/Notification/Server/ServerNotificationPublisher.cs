using DevelopmentInProgress.TradeView.Interface.Server;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DevelopmentInProgress.TradeServer.StrategyRunner.WebHost.Notification.Server
{
    public class ServerNotificationPublisher : IServerNotificationPublisher
    {
        public Task PublishNotificationsAsync(IEnumerable<ServerNotification> notifications)
        {
            throw new NotImplementedException();
        }
    }
}
