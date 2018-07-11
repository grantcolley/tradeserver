using DevelopmentInProgress.MarketView.Interface.Interfaces;
using DevelopmentInProgress.MarketView.Interface.TradeStrategy;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace DevelopmentInProgress.TradeServer.StrategyEngine.Cache.Binance
{
    public class BinanceSubscriptionsCache : ISubscriptionsCache
    {
        private readonly string binance24HourStatisticsSubscriptionCacheKey = $"{nameof(BinanceSymbolSubscriptionCache)}";

        private readonly ConcurrentDictionary<string, ISubscriptionCache> caches;

        private bool disposed;

        public BinanceSubscriptionsCache(IExchangeService exchangeService)
        {
            ExchangeService = exchangeService;

            caches = new ConcurrentDictionary<string, ISubscriptionCache>();
        }

        public IExchangeService ExchangeService { get; private set; }

        public void Subscribe(string strategyName, List<StrategySymbol> strategySymbols, ITradeStrategy tradeStrategy)
        {
            foreach (var symbol in strategySymbols)
            {
                if (symbol.Subscribe == (MarketView.Interface.TradeStrategy.Subscribe.AggregateTrades | MarketView.Interface.TradeStrategy.Subscribe.OrderBook))
                {
                    ISubscriptionCache symbolCache;
                    if (!caches.TryGetValue(symbol.Symbol, out symbolCache))
                    {
                        symbolCache = new BinanceSymbolSubscriptionCache(symbol.Symbol, symbol.Limit, ExchangeService);
                        caches.TryAdd(symbol.Symbol, symbolCache);
                    }

                    symbolCache.Subscribe(strategyName, symbol, tradeStrategy);
                }

                if(symbol.Subscribe == MarketView.Interface.TradeStrategy.Subscribe.AccountInfo)
                {
                    ISubscriptionCache accountInfoCache;
                    if (!caches.TryGetValue(symbol.ApiKey, out accountInfoCache))
                    {
                        accountInfoCache = new BinanceAccountInfoSubscriptionCache(ExchangeService);
                        caches.TryAdd(symbol.ApiKey, accountInfoCache);
                    }

                    accountInfoCache.Subscribe(strategyName, symbol, tradeStrategy);
                }

                if (symbol.Subscribe == MarketView.Interface.TradeStrategy.Subscribe.Statistics)
                {
                    ISubscriptionCache statisticsCache;
                    if (!caches.TryGetValue(binance24HourStatisticsSubscriptionCacheKey, out statisticsCache))
                    {
                        statisticsCache = new Binance24HourStatisticsSubscriptionCache(ExchangeService);
                        caches.TryAdd(binance24HourStatisticsSubscriptionCacheKey, statisticsCache);
                    }

                    statisticsCache.Subscribe(strategyName, symbol, tradeStrategy);
                }
            }
        }

        public void Unsubscribe(string strategyName, List<StrategySymbol> strategySymbols, ITradeStrategy tradeStrategy)
        {
            foreach (var symbol in strategySymbols)
            {
                if (symbol.Subscribe == (MarketView.Interface.TradeStrategy.Subscribe.AggregateTrades | MarketView.Interface.TradeStrategy.Subscribe.OrderBook))
                {
                    Unsubscribe(strategyName, symbol, symbol.Symbol, tradeStrategy);
                }

                if (symbol.Subscribe == MarketView.Interface.TradeStrategy.Subscribe.AccountInfo)
                {
                    Unsubscribe(strategyName, symbol, symbol.ApiKey, tradeStrategy);
                }

                if (symbol.Subscribe == MarketView.Interface.TradeStrategy.Subscribe.Statistics)
                {
                    Unsubscribe(strategyName, symbol, binance24HourStatisticsSubscriptionCacheKey, tradeStrategy);
                }
            }
        }

        public void Unsubscribe(string strategyName, StrategySymbol symbol, string cacheKey, ITradeStrategy tradeStrategy)
        {
            if (caches.TryGetValue(cacheKey, out ISubscriptionCache cache))
            {
                cache.Unsubscribe(strategyName, symbol, tradeStrategy);

                if (!cache.HasSubscriptions)
                {
                    if (caches.TryRemove(cacheKey, out ISubscriptionCache cacheDispose))
                    {
                        cacheDispose.Dispose();
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
                foreach (var cache in caches)
                {
                    cache.Value.Dispose();
                }
            }

            disposed = true;
        }
    }
}