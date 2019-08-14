using DevelopmentInProgress.MarketView.Interface.Strategy;
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
        private ActionBlock<ActionBlockInput> strategyRunnerActionBlock;
        private CancellationToken cancellationToken;

        public StrategyRunnerBackgroundService(ILoggerFactory loggerFactory)
        {
            logger = loggerFactory.CreateLogger<StrategyRunnerBackgroundService>();
        }

        protected async override Task ExecuteAsync(CancellationToken cancellationToken)
        {
            this.cancellationToken = cancellationToken;

            logger.LogInformation("ExecuteAsync");

            try
            {
                strategyRunnerActionBlock = new ActionBlock<ActionBlockInput>(async actionBlockInput =>
                {
                    await actionBlockInput.StrategyRunner.RunAsync(actionBlockInput.Strategy, actionBlockInput.DownloadsPath, actionBlockInput.CancellationToken).ConfigureAwait(false);
                });

                while(!cancellationToken.IsCancellationRequested)
                {
                    await Task.Delay(1000);
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "ExecuteAsync");
            }
        }

        public async Task RunStrategyAsync(IStrategyRunner strategyRunner, Strategy strategy, string downloadsPath)
        {
            var actionBlockInput = new ActionBlockInput
            {
                StrategyRunner = strategyRunner,
                Strategy = strategy,
                DownloadsPath = downloadsPath,
                CancellationToken = cancellationToken
            };

            await strategyRunnerActionBlock.SendAsync(actionBlockInput).ConfigureAwait(false);
        }
    }
}