using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DevelopmentInProgress.MarketView.Interface.Events;
using DevelopmentInProgress.MarketView.Interface.Interfaces;
using DevelopmentInProgress.MarketView.Interface.Model;
using DevelopmentInProgress.MarketView.StrategyRunner.Test.Helpers.Data;

namespace DevelopmentInProgress.MarketView.StrategyRunner.Test.Helpers
{
    public class TestBinanceExchangeService : IExchangeService
    {
        public bool AggregateTradesException { get; set; }
        public bool OrderBookException { get; set; }
        public bool StatisticsException { get; set; }
        public bool AccountInfoException { get; set; }

        public Task<string> CancelOrderAsync(User user, string symbol, long orderId, string newClientOrderId = null, long recWindow = 0, CancellationToken cancellationToken = default(CancellationToken))
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<SymbolStats>> Get24HourStatisticsAsync(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<AccountInfo> GetAccountInfoAsync(User user, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<AccountTrade>> GetAccountTradesAsync(User user, string symbol, DateTime startDate, DateTime endDate, long recWindow = 0, CancellationToken cancellationToken = default(CancellationToken))
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<AggregateTrade>> GetAggregateTradesAsync(string symbol, int limit, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Candlestick>> GetCandlesticksAsync(string symbol, CandlestickInterval interval, DateTime startTime, DateTime endTime, int limit = 0, CancellationToken token = default(CancellationToken))
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Order>> GetOpenOrdersAsync(User user, string symbol = null, long recWindow = 0, Action<Exception> exception = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            throw new NotImplementedException();
        }

        public Task<OrderBook> GetOrderBookAsync(string symbol, int limit, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Symbol>> GetSymbolsAsync(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Trade>> GetTradesAsync(string symbol, int limit, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<Order> PlaceOrder(User user, ClientOrder clientOrder, long recWindow = 0, CancellationToken cancellationToken = default(CancellationToken))
        {
            throw new NotImplementedException();
        }

        public void SubscribeAccountInfo(User user, Action<AccountInfoEventArgs> callback, Action<Exception> exception, CancellationToken cancellationToken)
        {
            Task.Factory.StartNew(async () =>
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    callback.Invoke(new AccountInfoEventArgs { AccountInfo = TestDataHelper.AccountInfo });
                    await Task.Delay(500);

                    if (AccountInfoException)
                    {
                        exception.Invoke(new Exception("SubscribeAccountInfo"));
                    }
                }
            });
        }

        public void SubscribeAggregateTrades(string symbol, int limit, Action<TradeEventArgs> callback, Action<Exception> exception, CancellationToken cancellationToken)
        {
            Task.Factory.StartNew(async () =>
            {
                var localSymbol = symbol;
                while(!cancellationToken.IsCancellationRequested)
                {
                    callback.Invoke(new 
                        TradeEventArgs { Trades = TestDataHelper.GetAggregateTradesUpdated(localSymbol) });
                    await Task.Delay(500);


                    if(AggregateTradesException)
                    {
                        exception.Invoke(new Exception("SubscribeAggregateTrades"));
                    }
                }
            });
        }

        public void SubscribeOrderBook(string symbol, int limit, Action<OrderBookEventArgs> callback, Action<Exception> exception, CancellationToken cancellationToken)
        {
            Task.Factory.StartNew(async () =>
            {
                var localSymbol = symbol;
                while (!cancellationToken.IsCancellationRequested)
                {
                    callback.Invoke(new OrderBookEventArgs { OrderBook = TestDataHelper.GetOrderBook(localSymbol) });
                    await Task.Delay(500);

                    if (OrderBookException)
                    {
                        exception.Invoke(new Exception("SubscribeOrderBook"));
                    }
                }
            });
        }

        public void SubscribeStatistics(Action<StatisticsEventArgs> callback, Action<Exception> exception, CancellationToken cancellationToken)
        {
            Task.Factory.StartNew(async () =>
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    callback.Invoke(new StatisticsEventArgs { Statistics = TestDataHelper.SymbolsStatistics });
                    await Task.Delay(500);

                    if (StatisticsException)
                    {
                        exception.Invoke(new Exception("SubscribeStatistics"));
                    }
                }
            });
        }

        public void SubscribeTrades(string symbol, int limit, Action<TradeEventArgs> callback, Action<Exception> exception, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
