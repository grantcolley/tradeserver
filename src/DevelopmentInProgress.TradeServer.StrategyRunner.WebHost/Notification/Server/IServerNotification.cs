using System;

namespace DevelopmentInProgress.TradeServer.StrategyRunner.WebHost.Notification.Server
{
    public interface IServerNotification
    {
        event EventHandler<ServerNotificationEventArgs> ServerNotification;
    }
}
