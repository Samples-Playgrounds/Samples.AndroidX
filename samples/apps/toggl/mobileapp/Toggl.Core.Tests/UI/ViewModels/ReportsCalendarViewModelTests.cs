﻿using FluentAssertions;
using FsCheck.Xunit;
using Microsoft.Reactive.Testing;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reactive.Linq;
using Toggl.Core.Analytics;
using Toggl.Core.Models;
using Toggl.Core.Models.Interfaces;
using Toggl.Core.Tests.Generators;
using Toggl.Core.Tests.TestExtensions;
using Toggl.Core.UI.Parameters;
using Toggl.Core.UI.ViewModels;
using Toggl.Core.UI.ViewModels.ReportsCalendar;
using Toggl.Core.UI.ViewModels.ReportsCalendar.QuickSelectShortcuts;
using Toggl.Shared;
using Xunit;
using Task = System.Threading.Tasks.Task;

namespace Toggl.Core.Tests.UI.ViewModels
{
    public sealed class ReportsCalendarViewModelTests
    {
        public abstract class ReportsCalendarViewModelTest
            : BaseViewModelTests<ReportsCalendarViewModel>
        {
            protected override ReportsCalendarViewModel CreateViewModel()
                => new ReportsCalendarViewModel(TimeService, DataSource, RxActionFactory, NavigationService, SchedulerProvider);
        }

        public sealed class TheConstructor : ReportsCalendarViewModelTest
        {
            [Theory, LogIfTooSlow]
            [ConstructorData]
            public void ThrowsIfAnyOfTheArgumentsIsNull(
                bool useTimeService,
                bool useDataSource,
                bool useRxActionFactory,
                bool useNavigationService,
                bool useSchedulerProvider
            )
            {
                var timeService = useTimeService ? TimeService : null;
                var dataSource = useDataSource ? DataSource : null;
                var rxActionFactory = useRxActionFactory ? RxActionFactory : null;
                var navigationService = useNavigationService ? NavigationService : null;
                var schedulerProvider = useSchedulerProvider ? SchedulerProvider : null;

                Action tryingToConstructWithEmptyParameters =
                    () => new ReportsCalendarViewModel(timeService, dataSource, rxActionFactory, navigationService, schedulerProvider);

                tryingToConstructWithEmptyParameters
                    .Should().Throw<ArgumentNullException>();
            }
        }

        public sealed class TheInitializeMethod : ReportsCalendarViewModelTest
        {
            [Property]
            public async void InitializesCurrentMonthPropertySetOnPrepareToCurrentDateTimeOfTimeService(DateTimeOffset now)
            {
                var observer = TestScheduler.CreateObserver<CalendarMonth>();
                TimeService.CurrentDateTime.Returns(now);

                ViewModel.Initialize();
                await ViewModel.Initialize();
                ViewModel.CurrentMonthObservable.Subscribe(observer);

                TestScheduler.Start();
                var receivedValue = observer.Values().First();
                receivedValue.Year.Should().Be(now.Year);
                receivedValue.Month.Should().Be(now.Month);
            }

            [Fact, LogIfTooSlow]
            public async Task InitializesTheMonthsPropertyToTheMonthsToShow()
            {
                var observer = TestScheduler.CreateObserver<IImmutableList<ReportsCalendarPageViewModel>>();
                var now = new DateTimeOffset(2020, 4, 2, 1, 1, 1, TimeSpan.Zero);
                TimeService.CurrentDateTime.Returns(now);
                TimeService.MidnightObservable.Returns(Observable.Never<DateTimeOffset>());

                await ViewModel.Initialize();
                ViewModel.MonthsObservable.Subscribe(observer);

                TestScheduler.Start();
                var months = observer.Values().First();
                months.Should().HaveCount(ReportsCalendarViewModel.MonthsToShow);
                var firstDateTime = now.AddMonths(-(ReportsCalendarViewModel.MonthsToShow - 1));
                var month = new CalendarMonth(
                    firstDateTime.Year, firstDateTime.Month);
                for (int i = 0; i < (ReportsCalendarViewModel.MonthsToShow - 1); i++, month = month.Next())
                {
                    months[i].CalendarMonth.Should().Be(month);
                }
            }

            [Fact, LogIfTooSlow]
            public async Task FillsQuickSelectShortcutlist()
            {
                var expectedShortCuts = new List<Type>
                {
                    typeof(ReportsCalendarTodayQuickSelectShortcut),
                    typeof(ReportsCalendarYesterdayQuickSelectShortcut),
                    typeof(ReportsCalendarThisWeekQuickSelectShortcut),
                    typeof(ReportsCalendarLastWeekQuickSelectShortcut),
                    typeof(ReportsCalendarThisMonthQuickSelectShortcut),
                    typeof(ReportsCalendarLastMonthQuickSelectShortcut),
                    typeof(ReportsCalendarThisYearQuickSelectShortcut),
                    typeof(ReportsCalendarLastYearQuickSelectShortcut)
                };
                var now = new DateTimeOffset(2020, 4, 2, 1, 1, 1, TimeSpan.Zero);
                TimeService.CurrentDateTime.Returns(now);

                await ViewModel.Initialize();

                for (int i = 0; i < ViewModel.QuickSelectShortcuts.Count; i++)
                    ViewModel.QuickSelectShortcuts[i].GetType().Should().Be(expectedShortCuts[i]);
            }

            [Fact, LogIfTooSlow]
            public async Task InitializesTheDateRangeWithTheCurrentWeek()
            {
                var now = new DateTimeOffset(2018, 7, 1, 1, 1, 1, TimeSpan.Zero);
                TimeService.CurrentDateTime.Returns(now);
                TimeService.MidnightObservable.Returns(Observable.Never<DateTimeOffset>());
                var user = Substitute.For<IThreadSafeUser>();
                user.BeginningOfWeek.Returns(BeginningOfWeek.Sunday);
                DataSource.User.Current.Returns(Observable.Return(user));

                var dateRangeObserver = TestScheduler.CreateObserver<ReportsDateRangeParameter>();
                var monthsObserver = TestScheduler.CreateObserver<IImmutableList<ReportsCalendarPageViewModel>>();
                ViewModel.SelectedDateRangeObservable.Subscribe(dateRangeObserver);
                await ViewModel.Initialize();
                ViewModel.MonthsObservable.Subscribe(monthsObserver);
                ViewModel.ViewAppeared();
                TestScheduler.Start();

                var months = monthsObserver.Values().First();
                dateRangeObserver.Messages.AssertEqual(
                    ReactiveTest.OnNext<ReportsDateRangeParameter>(0,
                        dateRange => ensureDateRangeIsCorrect(
                            dateRange,
                            months[ReportsCalendarViewModel.MonthsToShow - 1].Days[0],
                            months[ReportsCalendarViewModel.MonthsToShow - 1].Days[6]))
                );
            }
        }

        public sealed class TheCurrentMonthProperty : ReportsCalendarViewModelTest
        {
            [Theory, LogIfTooSlow]
            [InlineData(2017, 12, 24, 2017, 12)]
            [InlineData(2017, 5, 12, 2016, 5)]
            [InlineData(2017, 5, 24, 2017, 5)]
            [InlineData(2017, 5, 19, 2016, 12)]
            [InlineData(2017, 5, 20, 2017, 1)]
            [InlineData(2017, 12, 12, 2016, 12)]
            [InlineData(2017, 5, 0, 2015, 5)]
            [InlineData(2017, 5, 7, 2015, 12)]
            [InlineData(2017, 5, 8, 2016, 1)]
            public void RepresentsTheCurrentPage(
                int currentYear,
                int currentMonth,
                int currentPage,
                int expectedYear,
                int expectedMonth)
            {
                var observer = TestScheduler.CreateObserver<CalendarMonth>();
                var now = new DateTimeOffset(currentYear, currentMonth, 1, 0, 0, 0, TimeSpan.Zero);
                TimeService.CurrentDateTime.Returns(now);
                ViewModel.Initialize();
                ViewModel.Initialize().Wait();
                ViewModel.CurrentMonthObservable.Subscribe(observer);

                ViewModel.UpdateMonth(currentPage);

                TestScheduler.Start();
                var receivedValue = observer.Values().Last();
                receivedValue.Year.Should().Be(expectedYear);
                receivedValue.Month.Should().Be(expectedMonth);
            }
        }

        public sealed class TheCurrentPageProperty : ReportsCalendarViewModelTest
        {
            [Fact, LogIfTooSlow]
            public void IsInitializedToTheMonthsToShowConstantMinusOne()
            {
                var observer = TestScheduler.CreateObserver<int>();
                var now = new DateTimeOffset(2020, 4, 2, 1, 1, 1, TimeSpan.Zero);
                TimeService.CurrentDateTime.Returns(now);
                ViewModel.Initialize();
                ViewModel.Initialize().Wait();
                ViewModel.CurrentPageObservable.Subscribe(observer);

                TestScheduler.Start();

                var currentPage = observer.Values().First();
                currentPage.Should().Be(ReportsCalendarViewModel.MonthsToShow - 1);
            }
        }

        public sealed class TheRowsInCurrentMonthProperty : ReportsCalendarViewModelTest
        {
            [Theory, LogIfTooSlow]
            [InlineData(2017, 12, 24, BeginningOfWeek.Monday, 5)]
            [InlineData(2017, 12, 22, BeginningOfWeek.Monday, 6)]
            [InlineData(2017, 2, 24, BeginningOfWeek.Wednesday, 4)]
            public async Task ReturnsTheRowCountOfCurrentlyShownMonth(
                int currentYear,
                int currentMonth,
                int currentPage,
                BeginningOfWeek beginningOfWeek,
                int expectedRowCount)
            {
                var observer = TestScheduler.CreateObserver<int>();
                var now = new DateTimeOffset(currentYear, currentMonth, 1, 0, 0, 0, TimeSpan.Zero);
                TimeService.MidnightObservable.Returns(Observable.Never<DateTimeOffset>());
                TimeService.CurrentDateTime.Returns(now);
                var user = Substitute.For<IThreadSafeUser>();
                user.BeginningOfWeek.Returns(beginningOfWeek);
                DataSource.User.Current.Returns(Observable.Return(user));

                ViewModel.Initialize();
                await ViewModel.Initialize();

                ViewModel.RowsInCurrentMonthObservable
                    .Subscribe(observer);

                ViewModel.SetCurrentPage(currentPage);
                TestScheduler.Start();

                var rowsInCurrentMonth = observer.Values().Last();
                rowsInCurrentMonth.Should().Be(expectedRowCount);
            }
        }

        public sealed class TheSelectedDateRangeObservableProperty : ReportsCalendarViewModelTest
        {
            [Theory, LogIfTooSlow]
            [InlineData(0, 0, 3, 3)]
            [InlineData(5, 23, 9, 0)]
            public void EmitsNewElementWheneverDateRangeIsChangedByTappingTwoCells(
                int startPageIndex,
                int startCellIndex,
                int endPageIndex,
                int endCellIndex)
            {
                var monthsObserver = TestScheduler.CreateObserver<IImmutableList<ReportsCalendarPageViewModel>>();
                var now = new DateTimeOffset(2017, 12, 19, 1, 2, 3, TimeSpan.Zero);
                var dateRangeObserver = TestScheduler.CreateObserver<ReportsDateRangeParameter>();
                TimeService.CurrentDateTime.Returns(now);
                TimeService.MidnightObservable.Returns(Observable.Never<DateTimeOffset>());
                var user = Substitute.For<IThreadSafeUser>();
                user.BeginningOfWeek.Returns(BeginningOfWeek.Sunday);
                DataSource.User.Current.Returns(Observable.Return(user));

                ViewModel.SelectedDateRangeObservable.Subscribe(dateRangeObserver);
                ViewModel.Initialize();
                ViewModel.Initialize().Wait();

                ViewModel.MonthsObservable.Subscribe(monthsObserver);
                TestScheduler.Start();
                var months = monthsObserver.Values().Last();

                var startMonth = months[startPageIndex];
                var firstTappedCellViewModel = startMonth.Days[startCellIndex];
                var endMonth = months[endPageIndex];
                var secondTappedCellViewModel = endMonth.Days[endCellIndex];

                ViewModel.SelectDay.Inputs.OnNext(firstTappedCellViewModel);
                TestScheduler.Start();
                ViewModel.SelectDay.Inputs.OnNext(secondTappedCellViewModel);
                TestScheduler.Start();

                var lastDateRange = dateRangeObserver.LastEmittedValue();

                Assert.True(ensureDateRangeIsCorrect(lastDateRange,
                        firstTappedCellViewModel,
                        secondTappedCellViewModel));
            }
        }

        private static bool ensureDateRangeIsCorrect(
            ReportsDateRangeParameter dateRange,
            ReportsCalendarDayViewModel expectedStart,
            ReportsCalendarDayViewModel expectedEnd)
            => dateRange.StartDate.Year == expectedStart.CalendarMonth.Year
               && dateRange.StartDate.Month == expectedStart.CalendarMonth.Month
               && dateRange.StartDate.Day == expectedStart.Day
               && dateRange.EndDate.Year == expectedEnd.CalendarMonth.Year
               && dateRange.EndDate.Month == expectedEnd.CalendarMonth.Month
               && dateRange.EndDate.Day == expectedEnd.Day;

        public abstract class TheCalendarDayTappedCommand : ReportsCalendarViewModelTest
        {
            private IImmutableList<ReportsCalendarPageViewModel> months;

            public TheCalendarDayTappedCommand()
            {
                var now = new DateTimeOffset(2017, 12, 19, 1, 2, 3, TimeSpan.Zero);
                TimeService.CurrentDateTime.Returns(now);
                TimeService.MidnightObservable.Returns(Observable.Never<DateTimeOffset>());
                var user = Substitute.For<IThreadSafeUser>();
                user.BeginningOfWeek.Returns(BeginningOfWeek.Sunday);
                DataSource.User.Current.Returns(Observable.Return(user));

                var monthsObservable = TestScheduler.CreateObserver<IImmutableList<ReportsCalendarPageViewModel>>();
                ViewModel.Initialize().Wait();
                ViewModel.MonthsObservable.Subscribe(monthsObservable);
                TestScheduler.Start();
                months = monthsObservable.Values().Last();
            }

            protected ReportsCalendarDayViewModel FindDayViewModel(int monthIndex, int dayIndex)
                => months[monthIndex].Days[dayIndex];

            public sealed class AfterTappingOneCell : TheCalendarDayTappedCommand
            {
                [Theory, LogIfTooSlow]
                [InlineData(5, 8)]
                public void MarksTheFirstTappedCellAsSelected(
                    int monthIndex, int dayIndex)
                {
                    var dateRangeObserver = TestScheduler.CreateObserver<ReportsDateRangeParameter>();
                    var dayViewModel = FindDayViewModel(monthIndex, dayIndex);
                    ViewModel.HighlightedDateRangeObservable.Subscribe(dateRangeObserver);

                    ViewModel.SelectDay.Inputs.OnNext(dayViewModel);
                    TestScheduler.Start();

                    var highLightDateRange = dateRangeObserver.Values().Last();
                    dayViewModel.IsSelected(highLightDateRange).Should().BeTrue();
                }

                [Theory, LogIfTooSlow]
                [InlineData(11, 0)]
                public void MarksTheFirstTappedCellAsStartOfSelection(
                    int monthIndex, int dayIndex)
                {
                    var dateRangeObserver = TestScheduler.CreateObserver<ReportsDateRangeParameter>();
                    var dayViewModel = FindDayViewModel(monthIndex, dayIndex);
                    ViewModel.HighlightedDateRangeObservable.Subscribe(dateRangeObserver);

                    ViewModel.SelectDay.Inputs.OnNext(dayViewModel);
                    TestScheduler.Start();

                    var highLightDateRange = dateRangeObserver.Values().Last();
                    dayViewModel.IsStartOfSelectedPeriod(highLightDateRange).Should().BeTrue();
                }

                [Theory, LogIfTooSlow]
                [InlineData(3, 20)]
                public void MarksTheFirstTappedCellAsEndOfSelection(
                    int monthIndex, int dayIndex)
                {
                    var dateRangeObserver = TestScheduler.CreateObserver<ReportsDateRangeParameter>();
                    var dayViewModel = FindDayViewModel(monthIndex, dayIndex);
                    ViewModel.HighlightedDateRangeObservable.Subscribe(dateRangeObserver);

                    ViewModel.SelectDay.Inputs.OnNext(dayViewModel);
                    TestScheduler.Start();

                    var highLightDateRange = dateRangeObserver.Values().Last();
                    dayViewModel.IsEndOfSelectedPeriod(highLightDateRange).Should().BeTrue();
                }
            }

            public sealed class AfterTappingTwoCells : TheCalendarDayTappedCommand
            {
                [Theory, LogIfTooSlow]
                [InlineData(0, 0, 5, 8)]
                public void MarksTheFirstTappedCellAsNotEndOfSelection(
                    int firstMonthIndex,
                    int firstDayindex,
                    int secondMonthIndex,
                    int secondDayIndex)
                {
                    var dateRangeObserver = TestScheduler.CreateObserver<ReportsDateRangeParameter>();
                    var firstDayViewModel = FindDayViewModel(firstMonthIndex, firstDayindex);
                    var secondDayViewModel = FindDayViewModel(secondMonthIndex, secondDayIndex);
                    ViewModel.HighlightedDateRangeObservable.Subscribe(dateRangeObserver);

                    ViewModel.SelectDay.Inputs.OnNext(firstDayViewModel);
                    TestScheduler.Start();
                    ViewModel.SelectDay.Inputs.OnNext(secondDayViewModel);
                    TestScheduler.Start();

                    var highLightDateRange = dateRangeObserver.Values().Last();
                    firstDayViewModel.IsEndOfSelectedPeriod(highLightDateRange).Should().BeFalse();
                }

                [Theory, LogIfTooSlow]
                [InlineData(1, 1, 9, 9)]
                public void MarksTheSecondTappedCellAsEndOfSelection(
                    int firstMonthIndex,
                    int firstDayindex,
                    int secondMonthIndex,
                    int secondDayIndex)
                {
                    var dateRangeObserver = TestScheduler.CreateObserver<ReportsDateRangeParameter>();
                    var firstDayViewModel = FindDayViewModel(firstMonthIndex, firstDayindex);
                    var secondDayViewModel = FindDayViewModel(secondMonthIndex, secondDayIndex);
                    ViewModel.HighlightedDateRangeObservable.Subscribe(dateRangeObserver);

                    ViewModel.SelectDay.Inputs.OnNext(firstDayViewModel);
                    TestScheduler.Start();
                    ViewModel.SelectDay.Inputs.OnNext(secondDayViewModel);
                    TestScheduler.Start();

                    var highLightDateRange = dateRangeObserver.Values().Last();
                    secondDayViewModel.IsEndOfSelectedPeriod(highLightDateRange).Should().BeTrue();
                }

                [Theory, LogIfTooSlow]
                [InlineData(1, 2, 3, 4)]
                public void MarksTheSecondTappedCellAsNotStartOfSelection(
                    int firstMonthIndex,
                    int firstDayindex,
                    int secondMonthIndex,
                    int secondDayIndex)
                {
                    var dateRangeObserver = TestScheduler.CreateObserver<ReportsDateRangeParameter>();
                    var firstDayViewModel = FindDayViewModel(firstMonthIndex, firstDayindex);
                    var secondDayViewModel = FindDayViewModel(secondMonthIndex, secondDayIndex);
                    ViewModel.HighlightedDateRangeObservable.Subscribe(dateRangeObserver);

                    ViewModel.SelectDay.Inputs.OnNext(firstDayViewModel);
                    TestScheduler.Start();
                    ViewModel.SelectDay.Inputs.OnNext(secondDayViewModel);
                    TestScheduler.Start();

                    var highLightDateRange = dateRangeObserver.Values().Last();
                    secondDayViewModel.IsStartOfSelectedPeriod(highLightDateRange).Should().BeFalse();
                }

                [Theory, LogIfTooSlow]
                [InlineData(2, 15, 7, 20)]
                public void MarksTheWholeIntervalAsSelected(
                    int firstMonthIndex,
                    int firstDayindex,
                    int secondMonthIndex,
                    int secondDayIndex)
                {
                    var dateRangeObserver = TestScheduler.CreateObserver<ReportsDateRangeParameter>();
                    var firstDayViewModel = FindDayViewModel(firstMonthIndex, firstDayindex);
                    var secondDayViewModel = FindDayViewModel(secondMonthIndex, secondDayIndex);
                    ViewModel.HighlightedDateRangeObservable.Subscribe(dateRangeObserver);

                    ViewModel.SelectDay.Inputs.OnNext(firstDayViewModel);
                    TestScheduler.Start();
                    ViewModel.SelectDay.Inputs.OnNext(secondDayViewModel);
                    TestScheduler.Start();

                    var highLightDateRange = dateRangeObserver.Values().Last();

                    for (int monthIndex = firstMonthIndex; monthIndex <= secondMonthIndex; monthIndex++)
                    {
                        var month = months[monthIndex];
                        var startIndex = monthIndex == firstMonthIndex
                            ? firstDayindex
                            : 0;
                        var endIndex = monthIndex == secondMonthIndex
                            ? secondDayIndex
                            : month.Days.Count - 1;
                        assertDaysInMonthSelected(month, startIndex, endIndex, highLightDateRange);
                    }
                }

                private void assertDaysInMonthSelected(ReportsCalendarPageViewModel calendarPage, int startindex, int endIndex, ReportsDateRangeParameter highLightDateRange)
                {
                    for (int i = startindex; i <= endIndex; i++)
                        calendarPage.Days[i].IsSelected(highLightDateRange).Should().BeTrue();
                }
            }
        }

        public sealed class TheQuickSelectCommand : ReportsCalendarViewModelTest
        {
            [Property]
            public void UsingAnyOfTheShortcutsDoesNotThrowAnyTimeOfTheYear(DateTimeOffset now)
            {
                var shortcutsObserver = TestScheduler.CreateObserver<IImmutableList<ReportsCalendarBaseQuickSelectShortcut>>();
                TimeService.CurrentDateTime.Returns(now);
                // in this property test it is not possible to use the default ViewModel,
                // because we have to reset it in each iteration of the test
                var viewModel = CreateViewModel();
                viewModel.Initialize().Wait();
                viewModel.QuickSelectShortcutsObservable.Subscribe(shortcutsObserver);
                TestScheduler.Start();
                var shortcuts = shortcutsObserver.Values().Last();
                var errorsObserver = TestScheduler.CreateObserver<Exception>();

                foreach (var shortcut in shortcuts)
                {
                    viewModel.SelectShortcut.Inputs.OnNext(shortcut);
                    TestScheduler.Start();
                }

                errorsObserver.Messages.Should().BeEmpty();
            }

            [Property]
            public void SelectingAnyDateRangeDoesNotMakeTheAppCrash(DateTimeOffset a, DateTimeOffset b, DateTimeOffset c)
            {
                var dates = new[] { a, b, c };
                Array.Sort(dates);
                var start = dates[0];
                var now = dates[1];
                var end = dates[2];
                TimeService.CurrentDateTime.Returns(now);
                var selectedRange = ReportsDateRangeParameter.WithDates(start, end).WithSource(ReportsSource.Calendar);
                var customShortcut = new CustomShortcut(selectedRange, TimeService);
                var errorObserver = TestScheduler.CreateObserver<Exception>();
                var executionObserver = TestScheduler.CreateObserver<bool>();

                // in this property test it is not possible to use the default ViewModel,
                // because we have to reset it in each iteration of the test
                var viewModel = CreateViewModel();
                viewModel.Initialize().Wait();
                viewModel.SelectShortcut.Errors.Subscribe(errorObserver);
                viewModel.SelectShortcut.Executing.Subscribe(executionObserver);

                viewModel.SelectShortcut.Inputs.OnNext(customShortcut);
                TestScheduler.Start();

                errorObserver.Messages.Should().BeEmpty();
                executionObserver.Messages.Should().NotBeEmpty();
            }

            private sealed class CustomShortcut : ReportsCalendarBaseQuickSelectShortcut
            {
                private ReportsDateRangeParameter range;

                public CustomShortcut(ReportsDateRangeParameter range, ITimeService timeService) : base(timeService, "", ReportPeriod.Unknown)
                {
                    this.range = range;
                }

                public override ReportsDateRangeParameter GetDateRange()
                    => range;
            }
        }
    }
}
