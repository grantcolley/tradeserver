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
            await tradeStrategy.AddExchangeService(strategySubscriptions, Exchange.Binance, ExchangeService);

            foreach (var strategySubscription in strategySubscriptions)
            {
                if (strategySubscription.Subscribe.HasFlag(MarketView.Interface.Strategy.Subscribe.Trades)
                    || strategySubscription.Subscribe.HasFlag(MarketView.Interface.Strategy.Subscribe.OrderBook)
                    || strategySubscription.Subscribe.HasFlag(MarketView.Interface.Strategy.Subscribe.Candlesticks))
                {
                    ISubscriptionCache symbolCache;
                    if (!Caches.TryGetValue(strategySubscription.Symbol, out symbolCache))
                    {
                        symbolCache = new BinanceSymbolSubscriptionCache(strategySubscription.Symbol, strategySubscription.Limit, strategySubscription.CandlestickInterval, ExchangeService);
                        Caches.TryAdd(strategySubscription.Symbol, symbolCache);
                    }

                    symbolCache.Subscribe(strategyName, strategySubscription, tradeStrategy);
                }

                if(strategySubscription.Subscribe.HasFlag(MarketView.Interface.Strategy.Subscribe.AccountInfo))
                {
                    var accountInfo = await ExchangeService.GetAccountInfoAsync(new User { ApiKey = strategySubscription.ApiKey, ApiSecret = strategySubscription.SecretKey }, new CancellationToken());

                    tradeStrategy.SubscribeAccountInfo(new AccountInfoEventArgs { AccountInfo = accountInfo });

                    ISubscriptionCache accountInfoCache;
                    if (!Caches.TryGetValue(strategySubscription.ApiKey, out accountInfoCache))
                    {
                        accountInfoCache = new BinanceAccountInfoSubscriptionCache(ExchangeService);
                        Caches.TryAdd(strategySubscription.ApiKey, accountInfoCache);
                    }

                    accountInfoCache.Subscribe(strategyName, strategySubscription, tradeStrategy);
                }

                if (strategySubscription.Subscribe.HasFlag(MarketView.Interface.Strategy.Subscribe.Statistics))
                {
                    ISubscriptionCache statisticsCache;
                    if (!Caches.TryGetValue(binance24HourStatisticsSubscriptionCacheKey, out statisticsCache))
                    {
                        statisticsCache = new Binance24HourStatisticsSubscriptionCache(ExchangeService);
                        Caches.TryAdd(binance24HourStatisticsSubscriptionCacheKey, statisticsCache);
                    }

                    statisticsCache.Subscribe(strategyName, strategySubscription, tradeStrategy);
                }
            }
        }

        public void Unsubscribe(string strategyName, List<StrategySubscription> strategySubscriptions, ITradeStrategy tradeStrategy)
        {
            foreach (var strategySubscription in strategySubscriptions)
            {
                if (strategySubscription.Subscribe.HasFlag(MarketView.Interface.Strategy.Subscribe.Trades)
                    || strategySubscription.Subscribe.HasFlag(MarketView.Interface.Strategy.Subscribe.OrderBook)
                    || strategySubscription.Subscribe.HasFlag(MarketView.Interface.Strategy.Subscribe.Candlesticks))
                {
                    Unsubscribe(strategyName, strategySubscription, strategySubscription.Symbol, tradeStrategy);
                }

                if (strategySubscription.Subscribe.HasFlag(MarketView.Interface.Strategy.Subscribe.AccountInfo))
                {
                    Unsubscribe(strategyName, strategySubscription, strategySubscription.ApiKey, tradeStrategy);
                }

                if (strategySubscription.Subscribe.HasFlag(MarketView.Interface.Strategy.Subscribe.Statistics))
                {
                    Unsubscribe(strategyName, strategySubscription, binance24HourStatisticsSubscriptionCacheKey, tradeStrategy);
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