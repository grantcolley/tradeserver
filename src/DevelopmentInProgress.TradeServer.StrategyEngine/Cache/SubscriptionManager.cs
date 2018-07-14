﻿using DevelopmentInProgress.MarketView.Interface.Interfaces;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
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

            Parallel.ForEach(subscribers, (subscriber) =>
            {
                OnUpdate(subscriber, args);
            });
        }

        /// <summary>
        /// Updates raised by the subscription to the exchange will be reported back to the strategy.
        /// When reporting the update to the strategy, if the strategy itself throws an exception
        /// then the exception will be reported to the strategy.
        /// Note: when reporting the exception to the strategy, if the strategy itself throws an 
        /// exception then the strategy will be 'forcibly unsubscribed'.
        /// </summary>
        /// <param name="kvp"></param>
        /// <param name="args"></param>
        private void OnUpdate(KeyValuePair<string, StrategyNotification<T>> kvp, T args)
        {
            Task.Factory.StartNew(() =>
                {
                    kvp.Value.Update.Invoke(args);
                })
                .ContinueWith((t) =>
                {
                    kvp.Value.Exception.Invoke(t.Exception);
                }, TaskContinuationOptions.OnlyOnFaulted)
                .ContinueWith((t) =>
                {
                    Unsubscribe(kvp.Key, kvp.Value.Exception);
                }, TaskContinuationOptions.OnlyOnFaulted);
        }

        /// <summary>
        /// An exception raised by the subscription to the exchange will be reported back to the strategy.
        /// Note: when reporting the exchange exception to the strategy, if the strategy itself throws an 
        /// exception then the strategy will be 'forcibly unsubscribed'.
        /// </summary>
        /// <param name="args">The exception to be reported to the strategy.</param>
        private async void Exception(Exception exception)
        {
            if (cancellationTokenSource.IsCancellationRequested)
            {
                return;
            }

            Parallel.ForEach(subscribers, (subscriber) =>
            {
                OnException(subscriber, exception);
            });
        }

        /// <summary>
        /// An exception raissed by the subscription to the exchange will be reported back to the strategy.
        /// Note: when reporting the exchange exception to the strategy, if the strategy itself throws an 
        /// exception then the strategy will be 'unsubscribed'.
        /// </summary>
        /// <param name="kvp">The strategy's notification object.</param>
        /// <param name="args">The exception to be reported to the strategy.</param>
        private void OnException(KeyValuePair<string, StrategyNotification<T>> kvp, Exception args)
        {
            Task.Factory.StartNew(() => kvp.Value.Exception.Invoke(args))
                .ContinueWith((t) =>
                {
                    Unsubscribe(kvp.Key, kvp.Value.Exception);
                },
                TaskContinuationOptions.OnlyOnFaulted);
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
