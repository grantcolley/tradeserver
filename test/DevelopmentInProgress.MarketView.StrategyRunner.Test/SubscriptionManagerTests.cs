using DevelopmentInProgress.TradeView.Interface.Enums;
using DevelopmentInProgress.TradeView.Interface.Model;
using DevelopmentInProgress.TradeView.Interface.Strategy;
using DevelopmentInProgress.MarketView.StrategyRunner.Test.Helpers;
using DevelopmentInProgress.TradeServer.StrategyRunner.WebHost.Cache.Subscriptions;
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
            var binanceExchangeService = new TestBinanceExchangeApi();
            var strategySubscription = new StrategySubscription { Exchange = Exchange.Binance, Symbol = "TRXBTC", Subscribe = Subscribe.Trades };
            var tradeStrategy = new TestTradeExceptionStrategy();

            // Act
            using (var symbolCache = new SymbolSubscriptionCache("TRXBTC", 500, CandlestickInterval.Day, binanceExchangeService))
            {
                symbolCache.Subscribe("Test", strategySubscription, tradeStrategy);

                await Task.Delay(1000);

                // Assert
                Assert.IsTrue(symbolCache.HasSubscriptions);
                Assert.AreEqual(symbolCache.Subscriptions(Subscribe.Trades), 1);
                Assert.IsTrue(tradeStrategy.AggregateTradesException);
            }
        }

        [TestMethod]
        public async Task OrderBookException_ForciblyUnsubscribed()
        {
            // Arrange
            var binanceExchangeService = new TestBinanceExchangeApi();
            var strategySubscription = new StrategySubscription { Exchange = Exchange.Binance, Symbol = "TRXBTC", Subscribe = Subscribe.OrderBook };
            var tradeStrategy = new TestTradeExceptionStrategy();

            // Act
            using (var symbolCache = new SymbolSubscriptionCache("TRXBTC", 500, CandlestickInterval.Day, binanceExchangeService))
            {
                symbolCache.Subscribe("Test", strategySubscription, tradeStrategy);

                await Task.Delay(1000);

                // Assert
                Assert.IsFalse(symbolCache.HasSubscriptions);
                Assert.AreEqual(symbolCache.Subscriptions(Subscribe.OrderBook), 0);
                Assert.IsTrue(tradeStrategy.OrderBookException);
            }
        }
    }
}
