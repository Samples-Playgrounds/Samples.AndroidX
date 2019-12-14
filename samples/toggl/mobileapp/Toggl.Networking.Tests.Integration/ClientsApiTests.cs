using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Toggl.Networking.Tests.Integration.BaseTests;
using Toggl.Shared.Models;
using Xunit;
using Client = Toggl.Networking.Models.Client;

namespace Toggl.Networking.Tests.Integration
{
    public sealed class ClientsApiTests
    {
        private static Expression<Func<IClient, bool>> clientWithSameIdNameAndWorkspaceAs(IClient client)
            => c => c.Id == client.Id && c.Name == client.Name && c.WorkspaceId == client.WorkspaceId;

        public sealed class TheGetAllMethod : AuthenticatedGetAllEndpointBaseTests<IClient>
        {
            protected override Task<List<IClient>> CallEndpointWith(ITogglApi togglApi)
                => togglApi.Clients.GetAll();

            [Fact, LogTestInfo]
            public async Task ReturnsAllClients()
            {
                var (togglClient, user) = await SetupTestUser();

                var firstClient = new Client { Name = "First", WorkspaceId = user.DefaultWorkspaceId.Value };
                var firstClientPosted = await togglClient.Clients.Create(firstClient);
                var secondClient = new Client { Name = "Second", WorkspaceId = user.DefaultWorkspaceId.Value };
                var secondClientPosted = await togglClient.Clients.Create(secondClient);

                var clients = await togglClient.Clients.GetAll();

                clients.Should().HaveCount(2);
                clients.Should().Contain(clientWithSameIdNameAndWorkspaceAs(firstClientPosted));
                clients.Should().Contain(clientWithSameIdNameAndWorkspaceAs(secondClientPosted));
            }
        }

        public sealed class TheGetAllSinceMethod : AuthenticatedGetSinceEndpointBaseTests<IClient>
        {
            protected override Task<List<IClient>> CallEndpointWith(ITogglApi togglApi, DateTimeOffset threshold)
                => togglApi.Clients.GetAllSince(threshold);

            protected override DateTimeOffset AtDateOf(IClient model) => model.At;

            protected override IClient MakeUniqueModel(ITogglApi api, IUser user)
                => new Client { Name = Guid.NewGuid().ToString(), WorkspaceId = user.DefaultWorkspaceId.Value };

            protected override Task<IClient> PostModelToApi(ITogglApi api, IClient model)
                => api.Clients.Create(model);

            protected override Expression<Func<IClient, bool>> ModelWithSameAttributesAs(IClient model)
                => clientWithSameIdNameAndWorkspaceAs(model);
        }

        public sealed class TheCreateMethod : AuthenticatedPostEndpointBaseTests<IClient>
        {
            protected override async Task<IClient> CallEndpointWith(ITogglApi togglApi)
            {
                var user = await togglApi.User.Get();
                var client = new Client { Name = Guid.NewGuid().ToString(), WorkspaceId = user.DefaultWorkspaceId.Value };
                return await togglApi.Clients.Create(client);
            }

            [Fact, LogTestInfo]
            public async Task CreatesNewClient()
            {
                var (togglClient, user) = await SetupTestUser();
                var newClient = new Client { Name = Guid.NewGuid().ToString(), WorkspaceId = user.DefaultWorkspaceId.Value };

                var persistedClient = await togglClient.Clients.Create(newClient);

                persistedClient.Name.Should().Be(newClient.Name);
                persistedClient.WorkspaceId.Should().Be(newClient.WorkspaceId);
            }
        }
    }
}
