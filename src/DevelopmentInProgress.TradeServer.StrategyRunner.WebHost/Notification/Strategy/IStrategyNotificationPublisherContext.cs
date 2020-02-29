﻿using DevelopmentInProgress.TradeView.Interface.Strategy;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DevelopmentInProgress.TradeServer.StrategyRunner.WebHost.Notification.Strategy
{
    public interface IStrategyNotificationPublisherContext
    {
        Task PublishCustomNotificationsAsync(string strategyName, string methodName, IEnumerable<StrategyNotification> message);
        Task PublishNotificationsAsync(string strategyName, IEnumerable<StrategyNotification> message);
        Task PublishTradesAsync(string strategyName, IEnumerable<StrategyNotification> message);
        Task PublishOrderBookAsync(string strategyName, IEnumerable<StrategyNotification> message);
        Task PublishAccountInfoAsync(string strategyName, IEnumerable<StrategyNotification> message);
        Task PublishCandlesticksAsync(string strategyName, IEnumerable<StrategyNotification> message);
        Task PublishStatisticsAsync(string strategyName, IEnumerable<StrategyNotification> message);
    }
}