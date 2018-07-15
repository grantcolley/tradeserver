using DevelopmentInProgress.MarketView.Interface.Events;
using DevelopmentInProgress.MarketView.Interface.Interfaces;
using DevelopmentInProgress.MarketView.Interface.TradeStrategy;
using System;

namespace DevelopmentInProgress.TradeServer.StrategyEngine.Cache.Binance
{
    public class Binance24HourStatisticsSubscriptionCache : ISubscriptionCache
    {
        private SubscribeStatistics subscribeStatistics;

        private bool disposed;

        public Binance24HourStatisticsSubscriptionCache(IExchangeService exchangeService)
        {
            ExchangeService = exchangeService;

            subscribeStatistics = new SubscribeStatistics(exchangeService);
        }

        public IExchangeService ExchangeService { get; set; }

        public bool HasSubscriptions
        {
            get
            {
                return subscribeStatistics.HasSubscriptions;
            }
        }

        public int Subscriptions(Subscribe subscribe)
        {
            return subscribeStatistics.Subscriptions;
        }

        public void Subscribe(string strategyName, StrategySubscription strategySubscription, ITradeStrategy tradeStrategy)
        {
            if (strategySubscription.Subscribe.HasFlag(MarketView.Interface.TradeStrategy.Subscribe.Statistics))
            {
                var statistics = new StrategyNotification<StatisticsEventArgs>
                {
                    Update = tradeStrategy.SubscribeStatistics,
                    Exception = tradeStrategy.SubscribeStatisticsException
                };

                subscribeStatistics.Subscribe(strategyName, statistics);
            }
        }

        public void Unsubscribe(string strategyName, StrategySubscription strategySubscription, ITradeStrategy tradeStrategy)
        {
            if (strategySubscription.Subscribe.HasFlag(MarketView.Interface.TradeStrategy.Subscribe.Statistics))
            {
                subscribeStatistics.Unsubscribe(strategyName, tradeStrategy.SubscribeStatisticsException);
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
                subscribeStatistics.Dispose();
            }

            disposed = true;
        }
    }
}