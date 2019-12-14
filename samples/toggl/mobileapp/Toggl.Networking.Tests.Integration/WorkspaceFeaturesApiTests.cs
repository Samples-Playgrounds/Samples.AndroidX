using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Toggl.Networking.Models;
using Toggl.Networking.Tests.Integration.BaseTests;
using Toggl.Shared;
using Toggl.Shared.Models;
using Xunit;
using Task = System.Threading.Tasks.Task;

namespace Toggl.Networking.Tests.Integration
{
    public sealed class WorkspaceFeaturesApiTests
    {
        public sealed class TheGetAllMethod : AuthenticatedEndpointBaseTests<List<IWorkspaceFeatureCollection>>
        {
            protected override Task<List<IWorkspaceFeatureCollection>> CallEndpointWith(ITogglApi togglApi)
                => togglApi.WorkspaceFeatures.GetAll();

            [Fact, LogTestInfo]
            public async Task ReturnsAllWorkspaceFeatures()
            {
                var (togglClient, user) = await SetupTestUser();
                var featuresInEnum = Enum.GetValues(typeof(WorkspaceFeatureId));

                var workspaceFeatureCollections = await togglClient.WorkspaceFeatures.GetAll();
                var distinctWorkspacesCount = workspaceFeatureCollections.Select(f => f.WorkspaceId).Distinct().Count();

                workspaceFeatureCollections.Should().HaveCount(1);
                workspaceFeatureCollections.First().Features.Should().HaveCount(featuresInEnum.Length);
                distinctWorkspacesCount.Should().Be(1);
            }

            [Fact, LogTestInfo]
            public async Task ReturnsAllWorkspaceFeaturesForMultipleWorkspaces()
            {
                var (togglClient, user) = await SetupTestUser();
                var anotherWorkspace = await togglClient.Workspaces.Create(new Workspace { Name = Guid.NewGuid().ToString() });

                var workspaceFeatureCollection = await togglClient.WorkspaceFeatures.GetAll();

                workspaceFeatureCollection.Should().HaveCount(2);
                workspaceFeatureCollection.Should().Contain(collection => collection.WorkspaceId == user.DefaultWorkspaceId);
                workspaceFeatureCollection.Should().Contain(collection => collection.WorkspaceId == anotherWorkspace.Id);
            }
        }
    }
}
