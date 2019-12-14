using FluentAssertions;
using System;
using Toggl.Core.Sync.ConflictResolution;
using Toggl.Storage;
using Xunit;

namespace Toggl.Core.Tests.Sync.ConflictResolution
{
    public sealed class AlwaysOverwriteTests
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

        [Fact, LogIfTooSlow]
        public void UpdateWhenThereIsAnExistingEntityLocally()
        {
            var existingEntity = new TestModel();
            var incomingEntity = new TestModel();

            var mode = resolver.Resolve(existingEntity, incomingEntity);

            mode.Should().Be(ConflictResolutionMode.Update);
        }

        [Fact, LogIfTooSlow]
        public void CreateNewWhenThereIsNoExistingEntity()
        {
            var incomingEntity = new TestModel();

            var mode = resolver.Resolve(null, incomingEntity);

            mode.Should().Be(ConflictResolutionMode.Create);
        }

        private sealed class TestModel
        {
        }

        private AlwaysOverwrite<TestModel> resolver { get; }
            = new AlwaysOverwrite<TestModel>();
    }
}
