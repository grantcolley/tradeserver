namespace DevelopmentInProgress.TradeServer.StrategyRunner.WebHost.Notification
{
    public interface IBatchNotificationFactory<T>
    {
        IBatchNotification<T> GetBatchNotifier(BatchNotificationType batchNotifierType);
    }
}