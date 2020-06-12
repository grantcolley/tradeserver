using DevelopmentInProgress.TradeServer.StrategyRunner.WebHost.Cache.TradeStrategy;
using DevelopmentInProgress.TradeView.Core.Strategy;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Net;
using System.Threading.Tasks;

namespace DevelopmentInProgress.TradeServer.StrategyRunner.WebHost.Web.Middleware
{
    public class UpdateStrategyMiddleware
    {
        public UpdateStrategyMiddleware(RequestDelegate next)
        {
        }

        public async Task Invoke(HttpContext context, ITradeStrategyCacheManager tradeStrategyCacheManager)
        {
            try
            {
                var json = context.Request.Form["strategyparameters"];

                // check the json can convert to type StrategyParameters
                var strategyParameters = JsonConvert.DeserializeObject<StrategyParameters>(json);

                await tradeStrategyCacheManager.UpdateStrategy(strategyParameters.StrategyName, json);
            }
            catch (Exception ex)
            {
                var response = context.Response;
                response.ContentType = "application/json";
                response.StatusCode = (int)HttpStatusCode.InternalServerError;
                await response.WriteAsync(JsonConvert.SerializeObject(ex));
            }
        }
    }
}