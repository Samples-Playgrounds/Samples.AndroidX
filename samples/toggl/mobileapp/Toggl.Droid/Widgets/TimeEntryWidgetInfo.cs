using System;
using Android.App;
using Android.Content;
using Toggl.Core.Models.Interfaces;
using Toggl.Shared.Extensions;

namespace Toggl.Droid.Widgets
{
    public sealed class TimeEntryWidgetInfo
    {
        public static string WidgetInfoSharedPreferencesName = "WidgetInfo";

        public const string IsRunningKey = "IsRunning";
        public const string StartTimeKey = "StartTime";
        public const string DescriptionKey = "Description";
        public const string HasProjectKey = "HasProject";
        public const string ProjectNameKey = "ProjectName";
        public const string ProjectColorKey = "ProjectColor";
        public const string HasClientKey = "HasClient";
        public const string ClientNameKey = "ClientName";

        public DateTimeOffset StartTime { get; private set; }
        public string Description { get; private set; }
        public bool HasProject { get; private set; }
        public string ProjectName { get; private set; }
        public string ProjectColor { get; private set; }
        public bool HasClient { get; private set; }
        public string ClientName { get; private set; }
        public bool IsRunning { get; private set; }

        private TimeEntryWidgetInfo() { }

        public static TimeEntryWidgetInfo FromSharedPreferences()
        {
            var sharedPreferences = Application.Context.GetSharedPreferences(WidgetInfoSharedPreferencesName, FileCreationMode.Private);

            var info = new TimeEntryWidgetInfo();
            info.IsRunning = sharedPreferences.GetBoolean(IsRunningKey, false);
            info.StartTime = DateTimeOffset.FromUnixTimeSeconds(sharedPreferences.GetLong(StartTimeKey, default));
            info.Description = sharedPreferences.GetString(DescriptionKey, "");
            info.HasProject = sharedPreferences.GetBoolean(HasProjectKey, false);
            info.ProjectName = sharedPreferences.GetString(ProjectNameKey, "");
            info.ProjectColor = sharedPreferences.GetString(ProjectColorKey, "");
            info.HasClient = sharedPreferences.GetBoolean(HasClientKey, false);
            info.ClientName = sharedPreferences.GetString(ClientNameKey, "");

            return info;
        }

        public static void Save(IThreadSafeTimeEntry timeEntry)
        {
            var sharedPreferences = Application.Context.GetSharedPreferences(WidgetInfoSharedPreferencesName, FileCreationMode.Private);
            var editor = sharedPreferences.Edit();
            editor.PutBoolean(IsRunningKey, timeEntry?.IsRunning() ?? false);
            editor.PutLong(StartTimeKey, timeEntry?.Start.ToUnixTimeSeconds() ?? default);
            editor.PutString(DescriptionKey, timeEntry?.Description ?? "");
            editor.PutBoolean(HasProjectKey, timeEntry?.ProjectId.HasValue ?? false);
            editor.PutString(ProjectNameKey, timeEntry?.Project?.Name ?? "");
            editor.PutString(ProjectColorKey, timeEntry?.Project?.Color ?? "");
            editor.PutBoolean(HasClientKey, timeEntry?.Project?.ClientId.HasValue ?? false);
            editor.PutString(ClientNameKey, timeEntry?.Project?.Client?.Name ?? "");
            editor.Apply();
        }

        public static void Clear()
        {
            var sharedPreferences = Application.Context.GetSharedPreferences(WidgetInfoSharedPreferencesName, FileCreationMode.Private);
            var editor = sharedPreferences.Edit();
            editor.Remove(IsRunningKey);
            editor.Remove(StartTimeKey);
            editor.Remove(DescriptionKey);
            editor.Remove(HasProjectKey);
            editor.Remove(ProjectNameKey);
            editor.Remove(ProjectColorKey);
            editor.Remove(HasClientKey);
            editor.Remove(ClientNameKey);
            editor.Commit();
        }
    }
}
