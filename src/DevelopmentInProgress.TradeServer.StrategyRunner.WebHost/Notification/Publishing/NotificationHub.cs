using Microsoft.AspNetCore.SignalR;
using System;
using System.Threading.Tasks;

namespace DevelopmentInProgress.TradeServer.StrategyRunner.WebHost.Notification.Publishing
{
    public class NotificationHub : Hub
    {
        public override async Task OnConnectedAsync()
        {
            var strategyName = Context.GetHttpContext().Request.Query["strategyname"];
            await Groups.AddToGroupAsync(Context.ConnectionId, strategyName);
            await Clients.Client(Context.ConnectionId).SendAsync("Connected", $"Connected and listening for notifications from Strategy {strategyName}. ConnectionId {Context.ConnectionId}");
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
            return base.OnDisconnectedAsync(exception);
        }
    }
}
