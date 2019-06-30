using DevelopmentInProgress.MarketView.Interface.Strategy;
using DevelopmentInProgress.MarketView.StrategyRunner.Test.Helpers;
using DevelopmentInProgress.TradeServer.StrategyRunner.WebHost.Cache.Binance;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;

namespace DevelopmentInProgress.MarketView.StrategyRunner.Test
{
    [TestClass]
    public class SubscriptionManagerTests
    {
        [TestMethod]
        public async Task AggregateTradeUpdate_Exception_RemainSubscribed_HandleException()
        {
            // Arrange
            var binanceExchangeService = new TestBinanceExchangeService();
            var strategySubscription = new StrategySubscription { Exchange = Exchange.Binance, Symbol = "TRXBTC", Subscribe = Subscribe.Trades };
            var tradeStrategy = new TestTradeExceptionStrategy();

            // Act
            using (var binanceSymbolCache = new BinanceSymbolSubscriptionCache("TRXBTC", 500, Interface.Model.CandlestickInterval.Day, binanceExchangeService))
            {
                binanceSymbolCache.Subscribe("Test", strategySubscription, tradeStrategy);

                await Task.Delay(1000);

                // Assert
                Assert.IsTrue(binanceSymbolCache.HasSubscriptions);
                Assert.AreEqual(binanceSymbolCache.Subscriptions(Subscribe.Trades), 1);
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
            using (var binanceSymbolCache = new BinanceSymbolSubscriptionCache("TRXBTC", 500, Interface.Model.CandlestickInterval.Day, binanceExchangeService))
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
