using System;
using System.Linq;
using System.Reactive.Linq;
using Toggl.Core.DataSources.Interfaces;
using Toggl.Core.Models.Interfaces;
using Toggl.Shared;
using Toggl.Shared.Extensions;
using Toggl.Shared.Models;
using Toggl.Storage.Models;

namespace Toggl.Core.Sync.States.CleanUp
{
    public sealed class DeleteNonReferencedInaccessibleTagsState : DeleteInaccessibleEntityState<IThreadSafeTag, IDatabaseTag>
    {
        private readonly IDataSource<IThreadSafeTag, IDatabaseTag> tagsDataSource;
        private readonly IDataSource<IThreadSafeTimeEntry, IDatabaseTimeEntry> timeEntriesDataSource;

        public DeleteNonReferencedInaccessibleTagsState(
            IDataSource<IThreadSafeTag, IDatabaseTag> tagsDataSource,
            IDataSource<IThreadSafeTimeEntry, IDatabaseTimeEntry> timeEntriesDataSource)
            : base(tagsDataSource)
        {
            Ensure.Argument.IsNotNull(tagsDataSource, nameof(tagsDataSource));
            Ensure.Argument.IsNotNull(timeEntriesDataSource, nameof(timeEntriesDataSource));

            this.tagsDataSource = tagsDataSource;
            this.timeEntriesDataSource = timeEntriesDataSource;
        }

        protected override IObservable<bool> SuitableForDeletion(IThreadSafeTag tag)
            => timeEntriesDataSource.GetAll(
                    timeEntry => isReferenced(tag, timeEntry),
                    includeInaccessibleEntities: true)
                .Select(references => references.None());

        private bool isReferenced(ITag tag, ITimeEntry timeEntry)
            => timeEntry.TagIds.Contains(tag.Id);
    }
}
