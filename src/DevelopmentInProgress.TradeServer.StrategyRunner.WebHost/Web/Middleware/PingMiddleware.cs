using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;

namespace DevelopmentInProgress.TradeServer.StrategyRunner.WebHost.Web.Middleware
{
    public class PingMiddleware
    {
        public PingMiddleware(RequestDelegate next)
        {
        }

        public async Task Invoke(HttpContext context)
        {
            await context.Response.WriteAsync($"{Environment.MachineName} {System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName} is Alive!");
        }
    }
}
