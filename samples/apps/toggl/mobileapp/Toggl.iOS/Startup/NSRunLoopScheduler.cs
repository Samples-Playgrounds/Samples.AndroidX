using CoreFoundation;
using Foundation;
using System;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;

namespace Toggl.iOS
{
    public sealed class NSRunloopScheduler : IScheduler
    {
        public DateTimeOffset Now => DateTimeOffset.Now;

        public IDisposable Schedule<TState>(TState state, Func<IScheduler, TState, IDisposable> action)
        {
            var innerDisp = new SingleAssignmentDisposable();

            DispatchQueue.MainQueue.DispatchAsync(() =>
            {
                if (innerDisp.IsDisposed)
                    return;

                innerDisp.Disposable = action(this, state);
            });

            return innerDisp;
        }

        public IDisposable Schedule<TState>(TState state, DateTimeOffset dueTime, Func<IScheduler, TState, IDisposable> action)
        {
            if (dueTime <= Now)
                return Schedule(state, action);

            return Schedule(state, dueTime - Now, action);
        }

        public IDisposable Schedule<TState>(TState state, TimeSpan dueTime, Func<IScheduler, TState, IDisposable> action)
        {
            var innerDisp = Disposable.Empty;
            var isCancelled = false;

            var timer = NSTimer.CreateScheduledTimer(dueTime, _ =>
            {
                if (isCancelled)
                    return;

                innerDisp = action(this, state);
            });

            return Disposable.Create(() =>
            {
                isCancelled = true;
                timer.Invalidate();
                innerDisp.Dispose();
            });
        }
    }
}
