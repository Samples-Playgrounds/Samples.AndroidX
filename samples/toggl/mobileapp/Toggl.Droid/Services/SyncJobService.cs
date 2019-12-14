using System;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Android.App;
using Android.App.Job;
using Toggl.Core.Extensions;
using static Toggl.Droid.Services.JobServicesConstants;

namespace Toggl.Droid.Services
{
    [Service(
        Name = "com.toggl.giskard.SyncJobService",
        Permission = "android.permission.BIND_JOB_SERVICE",
        Exported = true)]
    public class SyncJobService : JobService
    {
        public override bool OnStartJob(JobParameters parameters)
        {
            Task.Run(() =>
            {
                try
                {
                    runSync(parameters);
                }
                catch (Exception exception)
                {
                    finishJobClearingPendingSyncJobLock(parameters);
                }
            }).ConfigureAwait(false);

            return true;
        }

        public override bool OnStopJob(JobParameters parameters)
        {
            clearPendingSyncJobLock();
            return false;
        }

        private new TogglApplication Application => base.Application as TogglApplication;

        private void runSync(JobParameters parameters)
        {
            var dependencyContainer = AndroidDependencyContainer.Instance;
            if (!dependencyContainer.UserAccessManager.CheckIfLoggedIn())
            {
                finishJobClearingPendingSyncJobLock(parameters);
                return;
            }

            dependencyContainer.WidgetsService.Start();

            var shouldHandlePushNotifications = dependencyContainer
                .RemoteConfigService
                .GetPushNotificationsConfiguration()
                .HandlePushNotifications;
            
            if (shouldHandlePushNotifications)
            {
                var interactorFactory = dependencyContainer.InteractorFactory;
                var syncInteractor = Application.IsInForeground
                    ? interactorFactory.RunPushNotificationInitiatedSyncInForeground()
                    : interactorFactory.RunPushNotificationInitiatedSyncInBackground();

                syncInteractor.Execute().Wait();
            }

            finishJobClearingPendingSyncJobLock(parameters);
        }

        private void finishJobClearingPendingSyncJobLock(JobParameters parameters)
        {
            clearPendingSyncJobLock();
            JobFinished(parameters, false);
        }

        private void clearPendingSyncJobLock()
        {
            AndroidDependencyContainer.Instance
                .KeyValueStorage
                .SetBool(HasPendingSyncJobServiceScheduledKey, false);
        }
    }
}
