using DevelopmentInProgress.MarketView.Interface.Events;
using DevelopmentInProgress.MarketView.Interface.Interfaces;
using DevelopmentInProgress.MarketView.Interface.Model;
using DevelopmentInProgress.MarketView.Interface.Strategy;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DevelopmentInProgress.TradeServer.StrategyRunner.WebHost.Cache.Binance
{
    public class BinanceSubscriptionsCache : ISubscriptionsCache
    {
        private readonly string binance24HourStatisticsSubscriptionCacheKey = $"{nameof(BinanceSymbolSubscriptionCache)}";

        private bool disposed;

        public BinanceSubscriptionsCache(IExchangeService exchangeService)
        {
            ExchangeService = exchangeService;

            Caches = new ConcurrentDictionary<string, ISubscriptionCache>();
        }

        public IExchangeService ExchangeService { get; private set; }

        public ConcurrentDictionary<string, ISubscriptionCache> Caches { get; private set; }

        public bool HasSubscriptions
        {
            get
            {
                return (from c in Caches.Values where c.HasSubscriptions select c).Any();
            }
        }

        public async Task Subscribe(string strategyName, List<StrategySubscription> strategySubscriptions, ITradeStrategy tradeStrategy)
        {
            tradeStrategy.AddExchangeService(strategySubscriptions, Exchange.Binance, ExchangeService);

            foreach (var symbol in strategySubscriptions)
            {
                if (symbol.Subscribe.HasFlag(MarketView.Interface.Strategy.Subscribe.Trades)
                    || symbol.Subscribe.HasFlag(MarketView.Interface.Strategy.Subscribe.OrderBook))
                {
                    ISubscriptionCache symbolCache;
                    if (!Caches.TryGetValue(symbol.Symbol, out symbolCache))
                    {
                        symbolCache = new BinanceSymbolSubscriptionCache(symbol.Symbol, symbol.Limit, ExchangeService);
                        Caches.TryAdd(symbol.Symbol, symbolCache);
                    }

                    symbolCache.Subscribe(strategyName, symbol, tradeStrategy);
                }

                if(symbol.Subscribe.HasFlag(MarketView.Interface.Strategy.Subscribe.AccountInfo))
                {
                    var accountInfo = await ExchangeService.GetAccountInfoAsync(new User { ApiKey = symbol.ApiKey, ApiSecret = symbol.SecretKey }, new CancellationToken());

                    tradeStrategy.SubscribeAccountInfo(new AccountInfoEventArgs { AccountInfo = accountInfo });

                    ISubscriptionCache accountInfoCache;
                    if (!Caches.TryGetValue(symbol.ApiKey, out accountInfoCache))
                    {
                        accountInfoCache = new BinanceAccountInfoSubscriptionCache(ExchangeService);
                        Caches.TryAdd(symbol.ApiKey, accountInfoCache);
                    }

                    accountInfoCache.Subscribe(strategyName, symbol, tradeStrategy);
                }

                if (symbol.Subscribe.HasFlag(MarketView.Interface.Strategy.Subscribe.Statistics))
                {
                    ISubscriptionCache statisticsCache;
                    if (!Caches.TryGetValue(binance24HourStatisticsSubscriptionCacheKey, out statisticsCache))
                    {
                        statisticsCache = new Binance24HourStatisticsSubscriptionCache(ExchangeService);
                        Caches.TryAdd(binance24HourStatisticsSubscriptionCacheKey, statisticsCache);
                    }

                    statisticsCache.Subscribe(strategyName, symbol, tradeStrategy);
                }
            }
        }

        public void Unsubscribe(string strategyName, List<StrategySubscription> strategySubscriptions, ITradeStrategy tradeStrategy)
        {
            foreach (var symbol in strategySubscriptions)
            {
                if (symbol.Subscribe.HasFlag(MarketView.Interface.Strategy.Subscribe.Trades)
                    || symbol.Subscribe.HasFlag(MarketView.Interface.Strategy.Subscribe.OrderBook))
                {
                    Unsubscribe(strategyName, symbol, symbol.Symbol, tradeStrategy);
                }

                if (symbol.Subscribe.HasFlag(MarketView.Interface.Strategy.Subscribe.AccountInfo))
                {
                    Unsubscribe(strategyName, symbol, symbol.ApiKey, tradeStrategy);
                }

                if (symbol.Subscribe.HasFlag(MarketView.Interface.Strategy.Subscribe.Statistics))
                {
                    Unsubscribe(strategyName, symbol, binance24HourStatisticsSubscriptionCacheKey, tradeStrategy);
                }
            }
        }

        private void Unsubscribe(string strategyName, StrategySubscription symbol, string cacheKey, ITradeStrategy tradeStrategy)
        {
            if (Caches.TryGetValue(cacheKey, out ISubscriptionCache cache))
            {
                cache.Unsubscribe(strategyName, symbol, tradeStrategy);

                if (!cache.HasSubscriptions)
                {
                    if (Caches.TryRemove(cacheKey, out ISubscriptionCache cacheDispose))
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
                foreach (var cache in Caches)
                {
                    cache.Value.Dispose();
                }

                Caches.Clear();
            }

            disposed = true;
        }
    }
}