using DevelopmentInProgress.TradeView.Interface.Enums;
using DevelopmentInProgress.TradeView.Interface.Interfaces;
using System;
using System.Collections.Generic;

namespace DevelopmentInProgress.MarketView.StrategyRunner.Test.Helpers
{
    public class TestExchangeApiFactory : IExchangeApiFactory
    {
        public IExchangeApi GetExchangeApi(Exchange exchange)
        {
            switch (exchange)
            {
                case Exchange.Binance:
                    return new TestBinanceExchangeApi();
                case Exchange.Test:
                    return new TestExchangeApi();
                default:
                    throw new NotImplementedException();
            }
        }

        public Dictionary<Exchange, IExchangeApi> GetExchanges()
        {
            var exchanges = new Dictionary<Exchange, IExchangeApi>();
            exchanges.Add(Exchange.Binance, GetExchangeApi(Exchange.Binance));
            exchanges.Add(Exchange.Test, GetExchangeApi(Exchange.Test));
            return exchanges;
        }
    }
}
