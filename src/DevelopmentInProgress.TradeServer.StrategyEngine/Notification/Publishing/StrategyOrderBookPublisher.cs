﻿using DevelopmentInProgress.MarketView.Interface.TradeStrategy;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace DevelopmentInProgress.TradeServer.StrategyEngine.Notification.Publishing
{
    public class StrategyOrderBookPublisher : BatchNotification<StrategyNotification>, IBatchNotification<StrategyNotification>
    {
        private readonly INotificationPublisher notificationPublisher;

        public StrategyOrderBookPublisher(INotificationPublisher notificationPublisher)
        {
            this.notificationPublisher = notificationPublisher;

            Start();
        }

        public override async Task NotifyAsync(IEnumerable<StrategyNotification> notifications, CancellationToken cancellationToken)
        {
            await notificationPublisher.PublishOrderBookAsync(notifications);
        }
    }
}
