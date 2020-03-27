using Android.OS;
using System;
using System.Reactive.Concurrency;
using Toggl.Core.Analytics;
using Toggl.Shared;

namespace Toggl.Droid
{
    public sealed class AndroidSchedulerProvider : ISchedulerProvider
    {
        public IScheduler MainScheduler { get; }
        public IScheduler DefaultScheduler { get; }
        public IScheduler BackgroundScheduler { get; }

        public AndroidSchedulerProvider(IAnalyticsService analyticsService)
        {
            MainScheduler = new HandlerScheduler(new Handler(Looper.MainLooper), analyticsService)
                .ToTrackingScheduler(analyticsService);

            DefaultScheduler = Scheduler.Default
                .ToTrackingScheduler(analyticsService);

            BackgroundScheduler = NewThreadScheduler.Default
                .ToTrackingScheduler(analyticsService);
        }
    }

    public sealed class TrackedSchedulerWrapper : IScheduler
    {
        private readonly IScheduler innerScheduler;
        private readonly IAnalyticsService analyticsService;
        private readonly string analyticsEventName;

        public TrackedSchedulerWrapper(IScheduler innerScheduler, IAnalyticsService analyticsService)
        {
            analyticsEventName = innerScheduler.GetType().Name;

            this.innerScheduler = innerScheduler;
            this.analyticsService = analyticsService;
        }

        public DateTimeOffset Now => innerScheduler.Now;

        private void performAnalytics(string cause, Exception exception)
        {
            analyticsService.DebugSchedulerError.Track(
                analyticsEventName,
                cause,
                exception.GetType().Name,
                exception.StackTrace);

            analyticsService.Track(exception, exception.Message);
        }

        public IDisposable Schedule<TState>(TState state, Func<IScheduler, TState, IDisposable> action)
        {
            try
            {
                return innerScheduler.Schedule(state, action);
            }
            catch (Exception exception)
            {
                performAnalytics("Schedule:1", exception);
                throw;
            }
        }

        public IDisposable Schedule<TState>(TState state, TimeSpan dueTime, Func<IScheduler, TState, IDisposable> action)
        {
            try
            {
                return innerScheduler.Schedule(state, dueTime, action);
            }
            catch (Exception exception)
            {
                performAnalytics("Schedule:2", exception);
                throw;
            }
        }

        public IDisposable Schedule<TState>(TState state, DateTimeOffset dueTime, Func<IScheduler, TState, IDisposable> action)
        {
            try
            {
                return innerScheduler.Schedule(state, dueTime, action);
            }
            catch (Exception exception)
            {
                performAnalytics("Schedule:3", exception);
                throw;
            }
        }
    }

    public static class SchedulerExtensions
    {
        public static IScheduler ToTrackingScheduler(this IScheduler scheduler, IAnalyticsService analyticsService)
            => new TrackedSchedulerWrapper(scheduler, analyticsService);
    }
}
