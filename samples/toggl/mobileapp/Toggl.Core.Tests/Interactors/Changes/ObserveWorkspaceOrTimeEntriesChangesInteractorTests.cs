using FluentAssertions;
using Microsoft.Reactive.Testing;
using NSubstitute;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using Xunit;

namespace Toggl.Core.Tests.Interactors.Changes
{
    public class ObserveWorkspaceOrTimeEntriesChangesInteractorTests : BaseInteractorTests
    {
        [Fact, LogIfTooSlow]
        public void GetsAnEventWhenAChangeToWorkspacesHappens()
        {
            var itemsChangedSubject = new Subject<Unit>();
            DataSource.Workspaces.ItemsChanged.Returns(itemsChangedSubject);
            DataSource.TimeEntries.ItemsChanged.Returns(Observable.Never<Unit>());

            var testScheduler = new TestScheduler();
            var observer = testScheduler.CreateObserver<Unit>();

            InteractorFactory.ObserveWorkspaceOrTimeEntriesChanges().Execute()
                .Subscribe(observer);

            itemsChangedSubject.OnNext(Unit.Default);
            itemsChangedSubject.OnNext(Unit.Default);

            observer.Messages.Should().HaveCount(2);
        }

        [Fact, LogIfTooSlow]
        public void GetsAnEventWhenAChangeToTimeEntriesHappens()
        {
            var itemsChangedSubject = new Subject<Unit>();
            DataSource.TimeEntries.ItemsChanged.Returns(itemsChangedSubject);
            DataSource.Workspaces.ItemsChanged.Returns(Observable.Never<Unit>());

            var testScheduler = new TestScheduler();
            var observer = testScheduler.CreateObserver<Unit>();

            InteractorFactory.ObserveWorkspaceOrTimeEntriesChanges().Execute()
                .Subscribe(observer);

            itemsChangedSubject.OnNext(Unit.Default);
            itemsChangedSubject.OnNext(Unit.Default);

            observer.Messages.Should().HaveCount(2);
        }


        [Fact, LogIfTooSlow]
        public void GetsAnEventWhenAChangeToTimeEntriesOrWorkspacesHappens()
        {
            var itemsChangedSubject = new Subject<Unit>();
            DataSource.TimeEntries.ItemsChanged.Returns(itemsChangedSubject);

            var itemsChangedSubject2 = new Subject<Unit>();
            DataSource.Workspaces.ItemsChanged.Returns(itemsChangedSubject2);

            var testScheduler = new TestScheduler();
            var observer = testScheduler.CreateObserver<Unit>();

            InteractorFactory.ObserveWorkspaceOrTimeEntriesChanges().Execute()
                .Subscribe(observer);

            itemsChangedSubject.OnNext(Unit.Default);
            itemsChangedSubject.OnNext(Unit.Default);
            itemsChangedSubject2.OnNext(Unit.Default);

            observer.Messages.Should().HaveCount(3);
        }
    }
}
