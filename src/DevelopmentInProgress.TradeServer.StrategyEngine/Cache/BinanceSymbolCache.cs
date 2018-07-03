using DevelopmentInProgress.MarketView.Interface.Interfaces;
using DevelopmentInProgress.MarketView.Interface.TradeStrategy;

namespace DevelopmentInProgress.TradeServer.StrategyEngine.Cache
{
    public class BinanceSymbolCache : ISymbolCache
    {
        public BinanceSymbolCache(string symbol, IExchangeService exchangeService)
        {
            Symbol = symbol;
            ExchangeService = exchangeService;
        }

        public string Symbol { get; private set; }

        public IExchangeService ExchangeService { get; private set; }

        public bool HasSubscriptions => throw new System.NotImplementedException();
        
        public void Dispose()
        {
            throw new System.NotImplementedException();
        }

        public void Subscribe(string strategyName, StrategySymbol strategySymbol, ITradeStrategy tradeStrategy)
        {
            throw new System.NotImplementedException();
        }

        public void Unsubscribe(string strategyName, StrategySymbol strategySymbol, ITradeStrategy tradeStrategy)
        {
            throw new System.NotImplementedException();
        }
        
        //void SubscribeStatistics(Action<StatisticsEventArgs> callback, Action<Exception> exception);
        //void SubscribeOrderBook(string symbol, int limit, Action<OrderBookEventArgs> callback, Action<Exception> exception, CancellationToken cancellationToken);
        //void SubscribeAggregateTrades(string symbol, int limit, Action<AggregateTradeEventArgs> callback, Action<Exception> exception, CancellationToken cancellationToken);
        //void SubscribeAccountInfo(Interface.Model.User user, Action<AccountInfoEventArgs> callback, Action<Exception> exception, CancellationToken cancellationToken);
    }
}