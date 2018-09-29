using DevelopmentInProgress.MarketView.Interface.Strategy;
using DevelopmentInProgress.MarketView.StrategyEngine.Test.Helpers;
using DevelopmentInProgress.TradeServer.StrategyEngine.Cache;
using DevelopmentInProgress.TradeServer.StrategyEngine.Cache.Binance;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DevelopmentInProgress.MarketView.StrategyEngine.Test
{
    [TestClass]
    public class BinanceSubscriptionsCacheTests
    {
        [TestMethod]
        public async Task Subscribe()
        {
            // Arrange
            var binanceExchangeService = new TestBinanceExchangeService();

            var tradeStrategy1 = new TestTradeStrategy();
            var tradeStrategy2 = new TestTradeStrategy();
            var apiKey = "abc123";

            var trx = new StrategySubscription
            {
                Exchange = Exchange.Binance,
                Symbol = "TRXBTC",
                ApiKey = apiKey,
                Subscribe = (Interface.Strategy.Subscribe.AggregateTrades | Interface.Strategy.Subscribe.OrderBook | Interface.Strategy.Subscribe.AccountInfo)
            };

            var eth = new StrategySubscription
            {
                Exchange = Exchange.Binance,
                Symbol = "ETHBTC",
                Subscribe = (Interface.Strategy.Subscribe.AggregateTrades | Interface.Strategy.Subscribe.OrderBook | Interface.Strategy.Subscribe.Statistics)
            };

            var bnb = new StrategySubscription
            {
                Exchange = Exchange.Binance,
                Symbol = "BNBBTC",
                ApiKey = apiKey,
                Subscribe = (Interface.Strategy.Subscribe.AggregateTrades | Interface.Strategy.Subscribe.OrderBook | Interface.Strategy.Subscribe.AccountInfo)
            };

            var strategySubscriptions1 = new List<StrategySubscription>(new[] { trx, eth });
            var strategySubscriptions2 = new List<StrategySubscription>(new[] { eth, bnb });

            // Act
            using (var binanceSubscriptionsCache = new BinanceSubscriptionsCache(binanceExchangeService))
            {
                binanceSubscriptionsCache.Subscribe("TEST 1", strategySubscriptions1, tradeStrategy1);

                binanceSubscriptionsCache.Subscribe("TEST 2", strategySubscriptions2, tradeStrategy2);

                await Task.Delay(1000);

                // Assert
                Assert.IsTrue(binanceSubscriptionsCache.HasSubscriptions);

                Assert.AreEqual(tradeStrategy1.TradeSymbols.Count, 2);
                Assert.IsTrue(tradeStrategy1.TradeSymbols.Contains(trx.Symbol));
                Assert.IsTrue(tradeStrategy1.TradeSymbols.Contains(eth.Symbol));

                Assert.AreEqual(tradeStrategy1.OrderBookSymbols.Count, 2);
                Assert.IsTrue(tradeStrategy1.OrderBookSymbols.Contains(trx.Symbol));
                Assert.IsTrue(tradeStrategy1.OrderBookSymbols.Contains(eth.Symbol));

                Assert.AreEqual(tradeStrategy2.TradeSymbols.Count, 2);
                Assert.IsTrue(tradeStrategy2.TradeSymbols.Contains(bnb.Symbol));
                Assert.IsTrue(tradeStrategy2.TradeSymbols.Contains(eth.Symbol));

                Assert.AreEqual(tradeStrategy2.OrderBookSymbols.Count, 2);
                Assert.IsTrue(tradeStrategy2.OrderBookSymbols.Contains(bnb.Symbol));
                Assert.IsTrue(tradeStrategy2.OrderBookSymbols.Contains(eth.Symbol));

                if (binanceSubscriptionsCache.Caches.TryGetValue("TRXBTC", out ISubscriptionCache trxCache))
                {
                    Assert.IsNotNull(trxCache);
                    Assert.IsInstanceOfType(trxCache, typeof(BinanceSymbolSubscriptionCache));
                    Assert.IsTrue(trxCache.HasSubscriptions);
                    Assert.AreEqual(trxCache.Subscriptions(Interface.Strategy.Subscribe.AggregateTrades), 1);
                    Assert.AreEqual(trxCache.Subscriptions(Interface.Strategy.Subscribe.OrderBook), 1);
                }
                else
                {
                    Assert.Fail();
                }

                if (binanceSubscriptionsCache.Caches.TryGetValue("ETHBTC", out ISubscriptionCache ethCache))
                {
                    Assert.IsNotNull(ethCache);
                    Assert.IsInstanceOfType(ethCache, typeof(BinanceSymbolSubscriptionCache));
                    Assert.IsTrue(ethCache.HasSubscriptions);
                    Assert.AreEqual(ethCache.Subscriptions(Interface.Strategy.Subscribe.AggregateTrades), 2);
                    Assert.AreEqual(ethCache.Subscriptions(Interface.Strategy.Subscribe.OrderBook), 2);
                }
                else
                {
                    Assert.Fail();
                }

                if (binanceSubscriptionsCache.Caches.TryGetValue("BNBBTC", out ISubscriptionCache bnbCache))
                {
                    Assert.IsNotNull(bnbCache);
                    Assert.IsInstanceOfType(bnbCache, typeof(BinanceSymbolSubscriptionCache));
                    Assert.IsTrue(bnbCache.HasSubscriptions);
                    Assert.AreEqual(bnbCache.Subscriptions(Interface.Strategy.Subscribe.AggregateTrades), 1);
                    Assert.AreEqual(bnbCache.Subscriptions(Interface.Strategy.Subscribe.OrderBook), 1);
                }
                else
                {
                    Assert.Fail();
                }

                if (binanceSubscriptionsCache.Caches.TryGetValue($"{nameof(BinanceSymbolSubscriptionCache)}", out ISubscriptionCache statsCache))
                {
                    Assert.IsNotNull(statsCache);
                    Assert.IsInstanceOfType(statsCache, typeof(Binance24HourStatisticsSubscriptionCache));
                    Assert.IsTrue(statsCache.HasSubscriptions);
                    Assert.AreEqual(statsCache.Subscriptions(Interface.Strategy.Subscribe.Statistics), 2);
                }
                else
                {
                    Assert.Fail();
                }

                if (binanceSubscriptionsCache.Caches.TryGetValue(apiKey, out ISubscriptionCache accountCache))
                {
                    Assert.IsNotNull(accountCache);
                    Assert.IsInstanceOfType(accountCache, typeof(BinanceAccountInfoSubscriptionCache));
                    Assert.IsTrue(accountCache.HasSubscriptions);
                    Assert.AreEqual(accountCache.Subscriptions(Interface.Strategy.Subscribe.AccountInfo), 2);
                }
                else
                {
                    Assert.Fail();
                }
            }
        }

        [TestMethod]
        public async Task Unsubscribe_Partial()
        {
            // Arrange
            var binanceExchangeService = new TestBinanceExchangeService();

            var tradeStrategy1 = new TestTradeStrategy();
            var tradeStrategy2 = new TestTradeStrategy();
            var apiKey = "abc123";

            var trx = new StrategySubscription
            {
                Exchange = Exchange.Binance,
                Symbol = "TRXBTC",
                ApiKey = apiKey,
                Subscribe = (Interface.Strategy.Subscribe.AggregateTrades | Interface.Strategy.Subscribe.OrderBook | Interface.Strategy.Subscribe.AccountInfo)
            };

            var eth = new StrategySubscription
            {
                Exchange = Exchange.Binance,
                Symbol = "ETHBTC",
                Subscribe = (Interface.Strategy.Subscribe.AggregateTrades | Interface.Strategy.Subscribe.OrderBook | Interface.Strategy.Subscribe.Statistics)
            };

            var bnb = new StrategySubscription
            {
                Exchange = Exchange.Binance,
                Symbol = "BNBBTC",
                ApiKey = apiKey,
                Subscribe = (Interface.Strategy.Subscribe.AggregateTrades | Interface.Strategy.Subscribe.OrderBook | Interface.Strategy.Subscribe.AccountInfo)
            };

            var strategySubscriptions1 = new List<StrategySubscription>(new[] { trx, eth });
            var strategySubscriptions2 = new List<StrategySubscription>(new[] { eth, bnb });

            // Act
            using (var binanceSubscriptionsCache = new BinanceSubscriptionsCache(binanceExchangeService))
            {
                binanceSubscriptionsCache.Subscribe("TEST 1", strategySubscriptions1, tradeStrategy1);

                binanceSubscriptionsCache.Subscribe("TEST 2", strategySubscriptions2, tradeStrategy2);

                await Task.Delay(1000);

                binanceSubscriptionsCache.Unsubscribe("TEST 1", strategySubscriptions1, tradeStrategy1);

                // Assert
                Assert.IsTrue(binanceSubscriptionsCache.HasSubscriptions);

                Assert.AreEqual(tradeStrategy2.TradeSymbols.Count, 2);
                Assert.IsTrue(tradeStrategy2.TradeSymbols.Contains(bnb.Symbol));
                Assert.IsTrue(tradeStrategy2.TradeSymbols.Contains(eth.Symbol));

                Assert.AreEqual(tradeStrategy2.OrderBookSymbols.Count, 2);
                Assert.IsTrue(tradeStrategy2.OrderBookSymbols.Contains(bnb.Symbol));
                Assert.IsTrue(tradeStrategy2.OrderBookSymbols.Contains(eth.Symbol));

                if (binanceSubscriptionsCache.Caches.TryGetValue("TRXBTC", out ISubscriptionCache trxCache))
                {
                    Assert.Fail();
                }

                if (binanceSubscriptionsCache.Caches.TryGetValue("ETHBTC", out ISubscriptionCache ethCache))
                {
                    Assert.IsNotNull(ethCache);
                    Assert.IsInstanceOfType(ethCache, typeof(BinanceSymbolSubscriptionCache));
                    Assert.IsTrue(ethCache.HasSubscriptions);
                    Assert.AreEqual(ethCache.Subscriptions(Interface.Strategy.Subscribe.AggregateTrades), 1);
                    Assert.AreEqual(ethCache.Subscriptions(Interface.Strategy.Subscribe.OrderBook), 1);
                }
                else
                {
                    Assert.Fail();
                }

                if (binanceSubscriptionsCache.Caches.TryGetValue("BNBBTC", out ISubscriptionCache bnbCache))
                {
                    Assert.IsNotNull(bnbCache);
                    Assert.IsInstanceOfType(bnbCache, typeof(BinanceSymbolSubscriptionCache));
                    Assert.IsTrue(bnbCache.HasSubscriptions);
                    Assert.AreEqual(bnbCache.Subscriptions(Interface.Strategy.Subscribe.AggregateTrades), 1);
                    Assert.AreEqual(bnbCache.Subscriptions(Interface.Strategy.Subscribe.OrderBook), 1);
                }
                else
                {
                    Assert.Fail();
                }

                if (binanceSubscriptionsCache.Caches.TryGetValue($"{nameof(BinanceSymbolSubscriptionCache)}", out ISubscriptionCache statsCache))
                {
                    Assert.IsNotNull(statsCache);
                    Assert.IsInstanceOfType(statsCache, typeof(Binance24HourStatisticsSubscriptionCache));
                    Assert.IsTrue(statsCache.HasSubscriptions);
                    Assert.AreEqual(statsCache.Subscriptions(Interface.Strategy.Subscribe.Statistics), 1);
                }
                else
                {
                    Assert.Fail();
                }

                if (binanceSubscriptionsCache.Caches.TryGetValue(apiKey, out ISubscriptionCache accountCache))
                {
                    Assert.IsNotNull(accountCache);
                    Assert.IsInstanceOfType(accountCache, typeof(BinanceAccountInfoSubscriptionCache));
                    Assert.IsTrue(accountCache.HasSubscriptions);
                    Assert.AreEqual(accountCache.Subscriptions(Interface.Strategy.Subscribe.AccountInfo), 1);
                }
                else
                {
                    Assert.Fail();
                }
            }
        }

        [TestMethod]
        public async Task Unsubscribe_All()
        {
            // Arrange
            var binanceExchangeService = new TestBinanceExchangeService();

            var tradeStrategy1 = new TestTradeStrategy();
            var tradeStrategy2 = new TestTradeStrategy();
            var apiKey = "abc123";

            var trx = new StrategySubscription
            {
                Exchange = Exchange.Binance,
                Symbol = "TRXBTC",
                ApiKey = apiKey,
                Subscribe = (Interface.Strategy.Subscribe.AggregateTrades | Interface.Strategy.Subscribe.OrderBook | Interface.Strategy.Subscribe.AccountInfo)
            };

            var eth = new StrategySubscription
            {
                Exchange = Exchange.Binance,
                Symbol = "ETHBTC",
                Subscribe = (Interface.Strategy.Subscribe.AggregateTrades | Interface.Strategy.Subscribe.OrderBook | Interface.Strategy.Subscribe.Statistics)
            };

            var bnb = new StrategySubscription
            {
                Exchange = Exchange.Binance,
                Symbol = "BNBBTC",
                ApiKey = apiKey,
                Subscribe = (Interface.Strategy.Subscribe.AggregateTrades | Interface.Strategy.Subscribe.OrderBook | Interface.Strategy.Subscribe.AccountInfo)
            };

            var strategySubscriptions1 = new List<StrategySubscription>(new[] { trx, eth });
            var strategySubscriptions2 = new List<StrategySubscription>(new[] { eth, bnb });

            // Act
            using (var binanceSubscriptionsCache = new BinanceSubscriptionsCache(binanceExchangeService))
            {
                binanceSubscriptionsCache.Subscribe("TEST 1", strategySubscriptions1, tradeStrategy1);

                binanceSubscriptionsCache.Subscribe("TEST 2", strategySubscriptions2, tradeStrategy2);

                await Task.Delay(1000);

                binanceSubscriptionsCache.Unsubscribe("TEST 1", strategySubscriptions1, tradeStrategy1);

                binanceSubscriptionsCache.Unsubscribe("TEST 2", strategySubscriptions2, tradeStrategy2);

                // Assert
                Assert.IsFalse(binanceSubscriptionsCache.HasSubscriptions);

                if (binanceSubscriptionsCache.Caches.TryGetValue("TRXBTC", out ISubscriptionCache trxCache))
                {
                    Assert.Fail();
                }

                if (binanceSubscriptionsCache.Caches.TryGetValue("ETHBTC", out ISubscriptionCache ethCache))
                {
                    Assert.Fail();
                }

                if (binanceSubscriptionsCache.Caches.TryGetValue("BNBBTC", out ISubscriptionCache bnbCache))
                {
                    Assert.Fail();
                }

                if (binanceSubscriptionsCache.Caches.TryGetValue($"{nameof(BinanceSymbolSubscriptionCache)}", out ISubscriptionCache statsCache))
                {
                    Assert.Fail();
                }

                if (binanceSubscriptionsCache.Caches.TryGetValue(apiKey, out ISubscriptionCache accountCache))
                {
                    Assert.Fail();
                }
            }
        }
    }
}