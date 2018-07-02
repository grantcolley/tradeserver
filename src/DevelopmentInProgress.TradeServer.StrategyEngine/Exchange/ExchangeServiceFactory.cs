namespace DevelopmentInProgress.TradeServer.StrategyEngine.Exchange
{
    public abstract class ExchangeServiceFactory<T> : IExchangeServiceFactory<T>
    {
        public abstract T GetExchangeService(MarketView.Interface.TradeStrategy.Exchange exchange);
    }
}
