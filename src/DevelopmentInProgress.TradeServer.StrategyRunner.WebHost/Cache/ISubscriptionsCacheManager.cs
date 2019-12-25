using DevelopmentInProgress.TradeView.Interface.Strategy;
using System;
using System.Threading.Tasks;

namespace DevelopmentInProgress.TradeServer.StrategyRunner.WebHost.Cache
{
    public interface ISubscriptionsCacheManager : IDisposable
    {
        IExchangeSubscriptionsCacheFactory ExchangeSubscriptionsCacheFactory { get; }
        Task Subscribe(Strategy strategy, ITradeStrategy tradeStrategy);
        void Unsubscribe(Strategy strategy, ITradeStrategy tradeStrategy);
    }
}