using FluentAssertions;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using Toggl.Core.DataSources;
using Toggl.Core.Interactors;
using Toggl.Core.Models.Interfaces;
using Toggl.Core.Tests.Mocks;
using Toggl.Storage.Models;
using Xunit;

namespace Toggl.Core.Tests.Interactors.Sync
{
    public sealed class ContainsPlaceholdersInteractorTests
    {
        private readonly ITogglDataSource dataSource = Substitute.For<ITogglDataSource>();
        private readonly ContainsPlaceholdersInteractor interactor;

        public ContainsPlaceholdersInteractorTests()
        {
            interactor = new ContainsPlaceholdersInteractor(dataSource);
        }

        [Fact]
        public void ReturnsTrueWhenThereIsWorkspacePlaceholder()
        {
            hasWorkspacePlaceholders();
            hasNoProjectPlaceholders();
            hasNoTaskPlaceholders();
            hasNoTagPlaceholders();

            var hasPlaceholders = interactor.Execute().Wait();

            hasPlaceholders.Should().BeTrue();
        }

        [Fact]
        public void ReturnsTrueWhenThereIsProjectPlaceholder()
        {
            hasNoWorkspacePlaceholders();
            hasProjectPlaceholders();
            hasNoTaskPlaceholders();
            hasNoTagPlaceholders();

            var hasPlaceholders = interactor.Execute().Wait();

            hasPlaceholders.Should().BeTrue();
        }

        [Fact]
        public void ReturnsTrueWhenThereIsTaskPlaceholder()
        {
            hasNoWorkspacePlaceholders();
            hasNoProjectPlaceholders();
            hasTaskPlaceholders();
            hasNoTagPlaceholders();

            var placeholders = Observable.Return<IEnumerable<IThreadSafeTask>>(new[] { new MockTask() });
            dataSource.Tasks.GetAll(Arg.Any<Func<IDatabaseTask, bool>>()).Returns(placeholders);

            var hasPlaceholders = interactor.Execute().Wait();

            hasPlaceholders.Should().BeTrue();
        }

        [Fact]
        public void ReturnsTrueWhenThereIsTagPlaceholder()
        {
            hasNoWorkspacePlaceholders();
            hasNoProjectPlaceholders();
            hasNoTaskPlaceholders();
            hasTagPlaceholders();

            var placeholders = Observable.Return<IEnumerable<IThreadSafeTag>>(new[] { new MockTag() });
            dataSource.Tags.GetAll(Arg.Any<Func<IDatabaseTag, bool>>()).Returns(placeholders);

            var hasPlaceholders = interactor.Execute().Wait();

            hasPlaceholders.Should().BeTrue();
        }

        [Fact]
        public void ReturnsFalseWhenThereAreNoPlaceholders()
        {
            hasNoWorkspacePlaceholders();
            hasNoProjectPlaceholders();
            hasNoTaskPlaceholders();
            hasNoTagPlaceholders();

            var hasPlaceholders = interactor.Execute().Wait();

            hasPlaceholders.Should().BeFalse();
        }

        private void hasWorkspacePlaceholders()
        {
            var placeholders = Observable.Return<IEnumerable<IThreadSafeWorkspace>>(new[] { new MockWorkspace() });
            dataSource.Workspaces.GetAll(Arg.Any<Func<IDatabaseWorkspace, bool>>()).Returns(placeholders);
        }

        private void hasProjectPlaceholders()
        {
            var placeholders = Observable.Return<IEnumerable<IThreadSafeProject>>(new[] { new MockProject() });
            dataSource.Projects.GetAll(Arg.Any<Func<IDatabaseProject, bool>>()).Returns(placeholders);
        }

        private void hasTaskPlaceholders()
        {
            var placeholders = Observable.Return<IEnumerable<IThreadSafeTask>>(new[] { new MockTask() });
            dataSource.Tasks.GetAll(Arg.Any<Func<IDatabaseTask, bool>>()).Returns(placeholders);
        }

        private void hasTagPlaceholders()
        {
            var placeholders = Observable.Return<IEnumerable<IThreadSafeTag>>(new[] { new MockTag() });
            dataSource.Tags.GetAll(Arg.Any<Func<IDatabaseTag, bool>>()).Returns(placeholders);
        }

        private void hasNoWorkspacePlaceholders()
        {
            var noWorkspacePlaceholders = Observable.Return(new IThreadSafeWorkspace[0]);
            dataSource.Workspaces.GetAll(Arg.Any<Func<IDatabaseWorkspace, bool>>()).Returns(noWorkspacePlaceholders);
        }

        private void hasNoProjectPlaceholders()
        {
            var noProjectPlaceholders = Observable.Return(new IThreadSafeProject[0]);
            dataSource.Projects.GetAll(Arg.Any<Func<IDatabaseProject, bool>>()).Returns(noProjectPlaceholders);
        }

        private void hasNoTaskPlaceholders()
        {
            var noTaskPlaceholders = Observable.Return(new IThreadSafeTask[0]);
            dataSource.Tasks.GetAll(Arg.Any<Func<IDatabaseTask, bool>>()).Returns(noTaskPlaceholders);
        }

        private void hasNoTagPlaceholders()
        {
            var noTagPlaceholders = Observable.Return(new IThreadSafeTag[0]);
            dataSource.Tags.GetAll(Arg.Any<Func<IDatabaseTag, bool>>()).Returns(noTagPlaceholders);
        }
    }
}
