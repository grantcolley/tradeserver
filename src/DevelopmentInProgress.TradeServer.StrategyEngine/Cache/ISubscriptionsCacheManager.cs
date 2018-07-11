using DevelopmentInProgress.MarketView.Interface.TradeStrategy;

namespace DevelopmentInProgress.TradeServer.StrategyEngine.Cache
{
    public interface ISubscriptionsCacheManager
    {
        ISubscriptionsCacheFactory SubscriptionsCacheFactory { get; }
        void Subscribe(Strategy strategy, ITradeStrategy tradeStrategy);
        void Unsubscribe(Strategy strategy, ITradeStrategy tradeStrategy);
    }
}