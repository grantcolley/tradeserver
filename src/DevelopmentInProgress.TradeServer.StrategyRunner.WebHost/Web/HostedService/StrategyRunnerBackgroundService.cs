using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace DevelopmentInProgress.TradeServer.StrategyRunner.WebHost.Web.HostedService
{
    public class StrategyRunnerBackgroundService : BackgroundService
    {
        private readonly ILogger logger;
        private IStrategyRunnerActionBlock strategyRunnerActionBlock;
        private CancellationToken cancellationToken;

        public StrategyRunnerBackgroundService(IStrategyRunnerActionBlock strategyRunnerActionBlock, ILoggerFactory loggerFactory)
        {
            this.strategyRunnerActionBlock = strategyRunnerActionBlock;

            logger = loggerFactory.CreateLogger<StrategyRunnerBackgroundService>();
        }

        protected async override Task ExecuteAsync(CancellationToken cancellationToken)
        {
            this.cancellationToken = cancellationToken;

            logger.LogInformation("ExecuteAsync");

            try
            {
                strategyRunnerActionBlock.ActionBlock = new ActionBlock<StrategyRunnerActionBlockInput>(async actionBlockInput =>
                {
                    await actionBlockInput.StrategyRunner.RunAsync(actionBlockInput.Strategy, actionBlockInput.DownloadsPath, actionBlockInput.CancellationToken).ConfigureAwait(false);
                },
                new ExecutionDataflowBlockOptions { MaxDegreeOfParallelism = 5 });

                while (!cancellationToken.IsCancellationRequested)
                {
                    await Task.Delay(1000);
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "ExecuteAsync");
            }
        }
    }
}