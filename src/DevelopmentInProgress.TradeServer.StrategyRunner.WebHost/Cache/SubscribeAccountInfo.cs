using System;
using System.Threading;
using DevelopmentInProgress.TradeView.Interface.Events;
using DevelopmentInProgress.TradeView.Interface.Interfaces;
using DevelopmentInProgress.TradeView.Interface.Model;

namespace DevelopmentInProgress.TradeServer.StrategyRunner.WebHost.Cache
{
    public class SubscribeAccountInfo : SubscriptionBase<AccountInfoEventArgs>
    {
        public SubscribeAccountInfo(IExchangeApi exchangeApi)
            : base(exchangeApi)
        {
            User = new User();
        }

        public User User { get; private set; }

        public override void ExchangeSubscribe(Action<AccountInfoEventArgs> update, Action<Exception> exception, CancellationToken cancellationToken)
        {
            ExchangeApi.SubscribeAccountInfo(User, update, exception, cancellationToken);
        }
    }
}