using DevelopmentInProgress.MarketView.Interface.TradeStrategy;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DevelopmentInProgress.TradeServer.StrategyEngine.Notification.Publishing
{
    public interface INotificationPublisherContext
    {
        Task PublishNotificationsAsync(string strategyName, IEnumerable<StrategyNotification> message);
        Task PublishTradesAsync(string strategyName, IEnumerable<StrategyNotification> message);
        Task PublishOrderBookAsync(string strategyName, IEnumerable<StrategyNotification> message);
        Task PublishAccountInfoAsync(string strategyName, IEnumerable<StrategyNotification> message);
    }
}
