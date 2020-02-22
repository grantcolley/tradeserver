# tradeserver
A **.Net Core** web host for running crypto currency strategies.

##### Technologies
*	###### Net Core 2.2 and .Net Standard 2.0
#####

#### Table of Contents
* [The Console](#the-console)
* [WebHost](#webhost)
* [Startup](#startup)
* [Request pipelines and Middleware](#request-pipelines-and-middleware)
* [StrategyRunnerBackgroundService](#strategyrunnerbackgroundservice)
* [NotificationHub](#notificationhub)
* [Running a Strategy](#running-a-strategy)
* [Monitoring a Running Strategy](monitoring-a-running-strategy)
* [Trade Server Manager](#trade-server-manager)
* [Subscriptions Caching](#subscriptions-caching)

## The Console
The [console app](https://github.com/grantcolley/tradeserver/blob/master/src/DevelopmentInProgress.TradeServer.Console/Program.cs) takes three parameters:
- **s** = server name
- **u** = url of the webhost
- **p** = MaxDegreeOfParallelism for the dataflow StrategyRunnerActionBlock execution options

It creates and runs an instance of a WebHost, passing the parameters into it.

`dotnet DevelopmentInProgress.TradeServer.Console.dll --s=ServerName --u=http://+:5500 --p=5`

## WebHost
The WebHost has HTTP server features and is responsible for the trade server startup and lifetime management including configuring the server and request processing pipeline, logging, dependency injection, and configuration.

```C#
          var webHost = WebHost.CreateDefaultBuilder()
              .UseUrls(url)
              .UseStrategyRunnerStartup(args)
              .UseSerilog()
              .Build();
```

The WebHost's [UseStrategyRunnerStartup](https://github.com/grantcolley/tradeserver/blob/master/src/DevelopmentInProgress.TradeServer.StrategyRunner.WebHost/Web/WebHostExtensions.cs) extension method passes in the command line args to the WebHost and specifies the Startup class to use.

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
ASP.NET Core uses a [Startup](https://github.com/grantcolley/tradeserver/blob/master/src/DevelopmentInProgress.TradeServer.StrategyRunner.WebHost/Web/Startup.cs) class (named Startup by convention) to configure services and the applications request pipeline.

The Startup class includes a `Configure` method, which is used to create the request processing pipeline by branching the request path to the appropriate middleware. 

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

The Startup class also includes a `ConfigureServices` method, which is used to configure services to be consumed via dependency injection.

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

## Request pipelines and Middleware
The following table shows the middleware each request path is mapped to. 
|Request Path|Maps to Middleware|Description|
|------------|------------------|-----------|
|`http://localhost:5500/runstrategy`|[RunStrategyMiddleware](https://github.com/grantcolley/tradeserver/blob/master/src/DevelopmentInProgress.TradeServer.StrategyRunner.WebHost/Web/Middleware/RunStrategyMiddleware.cs)|Request to run a strategy|
|`http://localhost:5500/stopstrategy`|[StopStrategyMiddleware](https://github.com/grantcolley/tradeserver/blob/master/src/DevelopmentInProgress.TradeServer.StrategyRunner.WebHost/Web/Middleware/StopStrategyMiddleware.cs)|Stop a running strategy|
|`http://localhost:5500/updatestrategy`|[UpdateStrategyMiddleware](https://github.com/grantcolley/tradeserver/blob/master/src/DevelopmentInProgress.TradeServer.StrategyRunner.WebHost/Web/Middleware/UpdateStrategyMiddleware.cs)|Update a running strategy's parameters|
|`http://localhost:5500/isstrategyrunning`|[IsStrategyRunningMiddleware](https://github.com/grantcolley/tradeserver/blob/master/src/DevelopmentInProgress.TradeServer.StrategyRunner.WebHost/Web/Middleware/IsStrategyRunningMiddleware.cs)|Check if a strategy is running|
|`http://localhost:5500/ping`|[PingMiddleware](https://github.com/grantcolley/tradeserver/blob/master/src/DevelopmentInProgress.TradeServer.StrategyRunner.WebHost/Web/Middleware/PingMiddleware.cs)|Check if the trade server is running|
|`http://localhost:5500/notificationhub`|[DipSocketMiddleware](https://github.com/grantcolley/dipsocket/blob/master/src/DipSocket.NetCore.Extensions/DipSocketMiddleware.cs)|A websocket connection request|

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

## NotificationHub
The application uses [DipSocket](https://github.com/grantcolley/dipsocket), a lightweight publisher / subscriber implementation using WebSockets, for sending and receiving notifications to and from clients. The [NotificationHub](https://github.com/grantcolley/tradeserver/blob/master/src/DevelopmentInProgress.TradeServer.StrategyRunner.WebHost/Notification/Publishing/NotificationHub.cs) inherits the abstract class [DipSocketServer](https://github.com/grantcolley/dipsocket/blob/master/src/DipSocket/Server/DipSocketServer.cs) to manage client connections and channels. A client e.g. a running instance of [tradeview](https://github.com/grantcolley/tradeview), establishes a connection to the server with the purpose of running or monitoring a strategy on it. The strategy registers a DipSocket channel to which multiple client connections can subscribe. The strategy broadcasts notifications (e.g. live trade feed, buy and sell orders etc.) to the client connections. The [NotificationHub](https://github.com/grantcolley/tradeserver/blob/master/src/DevelopmentInProgress.TradeServer.StrategyRunner.WebHost/Notification/Publishing/NotificationHub.cs) overrides the OnClientConnectAsync and ReceiveAsync methods.

```C#
          public async override Task OnClientConnectAsync(WebSocket websocket, string clientId, string strategyName)
          {
                var connection = await base.AddWebSocketAsync(websocket).ConfigureAwait(false);

                SubscribeToChannel(strategyName, websocket);

                var connectionInfo = connection.GetConnectionInfo();

                var json = JsonConvert.SerializeObject(connectionInfo);

                var message = new Message { MethodName = "OnConnected", SenderConnectionId = "StrategyRunner", Data = json };

                await SendMessageAsync(websocket, message).ConfigureAwait(false);
          }
        
          public async override Task ReceiveAsync(WebSocket webSocket, Message message)
          {
                switch (message.MessageType)
                {
                    case MessageType.UnsubscribeFromChannel:
                        UnsubscribeFromChannel(message.Data, webSocket);
                        break;
                }
          }
```

## Running a Strategy
The clients loads the serialised [Strategy](https://github.com/grantcolley/tradeview/blob/master/src/DevelopmentInProgress.TradeView.Interface/Strategy/Strategy.cs) and strategy assemblies into a MultipartFormDataContent and post a request to the server.

```C#
            var client = new HttpClient();
            var multipartFormDataContent = new MultipartFormDataContent();

            var jsonContent = JsonConvert.SerializeObject(strategy);

            multipartFormDataContent.Add(new StringContent(jsonContent, Encoding.UTF8, "application/json"), "strategy");

            foreach (var file in files)
            {
                var fileInfo = new FileInfo(file);
                var fileStream = File.OpenRead(file);
                using (var br = new BinaryReader(fileStream))
                {
                    var byteArrayContent = new ByteArrayContent(br.ReadBytes((int)fileStream.Length));
                    multipartFormDataContent.Add(byteArrayContent, fileInfo.Name, fileInfo.FullName);
                }
            }

            Task<HttpResponseMessage> response = await client.PostAsync("http://localhost:5500/runstrategy", multipartFormDataContent);
```

The [RunStrategyMiddleware](https://github.com/grantcolley/tradeserver/blob/master/src/DevelopmentInProgress.TradeServer.StrategyRunner.WebHost/Web/Middleware/RunStrategyMiddleware.cs) processes the request. It deserialises the strategy and downloads the strategy assemblies into a sub directory under the working directory of the application. The running of the strategy is passed to the [StrategyRunnerActionBlock](https://github.com/grantcolley/tradeserver/blob/master/src/DevelopmentInProgress.TradeServer.StrategyRunner.WebHost/Web/HostedService/StrategyRunnerActionBlock.cs).

```C#
                var json = context.Request.Form["strategy"];

                var strategy = JsonConvert.DeserializeObject<Strategy>(json);

                var downloadsPath = Path.Combine(Directory.GetCurrentDirectory(), "downloads", Guid.NewGuid().ToString());

                if (context.Request.HasFormContentType)
                {
                    var form = context.Request.Form;

                    var downloads = from f
                                    in form.Files
                                    select Download(f, downloadsPath);

                    await Task.WhenAll(downloads.ToArray());
                }

                var strategyRunnerActionBlockInput = new StrategyRunnerActionBlockInput
                {
                    StrategyRunner = strategyRunner,
                    Strategy = strategy,
                    DownloadsPath = downloadsPath
                };

                await strategyRunnerActionBlock.RunStrategyAsync(strategyRunnerActionBlockInput).ConfigureAwait(false);
```

## Monitoring a Running Strategy

## Trade Server Manager

## Subscriptions Caching

