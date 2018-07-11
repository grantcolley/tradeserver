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
            User = new User();
        }

        public User User { get; private set; }

        public override void ExchangeSubscribe(Action<AccountInfoEventArgs> update, Action<Exception> exception, CancellationToken cancellationToken)
        {
            ExchangeService.SubscribeAccountInfo(User, update, exception, cancellationToken);
        }
    }
}