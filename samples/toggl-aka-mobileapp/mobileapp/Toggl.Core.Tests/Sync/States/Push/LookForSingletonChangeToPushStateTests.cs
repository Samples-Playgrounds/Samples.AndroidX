﻿using FluentAssertions;
using NSubstitute;
using System;
using System.Reactive.Linq;
using Toggl.Core.DataSources.Interfaces;
using Toggl.Core.Sync;
using Toggl.Core.Sync.States.Push;
using Toggl.Storage;
using Xunit;

namespace Toggl.Core.Tests.Sync.States.Push
{
    public sealed class LookForSingletonChangeToPushStateTests
    {
        private readonly ISingletonDataSource<IThreadSafeTestModel> dataSource
            = Substitute.For<ISingletonDataSource<IThreadSafeTestModel>>();

        [Fact, LogIfTooSlow]
        public void ConstructorThrowsWithNullDataSource()
        {
            Action creatingWithNullArgument = () => new LookForSingletonChangeToPushState<IThreadSafeTestModel>(null);

            creatingWithNullArgument.Should().Throw<ArgumentNullException>();
        }

        [Theory, LogIfTooSlow]
        [InlineData(SyncStatus.InSync)]
        [InlineData(SyncStatus.SyncFailed)]
        public void ReturnsNothingToPushTransitionWhenTheSingleEntityDoesNotNeedSyncing(SyncStatus syncStatus)
        {
            var entity = new TestModel(1, syncStatus);
            var state = new LookForSingletonChangeToPushState<IThreadSafeTestModel>(dataSource);
            dataSource.Get().Returns(Observable.Return(entity));

            var transition = state.Start().SingleAsync().Wait();

            transition.Result.Should().Be(state.NoMoreChanges);
        }

        [Fact, LogIfTooSlow]
        public void ReturnsPushEntityTransitionWhenTheRepositoryReturnsSomeEntity()
        {
            var state = new LookForSingletonChangeToPushState<IThreadSafeTestModel>(dataSource);
            var entity = new TestModel(1, SyncStatus.SyncNeeded);
            dataSource.Get().Returns(Observable.Return(entity));

            var transition = state.Start().SingleAsync().Wait();
            var parameter = ((Transition<IThreadSafeTestModel>)transition).Parameter;

            transition.Result.Should().Be(state.ChangeFound);
            parameter.Should().BeEquivalentTo(entity, options => options.IncludingProperties());
        }

        [Fact, LogIfTooSlow]
        public void ThrowsWhenRepositoryThrows()
        {
            var state = new LookForSingletonChangeToPushState<IThreadSafeTestModel>(dataSource);
            dataSource.Get().Returns(Observable.Throw<IThreadSafeTestModel>(new Exception()));

            Action callingStart = () => state.Start().SingleAsync().Wait();

            callingStart.Should().Throw<Exception>();
        }
    }
}
