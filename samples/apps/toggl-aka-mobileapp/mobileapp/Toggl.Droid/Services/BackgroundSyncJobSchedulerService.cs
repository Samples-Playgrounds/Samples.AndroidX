using Android.App;
using Android.App.Job;
using System;
using System.Reactive.Linq;
using Toggl.Droid.Helper;
using static Toggl.Shared.Extensions.CommonFunctions;

namespace Toggl.Droid.Services
{
    [Service(Exported = true,
             Permission = "android.permission.BIND_JOB_SERVICE",
             Name = "com.toggl.giskard.BackgroundSyncJobSchedulerService")]
    public class BackgroundSyncJobSchedulerService : JobService
    {
        public const int JobId = 1;

        private IDisposable disposable;

        public override bool OnStartJob(JobParameters @params)
        {
            // Background sync for Android 10 is temporary disabled due to a crash on Android 10 that is hard to reproduce
            // Calling JobFinished and returning early here stops the background job from running
            if (QApis.AreAvailable)
            {
                JobFinished(@params, false);
                return false;
            }

            AndroidDependencyContainer.EnsureInitialized(ApplicationContext);
            var dependencyContainer = AndroidDependencyContainer.Instance;
            if (!dependencyContainer.UserAccessManager.CheckIfLoggedIn())
                return false;

            // The widgets service will listen for changes to the running
            // time entry and it will update the data in the shared database
            // and that way the widget will show correct information after we sync.
            dependencyContainer.WidgetsService.Start();

            disposable = dependencyContainer.InteractorFactory.RunBackgroundSync()
                .Execute()
                .Subscribe(DoNothing, DoNothing,
                    () => JobFinished(@params, false));
            
            return true;
        }

        public override bool OnStopJob(JobParameters @params)
        {
            AndroidDependencyContainer
                .Instance.AnalyticsService
                .BackgroundSyncMustStopExcecution.Track();

            disposable?.Dispose();
            return true;
        }
    }
}
