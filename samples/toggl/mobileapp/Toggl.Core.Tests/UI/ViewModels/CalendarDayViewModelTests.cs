using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using FluentAssertions;
using NSubstitute;
using Toggl.Core.Analytics;
using Toggl.Core.Calendar;
using Toggl.Core.DataSources;
using Toggl.Core.DTOs;
using Toggl.Core.Interactors;
using Toggl.Core.Models;
using Toggl.Core.Models.Interfaces;
using Toggl.Core.Tests.Generators;
using Toggl.Core.Tests.Mocks;
using Toggl.Core.Tests.TestExtensions;
using Toggl.Core.UI.Navigation;
using Toggl.Core.UI.Transformations;
using Toggl.Core.UI.ViewModels;
using Toggl.Core.UI.ViewModels.Calendar;
using Toggl.Core.UI.Views;
using Toggl.Shared;
using Xunit;
using Task = System.Threading.Tasks.Task;

namespace Toggl.Core.Tests.UI.ViewModels
{
    public sealed class CalendarDayViewModelTests
    {
        public abstract class CalendarDayViewModelTest : BaseViewModelTests<CalendarDayViewModel>
        {
            protected const long TimeEntryId = 10;
            protected const long DefaultWorkspaceId = 1;
            protected IInteractor<IObservable<IEnumerable<CalendarItem>>> CalendarInteractor { get; }

            protected static DateTimeOffset Now { get; } = new DateTimeOffset(2018, 8, 10, 12, 0, 0, TimeSpan.Zero);

            protected CalendarDayViewModelTest()
            {
                CalendarInteractor = Substitute.For<IInteractor<IObservable<IEnumerable<CalendarItem>>>>();

                var workspace = new MockWorkspace { Id = DefaultWorkspaceId };
                var timeEntry = new MockTimeEntry { Id = TimeEntryId };

                TimeService.CurrentDateTime.Returns(Now);

                InteractorFactory
                    .GetCalendarItemsForDate(Arg.Any<DateTime>())
                    .Returns(CalendarInteractor);

                InteractorFactory
                    .GetDefaultWorkspace()
                    .Execute()
                    .Returns(Observable.Return(workspace));

                InteractorFactory
                    .CreateTimeEntry(Arg.Any<ITimeEntryPrototype>(), TimeEntryStartOrigin.CalendarEvent)
                    .Execute()
                    .ReturnsTaskOf<IThreadSafeTimeEntry>(timeEntry);

                InteractorFactory
                    .CreateTimeEntry(Arg.Any<ITimeEntryPrototype>(), TimeEntryStartOrigin.CalendarTapAndDrag)
                    .Execute()
                    .ReturnsTaskOf<IThreadSafeTimeEntry>(timeEntry);

                InteractorFactory
                    .UpdateTimeEntry(Arg.Any<EditTimeEntryDto>())
                    .Execute()
                    .ReturnsTaskOf<IThreadSafeTimeEntry>(timeEntry);
            }

            protected override CalendarDayViewModel CreateViewModel()
                => new CalendarDayViewModel(
                    new DateTimeOffset(2020, 1, 2, 3, 4, 5, TimeSpan.Zero),
                    TimeService,
                    DataSource,
                    RxActionFactory,
                    UserPreferences,
                    AnalyticsService,
                    BackgroundService,
                    InteractorFactory,
                    SchedulerProvider,
                    NavigationService);
        }

        public sealed class TheConstructor : CalendarDayViewModelTest
        {
            [Theory, LogIfTooSlow]
            [ConstructorData]
            public void ThrowsIfAnyOfTheArgumentsIsNull(
                bool useTimeService,
                bool useDataSource,
                bool useRxActionFactory,
                bool useUserPreferences,
                bool useAnalyticsService,
                bool useBackgroundService,
                bool useInteractorFactory,
                bool useSchedulerProvider,
                bool useNavigationService)
            {
                Action tryingToConstructWithEmptyParameters =
                    () => new CalendarDayViewModel(
                        default(DateTimeOffset),
                        useTimeService ? TimeService : null,
                        useDataSource ? DataSource : null,
                        useRxActionFactory ? RxActionFactory : null,
                        useUserPreferences ? UserPreferences : null,
                        useAnalyticsService ? AnalyticsService : null,
                        useBackgroundService ? BackgroundService : null,
                        useInteractorFactory ? InteractorFactory : null,
                        useSchedulerProvider ? SchedulerProvider : null,
                        useNavigationService ? NavigationService : null);

                tryingToConstructWithEmptyParameters.Should().Throw<ArgumentNullException>();
            }
        }

        public sealed class TheCalendarItemsProperty : CalendarDayViewModelTest
        {
            [Fact, LogIfTooSlow]
            public async Task ReturnsTheCalendarItemsForToday()
            {
                var now = new DateTimeOffset(2018, 8, 9, 12, 0, 0, TimeSpan.Zero);
                TimeService.CurrentDateTime.Returns(now);

                var items = new List<CalendarItem>
                {
                    new CalendarItem("id", CalendarItemSource.Calendar, now.AddMinutes(30), TimeSpan.FromMinutes(15), "Weekly meeting", CalendarIconKind.Event, "#ff0000"),
                    new CalendarItem("id", CalendarItemSource.TimeEntry, now.AddHours(-3), TimeSpan.FromMinutes(30), "Bug fixes", CalendarIconKind.None, "#00ff00"),
                    new CalendarItem("id", CalendarItemSource.Calendar, now.AddHours(2), TimeSpan.FromMinutes(30), "F**** timesheets", CalendarIconKind.Event, "#ff0000")
                };
                var interactor = Substitute.For<IInteractor<IObservable<IEnumerable<CalendarItem>>>>();
                interactor.Execute().Returns(Observable.Return(items));
                InteractorFactory.GetCalendarItemsForDate(Arg.Any<DateTime>()).Returns(interactor);

                await ViewModel.Initialize();

                TestScheduler.Start();
                ViewModel.CalendarItems[0].Should().BeEquivalentTo(items);
            }

            [Fact, LogIfTooSlow]
            public async Task RefetchesWheneverATimeEntryIsAdded()
            {
                var midnightSubject = new Subject<DateTimeOffset>();
                var createdSubject = new Subject<IThreadSafeTimeEntry>();
                TimeService.MidnightObservable.Returns(midnightSubject);
                await ViewModel.Initialize();
                CalendarInteractor.ClearReceivedCalls();
                TestScheduler.Start();

                createdSubject.OnNext(new MockTimeEntry());

                TestScheduler.Start();
                await CalendarInteractor.Received().Execute();
            }

            [Fact, LogIfTooSlow]
            public async Task RefetchesWheneverATimeEntryIsUpdated()
            {
                var midnightSubject = new Subject<DateTimeOffset>();
                var updatedSubject = new Subject<EntityUpdate<IThreadSafeTimeEntry>>();
                TimeService.MidnightObservable.Returns(midnightSubject);
                await ViewModel.Initialize();
                CalendarInteractor.ClearReceivedCalls();
                TestScheduler.Start();

                updatedSubject.OnNext(new EntityUpdate<IThreadSafeTimeEntry>(0, new MockTimeEntry()));

                TestScheduler.Start();
                await CalendarInteractor.Received().Execute();
            }

            [Fact, LogIfTooSlow]
            public async Task RefetchesWheneverATimeEntryIsDeleted()
            {
                var deletedSubject = new Subject<long>();
                var midnightSubject = new Subject<DateTimeOffset>();
                TimeService.MidnightObservable.Returns(midnightSubject);
                await ViewModel.Initialize();
                CalendarInteractor.ClearReceivedCalls();
                TestScheduler.Start();

                deletedSubject.OnNext(0);

                TestScheduler.Start();
                await CalendarInteractor.Received().Execute();
            }

            [Fact, LogIfTooSlow]
            public async Task RefetchesWheneverTheDayChanges()
            {
                var midnightSubject = new Subject<DateTimeOffset>();
                TimeService.MidnightObservable.Returns(midnightSubject);
                await ViewModel.Initialize();
                CalendarInteractor.ClearReceivedCalls();
                TestScheduler.Start();

                midnightSubject.OnNext(DateTimeOffset.Now);

                TestScheduler.Start();
                await CalendarInteractor.Received().Execute();
            }

            [Fact, LogIfTooSlow]
            public async Task RefetchesWheneverTheSelectedCalendarsChange()
            {
                var calendarSubject = new Subject<List<string>>();
                var midnightSubject = new Subject<DateTimeOffset>();
                TimeService.MidnightObservable.Returns(midnightSubject);
                UserPreferences.EnabledCalendars.Returns(calendarSubject);
                await ViewModel.Initialize();
                CalendarInteractor.ClearReceivedCalls();
                TestScheduler.Start();

                calendarSubject.OnNext(new List<string>());

                TestScheduler.Start();
                await CalendarInteractor.Received().Execute();
            }
        }

        public sealed class TheOnDurationSelectedAction : CalendarDayViewModelTest
        {
            [Fact, LogIfTooSlow]
            public void CreatesATimeEntryWithTheSelectedStartDate()
            {
                var now = DateTimeOffset.UtcNow;
                var duration = TimeSpan.FromMinutes(30);
                var tuple = (now, duration);

                ViewModel.OnDurationSelected.Execute(tuple);
                TestScheduler.Start();

                InteractorFactory
                    .CreateTimeEntry(Arg.Is<ITimeEntryPrototype>(p => p.StartTime == now), TimeEntryStartOrigin.CalendarTapAndDrag)
                    .Received()
                    .Execute();
            }

            [Fact, LogIfTooSlow]
            public async Task CreatesATimeEntryWithTheSelectedDuration()
            {
                var now = DateTimeOffset.UtcNow;
                var duration = TimeSpan.FromMinutes(30);
                var tuple = (now, duration);

                ViewModel.OnDurationSelected.Execute(tuple);
                TestScheduler.Start();

                await InteractorFactory
                    .CreateTimeEntry(Arg.Is<ITimeEntryPrototype>(p => p.Duration == duration), TimeEntryStartOrigin.CalendarTapAndDrag)
                    .Received()
                    .Execute();
            }

            [Fact, LogIfTooSlow]
            public async Task CreatesATimeEntryInTheDefaultWorkspace()
            {
                var now = DateTimeOffset.UtcNow;
                var duration = TimeSpan.FromMinutes(30);
                var tuple = (now, duration);

                ViewModel.OnDurationSelected.Execute(tuple);
                TestScheduler.Start();

                await InteractorFactory
                    .CreateTimeEntry(Arg.Is<ITimeEntryPrototype>(p => p.WorkspaceId == DefaultWorkspaceId), TimeEntryStartOrigin.CalendarTapAndDrag)
                    .Received()
                    .Execute();
            }
        }

        public sealed class TheEditTimeEntryAction : CalendarDayViewModelTest
        {
            private CalendarItem calendarItem = new CalendarItem(
                "id",
                CalendarItemSource.TimeEntry,
                new DateTimeOffset(2018, 8, 20, 10, 0, 0, TimeSpan.Zero),
                new TimeSpan(45),
                "This is a time entry",
                CalendarIconKind.None,
                color: "#ff0000",
                timeEntryId: TimeEntryId,
                calendarId: "abcd-1234-abcd-1234");

            [Fact, LogIfTooSlow]
            public async Task UpdatesATimeEntry()
            {
                ViewModel.OnTimeEntryEdited.Execute(calendarItem);

                await InteractorFactory
                    .UpdateTimeEntry(Arg.Is<DTOs.EditTimeEntryDto>(dto =>
                        dto.Id == TimeEntryId
                        && dto.StartTime == calendarItem.StartTime
                        && dto.StopTime == calendarItem.EndTime))
                    .Received()
                    .Execute();
            }

            [Fact, LogIfTooSlow]
            public async Task TracksTheTimeEntryEditedFromCalendarEventWhenDurationChanges()
            {
                var timeEntry = new MockTimeEntry
                {
                    Start = calendarItem.StartTime,
                    Duration = (long)calendarItem.Duration.Value.TotalSeconds + 10
                };
                InteractorFactory.GetTimeEntryById(Arg.Any<long>())
                    .Execute()
                    .Returns(Observable.Return(timeEntry));

                ViewModel.OnTimeEntryEdited.Execute(calendarItem);

                AnalyticsService.TimeEntryChangedFromCalendar.Received().Track(CalendarChangeEvent.Duration);
            }

            [Fact, LogIfTooSlow]
            public async Task TracksTheTimeEntryEditedFromCalendarEventWhenStartTimeChanges()
            {
                var timeEntry = new MockTimeEntry
                {
                    Start = calendarItem.StartTime.Add(TimeSpan.FromHours(1)),
                    Duration = (long)calendarItem.Duration.Value.TotalSeconds
                };
                InteractorFactory.GetTimeEntryById(Arg.Any<long>())
                    .Execute()
                    .Returns(Observable.Return(timeEntry));

                ViewModel.OnTimeEntryEdited.Execute(calendarItem);

                AnalyticsService.TimeEntryChangedFromCalendar.Received().Track(CalendarChangeEvent.StartTime);
            }

            [Fact, LogIfTooSlow]
            public async Task TracksTheTimeEntryEditedFromCalendarEventWhenBothStartTimeAndDurationChange()
            {
                var timeEntry = new MockTimeEntry
                {
                    Start = calendarItem.StartTime.Add(TimeSpan.FromHours(1)),
                    Duration = (long)calendarItem.Duration.Value.TotalSeconds + 10
                };
                InteractorFactory.GetTimeEntryById(Arg.Any<long>())
                    .Execute()
                    .Returns(Observable.Return(timeEntry));

                ViewModel.OnTimeEntryEdited.Execute(calendarItem);

                AnalyticsService.TimeEntryChangedFromCalendar.Received().Track(CalendarChangeEvent.Duration);
                AnalyticsService.TimeEntryChangedFromCalendar.Received().Track(CalendarChangeEvent.StartTime);
            }
        }
        
        public sealed class TheTimeTrackedOnDayObservable : CalendarDayViewModelTest
        {
            [Fact, LogIfTooSlow]
            public async Task ContainsTheSumOfDurationsInTheTimeEntryCalendarItems()
            {
                var now = new DateTimeOffset(2018, 8, 9, 12, 0, 0, TimeSpan.Zero);
                TimeService.CurrentDateTime.Returns(now);

                var items = new List<CalendarItem>
                {
                    new CalendarItem("id", CalendarItemSource.TimeEntry, now.AddMinutes(30), TimeSpan.FromMinutes(30), "Weekly meeting", CalendarIconKind.Event, "#ff0000"),
                    new CalendarItem("id", CalendarItemSource.TimeEntry, now.AddHours(-3), TimeSpan.FromMinutes(30), "Bug fixes", CalendarIconKind.None, "#00ff00"),
                    new CalendarItem("id", CalendarItemSource.TimeEntry, now.AddHours(2), TimeSpan.FromMinutes(30), "F**** timesheets", CalendarIconKind.Event, "#ff0000"),
                    new CalendarItem("id", CalendarItemSource.Calendar, now.AddHours(2), TimeSpan.FromMinutes(30), "F**** timesheets", CalendarIconKind.Event, "#ff0000")
                };
                var interactor = Substitute.For<IInteractor<IObservable<IEnumerable<CalendarItem>>>>();
                interactor.Execute().Returns(Observable.Return(items));
                InteractorFactory.GetCalendarItemsForDate(Arg.Any<DateTime>()).Returns(interactor);
                var preferences = Substitute.For<IThreadSafePreferences>();
                preferences.DurationFormat.Returns(DurationFormat.Classic);
                this.DataSource.Preferences.Current.Returns(Observable.Return(preferences));
                var timeOnDayObserver = TestScheduler.CreateObserver<string>();
                ViewModel.TimeTrackedOnDay.Subscribe(timeOnDayObserver);
                
                await ViewModel.Initialize();

                TestScheduler.Start();

                timeOnDayObserver.LastEmittedValue().Should().Be(DurationAndFormatToString.Convert(TimeSpan.FromMinutes(90), DurationFormat.Classic));
            }
            
            [Fact, LogIfTooSlow]
            public async Task ItsTheSameAsTheTimeEntriesTodayIfTheCalendarDayInFactToday()
            {
                var now = new DateTimeOffset(2018, 8, 9, 0, 0, 0, DateTimeOffset.Now.Offset);
                TimeService.CurrentDateTime.Returns(now);
                var yesterday = now.AddDays(-1);
                var yesterdayItems = new List<CalendarItem>
                {
                    new CalendarItem("id", CalendarItemSource.TimeEntry, yesterday.AddMinutes(30), TimeSpan.FromMinutes(30), "Weekly meeting", CalendarIconKind.Event, "#ff0000"),
                    new CalendarItem("id", CalendarItemSource.TimeEntry, yesterday.AddHours(3), TimeSpan.FromMinutes(30), "Bug fixes", CalendarIconKind.None, "#00ff00"),
                    new CalendarItem("id", CalendarItemSource.TimeEntry, yesterday.AddHours(2), TimeSpan.FromMinutes(30), "F**** timesheets", CalendarIconKind.Event, "#ff0000"),
                    new CalendarItem("id", CalendarItemSource.Calendar, yesterday.AddHours(2), TimeSpan.FromMinutes(30), "F**** timesheets", CalendarIconKind.Event, "#ff0000")
                };
                var calendarItemsInteractor = Substitute.For<IInteractor<IObservable<IEnumerable<CalendarItem>>>>();
                calendarItemsInteractor.Execute().Returns(Observable.Return(yesterdayItems));
                InteractorFactory.GetCalendarItemsForDate(Arg.Any<DateTime>()).Returns(calendarItemsInteractor);
                InteractorFactory.ObserveTimeTrackedToday().Execute().Returns(Observable.Return(TimeSpan.FromMinutes(10)));
                var viewModel = new CalendarDayViewModel(
                    now.Date,
                    TimeService,
                    DataSource,
                    RxActionFactory,
                    UserPreferences,
                    AnalyticsService,
                    BackgroundService,
                    InteractorFactory,
                    SchedulerProvider,
                    NavigationService);
                var preferences = Substitute.For<IThreadSafePreferences>();
                preferences.DurationFormat.Returns(DurationFormat.Classic);
                this.DataSource.Preferences.Current.Returns(Observable.Return(preferences));
                var timeOnDayObserver = TestScheduler.CreateObserver<string>();
                viewModel.TimeTrackedOnDay.Subscribe(timeOnDayObserver);

                await viewModel.Initialize();

                TestScheduler.Start();
                timeOnDayObserver.LastEmittedValue().Should().Be(DurationAndFormatToString.Convert(TimeSpan.FromMinutes(10), DurationFormat.Classic));
            }
        }
    }
}
