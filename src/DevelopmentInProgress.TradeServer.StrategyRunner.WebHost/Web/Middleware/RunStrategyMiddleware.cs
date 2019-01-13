using DevelopmentInProgress.MarketView.Interface.Strategy;
using DevelopmentInProgress.TradeServer.StrategyEngine.WebHost.Web.HostedService;
using DevelopmentInProgress.TradeServer.StrategyRunner.WebHost.Notification;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace DevelopmentInProgress.TradeServer.StrategyRunner.WebHost.Web.Middleware
{
    public class RunStrategyMiddleware
    {
        private readonly IBatchNotification<StrategyNotification> strategyRunnerLogger;

        public RunStrategyMiddleware(RequestDelegate next, IBatchNotificationFactory<StrategyNotification> batchNotificationFactory)
        {
            strategyRunnerLogger = batchNotificationFactory.GetBatchNotifier(BatchNotificationType.StrategyRunnerLogger);
        }

        public async Task Invoke(HttpContext context, IStrategyRunner strategyRunner, IBackgroundTaskQueue backgroundTaskQueue)
        {
            try
            {
                var json = context.Request.Form["strategy"];

                var strategy = JsonConvert.DeserializeObject<Strategy>(json);

                var downloadsPath = Path.Combine(Directory.GetCurrentDirectory(), "downloads", Guid.NewGuid().ToString());

                if (!Directory.Exists(downloadsPath))
                {
                    Directory.CreateDirectory(downloadsPath);
                }

                if (context.Request.HasFormContentType)
                {
                    var form = context.Request.Form;

                    var downloads = from f
                                    in form.Files
                                    select Download(f, downloadsPath);

                    await Task.WhenAll(downloads.ToArray());
                }

                backgroundTaskQueue.QueueBackgroundWorkItem(async token => 
                {
                    try
                    {

                        // TODO pass in cancellation token...
                        var response = await strategyRunner.RunAsync(strategy, downloadsPath);
                    }
                    catch(Exception ex)
                    {
                        var strategyNotification = strategy.GetNotification(NotificationLevel.Error, NotificationEventId.RunStrategyMiddleware, ex.ToString());
                        strategyRunnerLogger.AddNotification(strategyNotification);
                    }
                });
                
                await context.Response.WriteAsync(json);
            }
            catch (Exception ex)
            {
                var response = context.Response;
                response.ContentType = "application/json";
                response.StatusCode = (int)HttpStatusCode.InternalServerError;
                await response.WriteAsync(JsonConvert.SerializeObject(ex));
            }
        }

        private async Task Download(IFormFile formFile, string downloadsPath)
        {
            using (var fileStream = new FileStream(Path.Combine(downloadsPath, formFile.Name), FileMode.Create))
            {
                await formFile.CopyToAsync(fileStream);
            }
        }
    }
}
