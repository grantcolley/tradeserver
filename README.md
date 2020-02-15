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
* [HostedService](#hostedservice)
* [Notifications](#notifications)
* [Middleware](#middleware)
* [Running a Strategy](#running-a-strategy)
* [Trade Server Manager](#trade-server-manager)
* [Subscriptions Caching](#subscriptions-caching)

## The Console
The [console app](https://github.com/grantcolley/tradeserver/blob/master/src/DevelopmentInProgress.TradeServer.Console/Program.cs) takes two parameters: ServerName and Url. It creates and runs an instance of a WebHost, passing the parameters into it.

```C#
      dotnet DevelopmentInProgress.TradeServer.Console.dll --ServerName=TradeServer1 --Url=http://+:5500
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

The WebHost's [UseStrategyRunnerStartup](https://github.com/grantcolley/tradeserver/blob/master/src/DevelopmentInProgress.TradeServer.StrategyRunner.WebHost/Web/WebHostExtensions.cs) extension method passes in the command line args to the WebHost and species the [Startup](https://github.com/grantcolley/tradeserver/blob/master/src/DevelopmentInProgress.TradeServer.StrategyRunner.WebHost/Web/Startup.cs) class to use.

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

## StrategyRunnerBackgroundService

## HostedService

## Notifications

## Middleware

## Running a Strategy

## Trade Server Manager

## Subscriptions Caching

