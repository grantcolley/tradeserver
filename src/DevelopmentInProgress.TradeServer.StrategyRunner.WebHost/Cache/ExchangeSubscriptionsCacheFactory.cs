﻿using DevelopmentInProgress.TradeView.Interface.Interfaces;
using DevelopmentInProgress.TradeView.Interface.Enums;
using System;
using System.Collections.Concurrent;

namespace DevelopmentInProgress.TradeServer.StrategyRunner.WebHost.Cache
{
    public class ExchangeSubscriptionsCacheFactory : IExchangeSubscriptionsCacheFactory
    {
        private readonly IExchangeService exchangeService;
        private readonly ConcurrentDictionary<Exchange, IExchangeSubscriptionsCache> exchangeSubscriptionsCache;

        private bool disposed;

        public ExchangeSubscriptionsCacheFactory(IExchangeService exchangeService)
        {
            this.exchangeService = exchangeService;
            exchangeSubscriptionsCache = new ConcurrentDictionary<Exchange, IExchangeSubscriptionsCache>();
        }

        public IExchangeSubscriptionsCache GetExchangeSubscriptionsCache(Exchange exchange)
        {
            return exchangeSubscriptionsCache.GetOrAdd(exchange, new ExchangeSubscriptionsCache(exchange, exchangeService));
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (disposed)
            {
                return;
            }

            if (disposing)
            {
                foreach(var subscriptionsCache in exchangeSubscriptionsCache.Values)
                {
                    subscriptionsCache.Dispose();
                }
            }

            disposed = true;
        }
    }
}