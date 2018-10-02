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
            string url;

            if (args == null
                || args.Length.Equals(0))
            {
                url = "http://+:5500";
            }
            else
            {
                url = args[0];
            }

            try
            {
                Log.Logger = new LoggerConfiguration()
                    .WriteTo.File("DevelopmentInProgress.TradeServer.Console-.log", rollingInterval: RollingInterval.Day)
                    .WriteTo.Console()
                    .CreateLogger();

                Log.Information("Launching DevelopmentInProgress.TradeServer.Console");

                var webHost = WebHost.CreateDefaultBuilder()
                    .UseUrls(url)
                    .UseStrategyEngineStartup()
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
