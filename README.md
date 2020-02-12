# tradeserver
A .Net Core web host for running crypto currency strategies.

##### Technologies
*	###### Net Core 2.2 and .Net Standard 2.0
#####

#### Table of Contents
* [The WebHost](#the-webhost)
* [HostedService](#hostedservice)
* [Notifications](#notifications)
* [Middleware](#middleware)
* [Running a Strategy](#running-a-strategy)
* [Trade Server Manager](#trade-server-manager)
* [Subscriptions Caching](#subscriptions-caching)

## The WebHost
A [console app](https://github.com/grantcolley/tradeserver/blob/master/src/DevelopmentInProgress.TradeServer.Console/Program.cs) creating an instance of a WebHost which is responsible for trade server startup and lifetime management including configuring the server and request processing pipeline, logging, dependency injection, and configuration. The 

```C#
                var webHost = WebHost.CreateDefaultBuilder()
                    .UseUrls(url)
                    .UseStrategyRunnerStartup(args)
                    .UseSerilog()
                    .Build();
```

The [UseStrategyRunnerStartup](https://github.com/grantcolley/tradeserver/blob/master/src/DevelopmentInProgress.TradeServer.StrategyRunner.WebHost/Web/WebHostExtensions.cs) extension method passes in command line args to the WebHost and species the [Startup](https://github.com/grantcolley/tradeserver/blob/master/src/DevelopmentInProgress.TradeServer.StrategyRunner.WebHost/Web/Startup.cs) to use.
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

## HostedService

## Notifications

## Middleware

## Running a Strategy

## Trade Server Manager

## Subscriptions Caching

