﻿using DevelopmentInProgress.TradeView.Core.Interfaces;
using DevelopmentInProgress.TradeView.Core.Strategy;
using System;

namespace DevelopmentInProgress.TradeServer.StrategyRunner.WebHost.Cache.Subscriptions
{
    public interface ISubscriptionCache : IDisposable
    {
        IExchangeApi ExchangeApi { get; }
        bool HasSubscriptions { get; }
        int Subscriptions(Subscribe subscribe);
        void Subscribe(string strategyName, StrategySubscription strategySubscription, ITradeStrategy tradeStrategy);
        void Unsubscribe(string strategyName, StrategySubscription strategySubscription, ITradeStrategy tradeStrategy);
    }
}