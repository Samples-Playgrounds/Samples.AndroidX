using FluentAssertions;
using NSubstitute;
using System;
using Toggl.Core.Models;
using Toggl.Core.Models.Interfaces;
using Toggl.Storage;
using Xunit;

namespace Toggl.Core.Tests.SyncModel
{
    public sealed class SyncFailureItemTests
    {
        public sealed class TheConstructor
        {
            [Fact, LogIfTooSlow]
            public void ThrowsIfTheArgumentIsNull()
            {
                Action tryingToConstructWithEmptyParameters =
                    () => new SyncFailureItem(null);

                tryingToConstructWithEmptyParameters
                    .Should().Throw<ArgumentNullException>();
            }

            [Fact, LogIfTooSlow]
            public void SetsProjectTypeIfConstructedWithAProject()
            {
                var project = Substitute.For<IThreadSafeProject>();
                project.Name.Returns("My Project");

                SyncFailureItem syncFailure = new SyncFailureItem(project);

                syncFailure.Type.Should().Be(ItemType.Project);
            }

            [Fact, LogIfTooSlow]
            public void SetsTagTypeIfConstructedWithATag()
            {
                var tag = Substitute.For<IThreadSafeTag>();
                tag.Name.Returns("My Tag");

                SyncFailureItem syncFailure = new SyncFailureItem(tag);

                syncFailure.Type.Should().Be(ItemType.Tag);
            }

            [Fact, LogIfTooSlow]
            public void SetsClientTypeIfConstructedWithAClient()
            {
                var client = Substitute.For<IThreadSafeClient>();
                client.Name.Returns("My Client");

                SyncFailureItem syncFailure = new SyncFailureItem(client);

                syncFailure.Type.Should().Be(ItemType.Client);
            }

            [Fact, LogIfTooSlow]
            public void SetsTheCorrectProperties()
            {
                var tag = Substitute.For<IThreadSafeClient>();
                tag.Name.Returns("My Client");
                tag.SyncStatus.Returns(SyncStatus.SyncFailed);
                tag.LastSyncErrorMessage.Returns("Something bad happened");

                SyncFailureItem syncFailure = new SyncFailureItem(tag);

                syncFailure.Name.Should().Be("My Client");
                syncFailure.SyncStatus.Should().Be(SyncStatus.SyncFailed);
                syncFailure.SyncErrorMessage.Should().Be("Something bad happened");
            }
        }
    }
}
