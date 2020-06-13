﻿using DevelopmentInProgress.TradeView.Core.Events;
using DevelopmentInProgress.TradeView.Core.Interfaces;
using DevelopmentInProgress.TradeView.Core.Strategy;
using System;

namespace DevelopmentInProgress.TradeServer.StrategyRunner.WebHost.Cache.Subscriptions
{
    public class SymbolSubscriptionCache : ISubscriptionCache
    {
        private SubscribeTrades subscribeTrades;
        private SubscribeOrderBook subscribeOrderBook;
        private SubscribeCandlesticks subscribeCandlesticks;

        private bool disposed;

        public SymbolSubscriptionCache(string symbol, int limit, TradeView.Core.Model.CandlestickInterval candlestickInterval, IExchangeApi exchangeApi)
        {
            Symbol = symbol;
            Limit = limit;
            ExchangeApi = exchangeApi;

            subscribeTrades = new SubscribeTrades(symbol, limit, ExchangeApi);
            subscribeOrderBook = new SubscribeOrderBook(symbol, limit, ExchangeApi);
            subscribeCandlesticks = new SubscribeCandlesticks(symbol, limit, ExchangeApi, candlestickInterval);
        }

        public string Symbol { get; private set; }

        public int Limit { get; private set; }

        public IExchangeApi ExchangeApi { get; set; }

        public bool HasSubscriptions
        {
            get
            {
                return subscribeTrades.HasSubscriptions
                    || subscribeOrderBook.HasSubscriptions
                    || subscribeCandlesticks.HasSubscriptions;
            }
        }

        public int Subscriptions(Subscribes subscribes)
        {
            switch (subscribes)
            {
                case TradeView.Core.Strategy.Subscribes.Trades:
                    return subscribeTrades.Subscriptions;
                case TradeView.Core.Strategy.Subscribes.OrderBook:
                    return subscribeOrderBook.Subscriptions;
                case TradeView.Core.Strategy.Subscribes.Candlesticks:
                    return subscribeCandlesticks.Subscriptions;
                default:
                    throw new NotImplementedException($"{this.GetType().Name}.Subscriptions({subscribes})");
            }
        }

        public void Subscribe(string strategyName, StrategySubscription strategySubscription, ITradeStrategy tradeStrategy)
        {
            if (strategySubscription.Subscribes.HasFlag(TradeView.Core.Strategy.Subscribes.Trades))
            {
                var trades = new StrategyNotification<TradeEventArgs>
                {
                    Update = tradeStrategy.SubscribeTrades,
                    Exception = tradeStrategy.SubscribeTradesException
                };

                subscribeTrades.Subscribe(strategyName, trades);
            }

            if (strategySubscription.Subscribes.HasFlag(TradeView.Core.Strategy.Subscribes.OrderBook))
            {
                var orderBook = new StrategyNotification<OrderBookEventArgs>
                {
                    Update = tradeStrategy.SubscribeOrderBook,
                    Exception = tradeStrategy.SubscribeOrderBookException
                };

                subscribeOrderBook.Subscribe(strategyName, orderBook);
            }

            if (strategySubscription.Subscribes.HasFlag(TradeView.Core.Strategy.Subscribes.Candlesticks))
            {
                var candlestick = new StrategyNotification<CandlestickEventArgs>
                {
                    Update = tradeStrategy.SubscribeCandlesticks,
                    Exception = tradeStrategy.SubscribeCandlesticksException
                };

                subscribeCandlesticks.Subscribe(strategyName, candlestick);
            }
        }

        public void Unsubscribe(string strategyName, StrategySubscription strategySubscription, ITradeStrategy tradeStrategy)
        {
            if (strategySubscription.Subscribes.HasFlag(TradeView.Core.Strategy.Subscribes.Trades))
            {
                subscribeTrades.Unsubscribe(strategyName, tradeStrategy.SubscribeTradesException);
            }

            if (strategySubscription.Subscribes.HasFlag(TradeView.Core.Strategy.Subscribes.OrderBook))
            {
                subscribeOrderBook.Unsubscribe(strategyName, tradeStrategy.SubscribeOrderBookException);
            }

            if (strategySubscription.Subscribes.HasFlag(TradeView.Core.Strategy.Subscribes.Candlesticks))
            {
                subscribeCandlesticks.Unsubscribe(strategyName, tradeStrategy.SubscribeCandlesticksException);
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
                subscribeTrades.Dispose();
                subscribeOrderBook.Dispose();
                subscribeCandlesticks.Dispose();
            }

            disposed = true;
        }
    }
}
