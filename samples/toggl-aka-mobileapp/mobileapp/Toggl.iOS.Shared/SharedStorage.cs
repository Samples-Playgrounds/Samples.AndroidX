using System;
using Foundation;
using Toggl.iOS.Shared.Analytics;
using Toggl.iOS.Shared.Extensions;
using Toggl.iOS.Shared.Models;
using Toggl.Shared.Models;

namespace Toggl.iOS.Shared
{
    public sealed partial class SharedStorage
    {
        private const string apiTokenKey = "APITokenKey";
        private const string needsSyncKey = "NeedsSyncKey";
        private const string userIdKey = "UserId";
        private const string siriTrackingEventsKey = "SiriTrackingEventsKey";
        private const string widgetTrackingEventsKey = "WidgetTrackingEventsKey";
        private const string defaultWorkspaceId = "DefaultWorkspaceId";
        private const string widgetUpdatedDateKey = "WidgetUpdatedDate";
        private const string widgetInstalledKey = "WidgetInstalled";
        private const string runningTimeEntry = "RunningTimeEntry";
        private const string durationFormatKey = "DurationFormat";
        private const string suggestionsKey = "Suggestions";

        private NSUserDefaults userDefaults;

        private SharedStorage()
        {
            var bundleId = NSBundle.MainBundle.BundleIdentifier;
            if (bundleId.Contains("SiriExtension") || bundleId.Contains("TimerWidgetExtension"))
            {
                bundleId = bundleId.Substring(0, bundleId.LastIndexOf("."));
            }
            userDefaults = new NSUserDefaults($"group.{bundleId}.extensions", NSUserDefaultsType.SuiteName);
        }

        public static SharedStorage Instance { get; } = new SharedStorage();

        public void SetApiToken(string apiToken)
        {
            userDefaults.SetString(apiToken, apiTokenKey);
            userDefaults.Synchronize();
        }

        public void SetNeedsSync(bool value)
        {
            userDefaults.SetBool(value, needsSyncKey);
            userDefaults.Synchronize();
        }

        public void SetUserId(double userId)
        {
            userDefaults.SetDouble(userId, userIdKey);
            userDefaults.Synchronize();
        }

        public void SetDefaultWorkspaceId(long workspaceId)
        {
            userDefaults.SetDouble(workspaceId, defaultWorkspaceId);
            userDefaults.Synchronize();
        }

        public void SetDurationFormat(int durationFormat)
        {
            userDefaults.SetInt(durationFormat, durationFormatKey);
            userDefaults.Synchronize();
        }

        public void AddSiriTrackingEvent(SiriTrackingEvent e)
        {
            var currentEvents = (NSMutableArray) getTrackableEvents(siriTrackingEventsKey).MutableCopy();
            currentEvents.Add(e);

            userDefaults[siriTrackingEventsKey] = NSKeyedArchiver.ArchivedDataWithRootObject(currentEvents);
            userDefaults.Synchronize();
        }

        public void AddWidgetTrackingEvent(WidgetTrackingEvent e)
        {
            var currentEvents = (NSMutableArray) getTrackableEvents(widgetTrackingEventsKey).MutableCopy();
            currentEvents.Add(e);

            userDefaults[widgetTrackingEventsKey] = NSKeyedArchiver.ArchivedDataWithRootObject(currentEvents);
            userDefaults.Synchronize();
        }

        public SiriTrackingEvent[] PopSiriTrackableEvents()
        {
            var eventArrays = getTrackableEvents(siriTrackingEventsKey);
            userDefaults.RemoveObject(siriTrackingEventsKey);
            return NSArray.FromArrayNative<SiriTrackingEvent>(eventArrays);
        }

        public WidgetTrackingEvent[] PopWidgetTrackableEvents()
        {
            var eventArrays = getTrackableEvents(widgetTrackingEventsKey);
            userDefaults.RemoveObject(widgetTrackingEventsKey);
            return NSArray.FromArrayNative<WidgetTrackingEvent>(eventArrays);
        }

        public double GetUserId() => userDefaults.DoubleForKey(userIdKey);

        public string GetApiToken() => userDefaults.StringForKey(apiTokenKey);

        public bool GetNeedsSync() => userDefaults.BoolForKey(needsSyncKey);

        public long GetDefaultWorkspaceId() => (long)userDefaults.DoubleForKey(defaultWorkspaceId);

        public int GetDurationFormat() => (int) userDefaults.IntForKey(durationFormatKey);

        public void DeleteEverything()
        {
            userDefaults.RemoveObject(apiTokenKey);
            userDefaults.RemoveObject(needsSyncKey);
            userDefaults.RemoveObject(userIdKey);
            userDefaults.RemoveObject(siriTrackingEventsKey);
            userDefaults.RemoveObject(widgetTrackingEventsKey);
            userDefaults.RemoveObject(defaultWorkspaceId);
            userDefaults.RemoveObject(widgetUpdatedDateKey);
            userDefaults.RemoveObject(widgetInstalledKey);
            userDefaults.RemoveObject(runningTimeEntry);
            userDefaults.RemoveObject(durationFormatKey);
            userDefaults.RemoveObject(suggestionsKey);
            userDefaults.Synchronize();
        }

        public void SetWidgetUpdatedDate(DateTimeOffset? date)
        {
            if (date.HasValue)
            {
                userDefaults[widgetUpdatedDateKey] = date.Value.ToNSDate();
            }
            else
            {
                userDefaults.RemoveObject(widgetUpdatedDateKey);
            }

            userDefaults.Synchronize();
        }

        public DateTimeOffset? GetWidgetUpdatedDate()
        {
            var date = userDefaults[widgetUpdatedDateKey] as NSDate;

            if (date == null)
                return null;

            return date.ToDateTimeOffset();
        }

        public void SetWidgetInstalled(bool installed)
        {
            userDefaults.SetBool(installed, widgetInstalledKey);
            userDefaults.Synchronize();
        }

        public bool GetWidgetInstalled() => userDefaults.BoolForKey(widgetInstalledKey);

        private NSArray getTrackableEvents(string key)
        {
            var eventArrayData = userDefaults.ValueForKey(new NSString(key)) as NSData;

            if (eventArrayData == null)
                return new NSArray();

            return NSKeyedUnarchiver.UnarchiveObject(eventArrayData) as NSArray;
        }
    }
}
