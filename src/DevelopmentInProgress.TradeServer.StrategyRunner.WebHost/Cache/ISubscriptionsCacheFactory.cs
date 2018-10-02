using DevelopmentInProgress.MarketView.Interface.Strategy;
using System;

namespace DevelopmentInProgress.TradeServer.StrategyRunner.WebHost.Cache
{
    public interface ISubscriptionsCacheFactory : IDisposable
    {
        ISubscriptionsCache GetSubscriptionsCache(Exchange exchange);
    }
}