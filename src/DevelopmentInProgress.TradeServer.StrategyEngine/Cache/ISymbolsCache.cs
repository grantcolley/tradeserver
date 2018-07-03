using DevelopmentInProgress.MarketView.Interface.Interfaces;
using DevelopmentInProgress.MarketView.Interface.TradeStrategy;
using System;
using System.Collections.Generic;

namespace DevelopmentInProgress.TradeServer.StrategyEngine.Cache
{
    public interface ISymbolsCache : IDisposable
    {
        IExchangeService ExchangeService { get; }
        void Subscribe(string strategyName, List<StrategySymbol> strategySymbol, ITradeStrategy tradeStrategy);
        void Unsubscribe(string strategyName, List<StrategySymbol> strategySymbol, ITradeStrategy tradeStrategy);
    }
}