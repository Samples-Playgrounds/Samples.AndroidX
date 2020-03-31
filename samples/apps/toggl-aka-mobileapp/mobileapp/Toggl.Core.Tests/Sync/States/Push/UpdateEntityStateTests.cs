using FluentAssertions;
using Microsoft.Reactive.Testing;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Reactive;
using System.Reactive.Linq;
using Toggl.Core.Analytics;
using Toggl.Core.DataSources.Interfaces;
using Toggl.Core.Extensions;
using Toggl.Core.Sync;
using Toggl.Core.Sync.States.Push;
using Toggl.Core.Tests.Helpers;
using Toggl.Core.Tests.Sync.States.Push.BaseStates;
using Toggl.Core.Tests.TestExtensions;
using Toggl.Networking.ApiClients;
using Toggl.Networking.Exceptions;
using Toggl.Shared;
using Toggl.Storage;
using Xunit;
using static Toggl.Core.Sync.PushSyncOperation;

namespace Toggl.Core.Tests.Sync.States.Push
{
    public sealed class UpdateEntityStateTests : BasePushEntityStateTests
    {
        private readonly ITestAnalyticsService analyticsService
            = Substitute.For<ITestAnalyticsService>();

        private readonly IUpdatingApiClient<ITestModel> api
            = Substitute.For<IUpdatingApiClient<ITestModel>>();

        private readonly IDataSource<IThreadSafeTestModel, IDatabaseTestModel> dataSource
            = Substitute.For<IDataSource<IThreadSafeTestModel, IDatabaseTestModel>>();

        protected override PushSyncOperation Operation => PushSyncOperation.Update;

        public UpdateEntityStateTests()
        {
            SyncAnalyticsExtensions.SearchStrategy = TestSyncAnalyticsExtensionsSearchStrategy;
        }

        [Fact, LogIfTooSlow]
        public void ReturnsTheFailTransitionWhenEntityIsNull()
        {
            var state = (UpdateEntityState<ITestModel, IThreadSafeTestModel>)CreateState();
            var transition = state.Start(null).SingleAsync().Wait();
            var parameter = ((Transition<Exception>)transition).Parameter;

            transition.Result.Should().Be(state.UnknownError);
            parameter.Should().BeOfType<ArgumentNullException>();
        }

        [Theory, LogIfTooSlow]
        [MemberData(nameof(ApiExceptions.ServerExceptions), MemberType = typeof(ApiExceptions))]
        public void ReturnsTheServerErrorTransitionWhenHttpFailsWithServerError(ServerErrorException exception)
        {
            var state = (UpdateEntityState<ITestModel, IThreadSafeTestModel>)CreateState();
            var entity = new TestModel(1, SyncStatus.InSync);
            api.Update(Arg.Any<ITestModel>()).ReturnsThrowingTaskOf(exception);

            var transition = state.Start(entity).SingleAsync().Wait();
            var parameter = ((Transition<ServerErrorException>)transition).Parameter;

            transition.Result.Should().Be(state.ServerError);
            parameter.Should().BeAssignableTo<ServerErrorException>();
        }

        [Theory, LogIfTooSlow]
        [MemberData(nameof(ApiExceptions.ClientExceptionsWhichAreNotReThrownInSyncStates), MemberType = typeof(ApiExceptions))]
        public void ReturnsTheClientErrorTransitionWhenHttpFailsWithClientError(ClientErrorException exception)
        {
            var state = (UpdateEntityState<ITestModel, IThreadSafeTestModel>)CreateState();
            var entity = new TestModel(1, SyncStatus.InSync);
            api.Update(Arg.Any<ITestModel>()).ReturnsThrowingTaskOf(exception);

            var transition = state.Start(entity).SingleAsync().Wait();
            var parameter = ((Transition<(Exception Reason, IThreadSafeTestModel)>)transition).Parameter;

            transition.Result.Should().Be(state.ClientError);
            parameter.Reason.Should().BeAssignableTo<ClientErrorException>();
        }

        [Fact, LogIfTooSlow]
        public void ReturnsTheUnknownErrorTransitionWhenHttpFailsWithNonApiError()
        {
            var state = (UpdateEntityState<ITestModel, IThreadSafeTestModel>)CreateState();
            var entity = new TestModel(1, SyncStatus.InSync);
            api.Update(Arg.Any<ITestModel>()).ReturnsThrowingTaskOf(new TestException());

            var transition = state.Start(entity).SingleAsync().Wait();
            var parameter = ((Transition<Exception>)transition).Parameter;

            transition.Result.Should().Be(state.UnknownError);
            parameter.Should().BeOfType<TestException>();
        }

        [Fact, LogIfTooSlow]
        public void ReturnsTheFailTransitionWhenDatabaseOperationFails()
        {
            var state = (UpdateEntityState<ITestModel, IThreadSafeTestModel>)CreateState();
            var entity = new TestModel(1, SyncStatus.InSync);
            dataSource
                .OverwriteIfOriginalDidNotChange(Arg.Any<IThreadSafeTestModel>(), Arg.Any<IThreadSafeTestModel>())
                .Returns(Observable.Throw<IEnumerable<IConflictResolutionResult<IThreadSafeTestModel>>>(new TestException()));

            var transition = state.Start(entity).SingleAsync().Wait();
            var parameter = ((Transition<Exception>)transition).Parameter;

            transition.Result.Should().Be(state.UnknownError);
            parameter.Should().BeOfType<TestException>();
        }

        [Fact, LogIfTooSlow]
        public void UpdateApiCallIsCalledWithTheInputEntity()
        {
            var state = (UpdateEntityState<ITestModel, IThreadSafeTestModel>)CreateState();
            var entity = new TestModel(1, SyncStatus.InSync);
            api.Update(entity).ReturnsTaskOf(Substitute.For<ITestModel>());
            dataSource
                .OverwriteIfOriginalDidNotChange(Arg.Any<IThreadSafeTestModel>(), Arg.Any<IThreadSafeTestModel>())
                .Returns(Observable.Return(new[] { new UpdateResult<IThreadSafeTestModel>(entity.Id, entity) }));

            state.Start(entity).SingleAsync().Wait();

            api.Received().Update(Arg.Is(entity));
        }

        [Fact, LogIfTooSlow]
        public void WaitsForASlotFromTheRateLimiter()
        {
            var scheduler = new TestScheduler();
            var delay = TimeSpan.FromSeconds(1);
            RateLimiter.WaitForFreeSlot().Returns(Observable.Return(Unit.Default).Delay(delay, scheduler));
            var state = (UpdateEntityState<ITestModel, IThreadSafeTestModel>)CreateState();
            var entity = new TestModel(-1, SyncStatus.SyncFailed);

            state.Start(entity).Subscribe();

            scheduler.AdvanceBy(delay.Ticks - 1);
            api.DidNotReceive().Update(Arg.Any<ITestModel>());
            scheduler.AdvanceBy(1);
            api.Received().Update(Arg.Any<ITestModel>());
        }

        [Fact, LogIfTooSlow]
        public void ReturnsTheEntityChangedTransitionWhenEntityChangesLocally()
        {
            var state = (UpdateEntityState<ITestModel, IThreadSafeTestModel>)CreateState();
            var at = new DateTimeOffset(2017, 9, 1, 12, 34, 56, TimeSpan.Zero);
            var entity = new TestModel { Id = 1, At = at, SyncStatus = SyncStatus.SyncNeeded };
            api.Update(Arg.Any<ITestModel>()).ReturnsTaskOf(entity);
            dataSource
                .OverwriteIfOriginalDidNotChange(Arg.Any<IThreadSafeTestModel>(), Arg.Any<IThreadSafeTestModel>())
                .Returns(Observable.Return(new[] { new IgnoreResult<IThreadSafeTestModel>(entity.Id) }));

            var transition = state.Start(entity).SingleAsync().Wait();
            var parameter = ((Transition<IThreadSafeTestModel>)transition).Parameter;

            transition.Result.Should().Be(state.EntityChanged);
            parameter.Id.Should().Be(entity.Id);
        }

        [Fact, LogIfTooSlow]
        public void ReturnsTheUpdatingSuccessfulTransitionWhenEntityDoesNotChangeLocallyAndAllFunctionsAreCalledWithCorrectParameters()
        {
            var state = (UpdateEntityState<ITestModel, IThreadSafeTestModel>)CreateState();
            var at = new DateTimeOffset(2017, 9, 1, 12, 34, 56, TimeSpan.Zero);
            var entity = new TestModel { Id = 1, At = at, SyncStatus = SyncStatus.SyncNeeded };
            var serverEntity = new TestModel { Id = 2, At = at, SyncStatus = SyncStatus.SyncNeeded };
            var localEntity = new TestModel { Id = 3, At = at, SyncStatus = SyncStatus.SyncNeeded };
            var updatedEntity = new TestModel { Id = 4, At = at, SyncStatus = SyncStatus.SyncNeeded };
            api.Update(entity).ReturnsTaskOf(serverEntity);
            dataSource
                .GetById(entity.Id)
                .Returns(Observable.Return(localEntity));
            dataSource
                .OverwriteIfOriginalDidNotChange(Arg.Any<IThreadSafeTestModel>(), Arg.Any<IThreadSafeTestModel>())
                .Returns(Observable.Return(new[] { new UpdateResult<IThreadSafeTestModel>(entity.Id, updatedEntity) }));

            var transition = state.Start(entity).SingleAsync().Wait();
            var parameter = ((Transition<IThreadSafeTestModel>)transition).Parameter;

            transition.Result.Should().Be(state.Done);
            parameter.Should().BeEquivalentTo(updatedEntity, options => options.IncludingProperties());
            dataSource.Received()
                .OverwriteIfOriginalDidNotChange(
                    Arg.Is<IThreadSafeTestModel>(theOriginalEntity => theOriginalEntity.Id == entity.Id), Arg.Is<IThreadSafeTestModel>(theUpdatedEntity => theUpdatedEntity.Id == serverEntity.Id));
        }

        [Fact, LogIfTooSlow]
        public void TracksEntitySyncStatusInCaseOfSuccess()
        {
            var state = (UpdateEntityState<ITestModel, IThreadSafeTestModel>)CreateState();
            var at = new DateTimeOffset(2017, 9, 1, 12, 34, 56, TimeSpan.Zero);
            var entity = new TestModel { Id = 1, At = at, SyncStatus = SyncStatus.SyncNeeded };
            var serverEntity = new TestModel { Id = 2, At = at, SyncStatus = SyncStatus.SyncNeeded };
            var localEntity = new TestModel { Id = 3, At = at, SyncStatus = SyncStatus.SyncNeeded };
            var updatedEntity = new TestModel { Id = 4, At = at, SyncStatus = SyncStatus.SyncNeeded };
            api.Update(entity).ReturnsTaskOf(serverEntity);
            dataSource
                .GetById(entity.Id)
                .Returns(Observable.Return(localEntity));
            dataSource
                .OverwriteIfOriginalDidNotChange(Arg.Any<IThreadSafeTestModel>(), Arg.Any<IThreadSafeTestModel>())
                .Returns(Observable.Return(new[] { new UpdateResult<IThreadSafeTestModel>(entity.Id, updatedEntity) }));

            state.Start(entity).Wait();

            analyticsService.EntitySyncStatus.Received().Track(
                entity.GetSafeTypeName(),
                $"{Update}:Success");
        }

        [Fact, LogIfTooSlow]
        public void TracksEntitySyncedInCaseOfSuccess()
        {
            var state = (UpdateEntityState<ITestModel, IThreadSafeTestModel>)CreateState();
            var at = new DateTimeOffset(2017, 9, 1, 12, 34, 56, TimeSpan.Zero);
            var entity = new TestModel { Id = 1, At = at, SyncStatus = SyncStatus.SyncNeeded };
            var serverEntity = new TestModel { Id = 2, At = at, SyncStatus = SyncStatus.SyncNeeded };
            var localEntity = new TestModel { Id = 3, At = at, SyncStatus = SyncStatus.SyncNeeded };
            var updatedEntity = new TestModel { Id = 4, At = at, SyncStatus = SyncStatus.SyncNeeded };
            api.Update(entity).ReturnsTaskOf(serverEntity);
            dataSource
                .GetById(entity.Id)
                .Returns(Observable.Return(localEntity));
            dataSource
                .OverwriteIfOriginalDidNotChange(Arg.Any<IThreadSafeTestModel>(), Arg.Any<IThreadSafeTestModel>())
                .Returns(Observable.Return(new[] { new UpdateResult<IThreadSafeTestModel>(entity.Id, updatedEntity) }));

            state.Start(entity).Wait();

            analyticsService.EntitySynced.Received().Track(Update, entity.GetSafeTypeName());
        }

        [Fact, LogIfTooSlow]
        public void TracksEntitySyncStatusInCaseOfFailure()
        {
            var exception = new Exception();
            var state = (UpdateEntityState<ITestModel, IThreadSafeTestModel>)CreateState();
            var at = new DateTimeOffset(2017, 9, 1, 12, 34, 56, TimeSpan.Zero);
            var entity = new TestModel { Id = 1, At = at, SyncStatus = SyncStatus.SyncNeeded };
            var serverEntity = new TestModel { Id = 2, At = at, SyncStatus = SyncStatus.SyncNeeded };
            api.Update(entity).ReturnsTaskOf(serverEntity);
            PrepareApiCallFunctionToThrow(exception);

            state.Start(entity).Wait();

            analyticsService.EntitySyncStatus.Received().Track(
                entity.GetSafeTypeName(),
                $"{Update}:Failure");
        }

        [Theory, LogIfTooSlow]
        [MemberData(nameof(EntityTypes), MemberType = typeof(BasePushEntityStateTests))]
        public void TracksEntitySyncErrorInCaseOfFailure(Type entityType)
        {
            var exception = new Exception("SomeRandomMessage");
            var entity = (IThreadSafeTestModel)Substitute.For(new[] { entityType }, new object[0]);
            var state = new UpdateEntityState<ITestModel, IThreadSafeTestModel>(api, dataSource, analyticsService, LeakyBucket, RateLimiter, _ => null);
            var expectedMessage = $"{Update}:{exception.Message}";
            var analyticsEvent = entity.GetType().ToSyncErrorAnalyticsEvent(analyticsService);
            PrepareApiCallFunctionToThrow(exception);

            state.Start(entity).Wait();

            analyticsEvent.Received().Track(expectedMessage);
        }

        protected override BasePushEntityState<IThreadSafeTestModel> CreateState()
            => new UpdateEntityState<ITestModel, IThreadSafeTestModel>(api, dataSource, analyticsService, LeakyBucket, RateLimiter, TestModel.From);

        protected override void PrepareApiCallFunctionToThrow(Exception e)
        {
            api.Update(Arg.Any<ITestModel>()).ReturnsThrowingTaskOf(e);
        }

        protected override void PrepareDatabaseOperationToThrow(Exception e)
        {
            dataSource.OverwriteIfOriginalDidNotChange(Arg.Any<IThreadSafeTestModel>(), Arg.Any<IThreadSafeTestModel>())
                .Returns(_ => Observable.Throw<IEnumerable<IConflictResolutionResult<IThreadSafeTestModel>>>(e));
        }
    }
}
