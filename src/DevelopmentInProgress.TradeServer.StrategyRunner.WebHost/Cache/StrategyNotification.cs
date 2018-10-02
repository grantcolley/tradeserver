using System;

namespace DevelopmentInProgress.TradeServer.StrategyRunner.WebHost.Cache
{
    public class StrategyNotification<T>
    {
        public Action<T> Update { get; set; }
        public Action<Exception> Exception { get; set; }
    }
}