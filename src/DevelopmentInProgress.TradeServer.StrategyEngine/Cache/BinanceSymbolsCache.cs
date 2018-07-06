using DevelopmentInProgress.MarketView.Interface.Interfaces;
using DevelopmentInProgress.MarketView.Interface.TradeStrategy;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace DevelopmentInProgress.TradeServer.StrategyEngine.Cache
{
    public class BinanceSymbolsCache : ISymbolsCache
    {
        private readonly ConcurrentDictionary<string, BinanceSymbolCache> symbolsCache;

        public BinanceSymbolsCache(IExchangeService exchangeService)
        {
            ExchangeService = exchangeService;

            symbolsCache = new ConcurrentDictionary<string, BinanceSymbolCache>();
        }

        public IExchangeService ExchangeService { get; private set; }

        public void Subscribe(string strategyName, List<StrategySymbol> strategySymbols, ITradeStrategy tradeStrategy)
        {
            foreach (var symbol in strategySymbols)
            {
                BinanceSymbolCache symbolCache;
                if (!symbolsCache.TryGetValue(symbol.Symbol, out symbolCache))
                {
                    symbolCache = new BinanceSymbolCache(symbol.Symbol, symbol.Limit, ExchangeService);
                    symbolsCache.TryAdd(symbol.Symbol, symbolCache);
                }

                symbolCache.Subscribe(strategyName, symbol, tradeStrategy);
            }
        }

        public void Unsubscribe(string strategyName, List<StrategySymbol> strategySymbols, ITradeStrategy tradeStrategy)
        {
            foreach (var symbol in strategySymbols)
            {
                if (symbolsCache.TryGetValue(symbol.Symbol, out BinanceSymbolCache symbolCache))
                {
                    symbolCache.Unsubscribe(strategyName, symbol, tradeStrategy);

                    if (!symbolCache.HasSubscriptions)
                    {
                        if (symbolsCache.TryRemove(symbol.Symbol, out BinanceSymbolCache symbolCacheDispose))
                        {
                            symbolCacheDispose.Dispose();
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