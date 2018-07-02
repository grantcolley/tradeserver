using DevelopmentInProgress.MarketView.Interface.Interfaces;
using DevelopmentInProgress.MarketView.Interface.TradeStrategy;
using DevelopmentInProgress.TradeServer.StrategyEngine.ExchangeService;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace DevelopmentInProgress.TradeServer.StrategyEngine.Cache
{
    public class SymbolsCache : ISymbolsCache
    {
        private IExchangeServiceFactory<IExchangeService> exchangeServiceFactory;
        private Dictionary<Exchange, ConcurrentDictionary<string, SymbolCache>> exchangesSymbolsCache;

        public SymbolsCache(IExchangeServiceFactory<IExchangeService> exchangeServiceFactory)
        {
            this.exchangeServiceFactory = exchangeServiceFactory;

            exchangesSymbolsCache = new Dictionary<Exchange, ConcurrentDictionary<string, SymbolCache>>();
            exchangesSymbolsCache.Add(Exchange.Binance, new ConcurrentDictionary<string, SymbolCache>());
        }

        public void Subscribe(Strategy strategy, ITradeStrategy tradeStrategy)
        {
            foreach(var symbol in strategy.Symbols)
            {
                if(exchangesSymbolsCache.TryGetValue(symbol.Exchange, out ConcurrentDictionary<string, SymbolCache>  exchangeSymbolsCache))
                {
                    SymbolCache symbolCache;
                    if (!exchangeSymbolsCache.TryGetValue(symbol.Symbol, out symbolCache))
                    {
                        symbolCache = new SymbolCache(symbol.Symbol, exchangeServiceFactory.GetExchangeService(symbol.Exchange));
                        exchangeSymbolsCache.TryAdd(symbol.Symbol, symbolCache);
                    }

                    symbolCache.Subscribe(tradeStrategy);
                }
            }
        }

        public void Unsubscribe(Strategy strategy, ITradeStrategy tradeStrategy)
        {
            foreach (var symbol in strategy.Symbols)
            {
                if (exchangesSymbolsCache.TryGetValue(symbol.Exchange, out ConcurrentDictionary<string, SymbolCache> exchangeSymbolsCache))
                {
                    if (exchangeSymbolsCache.TryGetValue(symbol.Symbol, out SymbolCache symbolCache))
                    {
                        symbolCache.Unsubscribe(tradeStrategy);

                        if (!symbolCache.HasSubscriptions)
                        {
                            if (exchangeSymbolsCache.TryRemove(symbol.Symbol, out SymbolCache symbolCacheDispose))
                            {
                                symbolCacheDispose.Dispose();
                            }
                         }
                    }
                }
            }
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}