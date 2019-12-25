using System;
using System.Threading;
using DevelopmentInProgress.TradeView.Interface.Events;
using DevelopmentInProgress.TradeView.Interface.Interfaces;

namespace DevelopmentInProgress.TradeServer.StrategyRunner.WebHost.Cache
{
    public class SubscribeTrades : SymbolSubscriptionBase<TradeEventArgs>
    {
        public SubscribeTrades(string symbol, int limit, IExchangeApi exchangeApi)
            : base(symbol, limit, exchangeApi)
        {
        }

        public override void ExchangeSubscribe(Action<TradeEventArgs> update, Action<Exception> exception, CancellationToken cancellationToken)
        {
            ExchangeApi.SubscribeAggregateTrades(Symbol, Limit, update, exception, cancellationToken);
        }
    }
}