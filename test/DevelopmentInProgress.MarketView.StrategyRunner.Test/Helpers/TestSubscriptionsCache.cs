using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using DevelopmentInProgress.MarketView.Interface.Interfaces;
using DevelopmentInProgress.MarketView.Interface.Strategy;
using DevelopmentInProgress.TradeServer.StrategyRunner.WebHost.Cache;

namespace DevelopmentInProgress.MarketView.StrategyRunner.Test.Helpers
{
    public class TestSubscriptionsCache : ISubscriptionsCache
    {
        public TestSubscriptionsCache(IExchangeService exchangeService)
        {
            ExchangeService = exchangeService;
            Caches = new ConcurrentDictionary<string, ISubscriptionCache>();
        }

        public bool HasSubscriptions
        {
            get { return Caches.Any(); }
        }

        public IExchangeService ExchangeService { get; private set; }

        public ConcurrentDictionary<string, ISubscriptionCache> Caches { get; private set; }

        public void Dispose()
        {
        }

        public void Subscribe(string strategyName, List<StrategySubscription> strategySubscription, ITradeStrategy tradeStrategy)
        {
            foreach (var subscription in strategySubscription)
            {
                Caches.TryAdd(subscription.Symbol, new TestSubscriptionCache());
            }
        }

        public void Unsubscribe(string strategyName, List<StrategySubscription> strategySubscription, ITradeStrategy tradeStrategy)
        {
            foreach (var subscription in strategySubscription)
            {
                Caches.TryRemove(subscription.Symbol, out ISubscriptionCache subscriptionCache);
            }
        }
    }
}
