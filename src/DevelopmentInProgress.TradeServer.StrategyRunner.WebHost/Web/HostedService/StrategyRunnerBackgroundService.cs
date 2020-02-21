using DevelopmentInProgress.TradeView.Interface.Server;
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
        private readonly IServer server;
        private readonly ILogger logger;
        private readonly IStrategyRunnerActionBlock strategyRunnerActionBlock;
        private CancellationToken cancellationToken;

        public StrategyRunnerBackgroundService(IServer server, IStrategyRunnerActionBlock strategyRunnerActionBlock, ILoggerFactory loggerFactory)
        {
            this.server = server;
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
                    await actionBlockInput.StrategyRunner.RunAsync(actionBlockInput.Strategy, actionBlockInput.DownloadsPath, this.cancellationToken).ConfigureAwait(false);
                },
                new ExecutionDataflowBlockOptions { MaxDegreeOfParallelism = server.MaxDegreeOfParallelism });

                while (!this.cancellationToken.IsCancellationRequested)
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