using System;

namespace Toggl.Core.Exceptions
{
    public sealed class NotAuthorizedException : Exception
    {
        public NotAuthorizedException(string message) : base(message)
        {
        }
    }
}
