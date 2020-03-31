using FluentAssertions;
using System;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Toggl.Networking.ApiClients;
using Toggl.Networking.Helpers;
using Toggl.Networking.Tests.Integration.BaseTests;
using Toggl.Networking.Tests.Integration.Helper;
using Toggl.Shared;
using Toggl.Shared.Extensions;
using Toggl.Shared.Models;
using Toggl.Shared.Models.Reports;
using Xunit;

namespace Toggl.Networking.Tests.Integration
{
    public sealed class TimeEntriesReportsApiTests : AuthenticatedEndpointBaseTests<ITimeEntriesTotals>
    {
        [Fact, LogTestInfo]
        public async Task ReturnsZerosWhenTheUserDoesNotHaveAnyTimeEntries()
        {
            var (api, user) = await SetupTestUser();
            var start = DateTimeOffset.UtcNow.AddDays(-8);
            var end = start.AddDays(5);

            var totals = await api.TimeEntriesReports.GetTotals(user.Id, user.DefaultWorkspaceId.Value, start, end);

            totals.Resolution.Should().Be(Resolution.Day);
            totals.Groups.Should().HaveCount(6);
            totals.Groups.ForEach(group =>
            {
                group.Total.Should().Be(TimeSpan.Zero);
                group.Billable.Should().Be(TimeSpan.Zero);
            });
        }

        [Fact, LogTestInfo]
        public async Task ReturnsTrackedSecondsSumWithZeroBillableForAFreeWorkspace()
        {
            var (api, user) = await SetupTestUser();
            var start = DateTimeOffset.UtcNow.AddDays(-8);
            var end = start.AddDays(5);

            await createTimeEntry(api.TimeEntries, user, start, 1, false);
            await createTimeEntry(api.TimeEntries, user, start, 2, false);
            await createTimeEntry(api.TimeEntries, user, start.AddDays(1), 3, false);
            await createTimeEntry(api.TimeEntries, user, start.AddDays(1), 4, false);

            var totals = await api.TimeEntriesReports.GetTotals(user.Id, user.DefaultWorkspaceId.Value, start, end);

            totals.Resolution.Should().Be(Resolution.Day);
            totals.Groups.Should().HaveCount(6);
            totals.Groups[0].Total.Should().Be(TimeSpan.FromSeconds(3));
            totals.Groups[0].Billable.Should().Be(TimeSpan.Zero);
            totals.Groups[1].Total.Should().Be(TimeSpan.FromSeconds(7));
            totals.Groups[1].Billable.Should().Be(TimeSpan.Zero);
        }

        [Fact, LogTestInfo]
        public async Task ReturnsTrackedBillableSecondsSumsForAPaidWorkspace()
        {
            var (api, user) = await SetupTestUser();
            await WorkspaceHelper.SetSubscription(user, user.DefaultWorkspaceId.Value, PricingPlans.StarterAnnual);
            var start = DateTimeOffset.UtcNow.AddDays(-8);
            var end = start.AddDays(5);

            await createTimeEntry(api.TimeEntries, user, start, 1, false);
            await createTimeEntry(api.TimeEntries, user, start, 2, true);
            await createTimeEntry(api.TimeEntries, user, start.AddDays(1), 3, false);
            await createTimeEntry(api.TimeEntries, user, start.AddDays(1), 4, true);

            var totals = await api.TimeEntriesReports.GetTotals(user.Id, user.DefaultWorkspaceId.Value, start, end);

            totals.Resolution.Should().Be(Resolution.Day);
            totals.Groups.Should().HaveCount(6);
            totals.Groups[0].Total.Should().Be(TimeSpan.FromSeconds(3));
            totals.Groups[0].Billable.Should().Be(TimeSpan.FromSeconds(2));
            totals.Groups[1].Total.Should().Be(TimeSpan.FromSeconds(7));
            totals.Groups[1].Billable.Should().Be(TimeSpan.FromSeconds(4));
        }

        [Theory, LogTestInfo]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(33)]
        [InlineData(34)]
        [InlineData(183)]
        [InlineData(365)]
        public async Task ReturnsAtMost34ItemsForTheGraph(int days)
        {
            var (api, user) = await SetupTestUser();
            var start = DateTimeOffset.UtcNow.AddDays(-days);
            var end = DateTimeOffset.UtcNow;

            var totals = await api.TimeEntriesReports.GetTotals(user.Id, user.DefaultWorkspaceId.Value, start, end);

            totals.Groups.Should().HaveCountLessOrEqualTo(34);
        }

        [Fact, LogTestInfo]
        public async Task DoesNotAllowToUseMoreThan365DaysRange()
        {
            var (api, user) = await SetupTestUser();
            var start = DateTimeOffset.UtcNow.AddDays(-366);
            var end = DateTimeOffset.UtcNow;

            Action callingTheApi = () => api.TimeEntriesReports.GetTotals(user.Id, user.DefaultWorkspaceId.Value, start, end).Wait();

            callingTheApi.Should().Throw<ArgumentOutOfRangeException>();
        }

        private async Task<ITimeEntry> createTimeEntry(ITimeEntriesApi api, IUser user, DateTimeOffset start, long? duration, bool billable)
            => await api.Create(new Models.TimeEntry
            {
                At = start,
                Description = Guid.NewGuid().ToString(),
                Start = start,
                Duration = duration,
                UserId = user.Id,
                WorkspaceId = user.DefaultWorkspaceId.Value,
                TagIds = new long[0],
                Billable = billable
            });

        protected override async Task<ITimeEntriesTotals> CallEndpointWith(ITogglApi togglApi)
        {
            var start = new DateTimeOffset(2017, 01, 10, 11, 22, 33, TimeSpan.Zero);
            var end = start.AddMonths(5);
            var user = await togglApi.User.Get();
            return await togglApi.TimeEntriesReports
                .GetTotals(user.Id, user.DefaultWorkspaceId.Value, start, end);
        }
    }
}
