using System;

namespace Toggl.Networking.Tests
{
    public sealed class TestException : Exception
    {
        public TestException(string message)
            : base(message)
        {
        }
    }
}
