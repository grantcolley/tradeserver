using DevelopmentInProgress.MarketView.Interface.Interfaces;
using DevelopmentInProgress.MarketView.Interface.TradeStrategy;
using System;

namespace DevelopmentInProgress.TradeServer.StrategyEngine.Cache
{
    public class SymbolCache : ISymbolCache
    {
        private readonly IExchangeService exchangeService;

        public SymbolCache(string symbol, IExchangeService exchangeService)
        {
            this.exchangeService = exchangeService;

            Symbol = symbol;
        }

        public string Symbol { get; private set; }

        public bool HasSubscriptions
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public void Subscribe(ITradeStrategy tradeStrategy)
        {

        }

        public void Unsubscribe(ITradeStrategy tradeStrategy)
        {

        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        //void SubscribeStatistics(Action<StatisticsEventArgs> callback, Action<Exception> exception);
        //void SubscribeOrderBook(string symbol, int limit, Action<OrderBookEventArgs> callback, Action<Exception> exception, CancellationToken cancellationToken);
        //void SubscribeAggregateTrades(string symbol, int limit, Action<AggregateTradeEventArgs> callback, Action<Exception> exception, CancellationToken cancellationToken);
        //void SubscribeAccountInfo(Interface.Model.User user, Action<AccountInfoEventArgs> callback, Action<Exception> exception, CancellationToken cancellationToken);
    }
}
