using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

namespace DevelopmentInProgress.TradeServer.StrategyRunner.WebHost.Web
{
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
}
