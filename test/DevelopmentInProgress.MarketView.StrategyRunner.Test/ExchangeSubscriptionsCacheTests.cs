using DevelopmentInProgress.TradeView.Core.Enums;
using DevelopmentInProgress.TradeView.Core.Strategy;
using DevelopmentInProgress.MarketView.StrategyRunner.Test.Helpers;
using DevelopmentInProgress.TradeServer.StrategyRunner.WebHost.Cache.Subscriptions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Threading.Tasks;
using DevelopmentInProgress.TradeView.Service;

namespace DevelopmentInProgress.MarketView.StrategyRunner.Test
{
    [TestClass]
    public class ExchangeSubscriptionsCacheTests
    {
        [TestMethod]
        public async Task Subscribe()
        {
            // Arrange
            var exchangeService = new ExchangeService(new TestExchangeApiFactory());

            var tradeStrategy1 = new TestTradeStrategy();
            var tradeStrategy2 = new TestTradeStrategy();
            var apiKey = "abc123";

            var trx = new StrategySubscription
            {
                Exchange = Exchange.Binance,
                Symbol = "TRXBTC",
                ApiKey = apiKey,
                Subscribe = (TradeView.Core.Strategy.Subscribe.Trades | TradeView.Core.Strategy.Subscribe.OrderBook | TradeView.Core.Strategy.Subscribe.AccountInfo)
            };

            var eth = new StrategySubscription
            {
                Exchange = Exchange.Binance,
                Symbol = "ETHBTC",
                Subscribe = (TradeView.Core.Strategy.Subscribe.Trades | TradeView.Core.Strategy.Subscribe.OrderBook)
            };

            var bnb = new StrategySubscription
            {
                Exchange = Exchange.Binance,
                Symbol = "BNBBTC",
                ApiKey = apiKey,
                Subscribe = (TradeView.Core.Strategy.Subscribe.Trades | TradeView.Core.Strategy.Subscribe.OrderBook | TradeView.Core.Strategy.Subscribe.AccountInfo)
            };

            var strategySubscriptions1 = new List<StrategySubscription>(new[] { trx, eth });
            var strategySubscriptions2 = new List<StrategySubscription>(new[] { eth, bnb });

            // Act
            using (var exchangeSubscriptionsCache = new ExchangeSubscriptionsCache(Exchange.Binance, exchangeService))
            {
                await exchangeSubscriptionsCache.Subscribe("TEST 1", strategySubscriptions1, tradeStrategy1);

                await exchangeSubscriptionsCache.Subscribe("TEST 2", strategySubscriptions2, tradeStrategy2);

                await Task.Delay(1000);

                // Assert
                Assert.IsTrue(exchangeSubscriptionsCache.HasSubscriptions);

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

                if (exchangeSubscriptionsCache.Caches.TryGetValue("TRXBTC", out ISubscriptionCache trxCache))
                {
                    Assert.IsNotNull(trxCache);
                    Assert.IsInstanceOfType(trxCache, typeof(SymbolSubscriptionCache));
                    Assert.IsTrue(trxCache.HasSubscriptions);
                    Assert.AreEqual(trxCache.Subscriptions(TradeView.Core.Strategy.Subscribe.Trades), 1);
                    Assert.AreEqual(trxCache.Subscriptions(TradeView.Core.Strategy.Subscribe.OrderBook), 1);
                }
                else
                {
                    Assert.Fail();
                }

                if (exchangeSubscriptionsCache.Caches.TryGetValue("ETHBTC", out ISubscriptionCache ethCache))
                {
                    Assert.IsNotNull(ethCache);
                    Assert.IsInstanceOfType(ethCache, typeof(SymbolSubscriptionCache));
                    Assert.IsTrue(ethCache.HasSubscriptions);
                    Assert.AreEqual(ethCache.Subscriptions(TradeView.Core.Strategy.Subscribe.Trades), 2);
                    Assert.AreEqual(ethCache.Subscriptions(TradeView.Core.Strategy.Subscribe.OrderBook), 2);
                }
                else
                {
                    Assert.Fail();
                }

                if (exchangeSubscriptionsCache.Caches.TryGetValue("BNBBTC", out ISubscriptionCache bnbCache))
                {
                    Assert.IsNotNull(bnbCache);
                    Assert.IsInstanceOfType(bnbCache, typeof(SymbolSubscriptionCache));
                    Assert.IsTrue(bnbCache.HasSubscriptions);
                    Assert.AreEqual(bnbCache.Subscriptions(TradeView.Core.Strategy.Subscribe.Trades), 1);
                    Assert.AreEqual(bnbCache.Subscriptions(TradeView.Core.Strategy.Subscribe.OrderBook), 1);
                }
                else
                {
                    Assert.Fail();
                }

                if (exchangeSubscriptionsCache.Caches.TryGetValue(apiKey, out ISubscriptionCache accountCache))
                {
                    Assert.IsNotNull(accountCache);
                    Assert.IsInstanceOfType(accountCache, typeof(AccountInfoSubscriptionCache));
                    Assert.IsTrue(accountCache.HasSubscriptions);
                    Assert.AreEqual(accountCache.Subscriptions(TradeView.Core.Strategy.Subscribe.AccountInfo), 2);
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
            var exchangeService = new ExchangeService(new TestExchangeApiFactory());

            var tradeStrategy1 = new TestTradeStrategy();
            var tradeStrategy2 = new TestTradeStrategy();
            var apiKey = "abc123";

            var trx = new StrategySubscription
            {
                Exchange = Exchange.Binance,
                Symbol = "TRXBTC",
                ApiKey = apiKey,
                Subscribe = (TradeView.Core.Strategy.Subscribe.Trades | TradeView.Core.Strategy.Subscribe.OrderBook | TradeView.Core.Strategy.Subscribe.AccountInfo)
            };

            var eth = new StrategySubscription
            {
                Exchange = Exchange.Binance,
                Symbol = "ETHBTC",
                Subscribe = (TradeView.Core.Strategy.Subscribe.Trades | TradeView.Core.Strategy.Subscribe.OrderBook)
            };

            var bnb = new StrategySubscription
            {
                Exchange = Exchange.Binance,
                Symbol = "BNBBTC",
                ApiKey = apiKey,
                Subscribe = (TradeView.Core.Strategy.Subscribe.Trades | TradeView.Core.Strategy.Subscribe.OrderBook | TradeView.Core.Strategy.Subscribe.AccountInfo)
            };

            var strategySubscriptions1 = new List<StrategySubscription>(new[] { trx, eth });
            var strategySubscriptions2 = new List<StrategySubscription>(new[] { eth, bnb });

            // Act
            using (var exchangeSubscriptionsCache = new ExchangeSubscriptionsCache(Exchange.Binance, exchangeService))
            {
                await exchangeSubscriptionsCache.Subscribe("TEST 1", strategySubscriptions1, tradeStrategy1);

                await exchangeSubscriptionsCache.Subscribe("TEST 2", strategySubscriptions2, tradeStrategy2);

                await Task.Delay(1000);

                exchangeSubscriptionsCache.Unsubscribe("TEST 1", strategySubscriptions1, tradeStrategy1);

                // Assert
                Assert.IsTrue(exchangeSubscriptionsCache.HasSubscriptions);

                Assert.AreEqual(tradeStrategy2.TradeSymbols.Count, 2);
                Assert.IsTrue(tradeStrategy2.TradeSymbols.Contains(bnb.Symbol));
                Assert.IsTrue(tradeStrategy2.TradeSymbols.Contains(eth.Symbol));

                Assert.AreEqual(tradeStrategy2.OrderBookSymbols.Count, 2);
                Assert.IsTrue(tradeStrategy2.OrderBookSymbols.Contains(bnb.Symbol));
                Assert.IsTrue(tradeStrategy2.OrderBookSymbols.Contains(eth.Symbol));

                if (exchangeSubscriptionsCache.Caches.TryGetValue("TRXBTC", out ISubscriptionCache trxCache))
                {
                    Assert.Fail();
                }

                if (exchangeSubscriptionsCache.Caches.TryGetValue("ETHBTC", out ISubscriptionCache ethCache))
                {
                    Assert.IsNotNull(ethCache);
                    Assert.IsInstanceOfType(ethCache, typeof(SymbolSubscriptionCache));
                    Assert.IsTrue(ethCache.HasSubscriptions);
                    Assert.AreEqual(ethCache.Subscriptions(TradeView.Core.Strategy.Subscribe.Trades), 1);
                    Assert.AreEqual(ethCache.Subscriptions(TradeView.Core.Strategy.Subscribe.OrderBook), 1);
                }
                else
                {
                    Assert.Fail();
                }

                if (exchangeSubscriptionsCache.Caches.TryGetValue("BNBBTC", out ISubscriptionCache bnbCache))
                {
                    Assert.IsNotNull(bnbCache);
                    Assert.IsInstanceOfType(bnbCache, typeof(SymbolSubscriptionCache));
                    Assert.IsTrue(bnbCache.HasSubscriptions);
                    Assert.AreEqual(bnbCache.Subscriptions(TradeView.Core.Strategy.Subscribe.Trades), 1);
                    Assert.AreEqual(bnbCache.Subscriptions(TradeView.Core.Strategy.Subscribe.OrderBook), 1);
                }
                else
                {
                    Assert.Fail();
                }

                if (exchangeSubscriptionsCache.Caches.TryGetValue(apiKey, out ISubscriptionCache accountCache))
                {
                    Assert.IsNotNull(accountCache);
                    Assert.IsInstanceOfType(accountCache, typeof(AccountInfoSubscriptionCache));
                    Assert.IsTrue(accountCache.HasSubscriptions);
                    Assert.AreEqual(accountCache.Subscriptions(TradeView.Core.Strategy.Subscribe.AccountInfo), 1);
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
            var exchangeService = new ExchangeService(new TestExchangeApiFactory());

            var tradeStrategy1 = new TestTradeStrategy();
            var tradeStrategy2 = new TestTradeStrategy();
            var apiKey = "abc123";

            var trx = new StrategySubscription
            {
                Exchange = Exchange.Binance,
                Symbol = "TRXBTC",
                ApiKey = apiKey,
                Subscribe = (TradeView.Core.Strategy.Subscribe.Trades | TradeView.Core.Strategy.Subscribe.OrderBook | TradeView.Core.Strategy.Subscribe.AccountInfo)
            };

            var eth = new StrategySubscription
            {
                Exchange = Exchange.Binance,
                Symbol = "ETHBTC",
                Subscribe = (TradeView.Core.Strategy.Subscribe.Trades | TradeView.Core.Strategy.Subscribe.OrderBook)
            };

            var bnb = new StrategySubscription
            {
                Exchange = Exchange.Binance,
                Symbol = "BNBBTC",
                ApiKey = apiKey,
                Subscribe = (TradeView.Core.Strategy.Subscribe.Trades | TradeView.Core.Strategy.Subscribe.OrderBook | TradeView.Core.Strategy.Subscribe.AccountInfo)
            };

            var strategySubscriptions1 = new List<StrategySubscription>(new[] { trx, eth });
            var strategySubscriptions2 = new List<StrategySubscription>(new[] { eth, bnb });

            // Act
            using (var exchangeSubscriptionsCache = new ExchangeSubscriptionsCache(Exchange.Binance, exchangeService))
            {
                await exchangeSubscriptionsCache.Subscribe("TEST 1", strategySubscriptions1, tradeStrategy1);

                await exchangeSubscriptionsCache.Subscribe("TEST 2", strategySubscriptions2, tradeStrategy2);

                await Task.Delay(1000);

                exchangeSubscriptionsCache.Unsubscribe("TEST 1", strategySubscriptions1, tradeStrategy1);

                exchangeSubscriptionsCache.Unsubscribe("TEST 2", strategySubscriptions2, tradeStrategy2);

                // Assert
                Assert.IsFalse(exchangeSubscriptionsCache.HasSubscriptions);

                if (exchangeSubscriptionsCache.Caches.TryGetValue("TRXBTC", out ISubscriptionCache trxCache))
                {
                    Assert.Fail();
                }

                if (exchangeSubscriptionsCache.Caches.TryGetValue("ETHBTC", out ISubscriptionCache ethCache))
                {
                    Assert.Fail();
                }

                if (exchangeSubscriptionsCache.Caches.TryGetValue("BNBBTC", out ISubscriptionCache bnbCache))
                {
                    Assert.Fail();
                }

                if (exchangeSubscriptionsCache.Caches.TryGetValue(apiKey, out ISubscriptionCache accountCache))
                {
                    Assert.Fail();
                }
            }
        }
    }
}