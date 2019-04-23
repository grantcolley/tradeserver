using System;
using System.Threading;
using System.Threading.Tasks;
using DevelopmentInProgress.MarketView.Interface.Events;
using DevelopmentInProgress.MarketView.Interface.Strategy;

namespace DevelopmentInProgress.MarketView.StrategyRunner.Test.Helpers
{
    public class TestTradeExceptionStrategy : ITradeStrategy
    {
        public event EventHandler<TradeStrategyNotificationEventArgs> StrategyAccountInfoEvent;
        public event EventHandler<TradeStrategyNotificationEventArgs> StrategyNotificationEvent;
        public event EventHandler<TradeStrategyNotificationEventArgs> StrategyOrderBookEvent;
        public event EventHandler<TradeStrategyNotificationEventArgs> StrategyTradeEvent;

        public bool AggregateTradesException { get; set; }
        public bool OrderBookException { get; set; }

        public Task<Strategy> RunAsync(Strategy strategy, CancellationToken cancellationToken)
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

        public void SubscribeTrades(TradeEventArgs aggregateTradeEventArgs)
        {
            throw new Exception("SubscribeAggregateTrades");
        }

        public void SubscribeTradesException(Exception exception)
        {
            AggregateTradesException = true;            
        }

        public void SubscribeOrderBook(OrderBookEventArgs orderBookEventArgs)
        {
            throw new Exception("SubscribeOrderBook");
        }

        public void SubscribeOrderBookException(Exception exception)
        {
            OrderBookException = true;
            throw new Exception("SubscribeOrderBookException");
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
