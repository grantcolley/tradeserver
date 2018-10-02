using DevelopmentInProgress.MarketView.Interface.Interfaces;
using DevelopmentInProgress.MarketView.Interface.Strategy;
using DevelopmentInProgress.TradeServer.StrategyRunner.WebHost.Cache.Binance;
using DevelopmentInProgress.TradeServer.StrategyRunner.WebHost.ExchangeService;
using System;
using System.Collections.Generic;

namespace DevelopmentInProgress.TradeServer.StrategyRunner.WebHost.Cache
{
    public class SubscriptionsCacheFactory : ISubscriptionsCacheFactory
    {
        private readonly Dictionary<Exchange, ISubscriptionsCache> exchangeSubscriptionsCache;

        private bool disposed;

        public SubscriptionsCacheFactory(IExchangeServiceFactory<IExchangeService> exchangeServiceFactory)
        {
            exchangeSubscriptionsCache = new Dictionary<Exchange, ISubscriptionsCache>();
            exchangeSubscriptionsCache.Add(Exchange.Binance, new BinanceSubscriptionsCache(exchangeServiceFactory.GetExchangeService(Exchange.Binance)));
        }

        public ISubscriptionsCache GetSubscriptionsCache(Exchange exchange)
        {
            return exchangeSubscriptionsCache.GetValueOrDefault(exchange);
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