using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DevelopmentInProgress.TradeView.Interface.Interfaces;
using DevelopmentInProgress.TradeView.Interface.Strategy;
using DevelopmentInProgress.TradeServer.StrategyRunner.WebHost.Cache;

namespace DevelopmentInProgress.MarketView.StrategyRunner.Test.Helpers
{
    public class TestSubscriptionsCache : IExchangeSubscriptionsCache
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

        public async Task Subscribe(string strategyName, List<StrategySubscription> strategySubscription, ITradeStrategy tradeStrategy)
        {
            await Task.Factory.StartNew(()=>
            {
                foreach (var subscription in strategySubscription)
                {
                    Caches.TryAdd(subscription.Symbol, new TestSubscriptionCache());
                }
            });
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
