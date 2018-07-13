using DevelopmentInProgress.MarketView.Interface.TradeStrategy;
using DevelopmentInProgress.MarketView.StrategyEngine.Test.Helpers;
using DevelopmentInProgress.TradeServer.StrategyEngine.Cache.Binance;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using System.Threading.Tasks;

namespace DevelopmentInProgress.MarketView.StrategyEngine.Test
{
    [TestClass]
    public class Binance24HourStatisticsSubscriptionCacheTests
    {
        [TestMethod]
        public async Task Subscribe_Statistics_Single_Subscriber()
        {
            // Arrange
            var binanceExchangeService = new TestBinanceExchangeService();
            var strategySubscription = new StrategySubscription { Exchange = Exchange.Binance, Subscribe = Subscribe.Statistics };
            var tradeStrategy = new TestTradeStrategy();

            // Act
            using (var binanceStatsCache = new Binance24HourStatisticsSubscriptionCache(binanceExchangeService))
            {
                binanceStatsCache.Subscribe("Test", strategySubscription, tradeStrategy);

                await Task.Delay(1000);

                // Assert
                Assert.IsTrue(binanceStatsCache.HasSubscriptions);
                Assert.AreEqual(binanceStatsCache.Subscriptions(Subscribe.Statistics), 1);
                Assert.IsNotNull(tradeStrategy.Statistics);
                Assert.IsTrue(tradeStrategy.Statistics.Any());
            }
        }

        [TestMethod]
        public async Task Subscribe_Statistics_Multiple_Subscribers()
        {
            // Arrange
            var binanceExchangeService = new TestBinanceExchangeService();
            var strategySubscription1 = new StrategySubscription { Exchange = Exchange.Binance, Subscribe = Subscribe.Statistics };
            var strategySubscription2 = new StrategySubscription { Exchange = Exchange.Binance, Subscribe = Subscribe.Statistics };
            var tradeStrategy1 = new TestTradeStrategy();
            var tradeStrategy2 = new TestTradeStrategy();

            using (var binanceStatsCache = new Binance24HourStatisticsSubscriptionCache(binanceExchangeService))
            {
                // Act
                binanceStatsCache.Subscribe("Test 1", strategySubscription1, tradeStrategy1);

                binanceStatsCache.Subscribe("Test 2", strategySubscription2, tradeStrategy2);

                await Task.Delay(2000);

                // Assert
                Assert.IsTrue(binanceStatsCache.HasSubscriptions);
                Assert.AreEqual(binanceStatsCache.Subscriptions(Subscribe.Statistics), 2);
                Assert.IsNotNull(tradeStrategy1.Statistics);
                Assert.IsTrue(tradeStrategy1.Statistics.Any());
                Assert.IsNotNull(tradeStrategy2.Statistics);
                Assert.IsTrue(tradeStrategy2.Statistics.Any());
            }
        }

        [TestMethod]
        public async Task Subscribe_Statistics_Single_Subscriber_Unsubscribe()
        {
            // Arrange
            var binanceExchangeService = new TestBinanceExchangeService();
            var strategySubscription = new StrategySubscription { Exchange = Exchange.Binance, Subscribe = Subscribe.Statistics };
            var tradeStrategy = new TestTradeStrategy();

            using (var binanceStatsCache = new Binance24HourStatisticsSubscriptionCache(binanceExchangeService))
            {
                // Act
                binanceStatsCache.Subscribe("Test", strategySubscription, tradeStrategy);

                await Task.Delay(1000);

                binanceStatsCache.Unsubscribe("Test", strategySubscription, tradeStrategy);

                await Task.Delay(1000);

                // Assert
                Assert.IsFalse(binanceStatsCache.HasSubscriptions);
                Assert.AreEqual(binanceStatsCache.Subscriptions(Subscribe.Statistics), 0);
                Assert.IsNotNull(tradeStrategy.Statistics);
                Assert.IsTrue(tradeStrategy.Statistics.Any());
            }
        }

        [TestMethod]
        public async Task Subscribe_Statistics_Multiple_Subscribers_Unsubscribe_Single_Subscriber()
        {
            // Arrange
            var binanceExchangeService = new TestBinanceExchangeService();
            var strategySubscription1 = new StrategySubscription { Exchange = Exchange.Binance, Subscribe = Subscribe.Statistics };
            var strategySubscription2 = new StrategySubscription { Exchange = Exchange.Binance, Subscribe = Subscribe.Statistics };
            var tradeStrategy1 = new TestTradeStrategy();
            var tradeStrategy2 = new TestTradeStrategy();

            using (var binanceStatsCache = new Binance24HourStatisticsSubscriptionCache(binanceExchangeService))
            {
                // Act
                binanceStatsCache.Subscribe("Test 1", strategySubscription1, tradeStrategy1);

                binanceStatsCache.Subscribe("Test 2", strategySubscription2, tradeStrategy2);

                await Task.Delay(2000);

                binanceStatsCache.Unsubscribe("Test 2", strategySubscription2, tradeStrategy2);

                await Task.Delay(1000);

                // Assert
                Assert.IsTrue(binanceStatsCache.HasSubscriptions);
                Assert.AreEqual(binanceStatsCache.Subscriptions(Subscribe.Statistics), 1);
                Assert.IsNotNull(tradeStrategy1.Statistics);
                Assert.IsTrue(tradeStrategy1.Statistics.Any());
                Assert.IsNotNull(tradeStrategy2.Statistics);
                Assert.IsTrue(tradeStrategy2.Statistics.Any());
            }
        }

        [TestMethod]
        public async Task Subscribe_Statistics_Multiple_Subscribers_Unsubscribe_All_Subscribers()
        {
            // Arrange
            var binanceExchangeService = new TestBinanceExchangeService();
            var strategySubscription1 = new StrategySubscription { Exchange = Exchange.Binance, Subscribe = Subscribe.Statistics };
            var strategySubscription2 = new StrategySubscription { Exchange = Exchange.Binance, Subscribe = Subscribe.Statistics };
            var tradeStrategy1 = new TestTradeStrategy();
            var tradeStrategy2 = new TestTradeStrategy();

            using (var binanceStatsCache = new Binance24HourStatisticsSubscriptionCache(binanceExchangeService))
            {
                // Act
                binanceStatsCache.Subscribe("Test 1", strategySubscription1, tradeStrategy1);

                binanceStatsCache.Subscribe("Test 2", strategySubscription2, tradeStrategy2);

                await Task.Delay(2000);

                binanceStatsCache.Unsubscribe("Test 2", strategySubscription2, tradeStrategy2);

                binanceStatsCache.Unsubscribe("Test 1", strategySubscription1, tradeStrategy1);

                await Task.Delay(1000);

                // Assert
                Assert.IsFalse(binanceStatsCache.HasSubscriptions);
                Assert.AreEqual(binanceStatsCache.Subscriptions(Subscribe.Statistics), 0);
                Assert.IsNotNull(tradeStrategy1.Statistics);
                Assert.IsTrue(tradeStrategy1.Statistics.Any());
                Assert.IsNotNull(tradeStrategy2.Statistics);
                Assert.IsTrue(tradeStrategy2.Statistics.Any());
            }
        }

        [TestMethod]
        public async Task Statistics_Exception()
        {
            // Arrange
            var binanceExchangeService = new TestBinanceExchangeService { StatisticsException = true };
            var strategySubscription = new StrategySubscription { Exchange = Exchange.Binance, Subscribe = Subscribe.Statistics };
            var tradeStrategy = new TestTradeStrategy();

            // Act
            using (var binanceStatsCache = new Binance24HourStatisticsSubscriptionCache(binanceExchangeService))
            {
                binanceStatsCache.Subscribe("Test", strategySubscription, tradeStrategy);

                await Task.Delay(1000);

                // Assert
                Assert.IsTrue(binanceStatsCache.HasSubscriptions);
                Assert.AreEqual(binanceStatsCache.Subscriptions(Subscribe.Statistics), 1);
                Assert.IsNotNull(tradeStrategy.Statistics);
                Assert.IsTrue(tradeStrategy.Statistics.Any());
                Assert.IsTrue(tradeStrategy.StatisticsException);
            }
        }
    }
}
