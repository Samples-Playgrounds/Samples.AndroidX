using FluentAssertions;
using System;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Toggl.Networking.ApiClients;
using Toggl.Networking.Helpers;
using Toggl.Networking.Models;
using Toggl.Networking.Tests.Integration.BaseTests;
using Toggl.Networking.Tests.Integration.Helper;
using Toggl.Shared.Models;
using Toggl.Shared.Models.Reports;
using Xunit;
using Task = System.Threading.Tasks.Task;

namespace Toggl.Networking.Tests.Integration
{
    public sealed class ProjectsSummaryReportsApiTests
    {
        public sealed class TheGetByWorkspaceMethod : AuthenticatedEndpointBaseTests<IProjectsSummary>
        {
            [Fact, LogTestInfo]
            public async Task ReturnsEmptyListWhenTheUserHasNoTimeEntriesInTheSpecifiedRange()
            {
                var (api, user) = await SetupTestUser();
                var start = DateTimeOffset.UtcNow.AddDays(-8);
                var end = start.AddDays(5);

                var summary = await api.ProjectsSummary.GetByWorkspace(user.DefaultWorkspaceId.Value, start, end);

                summary.ProjectsSummaries.Should().BeEmpty();
            }

            [Fact, LogTestInfo]
            public async Task UsesStartPlusSevenDaysWhenThereIsNoEndDateSpecified()
            {
                var (api, user) = await SetupTestUser();
                var start = DateTimeOffset.UtcNow.AddDays(-8);
                await createTimeEntry(api.TimeEntries, user, start.AddDays(2), 1);
                await createTimeEntry(api.TimeEntries, user, start.AddDays(8), 1);

                var summary =
                    await api.ProjectsSummary.GetByWorkspace(user.DefaultWorkspaceId.Value, start, null);

                summary.ProjectsSummaries.Should().HaveCount(1);
            }

            [Fact, LogTestInfo]
            public async Task DoesNotReturnTrackedBillableSecondsWhenWorkspaceIsNotPaid()
            {
                var (api, user) = await SetupTestUser();
                var start = DateTimeOffset.UtcNow.AddDays(-8);
                await createTimeEntry(api.TimeEntries, user, start.AddDays(2), 10, false);
                await createTimeEntry(api.TimeEntries, user, start.AddDays(8), 20, false);

                var summary = await api.ProjectsSummary.GetByWorkspace(user.DefaultWorkspaceId.Value, start, DateTimeOffset.UtcNow);

                summary.ProjectsSummaries.ForEach(project => project.BillableSeconds.Should().BeNull());
            }

            [Fact, LogTestInfo]
            public async Task ReturnsTheSumOfTrackedSecondsForTheGivenInterval()
            {
                var (api, user) = await SetupTestUser();
                var start = DateTimeOffset.UtcNow.AddDays(-8);
                var project = await api.Projects.Create(new Project { Active = true, Name = Guid.NewGuid().ToString(), WorkspaceId = user.DefaultWorkspaceId.Value });
                await createTimeEntry(api.TimeEntries, user, start.AddDays(2), 10, false, project.Id);
                await createTimeEntry(api.TimeEntries, user, start.AddDays(3), 20, false, project.Id);
                await createTimeEntry(api.TimeEntries, user, start.AddDays(8), 40, false, project.Id);

                var summary = await api.ProjectsSummary.GetByWorkspace(user.DefaultWorkspaceId.Value, start, start.AddDays(4));
                var projectSummary = summary.ProjectsSummaries.Single(s => s.ProjectId == project.Id);

                projectSummary.TrackedSeconds.Should().Be(30);
            }

            [Fact, LogTestInfo]
            public async Task ReturnsTheSumOfTrackedSecondsForTheGivenIntervalForEachProject()
            {
                var (api, user) = await SetupTestUser();
                var start = DateTimeOffset.UtcNow.AddDays(-6);
                var projectA = await api.Projects.Create(new Project { Active = true, Name = Guid.NewGuid().ToString(), WorkspaceId = user.DefaultWorkspaceId.Value });
                var projectB = await api.Projects.Create(new Project { Active = true, Name = Guid.NewGuid().ToString(), WorkspaceId = user.DefaultWorkspaceId.Value });
                await createTimeEntry(api.TimeEntries, user, start.AddDays(2), 10, false, projectA.Id);
                await createTimeEntry(api.TimeEntries, user, start.AddDays(3), 20, false, projectA.Id);
                await createTimeEntry(api.TimeEntries, user, start.AddDays(4), 40, false, projectB.Id);
                await createTimeEntry(api.TimeEntries, user, start.AddDays(5), 25, false, projectB.Id);

                var summary =
                    await api.ProjectsSummary.GetByWorkspace(user.DefaultWorkspaceId.Value, start, null);
                var projectASummary = summary.ProjectsSummaries.Single(s => s.ProjectId == projectA.Id);
                var projectBSummary = summary.ProjectsSummaries.Single(s => s.ProjectId == projectB.Id);

                projectASummary.TrackedSeconds.Should().Be(30);
                projectBSummary.TrackedSeconds.Should().Be(65);
            }

            [Fact, LogTestInfo]
            public async Task ReturnsTheSumOfBillableSecondsForTheGivenIntervalWhenTheWorkspaceIsPaid()
            {
                var (api, user) = await SetupTestUser();
                var start = DateTimeOffset.UtcNow.AddDays(-8);
                await WorkspaceHelper.SetSubscription(user, user.DefaultWorkspaceId.Value, PricingPlans.StarterAnnual);
                var project = await api.Projects.Create(new Project { Active = true, Name = Guid.NewGuid().ToString(), WorkspaceId = user.DefaultWorkspaceId.Value });
                await createTimeEntry(api.TimeEntries, user, start.AddDays(2), 10, false, project.Id);
                await createTimeEntry(api.TimeEntries, user, start.AddDays(3), 20, true, project.Id);
                await createTimeEntry(api.TimeEntries, user, start.AddDays(8), 40, true, project.Id);

                var summary = await api.ProjectsSummary.GetByWorkspace(user.DefaultWorkspaceId.Value, start, start.AddDays(5));
                var projectSummary = summary.ProjectsSummaries.Single(s => s.ProjectId == project.Id);

                projectSummary.TrackedSeconds.Should().Be(30);
                projectSummary.BillableSeconds.Should().Be(20);
            }

            [Fact, LogTestInfo]
            public async Task DoesNotIncludeRunningTimeEntriesInTheReports()
            {
                var (api, user) = await SetupTestUser();
                var start = DateTimeOffset.UtcNow.AddDays(-8);
                var project = await api.Projects.Create(new Project { Active = true, Name = Guid.NewGuid().ToString(), WorkspaceId = user.DefaultWorkspaceId.Value });
                await createTimeEntry(api.TimeEntries, user, start.AddDays(1), null, false, project.Id);
                await createTimeEntry(api.TimeEntries, user, start.AddDays(2), 10, false, project.Id);
                await createTimeEntry(api.TimeEntries, user, start.AddDays(3), 20, false, project.Id);

                var summary = await api.ProjectsSummary.GetByWorkspace(user.DefaultWorkspaceId.Value, start, start.AddDays(4));
                var projectSummary = summary.ProjectsSummaries.Single(s => s.ProjectId == project.Id);

                projectSummary.TrackedSeconds.Should().Be(30);
            }

            [Fact, LogTestInfo]
            public async Task IncludesSummariesForArchivedProjects()
            {
                var (api, user) = await SetupTestUser();
                var start = DateTimeOffset.UtcNow.AddDays(-8);
                var project = await api.Projects.Create(new Project { Active = false, Name = Guid.NewGuid().ToString(), WorkspaceId = user.DefaultWorkspaceId.Value });
                await createTimeEntry(api.TimeEntries, user, start.AddDays(2), 10, false, project.Id);

                var summary = await api.ProjectsSummary.GetByWorkspace(user.DefaultWorkspaceId.Value, start, start.AddDays(4));
                var projectSummary = summary.ProjectsSummaries.Single(s => s.ProjectId == project.Id);

                projectSummary.TrackedSeconds.Should().Be(10);
            }

            private async Task<ITimeEntry> createTimeEntry(ITimeEntriesApi api, IUser user, DateTimeOffset at, long? duration, bool billable = false, long? projectId = null)
                => await api.Create(new TimeEntry
                {
                    At = at,
                    Description = Guid.NewGuid().ToString(),
                    Start = at,
                    Duration = duration,
                    UserId = user.Id,
                    WorkspaceId = user.DefaultWorkspaceId.Value,
                    TagIds = new long[0],
                    Billable = billable,
                    ProjectId = projectId
                });

            protected override async Task<IProjectsSummary> CallEndpointWith(ITogglApi togglApi)
            {
                var start = new DateTimeOffset(2017, 01, 10, 11, 22, 33, TimeSpan.Zero);
                var end = start.AddMonths(5);
                var user = await ValidApi.User.Get();
                return await togglApi.ProjectsSummary
                    .GetByWorkspace(user.DefaultWorkspaceId.Value, start, end);
            }
        }
    }
}
