using Toggl.Core.Models.Interfaces;
using Toggl.Shared;
using Toggl.Storage;

namespace Toggl.Core.Models
{
    internal static class ModelExtensions
    {
        public static IThreadSafePreferences With(
            this IThreadSafePreferences original,
            New<TimeFormat> timeOfDayFormat = default(New<TimeFormat>),
            New<DateFormat> dateFormat = default(New<DateFormat>),
            New<DurationFormat> durationFormat = default(New<DurationFormat>),
            New<bool> collapseTimeEntries = default(New<bool>),
            New<SyncStatus> syncStatus = default(New<SyncStatus>),
            New<string> lastSyncErrorMessage = default(New<string>),
            New<bool> isDeleted = default(New<bool>)
        )
            => new Preferences(
                timeOfDayFormat.ValueOr(original.TimeOfDayFormat),
                dateFormat.ValueOr(original.DateFormat),
                durationFormat.ValueOr(original.DurationFormat),
                collapseTimeEntries.ValueOr(original.CollapseTimeEntries),
                syncStatus.ValueOr(original.SyncStatus),
                lastSyncErrorMessage.ValueOr(original.LastSyncErrorMessage),
                isDeleted.ValueOr(original.IsDeleted)
            );
    }
}
