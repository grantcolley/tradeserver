using DevelopmentInProgress.MarketView.Interface.TradeStrategy;
using DevelopmentInProgress.MarketView.StrategyEngine.Test.Helpers;
using DevelopmentInProgress.TradeServer.StrategyEngine.Cache.Binance;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using System.Threading.Tasks;

namespace DevelopmentInProgress.MarketView.StrategyEngine.Test
{
    [TestClass]
    public class BinanceSymbolSubscriptionCacheTests
    {
        [TestMethod]
        public async Task Subscribe_AggregateTrades_Single_Subscriber()
        {
            // Arrange
            var binanceExchangeService = new TestBinanceExchangeService();
            var strategySymbol = new StrategySymbol { Exchange = Exchange.Binance, Symbol = "TRXBTC", Subscribe = Subscribe.AggregateTrades };
            var tradeStrategy = new TestTradeStrategy();

            // Act
            using (var binanceSymbolCache = new BinanceSymbolSubscriptionCache("TRXBTC", 500, binanceExchangeService))
            {
                binanceSymbolCache.Subscribe("Test", strategySymbol, tradeStrategy);

                await Task.Delay(1000);

                // Assert
                Assert.IsTrue(binanceSymbolCache.HasSubscriptions);
                Assert.AreEqual(binanceSymbolCache.Subscriptions(Subscribe.AggregateTrades), 1);
                Assert.IsNotNull(tradeStrategy.AggregateTrades);
                Assert.IsTrue(tradeStrategy.AggregateTrades.Any());
            }
        }

        [TestMethod]
        public async Task Subscribe_AggregateTrades_Multiple_Subscribers()
        {
            // Arrange
            var binanceExchangeService = new TestBinanceExchangeService();
            var strategySymbol1 = new StrategySymbol { Exchange = Exchange.Binance, Symbol = "TRXBTC", Subscribe = Subscribe.AggregateTrades };
            var strategySymbol2 = new StrategySymbol { Exchange = Exchange.Binance, Symbol = "TRXBTC", Subscribe = Subscribe.AggregateTrades };
            var tradeStrategy1 = new TestTradeStrategy();
            var tradeStrategy2 = new TestTradeStrategy();

            using (var binanceSymbolCache = new BinanceSymbolSubscriptionCache("TRXBTC", 500, binanceExchangeService))
            {
                // Act
                binanceSymbolCache.Subscribe("Test 1", strategySymbol1, tradeStrategy1);

                binanceSymbolCache.Subscribe("Test 2", strategySymbol2, tradeStrategy2);

                await Task.Delay(2000);

                // Assert
                Assert.IsTrue(binanceSymbolCache.HasSubscriptions);
                Assert.AreEqual(binanceSymbolCache.Subscriptions(Subscribe.AggregateTrades), 2);
                Assert.IsNotNull(tradeStrategy1.AggregateTrades);
                Assert.IsTrue(tradeStrategy1.AggregateTrades.Any());
                Assert.IsNotNull(tradeStrategy2.AggregateTrades);
                Assert.IsTrue(tradeStrategy2.AggregateTrades.Any());
            }
        }

        [TestMethod]
        public async Task Subscribe_AggregateTrades_Single_Subscriber_Unsubscribe()
        {
            // Arrange
            var binanceExchangeService = new TestBinanceExchangeService();
            var strategySymbol = new StrategySymbol { Exchange = Exchange.Binance, Symbol = "TRXBTC", Subscribe = Subscribe.AggregateTrades };
            var tradeStrategy = new TestTradeStrategy();

            using (var binanceSymbolCache = new BinanceSymbolSubscriptionCache("TRXBTC", 500, binanceExchangeService))
            {
                // Act
                binanceSymbolCache.Subscribe("Test", strategySymbol, tradeStrategy);

                await Task.Delay(1000);

                binanceSymbolCache.Unsubscribe("Test", strategySymbol, tradeStrategy);

                await Task.Delay(1000);

                // Assert
                Assert.IsFalse(binanceSymbolCache.HasSubscriptions);
                Assert.AreEqual(binanceSymbolCache.Subscriptions(Subscribe.AggregateTrades), 0);
                Assert.IsNotNull(tradeStrategy.AggregateTrades);
                Assert.IsTrue(tradeStrategy.AggregateTrades.Any());
            }
        }

        [TestMethod]
        public async Task Subscribe_AggregateTrades_Multiple_Subscribers_Unsubscribe_Single_Subscriber()
        {
            // Arrange
            var binanceExchangeService = new TestBinanceExchangeService();
            var strategySymbol1 = new StrategySymbol { Exchange = Exchange.Binance, Symbol = "TRXBTC", Subscribe = Subscribe.AggregateTrades };
            var strategySymbol2 = new StrategySymbol { Exchange = Exchange.Binance, Symbol = "TRXBTC", Subscribe = Subscribe.AggregateTrades };
            var tradeStrategy1 = new TestTradeStrategy();
            var tradeStrategy2 = new TestTradeStrategy();

            using (var binanceSymbolCache = new BinanceSymbolSubscriptionCache("TRXBTC", 500, binanceExchangeService))
            {
                // Act
                binanceSymbolCache.Subscribe("Test 1", strategySymbol1, tradeStrategy1);

                binanceSymbolCache.Subscribe("Test 2", strategySymbol2, tradeStrategy2);

                await Task.Delay(2000);

                binanceSymbolCache.Unsubscribe("Test 2", strategySymbol2, tradeStrategy2);

                await Task.Delay(1000);

                // Assert
                Assert.IsTrue(binanceSymbolCache.HasSubscriptions);
                Assert.AreEqual(binanceSymbolCache.Subscriptions(Subscribe.AggregateTrades), 1);
                Assert.IsNotNull(tradeStrategy1.AggregateTrades);
                Assert.IsTrue(tradeStrategy1.AggregateTrades.Any());
                Assert.IsNotNull(tradeStrategy2.AggregateTrades);
                Assert.IsTrue(tradeStrategy2.AggregateTrades.Any());
            }
        }

        [TestMethod]
        public async Task Subscribe_AggregateTrades_Multiple_Subscribers_Unsubscribe_All_Subscribers()
        {
            // Arrange
            var binanceExchangeService = new TestBinanceExchangeService();
            var strategySymbol1 = new StrategySymbol { Exchange = Exchange.Binance, Symbol = "TRXBTC", Subscribe = Subscribe.AggregateTrades };
            var strategySymbol2 = new StrategySymbol { Exchange = Exchange.Binance, Symbol = "TRXBTC", Subscribe = Subscribe.AggregateTrades };
            var tradeStrategy1 = new TestTradeStrategy();
            var tradeStrategy2 = new TestTradeStrategy();

            using (var binanceSymbolCache = new BinanceSymbolSubscriptionCache("TRXBTC", 500, binanceExchangeService))
            {
                // Act
                binanceSymbolCache.Subscribe("Test 1", strategySymbol1, tradeStrategy1);

                binanceSymbolCache.Subscribe("Test 2", strategySymbol2, tradeStrategy2);

                await Task.Delay(2000);

                binanceSymbolCache.Unsubscribe("Test 2", strategySymbol2, tradeStrategy2);

                binanceSymbolCache.Unsubscribe("Test 1", strategySymbol1, tradeStrategy1);

                await Task.Delay(1000);

                // Assert
                Assert.IsFalse(binanceSymbolCache.HasSubscriptions);
                Assert.AreEqual(binanceSymbolCache.Subscriptions(Subscribe.AggregateTrades), 0);
                Assert.IsNotNull(tradeStrategy1.AggregateTrades);
                Assert.IsTrue(tradeStrategy1.AggregateTrades.Any());
                Assert.IsNotNull(tradeStrategy2.AggregateTrades);
                Assert.IsTrue(tradeStrategy2.AggregateTrades.Any());
            }
        }

        [TestMethod]
        public async Task AggregateTrades_Exception()
        {
            // Arrange
            var binanceExchangeService = new TestBinanceExchangeService { AggregateTradesException = true };
            var strategySymbol = new StrategySymbol { Exchange = Exchange.Binance, Symbol = "TRXBTC", Subscribe = Subscribe.AggregateTrades };
            var tradeStrategy = new TestTradeStrategy();

            // Act
            using (var binanceSymbolCache = new BinanceSymbolSubscriptionCache("TRXBTC", 500, binanceExchangeService))
            {
                binanceSymbolCache.Subscribe("Test", strategySymbol, tradeStrategy);

                await Task.Delay(1000);

                // Assert
                Assert.IsTrue(binanceSymbolCache.HasSubscriptions);
                Assert.AreEqual(binanceSymbolCache.Subscriptions(Subscribe.AggregateTrades), 1);
                Assert.IsNotNull(tradeStrategy.AggregateTrades);
                Assert.IsTrue(tradeStrategy.AggregateTrades.Any());
                Assert.IsTrue(tradeStrategy.AggregateTradesException);
            }
        }

        [TestMethod]
        public async Task Subscribe_OrderBook_Single_Subscriber()
        {
            // Arrange
            var binanceExchangeService = new TestBinanceExchangeService();
            var strategySymbol = new StrategySymbol { Exchange = Exchange.Binance, Symbol = "TRXBTC", Subscribe = Subscribe.OrderBook };
            var tradeStrategy = new TestTradeStrategy();

            // Act
            using (var binanceSymbolCache = new BinanceSymbolSubscriptionCache("TRXBTC", 500, binanceExchangeService))
            {
                binanceSymbolCache.Subscribe("Test", strategySymbol, tradeStrategy);

                await Task.Delay(1000);

                // Assert
                Assert.IsTrue(binanceSymbolCache.HasSubscriptions);
                Assert.AreEqual(binanceSymbolCache.Subscriptions(Subscribe.OrderBook), 1);
                Assert.IsNotNull(tradeStrategy.OrderBook);
                Assert.IsTrue(tradeStrategy.OrderBook.Asks.Any());
            }
        }

        [TestMethod]
        public async Task Subscribe_OrderBook_Multiple_Subscribers()
        {
            // Arrange
            var binanceExchangeService = new TestBinanceExchangeService();
            var strategySymbol1 = new StrategySymbol { Exchange = Exchange.Binance, Symbol = "TRXBTC", Subscribe = Subscribe.OrderBook };
            var strategySymbol2 = new StrategySymbol { Exchange = Exchange.Binance, Symbol = "TRXBTC", Subscribe = Subscribe.OrderBook };
            var tradeStrategy1 = new TestTradeStrategy();
            var tradeStrategy2 = new TestTradeStrategy();

            using (var binanceSymbolCache = new BinanceSymbolSubscriptionCache("TRXBTC", 500, binanceExchangeService))
            {
                // Act
                binanceSymbolCache.Subscribe("Test 1", strategySymbol1, tradeStrategy1);

                binanceSymbolCache.Subscribe("Test 2", strategySymbol2, tradeStrategy2);

                await Task.Delay(2000);

                // Assert
                Assert.IsTrue(binanceSymbolCache.HasSubscriptions);
                Assert.AreEqual(binanceSymbolCache.Subscriptions(Subscribe.OrderBook), 2);
                Assert.IsNotNull(tradeStrategy1.OrderBook);
                Assert.IsTrue(tradeStrategy1.OrderBook.Asks.Any());
                Assert.IsNotNull(tradeStrategy2.OrderBook);
                Assert.IsTrue(tradeStrategy2.OrderBook.Asks.Any());
            }
        }

        [TestMethod]
        public async Task Subscribe_OrderBook_Single_Subscriber_Unsubscribe()
        {
            // Arrange
            var binanceExchangeService = new TestBinanceExchangeService();
            var strategySymbol = new StrategySymbol { Exchange = Exchange.Binance, Symbol = "TRXBTC", Subscribe = Subscribe.OrderBook };
            var tradeStrategy = new TestTradeStrategy();

            using (var binanceSymbolCache = new BinanceSymbolSubscriptionCache("TRXBTC", 500, binanceExchangeService))
            {
                // Act
                binanceSymbolCache.Subscribe("Test", strategySymbol, tradeStrategy);

                await Task.Delay(1000);

                binanceSymbolCache.Unsubscribe("Test", strategySymbol, tradeStrategy);

                await Task.Delay(1000);

                // Assert
                Assert.IsFalse(binanceSymbolCache.HasSubscriptions);
                Assert.AreEqual(binanceSymbolCache.Subscriptions(Subscribe.OrderBook), 0);
                Assert.IsNotNull(tradeStrategy.OrderBook);
                Assert.IsTrue(tradeStrategy.OrderBook.Asks.Any());
            }
        }

        [TestMethod]
        public async Task Subscribe_OrderBook_Multiple_Subscribers_Unsubscribe_Single_Subscriber()
        {
            // Arrange
            var binanceExchangeService = new TestBinanceExchangeService();
            var strategySymbol1 = new StrategySymbol { Exchange = Exchange.Binance, Symbol = "TRXBTC", Subscribe = Subscribe.OrderBook };
            var strategySymbol2 = new StrategySymbol { Exchange = Exchange.Binance, Symbol = "TRXBTC", Subscribe = Subscribe.OrderBook };
            var tradeStrategy1 = new TestTradeStrategy();
            var tradeStrategy2 = new TestTradeStrategy();

            using (var binanceSymbolCache = new BinanceSymbolSubscriptionCache("TRXBTC", 500, binanceExchangeService))
            {
                // Act
                binanceSymbolCache.Subscribe("Test 1", strategySymbol1, tradeStrategy1);

                binanceSymbolCache.Subscribe("Test 2", strategySymbol2, tradeStrategy2);

                await Task.Delay(2000);

                binanceSymbolCache.Unsubscribe("Test 2", strategySymbol2, tradeStrategy2);

                await Task.Delay(1000);

                // Assert
                Assert.IsTrue(binanceSymbolCache.HasSubscriptions);
                Assert.AreEqual(binanceSymbolCache.Subscriptions(Subscribe.OrderBook), 1);
                Assert.IsNotNull(tradeStrategy1.OrderBook);
                Assert.IsTrue(tradeStrategy1.OrderBook.Asks.Any());
                Assert.IsNotNull(tradeStrategy2.OrderBook);
                Assert.IsTrue(tradeStrategy2.OrderBook.Asks.Any());
            }
        }

        [TestMethod]
        public async Task Subscribe_OrderBook_Multiple_Subscribers_Unsubscribe_All_Subscribers()
        {
            // Arrange
            var binanceExchangeService = new TestBinanceExchangeService();
            var strategySymbol1 = new StrategySymbol { Exchange = Exchange.Binance, Symbol = "TRXBTC", Subscribe = Subscribe.OrderBook };
            var strategySymbol2 = new StrategySymbol { Exchange = Exchange.Binance, Symbol = "TRXBTC", Subscribe = Subscribe.OrderBook };
            var tradeStrategy1 = new TestTradeStrategy();
            var tradeStrategy2 = new TestTradeStrategy();

            using (var binanceSymbolCache = new BinanceSymbolSubscriptionCache("TRXBTC", 500, binanceExchangeService))
            {
                // Act
                binanceSymbolCache.Subscribe("Test 1", strategySymbol1, tradeStrategy1);

                binanceSymbolCache.Subscribe("Test 2", strategySymbol2, tradeStrategy2);

                await Task.Delay(2000);

                binanceSymbolCache.Unsubscribe("Test 2", strategySymbol2, tradeStrategy2);

                binanceSymbolCache.Unsubscribe("Test 1", strategySymbol1, tradeStrategy1);

                await Task.Delay(1000);

                // Assert
                Assert.IsFalse(binanceSymbolCache.HasSubscriptions);
                Assert.AreEqual(binanceSymbolCache.Subscriptions(Subscribe.OrderBook), 0);
                Assert.IsNotNull(tradeStrategy1.OrderBook);
                Assert.IsTrue(tradeStrategy1.OrderBook.Asks.Any());
                Assert.IsNotNull(tradeStrategy2.OrderBook);
                Assert.IsTrue(tradeStrategy2.OrderBook.Asks.Any());
            }
        }

        [TestMethod]
        public async Task OrderBook_Exception()
        {
            // Arrange
            var binanceExchangeService = new TestBinanceExchangeService { OrderBookException = true };
            var strategySymbol = new StrategySymbol { Exchange = Exchange.Binance, Symbol = "TRXBTC", Subscribe = Subscribe.OrderBook };
            var tradeStrategy = new TestTradeStrategy();

            // Act
            using (var binanceSymbolCache = new BinanceSymbolSubscriptionCache("TRXBTC", 500, binanceExchangeService))
            {
                binanceSymbolCache.Subscribe("Test", strategySymbol, tradeStrategy);

                await Task.Delay(1000);

                // Assert
                Assert.IsTrue(binanceSymbolCache.HasSubscriptions);
                Assert.AreEqual(binanceSymbolCache.Subscriptions(Subscribe.OrderBook), 1);
                Assert.IsNotNull(tradeStrategy.OrderBook);
                Assert.IsTrue(tradeStrategy.OrderBook.Asks.Any());
                Assert.IsTrue(tradeStrategy.OrderBookException);
            }
        }

        [TestMethod]
        public async Task Subscribe_OrderBook_AggregateTrades_Multiple_Subscribers_Unsubscribe_Some()
        {
            // Arrange
            var binanceExchangeService = new TestBinanceExchangeService();
            var strategySymbol1 = new StrategySymbol { Exchange = Exchange.Binance, Symbol = "TRXBTC", Subscribe = (Subscribe.OrderBook | Subscribe.AggregateTrades) };
            var strategySymbol2 = new StrategySymbol { Exchange = Exchange.Binance, Symbol = "TRXBTC", Subscribe = (Subscribe.OrderBook | Subscribe.AggregateTrades) };
            var tradeStrategy1 = new TestTradeStrategy();
            var tradeStrategy2 = new TestTradeStrategy();

            using (var binanceSymbolCache = new BinanceSymbolSubscriptionCache("TRXBTC", 500, binanceExchangeService))
            {
                // Act
                binanceSymbolCache.Subscribe("Test 1", strategySymbol1, tradeStrategy1);

                binanceSymbolCache.Subscribe("Test 2", strategySymbol2, tradeStrategy2);

                await Task.Delay(2000);

                binanceSymbolCache.Unsubscribe("Test 2", strategySymbol2, tradeStrategy2);

                await Task.Delay(1000);

                // Assert
                Assert.IsTrue(binanceSymbolCache.HasSubscriptions);
                Assert.AreEqual(binanceSymbolCache.Subscriptions(Subscribe.OrderBook), 1);
                Assert.AreEqual(binanceSymbolCache.Subscriptions(Subscribe.AggregateTrades), 1);
                Assert.IsNotNull(tradeStrategy1.OrderBook);
                Assert.IsTrue(tradeStrategy1.OrderBook.Asks.Any());
                Assert.IsNotNull(tradeStrategy1.AggregateTrades);
                Assert.IsTrue(tradeStrategy1.AggregateTrades.Any());
                Assert.IsNotNull(tradeStrategy2.OrderBook);
                Assert.IsTrue(tradeStrategy2.OrderBook.Asks.Any());
                Assert.IsNotNull(tradeStrategy2.AggregateTrades);
                Assert.IsTrue(tradeStrategy2.AggregateTrades.Any());
            }
        }

        [TestMethod]
        public async Task Subscribe_OrderBook_AggregateTrades_Multiple_Subscribers_Unsubscribe_All()
        {
            // Arrange
            var binanceExchangeService = new TestBinanceExchangeService();
            var strategySymbol1 = new StrategySymbol { Exchange = Exchange.Binance, Symbol = "TRXBTC", Subscribe = (Subscribe.OrderBook | Subscribe.AggregateTrades) };
            var strategySymbol2 = new StrategySymbol { Exchange = Exchange.Binance, Symbol = "TRXBTC", Subscribe = (Subscribe.OrderBook | Subscribe.AggregateTrades) };
            var tradeStrategy1 = new TestTradeStrategy();
            var tradeStrategy2 = new TestTradeStrategy();

            using (var binanceSymbolCache = new BinanceSymbolSubscriptionCache("TRXBTC", 500, binanceExchangeService))
            {
                // Act
                binanceSymbolCache.Subscribe("Test 1", strategySymbol1, tradeStrategy1);

                binanceSymbolCache.Subscribe("Test 2", strategySymbol2, tradeStrategy2);

                await Task.Delay(2000);

                binanceSymbolCache.Unsubscribe("Test 2", strategySymbol2, tradeStrategy1);

                binanceSymbolCache.Unsubscribe("Test 1", strategySymbol1, tradeStrategy2);

                await Task.Delay(1000);

                // Assert
                Assert.IsFalse(binanceSymbolCache.HasSubscriptions);
                Assert.AreEqual(binanceSymbolCache.Subscriptions(Subscribe.OrderBook), 0);
                Assert.AreEqual(binanceSymbolCache.Subscriptions(Subscribe.AggregateTrades), 0);
                Assert.IsNotNull(tradeStrategy1.OrderBook);
                Assert.IsTrue(tradeStrategy1.OrderBook.Asks.Any());
                Assert.IsNotNull(tradeStrategy1.AggregateTrades);
                Assert.IsTrue(tradeStrategy1.AggregateTrades.Any());
                Assert.IsNotNull(tradeStrategy2.OrderBook);
                Assert.IsTrue(tradeStrategy2.OrderBook.Asks.Any());
                Assert.IsNotNull(tradeStrategy2.AggregateTrades);
                Assert.IsTrue(tradeStrategy2.AggregateTrades.Any());
            }
        }
    }
}
