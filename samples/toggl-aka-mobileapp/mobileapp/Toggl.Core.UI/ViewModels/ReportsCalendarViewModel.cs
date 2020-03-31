using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using Toggl.Core.Analytics;
using Toggl.Core.DataSources;
using Toggl.Core.Models;
using Toggl.Core.Services;
using Toggl.Core.UI.Navigation;
using Toggl.Core.UI.Parameters;
using Toggl.Core.UI.ViewModels.ReportsCalendar;
using Toggl.Core.UI.ViewModels.ReportsCalendar.QuickSelectShortcuts;
using Toggl.Shared;
using Toggl.Shared.Extensions;

namespace Toggl.Core.UI.ViewModels
{
    [Preserve(AllMembers = true)]
    public sealed class ReportsCalendarViewModel : ViewModel
    {
        public const int MonthsToShow = 25;

        private readonly string[] dayHeaders =
        {
            Resources.SundayInitial,
            Resources.MondayInitial,
            Resources.TuesdayInitial,
            Resources.WednesdayInitial,
            Resources.ThursdayInitial,
            Resources.FridayInitial,
            Resources.SaturdayInitial
        };
        private readonly ITimeService timeService;
        private readonly ITogglDataSource dataSource;
        private readonly ISchedulerProvider schedulerProvider;
        private readonly ISubject<Unit> reloadSubject = new Subject<Unit>();
        private readonly ISubject<ReportsDateRangeParameter> selectedDateRangeSubject = new Subject<ReportsDateRangeParameter>();
        private readonly ISubject<ReportsDateRangeParameter> highlightedDateRangeSubject = new BehaviorSubject<ReportsDateRangeParameter>(default(ReportsDateRangeParameter));
        private IObservable<BeginningOfWeek> beginningOfWeekObservable;

        private bool isInitialized;
        private bool viewAppearedOnce;
        private CalendarMonth initialMonth;
        private ReportsCalendarDayViewModel startOfSelection;
        private ReportPeriod reportPeriod = ReportPeriod.ThisWeek;
        private BeginningOfWeek beginningOfWeek;

        public IObservable<int> RowsInCurrentMonthObservable { get; private set; }

        public IObservable<CalendarMonth> CurrentMonthObservable { get; private set; }

        private readonly BehaviorSubject<int> currentPageSubject = new BehaviorSubject<int>(MonthsToShow - 1);

        public int CurrentPage => currentPageSubject.Value;

        private readonly ISubject<int> monthSubject = new Subject<int>();

        public IObservable<int> CurrentPageObservable { get; }

        public IObservable<Unit> ReloadObservable { get; private set; }

        public IObservable<ReportsDateRangeParameter> SelectedDateRangeObservable { get; }

        public IObservable<ReportsDateRangeParameter> HighlightedDateRangeObservable { get; }

        public IImmutableList<ReportsCalendarBaseQuickSelectShortcut> QuickSelectShortcuts { get; private set; }

        public IObservable<IImmutableList<ReportsCalendarBaseQuickSelectShortcut>> QuickSelectShortcutsObservable { get; private set; }

        public IObservable<IImmutableList<ReportsCalendarPageViewModel>> MonthsObservable { get; private set; }

        public IObservable<IReadOnlyList<string>> DayHeadersObservable { get; private set; }

        public InputAction<ReportsCalendarDayViewModel> SelectDay { get; }

        public InputAction<ReportsCalendarBaseQuickSelectShortcut> SelectShortcut { get; }

        public ReportsCalendarViewModel(
            ITimeService timeService,
            ITogglDataSource dataSource,
            IRxActionFactory rxActionFactory,
            INavigationService navigationService,
            ISchedulerProvider schedulerProvider)
            : base(navigationService)
        {
            Ensure.Argument.IsNotNull(dataSource, nameof(dataSource));
            Ensure.Argument.IsNotNull(timeService, nameof(timeService));
            Ensure.Argument.IsNotNull(rxActionFactory, nameof(rxActionFactory));
            Ensure.Argument.IsNotNull(navigationService, nameof(navigationService));
            Ensure.Argument.IsNotNull(schedulerProvider, nameof(schedulerProvider));

            this.dataSource = dataSource;
            this.timeService = timeService;
            this.schedulerProvider = schedulerProvider;

            SelectDay = rxActionFactory.FromAsync<ReportsCalendarDayViewModel>(calendarDayTapped);
            SelectShortcut = rxActionFactory.FromAction<ReportsCalendarBaseQuickSelectShortcut>(quickSelect);

            CurrentPageObservable = currentPageSubject
                .AsObservable()
                .DistinctUntilChanged();

            SelectedDateRangeObservable = selectedDateRangeSubject
                .ShareReplay(1);

            HighlightedDateRangeObservable = highlightedDateRangeSubject
                .ShareReplay(1);
        }

        public void SetCurrentPage(int newPage)
        {
            currentPageSubject.OnNext(newPage);
        }

        public void UpdateMonth(int newPage)
        {
            monthSubject.OnNext(newPage);
        }

        public override async Task Initialize()
        {
            await base.Initialize();

            var now = timeService.CurrentDateTime;
            initialMonth = new CalendarMonth(now.Year, now.Month).AddMonths(-(MonthsToShow - 1));

            beginningOfWeekObservable = dataSource.User.Current
                .Select(user => user.BeginningOfWeek)
                .DistinctUntilChanged();

            DayHeadersObservable = beginningOfWeekObservable
                .Select(mapDayHeaders)
                .AsDriver(new List<string>().AsReadOnly(), schedulerProvider);

            ReloadObservable = reloadSubject.AsObservable();

            MonthsObservable = beginningOfWeekObservable.CombineLatest(
                timeService.MidnightObservable.StartWith(timeService.CurrentDateTime),
                (beginningOfWeek, today) =>
                {
                    var monthIterator = new CalendarMonth(today.Year, today.Month).AddMonths(-(MonthsToShow - 1));
                    var months = new List<ReportsCalendarPageViewModel>();
                    for (int i = 0; i < MonthsToShow; i++, monthIterator = monthIterator.Next())
                        months.Add(new ReportsCalendarPageViewModel(monthIterator, beginningOfWeek, today));
                    return months.ToImmutableList();
                })
                .AsDriver(ImmutableList<ReportsCalendarPageViewModel>.Empty, schedulerProvider);

            RowsInCurrentMonthObservable = MonthsObservable.CombineLatest(
                CurrentPageObservable.DistinctUntilChanged(),
                (months, page) => months[page].RowCount)
                .Select(CommonFunctions.Identity)
                .AsDriver(0, schedulerProvider);

            CurrentMonthObservable = monthSubject
                .AsObservable()
                .StartWith(MonthsToShow - 1)
                .Select(convertPageIndexToCalendarMonth)
                .Share()
                .AsDriver(default, schedulerProvider);

            QuickSelectShortcutsObservable = beginningOfWeekObservable
                .Select(createQuickSelectShortcuts)
                .AsDriver(ImmutableList<ReportsCalendarBaseQuickSelectShortcut>.Empty, schedulerProvider);

            beginningOfWeek = (await dataSource.User.Current.FirstAsync()).BeginningOfWeek;

            QuickSelectShortcuts = createQuickSelectShortcuts(beginningOfWeek);

            SelectPeriod(reportPeriod);

            isInitialized = true;
            viewAppearedOnce = false;
        }

        public override void ViewAppeared()
        {
            base.ViewAppeared();

            if (viewAppearedOnce)
                return;

            viewAppearedOnce = true;
            SelectInitialShortcut();
        }

        public void Reload()
        {
            reloadSubject.OnNext(Unit.Default);
        }

        public void SelectInitialShortcut()
        {
            var initialShortcut = QuickSelectShortcuts.Single(shortcut => shortcut.Period == reportPeriod);
            selectedDateRangeSubject.OnNext(initialShortcut.GetDateRange().WithSource(ReportsSource.Initial));
            highlightedDateRangeSubject.OnNext(initialShortcut.GetDateRange().WithSource(ReportsSource.Initial));
        }

        public void SelectPeriod(ReportPeriod period)
        {
            reportPeriod = period;

            if (isInitialized)
            {
                var initialShortcut = QuickSelectShortcuts.Single(shortcut => shortcut.Period == period);
                changeDateRange(initialShortcut.GetDateRange().WithSource(ReportsSource.Initial));
            }
        }

        private async Task calendarDayTapped(ReportsCalendarDayViewModel tappedDay)
        {
            if (startOfSelection == null)
            {
                var date = tappedDay.DateTimeOffset;

                var dateRange = ReportsDateRangeParameter
                    .WithDates(date, date)
                    .WithSource(ReportsSource.Calendar);
                startOfSelection = tappedDay;
                highlightDateRange(dateRange);
            }
            else
            {
                var startDate = startOfSelection.DateTimeOffset;
                var endDate = tappedDay.DateTimeOffset;

                if (System.Math.Abs((endDate - startDate).Days) > 365)
                {
                    await View.Alert(
                        Resources.ReportTooLongTitle,
                        Resources.ReportTooLongDescription,
                        Resources.Ok
                    );
                }
                else
                {
                    var dateRange = ReportsDateRangeParameter
                        .WithDates(startDate, endDate)
                        .WithSource(ReportsSource.Calendar);
                    startOfSelection = null;
                    changeDateRange(dateRange);
                }
            }
        }

        private IReadOnlyList<string> mapDayHeaders(BeginningOfWeek newBeginningOfWeek)
        {
            var updatedDayHeaders = new List<string>();
            for (var i = 0; i < dayHeaders.Length; i++)
            {
                updatedDayHeaders.Add(dayHeaderFor(i, newBeginningOfWeek));
            }

            return updatedDayHeaders.AsReadOnly();
        }

        private string dayHeaderFor(int index, BeginningOfWeek newBeginningOfWeek)
            => dayHeaders[(index + (int)newBeginningOfWeek + 7) % 7];

        public void SelectStartOfSelectionIfNeeded()
        {
            if (startOfSelection == null) return;

            var date = startOfSelection.DateTimeOffset;
            var dateRange = ReportsDateRangeParameter
                .WithDates(date, date)
                .WithSource(ReportsSource.Calendar);
            changeDateRange(dateRange);
        }

        private IImmutableList<ReportsCalendarBaseQuickSelectShortcut> createQuickSelectShortcuts(BeginningOfWeek beginningOfWeek)
            => ImmutableList.Create<ReportsCalendarBaseQuickSelectShortcut>(
                new ReportsCalendarTodayQuickSelectShortcut(timeService),
                new ReportsCalendarYesterdayQuickSelectShortcut(timeService),
                new ReportsCalendarThisWeekQuickSelectShortcut(timeService, beginningOfWeek),
                new ReportsCalendarLastWeekQuickSelectShortcut(timeService, beginningOfWeek),
                new ReportsCalendarThisMonthQuickSelectShortcut(timeService),
                new ReportsCalendarLastMonthQuickSelectShortcut(timeService),
                new ReportsCalendarThisYearQuickSelectShortcut(timeService),
                new ReportsCalendarLastYearQuickSelectShortcut(timeService)
            );

        private CalendarMonth convertPageIndexToCalendarMonth(int pageIndex)
            => initialMonth.AddMonths(pageIndex);

        private void changeDateRange(ReportsDateRangeParameter newDateRange)
        {
            startOfSelection = null;

            highlightDateRange(newDateRange);

            selectedDateRangeSubject.OnNext(newDateRange);
        }

        private void quickSelect(ReportsCalendarBaseQuickSelectShortcut quickSelectShortCut)
        {
            changeDateRange(quickSelectShortCut.GetDateRange());
        }

        private void highlightDateRange(ReportsDateRangeParameter dateRange)
        {
            highlightedDateRangeSubject.OnNext(dateRange);
        }
    }
}
