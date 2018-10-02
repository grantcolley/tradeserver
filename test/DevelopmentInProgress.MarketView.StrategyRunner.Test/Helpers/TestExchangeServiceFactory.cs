using DevelopmentInProgress.MarketView.Interface.Interfaces;
using DevelopmentInProgress.MarketView.Interface.Strategy;
using DevelopmentInProgress.TradeServer.StrategyRunner.WebHost.ExchangeService;
using System.Collections.Generic;

namespace DevelopmentInProgress.MarketView.StrategyRunner.Test.Helpers
{
    public class TestExchangeServiceFactory : ExchangeServiceFactory<IExchangeService>
    {
        private readonly Dictionary<Exchange, IExchangeService> exchangesServices;

        public TestExchangeServiceFactory()
        {
            exchangesServices = new Dictionary<Exchange, IExchangeService>();
            exchangesServices.Add(Exchange.Binance, new TestBinanceExchangeService());
            exchangesServices.Add(Exchange.Test, new TestExchangeService());
        }

        public override IExchangeService GetExchangeService(Exchange exchange)
        {
            return exchangesServices[exchange];
        }
    }
}
