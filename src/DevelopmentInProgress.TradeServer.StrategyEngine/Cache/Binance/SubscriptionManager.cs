using DevelopmentInProgress.MarketView.Interface.Interfaces;
using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DevelopmentInProgress.TradeServer.StrategyEngine.Cache.Binance
{
    public abstract class SubscriptionManager<T> : ISubscriptionManager<T>, IDisposable
    {
        private ConcurrentDictionary<string, StrategyNotification<T>> subscribers;
        private CancellationTokenSource cancellationTokenSource;

        private bool disposed;

        public SubscriptionManager(string symbol, int limit, IExchangeService exchangeService)
        {
            Symbol = symbol;
            Limit = limit;
            ExchangeService = exchangeService;

            subscribers = new ConcurrentDictionary<string, StrategyNotification<T>>();

            cancellationTokenSource = new CancellationTokenSource();
        }

        public abstract void ExchangeSubscribe(string symbol, int limit, Action<T> update, Action<Exception> exception, CancellationToken cancellationToken);

        public string Symbol { get; private set; }

        public int Limit { get; private set; }

        protected IExchangeService ExchangeService { get; set; }

        public bool HasSubscriptions
        {
            get
            {
                if (subscribers.Any())
                {
                    return true;
                }

                return false;
            }
        }

        public int Subscriptions
        {
            get
            {
                return subscribers.Count();
            }

        }

        public void Subscribe(string strategyName, StrategyNotification<T> strategyNotification)
        {
            if (cancellationTokenSource.IsCancellationRequested)
            {
                return;
            }

            var hasSubscribers = subscribers.Any();

            if (!subscribers.TryAdd(strategyName, strategyNotification))
            {
                strategyNotification.Exception(new Exception($"Failed to subscribe {strategyName} : {typeof(T).Name}"));
                return;
            }

            if (!hasSubscribers)
            {
                ExchangeSubscribe(Symbol, Limit, Update, Exception, cancellationTokenSource.Token);
            }
        }

        public void Unsubscribe(string strategyName, Action<Exception> exception)
        {
            if (cancellationTokenSource.IsCancellationRequested)
            {
                return;
            }

            if (!subscribers.TryRemove(strategyName, out StrategyNotification<T> e))
            {
                exception(new Exception($"Failed to unsubscribe {strategyName} : {typeof(T).Name}"));
                return;
            }

            if (!subscribers.Any())
            {
                if (!cancellationTokenSource.IsCancellationRequested)
                {
                    cancellationTokenSource.Cancel();
                }
            }
        }

        private async void Update(T args)
        {
            if (cancellationTokenSource.IsCancellationRequested)
            {
                return;
            }

            var subs = (from s in subscribers.Values select OnUpdate(s.Update, args)).ToArray();

            await Task.WhenAll(subs);
        }

        private async Task OnUpdate(Action<T> action, T args)
        {
            await Task.Run(() => action.Invoke(args));
        }

        private async void Exception(Exception exception)
        {
            if (cancellationTokenSource.IsCancellationRequested)
            {
                return;
            }

            var subs = (from s in subscribers.Values select OnException(s.Exception, exception)).ToArray();

            await Task.WhenAll(subs);
        }

        private async Task OnException(Action<Exception> exception, Exception args)
        {
            await Task.Run(() => exception.Invoke(args));
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (disposed)
            {
                return;
            }

            if (disposing)
            {
                cancellationTokenSource.Cancel();
                cancellationTokenSource.Dispose();
            }

            disposed = true;
        }
    }
}
