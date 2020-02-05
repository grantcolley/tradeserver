using DevelopmentInProgress.TradeView.Interface.Strategy;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace DevelopmentInProgress.TradeServer.StrategyRunner.WebHost.Cache
{
    public class SubscriptionsCacheManager : ISubscriptionsCacheManager
    {
        private bool disposed;

        public SubscriptionsCacheManager(IExchangeSubscriptionsCacheFactory exchangeSubscriptionsCacheFactory)
        {
            ExchangeSubscriptionsCacheFactory = exchangeSubscriptionsCacheFactory;
        }

        public IExchangeSubscriptionsCacheFactory ExchangeSubscriptionsCacheFactory { get; private set; }

        public async Task Subscribe(Strategy strategy, ITradeStrategy tradeStrategy)
        {
            var exchangeSymbolsList = (from s in strategy.StrategySubscriptions
                                  group s by s.Exchange into es
                                  select new { Exchange = es.Key, StrategySubscriptions = es.ToList() }).ToList();

            foreach(var exchangeSymbols in exchangeSymbolsList)
            {
                var exchangeSubscriptionsCache = ExchangeSubscriptionsCacheFactory.GetExchangeSubscriptionsCache(exchangeSymbols.Exchange);
                await exchangeSubscriptionsCache.Subscribe(strategy.Name, exchangeSymbols.StrategySubscriptions, tradeStrategy);
            }
        }

        public void Unsubscribe(Strategy strategy, ITradeStrategy tradeStrategy)
        {
            var exchangeSymbolsList = (from s in strategy.StrategySubscriptions
                                       group s by s.Exchange into es
                                       select new { Exchange = es.Key, StrategySubscriptions = es.ToList() }).ToList();

            foreach (var exchangeSymbols in exchangeSymbolsList)
            {
                var exchangeSubscriptionsCache = ExchangeSubscriptionsCacheFactory.GetExchangeSubscriptionsCache(exchangeSymbols.Exchange);
                exchangeSubscriptionsCache.Unsubscribe(strategy.Name, exchangeSymbols.StrategySubscriptions, tradeStrategy);
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
                ExchangeSubscriptionsCacheFactory.Dispose();
            }

            disposed = true;
        }
    }
}