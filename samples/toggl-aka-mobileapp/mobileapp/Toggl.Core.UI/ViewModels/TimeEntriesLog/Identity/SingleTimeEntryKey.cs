namespace Toggl.Core.UI.ViewModels.TimeEntriesLog.Identity
{
    internal sealed class SingleTimeEntryKey : IMainLogKey
    {
        private readonly long timeEntryId;

        public SingleTimeEntryKey(long timeEntryId)
        {
            this.timeEntryId = timeEntryId;
        }

        public bool Equals(IMainLogKey other)
            => other is SingleTimeEntryKey singleKey && timeEntryId == singleKey.timeEntryId;

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj is SingleTimeEntryKey other && Equals(other);
        }

        public override int GetHashCode()
            => timeEntryId.GetHashCode();
    }
}
