using DevelopmentInProgress.MarketView.Interface.Interfaces;
using DevelopmentInProgress.MarketView.Interface.TradeStrategy;
using System;

namespace DevelopmentInProgress.TradeServer.StrategyEngine.Cache
{
    public interface ISymbolCache : IDisposable
    {
        string Symbol { get; }
        bool HasSubscriptions { get; }
        IExchangeService ExchangeService { get; }
        void Subscribe(string strategyName, StrategySymbol strategySymbol, ITradeStrategy tradeStrategy);
        void Unsubscribe(string strategyName, StrategySymbol strategySymbol, ITradeStrategy tradeStrategy);
    }
}