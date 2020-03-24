using Toggl.Core.Models.Interfaces;
using Toggl.Shared;
using Toggl.Shared.Models;
using Toggl.Storage;
using Toggl.Storage.Models;

namespace Toggl.Core.Models
{
    internal class Preferences : IThreadSafePreferences
    {
        public TimeFormat TimeOfDayFormat { get; }
        public DateFormat DateFormat { get; }
        public DurationFormat DurationFormat { get; }
        public bool CollapseTimeEntries { get; }
        public SyncStatus SyncStatus { get; }
        public string LastSyncErrorMessage { get; }
        public bool IsDeleted { get; }

        public const long fakeId = 0;
        public long Id => fakeId;

        private Preferences(IPreferences entity, SyncStatus syncStatus, string lastSyncErrorMessage, bool isDeleted = false)
            : this(entity.TimeOfDayFormat, entity.DateFormat, entity.DurationFormat, entity.CollapseTimeEntries, syncStatus, lastSyncErrorMessage, isDeleted)
        { }

        public Preferences(TimeFormat timeOfDayFormat, DateFormat dateFormat, DurationFormat durationFormat, bool collapseTimeEntries, SyncStatus syncStatus = default(SyncStatus), string lastSyncErrorMessage = "", bool isDeleted = false)
        {
            Ensure.Argument.IsADefinedEnumValue(syncStatus, nameof(syncStatus));
            Ensure.Argument.IsNotNull(dateFormat.Localized, nameof(dateFormat));
            Ensure.Argument.IsNotNull(timeOfDayFormat.Localized, nameof(timeOfDayFormat));

            TimeOfDayFormat = timeOfDayFormat;
            DateFormat = dateFormat;
            DurationFormat = durationFormat;
            CollapseTimeEntries = collapseTimeEntries;
            SyncStatus = syncStatus;
            LastSyncErrorMessage = lastSyncErrorMessage;
            IsDeleted = isDeleted;
        }

        public static Preferences From(IDatabasePreferences entity)
        {
            return new Preferences(entity, entity.SyncStatus, entity.LastSyncErrorMessage, entity.IsDeleted);
        }

        public static Preferences Clean(IPreferences entity)
            => new Preferences(entity, SyncStatus.InSync, null);

        public static Preferences Unsyncable(IPreferences entity, string errorMessage)
            => new Preferences(entity, SyncStatus.SyncFailed, errorMessage);

        public static Preferences DefaultPreferences { get; } =
            new Preferences(
                TimeFormat.FromLocalizedTimeFormat("H:mm"),
                DateFormat.FromLocalizedDateFormat("DD.MM.YYYY"),
                DurationFormat.Improved,
                false
            );
    }
}
