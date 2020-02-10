using DevelopmentInProgress.TradeServer.StrategyRunner.WebHost.Web;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Serilog;
using System;

namespace DevelopmentInProgress.TradeServer.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args == null
                || args.Length.Equals(0))
            {
                args = new[] 
                {
                    $"ServerName=TradeServer_{Guid.NewGuid().ToString()}", 
                    "Url=http://+:5500" 
                };
            }

            var url = args[1].Split("=")[1];

            try
            {
                Log.Logger = new LoggerConfiguration()
                    .WriteTo.File("DevelopmentInProgress.TradeServer.Console-.log", rollingInterval: RollingInterval.Day)
                    .WriteTo.Console()
                    .CreateLogger();

                Log.Information("Launching DevelopmentInProgress.TradeServer.Console");

                var webHost = WebHost.CreateDefaultBuilder()
                    .UseUrls(url)
                    .UseStrategyRunnerStartup(args)
                    .UseSerilog()
                    .Build();

                var task = webHost.RunAsync();
                task.GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "DevelopmentInProgress.TradeServer.Console terminated unexpectedly");
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }
    }
}
