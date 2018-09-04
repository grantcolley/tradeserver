using DevelopmentInProgress.MarketView.Interface.TradeStrategy;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DevelopmentInProgress.TradeServer.StrategyEngine.Notification.Publishing
{
    public interface INotificationPublisher
    {
        Task PublishNotificationsAsync(IEnumerable<StrategyNotification> notifications);
        Task PublishTradesAsync(IEnumerable<StrategyNotification> notifications);
        Task PublishOrderBookAsync(IEnumerable<StrategyNotification> notifications);
        Task PublishAccountInfoAsync(IEnumerable<StrategyNotification> notifications);
    }
}
