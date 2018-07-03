using DevelopmentInProgress.MarketView.Interface.TradeStrategy;

namespace DevelopmentInProgress.TradeServer.StrategyEngine.Cache
{
    public interface ISymbolsCacheManager
    {
        void Subscribe(Strategy strategy, ITradeStrategy tradeStrategy);
        void Unsubscribe(Strategy strategy, ITradeStrategy tradeStrategy);
    }
}