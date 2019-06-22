using DevelopmentInProgress.MarketView.Interface.Strategy;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DevelopmentInProgress.TradeServer.StrategyRunner.WebHost.Notification.Publishing
{
    public interface INotificationPublisher
    {
        Task PublishCustomNotificationsAsync(string methodName, IEnumerable<StrategyNotification> notifications);
        Task PublishNotificationsAsync(IEnumerable<StrategyNotification> notifications);
        Task PublishTradesAsync(IEnumerable<StrategyNotification> notifications);
        Task PublishOrderBookAsync(IEnumerable<StrategyNotification> notifications);
        Task PublishAccountInfoAsync(IEnumerable<StrategyNotification> notifications);
    }
}
