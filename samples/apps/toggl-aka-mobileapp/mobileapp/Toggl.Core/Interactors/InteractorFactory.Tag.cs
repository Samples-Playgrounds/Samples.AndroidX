using System;
using System.Collections.Generic;
using Toggl.Core.Interactors.Generic;
using Toggl.Core.Models.Interfaces;
using Toggl.Storage.Models;

namespace Toggl.Core.Interactors
{
    public sealed partial class InteractorFactory
    {
        public IInteractor<IObservable<IThreadSafeTag>> CreateTag(string tagName, long workspaceId)
            => new CreateTagInteractor(idProvider, timeService, dataSource.Tags, tagName, workspaceId);

        public IInteractor<IObservable<IThreadSafeTag>> GetTagById(long id)
            => new GetByIdInteractor<IThreadSafeTag, IDatabaseTag>(dataSource.Tags, analyticsService, id)
                .TrackException<Exception, IThreadSafeTag>("GetTagById");

        public IInteractor<IObservable<IEnumerable<IThreadSafeTag>>> GetMultipleTagsById(params long[] ids)
            => new GetMultipleByIdInteractor<IThreadSafeTag, IDatabaseTag>(dataSource.Tags, ids);
    }
}
