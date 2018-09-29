﻿using DevelopmentInProgress.MarketView.Interface.Strategy;
using System;

namespace DevelopmentInProgress.TradeServer.StrategyEngine.Cache
{
    public interface ISubscriptionsCacheManager : IDisposable
    {
        ISubscriptionsCacheFactory SubscriptionsCacheFactory { get; }
        void Subscribe(Strategy strategy, ITradeStrategy tradeStrategy);
        void Unsubscribe(Strategy strategy, ITradeStrategy tradeStrategy);
    }
}