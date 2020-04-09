using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using Toggl.Core.Analytics;
using Toggl.Core.DataSources.Interfaces;
using Toggl.Core.Extensions;
using Toggl.Core.Models.Interfaces;
using Toggl.Networking.ApiClients;
using Toggl.Shared;
using Toggl.Shared.Extensions;
using Toggl.Shared.Models;
using Toggl.Storage;
using Toggl.Storage.Models;
using static Toggl.Core.Sync.PushSyncOperation;

namespace Toggl.Core.Sync.States.Push
{
    internal sealed class CreateEntityState<TModel, TDatabaseModel, TThreadsafeModel>
        : BasePushEntityState<TThreadsafeModel>
        where TModel : IIdentifiable
        where TDatabaseModel : TModel, IDatabaseModel
        where TThreadsafeModel : class, TDatabaseModel, IThreadSafeModel
    {
        private readonly ICreatingApiClient<TModel> api;

        private readonly IDataSource<TThreadsafeModel, TDatabaseModel> dataSource;

        private readonly Func<TModel, TThreadsafeModel> convertToThreadsafeModel;

        private readonly ILeakyBucket leakyBucket;
        private readonly IRateLimiter limiter;

        public StateResult<TThreadsafeModel> EntityChanged { get; } = new StateResult<TThreadsafeModel>();

        public StateResult<TThreadsafeModel> Done { get; } = new StateResult<TThreadsafeModel>();

        public CreateEntityState(
            ICreatingApiClient<TModel> api,
            IDataSource<TThreadsafeModel, TDatabaseModel> dataSource,
            IAnalyticsService analyticsService,
            ILeakyBucket leakyBucket,
            IRateLimiter limiter,
            Func<TModel, TThreadsafeModel> convertToThreadsafeModel)
            : base(analyticsService)
        {
            Ensure.Argument.IsNotNull(api, nameof(api));
            Ensure.Argument.IsNotNull(convertToThreadsafeModel, nameof(convertToThreadsafeModel));
            Ensure.Argument.IsNotNull(leakyBucket, nameof(leakyBucket));
            Ensure.Argument.IsNotNull(limiter, nameof(limiter));
            Ensure.Argument.IsNotNull(dataSource, nameof(dataSource));

            this.api = api;
            this.convertToThreadsafeModel = convertToThreadsafeModel;
            this.dataSource = dataSource;
            this.leakyBucket = leakyBucket;
            this.limiter = limiter;
        }

        public override IObservable<ITransition> Start(TThreadsafeModel entity)
        {
            if (!leakyBucket.TryClaimFreeSlot(out var timeToFreeSlot))
                return Observable.Return(PreventOverloadingServer.Transition(timeToFreeSlot));

            return create(entity)
                .Select(convertToThreadsafeModel)
                .Track(AnalyticsService.EntitySynced, Create, entity.GetSafeTypeName())
                .Track(AnalyticsService.EntitySyncStatus, entity.GetSafeTypeName(), $"{Create}:Success")
                .SelectMany(tryOverwrite(entity))
                .Catch(Fail(entity, Create));
        }

        private IObservable<TModel> create(TThreadsafeModel entity)
            => entity == null
                ? Observable.Throw<TModel>(new ArgumentNullException(nameof(entity)))
                : limiter.WaitForFreeSlot()
                    .ThenExecute(() => api.Create(entity).ToObservable());

        private Func<TThreadsafeModel, IObservable<ITransition>> tryOverwrite(TThreadsafeModel originalEntity)
            => serverEntity
                => dataSource.OverwriteIfOriginalDidNotChange(originalEntity, serverEntity)
                             .SelectMany(results => updateId(results, originalEntity, serverEntity));

        private IObservable<ITransition> updateId(
            IEnumerable<IConflictResolutionResult<TThreadsafeModel>> results,
            TThreadsafeModel originalEntity,
            TThreadsafeModel serverEntity)
        {
            return dataSource.ChangeId(originalEntity.Id, serverEntity.Id)
                .Select(transitionBasedOnResolutionResult(results, originalEntity, serverEntity));
        }

        private Func<TThreadsafeModel, Transition<TThreadsafeModel>> transitionBasedOnResolutionResult(
            IEnumerable<IConflictResolutionResult<TThreadsafeModel>> results,
            TThreadsafeModel originalEntity, TThreadsafeModel serverEntity)
        {
            foreach (var result in results)
            {
                switch (result)
                {
                    case UpdateResult<TThreadsafeModel> u when u.OriginalId == originalEntity.Id:
                        return Done.Transition;

                    case IgnoreResult<TThreadsafeModel> i when i.Id == originalEntity.Id || i.Id == serverEntity.Id:
                        return EntityChanged.Transition;
                }
            }

            throw new ArgumentException("Results must contain result with one of the specified ids.");
        }
    }
}
