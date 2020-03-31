using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using Toggl.Core.DataSources.Interfaces;
using Toggl.Core.Models.Interfaces;
using Toggl.Networking.ApiClients;
using Toggl.Shared;
using Toggl.Storage;
using Toggl.Storage.Models;
using Toggl.Storage.Settings;

namespace Toggl.Core.Interactors.Settings
{
    public class SendFeedbackInteractor : IInteractor<IObservable<Unit>>
    {
        public const string LastLogin = "Last login";
        public const string PhoneModel = "Phone model";
        public const string ManualModeIsOn = "Manual mode is on";
        public const string AppNameAndVersion = "App name and version";
        public const string OperatingSystem = "Platform and OS version";
        public const string LastSyncAttempt = "Time of last attempted sync";
        public const string DeviceTime = "Device's time when sending this feedback";
        public const string DeviceTimeZone = "Device-based timezone";
        public const string AccountTimeZone = "Toggl account based timezone";
        public const string LastSuccessfulSync = "Time of last successful full sync";
        public const string NumberOfUnsyncedTimeEntries = "Number of unsynced time entries";
        public const string NumberOfWorkspaces = "Number of workspaces available to the user";
        public const string NumberOfUnsyncableTimeEntries = "Number of unsyncable time entries";
        public const string NumberOfTimeEntries = "Number of time entries in our database in total";
        public const string InstallLocation = "Install location";
        public const string UserId = "User Id";

        private const string unspecified = "[unspecified]";

        private readonly string message;
        private readonly ITimeService timeService;
        private readonly IFeedbackApi feedbackApi;
        private readonly IPlatformInfo platformInfo;
        private readonly IUserPreferences userPreferences;
        private readonly ILastTimeUsageStorage lastTimeUsageStorage;
        private readonly ISingletonDataSource<IThreadSafeUser> userDataSource;
        private readonly IDataSource<IThreadSafeWorkspace, IDatabaseWorkspace> workspacesDataSource;
        private readonly IDataSource<IThreadSafeTimeEntry, IDatabaseTimeEntry> timeEntriesDataSource;
        private readonly IInteractorFactory interactorFactory;

        public SendFeedbackInteractor(
            IFeedbackApi feedbackApi,
            ISingletonDataSource<IThreadSafeUser> userDataSource,
            IDataSource<IThreadSafeWorkspace, IDatabaseWorkspace> workspacesDataSource,
            IDataSource<IThreadSafeTimeEntry, IDatabaseTimeEntry> timeEntriesDataSource,
            IPlatformInfo platformInfo,
            IUserPreferences userPreferences,
            ILastTimeUsageStorage lastTimeUsageStorage,
            ITimeService timeService,
            IInteractorFactory interactorFactory,
            string message)
        {
            Ensure.Argument.IsNotNull(message, nameof(message));
            Ensure.Argument.IsNotNull(feedbackApi, nameof(feedbackApi));
            Ensure.Argument.IsNotNull(timeService, nameof(timeService));
            Ensure.Argument.IsNotNull(platformInfo, nameof(platformInfo));
            Ensure.Argument.IsNotNull(userDataSource, nameof(userDataSource));
            Ensure.Argument.IsNotNull(userPreferences, nameof(userPreferences));
            Ensure.Argument.IsNotNull(lastTimeUsageStorage, nameof(lastTimeUsageStorage));
            Ensure.Argument.IsNotNull(workspacesDataSource, nameof(workspacesDataSource));
            Ensure.Argument.IsNotNull(timeEntriesDataSource, nameof(timeEntriesDataSource));
            Ensure.Argument.IsNotNull(interactorFactory, nameof(interactorFactory));

            this.message = message;
            this.feedbackApi = feedbackApi;
            this.timeService = timeService;
            this.platformInfo = platformInfo;
            this.userDataSource = userDataSource;
            this.userPreferences = userPreferences;
            this.workspacesDataSource = workspacesDataSource;
            this.timeEntriesDataSource = timeEntriesDataSource;
            this.lastTimeUsageStorage = lastTimeUsageStorage;
            this.interactorFactory = interactorFactory;
        }

        private IObservable<string> accountTimezone
            => userDataSource
                .Current
                .Select(u => u.Timezone);

        private IObservable<int> workspacesCount
            => workspacesDataSource
                .GetAll()
                .Select(list => list.Count());

        private IObservable<int> timeEntriesCount
            => timeEntriesDataSource
                .GetAll()
                .Select(list => list.Count());

        private IObservable<int> unsyncedTimeEntriesCount
            => timeEntriesDataSource
                .GetAll(te => te.SyncStatus == SyncStatus.SyncNeeded)
                .Select(list => list.Count());

        private IObservable<int> unsyncabeTimeEntriesCount
            => timeEntriesDataSource
                .GetAll(te => te.SyncStatus == SyncStatus.SyncFailed)
                .Select(list => list.Count());

        public IObservable<Unit> Execute()
            => Observable.Zip(
                workspacesCount,
                timeEntriesCount,
                unsyncedTimeEntriesCount,
                unsyncabeTimeEntriesCount,
                accountTimezone,
                interactorFactory.GetCurrentUser().Execute(),
                combineData)
                .SelectMany(data =>
                    userDataSource.Get().SelectMany(user =>
                        feedbackApi.Send(user.Email, message, data).ToObservable()));

        private Dictionary<string, string> combineData(
            int workspaces,
            int timeEntries,
            int unsyncedTimeEntries,
            int unsyncableTimeEntriesCount,
            string accountTimezone,
            IThreadSafeUser user)
        {
            var data = new Dictionary<string, string>
            {
                [PhoneModel] = platformInfo.PhoneModel,
                [OperatingSystem] = platformInfo.OperatingSystem,
                [AppNameAndVersion] = $"{platformInfo.Platform}/{platformInfo.Version}",
                [DeviceTimeZone] = platformInfo.TimezoneIdentifier ?? unspecified,
                [AccountTimeZone] = accountTimezone ?? unspecified,
                [NumberOfWorkspaces] = workspaces.ToString(),
                [NumberOfTimeEntries] = timeEntries.ToString(),
                [NumberOfUnsyncedTimeEntries] = unsyncedTimeEntries.ToString(),
                [NumberOfUnsyncableTimeEntries] = unsyncableTimeEntriesCount.ToString(),
                [LastSyncAttempt] = lastTimeUsageStorage.LastSyncAttempt?.ToString() ?? "never",
                [LastSuccessfulSync] = lastTimeUsageStorage.LastSuccessfulSync?.ToString() ?? "never",
                [DeviceTime] = timeService.CurrentDateTime.ToString(),
                [ManualModeIsOn] = userPreferences.IsManualModeEnabled ? "yes" : "no",
                [LastLogin] = lastTimeUsageStorage.LastLogin?.ToString() ?? "never",
                [UserId] = user.Id.ToString()
            };

            if (platformInfo.Platform == Platform.Giskard)
                data[InstallLocation] = platformInfo.InstallLocation.ToString();

            return data;
        }
    }
}
