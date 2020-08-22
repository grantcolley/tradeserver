using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;

namespace DevelopmentInProgress.TradeServer.StrategyRunner.WebHost.Web.Middleware
{
    public class PingMiddleware
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA1801:Review unused parameters")]
        public PingMiddleware(RequestDelegate next)
        {
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1822:Mark members as static")]
        public async Task Invoke(HttpContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            await context.Response.WriteAsync($"{Environment.MachineName} {System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName} is Alive!").ConfigureAwait(false);
        }
    }
}
