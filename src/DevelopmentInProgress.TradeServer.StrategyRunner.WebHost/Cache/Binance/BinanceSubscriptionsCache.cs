//using DevelopmentInProgress.TradeView.Interface.Enums;
//using DevelopmentInProgress.TradeView.Interface.Events;
//using DevelopmentInProgress.TradeView.Interface.Interfaces;
//using DevelopmentInProgress.TradeView.Interface.Model;
//using DevelopmentInProgress.TradeView.Interface.Strategy;
//using System;
//using System.Collections.Concurrent;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading;
//using System.Threading.Tasks;

//namespace DevelopmentInProgress.TradeServer.StrategyRunner.WebHost.Cache.Binance
//{
//    public class BinanceSubscriptionsCache : ISubscriptionsCache
//    {
//        private readonly string binance24HourStatisticsSubscriptionCacheKey = $"{nameof(BinanceSymbolSubscriptionCache)}";

//        private bool disposed;

//        public BinanceSubscriptionsCache(IExchangeApi exchangeApi)
//        {
//            ExchangeApi = exchangeApi;

//            Caches = new ConcurrentDictionary<string, ISubscriptionCache>();
//        }

//        public IExchangeApi ExchangeApi { get; private set; }

//        public ConcurrentDictionary<string, ISubscriptionCache> Caches { get; private set; }

//        public bool HasSubscriptions
//        {
//            get
//            {
//                return (from c in Caches.Values where c.HasSubscriptions select c).Any();
//            }
//        }

//        public async Task Subscribe(string strategyName, List<StrategySubscription> strategySubscriptions, ITradeStrategy tradeStrategy)
//        {
//            await tradeStrategy.AddExchangeService(strategySubscriptions, Exchange.Binance, ExchangeApi);

//            foreach (var strategySubscription in strategySubscriptions)
//            {
//                if (strategySubscription.Subscribe.HasFlag(TradeView.Interface.Strategy.Subscribe.Trades)
//                    || strategySubscription.Subscribe.HasFlag(TradeView.Interface.Strategy.Subscribe.OrderBook)
//                    || strategySubscription.Subscribe.HasFlag(TradeView.Interface.Strategy.Subscribe.Candlesticks))
//                {
//                    ISubscriptionCache symbolCache;
//                    if (!Caches.TryGetValue(strategySubscription.Symbol, out symbolCache))
//                    {
//                        symbolCache = new BinanceSymbolSubscriptionCache(strategySubscription.Symbol, strategySubscription.Limit, strategySubscription.CandlestickInterval, ExchangeApi);
//                        Caches.TryAdd(strategySubscription.Symbol, symbolCache);
//                    }

//                    symbolCache.Subscribe(strategyName, strategySubscription, tradeStrategy);
//                }

//                if(strategySubscription.Subscribe.HasFlag(TradeView.Interface.Strategy.Subscribe.AccountInfo))
//                {
//                    var accountInfo = await ExchangeApi.GetAccountInfoAsync(new User { ApiKey = strategySubscription.ApiKey, ApiSecret = strategySubscription.SecretKey }, new CancellationToken());

//                    tradeStrategy.SubscribeAccountInfo(new AccountInfoEventArgs { AccountInfo = accountInfo });

//                    ISubscriptionCache accountInfoCache;
//                    if (!Caches.TryGetValue(strategySubscription.ApiKey, out accountInfoCache))
//                    {
//                        accountInfoCache = new BinanceAccountInfoSubscriptionCache(ExchangeApi);
//                        Caches.TryAdd(strategySubscription.ApiKey, accountInfoCache);
//                    }

//                    accountInfoCache.Subscribe(strategyName, strategySubscription, tradeStrategy);
//                }
//            }
//        }

//        public void Unsubscribe(string strategyName, List<StrategySubscription> strategySubscriptions, ITradeStrategy tradeStrategy)
//        {
//            foreach (var strategySubscription in strategySubscriptions)
//            {
//                if (strategySubscription.Subscribe.HasFlag(TradeView.Interface.Strategy.Subscribe.Trades)
//                    || strategySubscription.Subscribe.HasFlag(TradeView.Interface.Strategy.Subscribe.OrderBook)
//                    || strategySubscription.Subscribe.HasFlag(TradeView.Interface.Strategy.Subscribe.Candlesticks))
//                {
//                    Unsubscribe(strategyName, strategySubscription, strategySubscription.Symbol, tradeStrategy);
//                }

//                if (strategySubscription.Subscribe.HasFlag(TradeView.Interface.Strategy.Subscribe.AccountInfo))
//                {
//                    Unsubscribe(strategyName, strategySubscription, strategySubscription.ApiKey, tradeStrategy);
//                }

//                if (strategySubscription.Subscribe.HasFlag(TradeView.Interface.Strategy.Subscribe.Statistics))
//                {
//                    Unsubscribe(strategyName, strategySubscription, binance24HourStatisticsSubscriptionCacheKey, tradeStrategy);
//                }
//            }
//        }

//        private void Unsubscribe(string strategyName, StrategySubscription symbol, string cacheKey, ITradeStrategy tradeStrategy)
//        {
//            if (Caches.TryGetValue(cacheKey, out ISubscriptionCache cache))
//            {
//                cache.Unsubscribe(strategyName, symbol, tradeStrategy);

//                if (!cache.HasSubscriptions)
//                {
//                    if (Caches.TryRemove(cacheKey, out ISubscriptionCache cacheDispose))
//                    {
//                        cacheDispose.Dispose();
//                    }
//                }
//            }
//        }

//        public void Dispose()
//        {
//            Dispose(true);
//            GC.SuppressFinalize(this);
//        }

//        private void Dispose(bool disposing)
//        {
//            if(disposed)
//            {
//                return;
//            }

//            if(disposing)
//            {
//                foreach (var cache in Caches)
//                {
//                    cache.Value.Dispose();
//                }

//                Caches.Clear();
//            }

//            disposed = true;
//        }
//    }
//}