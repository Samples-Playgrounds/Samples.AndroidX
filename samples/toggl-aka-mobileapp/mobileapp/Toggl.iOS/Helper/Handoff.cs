using System;
using Foundation;

namespace Toggl.iOS.Helper
{
    public static class Handoff
    {
        public static class Url
        {
#if USE_PRODUCTION_API
            private const string domain = "toggl.com";
#else
            private const string domain = "toggl.space";
#endif
            private const string dateFormat = "yyyy-MM-dd";

            public static NSUrl Log { get; } = new NSUrl($"https://{domain}/app");

            public static NSUrl Settings { get; } = new NSUrl($"https://{domain}/app/profile");

            public static NSUrl Reports(long workspaceId, DateTimeOffset start, DateTimeOffset end)
                => new NSUrl($"https://{domain}/app/reports/summary/{workspaceId}/from/{start.ToString(dateFormat)}/to/{end.ToString(dateFormat)}");
        }

        public static class Action
        {
            public const string Log = "com.toggl.daneel.log";
            public const string Reports = "com.toggl.daneel.reports";
            public const string Settings = "com.toggl.daneel.settings";
        }
    }
}
