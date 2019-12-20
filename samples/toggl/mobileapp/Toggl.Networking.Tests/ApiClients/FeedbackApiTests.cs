using FluentAssertions;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Toggl.Networking.ApiClients;
using Toggl.Networking.Network;
using Toggl.Networking.Serialization;
using Toggl.Shared;
using Toggl.Shared.Extensions;
using Xunit;

namespace Toggl.Networking.Tests.ApiClients
{
    public sealed class FeedbackApiTests
    {
        public sealed class TheSendMethod
        {
            private readonly IApiClient apiClient;

            private readonly IFeedbackApi feedbackApi;

            public TheSendMethod()
            {
                var endpoints = new Endpoints(ApiEnvironment.Staging);
                var serializer = new JsonSerializer();
                var credentials = Credentials.WithPassword("some@email.com".ToEmail(), "123456".ToPassword());
                apiClient = Substitute.For<IApiClient>();
                feedbackApi = new FeedbackApiClient(endpoints, apiClient, serializer, credentials);
            }

            [Fact]
            public void ThrowsForAnInvalidEmailAddress()
            {
                var invalidEmail = Email.From($"{Guid.NewGuid()}@toggl.");

                Func<Task> sendingFeedback = async () => await feedbackApi.Send(invalidEmail, "ABC.", new Dictionary<string, string>());

                sendingFeedback.Should().Throw<ArgumentException>();
            }

            [Theory]
            [InlineData(null)]
            [InlineData("")]
            [InlineData("      ")]
            public void ThrowsForAnInvalidMessage(string message)
            {
                var invalidEmail = Email.From($"{Guid.NewGuid()}@toggl.space");

                Func<Task> sendingFeedback = async () => await feedbackApi.Send(invalidEmail, message, new Dictionary<string, string>());

                sendingFeedback.Should().Throw<ArgumentException>();
            }

            [Fact]
            public async Task SerializesTheJsonCorrectly()
            {
                var email = Email.From($"{Guid.NewGuid()}@toggl.space");
                var message = "XYZ.";
                var data = new Dictionary<string, string>
                {
                    ["device"] = "SomePhone",
                    ["some random key"] = "some also random value"
                };
                var serializedJson = $"{{\"email\":\"{email}\",\"message\":\"{message}\",\"data\":[{{\"key\":\"device\",\"value\":\"{data["device"]}\"}},{{\"key\":\"some random key\",\"value\":\"{data["some random key"]}\"}}]}}";
                var response = Substitute.For<IResponse>();
                response.IsSuccess.Returns(true);
                apiClient.Send(Arg.Any<IRequest>()).Returns(Task.FromResult(response));

                await feedbackApi.Send(email, message, data);

                await apiClient.Received().Send(Arg.Is<IRequest>(req =>
                    req.Body.Left == serializedJson));
            }

            [Fact]
            public async Task SerializesTheJsonCorrectlyWithoutAnyAdditionalData()
            {
                var email = Email.From($"{Guid.NewGuid()}@toggl.space");
                var message = "XYZ.";
                var serializedJson = $"{{\"email\":\"{email}\",\"message\":\"{message}\",\"data\":[]}}";
                var response = Substitute.For<IResponse>();
                response.IsSuccess.Returns(true);
                apiClient.Send(Arg.Any<IRequest>()).Returns(Task.FromResult(response));

                await feedbackApi.Send(email, message, null);

                await apiClient.Received().Send(Arg.Is<IRequest>(req =>
                    req.Body.Left == serializedJson));
            }
        }
    }
}
