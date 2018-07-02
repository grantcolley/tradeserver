using DevelopmentInProgress.MarketView.Interface.TradeStrategy;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DevelopmentInProgress.TradeServer.StrategyEngine.Notification.Publishing
{
    public interface INotificationPublisher
    {
        Task PublishAsync(IEnumerable<StrategyNotification> notifications);
    }
}
