using System;
using System.Threading;
using DevelopmentInProgress.MarketView.Interface.Events;
using DevelopmentInProgress.MarketView.Interface.Interfaces;
using DevelopmentInProgress.MarketView.Interface.Model;

namespace DevelopmentInProgress.TradeServer.StrategyEngine.Cache.Binance
{
    public class SubscribeAccountInfo : SubscriptionManager<AccountInfoEventArgs>
    {
        public SubscribeAccountInfo(IExchangeService exchangeService)
            : base(exchangeService)
        {
        }

        public override void ExchangeSubscribe(Action<AccountInfoEventArgs> update, Action<Exception> exception, CancellationToken cancellationToken)
        {
            var user = new User();
            ExchangeService.SubscribeAccountInfo(user, update, exception, cancellationToken);
        }
    }
}