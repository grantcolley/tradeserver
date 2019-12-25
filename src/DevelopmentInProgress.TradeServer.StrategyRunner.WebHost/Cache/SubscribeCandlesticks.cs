using System;
using System.Threading;
using DevelopmentInProgress.TradeView.Interface.Model;
using DevelopmentInProgress.TradeView.Interface.Events;
using DevelopmentInProgress.TradeView.Interface.Interfaces;

namespace DevelopmentInProgress.TradeServer.StrategyRunner.WebHost.Cache
{
    public class SubscribeCandlesticks : SymbolSubscriptionBase<CandlestickEventArgs>
    {
        public SubscribeCandlesticks(string symbol, int limit, IExchangeApi exchangeApi, CandlestickInterval candlestickInterval)
            : base(symbol, limit, exchangeApi)
        {
            CandlestickInterval = candlestickInterval;
        }

        public CandlestickInterval CandlestickInterval { get; private set; }

        public override void ExchangeSubscribe(Action<CandlestickEventArgs> update, Action<Exception> exception, CancellationToken cancellationToken)
        {
            ExchangeApi.SubscribeCandlesticks(Symbol, CandlestickInterval, Limit, update, exception, cancellationToken);
        }
    }
}