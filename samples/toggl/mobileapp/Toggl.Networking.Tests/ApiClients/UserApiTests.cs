using FluentAssertions;
using NSubstitute;
using System;
using System.Threading.Tasks;
using Toggl.Networking.ApiClients;
using Toggl.Networking.Models;
using Toggl.Networking.Network;
using Toggl.Networking.Serialization;
using Toggl.Shared;
using Toggl.Shared.Models;
using Xunit;
using Task = System.Threading.Tasks.Task;

namespace Toggl.Networking.Tests.ApiClients
{
    public class UserApiTests
    {
        public abstract class Base
        {
            private readonly IApiClient apiClient = Substitute.For<IApiClient>();
            private readonly IJsonSerializer jsonSerializer = Substitute.For<IJsonSerializer>();
            private readonly Credentials credentials = Credentials.None;
            private readonly UserApi api;

            public Base()
            {
                api = new UserApi(
                    new Endpoints(ApiEnvironment.Staging),
                    apiClient,
                    jsonSerializer,
                    credentials);
            }

            [Fact, LogIfTooSlow]
            public void SucceedsIfReturnedUserHasApiToken()
            {
                var userWithValidApiToken = userWithApiToken(Guid.NewGuid().ToString());
                setupMocksToReturnUser(userWithValidApiToken);

                var user = CallEndpoint(api).GetAwaiter().GetResult();

                user.Should().Be(userWithValidApiToken);
            }

            protected abstract Task<IUser> CallEndpoint(IUserApi api);

            private Action callingEndpointWithReturnedUser(User user)
            {
                setupMocksToReturnUser(user);

                return () => CallEndpoint(api).Wait();
            }

            private static User userWithApiToken(string apiToken)
                => new User { ApiToken = apiToken };

            private static IResponse successfulResponse()
            {
                var response = Substitute.For<IResponse>();
                response.IsSuccess.Returns(true);
                response.RawData.Returns("some data so it tries to serialize");
                return response;
            }

            private void setupMocksToReturnUser(User user)
            {
                var response = successfulResponse();
                apiClient.Send(Arg.Any<IRequest>()).Returns(taskReturning(response));
                jsonSerializer.Deserialize<User>(Arg.Any<string>()).Returns(user);
            }

            private static Task<T> taskReturning<T>(T value) => Task.Run(() => value);
        }

        public class TheGetMethod : Base
        {
            protected override Task<IUser> CallEndpoint(IUserApi api)
                => api.Get();
        }

        public class TheGetWithGoogleMethod : Base
        {
            protected override Task<IUser> CallEndpoint(IUserApi api)
                => api.GetWithGoogle();
        }

        public class TheUpdateMethod : Base
        {
            protected override Task<IUser> CallEndpoint(IUserApi api)
                => api.Update(new User());
        }

        public class TheSignUpMethod : Base
        {
            protected override Task<IUser> CallEndpoint(IUserApi api)
                => api.SignUp(Email.From("a@b.com"), Password.Empty, true, 237, null);
        }

        public class TheSignUpWithGoogleMethod : Base
        {
            protected override Task<IUser> CallEndpoint(IUserApi api)
                => api.SignUpWithGoogle("", true, 237, null);
        }
    }
}
