using FluentAssertions;
using FsCheck.Xunit;
using System;
using Toggl.Core.Sync.ConflictResolution;
using Toggl.Shared.Models;
using Toggl.Storage;
using Xunit;

namespace Toggl.Core.Tests.Sync.ConflictResolution
{
    public sealed class PreferNewerTests
    {
        [Fact, LogIfTooSlow]
        public void ThrowsWhenIncomingEntityIsNull()
        {
            var existingEntity = new TestModel();

            Action resolving = () => resolver.Resolve(null, null);
            Action resolvingWithExistingLocalEntity = () => resolver.Resolve(existingEntity, null);

            resolving.Should().Throw<ArgumentNullException>();
            resolvingWithExistingLocalEntity.Should().Throw<ArgumentNullException>();
        }

        [Property]
        public void IgnoreOutdatedIncomingDataWhenLocalEntityIsDirty(DateTimeOffset existing, DateTimeOffset incoming)
        {
            if (existing <= incoming)
                (existing, incoming) = (incoming, existing);
            var existingEntity = new TestModel(existing, syncStatus: SyncStatus.SyncNeeded);
            var incomingEntity = new TestModel(incoming);

            var mode = resolver.Resolve(existingEntity, incomingEntity);

            mode.Should().Be(ConflictResolutionMode.Ignore);
        }

        [Property]
        public void CreateNewWhenThereIsNoExistingEntity(DateTimeOffset at)
        {
            var incomingEntity = new TestModel(at);

            var mode = resolver.Resolve(null, incomingEntity);

            mode.Should().Be(ConflictResolutionMode.Create);
        }

        [Property]
        public void DeleteWhenTheIncomingDataHasSomeServerDeletedAt(DateTimeOffset existing, DateTimeOffset incoming, DateTimeOffset serverDeletedAt)
        {
            var existingEntity = new TestModel(existing);
            var incomingEntity = new TestModel(incoming, serverDeletedAt);

            var mode = resolver.Resolve(existingEntity, incomingEntity);

            mode.Should().Be(ConflictResolutionMode.Delete);
        }

        [Property]
        public void DeleteWhenTheIncomingDataHasSomeServerDeletedAtEvenWhenLocalEntityIsDirty(DateTimeOffset existing, DateTimeOffset incoming, DateTimeOffset serverDeletedAt)
        {
            var existingEntity = new TestModel(existing, syncStatus: SyncStatus.SyncNeeded);
            var incomingEntity = new TestModel(incoming, serverDeletedAt);

            var mode = resolver.Resolve(existingEntity, incomingEntity);

            mode.Should().Be(ConflictResolutionMode.Delete);
        }

        [Property]
        public void IgnoreWhenTheIncomingDataHasSomeServerDeletedAtButThereIsNoExistingEntity(DateTimeOffset incoming, DateTimeOffset serverDeletedAt)
        {
            var incomingEntity = new TestModel(incoming, serverDeletedAt);

            var mode = resolver.Resolve(null, incomingEntity);

            mode.Should().Be(ConflictResolutionMode.Ignore);
        }

        [Property]
        public void ThrowAwayIncommingIfUserMadeChangesLocally(DateTimeOffset existing, int seed)
        {
            DateTimeOffset incoming = existing.Add(randomTimeSpan(seed, resolver.MarginOfError.TotalSeconds));
            var existingEntity = new TestModel(existing, syncStatus: SyncStatus.SyncNeeded);
            var incomingEntity = new TestModel(incoming);

            var mode = resolver.Resolve(existingEntity, incomingEntity);

            mode.Should().Be(ConflictResolutionMode.Ignore);
        }

        [Property]
        public void UpdateWhenIncomingChangeIsNewerThanExising(DateTimeOffset existing, DateTimeOffset incoming)
        {
            if (existing > incoming)
                (existing, incoming) = (incoming, existing);
            var existingEntity = new TestModel(existing, syncStatus: SyncStatus.SyncNeeded);
            var incomingEntity = new TestModel(incoming);

            var mode = resolver.Resolve(existingEntity, incomingEntity);

            mode.Should().Be(ConflictResolutionMode.Update);
        }

        [Property]
        public void UpdateWhenIncomingChangeIsNewerThanExisingConsideringTheMarginOfError(DateTimeOffset existing, DateTimeOffset incoming)
        {
            if (existing > incoming.Subtract(resolver.MarginOfError))
                (existing, incoming) = (incoming, existing);
            var existingEntity = new TestModel(existing, syncStatus: SyncStatus.SyncNeeded);
            var incomingEntity = new TestModel(incoming);

            var mode = resolver.Resolve(existingEntity, incomingEntity);

            mode.Should().Be(ConflictResolutionMode.Update);
        }

        [Property]
        public void UpdateIfUserMadeChangesLocallyRecentlyButThereIsNoMarginOfError(DateTimeOffset existing, int seed)
        {
            DateTimeOffset incoming = existing.Add(randomTimeSpan(seed, 1));
            var existingEntity = new TestModel(existing, syncStatus: SyncStatus.SyncNeeded);
            var incomingEntity = new TestModel(incoming);

            var mode = zeroMarginOfErrorResolver.Resolve(existingEntity, incomingEntity);

            mode.Should().Be(ConflictResolutionMode.Update);
        }

        [Property]
        public void AlwaysOverrideNonDeletedCleanEntity(DateTimeOffset existing, DateTimeOffset incoming)
        {
            if (existing <= incoming)
                (existing, incoming) = (incoming, existing);

            var existingEntity = new TestModel(existing, syncStatus: SyncStatus.InSync);
            var incomingEntity = new TestModel(incoming, deleted: null);

            var mode = resolver.Resolve(existingEntity, incomingEntity);

            mode.Should().Be(ConflictResolutionMode.Update);
        }

        [Property]
        public void AlwaysOverrideNonDeletedEntityWhichNeedsRefetch(DateTimeOffset existing, DateTimeOffset incoming)
        {
            if (existing <= incoming)
                (existing, incoming) = (incoming, existing);

            var existingEntity = new TestModel(existing, syncStatus: SyncStatus.RefetchingNeeded);
            var incomingEntity = new TestModel(incoming, deleted: null);

            var mode = resolver.Resolve(existingEntity, incomingEntity);

            mode.Should().Be(ConflictResolutionMode.Update);
        }

        private sealed class TestModel : IDeletable, IDatabaseSyncable, ILastChangedDatable
        {
            private readonly DateTimeOffset now = new DateTimeOffset(2017, 01, 05, 12, 34, 56, TimeSpan.Zero);

            public SyncStatus SyncStatus { get; }
            public string LastSyncErrorMessage { get; }
            public DateTimeOffset At { get; }
            public DateTimeOffset? ServerDeletedAt { get; }
            public bool IsDeleted => throw new NotImplementedException();

            public TestModel(DateTimeOffset? at = null, DateTimeOffset? deleted = null, SyncStatus syncStatus = SyncStatus.InSync)
            {
                At = at ?? now;
                ServerDeletedAt = deleted;
                SyncStatus = syncStatus;
            }
        }

        private TimeSpan randomTimeSpan(int seed, double max)
        {
            var lessThanMarginOfErrorSeconds = (new Random(seed)).NextDouble() * max;
            return TimeSpan.FromSeconds(lessThanMarginOfErrorSeconds);
        }

        private PreferNewer<TestModel> resolver { get; }
            = new PreferNewer<TestModel>(TimeSpan.FromSeconds(5));

        private PreferNewer<TestModel> zeroMarginOfErrorResolver { get; }
            = new PreferNewer<TestModel>();
    }
}
