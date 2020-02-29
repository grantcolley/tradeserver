using DevelopmentInProgress.TradeView.Common.Extensions;
using DevelopmentInProgress.TradeView.Interface.Strategy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DevelopmentInProgress.TradeServer.StrategyRunner.WebHost.Notification.Strategy
{
    public class StrategyBatchCustomNotificationPublisher : BatchNotification<StrategyNotification>, IBatchNotification<StrategyNotification>
    {
        private readonly IStrategyNotificationPublisher notificationPublisher;

        public StrategyBatchCustomNotificationPublisher(IStrategyNotificationPublisher notificationPublisher)
        {
            this.notificationPublisher = notificationPublisher;

            Start();
        }

        public override Task NotifyAsync(IEnumerable<StrategyNotification> notifications, CancellationToken cancellationToken)
        {
            var tcs = new TaskCompletionSource<object>();

            try
            {
                if (!cancellationToken.IsCancellationRequested)
                {
                    var methodNameGroups = notifications.GroupBy(n => n.MethodName, n => n).ToList();
                    foreach (var methodNameGroup in methodNameGroups)
                    {
                        notificationPublisher.PublishCustomNotificationsAsync(methodNameGroup.Key, notifications).FireAndForget();
                    }
                }

                tcs.SetResult(null);
            }
            catch (Exception ex)
            {
                tcs.SetException(ex);
            }

            return tcs.Task;
        }
    }
}