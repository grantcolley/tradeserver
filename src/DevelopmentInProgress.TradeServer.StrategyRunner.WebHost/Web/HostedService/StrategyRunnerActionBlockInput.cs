using DevelopmentInProgress.TradeView.Interface.Strategy;

namespace DevelopmentInProgress.TradeServer.StrategyRunner.WebHost.Web.HostedService
{
    public class StrategyRunnerActionBlockInput
    {
        public Strategy Strategy { get; set; }
        public string DownloadsPath { get; set; }
        public IStrategyRunner StrategyRunner { get; set; }
    }
}
