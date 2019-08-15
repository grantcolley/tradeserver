using System;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace DevelopmentInProgress.TradeServer.StrategyRunner.WebHost.Web.HostedService
{
    public class StrategyRunnerActionBlock : IStrategyRunnerActionBlock
    {
        public ActionBlock<StrategyRunnerActionBlockInput> ActionBlock { get;set; }

        public async Task RunStrategyAsync(StrategyRunnerActionBlockInput strategyRunnerActionBlockInput)
        {
            if(ActionBlock == null)
            {
                throw new NullReferenceException("StrategyRunnerActionBlock.ActionBlock is null");
            }

            if(strategyRunnerActionBlockInput == null)
            {
                throw new ArgumentNullException("strategyRunnerActionBlockInput is null");
            }

            await ActionBlock.SendAsync(strategyRunnerActionBlockInput).ConfigureAwait(false);
        }
    }
}
