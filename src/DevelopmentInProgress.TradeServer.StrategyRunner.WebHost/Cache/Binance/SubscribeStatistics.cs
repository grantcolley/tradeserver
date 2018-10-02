using System;
using System.Threading;
using DevelopmentInProgress.MarketView.Interface.Events;
using DevelopmentInProgress.MarketView.Interface.Interfaces;

namespace DevelopmentInProgress.TradeServer.StrategyRunner.WebHost.Cache.Binance
{
    public class SubscribeStatistics : SubscriptionManager<StatisticsEventArgs>
    {
        public SubscribeStatistics(IExchangeService exchangeService)
            : base(exchangeService)
        {
        }

        public override void ExchangeSubscribe(Action<StatisticsEventArgs> update, Action<Exception> exception, CancellationToken cancellationToken)
        {
            ExchangeService.SubscribeStatistics(update, exception, cancellationToken);
        }
    }
}