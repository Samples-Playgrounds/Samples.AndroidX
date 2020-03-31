using Toggl.Core.Models.Interfaces;
using Toggl.Core.Tests.Mocks;
using Toggl.Networking.Models;
using Toggl.Shared;
using Toggl.Shared.Models;
using Toggl.Storage;

namespace Toggl.Core.Tests.Sync.Extensions
{
    public static class IPreferencesExtensions
    {
        public static IPreferences With(this IPreferences preferences,
            New<DurationFormat> durationFormat = default(New<DurationFormat>))
            => new Preferences
            {
                TimeOfDayFormat = preferences.TimeOfDayFormat,
                DateFormat = preferences.DateFormat,
                DurationFormat = durationFormat.ValueOr(preferences.DurationFormat),
                CollapseTimeEntries = preferences.CollapseTimeEntries
            };

        public static IThreadSafePreferences ToSyncable(this IPreferences preferences)
            => new MockPreferences
            {
                CollapseTimeEntries = preferences.CollapseTimeEntries,
                DateFormat = preferences.DateFormat,
                DurationFormat = preferences.DurationFormat,
                Id = 0,
                IsDeleted = false,
                LastSyncErrorMessage = null,
                SyncStatus = SyncStatus.InSync,
                TimeOfDayFormat = preferences.TimeOfDayFormat
            };
    }
}
