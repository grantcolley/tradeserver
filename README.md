# tradeserver
A .Net Core web host for running crypto currency strategies.

##### Technologies
*	###### Net Core 2.2 and .Net Standard 2.0
#####

#### Table of Contents
* [The Console](#the-console)
* [WebHost](#webhost)
* [Startup](#startup)
* [StrategyRunnerBackgroundService](#strategyrunnerbackgroundservice)
* [Notifications](#notifications)
* [Middleware](#middleware)
* [Running a Strategy](#running-a-strategy)
* [Trade Server Manager](#trade-server-manager)
* [Subscriptions Caching](#subscriptions-caching)

## The Console
The [console app](https://github.com/grantcolley/tradeserver/blob/master/src/DevelopmentInProgress.TradeServer.Console/Program.cs) takes 3 parameters:
- s = server name)
- u = url of the webhost
- p = MaxDegreeOfParallelism for the StrategyRunnerActionBlock execution options

It creates and runs an instance of a WebHost, passing the parameters into it.

```C#
          dotnet DevelopmentInProgress.TradeServer.Console.dll --s=ServerName --u=http://+:5500 --p=5
```

## WebHost
The WebHost has HTTP server features and is responsible for trade server startup and lifetime management including configuring the server and request processing pipeline, logging, dependency injection, and configuration.

```C#
          var webHost = WebHost.CreateDefaultBuilder()
              .UseUrls(url)
              .UseStrategyRunnerStartup(args)
              .UseSerilog()
              .Build();
```

The WebHost's [UseStrategyRunnerStartup](https://github.com/grantcolley/tradeserver/blob/master/src/DevelopmentInProgress.TradeServer.StrategyRunner.WebHost/Web/WebHostExtensions.cs) extension method passes in the command line args to the WebHost and specifies the [Startup](https://github.com/grantcolley/tradeserver/blob/master/src/DevelopmentInProgress.TradeServer.StrategyRunner.WebHost/Web/Startup.cs) class to use.

```C#
          public static class WebHostExtensions
          {
              public static IWebHostBuilder UseStrategyRunnerStartup(this IWebHostBuilder webHost, string[] args)
              {
                  return webHost.ConfigureAppConfiguration((hostingContext, config) =>
                  {
                      config.AddCommandLine(args);
                  }).UseStartup<Startup>();
              }
          }
```

## Startup
ASP.NET Core uses a [Startup](https://github.com/grantcolley/tradeserver/blob/master/src/DevelopmentInProgress.TradeServer.StrategyRunner.WebHost/Web/Startup.cs) class (named Startup by convention) to configure services and the request pipeline.

The Startup class includes a Configure method, which is used to create the request processing pipeline. 

```C#
        public void Configure(IApplicationBuilder app)
        {
            app.UseDipSocket<NotificationHub>("/notificationhub");

            app.Map("/runstrategy", HandleRun);
            app.Map("/updatestrategy", HandleUpdate);
            app.Map("/stopstrategy", HandleStop);
            app.Map("/isstrategyrunning", HandleIsStrategyRunning);
            app.Map("/ping", HandlePing);
        }
```

The Startup class also includes a ConfigureServices method, which is used to configure services to be consumed via dependency injection.

```C#
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
            services.AddSingleton<IStrategyRunner, StrategyRunner>();
            services.AddSingleton<INotificationPublisherContext, NotificationPublisherContext>();
            services.AddSingleton<INotificationPublisher, NotificationPublisher>();
            services.AddSingleton<IBatchNotificationFactory<StrategyNotification>, StrategyBatchNotificationFactory>();
            services.AddSingleton<IExchangeApiFactory, ExchangeApiFactory>();
            services.AddSingleton<IExchangeService, ExchangeService>();
            services.AddSingleton<IExchangeSubscriptionsCacheFactory, ExchangeSubscriptionsCacheFactory>();
            services.AddSingleton<ISubscriptionsCacheManager, SubscriptionsCacheManager>();
            services.AddSingleton<ITradeStrategyCacheManager, TradeStrategyCacheManager>();

            services.AddHostedService<StrategyRunnerBackgroundService>();

            services.AddDipSocket<NotificationHub>();
        }
```

## StrategyRunnerBackgroundService
The Startup class adds a long running hosted service [StrategyRunnerBackgroundService](https://github.com/grantcolley/tradeserver/blob/master/src/DevelopmentInProgress.TradeServer.StrategyRunner.WebHost/Web/HostedService/StrategyRunnerBackgroundService.cs) which inherits the BackgroundService. It is a long running background task for running trade strategies that have been posted to the trade servers runstrategy request pipeline.

The StrategyRunnerBackgroundService contains a reference to the singleton [StrategyRunnerActionBlock](https://github.com/grantcolley/tradeserver/blob/master/src/DevelopmentInProgress.TradeServer.StrategyRunner.WebHost/Web/HostedService/StrategyRunnerActionBlock.cs) which has an ActionBlock dataflow that invokes an ActionBlock<StrategyRunnerActionBlockInput> delegate for each request to run a trade strategy.

```C#
          strategyRunnerActionBlock.ActionBlock = new ActionBlock<StrategyRunnerActionBlockInput>(async actionBlockInput =>
          {
               await actionBlockInput.StrategyRunner.RunAsync(actionBlockInput.Strategy,
                                                              actionBlockInput.DownloadsPath,
                                                              actionBlockInput.CancellationToken)
                                                              .ConfigureAwait(false);
          }, new ExecutionDataflowBlockOptions { MaxDegreeOfParallelism = server.MaxDegreeOfParallelism });
```

## Notifications

## Middleware

## Running a Strategy

## Trade Server Manager

## Subscriptions Caching

