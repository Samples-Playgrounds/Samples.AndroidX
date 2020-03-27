using FluentAssertions;
using System;
using Toggl.Core.Extensions;
using Toggl.Core.Tests.Helpers;
using Toggl.Networking.Exceptions;
using Toggl.Networking.Serialization;
using Xunit;

namespace Toggl.Core.Tests.Extensions
{
    public sealed class ExceptionExtensionsTests
    {
        [Fact]
        public void MarksSerializationExceptionAsNotAnonymized()
        {
            var exception = new SerializationException(typeof(Networking.Models.User), new Exception());

            var isAnonymized = exception.IsAnonymized();

            isAnonymized.Should().BeFalse();
        }

        [Theory]
        [MemberData(nameof(ApiExceptions.ClientExceptions), MemberType = typeof(ApiExceptions))]
        [MemberData(nameof(ApiExceptions.ServerExceptions), MemberType = typeof(ApiExceptions))]
        public void MarksApiExceptionsAsAnonymized(ApiException exception)
        {
            var isAnonymized = exception.IsAnonymized();

            isAnonymized.Should().BeTrue();
        }
    }
}
