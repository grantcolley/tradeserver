using DevelopmentInProgress.MarketView.Interface.Interfaces;
using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DevelopmentInProgress.TradeServer.StrategyEngine.Cache
{
    public abstract class SubscriptionManager<T> : ISubscriptionManager<T>, IDisposable
    {
        private ConcurrentDictionary<string, StrategyNotification<T>> subscribers;
        private CancellationTokenSource cancellationTokenSource;

        private bool disposed;

        public SubscriptionManager(IExchangeService exchangeService)
        {
            ExchangeService = exchangeService;
            subscribers = new ConcurrentDictionary<string, StrategyNotification<T>>();
            cancellationTokenSource = new CancellationTokenSource();
        }

        public abstract void ExchangeSubscribe(Action<T> update, Action<Exception> exception, CancellationToken cancellationToken);

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
                ExchangeSubscribe(Update, Exception, cancellationTokenSource.Token);
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

        private void Update(T args)
        {
            if (cancellationTokenSource.IsCancellationRequested)
            {
                return;
            }

            Parallel.ForEach(subscribers.Values, (value) =>
            {
                OnUpdate(value.Update, args);
            });
        }

        private void OnUpdate(Action<T> action, T args)
        {
            Task.Factory.StartNew(() => action.Invoke(args));
        }

        private async void Exception(Exception exception)
        {
            if (cancellationTokenSource.IsCancellationRequested)
            {
                return;
            }

            Parallel.ForEach(subscribers.Values, (value) =>
            {
                OnException(value.Exception, exception);
            });
        }

        private void OnException(Action<Exception> exception, Exception args)
        {
            Task.Factory.StartNew(() => exception.Invoke(args));
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
                subscribers.Clear();
                cancellationTokenSource.Cancel();
                cancellationTokenSource.Dispose();
            }

            disposed = true;
        }
    }
}
