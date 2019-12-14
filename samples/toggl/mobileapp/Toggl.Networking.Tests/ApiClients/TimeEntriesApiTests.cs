using System;
using FluentAssertions;
using NSubstitute;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reactive;
using System.Reactive.Linq;
using Toggl.Networking.ApiClients;
using Toggl.Networking.Exceptions;
using Toggl.Networking.Models;
using Toggl.Networking.Network;
using Toggl.Networking.Serialization;
using Toggl.Networking.Tests.Exceptions;
using Xunit;
using Task = System.Threading.Tasks.Task;

namespace Toggl.Networking.Tests.ApiClients
{
    public class TimeEntriesApiTests
    {
        public class TheDeleteMethod
        {
            private readonly IApiClient apiClient = Substitute.For<IApiClient>();
            private readonly IJsonSerializer jsonSerializer = Substitute.For<IJsonSerializer>();
            private readonly Credentials credentials = Credentials.None;
            private readonly TimeEntriesApi api;

            public TheDeleteMethod()
            {
                api = new TimeEntriesApi(
                    new Endpoints(ApiEnvironment.Staging),
                    apiClient,
                    jsonSerializer,
                    credentials,
                    new UserAgent("unit tests", "0.0"));
            }

            [Fact]
            public async Task SucceedsWhenApiDoesNotFindTheTimeEntry()
            {
                var notFoundResponse = new Response(
                    "Time entry not found",
                    isSuccess: false,
                    "text/plain; charset=utf-8",
                    Enumerable.Empty<KeyValuePair<string, IEnumerable<string>>>(),
                    HttpStatusCode.NotFound);
                apiClient.Send(Arg.Any<IRequest>()).Returns(notFoundResponse);

                await api.Delete(new TimeEntry { Id = 123 });
            }

            [Theory]
            [MemberData(nameof(ApiErrorResponsesTests.ClientErrorsList), MemberType = typeof(ApiErrorResponsesTests))]
            public async Task FailsForDifferentClientErrors(HttpStatusCode errorCode, Type exceptionType)
            {
                if (exceptionType == typeof(NotFoundException)) return;

                var response = new Response(
                    "",
                    isSuccess: false,
                    "text/plain; charset=utf-8",
                    Enumerable.Empty<KeyValuePair<string, IEnumerable<string>>>(),
                    errorCode);
                apiClient.Send(Arg.Any<IRequest>()).Returns(response);
                bool clientExceptionWasCaught = false;

                try
                {
                    await api.Delete(new TimeEntry { Id = 123 });
                }
                catch (ClientErrorException caughtException)
                {
                    clientExceptionWasCaught = true;
                }
                finally
                {
                    clientExceptionWasCaught.Should().BeTrue("A client error exception is expected to be thrown.");
                }
            }
        }
    }
}
