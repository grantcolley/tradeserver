using DevelopmentInProgress.MarketView.Interface.Strategy;
using DevelopmentInProgress.TradeServer.StrategyRunner.WebHost.Extensions;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DevelopmentInProgress.TradeServer.StrategyRunner.WebHost.Notification.Publishing
{
    public class StrategyCustomNotificationPublisher : BatchNotification<StrategyNotification>, IBatchNotification<StrategyNotification>
    {
        private readonly INotificationPublisher notificationPublisher;

        public StrategyCustomNotificationPublisher(INotificationPublisher notificationPublisher)
        {
            this.notificationPublisher = notificationPublisher;

            Start();
        }

        public override async Task NotifyAsync(IEnumerable<StrategyNotification> notifications, CancellationToken cancellationToken)
        {
            var methodNameGroups = notifications.GroupBy(n => n.MethodName, n => n).ToList();
            foreach(var methodNameGroup in methodNameGroups)
            {
                notificationPublisher.PublishCustomNotificationsAsync(methodNameGroup.Key, notifications).FireAndForget();
            }
        }
    }
}