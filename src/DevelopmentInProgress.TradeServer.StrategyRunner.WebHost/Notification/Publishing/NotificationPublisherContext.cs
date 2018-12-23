﻿using DevelopmentInProgress.MarketView.Interface.Strategy;
using DipSocket.Messages;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DevelopmentInProgress.TradeServer.StrategyRunner.WebHost.Notification.Publishing
{
    public class NotificationPublisherContext : INotificationPublisherContext
    {
        private readonly NotificationHub notificationHub;

        public NotificationPublisherContext(NotificationHub notificationHub)
        {
            this.notificationHub = notificationHub;
        }

        public async Task PublishNotificationsAsync(string strategyName, IEnumerable<StrategyNotification> notification)
        {
            var json = JsonConvert.SerializeObject(notification);
            var msg = new Message { SenderConnectionId = strategyName, MethodName = "Notification", Data = json };
            await notificationHub.SendMessageToChannelAsync(strategyName, msg);
        }

        public async Task PublishTradesAsync(string strategyName, IEnumerable<StrategyNotification> notification)
        {
            var json = JsonConvert.SerializeObject(notification);
            var msg = new Message { SenderConnectionId = strategyName, MethodName = "Trade", Data = json };
            await notificationHub.SendMessageToChannelAsync(strategyName, msg);
        }

        public async Task PublishOrderBookAsync(string strategyName, IEnumerable<StrategyNotification> notification)
        {
            var json = JsonConvert.SerializeObject(notification);
            var msg = new Message { SenderConnectionId = strategyName, MethodName = "OrderBook", Data = json };
            await notificationHub.SendMessageToChannelAsync(strategyName, msg);
        }

        public async Task PublishAccountInfoAsync(string strategyName, IEnumerable<StrategyNotification> notification)
        {
            var json = JsonConvert.SerializeObject(notification);
            var msg = new Message { SenderConnectionId = strategyName, MethodName = "AccountInfo", Data = json };
            await notificationHub.SendMessageToChannelAsync(strategyName, msg);
        }
    }
}
