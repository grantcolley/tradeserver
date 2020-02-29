﻿using DevelopmentInProgress.TradeServer.StrategyRunner.WebHost.Web;
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

                Log.Information($"Running as {Environment.UserName}");

                if (args == null
                    || args.Length.Equals(0))
                {
                    Log.Warning($"No args. Use defaults...");

                    args = new[]
                    {
                        $"s=TradeServer_{Guid.NewGuid().ToString()}",
                        "u=http://+:5500",
                        "p=5"
                    };
                }
                else if (InvalidArgs(args))
                {
                    Log.Error($"Invalid args");

                    foreach (var arg in args)
                    {
                        Log.Error($"{arg}");
                    }

                    Log.Error($"You must provide the following args:");
                    Log.Error($"--s=YourServerName");
                    Log.Error($"--u=http://+:5500");
                    Log.Error($"--p=5");

                    return;
                }

                Log.Information($"args");
                foreach (var arg in args)
                {
                    Log.Information($"{arg}");
                }

                var url = args.First(a => a.StartsWith("u=")).Split("=")[1];

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

        private static bool InvalidArgs(string[] args)
        {
            var snMissing = true;
            for(int i = 0; i < args.Length; i++)
            {
                if(args[i].StartsWith("--s="))
                {
                    snMissing = false;
                    break;
                }
            }

            var urlMissing = false;
            for (int i = 0; i < args.Length; i++)
            {
                if (args[i].StartsWith("--u="))
                {
                    urlMissing = false;
                    break;
                }
            }

            if(snMissing || urlMissing)
            {
                return true;
            }

            for (int i = 0; i < args.Length; i++)
            {
                args[i] = args[i].Substring(2, args[i].Length - 2);
            }

            return false;
        }
    }
}
