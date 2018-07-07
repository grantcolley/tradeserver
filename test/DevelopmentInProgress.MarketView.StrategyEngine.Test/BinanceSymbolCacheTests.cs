using DevelopmentInProgress.MarketView.Interface.TradeStrategy;
using DevelopmentInProgress.MarketView.StrategyEngine.Test.Helpers;
using DevelopmentInProgress.TradeServer.StrategyEngine.Cache;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using System.Threading.Tasks;

namespace DevelopmentInProgress.MarketView.StrategyEngine.Test
{
    [TestClass]
    public class BinanceSymbolCacheTests
    {
        [TestMethod]
        public async Task Subscribe_AggregateTrades_Single_Subscriber()
        {
            // Arrange
            using (var binanceExchangeService = new TestBinanceExchangeService())
            {
                var strategySymbol = new StrategySymbol { Exchange = Exchange.Binance, Symbol = "TRXBTC", Subscribe = Subscribe.AggregateTrades };
                var tradeStrategy = new TestTradeStrategy();

                var binanceSymbolCache = new BinanceSymbolCache("TRXBTC", 500, binanceExchangeService);

                // Act
                binanceSymbolCache.Subscribe("Test", strategySymbol, tradeStrategy);

                await Task.Delay(1000);

                binanceExchangeService.Cancel();

                // Assert
                Assert.IsTrue(binanceSymbolCache.HasSubscriptions);
                Assert.AreEqual(binanceSymbolCache.AggregateTradesSubscribers.Count(), 1);
                Assert.IsNotNull(tradeStrategy.AggregateTrades);
                Assert.IsTrue(tradeStrategy.AggregateTrades.Any());
            }
        }

        [TestMethod]
        public async Task Subscribe_AggregateTrades_Multiple_Subscribers()
        {
            // Arrange
            using (var binanceExchangeService = new TestBinanceExchangeService())
            {
                var strategySymbol1 = new StrategySymbol { Exchange = Exchange.Binance, Symbol = "TRXBTC", Subscribe = Subscribe.AggregateTrades };
                var strategySymbol2 = new StrategySymbol { Exchange = Exchange.Binance, Symbol = "TRXBTC", Subscribe = Subscribe.AggregateTrades };
                var tradeStrategy1 = new TestTradeStrategy();
                var tradeStrategy2 = new TestTradeStrategy();

                var binanceSymbolCache = new BinanceSymbolCache("TRXBTC", 500, binanceExchangeService);

                // Act
                binanceSymbolCache.Subscribe("Test 1", strategySymbol1, tradeStrategy1);

                binanceSymbolCache.Subscribe("Test 2", strategySymbol2, tradeStrategy2);

                await Task.Delay(2000);

                binanceExchangeService.Cancel();

                // Assert
                Assert.IsTrue(binanceSymbolCache.HasSubscriptions);
                Assert.AreEqual(binanceSymbolCache.AggregateTradesSubscribers.Count(), 2);
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
            using (var binanceExchangeService = new TestBinanceExchangeService())
            {
                var strategySymbol = new StrategySymbol { Exchange = Exchange.Binance, Symbol = "TRXBTC", Subscribe = Subscribe.AggregateTrades };
                var tradeStrategy = new TestTradeStrategy();

                var binanceSymbolCache = new BinanceSymbolCache("TRXBTC", 500, binanceExchangeService);

                // Act
                binanceSymbolCache.Subscribe("Test", strategySymbol, tradeStrategy);

                await Task.Delay(1000);

                binanceSymbolCache.Unsubscribe("Test", strategySymbol, tradeStrategy);

                await Task.Delay(1000);

                binanceExchangeService.Cancel();

                // Assert
                Assert.IsFalse(binanceSymbolCache.HasSubscriptions);
                Assert.AreEqual(binanceSymbolCache.AggregateTradesSubscribers.Count(), 0);
                Assert.IsNotNull(tradeStrategy.AggregateTrades);
                Assert.IsTrue(tradeStrategy.AggregateTrades.Any());
            }
        }

        [TestMethod]
        public async Task Subscribe_AggregateTrades_Multiple_Subscribers_Unsubscribe_Single_Subscriber()
        {
            // Arrange
            using (var binanceExchangeService = new TestBinanceExchangeService())
            {
                var strategySymbol1 = new StrategySymbol { Exchange = Exchange.Binance, Symbol = "TRXBTC", Subscribe = Subscribe.AggregateTrades };
                var strategySymbol2 = new StrategySymbol { Exchange = Exchange.Binance, Symbol = "TRXBTC", Subscribe = Subscribe.AggregateTrades };
                var tradeStrategy1 = new TestTradeStrategy();
                var tradeStrategy2 = new TestTradeStrategy();

                var binanceSymbolCache = new BinanceSymbolCache("TRXBTC", 500, binanceExchangeService);

                // Act
                binanceSymbolCache.Subscribe("Test 1", strategySymbol1, tradeStrategy1);

                binanceSymbolCache.Subscribe("Test 2", strategySymbol2, tradeStrategy2);

                await Task.Delay(2000);

                binanceSymbolCache.Unsubscribe("Test 2", strategySymbol2, tradeStrategy2);

                await Task.Delay(1000);

                binanceExchangeService.Cancel();

                // Assert
                Assert.IsTrue(binanceSymbolCache.HasSubscriptions);
                Assert.AreEqual(binanceSymbolCache.AggregateTradesSubscribers.Count(), 1);
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
            using (var binanceExchangeService = new TestBinanceExchangeService())
            {
                var strategySymbol1 = new StrategySymbol { Exchange = Exchange.Binance, Symbol = "TRXBTC", Subscribe = Subscribe.AggregateTrades };
                var strategySymbol2 = new StrategySymbol { Exchange = Exchange.Binance, Symbol = "TRXBTC", Subscribe = Subscribe.AggregateTrades };
                var tradeStrategy1 = new TestTradeStrategy();
                var tradeStrategy2 = new TestTradeStrategy();

                var binanceSymbolCache = new BinanceSymbolCache("TRXBTC", 500, binanceExchangeService);

                // Act
                binanceSymbolCache.Subscribe("Test 1", strategySymbol1, tradeStrategy1);

                binanceSymbolCache.Subscribe("Test 2", strategySymbol2, tradeStrategy2);

                await Task.Delay(2000);

                binanceSymbolCache.Unsubscribe("Test 2", strategySymbol2, tradeStrategy2);

                binanceSymbolCache.Unsubscribe("Test 1", strategySymbol1, tradeStrategy1);

                await Task.Delay(1000);

                binanceExchangeService.Cancel();

                // Assert
                Assert.IsFalse(binanceSymbolCache.HasSubscriptions);
                Assert.AreEqual(binanceSymbolCache.AggregateTradesSubscribers.Count(), 0);
                Assert.IsNotNull(tradeStrategy1.AggregateTrades);
                Assert.IsTrue(tradeStrategy1.AggregateTrades.Any());
                Assert.IsNotNull(tradeStrategy2.AggregateTrades);
                Assert.IsTrue(tradeStrategy2.AggregateTrades.Any());
            }
        }
    }
}
