using DevelopmentInProgress.MarketView.Interface.TradeStrategy;
using System;

namespace DevelopmentInProgress.TradeServer.StrategyEngine.Cache
{
    public interface ISymbolsCache : IDisposable
    {
        void Subscribe(Strategy strategy, ITradeStrategy tradeStrategy);
        void Unsubscribe(Strategy strategy, ITradeStrategy tradeStrategy);
    }
}
