using NSubstitute;
using System;
using System.Reactive.Linq;
using Toggl.Core.Analytics;
using Toggl.Core.Interactors;
using Toggl.Core.Models;
using Toggl.Core.Models.Interfaces;
using Toggl.Core.Tests.Mocks;
using Toggl.Core.Tests.TestExtensions;
using Xunit;
using Task = System.Threading.Tasks.Task;

namespace Toggl.Core.Tests.Interactors.TimeEntry
{
    public sealed class ContinueTimeEntryInteractorTests : BaseInteractorTests
    {
        private const long ProjectId = 10;

        private const long WorkspaceId = 11;

        private const long UserId = 12;

        private const long TaskId = 14;

        private long[] tagIds = { 15 };

        private const string validDescription = "Some random time entry";

        private DateTimeOffset startTime = new DateTimeOffset(2019, 6, 5, 14, 0, 0, TimeSpan.Zero);

        public ContinueTimeEntryInteractorTests()
        {
            var user = new MockUser { Id = UserId, DefaultWorkspaceId = WorkspaceId };
            var timeEntryToContinue = new MockTimeEntry
            {
                Id = 42,
                ProjectId = ProjectId,
                WorkspaceId = WorkspaceId,
                UserId = UserId,
                TaskId = TaskId,
                TagIds = tagIds,
                Description = validDescription,
                Start = startTime,
            };

            DataSource.User.Get().Returns(Observable.Return(user));
            DataSource.TimeEntries.GetById(42)
                .ReturnsObservableOf(timeEntryToContinue);

            TimeService.CurrentDateTime.Returns(DateTimeOffset.Now);
        }

        [Fact, LogIfTooSlow]
        public async Task CreatesATimeEntryWithTheCurrentTimeProvidedByTheTimeService()
        {
            var continueInteractor = new ContinueTimeEntryInteractor(
                InteractorFactory,
                TimeService,
                TimeEntryStartOrigin.SingleTimeEntrySwipe,
                42);
            await continueInteractor.Execute();

            await DataSource.TimeEntries.Received().Create(Arg.Is<IThreadSafeTimeEntry>(
                te => te.Start == TimeService.CurrentDateTime
            ));
        }

        [Theory, LogIfTooSlow]
        [InlineData(ContinueTimeEntryMode.SingleTimeEntryContinueButton)]
        [InlineData(ContinueTimeEntryMode.SingleTimeEntrySwipe)]
        [InlineData(ContinueTimeEntryMode.TimeEntriesGroupContinueButton)]
        [InlineData(ContinueTimeEntryMode.TimeEntriesGroupSwipe)]
        public async Task PropagatesCorrectTimeEntryStartOriginToAnalytics(ContinueTimeEntryMode continueMode)
        {
            var continueInteractor = new ContinueTimeEntryInteractor(
                InteractorFactory,
                TimeService,
                (TimeEntryStartOrigin)continueMode,
                42);
            await continueInteractor.Execute();

            AnalyticsService.Received().Track(Arg.Is<StartTimeEntryEvent>(
                ev => (int)ev.Origin == (int)continueMode && ev.Origin.ToString() == continueMode.ToString()));
        }
    }
}
