using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using DevelopmentInProgress.TradeServer.StrategyRunner.WebHost.Web.Middleware;
using DevelopmentInProgress.TradeServer.StrategyRunner.WebHost.Notification.Publishing;
using DevelopmentInProgress.TradeServer.StrategyRunner.WebHost.Notification;
using DevelopmentInProgress.TradeServer.StrategyRunner.WebHost.ExchangeService;
using DevelopmentInProgress.MarketView.Interface.Interfaces;
using DevelopmentInProgress.MarketView.Interface.Strategy;
using DevelopmentInProgress.TradeServer.StrategyRunner.WebHost.Cache;

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
            services.AddSignalR();
            services.AddSingleton<IStrategyRunner, StrategyRunner>();
            services.AddSingleton<INotificationPublisherContext, NotificationPublisherContext>();
            services.AddSingleton<INotificationPublisher, NotificationPublisher>();
            services.AddSingleton<IBatchNotificationFactory<StrategyNotification>, StrategyBatchNotificationFactory>();
            services.AddSingleton<IExchangeServiceFactory<IExchangeService>, StrategyExchangeServiceFactory>();
            services.AddSingleton<ISubscriptionsCacheFactory, SubscriptionsCacheFactory>();
            services.AddSingleton<ISubscriptionsCacheManager, SubscriptionsCacheManager>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            app.UseSignalR(routes => { routes.MapHub<NotificationHub>("/notificationhub"); });

            app.Map("/runstrategy", HandleRun);
            app.Map("/ping", HandlePing);
        }

        private static void HandleRun(IApplicationBuilder app)
        {
            app.UseRunStrategyMiddleware();
        }

        private static void HandlePing(IApplicationBuilder app)
        {
            app.UsePingMiddleware();
        }
    }
}
