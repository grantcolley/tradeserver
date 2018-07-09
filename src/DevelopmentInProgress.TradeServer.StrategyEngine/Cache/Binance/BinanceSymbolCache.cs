using DevelopmentInProgress.MarketView.Interface.Events;
using DevelopmentInProgress.MarketView.Interface.Interfaces;
using DevelopmentInProgress.MarketView.Interface.TradeStrategy;
using System;
using System.Runtime.CompilerServices;

[assembly:InternalsVisibleTo("DevelopmentInProgress.MarketView.StrategyEngine.Test")]
namespace DevelopmentInProgress.TradeServer.StrategyEngine.Cache.Binance
{
    public class BinanceSymbolCache : ISymbolCache
    {
        private SubscribeAggregateTrades subscribeAggregateTrades;

        private bool disposed;

        public BinanceSymbolCache(string symbol, int limit, IExchangeService exchangeService)
        {
            Symbol = symbol;
            Limit = limit;
            ExchangeService = exchangeService;

            subscribeAggregateTrades = new SubscribeAggregateTrades(symbol, limit, exchangeService);
        }

        public string Symbol { get; private set; }

        public int Limit { get; private set; }

        public IExchangeService ExchangeService { get; set; }

        public bool HasSubscriptions
        {
            get
            {
                return subscribeAggregateTrades.HasSubscriptions;
            }
        }

        public int Subscriptions(Subscribe subscribe)
        {
            switch(subscribe)
            {
                case MarketView.Interface.TradeStrategy.Subscribe.AggregateTrades:
                    return subscribeAggregateTrades.Subscriptions;
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
        }

        public void Unsubscribe(string strategyName, StrategySymbol strategySymbol, ITradeStrategy tradeStrategy)
        {
            if (strategySymbol.Subscribe == MarketView.Interface.TradeStrategy.Subscribe.AggregateTrades)
            {
                subscribeAggregateTrades.Unsubscribe(strategyName, tradeStrategy.SubscribeAggregateTradesException);
            }
        }

        //void SubscribeAggregateTrades(string symbol, int limit, Action<AggregateTradeEventArgs> callback, Action<Exception> exception);

        //void SubscribeStatistics(Action<StatisticsEventArgs> callback, Action<Exception> exception);
        //void SubscribeOrderBook(string symbol, int limit, Action<OrderBookEventArgs> callback, Action<Exception> exception, CancellationToken cancellationToken);
        //void SubscribeAccountInfo(Interface.Model.User user, Action<AccountInfoEventArgs> callback, Action<Exception> exception, CancellationToken cancellationToken);


        //void SubscribeStatistics(StatisticsEventArgs statisticsEventArgs);
        //void SubscribeStatisticsException(Exception exception);
        //void SubscribeOrderBook(OrderBookEventArgs orderBookEventArgs);
        //void SubscribeOrderBookException(Exception exception);
        //void SubscribeAccountInfo(AccountInfoEventArgs accountInfoEventArgs);
        //void SubscribeAccountInfoException(Exception exception);


    }
}