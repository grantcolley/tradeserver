using DevelopmentInProgress.TradeServer.StrategyRunner.WebHost.Web;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Serilog;
using System;
using System.Linq;

namespace DevelopmentInProgress.TradeServer.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                Log.Logger = new LoggerConfiguration()
                    .WriteTo.File("DevelopmentInProgress.TradeServer.Console-.log", rollingInterval: RollingInterval.Day)
                    .WriteTo.Console()
                    .CreateLogger();

                if (args == null
                    || args.Length.Equals(0))
                {
                    Log.Warning($"No args so use defaults...");

                    args = new[]
                    {
                        $"ServerName=TradeServer_{Guid.NewGuid().ToString()}",
                        "Url=http://+:5500"
                    };
                }
                else if (InValidArgs(args))
                {
                    Log.Error($"Invalid args");

                    foreach (var arg in args)
                    {
                        Log.Error($"{arg}");
                    }

                    Log.Error($"You must provide the following args:");
                    Log.Error($"--ServerName=YourServerName");
                    Log.Error($"--Url=http://+:5500");

                    return;
                }

                foreach (var arg in args)
                {
                    Log.Information($"{arg}");
                }

                var url = args.First(a => a.StartsWith("Url=")).Split("=")[1];

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

        private static bool InValidArgs(string[] args)
        {
            if (args.Length != 2)
            {
                return true;
            }
            
            if(!args[0].StartsWith("--ServerName=") && !args[1].StartsWith("--ServerName="))
            {
                return true;
            }

            if(!args[0].StartsWith("--Url=") && !args[1].StartsWith("--Url="))
            {
                return true;
            }

            args[0] = args[0].Substring(2, args[0].Length - 2);
            args[1] = args[1].Substring(2, args[1].Length - 2);

            return false;
        }
    }
}
