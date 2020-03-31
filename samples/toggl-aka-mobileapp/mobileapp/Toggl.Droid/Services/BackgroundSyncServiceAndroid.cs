using Android.App;
using Android.App.Job;
using Android.Content;
using Toggl.Core.Services;
using Toggl.Droid.Extensions;
using Toggl.Droid.Helper;

namespace Toggl.Droid.Services
{
    public sealed class BackgroundSyncServiceAndroid : BaseBackgroundSyncService
    {
        public override void EnableBackgroundSync()
        {
            // Background sync is temporary disabled for android 10 due to a crash on Android 10 devices
            if (QApis.AreAvailable)
            {
                DisableBackgroundSync();
                return;
            }

            var context = Application.Context;
            var jobScheduler = (JobScheduler)context.GetSystemService(Context.JobSchedulerService);
            var periodicity = (long)MinimumBackgroundFetchInterval.TotalMilliseconds;
            var jobInfo = context.CreateBackgroundSyncJobInfo(periodicity);
            jobScheduler.Schedule(jobInfo);
        }

        public override void DisableBackgroundSync()
        {
            var context = Application.Context;
            var jobScheduler = (JobScheduler)context.GetSystemService(Context.JobSchedulerService);
            jobScheduler.Cancel(BackgroundSyncJobSchedulerService.JobId);
        }
    }
}
