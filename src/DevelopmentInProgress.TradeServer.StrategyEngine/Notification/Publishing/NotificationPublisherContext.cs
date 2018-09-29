using DevelopmentInProgress.MarketView.Interface.Strategy;
using Microsoft.AspNetCore.SignalR;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DevelopmentInProgress.TradeServer.StrategyEngine.Notification.Publishing
{
    public class NotificationPublisherContext : INotificationPublisherContext
    {
        private readonly IHubContext<NotificationHub> hubContext;

        public NotificationPublisherContext(IHubContext<NotificationHub> hubContext)
        {
            this.hubContext = hubContext;
        }

        public async Task PublishNotificationsAsync(string strategyName, IEnumerable<StrategyNotification> message)
        {
            await hubContext.Clients.Group(strategyName).SendAsync("Notification", message);
        }

        public async Task PublishTradesAsync(string strategyName, IEnumerable<StrategyNotification> message)
        {
            await hubContext.Clients.Group(strategyName).SendAsync("Trade", message);
        }

        public async Task PublishOrderBookAsync(string strategyName, IEnumerable<StrategyNotification> message)
        {
            await hubContext.Clients.Group(strategyName).SendAsync("OrderBook", message);
        }

        public async Task PublishAccountInfoAsync(string strategyName, IEnumerable<StrategyNotification> message)
        {
            await hubContext.Clients.Group(strategyName).SendAsync("AccountInfo", message);
        }
    }
}
