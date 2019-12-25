﻿using DevelopmentInProgress.TradeView.Interface.Interfaces;
using DevelopmentInProgress.TradeView.Interface.Strategy;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DevelopmentInProgress.TradeServer.StrategyRunner.WebHost.Cache
{
    public interface IExchangeSubscriptionsCache : IDisposable
    {
        bool HasSubscriptions { get; }
        ConcurrentDictionary<string, ISubscriptionCache> Caches { get; }
        Task Subscribe(string strategyName, List<StrategySubscription> strategySubscription, ITradeStrategy tradeStrategy);
        void Unsubscribe(string strategyName, List<StrategySubscription> strategySubscription, ITradeStrategy tradeStrategy);
    }
}