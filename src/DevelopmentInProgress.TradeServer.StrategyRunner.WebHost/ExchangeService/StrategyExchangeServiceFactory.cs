//using DevelopmentInProgress.TradeView.Api.Binance;
//using DevelopmentInProgress.TradeView.Interface.Enums;
//using DevelopmentInProgress.TradeView.Interface.Interfaces;
//using System.Collections.Generic;

//namespace DevelopmentInProgress.TradeServer.StrategyRunner.WebHost.ExchangeService
//{
//    public class StrategyExchangeServiceFactory : ExchangeServiceFactory<IExchangeService>
//    {
//        private readonly Dictionary<Exchange, IExchangeService> exchangesServices;

//        public StrategyExchangeServiceFactory()
//        {
//            exchangesServices = new Dictionary<Exchange, IExchangeService>();
//            exchangesServices.Add(Exchange.Binance, new ExchangeService(new BinanceExchangeApi()));
//        }

//        public override IExchangeService GetExchangeService(Exchange exchange)
//        {
//            return exchangesServices[exchange];
//        }
//    }
//}
