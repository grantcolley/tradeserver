using DevelopmentInProgress.MarketView.Interface.Events;
using DevelopmentInProgress.MarketView.Interface.Interfaces;
using DevelopmentInProgress.MarketView.Interface.TradeStrategy;
using System;
using System.Runtime.CompilerServices;

[assembly:InternalsVisibleTo("DevelopmentInProgress.MarketView.StrategyEngine.Test")]
namespace DevelopmentInProgress.TradeServer.StrategyEngine.Cache.Binance
{
    public class BinanceSubscriptionCache : ISubscriptionCache
    {
        private SubscribeAggregateTrades subscribeAggregateTrades;
        private SubscribeAccountInfo subscribeAccountInfo;
        private SubscribeStatistics subscribeStatistics;
        private SubscribeOrderBook subscribeOrderBook;

        private bool disposed;

        public BinanceSubscriptionCache(string symbol, int limit, IExchangeService exchangeService)
        {
            Symbol = symbol;
            Limit = limit;
            ExchangeService = exchangeService;

            subscribeAggregateTrades = new SubscribeAggregateTrades(symbol, limit, exchangeService);
            subscribeOrderBook = new SubscribeOrderBook(symbol, limit, exchangeService);
            subscribeAccountInfo = new SubscribeAccountInfo(exchangeService);
            subscribeStatistics = new SubscribeStatistics(exchangeService);
        }
        
        public string Symbol { get; private set; }

        public int Limit { get; private set; }

        public IExchangeService ExchangeService { get; set; }

        public bool HasSubscriptions
        {
            get
            {
                return subscribeAggregateTrades.HasSubscriptions
                    || subscribeAccountInfo.HasSubscriptions
                    || subscribeOrderBook.HasSubscriptions
                    || subscribeStatistics.HasSubscriptions;
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
                case MarketView.Interface.TradeStrategy.Subscribe.AccountInfo:
                    return subscribeAccountInfo.Subscriptions;
                case MarketView.Interface.TradeStrategy.Subscribe.Statistics:
                    return subscribeStatistics.Subscriptions;
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
            }

            disposed = true;
        }

        public void Subscribe(string strategyName, StrategySymbol strategySymbol, ITradeStrategy tradeStrategy)
        {
            if (strategySymbol.Subscribe == MarketView.Interface.TradeStrategy.Subscribe.AggregateTrades)
            {
                var aggregateTrades = new StrategyNotification<AggregateTradeEventArgs>
                {
                    Update = tradeStrategy.SubscribeAggregateTrades,
                    Exception = tradeStrategy.SubscribeAggregateTradesException
                };

                subscribeAggregateTrades.Subscribe(strategyName, aggregateTrades);
            }

            if(strategySymbol.Subscribe == MarketView.Interface.TradeStrategy.Subscribe.AccountInfo)
            {
                var accountInfo = new StrategyNotification<AccountInfoEventArgs>
                {
                    Update = tradeStrategy.SubscribeAccountInfo,
                    Exception = tradeStrategy.SubscribeAccountInfoException
                };

                subscribeAccountInfo.User.ApiKey = strategySymbol.ApiKey;
                subscribeAccountInfo.User.ApiSecret = strategySymbol.ApiKey;
                subscribeAccountInfo.Subscribe(strategyName, accountInfo);
            }

            if (strategySymbol.Subscribe == MarketView.Interface.TradeStrategy.Subscribe.OrderBook)
            {
                var orderBook = new StrategyNotification<OrderBookEventArgs>
                {
                    Update = tradeStrategy.SubscribeOrderBook,
                    Exception = tradeStrategy.SubscribeOrderBookException
                };

                subscribeOrderBook.Subscribe(strategyName, orderBook);
            }

            if (strategySymbol.Subscribe == MarketView.Interface.TradeStrategy.Subscribe.Statistics)
            {
                var statistics = new StrategyNotification<StatisticsEventArgs>
                {
                    Update = tradeStrategy.SubscribeStatistics,
                    Exception = tradeStrategy.SubscribeStatisticsException
                };

                subscribeStatistics.Subscribe(strategyName, statistics);
            }
        }

        public void Unsubscribe(string strategyName, StrategySymbol strategySymbol, ITradeStrategy tradeStrategy)
        {
            if (strategySymbol.Subscribe == MarketView.Interface.TradeStrategy.Subscribe.AggregateTrades)
            {
                subscribeAggregateTrades.Unsubscribe(strategyName, tradeStrategy.SubscribeAggregateTradesException);
            }
        }
    }
}