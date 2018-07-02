namespace DevelopmentInProgress.TradeServer.StrategyEngine.Notification
{
    public interface IBatchNotification<T>
    {
        void AddNotification(T item);
    }
}
