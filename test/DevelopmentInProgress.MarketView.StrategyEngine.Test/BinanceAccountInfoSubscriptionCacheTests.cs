using DevelopmentInProgress.MarketView.Interface.TradeStrategy;
using DevelopmentInProgress.MarketView.StrategyEngine.Test.Helpers;
using DevelopmentInProgress.TradeServer.StrategyEngine.Cache.Binance;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using System.Threading.Tasks;

namespace DevelopmentInProgress.MarketView.StrategyEngine.Test
{
    [TestClass]
    public class BinanceAccountInfoSubscriptionCacheTests
    {
        [TestMethod]
        public async Task Subscribe_AccountInfo_Single_Subscriber()
        {
            // Arrange
            var binanceExchangeService = new TestBinanceExchangeService();
            var strategySymbol = new StrategySymbol { Exchange = Exchange.Binance, Subscribe = Subscribe.AccountInfo };
            var tradeStrategy = new TestTradeStrategy();

            // Act
            using (var binanceAccountInfoCache = new BinanceAccountInfoSubscriptionCache(binanceExchangeService))
            {
                binanceAccountInfoCache.Subscribe("Test", strategySymbol, tradeStrategy);

                await Task.Delay(1000);

                // Assert
                Assert.IsTrue(binanceAccountInfoCache.HasSubscriptions);
                Assert.AreEqual(binanceAccountInfoCache.Subscriptions(Subscribe.AccountInfo), 1);
                Assert.IsNotNull(tradeStrategy.AccountInfo);
                Assert.IsTrue(tradeStrategy.AccountInfo.Balances.Any());
            }
        }

        [TestMethod]
        public async Task Subscribe_AccountInfo_Multiple_Subscribers()
        {
            // Arrange
            var binanceExchangeService = new TestBinanceExchangeService();
            var strategySymbol1 = new StrategySymbol { Exchange = Exchange.Binance, Subscribe = Subscribe.AccountInfo };
            var strategySymbol2 = new StrategySymbol { Exchange = Exchange.Binance, Subscribe = Subscribe.AccountInfo };
            var tradeStrategy1 = new TestTradeStrategy();
            var tradeStrategy2 = new TestTradeStrategy();

            using (var binanceAccountInfoCache = new BinanceAccountInfoSubscriptionCache(binanceExchangeService))
            {
                // Act
                binanceAccountInfoCache.Subscribe("Test 1", strategySymbol1, tradeStrategy1);

                binanceAccountInfoCache.Subscribe("Test 2", strategySymbol2, tradeStrategy2);

                await Task.Delay(2000);

                // Assert
                Assert.IsTrue(binanceAccountInfoCache.HasSubscriptions);
                Assert.AreEqual(binanceAccountInfoCache.Subscriptions(Subscribe.AccountInfo), 2);
                Assert.IsNotNull(tradeStrategy1.AccountInfo);
                Assert.IsTrue(tradeStrategy1.AccountInfo.Balances.Any());
                Assert.IsNotNull(tradeStrategy2.AccountInfo);
                Assert.IsTrue(tradeStrategy2.AccountInfo.Balances.Any());
            }
        }

        [TestMethod]
        public async Task Subscribe_AccountInfo_Single_Subscriber_Unsubscribe()
        {
            // Arrange
            var binanceExchangeService = new TestBinanceExchangeService();
            var strategySymbol = new StrategySymbol { Exchange = Exchange.Binance, Subscribe = Subscribe.AccountInfo };
            var tradeStrategy = new TestTradeStrategy();

            using (var binanceAccountInfoCache = new BinanceAccountInfoSubscriptionCache(binanceExchangeService))
            {
                // Act
                binanceAccountInfoCache.Subscribe("Test", strategySymbol, tradeStrategy);

                await Task.Delay(1000);

                binanceAccountInfoCache.Unsubscribe("Test", strategySymbol, tradeStrategy);

                await Task.Delay(1000);

                // Assert
                Assert.IsFalse(binanceAccountInfoCache.HasSubscriptions);
                Assert.AreEqual(binanceAccountInfoCache.Subscriptions(Subscribe.AccountInfo), 0);
                Assert.IsNotNull(tradeStrategy.AccountInfo);
                Assert.IsTrue(tradeStrategy.AccountInfo.Balances.Any());
            }
        }

        [TestMethod]
        public async Task Subscribe_AccountInfo_Multiple_Subscribers_Unsubscribe_Single_Subscriber()
        {
            // Arrange
            var binanceExchangeService = new TestBinanceExchangeService();
            var strategySymbol1 = new StrategySymbol { Exchange = Exchange.Binance, Subscribe = Subscribe.AccountInfo };
            var strategySymbol2 = new StrategySymbol { Exchange = Exchange.Binance, Subscribe = Subscribe.AccountInfo };
            var tradeStrategy1 = new TestTradeStrategy();
            var tradeStrategy2 = new TestTradeStrategy();

            using (var binanceAccountInfoCache = new BinanceAccountInfoSubscriptionCache(binanceExchangeService))
            {
                // Act
                binanceAccountInfoCache.Subscribe("Test 1", strategySymbol1, tradeStrategy1);

                binanceAccountInfoCache.Subscribe("Test 2", strategySymbol2, tradeStrategy2);

                await Task.Delay(2000);

                binanceAccountInfoCache.Unsubscribe("Test 2", strategySymbol2, tradeStrategy2);

                await Task.Delay(1000);

                // Assert
                Assert.IsTrue(binanceAccountInfoCache.HasSubscriptions);
                Assert.AreEqual(binanceAccountInfoCache.Subscriptions(Subscribe.AccountInfo), 1);
                Assert.IsNotNull(tradeStrategy1.AccountInfo);
                Assert.IsTrue(tradeStrategy1.AccountInfo.Balances.Any());
                Assert.IsNotNull(tradeStrategy2.AccountInfo);
                Assert.IsTrue(tradeStrategy2.AccountInfo.Balances.Any());
            }
        }

        [TestMethod]
        public async Task Subscribe_AccountInfo_Multiple_Subscribers_Unsubscribe_All_Subscribers()
        {
            // Arrange
            var binanceExchangeService = new TestBinanceExchangeService();
            var strategySymbol1 = new StrategySymbol { Exchange = Exchange.Binance, Subscribe = Subscribe.AccountInfo };
            var strategySymbol2 = new StrategySymbol { Exchange = Exchange.Binance, Subscribe = Subscribe.AccountInfo };
            var tradeStrategy1 = new TestTradeStrategy();
            var tradeStrategy2 = new TestTradeStrategy();

            using (var binanceAccountInfoCache = new BinanceAccountInfoSubscriptionCache(binanceExchangeService))
            {
                // Act
                binanceAccountInfoCache.Subscribe("Test 1", strategySymbol1, tradeStrategy1);

                binanceAccountInfoCache.Subscribe("Test 2", strategySymbol2, tradeStrategy2);

                await Task.Delay(2000);

                binanceAccountInfoCache.Unsubscribe("Test 2", strategySymbol2, tradeStrategy2);

                binanceAccountInfoCache.Unsubscribe("Test 1", strategySymbol1, tradeStrategy1);

                await Task.Delay(1000);

                // Assert
                Assert.IsFalse(binanceAccountInfoCache.HasSubscriptions);
                Assert.AreEqual(binanceAccountInfoCache.Subscriptions(Subscribe.AccountInfo), 0);
                Assert.IsNotNull(tradeStrategy1.AccountInfo);
                Assert.IsTrue(tradeStrategy1.AccountInfo.Balances.Any());
                Assert.IsNotNull(tradeStrategy2.AccountInfo);
                Assert.IsTrue(tradeStrategy2.AccountInfo.Balances.Any());
            }
        }

        [TestMethod]
        public async Task AccountInfo_Exception()
        {
            // Arrange
            var binanceExchangeService = new TestBinanceExchangeService { AccountInfoException = true };
            var strategySymbol = new StrategySymbol { Exchange = Exchange.Binance, Subscribe = Subscribe.AccountInfo };
            var tradeStrategy = new TestTradeStrategy();

            // Act
            using (var binanceAccountInfoCache = new BinanceAccountInfoSubscriptionCache(binanceExchangeService))
            {
                binanceAccountInfoCache.Subscribe("Test", strategySymbol, tradeStrategy);

                await Task.Delay(1000);

                // Assert
                Assert.IsTrue(binanceAccountInfoCache.HasSubscriptions);
                Assert.AreEqual(binanceAccountInfoCache.Subscriptions(Subscribe.AccountInfo), 1);
                Assert.IsNotNull(tradeStrategy.AccountInfo);
                Assert.IsTrue(tradeStrategy.AccountInfo.Balances.Any());
                Assert.IsTrue(tradeStrategy.AccountInfoException);
            }
        }
    }
}
