using System;

namespace DevelopmentInProgress.TradeServer.StrategyEngine.Cache.Binance
{
    public interface ISubscriptionManager<T> : IDisposable
    {
        bool HasSubscriptions { get; }
        int Subscriptions { get; }
        void Subscribe(string strategyName, StrategyNotification<T> strategyNotification);
        void Unsubscribe(string strategyName, Action<Exception> exception);
    }
}
