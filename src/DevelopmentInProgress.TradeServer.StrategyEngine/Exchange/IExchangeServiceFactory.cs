namespace DevelopmentInProgress.TradeServer.StrategyEngine.Exchange
{
    public interface IExchangeServiceFactory<T>
    {
        T GetExchangeService(MarketView.Interface.TradeStrategy.Exchange exchange);
    }
}