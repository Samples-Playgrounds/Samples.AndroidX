using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Toggl.Networking.Exceptions;
using Toggl.Networking.Models;
using Toggl.Networking.Tests.Integration.BaseTests;
using Toggl.Shared.Models;
using Xunit;
using ThreadingTask = System.Threading.Tasks.Task;

namespace Toggl.Networking.Tests.Integration
{
    public sealed class ProjectsApiTests
    {
        public sealed class TheGetAllMethod : AuthenticatedGetAllEndpointBaseTests<IProject>
        {
            protected override Task<List<IProject>> CallEndpointWith(ITogglApi togglApi)
                => togglApi.Projects.GetAll();

            [Fact, LogTestInfo]
            public async ThreadingTask ReturnsAllProjects()
            {
                var (togglClient, user) = await SetupTestUser();

                var projectA = await createNewProject(togglClient, user.DefaultWorkspaceId.Value, createClient: true);
                var projectAPosted = await togglClient.Projects.Create(projectA);

                var projectB = await createNewProject(togglClient, user.DefaultWorkspaceId.Value);
                var projectBPosted = await togglClient.Projects.Create(projectB);

                var projects = await togglClient.Projects.GetAll();

                projects.Should().HaveCount(2);

                projects.Should().Contain(project => isTheSameAs(projectAPosted, project));
                projects.Should().Contain(project => isTheSameAs(projectBPosted, project));
            }

            [Fact, LogTestInfo]
            public async ThreadingTask ReturnsOnlyActiveProjects()
            {
                var (togglClient, user) = await SetupTestUser();

                var activeProject = await createNewProject(togglClient, user.DefaultWorkspaceId.Value);
                var activeProjectPosted = await togglClient.Projects.Create(activeProject);

                var inactiveProject = await createNewProject(togglClient, user.DefaultWorkspaceId.Value, isActive: false);
                var inactiveProjectPosted = await togglClient.Projects.Create(inactiveProject);

                var projects = await togglClient.Projects.GetAll();

                projects.Should().HaveCount(1);
                projects.Should().Contain(project => isTheSameAs(project, activeProjectPosted));
                projects.Should().NotContain(project => isTheSameAs(project, inactiveProjectPosted));
            }

            [Fact, LogTestInfo]
            public async ThreadingTask ReturnsEmptyListWhenThereAreNoActiveProjects()
            {
                var (togglClient, user) = await SetupTestUser();

                var noProjects = await togglClient.Projects.GetAll();

                Project project = await createNewProject(togglClient, user.DefaultWorkspaceId.Value, isActive: false);
                await togglClient.Projects.Create(project);

                project = await createNewProject(togglClient, user.DefaultWorkspaceId.Value, isActive: false);
                await togglClient.Projects.Create(project);

                var activeProjects = await togglClient.Projects.GetAll();

                noProjects.Should().HaveCount(0);
                activeProjects.Should().HaveCount(0);
            }
        }

        public sealed class TheGetAllSinceMethod : AuthenticatedGetSinceEndpointBaseTests<IProject>
        {
            protected override Task<List<IProject>> CallEndpointWith(ITogglApi togglApi, DateTimeOffset threshold)
                => togglApi.Projects.GetAllSince(threshold);

            protected override DateTimeOffset AtDateOf(IProject model)
                => model.At;

            protected override IProject MakeUniqueModel(ITogglApi api, IUser user)
                => new Project { Active = true, Name = Guid.NewGuid().ToString(), WorkspaceId = user.DefaultWorkspaceId.Value };

            protected override Task<IProject> PostModelToApi(ITogglApi api, IProject model)
                => api.Projects.Create(model);

            protected override Expression<Func<IProject, bool>> ModelWithSameAttributesAs(IProject model)
                => p => isTheSameAs(model, p);


            [Fact, LogTestInfo]
            public async ThreadingTask ReturnsProjectsAccordingToTheFetchArchivedFlag()
            {
                var (togglClient, user) = await SetupTestUser();

                var activeProject = await createNewProject(togglClient, user.DefaultWorkspaceId.Value);
                var activeProjectPosted = await togglClient.Projects.Create(activeProject);

                var inactiveProject = await createNewProject(togglClient, user.DefaultWorkspaceId.Value, isActive: false);
                var inactiveProjectPosted = await togglClient.Projects.Create(inactiveProject);

                var projects = await togglClient.Projects.GetAllSince(activeProjectPosted.At);

                projects.Should()
                    .Contain(project => isTheSameAs(project, activeProjectPosted));

                projects.Should().HaveCount(2);
            }
        }

        public sealed class TheCreateMethod : AuthenticatedPostEndpointBaseTests<IProject>
        {
            protected override async Task<IProject> CallEndpointWith(ITogglApi togglApi)
            {
                var user = await togglApi.User.Get();
                var project = await createNewProject(togglApi, user.DefaultWorkspaceId.Value);
                return await CallEndpointWith(togglApi, project);
            }

            private Task<IProject> CallEndpointWith(ITogglApi togglApi, IProject project)
                => togglApi.Projects.Create(project);

            [Fact, LogTestInfo]
            public async System.Threading.Tasks.Task CreatesNewProject()
            {
                var (togglClient, user) = await SetupTestUser();

                var project = await createNewProject(togglClient, user.DefaultWorkspaceId.Value);
                var persistedProject = await togglClient.Projects.Create(project);

                persistedProject.Name.Should().Be(project.Name);
                persistedProject.ClientId.Should().Be(project.ClientId);
                persistedProject.IsPrivate.Should().Be(project.IsPrivate);
                persistedProject.Color.Should().Be(project.Color);
            }
        }

        public sealed class TheSearchMethod : AuthenticatedEndpointBaseTests<List<IProject>>
        {
            [Fact, LogTestInfo]
            public async ThreadingTask ThrowsArgumentNullExceptionForNullIds()
            {
                var (togglApi, user) = await SetupTestUser();

                Action searchingNull = () => togglApi.Projects.Search(user.DefaultWorkspaceId.Value, null).Wait();

                searchingNull.Should().Throw<ArgumentNullException>();
            }

            [Fact, LogTestInfo]
            public async ThreadingTask ThrowsBadRequestExceptionForEmtpyArrayOfIds()
            {
                var (togglApi, user) = await SetupTestUser();
                var projectIds = new long[0];

                Action searchingWithEmptyIds = () => togglApi.Projects.Search(user.DefaultWorkspaceId.Value, projectIds).Wait();

                searchingWithEmptyIds.Should().Throw<BadRequestException>();
            }

            [Fact, LogTestInfo]
            public async ThreadingTask ReturnsEmtpyArrayForProjectsWhichDontExistOrDoNotBelongToOtherUser()
            {
                var (togglApi, user) = await SetupTestUser();
                var projectIds = new long[] { 1, 2, 3 };

                var projects = await togglApi.Projects.Search(user.DefaultWorkspaceId.Value, projectIds);

                projects.Should().HaveCount(0);
            }

            [Fact, LogTestInfo]
            public async ThreadingTask DoesNotFindProjectInAnInaccessibleWorkspace()
            {
                var (togglApiA, userA) = await SetupTestUser();
                var (togglApiB, userB) = await SetupTestUser();
                var projectA = await togglApiA.Projects.Create(new Project { Name = Guid.NewGuid().ToString(), WorkspaceId = userA.DefaultWorkspaceId.Value });

                var projects = await togglApiB.Projects.Search(userB.DefaultWorkspaceId.Value, new[] { projectA.Id });

                projects.Should().HaveCount(0);
            }

            [Fact, LogTestInfo]
            public async ThreadingTask DoesNotFindProjectInADifferentWorkspace()
            {
                var (togglApi, user) = await SetupTestUser();
                var secondWorkspace = await togglApi.Workspaces.Create(new Workspace { Name = Guid.NewGuid().ToString() });
                var projectA = await togglApi.Projects.Create(new Project { Name = Guid.NewGuid().ToString(), WorkspaceId = secondWorkspace.Id });
                var projectB = await togglApi.Projects.Create(new Project { Name = Guid.NewGuid().ToString(), WorkspaceId = secondWorkspace.Id });

                var projects = await togglApi.Projects.Search(user.DefaultWorkspaceId.Value, new[] { projectA.Id, projectB.Id });

                projects.Should().HaveCount(0);
            }

            [Fact, LogTestInfo]
            public async ThreadingTask ReturnsOnlyProjectInTheSearchedWorkspace()
            {
                var (togglApi, user) = await SetupTestUser();
                var secondWorkspace = await togglApi.Workspaces.Create(new Workspace { Name = Guid.NewGuid().ToString() });
                var projectA = await togglApi.Projects.Create(new Project { Name = Guid.NewGuid().ToString(), WorkspaceId = user.DefaultWorkspaceId.Value });
                var projectB = await togglApi.Projects.Create(new Project { Name = Guid.NewGuid().ToString(), WorkspaceId = secondWorkspace.Id });

                var projects = await togglApi.Projects.Search(user.DefaultWorkspaceId.Value, new[] { projectA.Id, projectB.Id });

                projects.Should().HaveCount(1);
                projects.Should().Contain(p => p.Id == projectA.Id);
            }

            protected override async Task<List<IProject>> CallEndpointWith(ITogglApi togglApi)
            {
                var user = await togglApi.User.Get();
                return await togglApi.Projects.Search(user.DefaultWorkspaceId.Value, new[] { -1L });
            }
        }

        private static async Task<Project> createNewProject(ITogglApi togglClient, long workspaceID, bool isActive = true, bool createClient = false)
        {
            IClient client = null;

            if (createClient)
            {
                client = new Client
                {
                    Name = Guid.NewGuid().ToString(),
                    WorkspaceId = workspaceID
                };

                client = await togglClient.Clients.Create(client);
            }

            return new Project
            {
                Name = Guid.NewGuid().ToString(),
                WorkspaceId = workspaceID,
                At = DateTimeOffset.UtcNow,
                Color = "#06aaf5",
                Active = isActive,
                ClientId = client?.Id
            };
        }

        private static bool isTheSameAs(IProject a, IProject b)
            => a.Id == b.Id
            && a.Name == b.Name
            && a.ClientId == b.ClientId
            && a.IsPrivate == b.IsPrivate
            && a.Color == b.Color;
    }
}
