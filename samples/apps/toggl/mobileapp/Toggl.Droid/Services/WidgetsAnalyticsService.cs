using Android.App;
using Android.Content;
using AndroidX.Core.App;
using Toggl.Droid.Extensions;
using static Toggl.Droid.Services.JobServicesConstants;

namespace Toggl.Droid.Widgets.Services
{
    [Service(Permission = "android.permission.BIND_JOB_SERVICE", Exported = true)]
    public sealed class WidgetsAnalyticsService : JobIntentService
    {
        public const string TimerWidgetInstallAction = nameof(TimerWidgetInstallAction);
        public const string TimerWidgetInstallStateParameter = nameof(TimerWidgetInstallStateParameter);

        public const string TimerWidgetResizeAction = nameof(TimerWidgetResizeAction);
        public const string TimerWidgetSizeParameter = nameof(TimerWidgetSizeParameter);

        public const string SuggestionsWidgetInstallAction = nameof(SuggestionsWidgetInstallAction);
        public const string SuggestionsWidgetInstallStateParameter = nameof(SuggestionsWidgetInstallStateParameter);

        public static void EnqueueWork(Context context, Intent intent)
        {
            var serviceClass = JavaUtils.ToClass<WidgetsAnalyticsService>();
            EnqueueWork(context, serviceClass, WidgetAnalyticsServiceJobId, intent);
        }

        protected override void OnHandleWork(Intent intent)
        {
            var action = intent.Action;
            switch (action)
            {
                case TimerWidgetInstallAction:
                    handleTrackTimerWidgetInstallState(intent);
                    break;
                case TimerWidgetResizeAction:
                    handleTrackTimerWidgetResize(intent);
                    break;
                case SuggestionsWidgetInstallAction:
                    handleTrackSuggestionsWidgetInstallState(intent);
                    break;
            }
        }

        private void handleTrackTimerWidgetInstallState(Intent intent)
        {
            var installationState = intent.GetBooleanExtra(TimerWidgetInstallStateParameter, false);
            var analyticsService = AndroidDependencyContainer.Instance.AnalyticsService;
            analyticsService.TimerWidgetInstallStateChange.Track(installationState);
        }

        private void handleTrackTimerWidgetResize(Intent intent)
        {
            var widgetSize = intent.GetIntExtra(TimerWidgetSizeParameter, 1);
            var analyticsService = AndroidDependencyContainer.Instance.AnalyticsService;
            analyticsService.TimerWidgetSizeChanged.Track(widgetSize);
        }

        private void handleTrackSuggestionsWidgetInstallState(Intent intent)
        {
            var installationState = intent.GetBooleanExtra(SuggestionsWidgetInstallStateParameter, false);
            var analyticsService = AndroidDependencyContainer.Instance.AnalyticsService;
            analyticsService.SuggestionsWidgetInstallStateChange.Track(installationState);
        }

    }
}
