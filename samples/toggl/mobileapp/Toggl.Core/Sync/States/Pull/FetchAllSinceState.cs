using System;
using System.Collections.Generic;
using System.Reactive.Threading.Tasks;
using Toggl.Core.Extensions;
using Toggl.Networking;
using Toggl.Shared.Extensions;
using Toggl.Shared.Models;
using Toggl.Storage;


namespace Toggl.Core.Sync.States.Pull
{
    internal sealed class FetchAllSinceState : BaseFetchSinceState
    {
        private readonly IRateLimiter limiter;

        protected override int NumberOfHttpRequests => 9;

        public FetchAllSinceState(
            ITogglApi api,
            ISinceParameterRepository since,
            ITimeService timeService,
            ILeakyBucket leakyBucket,
            IRateLimiter limiter)
            : base(api, leakyBucket, since, timeService)
        {
            this.limiter = limiter;
        }

        protected override IFetchObservables Fetch()
        {
            var (workspaces, user, features) = firstWave();
            var (preferences, tags, clients) = secondWave(workspaces, user, features);
            var (projects, timeEntries, tasks) = thirdWave(preferences, tags, clients);

            return new FetchObservables(
                workspaces, features, user, clients, projects, timeEntries, tags, tasks, preferences);
        }

        private (IObservable<List<IWorkspace>>, IObservable<IUser>, IObservable<List<IWorkspaceFeatureCollection>>) firstWave()
        {
            var workspaces =
                limiter.WaitForFreeSlot()
                    .ThenExecute(() => Api.Workspaces.GetAll().ToObservable())
                    .ConnectedReplay();

            var user =
                limiter.WaitForFreeSlot()
                    .ThenExecute(() => Api.User.Get().ToObservable())
                    .ConnectedReplay();

            var features =
                limiter.WaitForFreeSlot()
                    .ThenExecute(() => Api.WorkspaceFeatures.GetAll().ToObservable())
                    .ConnectedReplay();

            return (workspaces, user, features);
        }

        private (IObservable<IPreferences>, IObservable<List<ITag>>, IObservable<List<IClient>>) secondWave(
            IObservable<List<IWorkspace>> workspaces,
            IObservable<IUser> user,
            IObservable<List<IWorkspaceFeatureCollection>> features)
        {
            var preferences =
                workspaces.ThenExecute(limiter.WaitForFreeSlot)
                    .ThenExecute(() => Api.Preferences.Get().ToObservable())
                    .ConnectedReplay();

            var tags =
                user.ThenExecute(limiter.WaitForFreeSlot)
                    .ThenExecute(() => FetchRecentIfPossible(Api.Tags.GetAllSince, Api.Tags.GetAll))
                    .ConnectedReplay();

            var clients =
                features.ThenExecute(limiter.WaitForFreeSlot)
                    .ThenExecute(() => FetchRecentIfPossible(Api.Clients.GetAllSince, Api.Clients.GetAll))
                    .ConnectedReplay();

            return (preferences, tags, clients);
        }

        private (IObservable<List<IProject>>, IObservable<List<ITimeEntry>>, IObservable<List<ITask>>) thirdWave(
            IObservable<IPreferences> preferences, IObservable<List<ITag>> tags, IObservable<List<IClient>> clients)
        {
            var projects =
                preferences.ThenExecute(limiter.WaitForFreeSlot)
                    .ThenExecute(() => FetchRecentIfPossible(Api.Projects.GetAllSince, Api.Projects.GetAll))
                    .ConnectedReplay();

            var timeEntries =
                tags.ThenExecute(limiter.WaitForFreeSlot)
                    .ThenExecute(() => FetchRecentIfPossible(Api.TimeEntries.GetAllSince, FetchTwoMonthsOfTimeEntries))
                    .ConnectedReplay();

            var tasks =
                clients.ThenExecute(limiter.WaitForFreeSlot)
                    .ThenExecute(() => FetchRecentIfPossible(Api.Tasks.GetAllSince, Api.Tasks.GetAll))
                    .ConnectedReplay();

            return (projects, timeEntries, tasks);
        }
    }
}
