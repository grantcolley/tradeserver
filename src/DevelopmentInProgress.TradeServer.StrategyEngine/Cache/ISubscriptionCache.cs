using DevelopmentInProgress.MarketView.Interface.Interfaces;
using DevelopmentInProgress.MarketView.Interface.TradeStrategy;
using System;

namespace DevelopmentInProgress.TradeServer.StrategyEngine.Cache
{
    public interface ISubscriptionCache : IDisposable
    {
        IExchangeService ExchangeService { get; }
        bool HasSubscriptions { get; }
        int Subscriptions(Subscribe subscribe);
        void Subscribe(string strategyName, StrategySymbol strategySymbol, ITradeStrategy tradeStrategy);
        void Unsubscribe(string strategyName, StrategySymbol strategySymbol, ITradeStrategy tradeStrategy);
    }
}