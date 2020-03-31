using System;

namespace Toggl.Storage.Settings
{
    public interface ILastTimeUsageStorage
    {
        DateTimeOffset? LastSyncAttempt { get; }

        DateTimeOffset? LastSuccessfulSync { get; }

        DateTimeOffset? LastLogin { get; }

        DateTimeOffset? LastTimePlaceholdersWereCreated { get; }

        void SetFullSyncAttempt(DateTimeOffset now);

        void SetSuccessfulFullSync(DateTimeOffset now);

        void SetLogin(DateTimeOffset now);

        void SetPlaceholdersWereCreated(DateTimeOffset now);
    }
}
