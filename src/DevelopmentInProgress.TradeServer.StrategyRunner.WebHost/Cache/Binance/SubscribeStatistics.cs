//using System;
//using System.Threading;
//using DevelopmentInProgress.TradeView.Interface.Events;
//using DevelopmentInProgress.TradeView.Interface.Interfaces;

//namespace DevelopmentInProgress.TradeServer.StrategyRunner.WebHost.Cache.Binance
//{
//    public class SubscribeStatistics : SubscriptionManager<StatisticsEventArgs>
//    {
//        public SubscribeStatistics(IExchangeApi exchangeApi)
//            : base(exchangeApi)
//        {
//        }

//        public override void ExchangeSubscribe(Action<StatisticsEventArgs> update, Action<Exception> exception, CancellationToken cancellationToken)
//        {
//            ExchangeService.SubscribeStatistics(update, exception, cancellationToken);
//        }
//    }
//}