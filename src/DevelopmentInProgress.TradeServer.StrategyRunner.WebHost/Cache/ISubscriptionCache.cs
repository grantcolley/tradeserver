using DevelopmentInProgress.MarketView.Interface.Interfaces;
using DevelopmentInProgress.MarketView.Interface.Strategy;
using System;

namespace DevelopmentInProgress.TradeServer.StrategyRunner.WebHost.Cache
{
    public interface ISubscriptionCache : IDisposable
    {
        IExchangeService ExchangeService { get; }
        bool HasSubscriptions { get; }
        int Subscriptions(Subscribe subscribe);
        void Subscribe(string strategyName, StrategySubscription strategySubscription, ITradeStrategy tradeStrategy);
        void Unsubscribe(string strategyName, StrategySubscription strategySubscription, ITradeStrategy tradeStrategy);
    }
}