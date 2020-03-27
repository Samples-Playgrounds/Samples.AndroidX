using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using Toggl.Core.DataSources;
using Toggl.Core.Models.Interfaces;
using Toggl.Shared;
using Toggl.Shared.Extensions;

namespace Toggl.Core.Interactors
{
    public sealed class ObserveTimeTrackedTodayInteractor : IInteractor<IObservable<TimeSpan>>
    {
        private readonly ITimeService timeService;
        private readonly ITimeEntriesSource timeEntries;

        public ObserveTimeTrackedTodayInteractor(
            ITimeService timeService,
            ITimeEntriesSource timeEntries)
        {
            Ensure.Argument.IsNotNull(timeService, nameof(timeService));
            Ensure.Argument.IsNotNull(timeEntries, nameof(timeEntries));

            this.timeService = timeService;
            this.timeEntries = timeEntries;
        }

        public IObservable<TimeSpan> Execute()
            => whenUpdateIsNecessary()
                .Select(_ => calculateTimeTrackedToday())
                .Switch()
                .DistinctUntilChanged();

        private IObservable<Unit> whenUpdateIsNecessary()
            => Observable.Merge(
                    timeEntries.ItemsChanged,
                    timeService.MidnightObservable.SelectUnit(),
                    timeService.SignificantTimeChangeObservable)
                .StartWith(Unit.Default);

        private IObservable<TimeSpan> calculateTimeTrackedToday()
            => Observable.CombineLatest(
                calculateTimeAlreadyTrackedToday(),
                observeElapsedTimeOfRunningTimeEntryIfAny(),
                (alreadyTrackedToday, currentlyRunningTimeEntryDuration) =>
                    alreadyTrackedToday + currentlyRunningTimeEntryDuration);

        private IObservable<TimeSpan> calculateTimeAlreadyTrackedToday()
            => getAll(startedTodayAndStopped)
                .Select(entries => entries.Sum(timeEntry => timeEntry.Duration ?? 0.0))
                .Select(TimeSpan.FromSeconds);

        private IObservable<TimeSpan> observeElapsedTimeOfRunningTimeEntryIfAny()
            => getAll(startedTodayAndRunning)
                .Select(runningTimeEntries => runningTimeEntries.SingleOrDefault())
                .SelectMany(timeEntry =>
                    timeEntry == null
                        ? Observable.Return(TimeSpan.Zero)
                        : observeElapsedTimeOfRunningTimeEntry(timeEntry));

        private bool startedTodayAndStopped(IThreadSafeTimeEntry timeEntry)
            => timeEntry.Start.LocalDateTime.Date == timeService.CurrentDateTime.LocalDateTime.Date
               && timeEntry.Duration != null;

        private bool startedTodayAndRunning(IThreadSafeTimeEntry timeEntry)
            => timeEntry.Start.LocalDateTime.Date == timeService.CurrentDateTime.LocalDateTime.Date
                && timeEntry.Duration == null;

        private IObservable<TimeSpan> observeElapsedTimeOfRunningTimeEntry(IThreadSafeTimeEntry timeEntry)
            => timeService.CurrentDateTimeObservable
                .Select(now => now - timeEntry.Start)
                .StartWith(timeService.CurrentDateTime - timeEntry.Start);

        private IObservable<IEnumerable<IThreadSafeTimeEntry>> getAll(Func<IThreadSafeTimeEntry, bool> predicate)
            => new GetAllTimeEntriesVisibleToTheUserInteractor(timeEntries)
                .Execute()
                .SingleAsync()
                .Select(entries => entries.Where(predicate));
    }
}
