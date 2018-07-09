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

        public OrderBook OrderBook { get; set; }

        public Task<Strategy> RunAsync(Strategy strategy)
        {
            throw new NotImplementedException();
        }

        public void SubscribeAccountInfo(AccountInfoEventArgs accountInfoEventArgs)
        {
            throw new NotImplementedException();
        }

        public void SubscribeAccountInfoException(Exception exception)
        {
            throw new NotImplementedException();
        }

        public void SubscribeAggregateTrades(AggregateTradeEventArgs aggregateTradeEventArgs)
        {
            AggregateTrades = aggregateTradeEventArgs.AggregateTrades;
        }

        public void SubscribeAggregateTradesException(Exception exception)
        {
            throw new NotImplementedException();
        }

        public void SubscribeOrderBook(OrderBookEventArgs orderBookEventArgs)
        {
            OrderBook = orderBookEventArgs.OrderBook;
        }

        public void SubscribeOrderBookException(Exception exception)
        {
            throw new NotImplementedException();
        }

        public void SubscribeStatistics(StatisticsEventArgs statisticsEventArgs)
        {
            throw new NotImplementedException();
        }

        public void SubscribeStatisticsException(Exception exception)
        {
            throw new NotImplementedException();
        }
    }
}
