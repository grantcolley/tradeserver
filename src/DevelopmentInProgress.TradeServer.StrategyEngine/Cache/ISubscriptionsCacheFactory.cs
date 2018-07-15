using DevelopmentInProgress.MarketView.Interface.TradeStrategy;
using System;

namespace DevelopmentInProgress.TradeServer.StrategyEngine.Cache
{
    public interface ISubscriptionsCacheFactory : IDisposable
    {
        ISubscriptionsCache GetSubscriptionsCache(Exchange exchange);
    }
}