using DevelopmentInProgress.TradeView.Core.TradeStrategy;

namespace DevelopmentInProgress.TradeServer.StrategyRunner.WebHost.Web.HostedService
{
    public class StrategyRunnerActionBlockInput
    {
        public Strategy Strategy { get; set; }
        public string DownloadsPath { get; set; }
        public IStrategyRunner StrategyRunner { get; set; }
    }
}
