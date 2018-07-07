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
            var exchangeServiceFactory = new TestExchangeServiceFactory();
            var binanceExchangeService = exchangeServiceFactory.GetExchangeService(Exchange.Binance);
            var strategySymbol = new StrategySymbol { Exchange = Exchange.Binance, Symbol = "TRXBTC", Subscribe = Subscribe.AggregateTrades };
            var tradeStrategy = new TestTradeStrategy();

            var binanceSymbolCache = new BinanceSymbolCache("TRXBTC", 500, binanceExchangeService);

            // Act
            binanceSymbolCache.Subscribe("Test", strategySymbol, tradeStrategy);

            await Task.Delay(1000);

            // Assert
            Assert.IsTrue(binanceSymbolCache.HasSubscriptions);
            Assert.AreEqual(binanceSymbolCache.AggregateTradesSubscribers.Count(), 1);
            Assert.IsNotNull(tradeStrategy.AggregateTrades);
            Assert.IsTrue(tradeStrategy.AggregateTrades.Any());
        }

        [TestMethod]
        public void Subscribe_AggregateTrades_Multiple_Subscribers()
        {
            // Arrange

            // Act

            // Assert
        }

        [TestMethod]
        public void Subscribe_AggregateTrades_Single_Subscriber_Unsubscribe()
        {
            // Arrange

            // Act

            // Assert
        }

        [TestMethod]
        public void Subscribe_AggregateTrades_Multiple_Subscribers_Unsubscribe_Single_Subscriber()
        {
            // Arrange

            // Act

            // Assert
        }

        [TestMethod]
        public void Subscribe_AggregateTrades_Multiple_Subscribers_Unsubscribe_All_Subscribers()
        {
            // Arrange

            // Act

            // Assert
        }
    }
}
