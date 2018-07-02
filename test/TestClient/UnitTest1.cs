using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using System.IO;
using System.Collections.Generic;
using DevelopmentInProgress.MarketView.Interface.TradeStrategy;

namespace TestClient
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public async Task TestMethod1()
        {
            string url = "http://localhost:5500/runstrategy";
            string filePath1 = @"C:\GitHub\tradeserver\test\Strategy1\bin\Debug\netstandard2.0\Strategy1.dll";
            string filePath2 = @"C:\GitHub\tradeserver\test\Strategy1\bin\Debug\netstandard2.0\DependencyLibrary.dll";

            try
            {
                var jsonContent = JsonConvert.SerializeObject(new Strategy { Name = "Test Strategy", TargetAssembly = "Strategy1.dll", TargetType = "Strategy1.TestStrategy" });
                var files = new List<string>(new string[] { filePath2, filePath1 });

                var strategyRunnerClient = new StrategyRunnerClient();
                
                var response = await strategyRunnerClient.PostAsync(url, jsonContent, files);
            }
            catch(Exception e)
            {
                var ex = e.Message;
            }
        }
    }
}
