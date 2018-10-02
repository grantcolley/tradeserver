namespace DevelopmentInProgress.TradeServer.StrategyRunner.WebHost.Notification
{
    public interface IBatchNotification<T>
    {
        void AddNotification(T item);
    }
}
