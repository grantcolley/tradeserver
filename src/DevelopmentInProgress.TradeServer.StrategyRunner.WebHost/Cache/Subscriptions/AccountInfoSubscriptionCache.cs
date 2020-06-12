using DevelopmentInProgress.TradeView.Core.Events;
using DevelopmentInProgress.TradeView.Core.Interfaces;
using DevelopmentInProgress.TradeView.Core.Strategy;
using System;

namespace DevelopmentInProgress.TradeServer.StrategyRunner.WebHost.Cache.Subscriptions
{
    public class AccountInfoSubscriptionCache : ISubscriptionCache
    {
        private SubscribeAccountInfo subscribeAccountInfo;

        private bool disposed;

        public AccountInfoSubscriptionCache(IExchangeApi exchangeService)
        {
            ExchangeApi = exchangeService;

            subscribeAccountInfo = new SubscribeAccountInfo(exchangeService);
        }

        public IExchangeApi ExchangeApi { get; set; }

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
            if (strategySubscription.Subscribe.HasFlag(TradeView.Core.Strategy.Subscribe.AccountInfo))
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
            if (strategySubscription.Subscribe.HasFlag(TradeView.Core.Strategy.Subscribe.AccountInfo))
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
