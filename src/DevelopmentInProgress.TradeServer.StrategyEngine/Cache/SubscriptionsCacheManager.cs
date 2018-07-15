using DevelopmentInProgress.MarketView.Interface.TradeStrategy;
using System;
using System.Linq;

namespace DevelopmentInProgress.TradeServer.StrategyEngine.Cache
{
    public class SubscriptionsCacheManager : ISubscriptionsCacheManager
    {
        private bool disposed;

        public SubscriptionsCacheManager(ISubscriptionsCacheFactory subscriptionsCacheFactory)
        {
            SubscriptionsCacheFactory = subscriptionsCacheFactory;
        }

        public ISubscriptionsCacheFactory SubscriptionsCacheFactory { get; private set; }

        public void Subscribe(Strategy strategy, ITradeStrategy tradeStrategy)
        {
            var exchangeSymbolsList = (from s in strategy.StrategySubscriptions
                                  group s by s.Exchange into es
                                  select new { Exchange = es.Key, StrategySubscriptions = es.ToList() }).ToList();

            foreach(var exchangeSymbols in exchangeSymbolsList)
            {
                var symbolsCache = SubscriptionsCacheFactory.GetSubscriptionsCache(exchangeSymbols.Exchange);
                symbolsCache.Subscribe(strategy.Name, exchangeSymbols.StrategySubscriptions, tradeStrategy);
            }
        }

        public void Unsubscribe(Strategy strategy, ITradeStrategy tradeStrategy)
        {
            var exchangeSymbolsList = (from s in strategy.StrategySubscriptions
                                       group s by s.Exchange into es
                                       select new { Exchange = es.Key, StrategySubscriptions = es.ToList() }).ToList();

            foreach (var exchangeSymbols in exchangeSymbolsList)
            {
                var symbolsCache = SubscriptionsCacheFactory.GetSubscriptionsCache(exchangeSymbols.Exchange);
                symbolsCache.Unsubscribe(strategy.Name, exchangeSymbols.StrategySubscriptions, tradeStrategy);
            }
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
                SubscriptionsCacheFactory.Dispose();
            }

            disposed = true;
        }
    }
}