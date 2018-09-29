using DevelopmentInProgress.MarketView.Interface.Strategy;
using DevelopmentInProgress.MarketView.StrategyEngine.Test.Helpers;
using DevelopmentInProgress.TradeServer.StrategyEngine.Cache.Binance;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;

namespace DevelopmentInProgress.MarketView.StrategyEngine.Test
{
    [TestClass]
    public class SubscriptionManagerTests
    {
        [TestMethod]
        public async Task AggregateTradeUpdate_Exception_RemainSubscribed_HandleException()
        {
            // Arrange
            var binanceExchangeService = new TestBinanceExchangeService();
            var strategySubscription = new StrategySubscription { Exchange = Exchange.Binance, Symbol = "TRXBTC", Subscribe = Subscribe.AggregateTrades };
            var tradeStrategy = new TestTradeExceptionStrategy();

            // Act
            using (var binanceSymbolCache = new BinanceSymbolSubscriptionCache("TRXBTC", 500, binanceExchangeService))
            {
                binanceSymbolCache.Subscribe("Test", strategySubscription, tradeStrategy);

                await Task.Delay(1000);

                // Assert
                Assert.IsTrue(binanceSymbolCache.HasSubscriptions);
                Assert.AreEqual(binanceSymbolCache.Subscriptions(Subscribe.AggregateTrades), 1);
                Assert.IsTrue(tradeStrategy.AggregateTradesException);
            }
        }

        [TestMethod]
        public async Task OrderBookException_ForciblyUnsubscribed()
        {
            // Arrange
            var binanceExchangeService = new TestBinanceExchangeService();
            var strategySubscription = new StrategySubscription { Exchange = Exchange.Binance, Symbol = "TRXBTC", Subscribe = Subscribe.OrderBook };
            var tradeStrategy = new TestTradeExceptionStrategy();

            // Act
            using (var binanceSymbolCache = new BinanceSymbolSubscriptionCache("TRXBTC", 500, binanceExchangeService))
            {
                binanceSymbolCache.Subscribe("Test", strategySubscription, tradeStrategy);

                await Task.Delay(1000);

                // Assert
                Assert.IsFalse(binanceSymbolCache.HasSubscriptions);
                Assert.AreEqual(binanceSymbolCache.Subscriptions(Subscribe.OrderBook), 0);
                Assert.IsTrue(tradeStrategy.OrderBookException);
            }
        }
    }
}
