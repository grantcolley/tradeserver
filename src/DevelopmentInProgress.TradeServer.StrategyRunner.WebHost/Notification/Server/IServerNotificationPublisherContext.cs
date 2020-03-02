﻿using DevelopmentInProgress.TradeView.Interface.Server;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DevelopmentInProgress.TradeServer.StrategyRunner.WebHost.Notification.Server
{
    public interface IServerNotificationPublisherContext
    {
        Task PublishNotificationsAsync(IEnumerable<ServerNotification> notifications);
    }
}
