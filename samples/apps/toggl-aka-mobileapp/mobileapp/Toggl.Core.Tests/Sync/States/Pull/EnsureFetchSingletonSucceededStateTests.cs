using FluentAssertions;
using NSubstitute;
using System;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Toggl.Core.Sync.States;
using Toggl.Core.Sync.States.Pull;
using Toggl.Core.Tests.Helpers;
using Toggl.Networking.Exceptions;
using Xunit;

namespace Toggl.Core.Tests.Sync.States.Pull
{
    public sealed class EnsureFetchSingletonSucceededStateTests
    {
        [Fact, LogIfTooSlow]
        public async Task ReturnsContinueResultWhenFetchingCompletes()
        {
            var fetchObservables = createFetchObservables(Observable.Return(new TestModel()));
            var state = new EnsureFetchSingletonSucceededState<ITestModel>();
            var transition = await state.Start(fetchObservables);

            transition.Result.Should().Be(state.Done);
        }

        [Theory, LogIfTooSlow]
        [MemberData(nameof(ApiExceptions.ServerExceptions), MemberType = typeof(ApiExceptions))]
        [MemberData(nameof(ApiExceptions.ClientExceptionsWhichAreNotReThrownInSyncStates), MemberType = typeof(ApiExceptions))]
        public async Task ReturnsFailureResultWhenFetchingThrows(ApiException exception)
        {
            var fetchObservables = createFetchObservables(Observable.Throw<ITestModel>(exception));
            var state = new EnsureFetchSingletonSucceededState<ITestModel>();
            var transition = await state.Start(fetchObservables);

            transition.Result.Should().Be(state.ErrorOccured);
        }

        [Fact, LogIfTooSlow]
        public void ThrowsIfFetchObservablePublishesTwice()
        {
            var fetchObservables = createFetchObservables(
                Observable.Create<ITestModel>(observer =>
                {
                    observer.OnNext(new TestModel());
                    observer.OnNext(new TestModel());
                    return () => { };
                }));
            var state = new EnsureFetchSingletonSucceededState<ITestModel>();
            Action fetchTwice = () => state.Start(fetchObservables).Wait();

            fetchTwice.Should().Throw<InvalidOperationException>();
        }

        [Fact, LogIfTooSlow]
        public void ThrowsWhenTheDeviceIsOffline()
        {
            var observables = createFetchObservables(Observable.Throw<ITestModel>(new OfflineException(new Exception())));
            var state = new EnsureFetchSingletonSucceededState<ITestModel>();
            Action startingState = () => state.Start(observables).Wait();

            startingState.Should().Throw<OfflineException>();
        }

        private IFetchObservables createFetchObservables(IObservable<ITestModel> observable)
        {
            var observables = Substitute.For<IFetchObservables>();
            observables.GetSingle<ITestModel>().Returns(observable);
            return observables;
        }
    }
}
