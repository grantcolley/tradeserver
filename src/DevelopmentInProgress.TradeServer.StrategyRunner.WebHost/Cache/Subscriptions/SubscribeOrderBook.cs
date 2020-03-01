using System;
using System.Threading;
using DevelopmentInProgress.TradeView.Interface.Events;
using DevelopmentInProgress.TradeView.Interface.Interfaces;

namespace DevelopmentInProgress.TradeServer.StrategyRunner.WebHost.Cache.Subscriptions
{
    public class SubscribeOrderBook : SymbolSubscriptionBase<OrderBookEventArgs>
    {
        public SubscribeOrderBook(string symbol, int limit, IExchangeApi exchangeApi)
            : base(symbol, limit, exchangeApi)
        {
        }

        public override void ExchangeSubscribe(Action<OrderBookEventArgs> update, Action<Exception> exception, CancellationToken cancellationToken)
        {
            ExchangeApi.SubscribeOrderBook(Symbol, Limit, update, exception, cancellationToken);
        }
    }
}