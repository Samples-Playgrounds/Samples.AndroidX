namespace Toggl.Core.UI.ViewModels.TimeEntriesLog.Identity
{
    internal sealed class TimeEntriesGroupKey : IMainLogKey
    {
        private readonly GroupId groupId;

        public TimeEntriesGroupKey(GroupId groupId)
        {
            this.groupId = groupId;
        }

        public bool Equals(IMainLogKey other)
            => other is TimeEntriesGroupKey groupKey && groupId.Equals(groupKey.groupId);

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj is TimeEntriesGroupKey other && Equals(other);
        }

        public override int GetHashCode()
            => groupId.GetHashCode();
    }
}
