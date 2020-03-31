using FluentAssertions;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Toggl.Core.Analytics;
using Toggl.Core.Exceptions;
using Toggl.Core.Interactors;
using Toggl.Core.Services;
using Toggl.Core.Tests.TestExtensions;
using Toggl.Core.UI.Navigation;
using Toggl.Core.UI.ViewModels.Calendar;
using Toggl.Core.UI.ViewModels.Selectable;
using Toggl.Shared;
using Toggl.Shared.Extensions;
using Toggl.Storage.Settings;
using Xunit;
using static Toggl.Shared.Extensions.FunctionalExtensions;

namespace Toggl.Core.Tests.UI.ViewModels
{
    public sealed class SelectUserCalendarsViewModelBaseTests
    {
        public sealed class MockSelectUserCalendarsViewModel : SelectUserCalendarsViewModelBase
        {
            public MockSelectUserCalendarsViewModel(
                IUserPreferences userPreferences,
                IInteractorFactory interactorFactory,
                IOnboardingStorage onboardingStorage,
                IAnalyticsService analyticsService,
                INavigationService navigationService,
                IRxActionFactory rxActionFactory)
                : base(userPreferences, interactorFactory, onboardingStorage, analyticsService, navigationService, rxActionFactory)
            {
            }
        }

        public abstract class SelectUserCalendarsViewModelBaseTest : BaseViewModelTests<MockSelectUserCalendarsViewModel, bool, string[]>
        {
            protected SelectUserCalendarsViewModelBaseTest()
            {
                UserPreferences.EnabledCalendarIds().Returns(new List<string>());
            }

            protected override MockSelectUserCalendarsViewModel CreateViewModel()
                => new MockSelectUserCalendarsViewModel(UserPreferences, InteractorFactory, OnboardingStorage, AnalyticsService, NavigationService, RxActionFactory);
        }

        public sealed class TheConstructor : SelectUserCalendarsViewModelBaseTest
        {
            [Fact, LogIfTooSlow]
            public async Task FillsTheCalendarList()
            {
                var userCalendarsObservable = Enumerable
                    .Range(0, 9)
                    .Select(id => new UserCalendar(
                        id.ToString(),
                        $"Calendar #{id}",
                        $"Source #{id % 3}",
                        false))
                    .Apply(Observable.Return);
                InteractorFactory.GetUserCalendars().Execute().Returns(userCalendarsObservable);

                var viewModel = new MockSelectUserCalendarsViewModel(UserPreferences, InteractorFactory, OnboardingStorage, AnalyticsService, NavigationService, RxActionFactory);

                await viewModel.Initialize(false);
                var calendars = await viewModel.Calendars.FirstAsync();

                calendars.Should().HaveCount(3);
                calendars.ForEach(group => group.Items.Should().HaveCount(3));
            }
        }

        public sealed class TheInitializeMethod : SelectUserCalendarsViewModelBaseTest
        {
            [Fact, LogIfTooSlow]
            public async Task HandlesNotAuthorizedException()
            {
                InteractorFactory
                    .GetUserCalendars()
                    .Execute()
                    .Returns(Observable.Throw<IEnumerable<UserCalendar>>(new NotAuthorizedException("")));

                await ViewModel.Initialize(false);
                var calendars = await ViewModel.Calendars.FirstAsync();

                calendars.Should().HaveCount(0);
            }

            [Fact, LogIfTooSlow]
            public async Task MarksAllCalendarsAsNotSelected()
            {
                var userCalendarsObservable = Enumerable
                    .Range(0, 9)
                    .Select(id => new UserCalendar(
                        id.ToString(),
                        $"Calendar #{id}",
                        $"Source #{id % 3}",
                        false))
                    .Apply(Observable.Return);
                InteractorFactory.GetUserCalendars().Execute().Returns(userCalendarsObservable);

                await ViewModel.Initialize(false);
                var calendars = await ViewModel.Calendars.FirstAsync();

                foreach (var calendarGroup in calendars)
                {
                    calendarGroup.Items.None(calendar => calendar.Selected).Should().BeTrue();
                }
            }
        }

        public sealed class TheCloseWithDefaultResultMethod : SelectUserCalendarsViewModelBaseTest
        {
            [Fact, LogIfTooSlow]
            public async Task ClosesTheViewModelAndReturnsTheInitialCalendarIds()
            {
                var initialSelectedIds = new List<string> { "0", "1", "2", "3" };
                UserPreferences.EnabledCalendarIds().Returns(initialSelectedIds);

                var userCalendars = Enumerable
                    .Range(0, 9)
                    .Select(id => new UserCalendar(
                        id.ToString(),
                        $"Calendar #{id}",
                        $"Source #{id % 3}",
                        false));

                InteractorFactory
                    .GetUserCalendars()
                    .Execute()
                    .Returns(Observable.Return(userCalendars));
                await ViewModel.Initialize(false);
                var selectedIds = new[] { "0", "2", "4", "7" };

                var calendars = userCalendars
                    .Where(calendar => selectedIds.Contains(calendar.Id))
                    .Select(calendar => new SelectableUserCalendarViewModel(calendar, false));

                ViewModel.SelectCalendar.ExecuteSequentally(calendars).Subscribe();
                ViewModel.CloseWithDefaultResult();

                TestScheduler.Start();

                (await ViewModel.Result).Should().BeSequenceEquivalentTo(initialSelectedIds);
            }
        }

        public sealed class TheDoneAction : SelectUserCalendarsViewModelBaseTest
        {
            [Fact, LogIfTooSlow]
            public async Task ClosesTheViewModelAndReturnsSelectedCalendarIds()
            {
                var userCalendars = Enumerable
                    .Range(0, 9)
                    .Select(id => new UserCalendar(
                        id.ToString(),
                        $"Calendar #{id}",
                        $"Source #{id % 3}",
                        false));

                InteractorFactory
                    .GetUserCalendars()
                    .Execute()
                    .Returns(Observable.Return(userCalendars));

                await ViewModel.Initialize(false);
                var selectedIds = new[] { "0", "2", "4", "7" };

                var calendars = userCalendars
                    .Where(calendar => selectedIds.Contains(calendar.Id))
                    .Select(calendar => new SelectableUserCalendarViewModel(calendar, false));

                ViewModel.SelectCalendar.ExecuteSequentally(calendars)
                    .PrependAction(ViewModel.Save)
                    .Subscribe();

                TestScheduler.Start();

                (await ViewModel.Result).Should().BeSequenceEquivalentTo(selectedIds);
            }
        }

        public abstract class TheDoneActionEnabledProperty
        {
            public class WhenYouDoNotForceItemSelection : SelectUserCalendarsViewModelBaseTest
            {
                public WhenYouDoNotForceItemSelection()
                {
                    ViewModel.Initialize(false).Wait();
                }

                [Fact, LogIfTooSlow]
                public void ReturnsTrue()
                {
                    var observer = Substitute.For<IObserver<bool>>();

                    ViewModel.Save.Enabled.Subscribe(observer);
                    SchedulerProvider.TestScheduler.AdvanceBy(1);

                    observer.Received().OnNext(true);
                }
            }

            public class WhenYouForceItemSelection : SelectUserCalendarsViewModelBaseTest
            {
                public WhenYouForceItemSelection()
                {
                    ViewModel.Initialize(true).Wait();
                }

                [Fact, LogIfTooSlow]
                public void StartsWithFalse()
                {
                    var observer = Substitute.For<IObserver<bool>>();

                    ViewModel.Save.Enabled.Subscribe(observer);
                    SchedulerProvider.TestScheduler.AdvanceBy(1);

                    observer.Received().OnNext(false);
                }

                [Fact, LogIfTooSlow]
                public async Task EmitsTrueAfterOneCalendarHasBeenSelected()
                {
                    var observer = Substitute.For<IObserver<bool>>();
                    ViewModel.Save.Enabled.Subscribe(observer);
                    var selectableUserCalendar = new SelectableUserCalendarViewModel(
                        new UserCalendar(),
                        false
                    );

                    ViewModel.SelectCalendar.Execute(selectableUserCalendar);
                    TestScheduler.Start();

                    Received.InOrder(() =>
                    {
                        observer.OnNext(false);
                        observer.OnNext(true);
                    });
                }

                [Fact, LogIfTooSlow]
                public void DoesNotEmitAnythingWhenSelectingAdditionalCalendars()
                {
                    var observer = Substitute.For<IObserver<bool>>();
                    ViewModel.Save.Enabled.Subscribe(observer);
                    var selectedableUserCalendars = Enumerable
                        .Range(0, 10)
                        .Select(id =>
                        {
                            var userCalendar = new UserCalendar(id.ToString(), id.ToString(), "Doenst matter");
                            return new SelectableUserCalendarViewModel(userCalendar, false);
                        });

                    var auxObserver = TestScheduler.CreateObserver<Unit>();
                    ViewModel.SelectCalendar.ExecuteSequentally(selectedableUserCalendars)
                        .Subscribe(auxObserver);

                    TestScheduler.Start();

                    Received.InOrder(() =>
                    {
                        observer.OnNext(false);
                        observer.OnNext(true);
                    });
                }

                [Fact, LogIfTooSlow]
                public void EmitsFalseAfterAllTheCalendarsHaveBeenDeselected()
                {
                    var observer = Substitute.For<IObserver<bool>>();
                    ViewModel.Save.Enabled.Subscribe(observer);

                    var userCalendars = Enumerable
                        .Range(0, 3)
                        .Select(id => new UserCalendar(id.ToString(), id.ToString(), "Doesn't matter"));

                    var selectedableUserCalendars = userCalendars
                        .Select(userCalendar => new SelectableUserCalendarViewModel(userCalendar, false));

                    InteractorFactory
                        .GetUserCalendars()
                        .Execute()
                        .Returns(Observable.Return(userCalendars));


                    var auxObserver = TestScheduler.CreateObserver<Unit>();
                    ViewModel.SelectCalendar.ExecuteSequentally(
                            selectedableUserCalendars
                                .Concat(selectedableUserCalendars)
                        )
                        .Subscribe(auxObserver);

                    TestScheduler.Start();

                    Received.InOrder(() =>
                    {
                        observer.OnNext(false);
                        observer.OnNext(true);
                        observer.OnNext(false);
                    });
                }
            }
        }

        public sealed class TheAnalytics : SelectUserCalendarsViewModelBaseTest
        {
            [Fact, LogIfTooSlow]
            public async Task TracksNumberOfLinkedCalendarsChanged()
            {
                var userCalendars = Enumerable
                    .Range(0, 9)
                    .Select(id => new UserCalendar(
                        id.ToString(),
                        $"Calendar #{id}",
                        $"Source #{id % 3}",
                        false));

                InteractorFactory
                    .GetUserCalendars()
                    .Execute()
                    .Returns(Observable.Return(userCalendars));

                await ViewModel.Initialize(false);
                var selectedIds = new[] { "0", "2", "4", "7" };

                var calendars = userCalendars
                    .Where(calendar => selectedIds.Contains(calendar.Id))
                    .Select(calendar => new SelectableUserCalendarViewModel(calendar, false));

                ViewModel.SelectCalendar.ExecuteSequentally(calendars)
                    .PrependAction(ViewModel.Save)
                    .Subscribe();

                TestScheduler.Start();

                AnalyticsService.NumberOfLinkedCalendarsChanged.Received().Track(4);
                AnalyticsService.NumberOfLinkedCalendarsNewUser.DidNotReceiveWithAnyArgs().Track(4);
            }

            [Fact, LogIfTooSlow]
            public async Task TracksNumberOfLinkedCalendarsNewUser()
            {
                var initialSelectedIds = new List<string> { };
                UserPreferences.EnabledCalendarIds().Returns(initialSelectedIds);
                PermissionsChecker.CalendarPermissionGranted.Returns(Observable.Return(true));
                OnboardingStorage.IsFirstTimeConnectingCalendars().Returns(true);

                var userCalendars = Enumerable
                    .Range(0, 9)
                    .Select(id => new UserCalendar(
                        id.ToString(),
                        $"Calendar #{id}",
                        $"Source #{id % 3}",
                        false));

                InteractorFactory
                    .GetUserCalendars()
                    .Execute()
                    .Returns(Observable.Return(userCalendars));

                await ViewModel.Initialize(false);
                var selectedIds = new[] { "2", "4", "7" };

                var calendars = userCalendars
                    .Where(calendar => selectedIds.Contains(calendar.Id))
                    .Select(calendar => new SelectableUserCalendarViewModel(calendar, false));

                ViewModel.SelectCalendar.ExecuteSequentally(calendars)
                    .PrependAction(ViewModel.Save)
                    .Subscribe();

                TestScheduler.Start();

                ViewModel.ViewDisappeared();

                AnalyticsService.NumberOfLinkedCalendarsNewUser.Received().Track(3);
                AnalyticsService.NumberOfLinkedCalendarsChanged.DidNotReceiveWithAnyArgs().Track(3);
            }
        }

        public sealed class TheSelectCalendarAction : SelectUserCalendarsViewModelBaseTest
        {
            [Fact, LogIfTooSlow]
            public async Task MarksTheCalendarAsSelectedIfItIsNotSelected()
            {
                var userCalendars = Enumerable
                    .Range(0, 9)
                    .Select(id => new UserCalendar(
                        id.ToString(),
                        $"Calendar #{id}",
                        $"Source #{id % 3}",
                        false));
                var userCalendarsObservable = Observable.Return(userCalendars);
                InteractorFactory.GetUserCalendars().Execute().Returns(userCalendarsObservable);
                await ViewModel.Initialize(false);
                var viewModelCalendars = await ViewModel.Calendars.FirstAsync();
                var calendarToBeSelected = viewModelCalendars.First().Items.First();

                ViewModel.SelectCalendar.Execute(calendarToBeSelected);
                TestScheduler.Start();

                calendarToBeSelected.Selected.Should().BeTrue();
            }

            [Fact, LogIfTooSlow]
            public async Task MarksTheCalendarAsNotSelectedIfItIsSelected()
            {
                var userCalendars = Enumerable
                    .Range(0, 9)
                    .Select(id => new UserCalendar(
                        id.ToString(),
                        $"Calendar #{id}",
                        $"Source #{id % 3}",
                        true));
                var userCalendarsObservable = Observable.Return(userCalendars);
                InteractorFactory.GetUserCalendars().Execute().Returns(userCalendarsObservable);
                await ViewModel.Initialize(false);
                var viewModelCalendars = await ViewModel.Calendars.FirstAsync();
                var calendarToBeSelected = viewModelCalendars.First().Items.First();

                ViewModel.SelectCalendar.Execute(calendarToBeSelected); //Select the calendar
                TestScheduler.Start();
                ViewModel.SelectCalendar.Execute(calendarToBeSelected); //Deselect the calendar
                TestScheduler.Start();

                calendarToBeSelected.Selected.Should().BeFalse();
            }
        }
    }
}
