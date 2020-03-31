using FluentAssertions;
using Microsoft.Reactive.Testing;
using NSubstitute;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using Toggl.Core.Models.Interfaces;
using Toggl.Core.Tests.Mocks;
using Toggl.Core.Tests.TestExtensions;
using Xunit;

namespace Toggl.Core.Tests.Interactors.Workspace
{
    public class ObserveAllWorkspacesInteractorTests
    {
        public sealed class TheObserveAllWorkspacesInteractor : BaseInteractorTests
        {
            [Fact, LogIfTooSlow]
            public void GetsAllChangesToWorkspaces()
            {
                var itemsChangedSubject = new Subject<Unit>();
                DataSource.Workspaces.ItemsChanged.Returns(itemsChangedSubject);

                var workspaces = Enumerable.Range(0, 10)
                    .Select(id => new MockWorkspace { Id = id });

                var workspaces2 = Enumerable.Range(20, 10)
                    .Select(id => new MockWorkspace { Id = id });

                DataSource.Workspaces.GetAll()
                    .Returns(
                        Observable.Return(workspaces),
                        Observable.Return(workspaces2));

                var testScheduler = new TestScheduler();
                var observer = testScheduler.CreateObserver<IEnumerable<IThreadSafeWorkspace>>();

                InteractorFactory.ObserveAllWorkspaces().Execute()
                    .Subscribe(observer);

                itemsChangedSubject.OnNext(Unit.Default);

                observer.Messages.Should().HaveCount(2);
                observer.Messages.First().Value.Value.Should().BeEquivalentTo(workspaces);
                observer.LastEmittedValue().Should().BeEquivalentTo(workspaces2);
            }

            [Fact, LogIfTooSlow]
            public void DoesntEmitIfWorkspacesDidntChange()
            {
                var itemsNeverChange = Observable.Never<Unit>();
                DataSource.Workspaces.ItemsChanged.Returns(itemsNeverChange);

                var workspaces = Enumerable.Range(0, 10)
                    .Select(id => new MockWorkspace { Id = id });
                DataSource.Workspaces.GetAll().Returns(Observable.Return(workspaces));

                var testScheduler = new TestScheduler();
                var observer = testScheduler.CreateObserver<IEnumerable<IThreadSafeWorkspace>>();

                InteractorFactory.ObserveAllWorkspaces().Execute()
                    .Subscribe(observer);

                observer.Messages.Should().HaveCount(1);
                observer.Messages.First().Value.Value.Should().BeEquivalentTo(workspaces);
            }
        }
    }
}
