using Android.OS;
using System;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using Toggl.Core.Analytics;

namespace Toggl.Droid
{
    /// <summary>
    /// Custom handler scheduler
    /// </summary>
    /// <remarks>
    /// Roughly based on reactiveui/ReactiveUI HandlerScheduler class
    /// See https://github.com/reactiveui/ReactiveUI/blob/master/src/ReactiveUI/Platforms/android/HandlerScheduler.cs
    /// </remarks>
    public sealed class HandlerScheduler : IScheduler
    {
        private readonly Handler handler;
        private readonly IAnalyticsService analyticsService;
        private readonly string analyticsEventName;

        public DateTimeOffset Now => DateTimeOffset.Now;

        public HandlerScheduler(Handler handler, IAnalyticsService analyticsService)
        {
            this.handler = handler;
            this.analyticsService = analyticsService;
            this.analyticsEventName = GetType().Name;
        }

        public IDisposable Schedule<TState>(TState state, Func<IScheduler, TState, IDisposable> action)
        {
            bool isCancelled = false;
            var innerDisp = new SerialDisposable { Disposable = Disposable.Empty };

            handler.Post(() =>
            {
                try
                {
                    if (isCancelled)
                        return;

                    innerDisp.Disposable = action(this, state);
                }
                catch (Exception exception)
                {
                    performAnalytics("Schedule:Post:1", exception);
                    throw;
                }
            });

            return new CompositeDisposable(
                Disposable.Create(() => isCancelled = true),
                innerDisp
            );
        }

        public IDisposable Schedule<TState>(TState state, TimeSpan dueTime, Func<IScheduler, TState, IDisposable> action)
        {
            var isCancelled = false;
            var innerDisp = new SerialDisposable { Disposable = Disposable.Empty };

            handler.PostDelayed(() =>
            {
                try
                {
                    if (isCancelled)
                        return;

                    innerDisp.Disposable = action(this, state);
                }
                catch (Exception exception)
                {
                    performAnalytics("Schedule:Post:2", exception);
                }
            }, dueTime.Ticks / TimeSpan.TicksPerMillisecond);

            return new CompositeDisposable(
                Disposable.Create(() => isCancelled = true),
                innerDisp
            );
        }

        public IDisposable Schedule<TState>(TState state, DateTimeOffset dueTime, Func<IScheduler, TState, IDisposable> action)
        {
            if (dueTime <= Now)
                return Schedule(state, action);

            return Schedule(state, dueTime - Now, action);
        }

        private void performAnalytics(string cause, Exception exception)
        {
            analyticsService.DebugSchedulerError.Track(
                analyticsEventName,
                cause,
                exception.GetType().Name,
                exception.StackTrace);

            analyticsService.Track(exception, exception.Message);
        }
    }
}
