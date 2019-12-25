using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using DevelopmentInProgress.TradeServer.StrategyRunner.WebHost.Web.Middleware;
using DevelopmentInProgress.TradeServer.StrategyRunner.WebHost.Notification.Publishing;
using DevelopmentInProgress.TradeServer.StrategyRunner.WebHost.Notification;
using DevelopmentInProgress.TradeServer.StrategyRunner.WebHost.Cache;
using DevelopmentInProgress.TradeView.Interface.Interfaces;
using DevelopmentInProgress.TradeView.Interface.Strategy;
using DipSocket.NetCore.Extensions;
using DevelopmentInProgress.TradeServer.StrategyRunner.WebHost.Web.HostedService;
using DevelopmentInProgress.TradeView.Service;

namespace DevelopmentInProgress.TradeServer.StrategyRunner.WebHost.Web
{
    public class Startup
    {
        public Startup(IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<IStrategyRunnerActionBlock, StrategyRunnerActionBlock>();
            services.AddSingleton<IStrategyRunner, StrategyRunner>();
            services.AddSingleton<INotificationPublisherContext, NotificationPublisherContext>();
            services.AddSingleton<INotificationPublisher, NotificationPublisher>();
            services.AddSingleton<IBatchNotificationFactory<StrategyNotification>, StrategyBatchNotificationFactory>();
            services.AddSingleton<IExchangeService, ExchangeService>();
            services.AddSingleton<IExchangeSubscriptionsCacheFactory, ExchangeSubscriptionsCacheFactory>();
            services.AddSingleton<ISubscriptionsCacheManager, SubscriptionsCacheManager>();
            services.AddSingleton<ITradeStrategyCacheManager, TradeStrategyCacheManager>();

            services.AddHostedService<StrategyRunnerBackgroundService>();
            services.AddDipSocket<NotificationHub>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            app.UseDipSocket<NotificationHub>("/notificationhub");

            app.Map("/runstrategy", HandleRun);
            app.Map("/updatestrategy", HandleUpdate);
            app.Map("/stopstrategy", HandleStop);
            app.Map("/isstrategyrunning", HandleIsStrategyRunning);
            app.Map("/ping", HandlePing);
        }

        private static void HandleRun(IApplicationBuilder app)
        {
            app.UseRunStrategyMiddleware();
        }

        private static void HandleUpdate(IApplicationBuilder app)
        {
            app.UseUpdateStrategyMiddleware();
        }

        private static void HandleIsStrategyRunning(IApplicationBuilder app)
        {
            app.UseIsStrategyRunningMiddleware();
        }

        private static void HandleStop(IApplicationBuilder app)
        {
            app.UseStopStrategyMiddleware();
        }

        private static void HandlePing(IApplicationBuilder app)
        {
            app.UsePingMiddleware();
        }
    }
}
