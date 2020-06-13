﻿using DevelopmentInProgress.TradeView.Core.Enums;
using DevelopmentInProgress.TradeView.Core.Events;
using DevelopmentInProgress.TradeView.Core.Interfaces;
using DevelopmentInProgress.TradeView.Core.Model;
using DevelopmentInProgress.TradeView.Core.Strategy;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System;
using System.Threading;

namespace DevelopmentInProgress.TradeServer.StrategyRunner.WebHost.Cache.Subscriptions
{
    public class ExchangeSubscriptionsCache : IExchangeSubscriptionsCache
    {
        private readonly IExchangeService exchangeService;
        private readonly Exchange exchange;
        private bool disposed;

        public ExchangeSubscriptionsCache(Exchange exchange, IExchangeService exchangeService)
        {
            this.exchange = exchange;
            this.exchangeService = exchangeService;

            Caches = new ConcurrentDictionary<string, ISubscriptionCache>();
        }

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
            await tradeStrategy.AddExchangeService(strategySubscriptions, exchange, exchangeService);

            var exchangeApi = exchangeService.GetExchangeApi(exchange);

            foreach (var strategySubscription in strategySubscriptions)
            {
                if (strategySubscription.Subscribes.HasFlag(TradeView.Core.Strategy.Subscribes.Trades)
                    || strategySubscription.Subscribes.HasFlag(TradeView.Core.Strategy.Subscribes.OrderBook)
                    || strategySubscription.Subscribes.HasFlag(TradeView.Core.Strategy.Subscribes.Candlesticks))
                {
                    ISubscriptionCache symbolCache;
                    if (!Caches.TryGetValue(strategySubscription.Symbol, out symbolCache))
                    {
                        symbolCache = new SymbolSubscriptionCache(strategySubscription.Symbol, strategySubscription.Limit, strategySubscription.CandlestickInterval, exchangeApi);
                        Caches.TryAdd(strategySubscription.Symbol, symbolCache);
                    }

                    symbolCache.Subscribe(strategyName, strategySubscription, tradeStrategy);
                }

                if (strategySubscription.Subscribes.HasFlag(TradeView.Core.Strategy.Subscribes.AccountInfo))
                {
                    var user = new User
                    {
                        ApiKey = strategySubscription.ApiKey,
                        ApiSecret = strategySubscription.SecretKey,
                        ApiPassPhrase = strategySubscription.ApiPassPhrase,
                        Exchange = strategySubscription.Exchange
                    };

                    var accountInfo = await exchangeService.GetAccountInfoAsync(exchange, user, new CancellationToken());

                    tradeStrategy.SubscribeAccountInfo(new AccountInfoEventArgs { AccountInfo = accountInfo });

                    ISubscriptionCache accountInfoCache;
                    if (!Caches.TryGetValue(strategySubscription.ApiKey, out accountInfoCache))
                    {
                        accountInfoCache = new AccountInfoSubscriptionCache(exchangeApi);
                        Caches.TryAdd(strategySubscription.ApiKey, accountInfoCache);
                    }

                    accountInfoCache.Subscribe(strategyName, strategySubscription, tradeStrategy);
                }
            }
        }

        public void Unsubscribe(string strategyName, List<StrategySubscription> strategySubscriptions, ITradeStrategy tradeStrategy)
        {
            foreach (var strategySubscription in strategySubscriptions)
            {
                if (strategySubscription.Subscribes.HasFlag(TradeView.Core.Strategy.Subscribes.Trades)
                    || strategySubscription.Subscribes.HasFlag(TradeView.Core.Strategy.Subscribes.OrderBook)
                    || strategySubscription.Subscribes.HasFlag(TradeView.Core.Strategy.Subscribes.Candlesticks))
                {
                    Unsubscribe(strategyName, strategySubscription, strategySubscription.Symbol, tradeStrategy);
                }

                if (strategySubscription.Subscribes.HasFlag(TradeView.Core.Strategy.Subscribes.AccountInfo))
                {
                    Unsubscribe(strategyName, strategySubscription, strategySubscription.ApiKey, tradeStrategy);
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
            if (disposed)
            {
                return;
            }

            if (disposing)
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
