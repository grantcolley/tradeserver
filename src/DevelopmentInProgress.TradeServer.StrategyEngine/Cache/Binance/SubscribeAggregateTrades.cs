using System;
using System.Threading;
using DevelopmentInProgress.MarketView.Interface.Events;
using DevelopmentInProgress.MarketView.Interface.Interfaces;

namespace DevelopmentInProgress.TradeServer.StrategyEngine.Cache.Binance
{
    public class SubscribeAggregateTrades : SubscriptionManager<AggregateTradeEventArgs>
    {
        public SubscribeAggregateTrades(string symbol, int limit, IExchangeService exchangeService)
            : base(symbol, limit, exchangeService)
        {
        }

        public override void ExchangeSubscribe(string symbol, int limit, Action<AggregateTradeEventArgs> update, Action<Exception> exception, CancellationToken cancellationToken)
        {
            ExchangeService.SubscribeAggregateTrades(Symbol, Limit, update, exception, cancellationToken);
        }
    }
}