﻿using DevelopmentInProgress.MarketView.Interface.Events;
using DevelopmentInProgress.MarketView.Interface.Interfaces;
using DevelopmentInProgress.MarketView.Interface.TradeStrategy;
using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

[assembly:InternalsVisibleTo("DevelopmentInProgress.MarketView.StrategyEngine.Test")]
namespace DevelopmentInProgress.TradeServer.StrategyEngine.Cache
{
    public class BinanceSymbolCache : ISymbolCache
    {
        private ConcurrentDictionary<string, Action<AggregateTradeEventArgs>> aggregateTradesSubscribers;

        private CancellationTokenSource aggregateTradesCancellationTokenSource;

        private bool disposed;

        public BinanceSymbolCache(string symbol, int limit, IExchangeService exchangeService)
        {
            Symbol = symbol;
            Limit = limit;
            ExchangeService = exchangeService;

            aggregateTradesSubscribers = new ConcurrentDictionary<string, Action<AggregateTradeEventArgs>>();

            aggregateTradesCancellationTokenSource = new CancellationTokenSource();
        }

        internal ConcurrentDictionary<string, Action<AggregateTradeEventArgs>> AggregateTradesSubscribers { get { return aggregateTradesSubscribers; } }

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
                aggregateTradesCancellationTokenSource.Cancel();
                aggregateTradesCancellationTokenSource.Dispose();
            }

            disposed = true;
        }

        public void Subscribe(string strategyName, StrategySymbol strategySymbol, ITradeStrategy tradeStrategy)
        {
            if (strategySymbol.Subscribe == MarketView.Interface.TradeStrategy.Subscribe.AggregateTrades)
            {
                SubscribeAggregateTrades(strategyName, tradeStrategy);
            }
        }

        public void Unsubscribe(string strategyName, StrategySymbol strategySymbol, ITradeStrategy tradeStrategy)
        {
            if (strategySymbol.Subscribe == MarketView.Interface.TradeStrategy.Subscribe.AggregateTrades)
            {
                if(!aggregateTradesSubscribers.TryRemove(strategyName, out Action<AggregateTradeEventArgs> e))
                {
                    tradeStrategy.SubscribeAggregateTradesException(new Exception($"Failed to unsubscribe {strategyName} from aggregate trades."));
                    return;
                }
            }

            if(!aggregateTradesSubscribers.Any())
            {
                if (!aggregateTradesCancellationTokenSource.IsCancellationRequested)
                {
                    aggregateTradesCancellationTokenSource.Cancel();
                }
            }
        }

        private void SubscribeAggregateTrades(string strategyName, ITradeStrategy tradeStrategy)
        {
            if (aggregateTradesCancellationTokenSource.IsCancellationRequested)
            {
                return;
            }

            var hasSubscribers = aggregateTradesSubscribers.Any();

            if (!aggregateTradesSubscribers.TryAdd(strategyName, tradeStrategy.SubscribeAggregateTrades))
            {
                tradeStrategy.SubscribeAggregateTradesException(new Exception($"Failed to subscribe {strategyName} to aggregate trades."));
                return;
            }

            if (!hasSubscribers)
            {
                ExchangeService.SubscribeAggregateTrades(Symbol, Limit, SubscribeAggregateTrades, SubscribeAggregateTradesException, aggregateTradesCancellationTokenSource.Token);
            }
        }        

        private async void SubscribeAggregateTrades(AggregateTradeEventArgs aggregateTradeEventArgs)
        {
            if(aggregateTradesCancellationTokenSource.IsCancellationRequested)
            {
                return;
            }

            var subscribers = (from s 
                               in aggregateTradesSubscribers.Values
                               select OnAggregateTradesUpdate(s, aggregateTradeEventArgs)).ToArray();
            
            await Task.WhenAll(subscribers);
        }

        private async Task OnAggregateTradesUpdate(Action<AggregateTradeEventArgs> action, AggregateTradeEventArgs aggregateTradeEventArgs)
        {
            await Task.Run(() => action.Invoke(aggregateTradeEventArgs));
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