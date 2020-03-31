using System;
namespace Toggl.Core.Exceptions
{
    public class NoWorkspaceException : Exception
    {
        public NoWorkspaceException()
        {
        }

        public NoWorkspaceException(string message) : base(message)
        {
        }
    }
}
