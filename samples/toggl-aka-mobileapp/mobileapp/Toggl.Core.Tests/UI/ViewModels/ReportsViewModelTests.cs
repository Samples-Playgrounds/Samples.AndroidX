using FluentAssertions;
using Microsoft.Reactive.Testing;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using NUnit.Framework.Internal;
using Toggl.Core.Analytics;
using Toggl.Core.Interactors;
using Toggl.Core.Models;
using Toggl.Core.Models.Interfaces;
using Toggl.Core.Reports;
using Toggl.Core.Tests.Generators;
using Toggl.Core.Tests.Mocks;
using Toggl.Core.Tests.TestExtensions;
using Toggl.Core.UI.ViewModels.Reports;
using Toggl.Core.UI.Views;
using Toggl.Shared;
using Toggl.Shared.Extensions;
using Xunit;
using Notification = System.Reactive.Notification;
using Task = System.Threading.Tasks.Task;

namespace Toggl.Core.Tests.UI.ViewModels
{
    public sealed class ReportsViewModelTests
    {
        public abstract class ReportsViewModelTest : BaseViewModelTests<ReportsViewModel>
        {
            protected const long WorkspaceId = 10;

            protected IInteractor<IObservable<ProjectSummaryReport>> Interactor { get; }
                = Substitute.For<IInteractor<IObservable<ProjectSummaryReport>>>();

            public ReportsViewModelTest()
            {
                var workspaceObservable = Observable.Return(new MockWorkspace { Id = WorkspaceId });
                var workspaceIdObservable = Observable.Return(WorkspaceId);
                InteractorFactory.GetDefaultWorkspace().Execute().Returns(workspaceObservable);
                InteractorFactory.ObserveDefaultWorkspaceId().Execute().Returns(workspaceIdObservable);
            }

            protected override ReportsViewModel CreateViewModel()
            {
                InteractorFactory
                    .GetProjectSummary(Arg.Any<long>(), Arg.Any<DateTimeOffset>(), Arg.Any<DateTimeOffset?>())
                    .Returns(Interactor);

                return new ReportsViewModel(
                    DataSource,
                    TimeService,
                    NavigationService,
                    InteractorFactory,
                    AnalyticsService,
                    SchedulerProvider,
                    RxActionFactory
                );
            }

            protected async Task Initialize()
            {
                await ViewModel.Initialize();
                ViewModel.ViewAppeared();
                ViewModel.CalendarViewModel.ViewAppeared();
            }
        }

        public sealed class TheConstructor : ReportsViewModelTest
        {
            [Theory, LogIfTooSlow]
            [ConstructorData]
            public void ThrowsIfAnyOfTheArgumentsIsNull(bool useDataSource,
                                                        bool useTimeService,
                                                        bool useNavigationService,
                                                        bool useAnalyticsService,
                                                        bool useInteractorFactory,
                                                        bool useSchedulerProvider,
                                                        bool useRxActionFactory)
            {
                var timeService = useTimeService ? TimeService : null;
                var reportsProvider = useDataSource ? DataSource : null;
                var navigationService = useNavigationService ? NavigationService : null;
                var interactorFactory = useInteractorFactory ? InteractorFactory : null;
                var analyticsService = useAnalyticsService ? AnalyticsService : null;
                var schedulerProvider = useSchedulerProvider ? SchedulerProvider : null;
                var rxActionFactory = useRxActionFactory ? RxActionFactory : null;

                Action tryingToConstructWithEmptyParameters =
                    () => new ReportsViewModel(reportsProvider,
                                               timeService,
                                               navigationService,
                                               interactorFactory,
                                               analyticsService,
                                               schedulerProvider,
                                               rxActionFactory);

                tryingToConstructWithEmptyParameters
                    .Should().Throw<ArgumentNullException>();
            }
        }

        public sealed class TheBillablePercentageMethod : ReportsViewModelTest
        {
            [Fact, LogIfTooSlow]
            public async Task IsSetToNullIfTheTotalTimeOfAReportIsZero()
            {
                var billableObserver = TestScheduler.CreateObserver<float?>();
                var projectsNotSyncedCount = 0;
                TimeService.CurrentDateTime.Returns(DateTime.Now);
                TimeService.MidnightObservable.Returns(Observable.Never<DateTimeOffset>());
                Interactor.Execute()
                    .Returns(Observable.Return(new ProjectSummaryReport(new ChartSegment[0], projectsNotSyncedCount)));

                ViewModel.BillablePercentageObservable.Subscribe(billableObserver);

                await Initialize();

                TestScheduler.Start();

                var billablePercentage = billableObserver.Values().Last();
                billablePercentage.Should().BeNull();
            }
        }

        public sealed class TheIsLoadingProperty : ReportsViewModelTest
        {
            [Fact, LogIfTooSlow]
            public async Task IsSetToTrueWhenTheViewIsInitializedBeforeAnyLoadingOfReportsStarts()
            {
                var loadingObserver = TestScheduler.CreateObserver<bool>();
                TimeService.CurrentDateTime.Returns(DateTime.Now);
                ViewModel.IsLoadingObservable.Subscribe(loadingObserver);

                await ViewModel.Initialize();

                TestScheduler.Start();
                var isLoading = loadingObserver.Values().First();
                isLoading.Should().BeTrue();
            }

            [Fact, LogIfTooSlow]
            public async Task IsSetToTrueWhenAReportIsLoading()
            {
                var loadingObserver = TestScheduler.CreateObserver<bool>();
                var now = DateTimeOffset.Now;
                TimeService.CurrentDateTime.Returns(now);
                Interactor.Execute().Returns(Observable.Never<ProjectSummaryReport>());
                ViewModel.IsLoadingObservable.Subscribe(loadingObserver);

                await Initialize();

                TestScheduler.Start();
                var isLoading = loadingObserver.Values().Last();
                isLoading.Should().BeTrue();
            }

            [Fact, LogIfTooSlow]
            public async Task IsSetToFalseWhenLoadingIsCompleted()
            {
                var loadingObserver = TestScheduler.CreateObserver<bool>();
                var now = DateTimeOffset.Now;
                var projectsNotSyncedCount = 0;
                TimeService.CurrentDateTime.Returns(now);
                Interactor.Execute()
                    .Returns(Observable.Return(new ProjectSummaryReport(new ChartSegment[0], projectsNotSyncedCount)));
                ViewModel.IsLoadingObservable.Subscribe(loadingObserver);

                await Initialize();

                TestScheduler.Start();
                var isLoading = loadingObserver.Values().Last();
                isLoading.Should().BeFalse();
            }
        }

        public sealed class TheIsLoadingObservable : ReportsViewModelTest
        {
            private readonly ITestableObserver<bool> isLoadingObserver;

            public TheIsLoadingObservable()
            {
                isLoadingObserver = TestScheduler.CreateObserver<bool>();
                ViewModel.IsLoadingObservable.Subscribe(isLoadingObserver);
            }

            [Fact, LogIfTooSlow]
            public async Task IsSetToTrueWhenTheViewIsInitializedBeforeAnyLoadingOfReportsStarts()
            {
                TimeService.CurrentDateTime.Returns(DateTimeOffset.Now);
                await ViewModel.Initialize();

                TestScheduler.Start();
                isLoadingObserver.LastEmittedValue().Should().BeTrue();
            }

            [Fact, LogIfTooSlow]
            public async Task IsSetToTrueWhenAReportIsLoading()
            {
                var now = DateTimeOffset.Now;
                TimeService.CurrentDateTime.Returns(now);
                Interactor.Execute().Returns(Observable.Never<ProjectSummaryReport>());

                await Initialize();

                TestScheduler.Start();
                isLoadingObserver.LastEmittedValue().Should().BeTrue();
            }

            [Fact, LogIfTooSlow]
            public async Task IsSetToFalseWhenLoadingIsCompleted()
            {
                var now = DateTimeOffset.Now;
                var projectsNotSyncedCount = 0;
                TimeService.CurrentDateTime.Returns(now);
                Interactor.Execute().Returns(Observable.Return(new ProjectSummaryReport(new ChartSegment[0], projectsNotSyncedCount)));

                await Initialize();

                TestScheduler.Start();
                isLoadingObserver.LastEmittedValue().Should().BeFalse();
            }
        }

        public sealed class TheCurrentDateRangeStringProperty : ReportsViewModelTest
        {
            [Fact, LogIfTooSlow]
            public async Task IsInitializedToThisWeek()
            {
                var observer = TestScheduler.CreateObserver<string>();
                var now = DateTimeOffset.Now;
                TimeService.CurrentDateTime.Returns(now);
                ViewModel.CurrentDateRange.Subscribe(observer);
                await ViewModel.Initialize();

                TestScheduler.Start();
                observer.LastEmittedValue().Should().Be($"{Resources.ThisWeek} ▾");
            }

            public sealed class WhenAShortcutIsSelected : ReportsViewModelTest
            {
                public static IEnumerable<object[]> allCombinations
                {
                    get
                    {
                        foreach (var beginning in Enum.GetValues(typeof(BeginningOfWeek)))
                        {
                            yield return new object[] { beginning, ReportPeriod.Today, Resources.Today };
                            yield return new object[] { beginning, ReportPeriod.Yesterday, Resources.Yesterday };
                            yield return new object[] { beginning, ReportPeriod.LastMonth, Resources.LastMonth };
                            yield return new object[] { beginning, ReportPeriod.LastWeek, Resources.LastWeek };
                            yield return new object[] { beginning, ReportPeriod.LastYear, Resources.LastYear };
                            yield return new object[] { beginning, ReportPeriod.ThisMonth, Resources.ThisMonth };
                            yield return new object[] { beginning, ReportPeriod.ThisWeek, Resources.ThisWeek };
                            yield return new object[] { beginning, ReportPeriod.ThisYear, Resources.ThisYear };
                        }
                    }
                }

                [Theory, LogIfTooSlow]
                [MemberData(nameof(allCombinations))]
                public async Task EmitsCorrectStringWhenShortcutIsSelected(BeginningOfWeek beginningOfWeek, ReportPeriod period, string result)
                {
                    var user = new MockUser { BeginningOfWeek = beginningOfWeek };
                    DataSource.User.Current.Returns(Observable.Return(user));
                    var observer = TestScheduler.CreateObserver<string>();
                    var now = DateTimeOffset.Now;
                    TimeService.CurrentDateTime.Returns(now);
                    ViewModel.CurrentDateRange.Subscribe(observer);
                    await ViewModel.Initialize();

                    ViewModel.CalendarViewModel.SelectPeriod(period);

                    TestScheduler.Start();
                    observer.LastEmittedValue().Should().Be($"{result} ▾");
                }
            }

            public sealed class WhenADateRangeMatchingAShortcutIsSelected : ReportsViewModelTest
            {
                public static IEnumerable<object[]> allCombinations
                {
                    get
                    {
                        var now = DateTimeOffset.Now.RoundDownToLocalDate();
                        var beginningOfMonth = now.RoundDownToLocalMonth();
                        var beginningOfYear = now.RoundDownToLocalYear();

                        foreach (var beginning in Enum.GetValues(typeof(BeginningOfWeek)))
                        {
                            int diff = (7 + ((int)now.DayOfWeek - (int)beginning)) % 7;
                            var beginningOfWeek = now.AddDays(-1 * diff).Date;

                            yield return new object[] { beginning, now, now, Resources.Today };
                            yield return new object[] { beginning, now.AddDays(-1), now.AddDays(-1), Resources.Yesterday };
                            yield return new object[] { beginning, beginningOfMonth.AddMonths(-1), beginningOfMonth.AddDays(-1), Resources.LastMonth };
                            yield return new object[] { beginning, beginningOfWeek.AddDays(-7), beginningOfWeek.AddDays(-1), Resources.LastWeek };
                            yield return new object[] { beginning, beginningOfYear.AddMonths(-12), beginningOfYear.AddDays(-1), Resources.LastYear };
                            yield return new object[] { beginning, beginningOfMonth, beginningOfMonth.AddMonths(1).AddDays(-1), Resources.ThisMonth };
                            yield return new object[] { beginning, beginningOfWeek, beginningOfWeek.AddDays(6), Resources.ThisWeek };
                            yield return new object[] { beginning, beginningOfYear, beginningOfYear.AddMonths(12).AddDays(-1), Resources.ThisYear };
                        }
                    }
                }

                [Theory, LogIfTooSlow]
                [MemberData(nameof(allCombinations))]
                public async Task EmitsCorrectStringWhenDateRangeIsSelected(BeginningOfWeek beginningOfWeek, DateTimeOffset startDate, DateTimeOffset endDate, string result)
                {
                    var user = new MockUser { BeginningOfWeek = beginningOfWeek };
                    DataSource.User.Current.Returns(Observable.Return(user));
                    var observer = TestScheduler.CreateObserver<string>();
                    var now = DateTimeOffset.Now;
                    TimeService.CurrentDateTime.Returns(now);
                    ViewModel.CurrentDateRange.Subscribe(observer);
                    await ViewModel.Initialize();

                    await ViewModel.LoadReport(0, startDate, endDate, ReportsSource.Calendar);

                    TestScheduler.Start();
                    observer.LastEmittedValue().Should().Be($"{result} ▾");
                }
            }

            public sealed class WhenARandomDateRangeIsSelected : ReportsViewModelTest
            {
                public static IEnumerable<object[]> randomCombinations
                {
                    get
                    {
                        yield return new object[] { new DateTime(2019, 5, 23), new DateTime(2019, 5, 29), "05/23 - 05/29" };
                        yield return new object[] { new DateTime(2017, 5, 23), new DateTime(2019, 5, 29), "05/23 - 05/29" };
                        yield return new object[] { new DateTime(2019, 5, 23), new DateTime(2019, 5, 30), "05/23 - 05/30" };
                        yield return new object[] { new DateTime(2019, 3, 3), new DateTime(2019, 5, 29), "03/03 - 05/29" };
                    }
                }

                [Theory, LogIfTooSlow]
                [MemberData(nameof(randomCombinations))]
                public async Task EmitsCorrectStringWhenDateRangeIsSelected(DateTime startDate, DateTime endDate, string result)
                {
                    var mockPreferences = new MockPreferences
                    {
                        DateFormat = DateFormat.FromLocalizedDateFormat("MM/DD")
                    };
                    DataSource.Preferences.Current.Returns(Observable.Return(mockPreferences));

                    var observer = TestScheduler.CreateObserver<string>();
                    var now = DateTimeOffset.Now;
                    TimeService.CurrentDateTime.Returns(now);
                    ViewModel.CurrentDateRange.Subscribe(observer);
                    await ViewModel.Initialize();

                    await ViewModel.LoadReport(0, startDate, endDate, ReportsSource.Calendar);

                    TestScheduler.Start();
                    observer.LastEmittedValue().Should().Be($"{result} ▾");
                }
            }
        }

        public sealed class TheSegmentsProperty : ReportsViewModelTest
        {
            private readonly int projectsNotSyncedCount = 0;

            [Fact]
            public async Task DoesNotGroupProjectSegmentsWithPercentageGreaterThanOrEqualFivePercent()
            {
                ChartSegment[] segments =
                {
                    new ChartSegment("Project 1", "Client 1", 2, 2, 0, "#ffffff"),
                    new ChartSegment("Project 2", "Client 2", 2, 2, 0, "#ffffff"),
                    new ChartSegment("Project 3", "Client 3", 17, 17, 0, "#ffffff"),
                    new ChartSegment("Project 4", "Client 4", 23, 23, 0, "#ffffff"),
                    new ChartSegment("Project 5", "Client 5", 56, 56, 0, "#ffffff")
                };

                TimeService.CurrentDateTime.Returns(new DateTimeOffset(2018, 05, 15, 12, 00, 00, TimeSpan.Zero));
                Interactor.Execute().Returns(Observable.Return(new ProjectSummaryReport(segments, projectsNotSyncedCount)));
                var segmentsObservable = TestScheduler.CreateObserver<IReadOnlyList<ChartSegment>>();
                var groupedSegmentsObservable = TestScheduler.CreateObserver<IReadOnlyList<ChartSegment>>();
                ViewModel.SegmentsObservable.Subscribe(segmentsObservable);
                ViewModel.GroupedSegmentsObservable.Subscribe(groupedSegmentsObservable);

                await Initialize();

                TestScheduler.Start();

                var actualSegments = segmentsObservable.Values().Last();
                var actualGroupedSegments = groupedSegmentsObservable.Values().Last();
                actualSegments.Should().HaveCount(5);
                actualGroupedSegments.Should().HaveCount(4);
                actualGroupedSegments.Should().Contain(segment =>
                    segment.ProjectName == Resources.Other &&
                    segment.Percentage == segments[0].Percentage + segments[1].Percentage);
                actualGroupedSegments
                    .Where(project => project.ProjectName != Resources.Other)
                    .Select(segment => segment.Percentage)
                    .ForEach(percentage => percentage.Should().BeGreaterOrEqualTo(5));
            }

            [Fact]
            public async Task GroupsProjectSegmentsWithPercentageLesserThanOnePercent()
            {
                ChartSegment[] segments =
                {
                    new ChartSegment("Project 1", "Client 1", 0.9f, 2, 0, "#ffffff"),
                    new ChartSegment("Project 2", "Client 2", 0.3f, 3, 0, "#ffffff"),
                    new ChartSegment("Project 3", "Client 3", 7.8f, 4, 0, "#ffffff"),
                    new ChartSegment("Project 4", "Client 4", 12, 12, 0, "#ffffff"),
                    new ChartSegment("Project 5", "Client 5", 23, 23, 0, "#ffffff"),
                    new ChartSegment("Project 6", "Client 6", 56, 56, 0, "#ffffff")
                };

                TimeService.CurrentDateTime.Returns(new DateTimeOffset(2018, 05, 15, 12, 00, 00, TimeSpan.Zero));
                Interactor.Execute().Returns(Observable.Return(new ProjectSummaryReport(segments, projectsNotSyncedCount)));

                var segmentsObservable = TestScheduler.CreateObserver<IReadOnlyList<ChartSegment>>();
                var groupedSegmentsObservable = TestScheduler.CreateObserver<IReadOnlyList<ChartSegment>>();
                ViewModel.SegmentsObservable.Subscribe(segmentsObservable);
                ViewModel.GroupedSegmentsObservable.Subscribe(groupedSegmentsObservable);

                await Initialize();

                TestScheduler.Start();

                var actualSegments = segmentsObservable.Values().Last();
                var actualGroupedSegments = groupedSegmentsObservable.Values().Last();
                actualSegments.Should().HaveCount(6);
                actualGroupedSegments.Should().HaveCount(5);
                actualGroupedSegments.Should().Contain(segment =>
                    segment.ProjectName == Resources.Other &&
                    segment.Percentage == segments[0].Percentage + segments[1].Percentage);
                actualGroupedSegments
                    .Where(project => project.ProjectName != Resources.Other)
                    .Select(segment => segment.Percentage)
                    .ForEach(percentage => percentage.Should().BeGreaterOrEqualTo(5));
            }

            [Fact]
            public async Task GroupsOtherProjectsToAtLeastOnePercentRegardlessOfActualPercentage()
            {
                ChartSegment[] segments =
                {
                    new ChartSegment("Project 1", "Client 1", 0.2f, 2, 0, "#ffffff"),
                    new ChartSegment("Project 2", "Client 2", 0.3f, 3, 0, "#ffffff"),
                    new ChartSegment("Project 3", "Client 3", 8.5f, 4, 0, "#ffffff"),
                    new ChartSegment("Project 4", "Client 4", 12, 12, 0, "#ffffff"),
                    new ChartSegment("Project 5", "Client 5", 23, 23, 0, "#ffffff"),
                    new ChartSegment("Project 6", "Client 6", 56, 56, 0, "#ffffff")
                };

                TimeService.CurrentDateTime.Returns(new DateTimeOffset(2018, 05, 15, 12, 00, 00, TimeSpan.Zero));
                Interactor.Execute().Returns(Observable.Return(new ProjectSummaryReport(segments, projectsNotSyncedCount)));

                var segmentsObservable = TestScheduler.CreateObserver<IReadOnlyList<ChartSegment>>();
                var groupedSegmentsObservable = TestScheduler.CreateObserver<IReadOnlyList<ChartSegment>>();
                ViewModel.SegmentsObservable.Subscribe(segmentsObservable);
                ViewModel.GroupedSegmentsObservable.Subscribe(groupedSegmentsObservable);

                await Initialize();

                TestScheduler.Start();

                var actualSegments = segmentsObservable.Values().Last();
                var actualGroupedSegments = groupedSegmentsObservable.Values().Last();

                actualSegments.Should().HaveCount(6);
                actualGroupedSegments.Should().HaveCount(5);
                actualGroupedSegments.Should().Contain(segment =>
                    segment.ProjectName == Resources.Other &&
                    segment.Percentage == 1f);
                actualGroupedSegments
                    .Where(project => project.ProjectName != Resources.Other)
                    .Select(segment => segment.Percentage)
                    .ForEach(percentage => percentage.Should().BeGreaterOrEqualTo(5));
            }

            [Fact]
            public async Task GroupsProjectSegmentsWithPercentageBetweenOneAndFiveIntoOtherIfTotalOfOtherLessThanFivePercent()
            {
                ChartSegment[] segments =
                {
                    new ChartSegment("Project 1", "Client 1", 0.9f, 2, 0, "#ffffff"),
                    new ChartSegment("Project 2", "Client 2", 0.9f, 3, 0, "#ffffff"),
                    new ChartSegment("Project 3", "Client 3", 2.5f, 4, 0, "#ffffff"),
                    new ChartSegment("Project 4", "Client 4", 4, 12, 0, "#ffffff"),
                    new ChartSegment("Project 5", "Client 5", 31.7f, 23, 0, "#ffffff"),
                    new ChartSegment("Project 6", "Client 6", 60, 56, 0, "#ffffff")
                };

                TimeService.CurrentDateTime.Returns(new DateTimeOffset(2018, 05, 15, 12, 00, 00, TimeSpan.Zero));
                Interactor.Execute().Returns(Observable.Return(new ProjectSummaryReport(segments, projectsNotSyncedCount)));

                var segmentsObservable = TestScheduler.CreateObserver<IReadOnlyList<ChartSegment>>();
                var groupedSegmentsObservable = TestScheduler.CreateObserver<IReadOnlyList<ChartSegment>>();
                ViewModel.SegmentsObservable.Subscribe(segmentsObservable);
                ViewModel.GroupedSegmentsObservable.Subscribe(groupedSegmentsObservable);

                await Initialize();

                TestScheduler.Start();

                var actualSegments = segmentsObservable.Values().Last();
                var actualGroupedSegments = groupedSegmentsObservable.Values().Last();

                actualSegments.Should().HaveCount(6);
                actualGroupedSegments.Should().HaveCount(4);
                actualGroupedSegments.Should().Contain(segment =>
                    segment.ProjectName == Resources.Other &&
                    segment.Percentage == segments[0].Percentage + segments[1].Percentage + segments[2].Percentage);
                actualGroupedSegments
                    .Where(project => project.ProjectName != Resources.Other)
                    .Select(segment => segment.Percentage)
                    .ForEach(percentage => percentage.Should().BeGreaterOrEqualTo(4));
            }

            [Fact]
            public async Task SetsOtherProjectWithOneSegmentToThatSegmentButWithOnePercentIfLessThanOnePercent()
            {
                ChartSegment[] segments =
                {
                    new ChartSegment("Project 1", "Client 1", 0.2f, 2, 0, "#666666"),
                    new ChartSegment("Project 2", "Client 2", 8.8f, 4, 0, "#ffffff"),
                    new ChartSegment("Project 3", "Client 3", 12, 12, 0, "#ffffff"),
                    new ChartSegment("Project 4", "Client 4", 23, 23, 0, "#ffffff"),
                    new ChartSegment("Project 5", "Client 5", 56, 56, 0, "#ffffff")
                };

                TimeService.CurrentDateTime.Returns(new DateTimeOffset(2018, 05, 15, 12, 00, 00, TimeSpan.Zero));
                Interactor.Execute().Returns(Observable.Return(new ProjectSummaryReport(segments, projectsNotSyncedCount)));

                var segmentsObservable = TestScheduler.CreateObserver<IReadOnlyList<ChartSegment>>();
                var groupedSegmentsObservable = TestScheduler.CreateObserver<IReadOnlyList<ChartSegment>>();
                ViewModel.SegmentsObservable.Subscribe(segmentsObservable);
                ViewModel.GroupedSegmentsObservable.Subscribe(groupedSegmentsObservable);

                await Initialize();

                TestScheduler.Start();

                var actualSegments = segmentsObservable.Values().Last();
                var actualGroupedSegments = groupedSegmentsObservable.Values().Last();

                actualSegments.Should().HaveCount(5);
                actualGroupedSegments.Should().HaveCount(5);
                actualGroupedSegments.Should().Contain(segment =>
                    segment.ProjectName == "Project 1" &&
                    segment.Percentage == 1f &&
                    segment.Color == "#666666");
                actualGroupedSegments
                    .Where(project => project.ProjectName != "Project 1")
                    .Select(segment => segment.Percentage)
                    .ForEach(percentage => percentage.Should().BeGreaterOrEqualTo(5));
            }
        }

        public sealed class TheSelectWorkspaceCommand : ReportsViewModelTest
        {
            [Fact, LogIfTooSlow]
            public async Task ShouldTriggerAReportReload()
            {
                TimeService.CurrentDateTime.Returns(DateTimeOffset.Now);
                await ViewModel.Initialize();
                ViewModel.CalendarViewModel.ViewAppeared();
                TestScheduler.Start();

                var mockWorkspace = new MockWorkspace { Id = WorkspaceId + 1 };
                View.Select(Arg.Any<string>(), Arg.Any<IEnumerable<SelectOption<IThreadSafeWorkspace>>>(), Arg.Any<int>())
                    .Returns(Observable.Return(mockWorkspace));

                ViewModel.SelectWorkspace.Execute();
                TestScheduler.Start();

                InteractorFactory.Received().GetProjectSummary(
                    Arg.Is(mockWorkspace.Id),
                    Arg.Any<DateTimeOffset>(),
                    Arg.Any<DateTimeOffset>());
            }

            [Fact, LogIfTooSlow]
            public async Task ShouldChangeCurrentWorkspaceName()
            {
                TimeService.CurrentDateTime.Returns(DateTimeOffset.Now);
                var observer = TestScheduler.CreateObserver<string>();
                ViewModel.WorkspaceNameObservable.Subscribe(observer);

                var mockWorkspace = new MockWorkspace { Id = WorkspaceId + 1, Name = "Selected workspace" };
                View.Select(Arg.Any<string>(), Arg.Any<IEnumerable<SelectOption<IThreadSafeWorkspace>>>(), Arg.Any<int>())
                    .Returns(Observable.Return(mockWorkspace));
                InteractorFactory.GetDefaultWorkspace().Execute().Returns(Observable.Return(mockWorkspace));

                await ViewModel.Initialize();

                ViewModel.SelectWorkspace.Execute();
                TestScheduler.Start();

                observer.Messages.AssertEqual(
                    ReactiveTest.OnNext(1, ""),
                    ReactiveTest.OnNext(2, mockWorkspace.Name)
                );
            }

            [Fact, LogIfTooSlow]
            public async Task ShouldNotTriggerAReportReloadWhenSelectionIsCancelled()
            {
                TimeService.CurrentDateTime.Returns(DateTimeOffset.Now);
                await ViewModel.Initialize();
                View.Select(Arg.Any<string>(), Arg.Any<IEnumerable<SelectOption<IThreadSafeWorkspace>>>(), Arg.Any<int>())
                    .Returns(Observable.Return<IThreadSafeWorkspace>(null));

                ViewModel.SelectWorkspace.Execute();
                TestScheduler.Start();

                InteractorFactory.DidNotReceive().GetProjectSummary(
                    Arg.Any<long>(),
                    Arg.Any<DateTimeOffset>(),
                    Arg.Any<DateTimeOffset>());
            }

            [Fact, LogIfTooSlow]
            public async Task ShouldNotTriggerAReportReloadWhenTheSameWorkspaceIsSelected()
            {
                TimeService.CurrentDateTime.Returns(DateTimeOffset.Now);
                await ViewModel.Initialize();

                var mockWorkspace = new MockWorkspace { Id = WorkspaceId };
                View.Select(Arg.Any<string>(), Arg.Any<IEnumerable<SelectOption<IThreadSafeWorkspace>>>(), Arg.Any<int>())
                    .Returns(Observable.Return<IThreadSafeWorkspace>(mockWorkspace));

                ViewModel.SelectWorkspace.Execute();
                TestScheduler.Start();

                InteractorFactory.DidNotReceive().GetProjectSummary(Arg.Any<long>(), Arg.Any<DateTimeOffset>(),
                    Arg.Any<DateTimeOffset>());
            }
        }

        public sealed class TheStartDateAndEndDateObservables : ReportsViewModelTest
        {
            private readonly ITestableObserver<DateTimeOffset> startDateObserver;
            private readonly ITestableObserver<DateTimeOffset> endDateObserver;

            public TheStartDateAndEndDateObservables()
            {
                startDateObserver = TestScheduler.CreateObserver<DateTimeOffset>();
                endDateObserver = TestScheduler.CreateObserver<DateTimeOffset>();

                ViewModel.StartDate.Subscribe(startDateObserver);
                ViewModel.EndDate.Subscribe(endDateObserver);
            }

            [Fact, LogIfTooSlow]
            public async Task DoesNotEmitAnyValuesDuringInitialization()
            {
                TimeService.CurrentDateTime.Returns(DateTimeOffset.Now);
                await ViewModel.Initialize();

                startDateObserver.Messages.Should().BeEmpty();
                endDateObserver.Messages.Should().BeEmpty();
            }
        }

        public sealed class TheWorkspaceHasBillableFeatureEnabledObservable : ReportsViewModelTest
        {
            private readonly ITestableObserver<bool> isEnabledObserver;

            public TheWorkspaceHasBillableFeatureEnabledObservable()
            {
                isEnabledObserver = TestScheduler.CreateObserver<bool>();
                ViewModel.WorkspaceHasBillableFeatureEnabled.Subscribe(isEnabledObserver);
            }

            [Fact]
            public async Task IsDisabledByDefault()
            {
                TimeService.CurrentDateTime.Returns(DateTimeOffset.Now);
                await ViewModel.Initialize();

                TestScheduler.Start();
                isEnabledObserver.SingleEmittedValue().Should().BeFalse();
            }

            [Fact]
            public async Task StaysDisabledWhenSwitchingToAFreeWorkspace()
            {
                TimeService.CurrentDateTime.Returns(DateTimeOffset.Now);
                prepareWorkspace(isProEnabled: false);

                await ViewModel.Initialize();

                ViewModel.SelectWorkspace.Execute();
                TestScheduler.Start();

                isEnabledObserver.LastEmittedValue().Should().BeFalse();
            }

            [Fact]
            public async Task BecomesEnabledWhenSwitchingToAProWorkspace()
            {
                TimeService.CurrentDateTime.Returns(DateTimeOffset.Now);
                prepareWorkspace(isProEnabled: true);

                await Initialize();
                TestScheduler.Start();

                ViewModel.SelectWorkspace.Execute();
                TestScheduler.Start();

                isEnabledObserver.LastEmittedValue().Should().BeTrue();
            }

            private void prepareWorkspace(bool isProEnabled)
            {
                var workspace = new MockWorkspace { Id = 123 };
                var workspaceFeatures = new MockWorkspaceFeatureCollection
                {
                    Features = new[] { new MockWorkspaceFeature { FeatureId = WorkspaceFeatureId.Pro, Enabled = isProEnabled } }
                };

                var workspaceFeaturesObservable = Observable.Return(workspaceFeatures);
                var workspaceObservable = Observable.Return(workspace);
                InteractorFactory.GetWorkspaceFeaturesById(workspace.Id)
                    .Execute()
                    .Returns(workspaceFeaturesObservable);

                View.Select(Arg.Any<string>(), Arg.Any<IEnumerable<SelectOption<IThreadSafeWorkspace>>>(), Arg.Any<int>())
                    .Returns(workspaceObservable);
            }
        }

        public sealed class TheViewAppearedMethod : ReportsViewModelTest
        {
            [Theory, LogIfTooSlow]
            [InlineData(1)]
            [InlineData(2)]
            [InlineData(3)]
            [InlineData(5)]
            [InlineData(8)]
            [InlineData(13)]
            public async Task ShouldTriggerReloadForEveryAppearanceAfterSignificantTimePassed(int numberOfAppearances)
            {
                var timeService = new TimeService(TestScheduler);
                TestScheduler.AdvanceTo(DateTimeOffset.Now.Ticks);

                InteractorFactory
                    .GetProjectSummary(Arg.Any<long>(), Arg.Any<DateTimeOffset>(), Arg.Any<DateTimeOffset?>())
                    .Returns(Interactor);

                var viewModel = new ReportsViewModel(
                    DataSource,
                    timeService,
                    NavigationService,
                    InteractorFactory,
                    AnalyticsService,
                    SchedulerProvider,
                    RxActionFactory
                );

                Interactor.Execute()
                    .ReturnsForAnyArgs(Observable.Empty<ProjectSummaryReport>(SchedulerProvider.TestScheduler));
                await viewModel.Initialize();
                viewModel.CalendarViewModel.ViewAppeared();
                InteractorFactory.ClearReceivedCalls();

                for (int i = 0; i < numberOfAppearances; ++i)
                {
                    viewModel.ViewDisappeared();
                    TestScheduler.AdvanceBy(TimeSpan.FromSeconds(5).Ticks);
                    viewModel.ViewAppeared();
                }

                TestScheduler.Start();

                InteractorFactory
                    .Received(numberOfAppearances)
                    .GetProjectSummary(Arg.Any<long>(), Arg.Any<DateTimeOffset>(), Arg.Any<DateTimeOffset>());
            }

            [Theory, LogIfTooSlow]
            [InlineData(1)]
            [InlineData(2)]
            [InlineData(3)]
            [InlineData(5)]
            [InlineData(8)]
            [InlineData(13)]
            public async Task ShouldNotTriggerReloadAfterDisappearingAndAppearingImmediately(int numberOfAppearances)
            {
                TimeService.CurrentDateTime.Returns(DateTimeOffset.Now);
                Interactor.Execute()
                    .ReturnsForAnyArgs(Observable.Empty<ProjectSummaryReport>(SchedulerProvider.TestScheduler));
                await ViewModel.Initialize();
                ViewModel.CalendarViewModel.ViewAppeared();
                InteractorFactory.ClearReceivedCalls();

                for (int i = 0; i < numberOfAppearances; ++i)
                {
                    ViewModel.ViewDisappeared();
                    ViewModel.ViewAppeared();
                }

                TestScheduler.Start();

                InteractorFactory
                    .DidNotReceive()
                    .GetProjectSummary(Arg.Any<long>(), Arg.Any<DateTimeOffset>(), Arg.Any<DateTimeOffset>());
            }
        }

        public sealed class TheReports : ReportsViewModelTest
        {
            [Fact, LogIfTooSlow]
            public async Task AreNotReloadedAfterTheViewAppearedWhenTheDefaultWorkspaceHasNotChanged()
            {
                TimeService.CurrentDateTime.Returns(DateTimeOffset.Now);
                await ViewModel.Initialize();
                ViewModel.ViewAppeared();

                TestScheduler.Start();

                InteractorFactory.Received(1).GetProjectSummary(
                    Arg.Any<long>(),
                    Arg.Any<DateTimeOffset>(),
                    Arg.Any<DateTimeOffset>());
            }

            [Fact, LogIfTooSlow]
            public async Task AreReloadedWhenANewWorkspaceIdIsObserved()
            {
                var initialWorkspaceId = 10L;
                var newlySelectedWorkspaceId = 11L;
                var observable = TestScheduler.CreateColdObservable(
                    OnNext(0, initialWorkspaceId),
                    OnNext(1, newlySelectedWorkspaceId)
                );
                var workspace10 = Observable.Return(new MockWorkspace(initialWorkspaceId));
                var workspace11 = Observable.Return(new MockWorkspace(newlySelectedWorkspaceId));
                InteractorFactory.GetWorkspaceById(initialWorkspaceId).Execute().Returns(workspace10);
                InteractorFactory.GetWorkspaceById(newlySelectedWorkspaceId).Execute().Returns(workspace11);
                InteractorFactory.ObserveDefaultWorkspaceId().Execute().Returns(observable);
                TimeService.CurrentDateTime.Returns(DateTimeOffset.Now);
                await ViewModel.Initialize();
                ViewModel.ViewAppeared();
                TestScheduler.AdvanceTo(100);
                TestScheduler.Start();

                InteractorFactory.Received(1).GetProjectSummary(
                    initialWorkspaceId,
                    Arg.Any<DateTimeOffset>(),
                    Arg.Any<DateTimeOffset>());
                InteractorFactory.Received(1).GetProjectSummary(
                    newlySelectedWorkspaceId,
                    Arg.Any<DateTimeOffset>(),
                    Arg.Any<DateTimeOffset>());
            }
        }
    }
}
