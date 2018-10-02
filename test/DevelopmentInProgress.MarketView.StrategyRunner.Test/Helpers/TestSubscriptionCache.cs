using DevelopmentInProgress.MarketView.Interface.Interfaces;
using DevelopmentInProgress.MarketView.Interface.Strategy;
using DevelopmentInProgress.TradeServer.StrategyRunner.WebHost.Cache;

namespace DevelopmentInProgress.MarketView.StrategyRunner.Test.Helpers
{
    public class TestSubscriptionCache : ISubscriptionCache
    {
        public IExchangeService ExchangeService => throw new System.NotImplementedException();

        public bool HasSubscriptions => throw new System.NotImplementedException();

        public void Dispose()
        {
        }

        public void Subscribe(string strategyName, StrategySubscription strategySubscription, ITradeStrategy tradeStrategy)
        {
            throw new System.NotImplementedException();
        }

        public int Subscriptions(Subscribe subscribe)
        {
            throw new System.NotImplementedException();
        }

        public void Unsubscribe(string strategyName, StrategySubscription strategySubscription, ITradeStrategy tradeStrategy)
        {
            throw new System.NotImplementedException();
        }
    }
}
