using DevelopmentInProgress.TradeView.Interface.Strategy;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
namespace DevelopmentInProgress.TradeServer.StrategyRunner.WebHost.Notification.Strategy
{
    public class StrategyBatchParameterUpdatePublisher : BatchNotification<StrategyNotification>, IBatchNotification<StrategyNotification>
    {
        private readonly IStrategyNotificationPublisher notificationPublisher;

        public StrategyBatchParameterUpdatePublisher(IStrategyNotificationPublisher notificationPublisher)
        {
            this.notificationPublisher = notificationPublisher;

            Start();
        }

        public override async Task NotifyAsync(IEnumerable<StrategyNotification> notifications, CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                return;
            }

            await notificationPublisher.PublishParameterUpdateAsync(notifications);
        }
    }
}
