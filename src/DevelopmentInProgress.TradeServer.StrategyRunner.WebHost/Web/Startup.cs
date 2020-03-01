﻿using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using DevelopmentInProgress.TradeServer.StrategyRunner.WebHost.Cache.Subscriptions;
using DevelopmentInProgress.TradeServer.StrategyRunner.WebHost.Cache.TradeStrategy;
using DevelopmentInProgress.TradeServer.StrategyRunner.WebHost.Notification;
using DevelopmentInProgress.TradeServer.StrategyRunner.WebHost.Notification.Strategy;
using DevelopmentInProgress.TradeServer.StrategyRunner.WebHost.Notification.Server;
using DevelopmentInProgress.TradeServer.StrategyRunner.WebHost.Web.HostedService;
using DevelopmentInProgress.TradeServer.StrategyRunner.WebHost.Web.Middleware;
using DevelopmentInProgress.TradeView.Interface.Interfaces;
using DevelopmentInProgress.TradeView.Interface.Server;
using DevelopmentInProgress.TradeView.Interface.Strategy;
using DevelopmentInProgress.TradeView.Service;
using DipSocket.NetCore.Extensions;
using System;

namespace DevelopmentInProgress.TradeServer.StrategyRunner.WebHost.Web
{
    public class Startup
    {
        public Startup(IHostingEnvironment env, IConfiguration configuration)
        {

            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddEnvironmentVariables()
                .AddConfiguration(configuration);
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var server = new Server();
            server.Started = DateTime.Now;
            server.StartedBy = Environment.UserName;
            server.Name = Configuration["s"].ToString();
            server.Url = Configuration["u"].ToString();
            if(Convert.ToInt32(Configuration["p"]) > 0)
            {
                server.MaxDegreeOfParallelism = Convert.ToInt32(Configuration["p"]);
            }

            services.AddSingleton<IServer>(server);

            services.AddSingleton<IStrategyRunnerActionBlock, StrategyRunnerActionBlock>();
            services.AddTransient<IStrategyRunner, StrategyRunner>();
            services.AddSingleton<IStrategyNotificationPublisherContext, StrategyNotificationPublisherContext>();
            services.AddSingleton<IStrategyNotificationPublisher, StrategyNotificationPublisher>();
            services.AddSingleton<IBatchNotificationFactory<StrategyNotification>, StrategyBatchNotificationFactory>();
            services.AddSingleton<IExchangeApiFactory, ExchangeApiFactory>();
            services.AddSingleton<IExchangeService, ExchangeService>();
            services.AddSingleton<IExchangeSubscriptionsCacheFactory, ExchangeSubscriptionsCacheFactory>();
            services.AddSingleton<ISubscriptionsCacheManager, SubscriptionsCacheManager>();
            services.AddSingleton<ITradeStrategyCacheManager, TradeStrategyCacheManager>();

            services.AddHostedService<StrategyRunnerBackgroundService>();

            services.AddDipSocket<StrategyNotificationHub>();
            services.AddDipSocket<ServerHub>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app)
        {
            app.UseDipSocket<StrategyNotificationHub>("/notificationhub");
            app.UseDipSocket<ServerHub>("/serverhub");

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
