using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using Toggl.Core.Analytics;
using Toggl.Core.DataSources;
using Toggl.Core.Extensions;
using Toggl.Core.Interactors;
using Toggl.Core.Services;
using Toggl.Core.UI.Extensions;
using Toggl.Core.UI.Navigation;
using Toggl.Core.UI.Transformations;
using Toggl.Shared;
using Toggl.Shared.Extensions;
using Toggl.Shared.Extensions.Reactive;
using Toggl.Storage.Settings;

namespace Toggl.Core.UI.ViewModels.Calendar
{
    [Preserve(AllMembers = true)]
    public sealed class CalendarViewModel : ViewModel
    {
        private const int availableDayCount = 14;
        private const string dateFormat = "dddd, MMM d";

        private readonly ITimeService timeService;
        private readonly ITogglDataSource dataSource;
        private readonly IUserPreferences userPreferences;
        private readonly IAnalyticsService analyticsService;
        private readonly IBackgroundService backgroundService;
        private readonly IInteractorFactory interactorFactory;
        private readonly ISchedulerProvider schedulerProvider;
        private readonly IRxActionFactory rxActionFactory;

        private readonly ISubject<Unit> realoadWeekView = new Subject<Unit>();

        public IObservable<string> CurrentlyShownDateString { get; }

        public BehaviorRelay<DateTime> CurrentlyShownDate { get; }

        public IObservable<ImmutableList<CalendarWeeklyViewDayViewModel>> WeekViewDays { get; }

        public IObservable<ImmutableList<DayOfWeek>> WeekViewHeaders { get; }

        public ViewAction OpenSettings { get; }

        public InputAction<CalendarWeeklyViewDayViewModel> SelectDayFromWeekView { get; }

        public CalendarViewModel(
            ITogglDataSource dataSource,
            ITimeService timeService,
            IRxActionFactory rxActionFactory,
            IUserPreferences userPreferences,
            IAnalyticsService analyticsService,
            IBackgroundService backgroundService,
            IInteractorFactory interactorFactory,
            ISchedulerProvider schedulerProvider,
            INavigationService navigationService)
            : base(navigationService)
        {
            Ensure.Argument.IsNotNull(dataSource, nameof(dataSource));
            Ensure.Argument.IsNotNull(timeService, nameof(timeService));
            Ensure.Argument.IsNotNull(userPreferences, nameof(userPreferences));
            Ensure.Argument.IsNotNull(rxActionFactory, nameof(rxActionFactory));
            Ensure.Argument.IsNotNull(analyticsService, nameof(analyticsService));
            Ensure.Argument.IsNotNull(backgroundService, nameof(backgroundService));
            Ensure.Argument.IsNotNull(interactorFactory, nameof(interactorFactory));
            Ensure.Argument.IsNotNull(schedulerProvider, nameof(schedulerProvider));

            this.dataSource = dataSource;
            this.timeService = timeService;
            this.rxActionFactory = rxActionFactory;
            this.userPreferences = userPreferences;
            this.analyticsService = analyticsService;
            this.backgroundService = backgroundService;
            this.interactorFactory = interactorFactory;
            this.schedulerProvider = schedulerProvider;

            OpenSettings = rxActionFactory.FromAsync(openSettings);
            SelectDayFromWeekView = rxActionFactory.FromAction<CalendarWeeklyViewDayViewModel>(selectDayFromWeekView);

            var beginningOfWeekObservable = dataSource.User.Current.Select(user => user.BeginningOfWeek);

            WeekViewDays = beginningOfWeekObservable
                .ReemitWhen(timeService.MidnightObservable.SelectUnit())
                .Select(weekViewDays)
                .Select(days => days.ToImmutableList())
                .AsDriver(schedulerProvider);

            WeekViewHeaders = beginningOfWeekObservable
                .ReemitWhen(realoadWeekView)
                .Select(weekViewHeaders)
                .Select(dayOfWeekHeaders => dayOfWeekHeaders.ToImmutableList())
                .AsDriver(schedulerProvider);

            CurrentlyShownDate = new BehaviorRelay<DateTime>(timeService.CurrentDateTime.ToLocalTime().Date);

            CurrentlyShownDateString = CurrentlyShownDate.AsObservable()
                .DistinctUntilChanged()
                .Select(date => DateTimeToFormattedString.Convert(date, dateFormat))
                .AsDriver(schedulerProvider);
        }

        public void RealoadWeekView()
            => realoadWeekView.OnNext(Unit.Default);

        private IEnumerable<DayOfWeek> weekViewHeaders(BeginningOfWeek beginningOfWeek)
        {
            var currentDay = beginningOfWeek.ToDayOfWeekEnum();
            for (int i = 0; i < 7; i++)
            {
                yield return currentDay;
                currentDay = currentDay.NextEnumValue();
            }
        }

        private IEnumerable<CalendarWeeklyViewDayViewModel> weekViewDays(BeginningOfWeek beginningOfWeek)
        {
            var now = timeService.CurrentDateTime.ToLocalTime();
            var today = now.Date;
            var firstAvailableDate = now.AddDays(-availableDayCount + 1).Date;
            var firstShownDate = firstAvailableDate.BeginningOfWeek(beginningOfWeek).Date;
            var lastShownDate = now.BeginningOfWeek(beginningOfWeek).AddDays(7).Date;

            var currentDate = firstShownDate;
            while (currentDate != lastShownDate)
            {
                var dateIsViewable = currentDate <= today && currentDate >= firstAvailableDate;
                yield return new CalendarWeeklyViewDayViewModel(currentDate, currentDate == today, dateIsViewable);
                currentDate = currentDate.AddDays(1);
            }
        }

        public CalendarDayViewModel DayViewModelAt(int index)
        {
            var currentDate = timeService.CurrentDateTime.ToLocalTime().Date;
            var date = currentDate.AddDays(index);
            return new CalendarDayViewModel(
                date,
                timeService,
                dataSource,
                rxActionFactory,
                userPreferences,
                analyticsService,
                backgroundService,
                interactorFactory,
                schedulerProvider,
                NavigationService);
        }

        public DateTime IndexToDate(int index)
        {
            var today = timeService.CurrentDateTime.ToLocalTime().Date;
            return today.AddDays(index);
        }

        private Task openSettings()
            => Navigate<SettingsViewModel>();

        private void selectDayFromWeekView(CalendarWeeklyViewDayViewModel day)
        {
            CurrentlyShownDate.Accept(day.Date);

            var daysSinceToday = (timeService.CurrentDateTime.ToLocalTime().Date - day.Date).Days;
            var dayOfWeek = day.Date.DayOfWeek.ToString();
            analyticsService.CalendarWeeklyDatePickerSelectionChanged.Track(daysSinceToday, dayOfWeek);
        }
    }
}
