using System;
using Toggl.Shared;

namespace Toggl.Networking.Tests.Integration.Helper
{
    public static class RandomEmail
    {
        public static Email GenerateValid()
            => Email.From($"{Guid.NewGuid()}@mocks.toggl.com");

        public static Email GenerateInvalid()
            => Email.From($"non-existing-email-{Guid.NewGuid()}@ironicmocks.toggl.com");
    }
}
