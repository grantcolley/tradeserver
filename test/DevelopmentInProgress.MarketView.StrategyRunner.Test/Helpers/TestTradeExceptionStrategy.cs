using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DevelopmentInProgress.TradeView.Interface.Enums;
using DevelopmentInProgress.TradeView.Interface.Events;
using DevelopmentInProgress.TradeView.Interface.Interfaces;
using DevelopmentInProgress.TradeView.Interface.Strategy;

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
        public Strategy Strategy { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public void SetStrategy(Strategy strategy)
        {

        }

        public Task<Strategy> RunAsync(CancellationToken cancellationToken)
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
