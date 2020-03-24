using FluentAssertions;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using Microsoft.Reactive.Testing;
using Toggl.Core.Calendar;
using Toggl.Core.Interactors;
using Toggl.Core.Models.Interfaces;
using Toggl.Core.Tests.Generators;
using Toggl.Core.Tests.TestExtensions;
using Toggl.Core.UI.Navigation;
using Toggl.Core.UI.ViewModels;
using Toggl.Core.UI.ViewModels.Calendar;
using Toggl.Shared;
using Toggl.Shared.Extensions;
using Xunit;

namespace Toggl.Core.Tests.UI.ViewModels
{
    public sealed class CalendarViewModelTests
    {
        public abstract class CalendarViewModelTest : BaseViewModelTests<CalendarViewModel>
        {
            protected override CalendarViewModel CreateViewModel()
                => new CalendarViewModel(
                    DataSource,
                    TimeService,
                    RxActionFactory,
                    UserPreferences,
                    AnalyticsService,
                    BackgroundService,
                    InteractorFactory,
                    SchedulerProvider,
                    NavigationService
                );


            public static IEnumerable<object[]> BeginningOfWeekTestData
                => new[]
                {
                    new object[] { BeginningOfWeek.Monday },
                    new object[] { BeginningOfWeek.Tuesday },
                    new object[] { BeginningOfWeek.Wednesday },
                    new object[] { BeginningOfWeek.Thursday },
                    new object[] { BeginningOfWeek.Friday },
                    new object[] { BeginningOfWeek.Saturday },
                    new object[] { BeginningOfWeek.Sunday },
                };

            protected IThreadSafeUser UserWith(BeginningOfWeek beginningOfWeek)
            {
                var user = Substitute.For<IThreadSafeUser>();
                user.BeginningOfWeek.Returns(beginningOfWeek);
                return user;
            }

            protected void SetupBeginningOfWeek(BeginningOfWeek beginningOfWeek)
            {
                var user = UserWith(beginningOfWeek);
                DataSource.User.Current.Returns(Observable.Return(user));
            }
        }

        public sealed class TheConstructor : CalendarViewModelTest
        {
            [Theory, LogIfTooSlow]
            [ConstructorData]
            public void ThrowsIfAnyOfTheArgumentsIsNull(
                bool useDataSource,
                bool useTimeService,
                bool useUserPreferences,
                bool useAnalyticsService,
                bool useBackgroundService,
                bool useInteractorFactory,
                bool useSchedulerProvider,
                bool useNavigationService,
                bool useRxActionFactory)
            {
                var dataSource = useDataSource ? DataSource : null;
                var timeService = useTimeService ? TimeService : null;
                var userPreferences = useUserPreferences ? UserPreferences : null;
                var rxActionFactory = useRxActionFactory ? RxActionFactory : null;
                var analyticsService = useAnalyticsService ? AnalyticsService : null;
                var backgroundService = useBackgroundService ? BackgroundService : null;
                var interactorFactory = useInteractorFactory ? InteractorFactory : null;
                var schedulerProvider = useSchedulerProvider ? SchedulerProvider : null;
                var navigationService = useNavigationService ? NavigationService : null;

                Action tryingToConstructWithEmptyParameters =
                    () => new CalendarViewModel(
                        dataSource,
                        timeService,
                        rxActionFactory,
                        userPreferences,
                        analyticsService,
                        backgroundService,
                        interactorFactory,
                        schedulerProvider,
                        navigationService);

                tryingToConstructWithEmptyParameters.Should().Throw<ArgumentNullException>();
            }
        }

        public sealed class TheCurrentlyShownDateStringObservable : CalendarViewModelTest
        {
            private static readonly DateTimeOffset now = new DateTimeOffset(2020, 1, 2, 3, 4, 5, TimeSpan.Zero).Date;

            public TheCurrentlyShownDateStringObservable()
            {
                TimeService.CurrentDateTime.Returns(now);
            }

            [Fact, LogIfTooSlow]
            public async Task StartsWithTheCurrentDate()
            {
                var preferences = Substitute.For<IThreadSafePreferences>();
                preferences.DateFormat.Returns(DateFormat.ValidDateFormats[0]);
                DataSource.Preferences.Current.Returns(Observable.Return(preferences));
                var expectedResult = "Thursday, Jan 2";
                var observer = TestScheduler.CreateObserver<string>();
                var viewModel = CreateViewModel();
                viewModel.CurrentlyShownDateString.Subscribe(observer);

                TestScheduler.Start();

                observer.Values().Should().BeEquivalentTo(new[] { expectedResult });
            }

            [Theory, LogIfTooSlow]
            [InlineData(-1, "Wednesday, Jan 1")]
            [InlineData(-2, "Tuesday, Dec 31")]
            [InlineData(-7, "Thursday, Dec 26")]
            [InlineData(-14, "Thursday, Dec 19")]
            public void EmitsNewDateWhenCurrentlyVisiblePageChanges(int pageIndex, string expectedDate)
            {
                var expectedResults = new[]
                {
                    "Thursday, Jan 2",
                    expectedDate
                };
                var observer = TestScheduler.CreateObserver<string>();
                var viewModel = CreateViewModel();
                var date = viewModel.IndexToDate(pageIndex);
                viewModel.CurrentlyShownDateString.Subscribe(observer);

                viewModel.CurrentlyShownDate.Accept(date);
                TestScheduler.Start();

                observer.Values().Should().BeEquivalentTo(expectedResults);
            }

            [Theory, LogIfTooSlow]
            [InlineData(4, 5, 2020, "Monday, May 4")]
            [InlineData(30, 9, 1995, "Saturday, Sep 30")]
            public void EmitsNewDateWhenAnItemInWeekViewIsTapped(int day, int month, int year, string expectedDateString)
            {
                var preferences = Substitute.For<IThreadSafePreferences>();
                preferences.DateFormat.Returns(DateFormat.ValidDateFormats[0]);
                DataSource.Preferences.Current.Returns(Observable.Return(preferences));
                var expectedResults = new[]
                {
                    "Thursday, Jan 2",
                    expectedDateString
                };
                var observer = TestScheduler.CreateObserver<string>();
                var viewModel = CreateViewModel();
                viewModel.CurrentlyShownDateString.Subscribe(observer);
                var date = new DateTime(year, month, day);
                var tappedWeekViewDayViewModel = new CalendarWeeklyViewDayViewModel(date, false, true);

                viewModel.SelectDayFromWeekView.Execute(tappedWeekViewDayViewModel);
                TestScheduler.Start();

                observer.Values().Should().BeEquivalentTo(expectedResults);
            }
        }

        public sealed class TheCurrentlyShownDateProperty : CalendarViewModelTest
        {
            private static readonly DateTimeOffset now = new DateTimeOffset(2020, 1, 2, 3, 4, 5, TimeSpan.Zero).ToLocalTime().Date;

            public TheCurrentlyShownDateProperty()
            {
                TimeService.CurrentDateTime.Returns(now);
            }

            [Theory, LogIfTooSlow]
            [InlineData(4, 5, 2020)]
            [InlineData(30, 9, 1995)]
            public void EmitsNewDateWhenAnItemInWeekViewIsSelected(int day, int month, int year)
            {
                var selectedDate = new DateTime(year, month, day);
                var selectedDayViewModel = new CalendarWeeklyViewDayViewModel(selectedDate, false, true);
                var preferences = Substitute.For<IThreadSafePreferences>();
                preferences.DateFormat.Returns(DateFormat.ValidDateFormats[0]);
                DataSource.Preferences.Current.Returns(Observable.Return(preferences));
                var expectedResults = new[]
                {
                    now.LocalDateTime,
                    selectedDate
                };
                var observer = TestScheduler.CreateObserver<DateTime>();
                var viewModel = CreateViewModel();
                viewModel.CurrentlyShownDate.Subscribe(observer);

                viewModel.SelectDayFromWeekView.Execute(selectedDayViewModel);
                TestScheduler.Start();

                observer.Values().Should().BeEquivalentTo(expectedResults);
            }

            [Fact, LogIfTooSlow]
            public void StartsWithTheCurrentDate()
            {
                var preferences = Substitute.For<IThreadSafePreferences>();
                preferences.DateFormat.Returns(DateFormat.ValidDateFormats[0]);
                DataSource.Preferences.Current.Returns(Observable.Return(preferences));
                var expectedResults = new[]
                {
                    now.Date
                };
                var observer = TestScheduler.CreateObserver<DateTime>();
                var viewModel = CreateViewModel();
                viewModel.CurrentlyShownDate.Subscribe(observer);

                TestScheduler.Start();

                observer.Values().Should().BeEquivalentTo(expectedResults);
            }
        }

        public sealed class TheWeekViewDaysProperty : CalendarViewModelTest
        {
            private DateTimeOffset now = new DateTimeOffset(2017, 4, 3, 1, 2, 3, TimeSpan.Zero).ToLocalTime();

            public TheWeekViewDaysProperty()
            {
                TimeService.CurrentDateTime.Returns(now);
                TimeService.MidnightObservable.Returns(Observable.Never<DateTimeOffset>());
            }

            [Theory, LogIfTooSlow]
            [MemberData(nameof(BeginningOfWeekTestData))]
            public void AlwaysContains14ViewableDays(BeginningOfWeek beginningOfWeek)
            {
                SetupBeginningOfWeek(beginningOfWeek);
                var observer = TestScheduler.CreateObserver<IImmutableList<CalendarWeeklyViewDayViewModel>>();
                var viewModel = CreateViewModel();
                viewModel.WeekViewDays.Subscribe(observer);

                TestScheduler.Start();

                observer.Values().Should().HaveCount(1);
                observer.Values().First().Where(day => day.Enabled).Should().HaveCount(14);
            }

            [Theory, LogIfTooSlow]
            [MemberData(nameof(BeginningOfWeekTestData))]
            public void StartsWithTheSelectedBeginningOfWeek(BeginningOfWeek beginningOfWeek)
            {
                SetupBeginningOfWeek(beginningOfWeek);
                var observer = TestScheduler.CreateObserver<IImmutableList<CalendarWeeklyViewDayViewModel>>();
                var viewModel = CreateViewModel();
                viewModel.WeekViewDays.Subscribe(observer);

                TestScheduler.Start();

                observer.Values().Should().HaveCount(1);
                assertCollectionStartsWithCorrectBeginningOfWeek(observer.Values().First(), beginningOfWeek);

            }

            [Theory, LogIfTooSlow]
            [InlineData(1)]
            [InlineData(2)]
            [InlineData(3)]
            [InlineData(4)]
            [InlineData(5)]
            [InlineData(6)]
            public void EmitsNewCollectionWhenBeginningOfWeekSettingChanges(int updateCount)
            {
                setupBeginningOfWeekUpdates();
                var observer = TestScheduler.CreateObserver<IImmutableList<CalendarWeeklyViewDayViewModel>>();
                var viewModel = CreateViewModel();
                viewModel.WeekViewDays.Subscribe(observer);

                TestScheduler.Start();

                var observedValues = observer.Values().ToArray();
                observedValues.Should().HaveCount(updateCount + 1);
                for (int i = 0; i <= updateCount; i++)
                {
                    var weekDays = observedValues[i];
                    var expectedBeginningOfWeek = (BeginningOfWeek)i;
                    weekDays.First().Should().Match<CalendarWeeklyViewDayViewModel>(
                        firstDay => firstDay.Date.DayOfWeek == expectedBeginningOfWeek.ToDayOfWeekEnum()
                    );
                }

                void setupBeginningOfWeekUpdates()
                {
                    var observableMessages = new List<Recorded<Notification<IThreadSafeUser>>>();
                    observableMessages.Add(OnNext(0, UserWith(BeginningOfWeek.Sunday)));
                    for (int i = 1; i <= updateCount; i++)
                    {

                        var beginningOfWeek = (BeginningOfWeek)i;
                        var user = UserWith(beginningOfWeek);
                        observableMessages.Add(OnNext(i, user));
                    }

                    var observable = TestScheduler.CreateColdObservable(observableMessages.ToArray());
                    DataSource.User.Current.Returns(observable);
                }
            }

            [Theory, LogIfTooSlow]
            [MemberData(nameof(BeginningOfWeekTestData))]
            public void ContainsOneDayThatIsMarkedAsToday(BeginningOfWeek beginningOfWeek)
            {
                SetupBeginningOfWeek(beginningOfWeek);
                var observer = TestScheduler.CreateObserver<IImmutableList<CalendarWeeklyViewDayViewModel>>();
                var viewModel = CreateViewModel();
                viewModel.WeekViewDays.Subscribe(observer);

                TestScheduler.Start();

                var today = observer.Values().First().Single(date => date.Date == now.Date);
                today.IsToday.Should().BeTrue();
            }

            [Fact, LogIfTooSlow]
            public void EmitsNewCollectionAfterMidnight()
            {
                var midnightSubject = new Subject<DateTimeOffset>();
                SetupBeginningOfWeek(BeginningOfWeek.Monday);
                TimeService.MidnightObservable.Returns(midnightSubject);
                var observer = TestScheduler.CreateObserver<IImmutableList<CalendarWeeklyViewDayViewModel>>();
                var viewModel = CreateViewModel();
                viewModel.WeekViewDays.Subscribe(observer);

                midnightSubject.OnNext(now.AddDays(1));
                TestScheduler.Start();

                observer.Values().Should().HaveCount(2);
            }

            [Theory, LogIfTooSlow]
            [MemberData(nameof(BeginningOfWeekTestData))]
            public void TheCountOfItemsIsDivisibleBy7(BeginningOfWeek beginningOfWeek)
            {
                SetupBeginningOfWeek(beginningOfWeek);
                var observer = TestScheduler.CreateObserver<IImmutableList<CalendarWeeklyViewDayViewModel>>();
                var viewModel = CreateViewModel();
                viewModel.WeekViewDays.Subscribe(observer);

                TestScheduler.Start();

                (observer.Values().Single().Count % 7).Should().Be(0);
            }

            [Theory, LogIfTooSlow]
            [MemberData(nameof(BeginningOfWeekTestData))]
            public void AllDaysAfterTodayAreMarkedAsUnviewable(BeginningOfWeek beginningOfWeek)
            {
                SetupBeginningOfWeek(beginningOfWeek);
                var observer = TestScheduler.CreateObserver<IImmutableList<CalendarWeeklyViewDayViewModel>>();
                var viewModel = CreateViewModel();
                viewModel.WeekViewDays.Subscribe(observer);

                TestScheduler.Start();

                var daysAfterToday = observer.Values().Single().Where(day => day.Date > now.Date);
                if (daysAfterToday.None()) return;
                daysAfterToday.Should().OnlyContain(day => !day.Enabled);
            }

            [Theory, LogIfTooSlow]
            [MemberData(nameof(BeginningOfWeekTestData))]
            public void AllDaysMoreThan2WeeksAgoAreMarkedAsUnviewable(BeginningOfWeek beginningOfWeek)
            {
                SetupBeginningOfWeek(beginningOfWeek);
                var observer = TestScheduler.CreateObserver<IImmutableList<CalendarWeeklyViewDayViewModel>>();
                var viewModel = CreateViewModel();
                viewModel.WeekViewDays.Subscribe(observer);

                TestScheduler.Start();

                var twoWeeksAgo = now.Date.AddDays(-14);
                var daysBeforeTwoWeeks = observer.Values().Single().Where(day => day.Date < twoWeeksAgo);
                if (daysBeforeTwoWeeks.None()) return;
                daysBeforeTwoWeeks.Should().OnlyContain(day => !day.Enabled);
            }

            private void assertCollectionStartsWithCorrectBeginningOfWeek(
                IImmutableList<CalendarWeeklyViewDayViewModel> collection,
                BeginningOfWeek beginningOfWeek)
            {
                collection.First().Should().Match<CalendarWeeklyViewDayViewModel>(
                    weekViewDay => weekViewDay.Date.DayOfWeek == beginningOfWeek.ToDayOfWeekEnum());
            }
        }

        public sealed class WeekViewHeadersProperty : CalendarViewModelTest
        {
            [Theory, LogIfTooSlow]
            [MemberData(nameof(BeginningOfWeekTestData))]
            public void ContainsAllDaysOfWeek(BeginningOfWeek beginningOfWeek)
            {
                SetupBeginningOfWeek(beginningOfWeek);
                var viewModel = CreateViewModel();
                var observer = TestScheduler.CreateObserver<IImmutableList<DayOfWeek>>();
                viewModel.WeekViewHeaders.Subscribe(observer);

                TestScheduler.Start();

                observer.Values().Single().Should().OnlyHaveUniqueItems().And.HaveCount(7);
            }

            [Theory, LogIfTooSlow]
            [MemberData(nameof(BeginningOfWeekTestData))]
            public void StartsWithTheBeginningOfWeekSelectedInSettings(BeginningOfWeek beginningOfWeek)
            {
                SetupBeginningOfWeek(beginningOfWeek);
                var viewModel = CreateViewModel();
                var observer = TestScheduler.CreateObserver<IImmutableList<DayOfWeek>>();
                viewModel.WeekViewHeaders.Subscribe(observer);

                TestScheduler.Start();

                observer.Values().Single().First().Should().Be(beginningOfWeek.ToDayOfWeekEnum());
            }

            [Theory, LogIfTooSlow]
            [InlineData(1)]
            [InlineData(2)]
            [InlineData(3)]
            [InlineData(4)]
            [InlineData(5)]
            [InlineData(6)]
            public void EmitsNewCollectionWhenBeginningOfWeekSettingChanges(int updateCount)
            {
                setupBeginningOfWeekUpdates();
                var observer = TestScheduler.CreateObserver<IImmutableList<DayOfWeek>>();
                var viewModel = CreateViewModel();
                viewModel.WeekViewHeaders.Subscribe(observer);

                TestScheduler.Start();

                var observedValues = observer.Values().ToArray();
                observedValues.Should().HaveCount(updateCount + 1);
                for (int i = 0; i <= updateCount; i++)
                {
                    var dayHeaders = observedValues[i];
                    var expectedBeginningOfWeek = (BeginningOfWeek)i;
                    dayHeaders.First().Should().Match<DayOfWeek>(
                        firstHeader => firstHeader == expectedBeginningOfWeek.ToDayOfWeekEnum()
                    );
                }

                void setupBeginningOfWeekUpdates()
                {
                    var observableMessages = new List<Recorded<Notification<IThreadSafeUser>>>();
                    observableMessages.Add(OnNext(0, UserWith(BeginningOfWeek.Sunday)));
                    for (int i = 1; i <= updateCount; i++)
                    {

                        var beginningOfWeek = (BeginningOfWeek)i;
                        var user = UserWith(beginningOfWeek);
                        observableMessages.Add(OnNext(i, user));
                    }

                    var observable = TestScheduler.CreateColdObservable(observableMessages.ToArray());
                    DataSource.User.Current.Returns(observable);
                }
            }
        }

        public sealed class TheSelectDayFromWeekViewAction : CalendarViewModelTest
        {
            private readonly DateTimeOffset now = new DateTimeOffset(2020, 4, 5, 9, 2, 1, TimeSpan.Zero);

            public TheSelectDayFromWeekViewAction()
            {
                TimeService.CurrentDateTime.Returns(now);
                TimeService.MidnightObservable.Returns(Observable.Never<DateTimeOffset>());
            }

            [Fact, LogIfTooSlow]
            public void NewCurrentlySelectedDateIsEmittedAfterExecutingTheAction()
            {
                var observer = TestScheduler.CreateObserver<DateTime>();
                ViewModel.CurrentlyShownDate.Subscribe(observer);
                var dayToSelect = new CalendarWeeklyViewDayViewModel(now.AddDays(-3).Date, false,  true);

                ViewModel.SelectDayFromWeekView.Execute(dayToSelect);
                TestScheduler.Start();

                observer.Values().Last().Should().Be(dayToSelect.Date);
            }

            [Fact, LogIfTooSlow]
            public void TracksCalendarWeeklyDatePickerSelectionChangedEvent()
            {
                var daysSinceToday = 3;
                var dayToSelect = new CalendarWeeklyViewDayViewModel(now.AddDays(-daysSinceToday).Date, false,  true);

                ViewModel.SelectDayFromWeekView.Execute(dayToSelect);
                TestScheduler.Start();

                AnalyticsService.CalendarWeeklyDatePickerSelectionChanged.Received().Track(daysSinceToday, dayToSelect.Date.DayOfWeek.ToString());
            }
        }

        public sealed class TheOpenSettingsAction : CalendarViewModelTest
        {
            [Fact, LogIfTooSlow]
            public void NavigatesToTheSettingsViewModel()
            {
                ViewModel.OpenSettings.Execute();

                NavigationService.Received().Navigate<SettingsViewModel>(View);
            }
        }
    }
}
