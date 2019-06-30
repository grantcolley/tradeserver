using System;
using System.Threading;
using DevelopmentInProgress.MarketView.Interface.Model;
using DevelopmentInProgress.MarketView.Interface.Events;
using DevelopmentInProgress.MarketView.Interface.Interfaces;

namespace DevelopmentInProgress.TradeServer.StrategyRunner.WebHost.Cache.Binance
{
    public class SubscribeCandlesticks : SubscriptionManager<CandlestickEventArgs>
    {
        public SubscribeCandlesticks(string symbol, CandlestickInterval candlestickInterval, int limit, IExchangeService exchangeService)
            : base(exchangeService)
        {
            Symbol = symbol;
            Limit = limit;
            CandlestickInterval = candlestickInterval;
        }

        public string Symbol { get; private set; }

        public int Limit { get; private set; }

        public CandlestickInterval CandlestickInterval { get; private set; }

        public override void ExchangeSubscribe(Action<CandlestickEventArgs> update, Action<Exception> exception, CancellationToken cancellationToken)
        {
            ExchangeService.SubscribeCandlesticks(Symbol, CandlestickInterval, Limit, update, exception, cancellationToken);
        }
    }
}