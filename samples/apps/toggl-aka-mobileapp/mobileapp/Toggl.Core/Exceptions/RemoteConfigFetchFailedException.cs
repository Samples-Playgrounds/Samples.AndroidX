using System;
namespace Toggl.Core.Exceptions
{
    public sealed class RemoteConfigFetchFailedException : Exception
    {
        public RemoteConfigFetchFailedException(string message) : base(message)
        {
        }
    }
}
