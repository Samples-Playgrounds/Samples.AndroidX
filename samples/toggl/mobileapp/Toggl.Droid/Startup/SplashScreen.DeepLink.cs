using Android.Content;
using Android.OS;
using System;
using System.Reactive;
using AndroidX.Core.App;
using Toggl.Core.UI.Extensions;
using Toggl.Core.UI.Parameters;
using Toggl.Core.UI.ViewModels;
using Toggl.Droid.Activities;
using Toggl.Droid.Helper;
using Toggl.Shared;

namespace Toggl.Droid
{
    public partial class SplashScreen
    {
        private const ActivityFlags mainTabBarFromDeepLinkActivityFlags = ActivityFlags.ClearTask | ActivityFlags.ClearTop | ActivityFlags.NewTask | ActivityFlags.TaskOnHome;

        private void handleDeepLink(Uri uri, AndroidDependencyContainer dependencyContainer)
        {
            var timeService = dependencyContainer.TimeService;
            var interactorFactory = dependencyContainer.InteractorFactory;
            var urlParser = dependencyContainer.DeeplinkParser;

            var parameters = urlParser.Parse(uri);
            parameters.Match(
                te => runAndStartRootActivity(() => te.Start(interactorFactory, timeService)),
                te => runAndStartRootActivity(() => te.Continue(interactorFactory)),
                te => runAndStartRootActivity(() => te.Stop(interactorFactory, timeService)),
                te => runAndStartRootActivity(() => te.Create(interactorFactory)),
                te => runAndStartRootActivity(() => te.Update(interactorFactory, timeService)),
                showNewTimeEntry,
                showEditTimeEntry,
                showReports,
                showCalendar,
                _ => startRootActivityAndFinish());
        }

        private void runAndStartRootActivity(Action matchAction)
        {
            matchAction();
            startRootActivityAndFinish();
        }

        private void startRootActivityAndFinish()
        {
            StartActivity(createRootActivityIntent());
            Finish();
        }

        private void showNewTimeEntry(DeeplinkNewTimeEntryParameters deeplinkNewTimeEntryParameters)
        {
            var timeService = AndroidDependencyContainer.Instance.TimeService;
            loadAndCacheViewModelWithParams<StartTimeEntryViewModel, StartTimeEntryParameters>(
                deeplinkNewTimeEntryParameters.ToStartTimeEntryParameters(timeService));

            var mainIntent = createRootActivityIntent();

            TaskStackBuilder.Create(this)
                .AddNextIntent(mainIntent)
                .AddNextIntent(new Intent(this, typeof(StartTimeEntryActivity)))
                .StartActivities();
        }

        private void showEditTimeEntry(DeeplinkEditTimeEntryParameters deeplinkEditTimeEntryParameters)
        {
            loadAndCacheViewModelWithParams<EditTimeEntryViewModel, long[]>(new[] { deeplinkEditTimeEntryParameters.TimeEntryId });

            var mainIntent = createRootActivityIntent();

            TaskStackBuilder.Create(this)
                .AddNextIntent(mainIntent)
                .AddNextIntent(new Intent(this, typeof(EditTimeEntryActivity)))
                .StartActivities();
        }

        private void showReports(DeeplinkShowReportsParameters deeplinkShowReportsParameters)
        {
            var startDate = deeplinkShowReportsParameters.StartDate?.ToUnixTimeSeconds() ?? 0L;
            var endDate = deeplinkShowReportsParameters.EndDate?.ToUnixTimeSeconds() ?? 0L;
            var workspaceId = deeplinkShowReportsParameters.WorkspaceId ?? 0L;
            var intentExtras = new Bundle();
            intentExtras.PutInt(MainTabBarActivity.StartingTabExtra, Resource.Id.MainTabReportsItem);
            intentExtras.PutLong(MainTabBarActivity.StartDateExtra, startDate);
            intentExtras.PutLong(MainTabBarActivity.EndDateExtra, endDate);
            intentExtras.PutLong(MainTabBarActivity.StartDateExtra, workspaceId);

            var intent = createRootActivityIntent(intentExtras);
            StartActivity(intent);
            Finish();
        }

        private void showCalendar(DeeplinkShowCalendarParameters deeplinkShowCalendarParameters)
        {
            var intentExtras = new Bundle();
            intentExtras.PutInt(MainTabBarActivity.StartingTabExtra, Resource.Id.MainTabCalendarItem);

            var intent = createRootActivityIntent(intentExtras);
            StartActivity(intent);
            Finish();
        }

        private void loadAndCacheViewModelWithParams<TViewModelType, TViewModelParams>(TViewModelParams viewModelParams)
            where TViewModelType : ViewModel<TViewModelParams, Unit>
        {
            var dependencyContainer = AndroidDependencyContainer.Instance;
            var vmLoader = dependencyContainer.ViewModelLoader;
            var vmCache = dependencyContainer.ViewModelCache;

            var viewModel = vmLoader.Load<TViewModelType>();
            vmCache.Cache(viewModel);

            viewModel.Initialize(viewModelParams);
        }

        private Intent createRootActivityIntent(Bundle extras = null)
            => new Intent(this, typeof(MainTabBarActivity))
                .PutExtras(extras ?? Bundle.Empty)
                .AddFlags(mainTabBarFromDeepLinkActivityFlags);

        private string getTrackUrlFromProcessedText()
        {
            if (MarshmallowApis.AreNotAvailable)
                return null;

            var description = Intent.GetStringExtra(Intent.ExtraProcessText);
            if (string.IsNullOrWhiteSpace(description))
                return null;

            var applicationUrl = ApplicationUrls.TimeEntry.Start.WithDescription(description);
            return applicationUrl;
        }
    }
}
