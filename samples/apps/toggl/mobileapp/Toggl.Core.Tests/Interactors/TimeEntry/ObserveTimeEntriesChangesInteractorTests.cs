using FluentAssertions;
using Microsoft.Reactive.Testing;
using NSubstitute;
using System.Reactive;
using System.Reactive.Subjects;
using Xunit;

namespace Toggl.Core.Tests.Interactors.TimeEntry
{
    public class ObserveTimeEntriesChangesInteractorTests : BaseInteractorTests
    {
        [Fact, LogIfTooSlow]
        public void GetsAnEventWhenAChangeToTimeEntriesHappens()
        {
            var itemsChangedSubject = new Subject<Unit>();
            DataSource.TimeEntries.ItemsChanged.Returns(itemsChangedSubject);

            var testScheduler = new TestScheduler();
            var observer = testScheduler.CreateObserver<Unit>();

            InteractorFactory.ObserveTimeEntriesChanges().Execute()
                .Subscribe(observer);

            itemsChangedSubject.OnNext(Unit.Default);
            itemsChangedSubject.OnNext(Unit.Default);

            observer.Messages.Should().HaveCount(2);
        }
    }
}
