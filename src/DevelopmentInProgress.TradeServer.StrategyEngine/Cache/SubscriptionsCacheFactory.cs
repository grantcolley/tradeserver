using DevelopmentInProgress.MarketView.Interface.Interfaces;
using DevelopmentInProgress.MarketView.Interface.TradeStrategy;
using DevelopmentInProgress.TradeServer.StrategyEngine.Cache.Binance;
using DevelopmentInProgress.TradeServer.StrategyEngine.ExchangeService;
using System.Collections.Generic;

namespace DevelopmentInProgress.TradeServer.StrategyEngine.Cache
{
    public class SubscriptionsCacheFactory : ISubscriptionsCacheFactory
    {
        private readonly Dictionary<Exchange, ISubscriptionsCache> exchangeSubscriptionsCache;

        public SubscriptionsCacheFactory(IExchangeServiceFactory<IExchangeService> exchangeServiceFactory)
        {
            exchangeSubscriptionsCache = new Dictionary<Exchange, ISubscriptionsCache>();
            exchangeSubscriptionsCache.Add(Exchange.Binance, new BinanceSubscriptionsCache(exchangeServiceFactory.GetExchangeService(Exchange.Binance)));
        }

        public ISubscriptionsCache GetSubscriptionsCache(Exchange exchange)
        {
            return exchangeSubscriptionsCache.GetValueOrDefault(exchange);
        }
    }
}