using DevelopmentInProgress.MarketView.Interface.Interfaces;
using DevelopmentInProgress.MarketView.Interface.TradeStrategy;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace DevelopmentInProgress.TradeServer.StrategyEngine.Cache.Binance
{
    public class BinanceSubscriptionsCache : ISubscriptionsCache
    {
        private readonly ConcurrentDictionary<string, BinanceSubscriptionCache> symbolsCache;

        private bool disposed;

        public BinanceSubscriptionsCache(IExchangeService exchangeService)
        {
            ExchangeService = exchangeService;

            symbolsCache = new ConcurrentDictionary<string, BinanceSubscriptionCache>();
        }

        public IExchangeService ExchangeService { get; private set; }

        public void Subscribe(string strategyName, List<StrategySymbol> strategySymbols, ITradeStrategy tradeStrategy)
        {
            foreach (var symbol in strategySymbols)
            {
                BinanceSubscriptionCache symbolCache;
                if (!symbolsCache.TryGetValue(symbol.Symbol, out symbolCache))
                {
                    symbolCache = new BinanceSubscriptionCache(symbol.Symbol, symbol.Limit, ExchangeService);
                    symbolsCache.TryAdd(symbol.Symbol, symbolCache);
                }

                symbolCache.Subscribe(strategyName, symbol, tradeStrategy);
            }
        }

        public void Unsubscribe(string strategyName, List<StrategySymbol> strategySymbols, ITradeStrategy tradeStrategy)
        {
            foreach (var symbol in strategySymbols)
            {
                if (symbolsCache.TryGetValue(symbol.Symbol, out BinanceSubscriptionCache symbolCache))
                {
                    symbolCache.Unsubscribe(strategyName, symbol, tradeStrategy);

                    if (!symbolCache.HasSubscriptions)
                    {
                        if (symbolsCache.TryRemove(symbol.Symbol, out BinanceSubscriptionCache symbolCacheDispose))
                        {
                            symbolCacheDispose.Dispose();
                        }
                    }
                }
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if(disposed)
            {
                return;
            }

            if(disposing)
            {
                foreach (var symbolCache in symbolsCache)
                {
                    symbolCache.Value.Dispose();
                }
            }

            disposed = true;
        }
    }
}