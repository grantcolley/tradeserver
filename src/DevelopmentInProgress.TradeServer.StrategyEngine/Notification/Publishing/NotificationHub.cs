using Microsoft.AspNetCore.SignalR;
using System;
using System.Threading.Tasks;

namespace DevelopmentInProgress.TradeServer.StrategyEngine.Notification.Publishing
{
    public class NotificationHub : Hub
    {
        public override async Task OnConnectedAsync()
        {
            var strategyId = Context.GetHttpContext().Request.Query["strategyId"];
            await Groups.AddToGroupAsync(Context.ConnectionId, strategyId);
            await Clients.Client(Context.ConnectionId).SendAsync("Connected", $"Connected and listening for notifications from Strategy Id {strategyId}. ConnectionId {Context.ConnectionId}");
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
            return base.OnDisconnectedAsync(exception);
        }
    }
}
