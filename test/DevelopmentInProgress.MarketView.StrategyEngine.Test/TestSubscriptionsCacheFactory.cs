using DevelopmentInProgress.MarketView.Interface.Interfaces;
using DevelopmentInProgress.MarketView.Interface.TradeStrategy;
using DevelopmentInProgress.MarketView.StrategyEngine.Test.Helpers;
using DevelopmentInProgress.TradeServer.StrategyEngine.Cache;
using DevelopmentInProgress.TradeServer.StrategyEngine.Cache.Binance;
using DevelopmentInProgress.TradeServer.StrategyEngine.ExchangeService;
using System.Collections.Generic;

namespace DevelopmentInProgress.MarketView.StrategyEngine.Test
{
    public class TestSubscriptionsCacheFactory : ISubscriptionsCacheFactory
    {
        private readonly Dictionary<Exchange, ISubscriptionsCache> exchangeSubscriptionsCache;

        public TestSubscriptionsCacheFactory(IExchangeServiceFactory<IExchangeService> exchangeServiceFactory)
        {
            exchangeSubscriptionsCache = new Dictionary<Exchange, ISubscriptionsCache>();
            exchangeSubscriptionsCache.Add(Exchange.Binance, new BinanceSubscriptionsCache(exchangeServiceFactory.GetExchangeService(Exchange.Binance)));
            exchangeSubscriptionsCache.Add(Exchange.Test, new TestSubscriptionsCache(exchangeServiceFactory.GetExchangeService(Exchange.Test)));
        }

        public ISubscriptionsCache GetSubscriptionsCache(Exchange exchange)
        {
            return exchangeSubscriptionsCache.GetValueOrDefault(exchange);
        }

        public void Dispose()
        {
        }
    }
}
