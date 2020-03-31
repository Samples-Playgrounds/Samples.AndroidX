using FluentAssertions;
using Microsoft.Reactive.Testing;
using NSubstitute;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using Xunit;

namespace Toggl.Core.Tests.Interactors.Workspace
{
    public class ObserveWorkspacesChangesInteractorTests
    {
        public sealed class TheObserveAllWorkspacesInteractorTests : BaseInteractorTests
        {
            [Fact, LogIfTooSlow]
            public async Task GetsAnEventWhenAChangeToWorkspacesHappens()
            {
                var itemsChangedSubject = new Subject<Unit>();
                DataSource.Workspaces.ItemsChanged.Returns(itemsChangedSubject.AsObservable());

                var testScheduler = new TestScheduler();
                var observer = testScheduler.CreateObserver<Unit>();

                InteractorFactory.ObserveWorkspacesChanges().Execute()
                    .Subscribe(observer);

                itemsChangedSubject.OnNext(Unit.Default);
                itemsChangedSubject.OnNext(Unit.Default);

                observer.Messages.Should().HaveCount(2);
            }
        }
    }
}
