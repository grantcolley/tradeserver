using DevelopmentInProgress.TradeView.Interface.Server;
using DipSocket.Messages;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DevelopmentInProgress.TradeServer.StrategyRunner.WebHost.Notification.Server
{
    public class ServerNotificationPublisherContext : IServerNotificationPublisherContext
    {
        private readonly IServer server;
        private readonly ServerNotificationHub notificationHub;
        private readonly string serverName;

        public ServerNotificationPublisherContext(IServer server, ServerNotificationHub notificationHub)
        {
            this.server = server;
            this.notificationHub = notificationHub;

            serverName = this.server.Name;
            this.notificationHub.SetServerChannelName(serverName);
        }

        public async Task PublishNotificationsAsync(IEnumerable<ServerNotification> notification)
        {
            var json = JsonConvert.SerializeObject(notification);
            var msg = new Message { SenderConnectionId = serverName, MessageType = MessageType.SendToChannel, MethodName = "Dashbord", Data = json };
            await notificationHub.SendMessageToChannelAsync(serverName, msg);
        }
    }
}
