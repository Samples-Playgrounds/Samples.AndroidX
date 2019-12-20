using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Toggl.Networking.Exceptions;
using Toggl.Networking.Models;
using Toggl.Networking.Tests.Integration.BaseTests;
using Toggl.Networking.Tests.Integration.Helper;
using Toggl.Shared;
using Toggl.Shared.Models;
using Xunit;
using Task = System.Threading.Tasks.Task;

namespace Toggl.Networking.Tests.Integration
{
    public sealed class WorkspacesApiTests
    {
        public sealed class TheGetMethod : AuthenticatedEndpointBaseTests<List<IWorkspace>>
        {
            protected override Task<List<IWorkspace>> CallEndpointWith(ITogglApi togglApi)
                => togglApi.Workspaces.GetAll();

            [Fact, LogTestInfo]
            public async Task ReturnsOneWorkspaceForANewUser()
            {
                var (togglClient, user) = await SetupTestUser();

                var workspaces = await togglClient.Workspaces.GetAll();

                workspaces.Should().HaveCount(1);
                workspaces.Should().Contain(ws => ws.Id == user.DefaultWorkspaceId);
            }

            [Fact, LogTestInfo]
            public async Task ReturnsAllWorkspaces()
            {
                var (togglClient, user) = await SetupTestUser();
                var secondWorkspace =
                    await togglClient.Workspaces.Create(new Workspace { Name = Guid.NewGuid().ToString() });

                var workspaces = await togglClient.Workspaces.GetAll();

                workspaces.Should().HaveCount(2);
                workspaces.Should().Contain(ws => ws.Id == user.DefaultWorkspaceId);
                workspaces.Should().Contain(ws => ws.Id == secondWorkspace.Id);
            }

            [Fact, LogTestInfo]
            public async Task ReturnsEmptyListWhenTheDefaultWorkspaceIsDeleted()
            {
                var (togglClient, user) = await SetupTestUser();
                await WorkspaceHelper.Delete(user, user.DefaultWorkspaceId.Value).ConfigureAwait(false);

                var workspaces = await togglClient.Workspaces.GetAll();

                workspaces.Should().BeEmpty();
            }
        }

        public sealed class TheGetByIdMethod : AuthenticatedGetEndpointBaseTests<IWorkspace>
        {
            protected override async Task<IWorkspace> CallEndpointWith(ITogglApi togglApi)
            {
                var user = await togglApi.User.Get();
                return await togglApi.Workspaces.GetById(user.DefaultWorkspaceId.Value);
            }

            [Fact, LogTestInfo]
            public async Task ReturnsDefaultWorkspace()
            {
                var (togglClient, user) = await SetupTestUser();

                var workspace = await togglClient.Workspaces.GetById(user.DefaultWorkspaceId.Value);

                workspace.Id.Should().Be(user.DefaultWorkspaceId);
            }

            [Fact, LogTestInfo]
            public async Task ReturnsCreatedWorkspace()
            {
                var (togglClient, user) = await SetupTestUser();
                var secondWorkspace = await togglClient.Workspaces.Create(new Workspace { Name = Guid.NewGuid().ToString() });

                var workspace = await togglClient.Workspaces.GetById(secondWorkspace.Id);

                workspace.Id.Should().Be(secondWorkspace.Id);
                workspace.Name.Should().Be(secondWorkspace.Name);
            }

            [Fact, LogTestInfo]
            public async Task FailsForWrongWorkspaceId()
            {
                var (togglClient, user) = await SetupTestUser();

                Func<Task> gettingById = async () => await togglClient.Workspaces.GetById(user.DefaultWorkspaceId.Value - 1);

                gettingById.Should().Throw<ForbiddenException>();
            }
        }

        public sealed class TheCreateMethod : AuthenticatedPostEndpointBaseTests<IWorkspace>
        {
            protected override Task<IWorkspace> CallEndpointWith(ITogglApi togglApi)
                => togglApi.Workspaces.Create(new Workspace { Name = Guid.NewGuid().ToString() });

            [Fact, LogTestInfo]
            public async Task CreatesWorkspaceWithGivenName()
            {
                var (api, user) = await SetupTestUser();
                var name = Guid.NewGuid().ToString();

                var workspace = await api.Workspaces.Create(new Workspace { Name = name });

                workspace.Name.Should().Be(name);
            }

            [Fact, LogTestInfo]
            public async Task CreatesWorkspaceWithTheFreePlan()
            {
                var (api, user) = await SetupTestUser();
                var name = Guid.NewGuid().ToString();

                var workspace = await api.Workspaces.Create(new Workspace { Name = name });
                var features = (await api.WorkspaceFeatures.GetAll())
                    .Single(workspaceFeatures => workspaceFeatures.WorkspaceId == workspace.Id);

                features.Features.Should().Contain(feature =>
                    feature.FeatureId == WorkspaceFeatureId.Pro && feature.Enabled == false);
            }
        }
    }
}
