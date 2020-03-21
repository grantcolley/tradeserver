﻿using DevelopmentInProgress.TradeServer.StrategyRunner.WebHost.Notification.Server;
using DevelopmentInProgress.TradeView.Interface.Strategy;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DevelopmentInProgress.TradeServer.StrategyRunner.WebHost.Cache.TradeStrategy
{
    public interface ITradeStrategyCacheManager : IServerNotification
    {
        bool TryAddTradeStrategy(string strategyName, ITradeStrategy tradeStrategy);
        bool TryRemoveTradeStrategy(string strategyName, out ITradeStrategy tradeStrategy);
        bool TryGetTradeStrategy(string strategyName, out ITradeStrategy tradeStrategy);
        Task StopStrategy(string strategyName, string parameters);
        Task UpdateStrategy(string strategyName, string parameters);
        List<Strategy> GetStrategies();
    }
}