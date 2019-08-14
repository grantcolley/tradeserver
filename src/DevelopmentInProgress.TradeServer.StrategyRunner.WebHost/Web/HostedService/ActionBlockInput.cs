using DevelopmentInProgress.MarketView.Interface.Strategy;
using System.Threading;

namespace DevelopmentInProgress.TradeServer.StrategyRunner.WebHost.Web.HostedService
{
    public class ActionBlockInput
    {
        public Strategy Strategy { get; set; }
        public string DownloadsPath { get; set; }
        public CancellationToken CancellationToken { get; set; }
        public IStrategyRunner StrategyRunner { get; set; }
    }
}
