using System;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using NSubstitute;
using Toggl.Core.Analytics;
using Toggl.Core.Calendar;
using Toggl.Core.DTOs;
using Toggl.Core.Models;
using Toggl.Core.Models.Interfaces;
using Toggl.Core.Tests.Generators;
using Toggl.Core.Tests.Mocks;
using Toggl.Core.Tests.TestExtensions;
using Toggl.Core.UI.Parameters;
using Toggl.Core.UI.ViewModels;
using Toggl.Core.UI.ViewModels.Calendar.ContextualMenu;
using Toggl.Core.UI.Views;
using Toggl.Shared;
using Xunit;
using ColorHelper = Toggl.Core.Helper.Colors;
using Task = System.Threading.Tasks.Task;

namespace Toggl.Core.Tests.UI.ViewModels
{
    public sealed class CalendarContextualMenuViewModelTests
    {
        public abstract class CalendarContextualMenuViewModelTest : BaseViewModelTests<CalendarContextualMenuViewModel>
        {
            protected override CalendarContextualMenuViewModel CreateViewModel()
                => new CalendarContextualMenuViewModel(InteractorFactory, SchedulerProvider, AnalyticsService, RxActionFactory, TimeService, NavigationService);
            
            protected CalendarItem CreateEmptyCalendarItem()
                => new CalendarItem();

            protected CalendarItem CreateDummyTimeEntryCalendarItem(bool isRunning = false, long timeEntryId = 1, DateTimeOffset? startTime = null, TimeSpan? duration = null)
            {
                TimeSpan? nullTimespan = null;
                return new CalendarItem("1",
                    CalendarItemSource.TimeEntry,
                    startTime ?? DateTimeOffset.Now,
                    isRunning ? nullTimespan : (duration ?? TimeSpan.FromMinutes(30)),
                    "",
                    CalendarIconKind.None,
                    timeEntryId: timeEntryId);
            }

            protected CalendarItem CreateDummyCalendarEventCalendarItem(
                string description = "",
                DateTimeOffset? startTime = null,
                TimeSpan? duration = null)
                => new CalendarItem("Id",
                    CalendarItemSource.Calendar,
                    startTime ?? DateTimeOffset.Now,
                    duration,
                    description,
                    CalendarIconKind.Event,
                    calendarId: "Id");
        }

        public sealed class TheConstructor : CalendarContextualMenuViewModelTest
        {
            [Theory, LogIfTooSlow]
            [ConstructorData]
            public void ThrowsIfAnyOfTheArgumentsIsNull(
                bool useInteractorFactory,
                bool useSchedulerProvider,
                bool useAnalyticsService,
                bool useRxActionFactory,
                bool useTimeService,
                bool useNavigationService)
            {
                Action tryingToConstructWithEmptyParameters =
                    () => new CalendarContextualMenuViewModel(
                        useInteractorFactory ? InteractorFactory : null,
                        useSchedulerProvider ? SchedulerProvider : null,
                        useAnalyticsService ? AnalyticsService : null,
                        useRxActionFactory ? RxActionFactory : null,
                        useTimeService ? TimeService : null,
                        useNavigationService ? NavigationService : null
                    );

                tryingToConstructWithEmptyParameters.Should().Throw<ArgumentNullException>();
            }
        }

        public sealed class TheCurrentMenuObservable : CalendarContextualMenuViewModelTest
        {
            [Fact]
            public void ShouldStartEmpty()
            {
                var menuObserver = TestScheduler.CreateObserver<CalendarContextualMenu>();
                var visibilityObserver = TestScheduler.CreateObserver<bool>();
                ViewModel.CurrentMenu.Subscribe(menuObserver);
                ViewModel.MenuVisible.Subscribe(visibilityObserver);

                TestScheduler.Start();

                menuObserver.Messages.Should().HaveCount(1);
                menuObserver.Messages.First().Value.Value.Actions.Should().BeEmpty();
                visibilityObserver.Messages.Should().HaveCount(1);
                visibilityObserver.Messages.First().Value.Value.Should().BeFalse();
            }

            [Fact]
            public void EmitsDiscardEditSaveActionsWhenNewCalendarItemIsBeingEdited()
            {
                var observer = TestScheduler.CreateObserver<CalendarContextualMenu>();
                var calendarItem = CreateEmptyCalendarItem();
                ViewModel.CurrentMenu.Subscribe(observer);

                ViewModel.OnCalendarItemUpdated.Inputs.OnNext(calendarItem);
                TestScheduler.Start();

                observer.Messages.Should().HaveCount(2);

                var kinds = observer.Messages[1].Value.Value.Actions.Select(action => action.ActionKind);
                kinds.Should().ContainInOrder(
                    CalendarMenuActionKind.Discard,
                    CalendarMenuActionKind.Edit,
                    CalendarMenuActionKind.Save);
            }

            [Fact]
            public void EmitsDeleteEditSaveContinueActionsWhenExistingTimeEntryCalendarItemIsBeingEdited()
            {
                var observer = TestScheduler.CreateObserver<CalendarContextualMenu>();
                var calendarItem = CreateDummyTimeEntryCalendarItem(isRunning: false);
                ViewModel.CurrentMenu.Subscribe(observer);

                ViewModel.OnCalendarItemUpdated.Inputs.OnNext(calendarItem);
                TestScheduler.Start();

                observer.Messages.Should().HaveCount(2);
                var kinds = observer.Messages[1].Value.Value.Actions.Select(action => action.ActionKind);
                kinds.Should().ContainInOrder(
                    CalendarMenuActionKind.Delete,
                    CalendarMenuActionKind.Edit,
                    CalendarMenuActionKind.Save,
                    CalendarMenuActionKind.Continue);
            }

            [Fact]
            public void EmitsDiscardEditSaveStopWhenItemIsBeingEdited()
            {
                var observer = TestScheduler.CreateObserver<CalendarContextualMenu>();
                var calendarItem = CreateDummyTimeEntryCalendarItem(isRunning: true);
                ViewModel.CurrentMenu.Subscribe(observer);

                ViewModel.OnCalendarItemUpdated.Inputs.OnNext(calendarItem);
                TestScheduler.Start();

                observer.Messages.Should().HaveCount(2);
                var kinds = observer.Messages[1].Value.Value.Actions.Select(action => action.ActionKind);
                kinds.Should().ContainInOrder(
                    CalendarMenuActionKind.Discard,
                    CalendarMenuActionKind.Edit,
                    CalendarMenuActionKind.Save,
                    CalendarMenuActionKind.Stop);
            }

            [Fact]
            public void EmitsCopyStartActionsWhenCalendarEventCalendarItemIsBeingEdited()
            {
                var observer = TestScheduler.CreateObserver<CalendarContextualMenu>();
                var calendarItem = CreateDummyCalendarEventCalendarItem(duration: TimeSpan.FromMinutes(30));
                ViewModel.CurrentMenu.Subscribe(observer);

                ViewModel.OnCalendarItemUpdated.Inputs.OnNext(calendarItem);
                TestScheduler.Start();

                observer.Messages.Should().HaveCount(2);
                var kinds = observer.Messages[1].Value.Value.Actions.Select(action => action.ActionKind);
                kinds.Should().ContainInOrder(
                    CalendarMenuActionKind.Copy,
                    CalendarMenuActionKind.Start);
            }

            [Fact]
            public void DoesNotEmitWhenTheCalendarItemBeingUpdatedIsntDifferent()
            {
                var observer = TestScheduler.CreateObserver<CalendarContextualMenu>();
                var startTime = new DateTimeOffset(2019, 10, 10, 10, 10, 10, TimeSpan.Zero);
                var calendarItem = CreateDummyTimeEntryCalendarItem(duration: TimeSpan.FromMinutes(30), startTime: startTime);
                ViewModel.CurrentMenu.Subscribe(observer);

                ViewModel.OnCalendarItemUpdated.Inputs.OnNext(calendarItem);
                TestScheduler.Start();
                ViewModel.OnCalendarItemUpdated.Inputs.OnNext(calendarItem.WithStartTime(DateTimeOffset.Now));
                TestScheduler.Start();

                observer.Messages.Should().HaveCount(2);
            }

            [Fact]
            public void EmitsClosedMenuWhenItemIsUpdatedWithNull()
            {
                var observer = TestScheduler.CreateObserver<CalendarContextualMenu>();
                var startTime = new DateTimeOffset(2019, 10, 10, 10, 10, 10, TimeSpan.Zero);
                var calendarItem = CreateDummyTimeEntryCalendarItem(duration: TimeSpan.FromMinutes(30), startTime: startTime);
                ViewModel.OnCalendarItemUpdated.Inputs.OnNext(calendarItem);
                TestScheduler.Start();
                ViewModel.CurrentMenu.Subscribe(observer);

                ViewModel.OnCalendarItemUpdated.Inputs.OnNext(null);
                TestScheduler.Start();

                observer.Messages.Should().HaveCount(2);
                observer.Messages.Last().Value.Value.Type.Should().Be(ContextualMenuType.Closed);
            }
        }

        public abstract class TheMenuActionTests : CalendarContextualMenuViewModelTest
        {
            protected CalendarContextualMenu ContextualMenu { get; }

            public TheMenuActionTests()
            {
                var observer = TestScheduler.CreateObserver<CalendarContextualMenu>();
                var calendarItem = CreateMenuTypeCalendarItemTrigger();
                ViewModel.CurrentMenu.Subscribe(observer);
                ViewModel.OnCalendarItemUpdated.Inputs.OnNext(calendarItem);
                TestScheduler.Start();

                ContextualMenu = observer.Messages.Last().Value.Value;
            }

            public abstract CalendarItem CreateMenuTypeCalendarItemTrigger();
            
            public virtual void TheDismissActionClosesTheMenuWhenTheCalendarItemDidNotChange()
                => ClosesTheMenuWithoutMakingChanges(() => ContextualMenu.Dismiss.Inputs.OnNext(Unit.Default));

            public virtual void TheDismissActionConfirmsBeforeDiscardingAndClosingTheMenu()
                => ConfirmsBeforeDiscardingChanges(() => ContextualMenu.Dismiss.Inputs.OnNext(Unit.Default));
            
            public void ExecutesActionAndClosesMenu(Action action)
            {
                var menuVisibilityObserver = TestScheduler.CreateObserver<bool>();
                var discardsObserver = TestScheduler.CreateObserver<Unit>();
                var menuObserver = TestScheduler.CreateObserver<CalendarContextualMenu>();
                ViewModel.MenuVisible.Subscribe(menuVisibilityObserver);
                ViewModel.DiscardChanges.Subscribe(discardsObserver);
                ViewModel.CurrentMenu.Subscribe(menuObserver);
                TestScheduler.Start();
                
                action();

                TestScheduler.Start();
                discardsObserver.Messages.Should().BeEmpty();
                menuVisibilityObserver.LastEmittedValue().Should().BeFalse();
                menuObserver.LastEmittedValue().Actions.Should().BeEmpty();
            }

            public void ClosesTheMenuWithoutMakingChanges(Action action)
            {
                var menuVisibilityObserver = TestScheduler.CreateObserver<bool>();
                var discardsObserver = TestScheduler.CreateObserver<Unit>();
                var menuObserver = TestScheduler.CreateObserver<CalendarContextualMenu>();
                var view = Substitute.For<IView>();
                ViewModel.MenuVisible.Subscribe(menuVisibilityObserver);
                ViewModel.DiscardChanges.Subscribe(discardsObserver);
                ViewModel.CurrentMenu.Subscribe(menuObserver);
                ViewModel.AttachView(view);
                discardsObserver.Messages.Should().HaveCount(0);
                TestScheduler.Start();

                action();

                TestScheduler.Start();
                view.DidNotReceiveWithAnyArgs().ConfirmDestructiveAction(Arg.Any<ActionType>());
                menuVisibilityObserver.LastEmittedValue().Should().BeFalse();
                menuObserver.LastEmittedValue().Actions.Should().BeEmpty();
                discardsObserver.Messages.Should().HaveCount(1);
            }
            
            public void ConfirmsBeforeDiscardingChanges(Action action)
            {
                var menuVisibilityObserver = TestScheduler.CreateObserver<bool>();
                var discardsObserver = TestScheduler.CreateObserver<Unit>();
                var menuObserver = TestScheduler.CreateObserver<CalendarContextualMenu>();
                var view = Substitute.For<IView>();
                view.ConfirmDestructiveAction(Arg.Any<ActionType>()).Returns(Observable.Return(false));
                ViewModel.MenuVisible.Subscribe(menuVisibilityObserver);
                ViewModel.DiscardChanges.Subscribe(discardsObserver);
                ViewModel.AttachView(view);
                menuVisibilityObserver.Messages.Clear();
                ViewModel.CurrentMenu.Subscribe(menuObserver);
                discardsObserver.Messages.Should().HaveCount(0);

                var updatedCalendarItem = CreateMenuTypeCalendarItemTrigger().WithDuration(TimeSpan.FromMinutes(7));
                ViewModel.OnCalendarItemUpdated.Inputs.OnNext(updatedCalendarItem);
                TestScheduler.Start();
                action();

                TestScheduler.Start();
                view.Received().ConfirmDestructiveAction(Arg.Is<ActionType>(type => type == ActionType.DiscardEditingChanges));
            }

            public void ClosesTheMenuAndDiscardChangesAfterConfirmingDestructiveAction()
            {
                
            }
            
            public void DoesNotCloseTheMenuAndDoesNotDiscardChangesAfterCancellingDestructiveAction()
            {
                
            }
        }

        public sealed class TheMenuActionForCalendarEvents : TheMenuActionTests
        {
            public override CalendarItem CreateMenuTypeCalendarItemTrigger()
                => CreateDummyCalendarEventCalendarItem();

            [Fact]
            public void TheCopyActionCreatesATimeEntryWithTheDetailsFromTheCalendarEvent()
            {
                var copyAction = ContextualMenu.Actions.First(action => action.ActionKind == CalendarMenuActionKind.Copy);
                var expectedDescription = "X";
                var expectedStartTime = new DateTimeOffset(2019, 10, 10, 10, 10, 10, TimeSpan.Zero);
                var expectedDuration = TimeSpan.FromHours(1);
                var calendarEventBeingEdited = CreateDummyCalendarEventCalendarItem(expectedDescription, expectedStartTime, expectedDuration);
                InteractorFactory.GetDefaultWorkspace().Execute().Returns(Observable.Return(new MockWorkspace(1)));
                ViewModel.OnCalendarItemUpdated.Inputs.OnNext(calendarEventBeingEdited);
                TestScheduler.Start();

                copyAction.MenuItemAction.Execute();
                
                var prototypeArg = Arg.Is<ITimeEntryPrototype>(te =>
                        te.Description == expectedDescription
                           && te.StartTime == expectedStartTime
                           && te.Duration == expectedDuration
                           && te.WorkspaceId == 1
                );
                TestScheduler.Start();
                InteractorFactory.Received().CreateTimeEntry(prototypeArg, TimeEntryStartOrigin.CalendarEvent);
            }

            [Fact]
            public void TheCopyActionClosesTheMenuAfterItsExecution()
                => ExecutesActionAndClosesMenu(TheCopyActionCreatesATimeEntryWithTheDetailsFromTheCalendarEvent);

            [Fact]
            public void TheStartActionCreatesARunningTimeEntryWithTheDetailsFromTheCalendarEvent()
            {
                var startAction = ContextualMenu.Actions.First(action => action.ActionKind == CalendarMenuActionKind.Start);
                var expectedDescription = "X";
                var originalStartTime = new DateTimeOffset(2019, 10, 10, 10, 10, 10, TimeSpan.Zero);
                var calendarEventBeingEdited = CreateDummyCalendarEventCalendarItem(expectedDescription, originalStartTime);
                var now = DateTimeOffset.Now;
                TimeService.CurrentDateTime.Returns(now);
                InteractorFactory.GetDefaultWorkspace().Execute().Returns(Observable.Return(new MockWorkspace(1)));
                ViewModel.OnCalendarItemUpdated.Inputs.OnNext(calendarEventBeingEdited);
                TestScheduler.Start();

                startAction.MenuItemAction.Execute();

                var prototypeArg = Arg.Is<ITimeEntryPrototype>(te =>
                    te.Description == expectedDescription
                    && te.StartTime == now
                    && te.StartTime != originalStartTime
                    && te.Duration == null
                    && te.WorkspaceId == 1
                );
                InteractorFactory.Received().CreateTimeEntry(prototypeArg, TimeEntryStartOrigin.CalendarEvent);
            }

            [Fact]
            public void TheStartActionClosesTheMenuAfterItsExecution()
                => ExecutesActionAndClosesMenu(TheStartActionCreatesARunningTimeEntryWithTheDetailsFromTheCalendarEvent);

            [Fact]
            public override void TheDismissActionClosesTheMenuWhenTheCalendarItemDidNotChange()
            {
                base.TheDismissActionClosesTheMenuWhenTheCalendarItemDidNotChange();
            }
        }

        public sealed class TheMenuForNewTimeEntries : TheMenuActionTests
        {
            public override CalendarItem CreateMenuTypeCalendarItemTrigger()
                => CreateEmptyCalendarItem();

            [Fact]
            public void TheDiscardActionTriggersTheDiscardChangesObservable()
            {
                var observer = TestScheduler.CreateObserver<Unit>();
                ViewModel.DiscardChanges.Subscribe(observer);
                var discardAction = ContextualMenu.Actions.First(action => action.ActionKind == CalendarMenuActionKind.Discard);
                var expectedStartTime = new DateTimeOffset(2019, 10, 10, 10, 10, 10, TimeSpan.Zero);
                var expectedDuration = TimeSpan.FromMinutes(30);
                var expectedDescription = "whatever";
                var newCalendarItem = new CalendarItem(
                    string.Empty,
                    CalendarItemSource.TimeEntry,
                    expectedStartTime,
                    expectedDuration,
                    expectedDescription,
                    CalendarIconKind.None);
                InteractorFactory.GetDefaultWorkspace().Execute().Returns(Observable.Return(new MockWorkspace(1)));
                TestScheduler.Start();
                observer.Messages.Should().HaveCount(0);
                ViewModel.OnCalendarItemUpdated.Inputs.OnNext(newCalendarItem);
                TestScheduler.Start();

                discardAction.MenuItemAction.Execute();

                TestScheduler.Start();
                observer.Messages.Should().HaveCount(1);
            }
            
            [Fact]
            public void TheDiscardActionExecutesActionAndClosesMenu()
                => ClosesTheMenuWithoutMakingChanges(TheDiscardActionTriggersTheDiscardChangesObservable);

            [Fact]
            public void TheEditActionNavigatesToTheStartTimeEntryViewModelWithProperParameters()
            {
                var editAction = ContextualMenu.Actions.First(action => action.ActionKind == CalendarMenuActionKind.Edit);

                var expectedStartTime = new DateTimeOffset(2019, 10, 10, 10, 10, 10, TimeSpan.Zero);
                var expectedDuration = TimeSpan.FromMinutes(30);
                var newCalendarItem = new CalendarItem(
                    string.Empty,
                    CalendarItemSource.TimeEntry,
                    expectedStartTime,
                    expectedDuration,
                    string.Empty,
                    CalendarIconKind.None);
                InteractorFactory.GetDefaultWorkspace().Execute().Returns(Observable.Return(new MockWorkspace(1)));
                var view = Substitute.For<IView>();
                ViewModel.AttachView(view);
                ViewModel.OnCalendarItemUpdated.Inputs.OnNext(newCalendarItem);
                TestScheduler.Start();

                editAction.MenuItemAction.Execute();

                var startTimeEntryArg = Arg.Is<StartTimeEntryParameters>(param =>
                    param.StartTime == expectedStartTime
                    && param.Duration == expectedDuration
                    && param.EntryDescription == string.Empty
                    && param.WorkspaceId == 1
                );
                TestScheduler.Start();
                NavigationService.Received().Navigate<StartTimeEntryViewModel, StartTimeEntryParameters, Unit>(startTimeEntryArg, view);
            }
            
            [Fact]
            public void TheEditActionExecutesActionAndClosesMenu()
                => ExecutesActionAndClosesMenu(TheEditActionNavigatesToTheStartTimeEntryViewModelWithProperParameters);

            [Fact]
            public void TheSaveActionCreatesATimeEntryWithNoDescriptionWithTheRightStartTimeAndDuration()
            {
                var saveAction = ContextualMenu.Actions.First(action => action.ActionKind == CalendarMenuActionKind.Save);
                var expectedStartTime = new DateTimeOffset(2019, 10, 10, 10, 10, 10, TimeSpan.Zero);
                var expectedDuration = TimeSpan.FromMinutes(30);
                var expectedDescription = "whatever";
                var newCalendarItem = new CalendarItem(
                    string.Empty,
                    CalendarItemSource.TimeEntry,
                    expectedStartTime,
                    expectedDuration,
                    expectedDescription,
                    CalendarIconKind.None);
                InteractorFactory.GetDefaultWorkspace().Execute().Returns(Observable.Return(new MockWorkspace(1)));
                ViewModel.OnCalendarItemUpdated.Inputs.OnNext(newCalendarItem);
                TestScheduler.Start();

                saveAction.MenuItemAction.Execute();

                var prototypeArg = Arg.Is<ITimeEntryPrototype>(te =>
                    te.Description == expectedDescription
                    && te.StartTime == expectedStartTime
                    && te.Duration == expectedDuration
                    && te.WorkspaceId == 1
                );
                TestScheduler.Start();
                InteractorFactory.Received().CreateTimeEntry(prototypeArg, TimeEntryStartOrigin.CalendarEvent);
            }
            
            [Fact]
            public void TheSaveActionExecutesActionAndClosesMenu()
                => ExecutesActionAndClosesMenu(TheSaveActionCreatesATimeEntryWithNoDescriptionWithTheRightStartTimeAndDuration);

            [Fact]
            public override void TheDismissActionClosesTheMenuWhenTheCalendarItemDidNotChange()
            {
                base.TheDismissActionClosesTheMenuWhenTheCalendarItemDidNotChange();
            }
            
            [Fact]
            public override void TheDismissActionConfirmsBeforeDiscardingAndClosingTheMenu()
            {
                base.TheDismissActionConfirmsBeforeDiscardingAndClosingTheMenu();
            }
        }

        public sealed class TheMenuForRunningEntries : TheMenuActionTests
        {
            public override CalendarItem CreateMenuTypeCalendarItemTrigger()
                => CreateDummyTimeEntryCalendarItem(isRunning: true);

            [Fact]
            public void TheDiscardActionDeletesTheRunningTimeEntry()
            {
                var discardAction = ContextualMenu.Actions.First(action => action.ActionKind == CalendarMenuActionKind.Discard);
                var runningTimeEntryId = 10;
                var runningTimeEntry = CreateDummyTimeEntryCalendarItem(isRunning: true, timeEntryId: runningTimeEntryId);
                ViewModel.OnCalendarItemUpdated.Inputs.OnNext(runningTimeEntry);
                TestScheduler.Start();

                discardAction.MenuItemAction.Execute();

                TestScheduler.Start();
                InteractorFactory.Received().DeleteTimeEntry(runningTimeEntryId);
            }
            
            [Fact]
            public void TheDiscardActionExecutesActionAndClosesMenu()
                => ExecutesActionAndClosesMenu(TheDiscardActionDeletesTheRunningTimeEntry);

            [Fact]
            public void TheEditActionNavigatesToTheEditTimeEntryViewModelWithTheRightId()
            {
                var editAction = ContextualMenu.Actions.First(action => action.ActionKind == CalendarMenuActionKind.Edit);
                var runningTimeEntryId = 10L;
                var runningTimeEntry = CreateDummyTimeEntryCalendarItem(isRunning: true, timeEntryId: runningTimeEntryId);
                var view = Substitute.For<IView>();
                ViewModel.AttachView(view);
                ViewModel.OnCalendarItemUpdated.Inputs.OnNext(runningTimeEntry);
                TestScheduler.Start();

                editAction.MenuItemAction.Execute();

                var idArg = Arg.Is<long[]>(ids => ids[0] == runningTimeEntryId);
                TestScheduler.Start();
                NavigationService.Received().Navigate<EditTimeEntryViewModel, long[], Unit>(idArg, view);
            }
            
            [Fact]
            public void TheEditActionExecutesActionAndClosesMenu()
                => ExecutesActionAndClosesMenu(TheEditActionNavigatesToTheEditTimeEntryViewModelWithTheRightId);

            [Fact]
            public void TheSaveActionUpdatesTheRunningTimeEntry()
            {
                var saveAction = ContextualMenu.Actions.First(action => action.ActionKind == CalendarMenuActionKind.Save);
                var runningTimeEntryId = 10L;
                var newStartTime = new DateTimeOffset(2019, 10, 10, 10, 10, 10, TimeSpan.Zero);
                var runningTimeEntry = CreateDummyTimeEntryCalendarItem(true, runningTimeEntryId, newStartTime);
                var originalStartTime = new DateTimeOffset(2019, 10, 10, 11, 10, 10, TimeSpan.Zero);
                var mockWorkspace = new MockWorkspace(1);
                var timeEntryMock = new MockTimeEntry(runningTimeEntryId, mockWorkspace, originalStartTime);
                InteractorFactory.GetTimeEntryById(runningTimeEntryId).Execute().Returns(Observable.Return(timeEntryMock));
                ViewModel.OnCalendarItemUpdated.Inputs.OnNext(runningTimeEntry);
                TestScheduler.Start();

                saveAction.MenuItemAction.Execute();

                var dtoArg = Arg.Is<EditTimeEntryDto>(dto
                    => dto.Id == runningTimeEntryId
                       && dto.StartTime == newStartTime
                       && dto.StartTime != originalStartTime
                       && dto.WorkspaceId == mockWorkspace.Id
                       && !dto.StopTime.HasValue);
                TestScheduler.Start();
                InteractorFactory.Received().UpdateTimeEntry(dtoArg);
            }
            
            [Fact]
            public void TheSaveActionExecutesActionAndClosesMenu()
                => ExecutesActionAndClosesMenu(TheSaveActionUpdatesTheRunningTimeEntry);

            [Fact]
            public void TheStopActionStopsTheRunningTimeEntry()
            {
                var stopAction = ContextualMenu.Actions.First(action => action.ActionKind == CalendarMenuActionKind.Stop);
                var runningTimeEntryId = 10L;
                var runningTimeEntry = CreateDummyTimeEntryCalendarItem(isRunning: true, timeEntryId: runningTimeEntryId);
                var now = DateTimeOffset.Now;
                TimeService.CurrentDateTime.Returns(now);
                ViewModel.OnCalendarItemUpdated.Inputs.OnNext(runningTimeEntry);
                TestScheduler.Start();

                stopAction.MenuItemAction.Execute();

                TestScheduler.Start();
                InteractorFactory.Received().StopTimeEntry(now, TimeEntryStopOrigin.CalendarContextualMenu);
            }
            
            [Fact]
            public void TheStopActionExecutesActionAndClosesMenu()
                => ExecutesActionAndClosesMenu(TheStopActionStopsTheRunningTimeEntry);

            [Fact]
            public override void TheDismissActionClosesTheMenuWhenTheCalendarItemDidNotChange()
            {
                base.TheDismissActionClosesTheMenuWhenTheCalendarItemDidNotChange();
            }
            
            [Fact]
            public override void TheDismissActionConfirmsBeforeDiscardingAndClosingTheMenu()
            {
                base.TheDismissActionConfirmsBeforeDiscardingAndClosingTheMenu();
            }
        }

        public sealed class TheMenuForStoppedEntries : TheMenuActionTests
        {
            public override CalendarItem CreateMenuTypeCalendarItemTrigger()
                => CreateDummyTimeEntryCalendarItem(isRunning: false);

            [Fact]
            public void TheDeleteActionDeletesTheRunningTimeEntry()
            {
                var deleteAction = ContextualMenu.Actions.First(action => action.ActionKind == CalendarMenuActionKind.Delete);
                var stoppedTimeEntryId = 10;
                var stoppedTimeEntry = CreateDummyTimeEntryCalendarItem(isRunning: false, timeEntryId: stoppedTimeEntryId);
                ViewModel.OnCalendarItemUpdated.Inputs.OnNext(stoppedTimeEntry);
                TestScheduler.Start();

                deleteAction.MenuItemAction.Execute();

                TestScheduler.Start();
                InteractorFactory.Received().DeleteTimeEntry(stoppedTimeEntryId);
            }
            
            [Fact]
            public void TheDeleteActionExecutesActionAndClosesMenu()
                => ExecutesActionAndClosesMenu(TheDeleteActionDeletesTheRunningTimeEntry);

            [Fact]
            public void TheEditActionNavigatesToTheEditTimeEntryViewModelWithTheRightId()
            {
                var editAction = ContextualMenu.Actions.First(action => action.ActionKind == CalendarMenuActionKind.Edit);
                var stoppedTimeEntryId = 10L;
                var stoppedTimeEntry = CreateDummyTimeEntryCalendarItem(isRunning: false, timeEntryId: stoppedTimeEntryId);
                var view = Substitute.For<IView>();
                ViewModel.AttachView(view);
                ViewModel.OnCalendarItemUpdated.Inputs.OnNext(stoppedTimeEntry);
                TestScheduler.Start();

                editAction.MenuItemAction.Execute();

                TestScheduler.Start();
                var idArg = Arg.Is<long[]>(ids => ids[0] == stoppedTimeEntryId);
                NavigationService.Received().Navigate<EditTimeEntryViewModel, long[], Unit>(idArg, view);
            }
            
            [Fact]
            public void TheEditActionExecutesActionAndClosesMenu()
                => ExecutesActionAndClosesMenu(TheEditActionNavigatesToTheEditTimeEntryViewModelWithTheRightId);

            [Fact]
            public void TheSaveActionUpdatesTheRunningTimeEntry()
            {
                var saveAction = ContextualMenu.Actions.First(action => action.ActionKind == CalendarMenuActionKind.Save);
                var stoppedTimeEntryId = 10L;
                var newStartTime = new DateTimeOffset(2019, 10, 10, 10, 10, 10, TimeSpan.Zero);
                var newEndTime = new DateTimeOffset(2019, 10, 10, 10, 30, 10, TimeSpan.Zero);
                var stoppedTimeEntry = CreateDummyTimeEntryCalendarItem(false, stoppedTimeEntryId, newStartTime, newEndTime - newStartTime);
                var originalStartTime = new DateTimeOffset(2019, 10, 10, 11, 10, 10, TimeSpan.Zero);
                var originalEndTime = new DateTimeOffset(2019, 10, 10, 11, 30, 10, TimeSpan.Zero);
                var mockWorkspace = new MockWorkspace(1);
                var timeEntryMock = new MockTimeEntry(stoppedTimeEntryId, mockWorkspace, originalStartTime, (long)(originalEndTime - originalStartTime).TotalSeconds);
                InteractorFactory.GetTimeEntryById(stoppedTimeEntryId).Execute().Returns(Observable.Return(timeEntryMock));
                ViewModel.OnCalendarItemUpdated.Inputs.OnNext(stoppedTimeEntry);
                TestScheduler.Start();

                saveAction.MenuItemAction.Execute();

                var dtoArg = Arg.Is<EditTimeEntryDto>(dto
                    => dto.Id == stoppedTimeEntryId
                       && dto.StartTime == newStartTime
                       && dto.StartTime != originalStartTime
                       && dto.WorkspaceId == mockWorkspace.Id
                       && dto.StopTime.HasValue
                       && dto.StopTime.Value == newEndTime
                       && dto.StopTime.Value != originalEndTime);
                TestScheduler.Start();
                InteractorFactory.Received().UpdateTimeEntry(dtoArg);
            }
            
            [Fact]
            public void TheSaveActionExecutesActionAndClosesMenu()
                => ExecutesActionAndClosesMenu(TheSaveActionUpdatesTheRunningTimeEntry);
            
            [Fact]
            public void TheContinueActionStartsANewTheRunningTimeEntryWithTheDetailsFromTheCalendarItemCalledFromTheMenuAction()
            {
                var continueAction = ContextualMenu.Actions.First(action => action.ActionKind == CalendarMenuActionKind.Continue);
                var stoppedTimeEntryId = 10L;
                var stoppedTimeEntry = CreateDummyTimeEntryCalendarItem(isRunning: false, timeEntryId: stoppedTimeEntryId);
                var mockWorkspace = new MockWorkspace(1);
                var mockProject = new MockProject(1, mockWorkspace);
                var mockTask = new MockTask(1, mockWorkspace, mockProject);
                var expectedTimeEntryToContinue = Substitute.For<IThreadSafeTimeEntry>();
                expectedTimeEntryToContinue.WorkspaceId.Returns(1);
                expectedTimeEntryToContinue.Description.Returns("");
                expectedTimeEntryToContinue.Duration.Returns(100);
                expectedTimeEntryToContinue.Start.Returns(stoppedTimeEntry.StartTime);
                expectedTimeEntryToContinue.Project.Returns(mockProject);
                expectedTimeEntryToContinue.Task.Returns(mockTask);
                expectedTimeEntryToContinue.TagIds.Returns(Array.Empty<long>());
                expectedTimeEntryToContinue.Billable.Returns(false);
                InteractorFactory.GetTimeEntryById(stoppedTimeEntryId).Execute().Returns(Observable.Return(expectedTimeEntryToContinue));
                ViewModel.OnCalendarItemUpdated.Inputs.OnNext(stoppedTimeEntry);
                TestScheduler.Start();

                continueAction.MenuItemAction.Execute();

                var continuePrototype = Arg.Is(stoppedTimeEntryId);

                TestScheduler.Start();
                InteractorFactory.Received().ContinueTimeEntry(continuePrototype, ContinueTimeEntryMode.CalendarContextualMenu);
            }
            
            [Fact]
            public void TheContinueActionExecutesActionAndClosesMenu()
                => ExecutesActionAndClosesMenu(TheContinueActionStartsANewTheRunningTimeEntryWithTheDetailsFromTheCalendarItemCalledFromTheMenuAction);
            
            [Fact]
            public override void TheDismissActionClosesTheMenuWhenTheCalendarItemDidNotChange()
            {
                base.TheDismissActionClosesTheMenuWhenTheCalendarItemDidNotChange();
            }
            
            [Fact]
            public override void TheDismissActionConfirmsBeforeDiscardingAndClosingTheMenu()
            {
                base.TheDismissActionConfirmsBeforeDiscardingAndClosingTheMenu();
            }
        }

        public sealed class TheOnCalendarItemUpdatedInputEntry : CalendarContextualMenuViewModelTest
        {
            [Fact]
            public void UpdatesTheCurrentItemInEditModeWhenTheContextualMenuIsClosed()
            {
                var observer = TestScheduler.CreateObserver<CalendarItem?>();
                var now = DateTimeOffset.Now;
                var calendarItem = new CalendarItem(
                    "",
                    CalendarItemSource.TimeEntry,
                    now,
                    TimeSpan.FromMinutes(30), 
                    "Such description",
                    CalendarIconKind.None,
                    "#c2c2c2",
                    project: "Such Project",
                    task: "Such Task",
                    client: "Such Client");
                ViewModel.CalendarItemInEditMode.Subscribe(observer);
                TestScheduler.Start();
                
                ViewModel.OnCalendarItemUpdated.Execute(calendarItem);
                TestScheduler.Start();

                observer.Messages.Should().HaveCount(1);
                observer.Messages.First().Value.Value.Should()
                    .Match<CalendarItem?>(ci =>
                        ci.HasValue &&
                        ci.Value.Id == ""
                        && ci.Value.Source == CalendarItemSource.TimeEntry
                        && ci.Value.StartTime == now
                        && ci.Value.Duration == TimeSpan.FromMinutes(30)
                    );
            }

            [Fact]
            public void UpdatesTheCurrentItemInEditModeWithoutConfirmationsWhenAnewItemIsInputtedAndCurrentItemInEditModeHasNotChanged()
            {
                var observer = TestScheduler.CreateObserver<CalendarItem?>();
                var now = DateTimeOffset.Now;
                var view = Substitute.For<IView>();
                ViewModel.AttachView(view);
                var startingCalendarItem = new CalendarItem(
                    "1",
                    CalendarItemSource.TimeEntry,
                    now,
                    TimeSpan.FromMinutes(30), 
                    "Such description",
                    CalendarIconKind.None,
                    "#c2c2c2",
                    project: "Such Project",
                    task: "Such Task",
                    client: "Such Client",
                    timeEntryId: 1);
                ViewModel.CalendarItemInEditMode.Subscribe(observer);
                TestScheduler.Start();
                ViewModel.OnCalendarItemUpdated.Execute(startingCalendarItem);
                TestScheduler.Start();
                var newCalendarItem = new CalendarItem(
                    "2",
                    CalendarItemSource.TimeEntry,
                    now,
                    TimeSpan.FromMinutes(30), 
                    "Such description",
                    CalendarIconKind.None,
                    "#c2c2c2",
                    project: "Such Project",
                    task: "Such Task",
                    client: "Such Client",
                    timeEntryId: 2);
                
                ViewModel.OnCalendarItemUpdated.Execute(newCalendarItem);
                TestScheduler.Start();

                view.DidNotReceiveWithAnyArgs().ConfirmDestructiveAction(Arg.Any<ActionType>());
                observer.Messages.Should().HaveCount(2);
                observer.Messages.Last().Value.Value.Should()
                    .Match<CalendarItem?>(ci =>
                        ci.HasValue &&
                        ci.Value.Id == "2"
                    );
            }
            
            [Fact]
            public void UpdatesTheCurrentItemInEditModeWhenAnewItemIsInputtedAndCurrentMenuIsFromACalendarEventItem()
            {
                var observer = TestScheduler.CreateObserver<CalendarItem?>();
                var now = DateTimeOffset.Now;
                var view = Substitute.For<IView>();
                ViewModel.AttachView(view);
                var startingCalendarItem = new CalendarItem(
                    "1",
                    CalendarItemSource.Calendar,
                    now,
                    TimeSpan.FromMinutes(30), 
                    "Such description",
                    CalendarIconKind.Event,
                    "#c2c2c2",
                    calendarId: "X");
                ViewModel.CalendarItemInEditMode.Subscribe(observer);
                TestScheduler.Start();
                ViewModel.OnCalendarItemUpdated.Execute(startingCalendarItem);
                TestScheduler.Start();
                var newCalendarItem = new CalendarItem(
                    "2",
                    CalendarItemSource.TimeEntry,
                    now,
                    TimeSpan.FromMinutes(30), 
                    "Such description",
                    CalendarIconKind.None,
                    "#c2c2c2",
                    project: "Such Project",
                    task: "Such Task",
                    client: "Such Client",
                    timeEntryId: 2);
                
                ViewModel.OnCalendarItemUpdated.Execute(newCalendarItem);
                TestScheduler.Start();

                view.DidNotReceiveWithAnyArgs().ConfirmDestructiveAction(Arg.Any<ActionType>());
                observer.Messages.Should().HaveCount(2);
                observer.Messages.Last().Value.Value.Should()
                    .Match<CalendarItem?>(ci =>
                        ci.HasValue &&
                        ci.Value.Id == "2"
                    );
            }
            
            [Fact]
            public void ConfirmsBeforeUpdatingTheCurrentItemInEditModeWhenANewItemIsInputtedAndTheCurrentItemInEditModeHasChanged()
            {
                var observer = TestScheduler.CreateObserver<CalendarItem?>();
                var now = DateTimeOffset.Now;
                var view = Substitute.For<IView>();
                view.ConfirmDestructiveAction(Arg.Any<ActionType>()).Returns(Observable.Return(true));
                ViewModel.AttachView(view);
                var startingCalendarItem = new CalendarItem(
                    "1",
                    CalendarItemSource.TimeEntry,
                    now,
                    TimeSpan.FromMinutes(30), 
                    "Such description",
                    CalendarIconKind.None,
                    "#c2c2c2",
                    project: "Such Project",
                    task: "Such Task",
                    client: "Such Client",
                    timeEntryId: 1);
                ViewModel.CalendarItemInEditMode.Subscribe(observer);
                TestScheduler.Start();
                ViewModel.OnCalendarItemUpdated.Execute(startingCalendarItem);
                TestScheduler.Start();
                ViewModel.OnCalendarItemUpdated.Execute(startingCalendarItem.WithDuration(TimeSpan.FromMinutes(15)));
                TestScheduler.Start();
                var newCalendarItem = new CalendarItem(
                    "2",
                    CalendarItemSource.TimeEntry,
                    now,
                    TimeSpan.FromMinutes(30), 
                    "Such description",
                    CalendarIconKind.None,
                    "#c2c2c2",
                    project: "Such Project",
                    task: "Such Task",
                    client: "Such Client",
                    timeEntryId: 2);
                
                ViewModel.OnCalendarItemUpdated.Execute(newCalendarItem);
                TestScheduler.Start();

                view.Received().ConfirmDestructiveAction(Arg.Is(ActionType.DiscardEditingChanges));
                observer.Messages.Should().HaveCount(2);
            }

            [Fact]
            public void ConfirmsBeforeClosingTheMenuWhenANullCalendarItemIsInputtedAndChangesWereMadeToTheCurrentItemInEditMode()
            {
                var observer = TestScheduler.CreateObserver<CalendarItem?>();
                var now = DateTimeOffset.Now;
                var view = Substitute.For<IView>();
                view.ConfirmDestructiveAction(Arg.Any<ActionType>()).Returns(Observable.Return(true));
                ViewModel.AttachView(view);
                var startingCalendarItem = new CalendarItem(
                    "1",
                    CalendarItemSource.TimeEntry,
                    now,
                    TimeSpan.FromMinutes(30), 
                    "Such description",
                    CalendarIconKind.None,
                    "#c2c2c2",
                    project: "Such Project",
                    task: "Such Task",
                    client: "Such Client",
                    timeEntryId: 1);
                ViewModel.CalendarItemInEditMode.Subscribe(observer);
                TestScheduler.Start();
                ViewModel.OnCalendarItemUpdated.Execute(startingCalendarItem);
                TestScheduler.Start();
                ViewModel.OnCalendarItemUpdated.Execute(startingCalendarItem.WithDuration(TimeSpan.FromMinutes(15)));
                TestScheduler.Start();
                
                ViewModel.OnCalendarItemUpdated.Execute(null);
                TestScheduler.Start();

                view.Received().ConfirmDestructiveAction(Arg.Is(ActionType.DiscardEditingChanges));
                observer.Messages.Should().HaveCount(2);
            }
            
            [Fact]
            public void DoesNotUpdateTheCurrentItemInEditModeWhenANewItemIsInputtedAndTheCurrentItemInEditModeHasChangedButConfirmationIsDenied()
            {
                var observer = TestScheduler.CreateObserver<CalendarItem?>();
                var now = DateTimeOffset.Now;
                var view = Substitute.For<IView>();
                view.ConfirmDestructiveAction(Arg.Any<ActionType>()).Returns(Observable.Return(false));
                ViewModel.AttachView(view);
                var startingCalendarItem = new CalendarItem(
                    "1",
                    CalendarItemSource.TimeEntry,
                    now,
                    TimeSpan.FromMinutes(30), 
                    "Such description",
                    CalendarIconKind.None,
                    "#c2c2c2",
                    project: "Such Project",
                    task: "Such Task",
                    client: "Such Client",
                    timeEntryId: 1);
                ViewModel.CalendarItemInEditMode.Subscribe(observer);
                TestScheduler.Start();
                ViewModel.OnCalendarItemUpdated.Execute(startingCalendarItem);
                TestScheduler.Start();
                ViewModel.OnCalendarItemUpdated.Execute(startingCalendarItem.WithDuration(TimeSpan.FromMinutes(15)));
                TestScheduler.Start();
                var newCalendarItem = new CalendarItem(
                    "2",
                    CalendarItemSource.TimeEntry,
                    now,
                    TimeSpan.FromMinutes(30), 
                    "Such description",
                    CalendarIconKind.None,
                    "#c2c2c2",
                    project: "Such Project",
                    task: "Such Task",
                    client: "Such Client",
                    timeEntryId: 2);
                
                ViewModel.OnCalendarItemUpdated.Execute(newCalendarItem);
                TestScheduler.Start();

                view.Received().ConfirmDestructiveAction(Arg.Is(ActionType.DiscardEditingChanges));
                observer.Messages.Should().HaveCount(1);
            }

            [Fact]
            public void DoesNotCloseTheMenuWhenANullCalendarItemIsInputtedAndChangesWereMadeToTheCurrentItemInEditModeAndConfirmationIsDenied()
            {
                var observer = TestScheduler.CreateObserver<CalendarItem?>();
                var now = DateTimeOffset.Now;
                var view = Substitute.For<IView>();
                view.ConfirmDestructiveAction(Arg.Any<ActionType>()).Returns(Observable.Return(false));
                ViewModel.AttachView(view);
                var startingCalendarItem = new CalendarItem(
                    "1",
                    CalendarItemSource.TimeEntry,
                    now,
                    TimeSpan.FromMinutes(30), 
                    "Such description",
                    CalendarIconKind.None,
                    "#c2c2c2",
                    project: "Such Project",
                    task: "Such Task",
                    client: "Such Client",
                    timeEntryId: 1);
                ViewModel.CalendarItemInEditMode.Subscribe(observer);
                TestScheduler.Start();
                ViewModel.OnCalendarItemUpdated.Execute(startingCalendarItem);
                TestScheduler.Start();
                ViewModel.OnCalendarItemUpdated.Execute(startingCalendarItem.WithDuration(TimeSpan.FromMinutes(15)));
                TestScheduler.Start();
                
                ViewModel.OnCalendarItemUpdated.Execute(null);
                TestScheduler.Start();

                view.Received().ConfirmDestructiveAction(Arg.Is(ActionType.DiscardEditingChanges));
                observer.Messages.Should().HaveCount(1);
            }
            
            [Fact]
            public void ClosesTheMenuWhenANullCalendarItemIsInputtedAndNoChangesWereMadeToTheCurrentItemInEditMode()
            {
                var observer = TestScheduler.CreateObserver<CalendarItem?>();
                var now = DateTimeOffset.Now;
                var view = Substitute.For<IView>();
                ViewModel.AttachView(view);
                var startingCalendarItem = new CalendarItem(
                    "1",
                    CalendarItemSource.TimeEntry,
                    now,
                    TimeSpan.FromMinutes(30), 
                    "Such description",
                    CalendarIconKind.None,
                    "#c2c2c2",
                    project: "Such Project",
                    task: "Such Task",
                    client: "Such Client",
                    timeEntryId: 1);
                ViewModel.CalendarItemInEditMode.Subscribe(observer);
                TestScheduler.Start();
                ViewModel.OnCalendarItemUpdated.Execute(startingCalendarItem);
                TestScheduler.Start();

                ViewModel.OnCalendarItemUpdated.Execute(null);
                TestScheduler.Start();

                view.DidNotReceiveWithAnyArgs().ConfirmDestructiveAction(Arg.Any<ActionType>());
                observer.Messages.Should().HaveCount(2);
                observer.Messages.Last().Value.Value.Should().BeNull();
            }
            
            [Fact]
            public void ClosesTheMenuWhenANullCalendarItemIsInputtedAndTheCurrentMenuIsFromACalendarEventItem()
            {
                var observer = TestScheduler.CreateObserver<CalendarItem?>();
                var now = DateTimeOffset.Now;
                var view = Substitute.For<IView>();
                ViewModel.AttachView(view);
                var startingCalendarItem = new CalendarItem(
                    "1",
                    CalendarItemSource.Calendar,
                    now,
                    TimeSpan.FromMinutes(30), 
                    "Such description",
                    CalendarIconKind.Event,
                    "#c2c2c2",
                    calendarId: "X");
                ViewModel.CalendarItemInEditMode.Subscribe(observer);
                TestScheduler.Start();
                ViewModel.OnCalendarItemUpdated.Execute(startingCalendarItem);
                TestScheduler.Start();
                
                ViewModel.OnCalendarItemUpdated.Execute(null);
                TestScheduler.Start();

                view.DidNotReceiveWithAnyArgs().ConfirmDestructiveAction(Arg.Any<ActionType>());
                observer.Messages.Should().HaveCount(2);
                observer.Messages.Last().Value.Value.Should().BeNull();
            }
        }

        public sealed class TheTimeEntryInfoObservable : CalendarContextualMenuViewModelTest
        {
            [Fact]
            public void StartsWithTheTimeEntryInfoFromPassedFirstThroughOnCalendarItemUpdated()
            {
                var observer = TestScheduler.CreateObserver<TimeEntryDisplayInfo>();
                var calendarItem = new CalendarItem(
                    "",
                    CalendarItemSource.TimeEntry,
                    DateTimeOffset.Now,
                    TimeSpan.FromMinutes(30),
                    "Such description",
                    CalendarIconKind.None,
                    "#c2c2c2",
                    project: "Such Project",
                    task: "Such Task",
                    client: "Such Client");
                ViewModel.TimeEntryInfo.Subscribe(observer);
                TestScheduler.Start();

                ViewModel.OnCalendarItemUpdated.Execute(calendarItem);
                TestScheduler.Start();

                observer.Messages.First().Value.Value
                    .Should()
                    .Match<TimeEntryDisplayInfo>(e =>
                        e.Description == "Such description"
                        && e.ProjectTaskColor == "#c2c2c2"
                        && e.Project == "Such Project"
                        && e.Task == "Such Task"
                        && e.Client == "Such Client");
            }

            [Fact]
            public void EmitsWhenItemIsUpdatedWithNewDetails()
            {
                var observer = TestScheduler.CreateObserver<TimeEntryDisplayInfo>();
                var startingCalendarItem = new CalendarItem(
                    "",
                    CalendarItemSource.TimeEntry,
                    DateTimeOffset.Now,
                    TimeSpan.FromMinutes(30),
                    "Old description",
                    CalendarIconKind.None,
                    "#c2c2c2",
                    project: "Old Project",
                    task: "Old Task",
                    client: "Old Client");

                var updatedCalendarItem = new CalendarItem(
                    "",
                    CalendarItemSource.TimeEntry,
                    DateTimeOffset.Now,
                    TimeSpan.FromMinutes(30),
                    "New description",
                    CalendarIconKind.None,
                    "#f2f2f2",
                    project: "New Project",
                    task: "New Task",
                    client: "New Client");
                ViewModel.TimeEntryInfo.Subscribe(observer);
                TestScheduler.Start();
                ViewModel.OnCalendarItemUpdated.Execute(startingCalendarItem);
                TestScheduler.Start();

                ViewModel.OnCalendarItemUpdated.Execute(updatedCalendarItem);

                TestScheduler.Start();
                observer.Messages[0].Value.Value
                    .Should()
                    .Match<TimeEntryDisplayInfo>(e =>
                        e.Description == "Old description"
                        && e.ProjectTaskColor == "#c2c2c2"
                        && e.Project == "Old Project"
                        && e.Task == "Old Task"
                        && e.Client == "Old Client");

                observer.Messages[1].Value.Value
                    .Should()
                    .Match<TimeEntryDisplayInfo>(e =>
                        e.Description == "New description"
                        && e.ProjectTaskColor == "#f2f2f2"
                        && e.Project == "New Project"
                        && e.Task == "New Task"
                        && e.Client == "New Client");
            }

            [Fact]
            public void DoesNotEmitWhenItemIsUpdatedWithTheSameDetailsEvenIfStartTimeAndDurationAreDifferent()
            {
                var observer = TestScheduler.CreateObserver<TimeEntryDisplayInfo>();
                var startingCalendarItem = new CalendarItem(
                    "",
                    CalendarItemSource.TimeEntry,
                    DateTimeOffset.Now,
                    TimeSpan.FromMinutes(30),
                    "description",
                    CalendarIconKind.None,
                    "#c2c2c2",
                    project: "project",
                    task: "task",
                    client: "client");

                var updatedCalendarItem = new CalendarItem(
                    "",
                    CalendarItemSource.TimeEntry,
                    DateTimeOffset.Now.AddHours(1),
                    TimeSpan.FromMinutes(25),
                    "description",
                    CalendarIconKind.None,
                    "#c2c2c2",
                    project: "project",
                    task: "task",
                    client: "client");
                ViewModel.TimeEntryInfo.Subscribe(observer);
                TestScheduler.Start();
                ViewModel.OnCalendarItemUpdated.Execute(startingCalendarItem);
                TestScheduler.Start();

                ViewModel.OnCalendarItemUpdated.Execute(updatedCalendarItem);
                TestScheduler.Start();

                observer.Messages.Should().HaveCount(1);
                observer.Messages[0].Value.Value
                    .Should()
                    .Match<TimeEntryDisplayInfo>(e =>
                        e.Description == "description"
                        && e.ProjectTaskColor == "#c2c2c2"
                        && e.Project == "project"
                        && e.Task == "task"
                        && e.Client == "client");
            }

            [Fact]
            public void DoesNotEmitAnotherContextualMenuWhenTheItemIsUpdated()
            {
                var observer = TestScheduler.CreateObserver<CalendarContextualMenu>();
                var startingCalendarItem = new CalendarItem("", CalendarItemSource.TimeEntry, DateTimeOffset.Now, TimeSpan.FromMinutes(30), "Old description", CalendarIconKind.None, "#c2c2c2", project: "Old Project", task: "Old Task", client: "Old Client");
                var updatedCalendarItem = new CalendarItem("", CalendarItemSource.TimeEntry, DateTimeOffset.Now, TimeSpan.FromMinutes(30), "New description", CalendarIconKind.None, "#f2f2f2", project: "New Project", task: "New Task", client: "New Client");
                ViewModel.OnCalendarItemUpdated.Execute(startingCalendarItem);
                TestScheduler.Start();
                ViewModel.CurrentMenu.Subscribe(observer);

                ViewModel.OnCalendarItemUpdated.Execute(updatedCalendarItem);
                TestScheduler.Start();

                observer.Messages.Should().HaveCount(1);
            }
        }

        public sealed class TheTimeEntryPeriodObservable : CalendarContextualMenuViewModelTest
        {
            [Fact]
            public void StartsWithThePeriodFromCalendarItemPassedFirstOnCalendarItemUpdated()
            {
                var observer = TestScheduler.CreateObserver<string>();
                var startTime = new DateTimeOffset(2019, 10, 10, 10, 10, 0, TimeSpan.Zero);
                var duration = TimeSpan.FromMinutes(30);
                var calendarItem = new CalendarItem(
                    "",
                    CalendarItemSource.TimeEntry,
                    startTime,
                    duration,
                    "Such description",
                    CalendarIconKind.None,
                    "#c2c2c2",
                    project: "Such Project",
                    task: "Such Task",
                    client: "Such Client");
                ViewModel.TimeEntryPeriod.Subscribe(observer);
                TestScheduler.Start();

                ViewModel.OnCalendarItemUpdated.Execute(calendarItem);
                TestScheduler.Start();

                observer.Messages.Should().HaveCount(1);
                observer.Messages.First().Value.Value
                    .ToLower()
                    .Should()
                    .Be($"{calendarItem.StartTime.ToLocalTime().ToString(Resources.EditingTwelveHoursFormat)} - {calendarItem.EndTime.Value.ToLocalTime().ToString(Resources.EditingTwelveHoursFormat)}".ToLower());
            }

            [Fact]
            public void EmitsWhenItemIsUpdatedWithADifferentStartTime()
            {
                var observer = TestScheduler.CreateObserver<string>();
                var startTime = new DateTimeOffset(2019, 10, 10, 10, 10, 0, TimeSpan.Zero);
                var duration = TimeSpan.FromMinutes(30);
                var calendarItem = new CalendarItem(
                    "",
                    CalendarItemSource.TimeEntry,
                    startTime,
                    duration,
                    "Such description",
                    CalendarIconKind.None,
                    "#c2c2c2",
                    project: "Such Project",
                    task: "Such Task",
                    client: "Such Client");
                ViewModel.TimeEntryPeriod.Subscribe(observer);
                ViewModel.OnCalendarItemUpdated.Execute(calendarItem);
                var newStartTime = startTime.AddHours(1);
                var newCalendarItem = calendarItem.WithStartTime(newStartTime);
                TestScheduler.Start();

                ViewModel.OnCalendarItemUpdated.Execute(newCalendarItem);
                TestScheduler.Start();

                observer.Messages.Should().HaveCount(2);
                observer.Messages.Last().Value.Value
                    .ToLower()
                    .Should()
                    .Be($"{newCalendarItem.StartTime.ToLocalTime().ToString(Resources.EditingTwelveHoursFormat)} - {newCalendarItem.EndTime.Value.ToLocalTime().ToString(Resources.EditingTwelveHoursFormat)}".ToLower());
            }

            [Fact]
            public void EmitsWhenItemIsUpdatedWithADifferentDuration()
            {
                var observer = TestScheduler.CreateObserver<string>();
                var startTime = new DateTimeOffset(2019, 10, 10, 10, 10, 0, TimeSpan.Zero);
                var duration = TimeSpan.FromMinutes(30);
                var calendarItem = new CalendarItem(
                    "",
                    CalendarItemSource.TimeEntry,
                    startTime,
                    duration,
                    "Such description",
                    CalendarIconKind.None,
                    "#c2c2c2",
                    project: "Such Project",
                    task: "Such Task",
                    client: "Such Client");
                ViewModel.TimeEntryPeriod.Subscribe(observer);
                ViewModel.OnCalendarItemUpdated.Execute(calendarItem);
                var newDuration = TimeSpan.FromMinutes(10);
                var newCalendarItem = calendarItem.WithDuration(newDuration);
                TestScheduler.Start();

                ViewModel.OnCalendarItemUpdated.Execute(newCalendarItem);
                TestScheduler.Start();

                observer.Messages.Should().HaveCount(2);
                observer.Messages.Last().Value.Value
                    .ToLower()
                    .Should()
                    .Be($"{newCalendarItem.StartTime.ToLocalTime().ToString(Resources.EditingTwelveHoursFormat)} - {newCalendarItem.EndTime.Value.ToLocalTime().ToString(Resources.EditingTwelveHoursFormat)}".ToLower());
            }

            [Fact]
            public void DoesNotEmitWhenItemIsUpdatedWithSameStartTimeAndDuration()
            {
                var observer = TestScheduler.CreateObserver<string>();
                var startTime = new DateTimeOffset(2019, 10, 10, 10, 10, 0, TimeSpan.Zero);
                var duration = TimeSpan.FromMinutes(30);
                var calendarItem = new CalendarItem(
                    "",
                    CalendarItemSource.TimeEntry,
                    startTime,
                    duration,
                    "Such description",
                    CalendarIconKind.None,
                    "#c2c2c2",
                    project: "Such Project",
                    task: "Such Task",
                    client: "Such Client");
                var newCalendarItem = new CalendarItem(
                    "",
                    CalendarItemSource.TimeEntry,
                    startTime,
                    duration,
                    "New description",
                    CalendarIconKind.None,
                    "#c2c2c2",
                    project: "New Project",
                    task: "New Task",
                    client: "New Client");
                ViewModel.TimeEntryPeriod.Subscribe(observer);
                ViewModel.OnCalendarItemUpdated.Execute(calendarItem);
                TestScheduler.Start();

                ViewModel.OnCalendarItemUpdated.Execute(newCalendarItem);
                TestScheduler.Start();

                observer.Messages.Should().HaveCount(1);
            }

            [Fact]
            public void EmitsEndTimeAsNowWhenItemIsUpdatedWithNullDuration()
            {
                var observer = TestScheduler.CreateObserver<string>();
                var startTime = new DateTimeOffset(2019, 10, 10, 10, 10, 0, TimeSpan.Zero);
                var duration = TimeSpan.FromMinutes(30);
                var calendarItem = new CalendarItem(
                    "",
                    CalendarItemSource.TimeEntry,
                    startTime,
                    duration,
                    "Such description",
                    CalendarIconKind.None,
                    "#c2c2c2",
                    project: "Such Project",
                    task: "Such Task",
                    client: "Such Client");
                ViewModel.TimeEntryPeriod.Subscribe(observer);
                ViewModel.OnCalendarItemUpdated.Execute(calendarItem);

                var newCalendarItem = calendarItem.WithDuration(null);
                TestScheduler.Start();

                ViewModel.OnCalendarItemUpdated.Execute(newCalendarItem);
                TestScheduler.Start();

                observer.Messages.Should().HaveCount(2);
                observer.Messages.Last().Value.Value
                    .ToLower()
                    .Should()
                    .Be($"{newCalendarItem.StartTime.ToLocalTime().ToString(Resources.EditingTwelveHoursFormat)} - {Shared.Resources.Now}".ToLower());
            }
        }
    }
}
