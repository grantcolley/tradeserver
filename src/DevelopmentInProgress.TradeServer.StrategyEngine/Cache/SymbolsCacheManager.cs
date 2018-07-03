using DevelopmentInProgress.MarketView.Interface.TradeStrategy;
using System.Linq;

namespace DevelopmentInProgress.TradeServer.StrategyEngine.Cache
{
    public class SymbolsCacheManager : ISymbolsCacheManager
    {
        private readonly ISymbolsCacheFactory symbolsCacheFactory;

        public SymbolsCacheManager(ISymbolsCacheFactory symbolsCacheFactory)
        {
            this.symbolsCacheFactory = symbolsCacheFactory;
        }

        public void Subscribe(Strategy strategy, ITradeStrategy tradeStrategy)
        {
            var exchangeSymbolsList = (from s in strategy.Symbols
                                  group s by s.Exchange into es
                                  select new { Exchange = es.Key, Symbols = es.ToList() }).ToList();

            foreach(var exchangeSymbols in exchangeSymbolsList)
            {
                var symbolsCache = symbolsCacheFactory.GetSymbolsCache(exchangeSymbols.Exchange);
                symbolsCache.Subscribe(strategy.Name, exchangeSymbols.Symbols, tradeStrategy);
            }
        }

        public void Unsubscribe(Strategy strategy, ITradeStrategy tradeStrategy)
        {
            var exchangeSymbolsList = (from s in strategy.Symbols
                                       group s by s.Exchange into es
                                       select new { Exchange = es.Key, Symbols = es.ToList() }).ToList();

            foreach (var exchangeSymbols in exchangeSymbolsList)
            {
                var symbolsCache = symbolsCacheFactory.GetSymbolsCache(exchangeSymbols.Exchange);
                symbolsCache.Unsubscribe(strategy.Name, exchangeSymbols.Symbols, tradeStrategy);
            }
        }
    }
}