using FluentAssertions;
using NSubstitute;
using System;
using System.Net;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Toggl.Networking.ApiClients;
using Toggl.Networking.Network;
using Toggl.Networking.Serialization;
using Xunit;

namespace Toggl.Networking.Tests.ApiClients
{
    public sealed class ProjectsSummaryReportsApiTests
    {
        private readonly IProjectsSummaryApi client;
        private readonly IJsonSerializer serializer;
        private readonly IApiClient apiClient;

        public ProjectsSummaryReportsApiTests()
        {
            var endpoints = new Endpoints(ApiEnvironment.Staging);
            apiClient = Substitute.For<IApiClient>();
            serializer = new JsonSerializer();
            client = new ProjectsSummaryApi(endpoints, apiClient, serializer, Credentials.None);
        }

        [Fact, LogIfTooSlow]
        public async Task SerializesTheDateParametersCorrectlyInTheRequestBody()
        {
            var from = new DateTimeOffset(2017, 1, 25, 11, 22, 33, TimeSpan.Zero);
            var to = from.AddDays(1);
            var expectedBody = "{\"start_date\":\"2017-01-25\",\"end_date\":\"2017-01-26\"}";
            var response = emptyResponse();
            apiClient.Send(Arg.Any<IRequest>()).Returns(response);

            await client.GetByWorkspace(123, from, to);

            await apiClient.Received().Send(Arg.Is<IRequest>(req => req.Body.Left == expectedBody));
        }

        [Fact, LogIfTooSlow]
        public async Task IgnoresTheEndDateParameterIfItIsNull()
        {
            var from = new DateTimeOffset(2017, 1, 27, 11, 22, 33, TimeSpan.Zero);
            var expectedBody = "{\"start_date\":\"2017-01-27\"}";
            var response = emptyResponse();
            apiClient.Send(Arg.Any<IRequest>()).Returns(response);

            await client.GetByWorkspace(123, from, null);

            await apiClient.Received().Send(Arg.Is<IRequest>(req => req.Body.Left == expectedBody));
        }

        [Fact, LogIfTooSlow]
        public void ThrowsWhenTheIntervalIsGreaterThanOneYear()
        {
            var now = new DateTimeOffset(2017, 1, 27, 11, 22, 33, TimeSpan.Zero);
            var from = now.AddYears(-1).AddDays(-2);
            var to = now.AddDays(-1);

            Action callingClient = () => client.GetByWorkspace(123, from, to).Wait();

            callingClient.Should().Throw<ArgumentOutOfRangeException>();
        }

        [Fact, LogIfTooSlow]
        public void DoesNotThrowWhenTheStartDateIsInThePastButThereIsNoEndDate()
        {
            var now = new DateTimeOffset(2017, 1, 27, 11, 22, 33, TimeSpan.Zero);
            var from = now.AddYears(-1).AddDays(-2);
            var response = emptyResponse();
            apiClient.Send(Arg.Any<IRequest>()).Returns(response);

            Action callingClient = () => client.GetByWorkspace(123, from, null).Wait();

            callingClient.Should().NotThrow();
        }

        private IResponse emptyResponse()
        {
            var response = Substitute.For<IResponse>();
            response.IsSuccess.Returns(true);
            response.StatusCode.Returns(HttpStatusCode.OK);
            response.RawData.Returns("[]");
            return response;
        }
    }
}
