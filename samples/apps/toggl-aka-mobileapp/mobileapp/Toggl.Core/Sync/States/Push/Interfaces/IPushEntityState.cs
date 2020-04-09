using System;
using Toggl.Core.Models.Interfaces;
using Toggl.Networking.Exceptions;

namespace Toggl.Core.Sync.States.Push.Interfaces
{
    public interface IPushEntityState<T> : ISyncState<T>
        where T : IThreadSafeModel
    {
        StateResult<ServerErrorException> ServerError { get; }

        StateResult<(Exception, T)> ClientError { get; }

        StateResult<Exception> UnknownError { get; }

        StateResult<TimeSpan> PreventOverloadingServer { get; }
    }
}
