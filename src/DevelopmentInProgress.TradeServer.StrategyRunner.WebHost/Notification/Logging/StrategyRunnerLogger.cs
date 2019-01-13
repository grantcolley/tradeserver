using DevelopmentInProgress.MarketView.Interface.Strategy;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DevelopmentInProgress.TradeServer.StrategyRunner.WebHost.Notification.Logging
{
    public class StrategyRunnerLogger : BatchNotification<StrategyNotification>, IBatchNotification<StrategyNotification>
    {
        private readonly ILogger logger;

        public StrategyRunnerLogger(ILoggerFactory loggerFactory)
        {
            logger = loggerFactory.CreateLogger<StrategyRunnerLogger>();

            Start();
        }

        public override async Task NotifyAsync(IEnumerable<StrategyNotification> notifications, CancellationToken cancellationToken)
        {
            var strategyNotifications = notifications.OrderBy(n => n.Timestamp).ToList();
            foreach (var strategyNotification in strategyNotifications)
            {
                logger.Log<StrategyNotification>(GetStepNotificationLogLevel(strategyNotification), strategyNotification.NotificationEvent, strategyNotification, null, null);
            }
        }

        private LogLevel GetStepNotificationLogLevel(StrategyNotification strategyNotification)
        {
            switch (strategyNotification.NotificationLevel)
            {
                case NotificationLevel.Debug:
                    return LogLevel.Debug;
                case NotificationLevel.Information:
                    return LogLevel.Information;
                case NotificationLevel.Warning:
                    return LogLevel.Warning;
                case NotificationLevel.Error:
                    return LogLevel.Error;
            }

            return LogLevel.Information;
        }
    }
}
