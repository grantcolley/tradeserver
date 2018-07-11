using System;
using System.Threading;
using DevelopmentInProgress.MarketView.Interface.Events;
using DevelopmentInProgress.MarketView.Interface.Interfaces;

namespace DevelopmentInProgress.TradeServer.StrategyEngine.Cache.Binance
{
    public class SubscribeOrderBook : SubscriptionManager<OrderBookEventArgs>
    {
        public SubscribeOrderBook(string symbol, int limit, IExchangeService exchangeService)
            : base(exchangeService)
        {
            Symbol = symbol;
            Limit = limit;
        }

        public string Symbol { get; private set; }

        public int Limit { get; private set; }

        public override void ExchangeSubscribe(Action<OrderBookEventArgs> update, Action<Exception> exception, CancellationToken cancellationToken)
        {
            ExchangeService.SubscribeOrderBook(Symbol, Limit, update, exception, cancellationToken);
        }
    }
}