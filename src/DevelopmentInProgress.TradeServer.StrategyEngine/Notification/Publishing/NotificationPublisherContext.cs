using DevelopmentInProgress.MarketView.Interface.TradeStrategy;
using Microsoft.AspNetCore.SignalR;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DevelopmentInProgress.TradeServer.StrategyEngine.Notification.Publishing
{
    public class NotificationPublisherContext : INotificationPublisherContext
    {
        private readonly IHubContext<NotificationHub> context;

        public NotificationPublisherContext(IHubContext<NotificationHub> context)
        {
            this.context = context;
        }

        public async Task NotifyAsync(string strategyName, IEnumerable<StrategyNotification> message)
        {
            var clients = context.Clients;
            var groups = context.Groups;

            await context.Clients.Group(strategyName).SendAsync("Trade", message);
        }
    }
}
