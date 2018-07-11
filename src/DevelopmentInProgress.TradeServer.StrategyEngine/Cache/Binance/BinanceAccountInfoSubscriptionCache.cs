using DevelopmentInProgress.MarketView.Interface.Events;
using DevelopmentInProgress.MarketView.Interface.Interfaces;
using DevelopmentInProgress.MarketView.Interface.TradeStrategy;
using System;

namespace DevelopmentInProgress.TradeServer.StrategyEngine.Cache.Binance
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

        public void Subscribe(string strategyName, StrategySymbol strategySymbol, ITradeStrategy tradeStrategy)
        {
            if (strategySymbol.Subscribe == MarketView.Interface.TradeStrategy.Subscribe.AccountInfo)
            {
                var accountInfo = new StrategyNotification<AccountInfoEventArgs>
                {
                    Update = tradeStrategy.SubscribeAccountInfo,
                    Exception = tradeStrategy.SubscribeAccountInfoException
                };

                subscribeAccountInfo.User.ApiKey = strategySymbol.ApiKey;
                subscribeAccountInfo.User.ApiSecret = strategySymbol.ApiKey;
                subscribeAccountInfo.Subscribe(strategyName, accountInfo);
            }
        }

        public void Unsubscribe(string strategyName, StrategySymbol strategySymbol, ITradeStrategy tradeStrategy)
        {
            if (strategySymbol.Subscribe == MarketView.Interface.TradeStrategy.Subscribe.AggregateTrades)
            {
                subscribeAccountInfo.Unsubscribe(strategyName, tradeStrategy.SubscribeAggregateTradesException);
            }
        }
    }
}