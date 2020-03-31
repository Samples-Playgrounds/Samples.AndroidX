using System;
using System.Reactive.Linq;
using Toggl.Core.Analytics;
using Toggl.Shared;
using Toggl.Shared.Models;

namespace Toggl.Core.Sync.States.PullTimeEntries
{
    public sealed class TimeEntriesAnalyticsState : ISyncState<IFetchObservables>
    {
        private readonly IAnalyticsService analyticsService;

        public StateResult<IFetchObservables> Done { get; } = new StateResult<IFetchObservables>();

        public TimeEntriesAnalyticsState(IAnalyticsService analyticsService)
        {
            Ensure.Argument.IsNotNull(analyticsService, nameof(analyticsService));

            this.analyticsService = analyticsService;
        }

        public IObservable<ITransition> Start(IFetchObservables fetchObservables)
            => fetchObservables.GetList<ITimeEntry>()
                .Do(timeEntries =>
                {
                    analyticsService.PushInitiatedSyncFetch.Track(timeEntries.Count.ToString());
                })
                .Select(_ => Done.Transition(fetchObservables));
    }
}

