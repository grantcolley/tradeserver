namespace DevelopmentInProgress.TradeServer.StrategyRunner.WebHost.Notification
{
    public abstract class BatchNotificationFactory<T> : IBatchNotificationFactory<T>
    {
        public abstract IBatchNotification<T> GetBatchNotifier(BatchNotificationType batchNotifierType);
    }
}
