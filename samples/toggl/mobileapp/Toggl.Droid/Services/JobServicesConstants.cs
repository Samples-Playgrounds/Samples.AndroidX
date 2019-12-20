namespace Toggl.Droid.Services
{
    public static class JobServicesConstants
    {
        public const int SyncJobServiceJobId = 1111;
        public const int BackgroundSyncJobServiceJobId = 1;
        public const int WidgetAnalyticsServiceJobId = 1001;
        public const int WidgetForegroundServiceJobId = 1002;

        public const string HasPendingSyncJobServiceScheduledKey = "HasPendingSyncJobServiceScheduledKey";
        public const string LastSyncJobScheduledAtKey = "LastSyncJobScheduledAtKey";
    }
}
