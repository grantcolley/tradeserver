using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DevelopmentInProgress.TradeView.Interface.Enums;
using DevelopmentInProgress.TradeView.Interface.Events;
using DevelopmentInProgress.TradeView.Interface.Interfaces;
using DevelopmentInProgress.TradeView.Interface.Model;
using DevelopmentInProgress.TradeView.Interface.Strategy;

namespace DevelopmentInProgress.MarketView.StrategyRunner.Test.Helpers
{
    public class TestTradeStrategy : ITradeStrategy
    {
        public event EventHandler<StrategyNotificationEventArgs> StrategyAccountInfoEvent;
        public event EventHandler<StrategyNotificationEventArgs> StrategyCustomNotificationEvent;
        public event EventHandler<StrategyNotificationEventArgs> StrategyNotificationEvent;
        public event EventHandler<StrategyNotificationEventArgs> StrategyOrderBookEvent;
        public event EventHandler<StrategyNotificationEventArgs> StrategyTradeEvent;
        public event EventHandler<StrategyNotificationEventArgs> StrategyCandlesticksEvent;
        public event EventHandler<StrategyNotificationEventArgs> StrategyStatisticsEvent;
        public event EventHandler<StrategyNotificationEventArgs> StrategyParameterUpdateEvent;

        public IEnumerable<ITrade> AggregateTrades { get; set; }
        public IEnumerable<SymbolStats> Statistics { get; set; }
        public OrderBook OrderBook { get; set; }
        public AccountInfo AccountInfo { get; set; }

        public List<string> TradeSymbols = new List<string>();
        public List<string> OrderBookSymbols = new List<string>();

        public bool AggregateTradesException { get; set; }
        public bool OrderBookException { get; set; }
        public bool StatisticsException { get; set; }
        public bool AccountInfoException { get; set; }
        public Strategy Strategy { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        private object tradeLock = new object();
        private object orderBookLock = new object();

        public void SetStrategy(Strategy strategy)
        {
            throw new NotImplementedException();
        }

        public Task<Strategy> RunAsync(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public void SubscribeAccountInfo(AccountInfoEventArgs accountInfoEventArgs)
        {
            AccountInfo = accountInfoEventArgs.AccountInfo;
        }

        public void SubscribeAccountInfoException(Exception exception)
        {
            AccountInfoException = true;
        }

        public void SubscribeTrades(TradeEventArgs tradeEventArgs)
        {
            lock (tradeLock)
            {
                AggregateTrades = tradeEventArgs.Trades;

                var symbol = tradeEventArgs.Trades.First().Symbol;

                if (!TradeSymbols.Contains(symbol))
                {
                    TradeSymbols.Add(symbol);
                }
            }
        }

        public void SubscribeTradesException(Exception exception)
        {
            AggregateTradesException = true;
        }

        public void SubscribeOrderBook(OrderBookEventArgs orderBookEventArgs)
        {
            lock (orderBookLock)
            {
                OrderBook = orderBookEventArgs.OrderBook;

                var symbol = orderBookEventArgs.OrderBook.Symbol;

                if(!OrderBookSymbols.Contains(symbol))
                {
                    OrderBookSymbols.Add(symbol);
                }
            }
        }

        public void SubscribeOrderBookException(Exception exception)
        {
            OrderBookException = true;
        }

        public void SubscribeStatistics(StatisticsEventArgs statisticsEventArgs)
        {
            Statistics = statisticsEventArgs.Statistics;
        }

        public void SubscribeStatisticsException(Exception exception)
        {
            StatisticsException = true;
        }

        public Task<bool> TryUpdateStrategyAsync(string strategyParameters)
        {
            throw new NotImplementedException();
        }

        public Task<bool> TryStopStrategy(string strategyParameters)
        {
            throw new NotImplementedException();
        }

        public void SubscribeCandlesticks(AccountInfoEventArgs accountInfoEventArgs)
        {
            throw new NotImplementedException();
        }

        public void SubscribeCandlesticksException(Exception exception)
        {
            throw new NotImplementedException();
        }

        public void SubscribeCandlesticks(CandlestickEventArgs candlestickEventArgs)
        {
            throw new NotImplementedException();
        }

        public Task UpdateParametersAsync(string parameters)
        {
            throw new NotImplementedException();
        }

        public Task AddExchangeService(IEnumerable<StrategySubscription> strategySubscriptions, Exchange exchange, IExchangeService exchangeService)
        {
            throw new NotImplementedException();
        }
    }
}
