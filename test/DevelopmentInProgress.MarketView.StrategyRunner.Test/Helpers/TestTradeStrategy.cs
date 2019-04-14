using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DevelopmentInProgress.MarketView.Interface.Events;
using DevelopmentInProgress.MarketView.Interface.Interfaces;
using DevelopmentInProgress.MarketView.Interface.Model;
using DevelopmentInProgress.MarketView.Interface.Strategy;

namespace DevelopmentInProgress.MarketView.StrategyRunner.Test.Helpers
{
    public class TestTradeStrategy : ITradeStrategy
    {
        public event EventHandler<TradeStrategyNotificationEventArgs> StrategyAccountInfoEvent;
        public event EventHandler<TradeStrategyNotificationEventArgs> StrategyNotificationEvent;
        public event EventHandler<TradeStrategyNotificationEventArgs> StrategyOrderBookEvent;
        public event EventHandler<TradeStrategyNotificationEventArgs> StrategyTradeEvent;

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

        private object tradeLock = new object();
        private object orderBookLock = new object();

        public Task<Strategy> RunAsync(Strategy strategy, CancellationToken cancellationToken)
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

        public void SubscribeTrades(AggregateTradeEventArgs aggregateTradeEventArgs)
        {
            lock (tradeLock)
            {
                AggregateTrades = aggregateTradeEventArgs.Trades;

                var symbol = aggregateTradeEventArgs.Trades.First().Symbol;

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
    }
}
