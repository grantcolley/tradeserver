using DevelopmentInProgress.MarketView.Interface.Events;
using DevelopmentInProgress.MarketView.Interface.Interfaces;
using DevelopmentInProgress.MarketView.Interface.Strategy;
using System;
using System.Runtime.CompilerServices;

[assembly:InternalsVisibleTo("DevelopmentInProgress.MarketView.StrategyRunner.Test")]
namespace DevelopmentInProgress.TradeServer.StrategyRunner.WebHost.Cache.Binance
{
    public class BinanceSymbolSubscriptionCache : ISubscriptionCache
    {
        private SubscribeTrades subscribeTrades;
        private SubscribeOrderBook subscribeOrderBook;
        private SubscribeCandlesticks subscribeCandlesticks;

        private bool disposed;

        public BinanceSymbolSubscriptionCache(string symbol, int limit, MarketView.Interface.Model.CandlestickInterval candlestickInterval, IExchangeService exchangeService)
        {
            Symbol = symbol;
            Limit = limit;
            ExchangeService = exchangeService;

            subscribeTrades = new SubscribeTrades(symbol, limit, exchangeService);
            subscribeOrderBook = new SubscribeOrderBook(symbol, limit, exchangeService);
            subscribeCandlesticks = new SubscribeCandlesticks(symbol, candlestickInterval, limit, exchangeService);
        }
        
        public string Symbol { get; private set; }

        public int Limit { get; private set; }

        public IExchangeService ExchangeService { get; set; }

        public bool HasSubscriptions
        {
            get
            {
                return subscribeTrades.HasSubscriptions
                    || subscribeOrderBook.HasSubscriptions
                    || subscribeCandlesticks.HasSubscriptions;
            }
        }

        public int Subscriptions(Subscribe subscribe)
        {
            switch(subscribe)
            {
                case MarketView.Interface.Strategy.Subscribe.Trades:
                    return subscribeTrades.Subscriptions;
                case MarketView.Interface.Strategy.Subscribe.OrderBook:
                    return subscribeOrderBook.Subscriptions;
                case MarketView.Interface.Strategy.Subscribe.Candlesticks:
                    return subscribeCandlesticks.Subscriptions;
                default:
                    throw new NotImplementedException($"{this.GetType().Name}.Subscriptions({subscribe})");
            }
        }

        public void Subscribe(string strategyName, StrategySubscription strategySubscription, ITradeStrategy tradeStrategy)
        {
            if (strategySubscription.Subscribe.HasFlag(MarketView.Interface.Strategy.Subscribe.Trades))
            {
                var trades = new StrategyNotification<TradeEventArgs>
                {
                    Update = tradeStrategy.SubscribeTrades,
                    Exception = tradeStrategy.SubscribeTradesException
                };

                subscribeTrades.Subscribe(strategyName, trades);
            }

            if (strategySubscription.Subscribe.HasFlag(MarketView.Interface.Strategy.Subscribe.OrderBook))
            {
                var orderBook = new StrategyNotification<OrderBookEventArgs>
                {
                    Update = tradeStrategy.SubscribeOrderBook,
                    Exception = tradeStrategy.SubscribeOrderBookException
                };

                subscribeOrderBook.Subscribe(strategyName, orderBook);
            }

            if(strategySubscription.Subscribe.HasFlag(MarketView.Interface.Strategy.Subscribe.Candlesticks))
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
            if (strategySubscription.Subscribe.HasFlag(MarketView.Interface.Strategy.Subscribe.Trades))
            {
                subscribeTrades.Unsubscribe(strategyName, tradeStrategy.SubscribeTradesException);
            }

            if (strategySubscription.Subscribe.HasFlag(MarketView.Interface.Strategy.Subscribe.OrderBook))
            {
                subscribeOrderBook.Unsubscribe(strategyName, tradeStrategy.SubscribeOrderBookException);
            }

            if (strategySubscription.Subscribe.HasFlag(MarketView.Interface.Strategy.Subscribe.Candlesticks))
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