using System;
namespace Toggl.Core.Exceptions
{
    public sealed class GoogleLoginException : Exception
    {
        public bool LoginWasCanceled { get; }

        public GoogleLoginException(bool loginWasCanceled)
        {
            LoginWasCanceled = loginWasCanceled;
        }
    }
}
