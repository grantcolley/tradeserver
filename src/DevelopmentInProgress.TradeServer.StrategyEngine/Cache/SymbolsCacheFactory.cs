using DevelopmentInProgress.MarketView.Interface.Interfaces;
using DevelopmentInProgress.MarketView.Interface.TradeStrategy;
using DevelopmentInProgress.TradeServer.StrategyEngine.Cache.Binance;
using DevelopmentInProgress.TradeServer.StrategyEngine.ExchangeService;
using System.Collections.Generic;

namespace DevelopmentInProgress.TradeServer.StrategyEngine.Cache
{
    public class SymbolsCacheFactory : ISymbolsCacheFactory
    {
        private readonly Dictionary<Exchange, ISymbolsCache> exchangeSymbolsCache;

        public SymbolsCacheFactory(IExchangeServiceFactory<IExchangeService> exchangeServiceFactory)
        {
            exchangeSymbolsCache = new Dictionary<Exchange, ISymbolsCache>();
            exchangeSymbolsCache.Add(Exchange.Binance, new BinanceSymbolsCache(exchangeServiceFactory.GetExchangeService(Exchange.Binance)));
        }

        public ISymbolsCache GetSymbolsCache(Exchange exchange)
        {
            return exchangeSymbolsCache.GetValueOrDefault(exchange);
        }
    }
}