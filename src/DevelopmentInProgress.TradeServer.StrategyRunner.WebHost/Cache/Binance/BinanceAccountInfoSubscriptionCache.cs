using DevelopmentInProgress.MarketView.Interface.Events;
using DevelopmentInProgress.MarketView.Interface.Interfaces;
using DevelopmentInProgress.MarketView.Interface.Strategy;
using System;

namespace DevelopmentInProgress.TradeServer.StrategyRunner.WebHost.Cache.Binance
{
    public class BinanceAccountInfoSubscriptionCache : ISubscriptionCache
    {
        private SubscribeAccountInfo subscribeAccountInfo;

        private bool disposed;

        public BinanceAccountInfoSubscriptionCache(IExchangeService exchangeService)
        {
            ExchangeService = exchangeService;

            subscribeAccountInfo = new SubscribeAccountInfo(exchangeService);
        }

        public IExchangeService ExchangeService { get; set; }

        public bool HasSubscriptions
        {
            get
            {
                return subscribeAccountInfo.HasSubscriptions;
            }
        }

        public int Subscriptions(Subscribe subscribe)
        {
            return subscribeAccountInfo.Subscriptions;
        }

        public void Subscribe(string strategyName, StrategySubscription strategySubscription, ITradeStrategy tradeStrategy)
        {
            if (strategySubscription.Subscribe.HasFlag(MarketView.Interface.Strategy.Subscribe.AccountInfo))
            {
                var accountInfo = new StrategyNotification<AccountInfoEventArgs>
                {
                    Update = tradeStrategy.SubscribeAccountInfo,
                    Exception = tradeStrategy.SubscribeAccountInfoException
                };

                subscribeAccountInfo.User.ApiKey = strategySubscription.ApiKey;
                subscribeAccountInfo.User.ApiSecret = strategySubscription.ApiKey;
                subscribeAccountInfo.Subscribe(strategyName, accountInfo);
            }
        }

        public void Unsubscribe(string strategyName, StrategySubscription strategySubscription, ITradeStrategy tradeStrategy)
        {
            if (strategySubscription.Subscribe.HasFlag(MarketView.Interface.Strategy.Subscribe.AccountInfo))
            {
                subscribeAccountInfo.Unsubscribe(strategyName, tradeStrategy.SubscribeAccountInfoException);
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
                subscribeAccountInfo.Dispose();
            }

            disposed = true;
        }
    }
}