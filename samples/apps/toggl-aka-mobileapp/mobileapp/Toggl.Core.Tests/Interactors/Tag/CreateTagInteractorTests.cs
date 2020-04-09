using FluentAssertions;
using FsCheck;
using FsCheck.Xunit;
using NSubstitute;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reactive.Linq;
using Toggl.Core.Interactors;
using Toggl.Core.Models.Interfaces;
using Toggl.Core.Tests.Generators;
using Toggl.Core.Tests.Mocks;
using Toggl.Storage;
using Toggl.Storage.Models;
using Xunit;

namespace Toggl.Core.Tests.Interactors
{
    public sealed class CreateTagInteractorTests
    {
        public abstract class CreateTagInteractorTest : BaseInteractorTests
        {
            [Theory, LogIfTooSlow]
            [ConstructorData]
            public void ThrowsIfAnyOfTheArgumentsIsNull(
                bool useIdProvider,
                bool useTimeService,
                bool useDataSource,
                bool useTagName,
                bool useWorkspaceId)
            {
                var tagName = useTagName ? "Something" : null;
                var idProvider = useIdProvider ? IdProvider : null;
                var timeService = useTimeService ? TimeService : null;
                var dataSource = useDataSource ? DataSource.Tags : null;
                var workspaceId = useWorkspaceId ? 11 : 0;

                Action tryingToConstructWithEmptyParameters =
                    () => new CreateTagInteractor(
                        idProvider,
                        timeService,
                        dataSource,
                        tagName,
                        workspaceId);

                tryingToConstructWithEmptyParameters
                    .Should().Throw<ArgumentNullException>();
            }
        }

        public sealed class TheExecuteMethod : BaseInteractorTests
        {
            private CreateTagInteractor createTagInteractor(string tagName, long workspaceId)
                => new CreateTagInteractor(
                        IdProvider,
                        TimeService,
                        DataSource.Tags,
                        tagName,
                        workspaceId);

            [Property]
            public void CreatesTagWithIdFromIdProvider(long nextId)
            {
                IdProvider.GetNextIdentifier().Returns(nextId);

                createTagInteractor("Some tag", 10).Execute().Wait();

                DataSource.Tags.Received().Create(
                    Arg.Is<IThreadSafeTag>(tag => tag.Id == nextId)
                ).Wait();
            }

            [Property]
            public void CreatesTagWithPassedName(NonEmptyString nonEmptyString)
            {
                var tagName = nonEmptyString.Get;

                createTagInteractor(tagName, 10).Execute().Wait();

                DataSource.Tags.Received().Create(
                    Arg.Is<IThreadSafeTag>(tag => tag.Name == tagName)
                ).Wait();
            }

            [Property]
            public void CreatesTagWithPassedWorkspaceId(NonZeroInt nonZeroint)
            {
                var workspaceId = nonZeroint.Get;
                createTagInteractor("Some tag", workspaceId).Execute().Wait();

                DataSource.Tags.Received().Create(
                    Arg.Is<IThreadSafeTag>(tag => tag.WorkspaceId == workspaceId)
                ).Wait();
            }

            [Property]
            public void CreatesTagWithAtDateFromTimeServiceCurrentDateTime(
                DateTimeOffset currentTime)
            {
                TimeService.CurrentDateTime.Returns(currentTime);

                createTagInteractor("Some tag", 100).Execute().Wait();

                DataSource.Tags.Received().Create(
                    Arg.Is<IThreadSafeTag>(tag => tag.At == currentTime)
                ).Wait();
            }

            [Property]
            public void CreatesTagWithSyncNeeded(
                NonEmptyString name, NonZeroInt workspaceId)
            {
                createTagInteractor(name.Get, workspaceId.Get).Execute().Wait();

                DataSource.Tags.Received().Create(
                    Arg.Is<IThreadSafeTag>(tag => tag.SyncStatus == SyncStatus.SyncNeeded)
                ).Wait();
            }

            [Property]
            public void DoesNotCreateTheTagIfItAlreadyExists(NonEmptyString name, NonZeroInt workspaceId)
            {
                var mockTag = new MockTag
                {
                    Name = name.Get,
                    WorkspaceId = workspaceId.Get
                };

                DataSource.Tags
                    .GetAll(Arg.Any<Func<IDatabaseTag, bool>>())
                    .Returns(Observable.Return<IEnumerable<IThreadSafeTag>>(new[] { mockTag }));

                var createdTag = createTagInteractor(name.Get, workspaceId.Get).Execute().Wait();

                DataSource.Tags.DidNotReceive().Create(
                    Arg.Is<IThreadSafeTag>(tag => tag.SyncStatus == SyncStatus.SyncNeeded)
                ).Wait();

                createdTag.Should().BeNull();
            }
        }
    }
}
