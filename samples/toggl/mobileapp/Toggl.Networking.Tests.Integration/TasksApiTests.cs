using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Toggl.Networking.Exceptions;
using Toggl.Networking.Helpers;
using Toggl.Networking.Models;
using Toggl.Networking.Tests.Integration.BaseTests;
using Toggl.Networking.Tests.Integration.Helper;
using Toggl.Shared.Models;
using Xunit;
using TogglTask = Toggl.Networking.Models.Task;

namespace Toggl.Networking.Tests.Integration
{
    public sealed class TasksApiTests
    {
        public sealed class TheGetAllMethod : AuthenticatedGetAllEndpointBaseTests<ITask>
        {
            private readonly SubscriptionPlanActivator plans = new SubscriptionPlanActivator();

            protected override Task<List<ITask>> CallEndpointWith(ITogglApi togglApi)
            {
                plans.EnsureDefaultWorkspaceIsOnPlan(togglApi, PricingPlans.StarterMonthly).Wait();
                return togglApi.Tasks.GetAll();
            }

            [Fact, LogTestInfo]
            public async System.Threading.Tasks.Task ReturnsAllTasks()
            {
                var (togglClient, user) = await SetupTestUser();
                var project = await createProject(togglClient, user.DefaultWorkspaceId.Value);
                await plans.EnsureDefaultWorkspaceIsOnPlan(user, PricingPlans.StarterMonthly);
                var taskA = randomTask(project, user.Id);
                await togglClient.Tasks.Create(taskA);
                var taskB = randomTask(project, user.Id);
                await togglClient.Tasks.Create(taskB);

                var tasks = await togglClient.Tasks.GetAll();

                tasks.Should().HaveCount(2);
                tasks.Should().Contain(x => isTheSameAs(taskA, x));
                tasks.Should().Contain(x => isTheSameAs(taskB, x));
            }

            [Fact, LogTestInfo]
            public async System.Threading.Tasks.Task ReturnsOnlyActiveTasks()
            {
                var (togglClient, user) = await SetupTestUser();
                var project = await createProject(togglClient, user.DefaultWorkspaceId.Value);
                await plans.EnsureDefaultWorkspaceIsOnPlan(user, PricingPlans.StarterMonthly);
                var task = randomTask(project, user.Id);
                await togglClient.Tasks.Create(task);
                var inactiveTask = randomTask(project, user.Id, isActive: false);
                await togglClient.Tasks.Create(inactiveTask);

                var tasks = await togglClient.Tasks.GetAll();

                tasks.Should().HaveCount(1);
                tasks.Should().Contain(x => isTheSameAs(x, task));
                tasks.Should().NotContain(x => isTheSameAs(x, inactiveTask));
            }

            [Fact, LogTestInfo]
            public async System.Threading.Tasks.Task ReturnsEmptyListWhenThereAreNoActiveTasks()
            {
                var (togglClient, user) = await SetupTestUser();
                var project = await createProject(togglClient, user.DefaultWorkspaceId.Value);
                await plans.EnsureDefaultWorkspaceIsOnPlan(user, PricingPlans.StarterMonthly);
                await togglClient.Tasks.Create(randomTask(project, user.Id, isActive: false));

                var tasks = await togglClient.Tasks.GetAll();

                tasks.Should().HaveCount(0);
            }
        }

        public sealed class TheGetAllSinceMethod : AuthenticatedGetSinceEndpointBaseTests<ITask>
        {
            private readonly SubscriptionPlanActivator plans = new SubscriptionPlanActivator();

            private IProject project;

            protected override Task<List<ITask>> CallEndpointWith(ITogglApi togglApi, DateTimeOffset threshold)
            {
                plans.EnsureDefaultWorkspaceIsOnPlan(togglApi, PricingPlans.StarterMonthly).Wait();
                return togglApi.Tasks.GetAllSince(threshold);
            }

            protected override DateTimeOffset AtDateOf(ITask model)
                => model.At;

            protected override ITask MakeUniqueModel(ITogglApi api, IUser user)
                => new TogglTask
                {
                    Active = true,
                    Name = Guid.NewGuid().ToString(),
                    WorkspaceId = user.DefaultWorkspaceId.Value,
                    ProjectId = getProject(api, user.DefaultWorkspaceId.Value).Id,
                    At = DateTimeOffset.UtcNow
                };

            protected override Task<ITask> PostModelToApi(ITogglApi api, ITask model)
            {
                plans.EnsureDefaultWorkspaceIsOnPlan(api, PricingPlans.StarterMonthly).Wait();
                return api.Tasks.Create(model);
            }

            protected override Expression<Func<ITask, bool>> ModelWithSameAttributesAs(ITask model)
                => t => isTheSameAs(model, t);

            private IProject getProject(ITogglApi api, long workspaceId)
                => project ?? (project = createProject(api, workspaceId).GetAwaiter().GetResult());
        }

        public sealed class TheCreateMethod : AuthenticatedPostEndpointBaseTests<ITask>
        {
            private readonly SubscriptionPlanActivator plans = new SubscriptionPlanActivator();

            [Fact, LogTestInfo]
            public async void CreatingTaskFailsInTheFreePlan()
            {
                var (togglApi, user) = await SetupTestUser();
                var project = await createProject(togglApi, user.DefaultWorkspaceId.Value);

                Action creatingTask = () => createTask(togglApi, project, user.Id).Wait();

                creatingTask.Should().Throw<ForbiddenException>();
            }

            [Theory, LogTestInfo]
            [InlineData(PricingPlans.StarterMonthly)]
            [InlineData(PricingPlans.StarterAnnual)]
            [InlineData(PricingPlans.PremiumMonthly)]
            [InlineData(PricingPlans.PremiumAnnual)]
            public async void CreatingTaskWorksForAllPricingPlansOtherThanTheFreePlan(PricingPlans plan)
            {
                var (togglApi, user) = await SetupTestUser();
                await plans.EnsureDefaultWorkspaceIsOnPlan(user, plan);
                var project = createProject(togglApi, user.DefaultWorkspaceId.Value)
                    .GetAwaiter().GetResult();

                Action creatingTask = () => createTask(togglApi, project, user.Id).Wait();

                creatingTask.Should().NotThrow();
            }

            protected override async Task<ITask> CallEndpointWith(ITogglApi togglApi)
            {
                var user = await togglApi.User.Get();
                await plans.EnsureDefaultWorkspaceIsOnPlan(user, PricingPlans.StarterMonthly);
                var project = await createProject(togglApi, user.DefaultWorkspaceId.Value);
                return await createTask(togglApi, project, user.Id);
            }
        }

        private static ITask randomTask(IProject project, long userId, bool isActive = true)
            => new TogglTask
            {
                WorkspaceId = project.WorkspaceId,
                ProjectId = project.Id,
                UserId = userId,
                Name = Guid.NewGuid().ToString(),
                Active = isActive,
                At = DateTimeOffset.UtcNow
            };

        private static IProject randomProject(long workspaceId)
            => new Project
            {
                WorkspaceId = workspaceId,
                Name = Guid.NewGuid().ToString(),
                Active = true,
                At = DateTimeOffset.UtcNow
            };

        private static Task<IProject> createProject(ITogglApi togglApi, long workspaceId)
            => togglApi.Projects.Create(randomProject(workspaceId));

        private static Task<ITask> createTask(ITogglApi togglApi, IProject project, long userId)
            => togglApi.Tasks.Create(randomTask(project, userId));

        private static bool isTheSameAs(ITask a, ITask b)
            => a.Name == b.Name
               && a.Active == b.Active
               && a.ProjectId == b.ProjectId
               && a.EstimatedSeconds == b.EstimatedSeconds
               && a.TrackedSeconds == b.TrackedSeconds
               && a.WorkspaceId == b.WorkspaceId
               && a.UserId == b.UserId;
    }
}
