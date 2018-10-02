using DevelopmentInProgress.MarketView.Api.Binance;
using DevelopmentInProgress.MarketView.Interface.Interfaces;
using System.Collections.Generic;

namespace DevelopmentInProgress.TradeServer.StrategyRunner.WebHost.ExchangeService
{
    public class StrategyExchangeServiceFactory : ExchangeServiceFactory<IExchangeService>
    {
        private readonly Dictionary<MarketView.Interface.Strategy.Exchange, IExchangeService> exchangesServices;

        public StrategyExchangeServiceFactory()
        {
            exchangesServices = new Dictionary<MarketView.Interface.Strategy.Exchange, IExchangeService>();
            exchangesServices.Add(MarketView.Interface.Strategy.Exchange.Binance, new MarketView.Service.ExchangeService(new BinanceExchangeApi()));
        }

        public override IExchangeService GetExchangeService(MarketView.Interface.Strategy.Exchange exchange)
        {
            return exchangesServices[exchange];
        }
    }
}
