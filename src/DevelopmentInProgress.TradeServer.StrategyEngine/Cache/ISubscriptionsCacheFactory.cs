using DevelopmentInProgress.MarketView.Interface.TradeStrategy;

namespace DevelopmentInProgress.TradeServer.StrategyEngine.Cache
{
    public interface ISubscriptionsCacheFactory
    {
        ISubscriptionsCache GetSubscriptionsCache(Exchange exchange);
    }
}