using DevelopmentInProgress.MarketView.Interface.Interfaces;
using DevelopmentInProgress.MarketView.Interface.TradeStrategy;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace DevelopmentInProgress.TradeServer.StrategyEngine.Cache
{
    public interface ISubscriptionsCache : IDisposable
    {
        bool HasSubscriptions { get; }
        IExchangeService ExchangeService { get; }
        ConcurrentDictionary<string, ISubscriptionCache> Caches { get; }
        void Subscribe(string strategyName, List<StrategySubscription> strategySubscription, ITradeStrategy tradeStrategy);
        void Unsubscribe(string strategyName, List<StrategySubscription> strategySubscription, ITradeStrategy tradeStrategy);
    }
}