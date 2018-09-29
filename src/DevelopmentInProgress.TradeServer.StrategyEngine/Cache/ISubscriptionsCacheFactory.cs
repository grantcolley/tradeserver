using DevelopmentInProgress.MarketView.Interface.Strategy;
using System;

namespace DevelopmentInProgress.TradeServer.StrategyEngine.Cache
{
    public interface ISubscriptionsCacheFactory : IDisposable
    {
        ISubscriptionsCache GetSubscriptionsCache(Exchange exchange);
    }
}