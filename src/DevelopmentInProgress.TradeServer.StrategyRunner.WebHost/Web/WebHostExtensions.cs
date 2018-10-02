using Microsoft.AspNetCore.Hosting;

namespace DevelopmentInProgress.TradeServer.StrategyRunner.WebHost.Web
{
    public static class WebHostExtensions
    {
        public static IWebHostBuilder UseStrategyEngineStartup(this IWebHostBuilder webHost)
        {
            return webHost.UseStartup<Startup>();
        }
    }
}
