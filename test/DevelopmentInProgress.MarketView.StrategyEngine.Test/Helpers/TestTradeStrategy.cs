using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DevelopmentInProgress.MarketView.Interface.Events;
using DevelopmentInProgress.MarketView.Interface.Model;
using DevelopmentInProgress.MarketView.Interface.TradeStrategy;

namespace DevelopmentInProgress.MarketView.StrategyEngine.Test.Helpers
{
    public class TestTradeStrategy : ITradeStrategy
    {
        public event EventHandler<TradeStrategyNotificationEventArgs> TradeStrategyNotificationEvent;

        public IEnumerable<AggregateTrade> AggregateTrades { get; set; }
        public IEnumerable<SymbolStats> Statistics { get; set; }
        public OrderBook OrderBook { get; set; }
        public AccountInfo AccountInfo { get; set; }

        public bool AggregateTradesException { get; set; }
        public bool OrderBookException { get; set; }
        public bool StatisticsException { get; set; }
        public bool AccountInfoException { get; set; }

        public Task<Strategy> RunAsync(Strategy strategy)
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

        public void SubscribeAggregateTrades(AggregateTradeEventArgs aggregateTradeEventArgs)
        {
            AggregateTrades = aggregateTradeEventArgs.AggregateTrades;
        }

        public void SubscribeAggregateTradesException(Exception exception)
        {
            AggregateTradesException = true;
        }

        public void SubscribeOrderBook(OrderBookEventArgs orderBookEventArgs)
        {
            OrderBook = orderBookEventArgs.OrderBook;
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
