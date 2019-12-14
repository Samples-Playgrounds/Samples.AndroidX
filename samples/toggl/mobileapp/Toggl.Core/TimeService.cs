using System;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Reactive.Threading.Tasks;
using System.Threading.Tasks;
using Toggl.Shared;
using Toggl.Shared.Extensions;

namespace Toggl.Core
{
    public sealed class TimeService : ITimeService
    {
        private readonly CompositeDisposable disposable = new CompositeDisposable();

        private readonly IScheduler scheduler;

        private readonly ISubject<Unit> significantTimeChange = new Subject<Unit>();

        public DateTimeOffset CurrentDateTime => floor(scheduler.Now);

        public IObservable<DateTimeOffset> CurrentDateTimeObservable { get; }

        public IObservable<DateTimeOffset> MidnightObservable { get; }

        public IObservable<Unit> SignificantTimeChangeObservable => significantTimeChange.AsObservable();

        public TimeService(IScheduler scheduler)
        {
            this.scheduler = scheduler;

            CurrentDateTimeObservable =
                Observable
                    .Interval(TimeSpan.FromSeconds(1), scheduler)
                    .Select(_ => CurrentDateTime)
                    .Publish().RefCount();

            var localTimeObservable = CurrentDateTimeObservable
                .Select(t => t.ToLocalTime());

            MidnightObservable = localTimeObservable
                .Zip(localTimeObservable.Skip(1), (previous, now) => (previous: previous, now: now))
                .Where(t => t.previous.Date != t.now.Date)
                .Select(t => t.now);
        }

        public Task RunAfterDelay(TimeSpan delay, Action action)
        {
            Ensure.Argument.IsNotNull(action, nameof(action));

            return Observable
                .Return(Unit.Default)
                .Delay(delay, scheduler)
                .Do(action)
                .ToTask();
        }

        public void SignificantTimeChanged() => significantTimeChange.OnNext(Unit.Default);

        private DateTimeOffset floor(DateTimeOffset t)
            => new DateTimeOffset(t.Year, t.Month, t.Day, t.Hour, t.Minute, t.Second, t.Offset);
    }
}
