using DevelopmentInProgress.MarketView.Interface.TradeStrategy;
using System;

namespace DevelopmentInProgress.TradeServer.StrategyEngine.Cache
{
    public interface ISymbolCache : IDisposable
    {
        string Symbol { get; }
        bool HasSubscriptions { get; }
        void Subscribe(ITradeStrategy tradeStrategy);
        void Unsubscribe(ITradeStrategy tradeStrategy);
    }
}