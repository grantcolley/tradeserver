using DevelopmentInProgress.TradeView.Core.Server;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace DevelopmentInProgress.TradeServer.StrategyRunner.WebHost.Notification.Server
{
    public class ServerBatchNotificationPublisher : BatchNotification<ServerNotification>, IBatchNotification<ServerNotification>
    {
        private IServerNotificationPublisher serverNotificationPublisher;

        public ServerBatchNotificationPublisher(IServerNotificationPublisher serverNotificationPublisher)
        {
            this.serverNotificationPublisher = serverNotificationPublisher;

            Start();
        }

        public override async Task NotifyAsync(IEnumerable<ServerNotification> items, CancellationToken cancellationToken)
        {
            if(cancellationToken.IsCancellationRequested)
            {
                return;
            }

            await serverNotificationPublisher.PublishNotificationsAsync(items).ConfigureAwait(false);
        }
    }
}
