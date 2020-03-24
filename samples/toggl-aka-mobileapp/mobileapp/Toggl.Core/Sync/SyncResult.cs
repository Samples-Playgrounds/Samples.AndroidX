using System;

namespace Toggl.Core.Sync
{
    public abstract class SyncResult
    {
    }

    public sealed class Success : SyncResult
    {
        public SyncState Operation { get; }

        public Success(SyncState operation)
        {
            Operation = operation;
        }
    }

    public sealed class Error : SyncResult
    {
        public Exception Exception { get; }

        public Error(Exception error)
        {
            Exception = error;
        }
    }
}