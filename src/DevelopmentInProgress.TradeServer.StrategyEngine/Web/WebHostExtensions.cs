using Microsoft.AspNetCore.Hosting;

namespace DevelopmentInProgress.TradeServer.StrategyEngine.Web
{
    public static class WebHostExtensions
    {
        public static IWebHostBuilder UseStrategyEngineStartup(this IWebHostBuilder webHost)
        {
            return webHost.UseStartup<Startup>();
        }
    }
}
