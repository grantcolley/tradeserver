//using DevelopmentInProgress.TradeView.Interface.Events;
//using DevelopmentInProgress.TradeView.Interface.Interfaces;
//using DevelopmentInProgress.TradeView.Interface.Strategy;
//using System;

//namespace DevelopmentInProgress.TradeServer.StrategyRunner.WebHost.Cache.Binance
//{
//    public class Binance24HourStatisticsSubscriptionCache : ISubscriptionCache
//    {
//        private SubscribeStatistics subscribeStatistics;

//        private bool disposed;

//        public Binance24HourStatisticsSubscriptionCache(IExchangeApi exchangeService)
//        {
//            ExchangeApi = exchangeService;

//            subscribeStatistics = new SubscribeStatistics(exchangeService);
//        }

//        public IExchangeApi ExchangeApi { get; set; }

//        public bool HasSubscriptions
//        {
//            get
//            {
//                return subscribeStatistics.HasSubscriptions;
//            }
//        }

//        public int Subscriptions(Subscribe subscribe)
//        {
//            return subscribeStatistics.Subscriptions;
//        }

//        public void Subscribe(string strategyName, StrategySubscription strategySubscription, ITradeStrategy tradeStrategy)
//        {
//            if (strategySubscription.Subscribe.HasFlag(TradeView.Interface.Strategy.Subscribe.Statistics))
//            {
//                var statistics = new StrategyNotification<StatisticsEventArgs>
//                {
//                    Update = tradeStrategy.SubscribeStatistics,
//                    Exception = tradeStrategy.SubscribeStatisticsException
//                };

//                subscribeStatistics.Subscribe(strategyName, statistics);
//            }
//        }

//        public void Unsubscribe(string strategyName, StrategySubscription strategySubscription, ITradeStrategy tradeStrategy)
//        {
//            if (strategySubscription.Subscribe.HasFlag(TradeView.Interface.Strategy.Subscribe.Statistics))
//            {
//                subscribeStatistics.Unsubscribe(strategyName, tradeStrategy.SubscribeStatisticsException);
//            }
//        }

//        public void Dispose()
//        {
//            Dispose(true);
//            GC.SuppressFinalize(this);
//        }

//        private void Dispose(bool disposing)
//        {
//            if (disposed)
//            {
//                return;
//            }

//            if (disposing)
//            {
//                subscribeStatistics.Dispose();
//            }

//            disposed = true;
//        }
//    }
//}