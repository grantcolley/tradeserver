using DevelopmentInProgress.MarketView.Interface.Events;
using DevelopmentInProgress.MarketView.Interface.Interfaces;
using DevelopmentInProgress.MarketView.Interface.TradeStrategy;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace DevelopmentInProgress.TradeServer.StrategyEngine.Cache
{
    public class BinanceSymbolCache : ISymbolCache
    {
        private ConcurrentDictionary<string, Action<AggregateTradeEventArgs>> aggregateTradesSubscribers;

        private CancellationTokenSource aggregateTradesCancellationTokenSource;

        public BinanceSymbolCache(string symbol, int limit, IExchangeService exchangeService)
        {
            Symbol = symbol;
            Limit = limit;
            ExchangeService = exchangeService;

            aggregateTradesSubscribers = new ConcurrentDictionary<string, Action<AggregateTradeEventArgs>>();

            aggregateTradesCancellationTokenSource = new CancellationTokenSource();
        }

        public string Symbol { get; private set; }

        public int Limit { get; private set; }

        public IExchangeService ExchangeService { get; private set; }

        public bool HasSubscriptions
        {
            get
            {
                if(aggregateTradesSubscribers.Any())
                {
                    return true;
                }

                return false;
            }
        }

        public void Dispose()
        {
            if(!aggregateTradesCancellationTokenSource.IsCancellationRequested)
            {
                aggregateTradesCancellationTokenSource.Cancel();
            }
        }

        public void Subscribe(string strategyName, StrategySymbol strategySymbol, ITradeStrategy tradeStrategy)
        {
            throw new System.NotImplementedException();
        }

        public void Unsubscribe(string strategyName, StrategySymbol strategySymbol, ITradeStrategy tradeStrategy)
        {
            throw new System.NotImplementedException();
        }

        void SubscribeAggregateTrades(string strategyName, ITradeStrategy tradeStrategy)
        {
            if (aggregateTradesCancellationTokenSource.IsCancellationRequested)
            {
                return;
            }

            var hasSubscribers = aggregateTradesSubscribers.Any();

            if (!aggregateTradesSubscribers.Keys.Contains(strategyName))
            {
                if (!aggregateTradesSubscribers.TryAdd(strategyName, tradeStrategy.SubscribeAggregateTrades))
                {
                    tradeStrategy.SubscribeAggregateTradesException(new Exception("Failed to subscribe to aggregate trades."));
                    return;
                }
            }

            if (!hasSubscribers)
            {
                ExchangeService.SubscribeAggregateTrades(Symbol, Limit, SubscribeAggregateTrades, SubscribeAggregateTradesException, aggregateTradesCancellationTokenSource.Token);
            }
        }        

        private void SubscribeAggregateTrades(AggregateTradeEventArgs aggregateTradeEventArgs)
        {

        }

        private void SubscribeAggregateTradesException(Exception exception)
        {

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