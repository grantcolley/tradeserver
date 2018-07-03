using DevelopmentInProgress.MarketView.Interface.TradeStrategy;

namespace DevelopmentInProgress.TradeServer.StrategyEngine.Cache
{
    public interface ISymbolsCacheFactory
    {
        ISymbolsCache GetSymbolsCache(Exchange exchange);
    }
}