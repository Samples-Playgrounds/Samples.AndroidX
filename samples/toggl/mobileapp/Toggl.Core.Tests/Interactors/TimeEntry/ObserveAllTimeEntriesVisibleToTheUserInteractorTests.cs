using FluentAssertions;
using Microsoft.Reactive.Testing;
using NSubstitute;
using System.Collections.Generic;
using System.Reactive;
using System.Reactive.Subjects;
using Toggl.Core.Models.Interfaces;
using Xunit;

namespace Toggl.Core.Tests.Interactors.TimeEntry
{
    public class ObserveAllTimeEntriesVisibleToTheUserInteractorTests
    {
        public sealed class WhenTimeEntriesChange : BaseInteractorTests
        {
            [Fact, LogIfTooSlow]
            public void EmitsAnEventWhenATimeEntryIsCreated()
            {
                var itemsChangedSubject = new Subject<Unit>();
                DataSource.TimeEntries.ItemsChanged.Returns(itemsChangedSubject);

                var testScheduler = new TestScheduler();
                var observer = testScheduler.CreateObserver<IEnumerable<IThreadSafeTimeEntry>>();

                InteractorFactory.ObserveAllTimeEntriesVisibleToTheUser()
                    .Execute()
                    .Subscribe(observer);

                itemsChangedSubject.OnNext(Unit.Default);

                observer.Messages.Should().HaveCount(1);
            }
        }

        public sealed class WhenWorkspacesChange : BaseInteractorTests
        {
            [Fact, LogIfTooSlow]
            public void EmitsAnEventWhenTimeEntriesChange()
            {
                var itemsChangedSubject = new Subject<Unit>();
                DataSource.Workspaces.ItemsChanged.Returns(itemsChangedSubject);

                var testScheduler = new TestScheduler();
                var observer = testScheduler.CreateObserver<IEnumerable<IThreadSafeTimeEntry>>();

                InteractorFactory.ObserveAllTimeEntriesVisibleToTheUser()
                    .Execute()
                    .Subscribe(observer);

                itemsChangedSubject.OnNext(Unit.Default);

                observer.Messages.Should().HaveCount(1);
            }
        }
    }
}
