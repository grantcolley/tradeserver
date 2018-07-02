﻿namespace DevelopmentInProgress.TradeServer.StrategyEngine.ExchangeService
{
    public interface IExchangeServiceFactory<T>
    {
        T GetExchangeService(MarketView.Interface.TradeStrategy.Exchange exchange);
    }
}