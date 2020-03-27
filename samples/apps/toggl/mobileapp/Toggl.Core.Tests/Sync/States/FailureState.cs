using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Toggl.Core.Sync.States;
using Toggl.Networking.Exceptions;
using Toggl.Networking.Network;
using Xunit;

namespace Toggl.Core.Tests.Sync.States
{
    public sealed class FailureStateTests
    {
        public sealed class TheStartMethod
        {
            private static readonly IRequest request = NSubstitute.Substitute.For<IRequest>();
            private static readonly IResponse response = NSubstitute.Substitute.For<IResponse>();

            [Theory, LogIfTooSlow]
            [MemberData(nameof(Exceptions))]
            public void ThrowsTheGivenException(Exception exception)
            {
                var state = new FailureState();

                Func<Task> start = async () => await state.Start(exception);

                start.Should().Throw<Exception>().Where(caught => caught == exception);
            }

            public static IEnumerable<object[]> Exceptions
                => new[]
                {
                    new object[] { new Exception() },
                    new object[] { new InternalServerErrorException(request, response) },
                    new object[] { new TooManyRequestsException(request, response),  }
                };
        }
    }
}
