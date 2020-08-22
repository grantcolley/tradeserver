using DevelopmentInProgress.TradeView.Core.Server;

namespace DevelopmentInProgress.TradeServer.StrategyRunner.WebHost.Notification.Server
{
    public interface IServerManager
    {
        IServerMonitor ServerMonitor { get; }
    }
}
