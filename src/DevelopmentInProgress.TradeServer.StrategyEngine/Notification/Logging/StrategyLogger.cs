using DevelopmentInProgress.MarketView.Interface.TradeStrategy;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DevelopmentInProgress.TradeServer.StrategyEngine.Notification.Logging
{
    public class StrategyLogger : BatchNotification<IEnumerable<StrategyNotification>>, IBatchNotification<IEnumerable<StrategyNotification>>
    {
        private readonly ILogger logger;

        public StrategyLogger(ILoggerFactory loggerFactory)
        {
            logger = loggerFactory.CreateLogger<StrategyLogger>();

            Start();
        }

        public override async Task NotifyAsync(IEnumerable<IEnumerable<StrategyNotification>> notifications, CancellationToken cancellationToken)
        {
            var flattenedList = notifications.SelectMany(n => n).OrderBy(n => n.Timestamp).ToList();
            foreach (var strategyNotification in flattenedList)
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
