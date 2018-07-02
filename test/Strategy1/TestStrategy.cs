using DevelopmentInProgress.MarketView.Interface.TradeStrategy;
using System;
using System.Threading.Tasks;

namespace Strategy1
{
    public class TestStrategy : ITradeStrategy
    {
        public event EventHandler<TradeStrategyNotificationEventArgs> TradeStrategyNotificationEvent;

        public async Task<Strategy> RunAsync(Strategy strategy)
        {
            var testDependency = new DependencyLibrary.DependencyTest();
            var message = testDependency.GetMessage();
            OnTradeStrategyNotificationEvent(strategy, message);
            return strategy;
        }

        public void SubscribeAccountInfo(DevelopmentInProgress.MarketView.Interface.Events.AccountInfoEventArgs accountInfoEventArgs)
        {
            throw new NotImplementedException();
        }

        public void SubscribeAccountInfoException(Exception exception)
        {
            throw new NotImplementedException();
        }

        public void SubscribeAggregateTrades(DevelopmentInProgress.MarketView.Interface.Events.AggregateTradeEventArgs aggregateTradeEventArgs)
        {
            throw new NotImplementedException();
        }

        public void SubscribeAggregateTradesException(Exception exception)
        {
            throw new NotImplementedException();
        }

        public void SubscribeOrderBook(DevelopmentInProgress.MarketView.Interface.Events.OrderBookEventArgs orderBookEventArgs)
        {
            throw new NotImplementedException();
        }

        public void SubscribeOrderBookException(Exception exception)
        {
            throw new NotImplementedException();
        }

        public void SubscribeStatistics(DevelopmentInProgress.MarketView.Interface.Events.StatisticsEventArgs statisticsEventArgs)
        {
            throw new NotImplementedException();
        }

        public void SubscribeStatisticsException(Exception exception)
        {
            throw new NotImplementedException();
        }

        private void OnTradeStrategyNotificationEvent(Strategy strategy, string message)
        {
            var strategyNotification = strategy.GetNotification(NotificationLevel.Information, 100, message);

            var tradeStrategyNotificationEvent = TradeStrategyNotificationEvent;
            tradeStrategyNotificationEvent?.Invoke(this, new TradeStrategyNotificationEventArgs { StrategyNotification = strategyNotification });
        }
    }
}
