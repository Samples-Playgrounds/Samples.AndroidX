using Toggl.Core.Models.Interfaces;

namespace Toggl.Core.DataSources
{
    public struct EntityUpdate<TThreadsafe>
        where TThreadsafe : IThreadSafeModel
    {
        public long Id { get; }

        public TThreadsafe Entity { get; }

        public EntityUpdate(long id, TThreadsafe entity)
        {
            Id = id;
            Entity = entity;
        }
    }
}
