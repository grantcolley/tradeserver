using System;
using System.Threading;
using DevelopmentInProgress.MarketView.Interface.Events;
using DevelopmentInProgress.MarketView.Interface.Interfaces;

namespace DevelopmentInProgress.TradeServer.StrategyRunner.WebHost.Cache.Binance
{
    public class SubscribeTrades : SubscriptionManager<TradeEventArgs>
    {
        public SubscribeTrades(string symbol, int limit, IExchangeService exchangeService)
            : base(exchangeService)
        {
            Symbol = symbol;
            Limit = limit;
        }

        public string Symbol { get; private set; }

        public int Limit { get; private set; }

        public override void ExchangeSubscribe(Action<TradeEventArgs> update, Action<Exception> exception, CancellationToken cancellationToken)
        {
            ExchangeService.SubscribeAggregateTrades(Symbol, Limit, update, exception, cancellationToken);
        }
    }
}