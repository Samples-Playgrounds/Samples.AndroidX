using System;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using Toggl.Core.Analytics;
using Toggl.Core.DataSources.Interfaces;
using Toggl.Core.Extensions;
using Toggl.Core.Models.Interfaces;
using Toggl.Networking.ApiClients.Interfaces;
using Toggl.Shared;
using Toggl.Shared.Models;
using Toggl.Storage;
using static Toggl.Core.Sync.PushSyncOperation;

namespace Toggl.Core.Sync.States.Push
{
    internal sealed class DeleteEntityState<TModel, TDatabaseModel, TThreadsafeModel>
        : BasePushEntityState<TThreadsafeModel>
        where TModel : IIdentifiable
        where TDatabaseModel : class, TModel, IDatabaseSyncable
        where TThreadsafeModel : class, TDatabaseModel, IThreadSafeModel
    {
        private readonly IDeletingApiClient<TModel> api;

        private readonly IDataSource<TThreadsafeModel, TDatabaseModel> dataSource;

        private readonly ILeakyBucket leakyBucket;
        private readonly IRateLimiter limiter;

        public StateResult Done { get; } = new StateResult();

        public DeleteEntityState(
            IDeletingApiClient<TModel> api,
            IAnalyticsService analyticsService,
            IDataSource<TThreadsafeModel, TDatabaseModel> dataSource,
            ILeakyBucket leakyBucket,
            IRateLimiter limiter)
            : base(analyticsService)
        {
            Ensure.Argument.IsNotNull(api, nameof(api));
            Ensure.Argument.IsNotNull(dataSource, nameof(dataSource));
            Ensure.Argument.IsNotNull(leakyBucket, nameof(leakyBucket));
            Ensure.Argument.IsNotNull(limiter, nameof(limiter));

            this.api = api;
            this.dataSource = dataSource;
            this.leakyBucket = leakyBucket;
            this.limiter = limiter;
        }

        public override IObservable<ITransition> Start(TThreadsafeModel entity)
        {
            if (!leakyBucket.TryClaimFreeSlot(out var timeToFreeSlot))
                return Observable.Return(PreventOverloadingServer.Transition(timeToFreeSlot));

            return delete(entity)
                .SelectMany(_ => dataSource.Delete(entity.Id))
                .Track(AnalyticsService.EntitySynced, Delete, entity.GetSafeTypeName())
                .Track(AnalyticsService.EntitySyncStatus, entity.GetSafeTypeName(), $"{Delete}:Success")
                .Select(_ => Done.Transition())
                .Catch(Fail(entity, Delete));
        }

        private IObservable<Unit> delete(TModel entity)
            => entity == null
                ? Observable.Throw<Unit>(new ArgumentNullException(nameof(entity)))
                : limiter.WaitForFreeSlot()
                    .ThenExecute(() => api.Delete(entity).ToObservable());
    }
}
