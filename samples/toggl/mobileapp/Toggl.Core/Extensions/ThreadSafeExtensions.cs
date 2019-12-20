using System;
using Toggl.Core.Models.Interfaces;
using Toggl.Shared;
using Toggl.Storage;

namespace Toggl.Core.Extensions
{
    public static class ThreadSafeExtensions
    {
        private const string inaccessibleProjectColor = "#cecece";

        public static string DisplayName(this IThreadSafeProject project)
        {
            var name = project.Name ?? "";

            switch (project.SyncStatus)
            {
                case Storage.SyncStatus.RefetchingNeeded:
                    return Resources.InaccessibleProject;
                default:
                    return project.Active ? name : $"{name} {Resources.ArchivedProjectDecorator}";
            }
        }

        public static string DisplayColor(this IThreadSafeProject project)
        {
            switch (project.SyncStatus)
            {
                case Storage.SyncStatus.RefetchingNeeded:
                    return inaccessibleProjectColor;
                default:
                    return project.Color ?? "";
            }
        }

        public static TimeSpan? TimeSpanDuration(this IThreadSafeTimeEntry timeEntry)
            => timeEntry.Duration.HasValue
            ? TimeSpan.FromSeconds(timeEntry.Duration.Value)
            : (TimeSpan?)null;

        public static DateTimeOffset? StopTime(this IThreadSafeTimeEntry timeEntry)
            => timeEntry.Duration.HasValue
                ? timeEntry.Start + new TimeSpan(timeEntry.Duration.Value)
                : null as DateTimeOffset?;

        public static bool IsPlaceholder(this IThreadSafeProject project)
            => project?.SyncStatus == SyncStatus.RefetchingNeeded;

        public static bool IsPlaceholder(this IThreadSafeTask task)
            => task?.SyncStatus == SyncStatus.RefetchingNeeded;
    }
}
