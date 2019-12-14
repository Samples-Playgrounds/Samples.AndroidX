using System;
using System.Linq;
using System.Reactive.Linq;
using Toggl.Core.DataSources.Interfaces;
using Toggl.Core.Models;
using Toggl.Core.Models.Interfaces;
using Toggl.Shared;
using Toggl.Shared.Extensions;
using Toggl.Storage;
using Toggl.Storage.Models;

namespace Toggl.Core.Interactors
{
    internal class CreateTagInteractor : IInteractor<IObservable<IThreadSafeTag>>
    {
        private readonly string tagName;
        private readonly long workspaceId;
        private readonly IIdProvider idProvider;
        private readonly ITimeService timeService;
        private readonly IDataSource<IThreadSafeTag, IDatabaseTag> dataSource;

        public CreateTagInteractor(
            IIdProvider idProvider,
            ITimeService timeService,
            IDataSource<IThreadSafeTag, IDatabaseTag> dataSource,
            string tagName,
            long workspaceId)
        {
            Ensure.Argument.IsNotNull(tagName, nameof(tagName));
            Ensure.Argument.IsNotNull(dataSource, nameof(dataSource));
            Ensure.Argument.IsNotNull(idProvider, nameof(idProvider));
            Ensure.Argument.IsNotNull(timeService, nameof(timeService));
            Ensure.Argument.IsNotZero(workspaceId, nameof(workspaceId));

            this.tagName = tagName;
            this.dataSource = dataSource;
            this.idProvider = idProvider;
            this.timeService = timeService;
            this.workspaceId = workspaceId;
        }

        public IObservable<IThreadSafeTag> Execute()
            => tagAlreadyExists()
                .SelectMany(tagExists => tagExists
                    ? Observable.Return<IThreadSafeTag>(null)
                    : createTag());

        private IObservable<bool> tagAlreadyExists()
            => dataSource
                .GetAll(tag => tag.Name == tagName && tag.WorkspaceId == workspaceId)
                .Select(tags => tags.Any());

        private IObservable<IThreadSafeTag> createTag()
            => idProvider
                .GetNextIdentifier()
                .Apply(Tag.Builder.Create)
                .SetName(tagName)
                .SetWorkspaceId(workspaceId)
                .SetAt(timeService.CurrentDateTime)
                .SetSyncStatus(SyncStatus.SyncNeeded)
                .Build()
                .Apply(dataSource.Create);
    }
}
