using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DevelopmentInProgress.MarketView.Interface.Events;
using DevelopmentInProgress.MarketView.Interface.Interfaces;
using DevelopmentInProgress.MarketView.Interface.Strategy;

namespace DevelopmentInProgress.MarketView.StrategyRunner.Test.Helpers
{
    public class TestTradeExceptionStrategy : ITradeStrategy
    {
        public event EventHandler<StrategyNotificationEventArgs> StrategyAccountInfoEvent;
        public event EventHandler<StrategyNotificationEventArgs> StrategyCustomNotificationEvent;
        public event EventHandler<StrategyNotificationEventArgs> StrategyNotificationEvent;
        public event EventHandler<StrategyNotificationEventArgs> StrategyOrderBookEvent;
        public event EventHandler<StrategyNotificationEventArgs> StrategyTradeEvent;
        public event EventHandler<StrategyNotificationEventArgs> StrategyCandlesticksEvent;
        public event EventHandler<StrategyNotificationEventArgs> StrategyStatisticsEvent;

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

        public Task AddExchangeService(IEnumerable<StrategySubscription> strategySubscriptions, Exchange exchange, IExchangeService exchangeService)
        {
            throw new NotImplementedException();
        }

        public Task<bool> TryUpdateStrategy(string strategyParameters)
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
    }
}
