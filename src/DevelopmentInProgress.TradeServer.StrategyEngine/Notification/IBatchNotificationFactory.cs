namespace DevelopmentInProgress.TradeServer.StrategyEngine.Notification
{
    public interface IBatchNotificationFactory<T>
    {
        IBatchNotification<T> GetBatchNotifier(BatchNotificationType batchNotifierType);
    }
}