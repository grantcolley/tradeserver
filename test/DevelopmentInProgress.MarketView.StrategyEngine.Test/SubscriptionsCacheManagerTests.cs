using DevelopmentInProgress.MarketView.Interface.TradeStrategy;
using DevelopmentInProgress.MarketView.StrategyEngine.Test.Helpers;
using DevelopmentInProgress.TradeServer.StrategyEngine.Cache;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace DevelopmentInProgress.MarketView.StrategyEngine.Test
{
    [TestClass]
    public class SubscriptionsCacheManagerTests
    {
        [TestMethod]
        public void Subscribe_SingleSymbolTest()
        {
            // Arrange
            var exchangeServiceFactory = new TestExchangeServiceFactory();
            var subscriptionsCacheFactory = new SubscriptionsCacheFactory(exchangeServiceFactory);
            var subscriptionsCacheManager = new SubscriptionsCacheManager(subscriptionsCacheFactory);

            var strategy = new Strategy { Name = "TestStrategy" };
            strategy..AddRange(new List<strategySubscription> { new strategySubscription { Exchange = Exchange.Binance, Symbol = "TRXBTC" } });

            var tradeStrategy = new TestTradeStrategy();

            // Act
            subscriptionsCacheManager.Subscribe(strategy, tradeStrategy);

            // Assert
            Assert.Fail();
        }
    }
}
