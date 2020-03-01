﻿using DevelopmentInProgress.TradeView.Interface.Enums;
using System;

namespace DevelopmentInProgress.TradeServer.StrategyRunner.WebHost.Cache.Subscriptions
{
    public interface IExchangeSubscriptionsCacheFactory : IDisposable
    {
        IExchangeSubscriptionsCache GetExchangeSubscriptionsCache(Exchange exchange);
    }
}