using System;

namespace Toggl.Core.Exceptions
{
    public sealed class NoDefaultWorkspaceException : Exception
    {
        public NoDefaultWorkspaceException() : base()
        {
        }

        public NoDefaultWorkspaceException(string message) : base(message)
        {
        }
    }
}
