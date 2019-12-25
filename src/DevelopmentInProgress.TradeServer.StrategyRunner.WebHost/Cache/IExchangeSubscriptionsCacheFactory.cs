using DevelopmentInProgress.TradeView.Interface.Enums;
using System;

namespace DevelopmentInProgress.TradeServer.StrategyRunner.WebHost.Cache
{
    public interface IExchangeSubscriptionsCacheFactory : IDisposable
    {
        IExchangeSubscriptionsCache GetExchangeSubscriptionsCache(Exchange exchange);
    }
}