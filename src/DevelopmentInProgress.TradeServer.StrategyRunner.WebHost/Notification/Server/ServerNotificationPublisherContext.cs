﻿using DevelopmentInProgress.TradeView.Interface.Server;
using DipSocket.Messages;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DevelopmentInProgress.TradeServer.StrategyRunner.WebHost.Notification.Server
{
    public class ServerNotificationPublisherContext : IServerNotificationPublisherContext
    {
        private readonly ServerNotificationHub notificationHub;
        private readonly IServerMonitor serverMonitor;

        public ServerNotificationPublisherContext(IServerMonitor serverMonitor, ServerNotificationHub notificationHub)
        {
            this.notificationHub = notificationHub;

            this.serverMonitor = serverMonitor;
        }

        public async Task PublishNotificationsAsync(IEnumerable<ServerNotification> notification)
        {
            var json = JsonConvert.SerializeObject(notification);
            var msg = new Message { SenderConnectionId = serverMonitor.Name, MessageType = MessageType.SendToChannel, MethodName = "OnNotification", Data = json };
            await notificationHub.SendMessageToChannelAsync(serverMonitor.Name, msg);
        }
    }
}
