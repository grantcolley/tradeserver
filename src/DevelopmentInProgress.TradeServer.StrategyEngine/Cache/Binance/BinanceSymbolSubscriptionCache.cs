using DevelopmentInProgress.MarketView.Interface.Events;
using DevelopmentInProgress.MarketView.Interface.Interfaces;
using DevelopmentInProgress.MarketView.Interface.TradeStrategy;
using System;
using System.Runtime.CompilerServices;

[assembly:InternalsVisibleTo("DevelopmentInProgress.MarketView.StrategyEngine.Test")]
namespace DevelopmentInProgress.TradeServer.StrategyEngine.Cache.Binance
{
    public class BinanceSymbolSubscriptionCache : ISubscriptionCache
    {
        private SubscribeAggregateTrades subscribeAggregateTrades;
        private SubscribeOrderBook subscribeOrderBook;

        private bool disposed;

        public BinanceSymbolSubscriptionCache(string symbol, int limit, IExchangeService exchangeService)
        {
            Symbol = symbol;
            Limit = limit;
            ExchangeService = exchangeService;

            subscribeAggregateTrades = new SubscribeAggregateTrades(symbol, limit, exchangeService);
            subscribeOrderBook = new SubscribeOrderBook(symbol, limit, exchangeService);
        }
        
        public string Symbol { get; private set; }

        public int Limit { get; private set; }

        public IExchangeService ExchangeService { get; set; }

        public bool HasSubscriptions
        {
            get
            {
                return subscribeAggregateTrades.HasSubscriptions
                    || subscribeOrderBook.HasSubscriptions;
            }
        }

        public int Subscriptions(Subscribe subscribe)
        {
            switch(subscribe)
            {
                case MarketView.Interface.TradeStrategy.Subscribe.AggregateTrades:
                    return subscribeAggregateTrades.Subscriptions;
                case MarketView.Interface.TradeStrategy.Subscribe.OrderBook:
                    return subscribeOrderBook.Subscriptions;
                default:
                    throw new NotImplementedException($"{this.GetType().Name}.Subscriptions({subscribe})");
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
                subscribeAggregateTrades.Dispose();
                subscribeOrderBook.Dispose();
            }

            disposed = true;
        }

        public void Subscribe(string strategyName, StrategySymbol strategySymbol, ITradeStrategy tradeStrategy)
        {
            if (strategySymbol.Subscribe.HasFlag(MarketView.Interface.TradeStrategy.Subscribe.AggregateTrades))
            {
                var aggregateTrades = new StrategyNotification<AggregateTradeEventArgs>
                {
                    Update = tradeStrategy.SubscribeAggregateTrades,
                    Exception = tradeStrategy.SubscribeAggregateTradesException
                };

                subscribeAggregateTrades.Subscribe(strategyName, aggregateTrades);
            }

            if (strategySymbol.Subscribe.HasFlag(MarketView.Interface.TradeStrategy.Subscribe.OrderBook))
            {
                var orderBook = new StrategyNotification<OrderBookEventArgs>
                {
                    Update = tradeStrategy.SubscribeOrderBook,
                    Exception = tradeStrategy.SubscribeOrderBookException
                };

                subscribeOrderBook.Subscribe(strategyName, orderBook);
            }
        }

        public void Unsubscribe(string strategyName, StrategySymbol strategySymbol, ITradeStrategy tradeStrategy)
        {
            if (strategySymbol.Subscribe.HasFlag(MarketView.Interface.TradeStrategy.Subscribe.AggregateTrades))
            {
                subscribeAggregateTrades.Unsubscribe(strategyName, tradeStrategy.SubscribeAggregateTradesException);
            }

            if (strategySymbol.Subscribe.HasFlag(MarketView.Interface.TradeStrategy.Subscribe.OrderBook))
            {
                subscribeOrderBook.Unsubscribe(strategyName, tradeStrategy.SubscribeOrderBookException);
            }
        }
    }
}