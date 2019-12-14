using Android.App;
using Android.Content;
using System;
using AndroidX.Core.App;
using Toggl.Droid.Extensions;

namespace Toggl.Droid.Services
{
    [Service(
        Exported = true,
        Permission = "android.permission.BIND_JOB_SERVICE",
        Name = "com.toggl.giskard.ScheduleEventNotificationsService")]
    public class ScheduleEventNotificationsService : JobIntentService
    {
        private IDisposable disposable;

        public const int JobId = 42;

        public static void EnqueueWork(Context context, Intent intent)
        {
            var componentName = new ComponentName(context, JavaUtils.ToClass<ScheduleEventNotificationsService>());
            EnqueueWork(context, componentName, JobId, intent);
        }

        protected override void OnHandleWork(Intent intent)
        {
            var dependencyContainer = AndroidDependencyContainer.Instance;

            if (!dependencyContainer.UserAccessManager.CheckIfLoggedIn())
                return;

            var interactorFactory = dependencyContainer.InteractorFactory;

            disposable?.Dispose();
            disposable = null;

            disposable = interactorFactory
                .ScheduleEventNotificationsForNextWeek()
                .Execute()
                .Subscribe();
        }

        public override void OnDestroy()
        {
            base.OnDestroy();

            disposable?.Dispose();
            disposable = null;
        }
    }
}
