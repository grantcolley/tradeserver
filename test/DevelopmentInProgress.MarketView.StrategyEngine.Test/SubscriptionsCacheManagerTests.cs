using DevelopmentInProgress.MarketView.Interface.TradeStrategy;
using DevelopmentInProgress.MarketView.StrategyEngine.Test.Helpers;
using DevelopmentInProgress.TradeServer.StrategyEngine.Cache;
using DevelopmentInProgress.TradeServer.StrategyEngine.Cache.Binance;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;

namespace DevelopmentInProgress.MarketView.StrategyEngine.Test
{
    [TestClass]
    public class SubscriptionsCacheManagerTests
    {
        [TestMethod]
        public void Subscribe_SingleSymbol_MultipleSubscriptions_Subscribe()
        {
            // Arrange
            var exchangeServiceFactory = new TestExchangeServiceFactory();
            var subscriptionsCacheFactory = new TestSubscriptionsCacheFactory(exchangeServiceFactory);

            var strategy = new Strategy { Name = "Test" };

            var trxBinance = new StrategySubscription
            {
                Exchange = Exchange.Binance,
                Symbol = "TRXBTC-BINANCE",
                Subscribe = (Subscribe.AggregateTrades)
            };

            var trxTest = new StrategySubscription
            {
                Exchange = Exchange.Test,
                Symbol = "TRXBTC-TEST",
                Subscribe = (Subscribe.AggregateTrades)
            };

            strategy.StrategySubscriptions.AddRange(new[] { trxBinance, trxTest });
            
            var tradeStrategy = new TestTradeStrategy();

            using (var subscriptionsCacheManager = new SubscriptionsCacheManager(subscriptionsCacheFactory))
            {
                // Act
                subscriptionsCacheManager.Subscribe(strategy, tradeStrategy);

                // Assert
                var binance = subscriptionsCacheManager.SubscriptionsCacheFactory.GetSubscriptionsCache(Exchange.Binance);
                
                Assert.IsTrue(binance.HasSubscriptions);

                if (binance.Caches.TryGetValue("TRXBTC-BINANCE", out ISubscriptionCache trxBinanceCache))
                {
                    Assert.IsNotNull(trxBinanceCache);
                    Assert.IsInstanceOfType(trxBinanceCache, typeof(BinanceSymbolSubscriptionCache));
                    Assert.IsTrue(trxBinanceCache.HasSubscriptions);
                    Assert.AreEqual(trxBinanceCache.Subscriptions(Subscribe.AggregateTrades), 1);
                }
                else
                {
                    Assert.Fail();
                }

                var test = subscriptionsCacheManager.SubscriptionsCacheFactory.GetSubscriptionsCache(Exchange.Test);

                Assert.IsTrue(test.HasSubscriptions);

                if (test.Caches.TryGetValue("TRXBTC-TEST", out ISubscriptionCache trxTestCache))
                {
                    Assert.IsNotNull(trxTestCache);
                    Assert.IsInstanceOfType(trxTestCache, typeof(TestSubscriptionCache));
                }
                else
                {
                    Assert.Fail();
                }
            }
        }

        [TestMethod]
        public async Task Subscribe_SingleSymbol_MultipleSubscriptions_Unsubscribe()
        {
            // Arrange
            var exchangeServiceFactory = new TestExchangeServiceFactory();
            var subscriptionsCacheFactory = new TestSubscriptionsCacheFactory(exchangeServiceFactory);

            var strategy = new Strategy { Name = "Test" };

            var trxBinance = new StrategySubscription
            {
                Exchange = Exchange.Binance,
                Symbol = "TRXBTC-BINANCE",
                Subscribe = (Subscribe.AggregateTrades)
            };

            var trxTest = new StrategySubscription
            {
                Exchange = Exchange.Test,
                Symbol = "TRXBTC-TEST",
                Subscribe = (Subscribe.AggregateTrades)
            };

            strategy.StrategySubscriptions.AddRange(new[] { trxBinance, trxTest });

            var tradeStrategy = new TestTradeStrategy();

            using (var subscriptionsCacheManager = new SubscriptionsCacheManager(subscriptionsCacheFactory))
            {
                // Act
                subscriptionsCacheManager.Subscribe(strategy, tradeStrategy);

                await Task.Delay(1000);

                subscriptionsCacheManager.Unsubscribe(strategy, tradeStrategy);

                // Assert
                var binance = subscriptionsCacheManager.SubscriptionsCacheFactory.GetSubscriptionsCache(Exchange.Binance);

                Assert.IsFalse(binance.HasSubscriptions);

                var test = subscriptionsCacheManager.SubscriptionsCacheFactory.GetSubscriptionsCache(Exchange.Test);

                Assert.IsFalse(test.HasSubscriptions);
            }
        }
    }
}
