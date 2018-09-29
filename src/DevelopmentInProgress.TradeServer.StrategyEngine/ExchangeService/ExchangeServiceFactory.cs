﻿namespace DevelopmentInProgress.TradeServer.StrategyEngine.ExchangeService
{
    public abstract class ExchangeServiceFactory<T> : IExchangeServiceFactory<T>
    {
        public abstract T GetExchangeService(MarketView.Interface.Strategy.Exchange exchange);
    }
}
