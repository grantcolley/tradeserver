using DevelopmentInProgress.TradeView.Interface.Strategy;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DevelopmentInProgress.TradeServer.StrategyRunner.WebHost.Notification.Strategy
{
    public class StrategyBatchLogger : BatchNotification<StrategyNotification>, IBatchNotification<StrategyNotification>
    {
        private readonly ILogger logger;

        public StrategyBatchLogger(ILoggerFactory loggerFactory)
        {
            logger = loggerFactory.CreateLogger<StrategyBatchLogger>();

            Start();
        }

        public override Task NotifyAsync(IEnumerable<StrategyNotification> notifications, CancellationToken cancellationToken)
        {
            var tcs = new TaskCompletionSource<object>();

            try
            {
                if (!cancellationToken.IsCancellationRequested)
                {
                    var strategyNotifications = notifications.OrderBy(n => n.Timestamp).ToList();

                    foreach (var strategyNotification in strategyNotifications)
                    {
                        logger.Log<StrategyNotification>(GetStepNotificationLogLevel(strategyNotification), strategyNotification.NotificationEvent, strategyNotification, null, null);
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
