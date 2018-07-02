using DevelopmentInProgress.MarketView.Api.Binance;
using DevelopmentInProgress.MarketView.Interface.Interfaces;
using DevelopmentInProgress.MarketView.Service;
using System.Collections.Generic;

namespace DevelopmentInProgress.TradeServer.StrategyEngine.Exchange
{
    public class StrategyExchangeServiceFactory : ExchangeServiceFactory<IExchangeService>
    {
        private readonly Dictionary<MarketView.Interface.TradeStrategy.Exchange, IExchangeService> exchangesServices;

        public StrategyExchangeServiceFactory()
        {
            exchangesServices = new Dictionary<MarketView.Interface.TradeStrategy.Exchange, IExchangeService>();
            exchangesServices.Add(MarketView.Interface.TradeStrategy.Exchange.Binance, new ExchangeService(new BinanceExchangeApi()));
        }

        public override IExchangeService GetExchangeService(MarketView.Interface.TradeStrategy.Exchange exchange)
        {
            return exchangesServices[exchange];
        }
    }
}
