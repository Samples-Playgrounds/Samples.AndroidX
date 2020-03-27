using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using Toggl.Core.Analytics;
using Toggl.Core.Calendar;
using Toggl.Core.Extensions;
using Toggl.Core.Interactors;
using Toggl.Core.Models.Interfaces;
using Toggl.Core.Services;
using Toggl.Core.UI.Extensions;
using Toggl.Core.UI.Navigation;
using Toggl.Core.UI.Parameters;
using Toggl.Core.UI.Views;
using Toggl.Shared;
using Toggl.Shared.Extensions;

namespace Toggl.Core.UI.ViewModels.Calendar.ContextualMenu
{
    public class CalendarContextualMenuViewModel : ViewModel
    {
        private readonly ISubject<CalendarContextualMenu> currentMenuSubject;
        private readonly Dictionary<ContextualMenuType, CalendarContextualMenu> contextualMenus;
        private readonly ISubject<Unit> discardChangesSubject = new Subject<Unit>();
        private readonly BehaviorSubject<bool> menuVisibilitySubject = new BehaviorSubject<bool>(false);
        private readonly ISubject<TimeEntryDisplayInfo> timeEntryInfoSubject = new Subject<TimeEntryDisplayInfo>();
        private readonly ISubject<string> timeEntryPeriodSubject = new Subject<string>();
        private readonly ISubject<CalendarItem?> calendarItemInEditMode = new Subject<CalendarItem?>();
        private readonly ISubject<CalendarItem> calendarItemRemoved = new Subject<CalendarItem>();

        private readonly IInteractorFactory interactorFactory;
        private readonly ISchedulerProvider schedulerProvider;
        private readonly IAnalyticsService analyticsService;
        private readonly IRxActionFactory rxActionFactory;
        private readonly ITimeService timeService;

        private CalendarItem? calendarItemThatOriginallyTriggeredTheMenu = null;
        private CalendarItem currentCalendarItem;
        private TimeEntryDisplayInfo currentTimeEntryDisplayInfo;
        private ContextualMenuType currentMenuType = ContextualMenuType.Closed;
        private DateTimeOffset currentStartTimeOffset;
        private TimeSpan? currentDuration;

        public IObservable<CalendarContextualMenu> CurrentMenu { get; }

        public IObservable<Unit> DiscardChanges { get; }

        public IObservable<bool> MenuVisible { get; }

        public IObservable<TimeEntryDisplayInfo> TimeEntryInfo { get; }

        public IObservable<string> TimeEntryPeriod { get; }

        public IObservable<CalendarItem?> CalendarItemInEditMode { get; }

        public IObservable<CalendarItem> CalendarItemRemoved { get; }

        public InputAction<CalendarItem?> OnCalendarItemUpdated { get; }

        public CalendarContextualMenuViewModel(
            IInteractorFactory interactorFactory,
            ISchedulerProvider schedulerProvider,
            IAnalyticsService analyticsService,
            IRxActionFactory rxActionFactory,
            ITimeService timeService,
            INavigationService navigationService)
            : base(navigationService)
        {
            Ensure.Argument.IsNotNull(interactorFactory, nameof(interactorFactory));
            Ensure.Argument.IsNotNull(schedulerProvider, nameof(schedulerProvider));
            Ensure.Argument.IsNotNull(analyticsService, nameof(analyticsService));
            Ensure.Argument.IsNotNull(rxActionFactory, nameof(rxActionFactory));
            Ensure.Argument.IsNotNull(timeService, nameof(timeService));
            this.interactorFactory = interactorFactory;
            this.schedulerProvider = schedulerProvider;
            this.analyticsService = analyticsService;
            this.rxActionFactory = rxActionFactory;
            this.timeService = timeService;

            OnCalendarItemUpdated = rxActionFactory.FromAsync<CalendarItem?>(handleCalendarItemInput);

            var closedMenu = new CalendarContextualMenu(ContextualMenuType.Closed, ImmutableList<CalendarMenuAction>.Empty, rxActionFactory.FromAction(CommonFunctions.DoNothing));
            contextualMenus = new Dictionary<ContextualMenuType, CalendarContextualMenu>
            {
                { ContextualMenuType.CalendarEvent, setupCalendarEventActions() },
                { ContextualMenuType.RunningTimeEntry, setupRunningTimeEntryActions() },
                { ContextualMenuType.StoppedTimeEntry, setupStoppedTimeEntryActions() },
                { ContextualMenuType.NewTimeEntry, setupNewTimeEntryContextualActions() },
                { ContextualMenuType.Closed, closedMenu }
            };

            currentMenuSubject = new BehaviorSubject<CalendarContextualMenu>(closedMenu);
            CurrentMenu = currentMenuSubject.AsDriver(schedulerProvider);
            DiscardChanges = discardChangesSubject.AsDriver(schedulerProvider);
            TimeEntryInfo = timeEntryInfoSubject.AsDriver(schedulerProvider);
            MenuVisible = menuVisibilitySubject.AsDriver(schedulerProvider);
            TimeEntryPeriod = timeEntryPeriodSubject.AsDriver(schedulerProvider);
            CalendarItemInEditMode = calendarItemInEditMode.AsDriver(schedulerProvider);
            CalendarItemRemoved = calendarItemRemoved.AsDriver(schedulerProvider);
        }

        private async Task handleCalendarItemInput(CalendarItem? calendarItem)
        {
            if (!calendarItem.HasValue)
            {
                if (needsToConfirmDestructiveChangesBeforeClosingMenu())
                {
                    var willCloseMenu = await View.ConfirmDestructiveAction(ActionType.DiscardEditingChanges);
                    if (!willCloseMenu)
                        return;
                }

                closeMenuDismissingUncommittedChanges();
                return;
            }

            var newCalendarItem = calendarItem.Value;
            if (needsToConfirmDestructiveChangesBeforeUpdatingCurrentCalendarItem(newCalendarItem))
            {
                var willUpdateCurrentItem = await View.ConfirmDestructiveAction(ActionType.DiscardEditingChanges);
                if (!willUpdateCurrentItem)
                    return;
                discardChangesSubject.OnNext(Unit.Default);
            }

            updateCalendarItem(newCalendarItem);
        }

        private void updateCalendarItem(CalendarItem newCalendarItem)
        {
            if (!calendarItemThatOriginallyTriggeredTheMenu.HasValue || isADifferentCalendarItem(newCalendarItem))
            {
                calendarItemThatOriginallyTriggeredTheMenu = newCalendarItem;
                calendarItemInEditMode.OnNext(newCalendarItem);
            }

            var newCalendarItemContextualMenuType = selectContextualMenuTypeFrom(newCalendarItem);
            handleMenuUpdate(newCalendarItemContextualMenuType);
            handleCalendarItemUpdate(newCalendarItem);
            currentCalendarItem = newCalendarItem;
        }

        private bool needsToConfirmDestructiveChangesBeforeClosingMenu()
            => contextualMenuIsAlreadyOpen() && changesWereMadeToTheCurrentItem();

        private bool needsToConfirmDestructiveChangesBeforeUpdatingCurrentCalendarItem(CalendarItem newCalendarItem)
        {
            return contextualMenuIsAlreadyOpen()
                   && isADifferentCalendarItem(newCalendarItem)
                   && changesWereMadeToTheCurrentItem();
        }

        private bool changesWereMadeToTheCurrentItem()
        {
            if (!calendarItemThatOriginallyTriggeredTheMenu.HasValue)
                return false;

            if (currentCalendarItem.Source == CalendarItemSource.Calendar)
                return false;

            var originalCalendarItem = calendarItemThatOriginallyTriggeredTheMenu.Value;
            return originalCalendarItem.StartTime != currentCalendarItem.StartTime
                   || originalCalendarItem.Duration != currentCalendarItem.Duration;
        }

        private bool isADifferentCalendarItem(CalendarItem newCalendarItem)
        {
            return newCalendarItem.Source != currentCalendarItem.Source
                   || (newCalendarItem.Source == CalendarItemSource.TimeEntry && newCalendarItem.TimeEntryId != currentCalendarItem.TimeEntryId)
                   || (newCalendarItem.Source == CalendarItemSource.Calendar && newCalendarItem.CalendarId != currentCalendarItem.CalendarId);
        }

        private bool contextualMenuIsAlreadyOpen() => menuVisibilitySubject.Value;

        private void handleMenuUpdate(ContextualMenuType contextualMenuType)
        {
            if (!contextualMenus.TryGetValue(contextualMenuType, out var actions))
                return;

            if (contextualMenuType == currentMenuType)
                return;

            currentMenuType = contextualMenuType;
            currentMenuSubject.OnNext(actions);
            menuVisibilitySubject.OnNext(true);
        }

        private void handleCalendarItemUpdate(CalendarItem calendarItem)
        {
            if (!hasSameTimeEntryDisplayInfoAs(calendarItem))
            {
                currentTimeEntryDisplayInfo = new TimeEntryDisplayInfo(calendarItem);
                timeEntryInfoSubject.OnNext(currentTimeEntryDisplayInfo);
            }

            if (calendarItem.StartTime != currentStartTimeOffset || calendarItem.Duration != currentDuration)
            {
                currentStartTimeOffset = calendarItem.StartTime;
                currentDuration = calendarItem.Duration;
                timeEntryPeriodSubject.OnNext(formatCurrentPeriod());
            }
        }

        private string formatCurrentPeriod()
        {
            var startTimeString = currentStartTimeOffset.ToLocalTime().ToString(Resources.EditingTwelveHoursFormat);
            var endTime = currentStartTimeOffset.ToLocalTime() + currentDuration;
            var endTimeString = endTime.HasValue
                ? endTime.Value.ToString(Resources.EditingTwelveHoursFormat)
                : Resources.Now;

            return $"{startTimeString} - {endTimeString}";
        }

        private void closeMenuDismissingUncommittedChanges()
        {
            currentCalendarItem = default;
            calendarItemThatOriginallyTriggeredTheMenu = null;
            currentMenuType = ContextualMenuType.Closed;
            discardChangesSubject.OnNext(Unit.Default);
            calendarItemInEditMode.OnNext(null);
            currentMenuSubject.OnNext(contextualMenus[ContextualMenuType.Closed]);
            menuVisibilitySubject.OnNext(false);
        }

        private void closeMenuWithCommittedChanges()
        {
            currentCalendarItem = default;
            calendarItemThatOriginallyTriggeredTheMenu = null;
            currentMenuType = ContextualMenuType.Closed;
            calendarItemInEditMode.OnNext(null);
            currentMenuSubject.OnNext(contextualMenus[ContextualMenuType.Closed]);
            menuVisibilitySubject.OnNext(false);
        }

        private CalendarContextualMenu setupCalendarEventActions()
        {
            var actions = ImmutableList.Create(
                createCalendarMenuActionFor(ContextualMenuType.CalendarEvent, CalendarMenuActionKind.Copy, Resources.CalendarCopyEventToTimeEntry,
                    trackThenRunCalendarEventCreationAsync(
                        CalendarTimeEntryCreatedType.CopyFromCalendarEvent,
                        CalendarContextualMenuActionType.CopyAsTimeEntry,
                        createTimeEntryFromCalendarItem)
                    ),
                createCalendarMenuActionFor(ContextualMenuType.CalendarEvent, CalendarMenuActionKind.Start, Resources.Start,
                    trackThenRunCalendarEventCreationAsync(
                        CalendarTimeEntryCreatedType.StartFromCalendarEvent,
                        CalendarContextualMenuActionType.StartFromCalendarEvent,
                        startTimeEntryFromCalendarItem)
                    ));

            return new CalendarContextualMenu(ContextualMenuType.CalendarEvent, actions, trackThenDismiss(analyticsService.CalendarEventContextualMenu));
        }

        private void trackCalendarEventCreation(CalendarItem item, CalendarTimeEntryCreatedType eventCreationType, CalendarContextualMenuActionType menuType)
        {
            var today = timeService.CurrentDateTime;
            var daysSinceToday = (int)(item.StartTime - today).TotalDays;
            var dayOfTheWeek = item.StartTime.DayOfWeek;
            analyticsService.CalendarEventContextualMenu.Track(menuType);
            analyticsService.CalendarTimeEntryCreated.Track(eventCreationType, daysSinceToday, dayOfTheWeek.ToString());
        }

        private void trackLongPressCreation(CalendarItem item, CalendarTimeEntryCreatedType eventCreationType, CalendarContextualMenuActionType menuType)
        {
            var today = timeService.CurrentDateTime;
            var daysSinceToday = (int)(item.StartTime - today).TotalDays;
            var dayOfTheWeek = item.StartTime.DayOfWeek;
            analyticsService.CalendarNewTimeEntryContextualMenu.Track(menuType);
            analyticsService.CalendarTimeEntryCreated.Track(eventCreationType, daysSinceToday, dayOfTheWeek.ToString());
        }

        private async Task createTimeEntryFromCalendarItem(CalendarItem calendarItem)
        {
            var workspace = await interactorFactory.GetDefaultWorkspace()
                .TrackException<InvalidOperationException, IThreadSafeWorkspace>("CalendarContextualMenuViewModel.createTimeEntryFromCalendarItem")
                .Execute();
            var prototype = calendarItem.AsTimeEntryPrototype(workspace.Id);
            await interactorFactory.CreateTimeEntry(prototype, TimeEntryStartOrigin.CalendarEvent).Execute();
            closeMenuWithCommittedChanges();
        }

        private async Task startTimeEntryFromCalendarItem(CalendarItem calendarItem)
        {
            var timeEntryToStart = calendarItem
                    .WithStartTime(timeService.CurrentDateTime)
                    .WithDuration(null);

            var workspace = await interactorFactory.GetDefaultWorkspace()
                .TrackException<InvalidOperationException, IThreadSafeWorkspace>("CalendarContextualMenuViewModel.startTimeEntryFromCalendarItem")
                .Execute();
            var prototype = timeEntryToStart.AsTimeEntryPrototype(workspace.Id);
            await interactorFactory.CreateTimeEntry(prototype, TimeEntryStartOrigin.CalendarEvent).Execute();
            closeMenuWithCommittedChanges();
        }

        private CalendarContextualMenu setupRunningTimeEntryActions()
        {
            var analyticsEvent = analyticsService.CalendarRunningTimeEntryContextualMenu;
            var actions = ImmutableList.Create(
                createCalendarMenuDiscardAction(ContextualMenuType.RunningTimeEntry, trackThenRunAsync(analyticsEvent, CalendarContextualMenuActionType.Discard, deleteTimeEntry)),
                createCalendarMenuEditAction(ContextualMenuType.RunningTimeEntry, trackThenRunAsync(analyticsEvent, CalendarContextualMenuActionType.Edit, editTimeEntry)),
                createCalendarMenuSaveAction(ContextualMenuType.RunningTimeEntry, trackThenRunAsync(analyticsEvent, CalendarContextualMenuActionType.Save, saveTimeEntry)),
                createCalendarMenuActionFor(ContextualMenuType.RunningTimeEntry, CalendarMenuActionKind.Stop, Resources.Stop, trackThenRunAsync(analyticsEvent, CalendarContextualMenuActionType.Stop, stopTimeEntry))
            );

            return new CalendarContextualMenu(ContextualMenuType.RunningTimeEntry, actions, trackThenDismiss(analyticsEvent));
        }

        private async Task deleteTimeEntry(CalendarItem calendarItem)
        {
            if (!calendarItem.TimeEntryId.HasValue)
                return;

            calendarItemRemoved.OnNext(calendarItem);
            await interactorFactory.DeleteTimeEntry(calendarItem.TimeEntryId.Value).Execute();
            closeMenuWithCommittedChanges();
        }

        private async Task editTimeEntry(CalendarItem calendarItem)
        {
            if (!calendarItem.TimeEntryId.HasValue)
                return;

            analyticsService.EditViewOpenedFromCalendar.Track();
            await Navigate<EditTimeEntryViewModel, long[]>(new[] { calendarItem.TimeEntryId.Value });
            closeMenuWithCommittedChanges();
        }

        private async Task saveTimeEntry(CalendarItem calendarItem)
        {
            if (!calendarItem.TimeEntryId.HasValue)
                return;

            var timeEntry = await interactorFactory.GetTimeEntryById(calendarItem.TimeEntryId.Value).Execute();

            var dto = new DTOs.EditTimeEntryDto
            {
                Id = timeEntry.Id,
                Description = timeEntry.Description,
                StartTime = calendarItem.StartTime,
                StopTime = calendarItem.Duration.HasValue ? calendarItem.EndTime : timeEntry.StopTime(),
                ProjectId = timeEntry.ProjectId,
                TaskId = timeEntry.TaskId,
                Billable = timeEntry.Billable,
                WorkspaceId = timeEntry.WorkspaceId,
                TagIds = timeEntry.TagIds
            };

            await interactorFactory.UpdateTimeEntry(dto).Execute();
            closeMenuWithCommittedChanges();
        }

        private async Task stopTimeEntry(CalendarItem calendarItem)
        {
            var currentDateTime = timeService.CurrentDateTime;
            await interactorFactory.StopTimeEntry(currentDateTime, TimeEntryStopOrigin.CalendarContextualMenu).Execute();

            closeMenuWithCommittedChanges();
        }

        private CalendarContextualMenu setupStoppedTimeEntryActions()
        {
            var analyticsEvent = analyticsService.CalendarExistingTimeEntryContextualMenu;
            var actions = ImmutableList.Create(
                createCalendarMenuActionFor(ContextualMenuType.StoppedTimeEntry, CalendarMenuActionKind.Delete, Resources.Delete, trackThenRunAsync(analyticsEvent, CalendarContextualMenuActionType.Delete, deleteTimeEntry)),
                createCalendarMenuEditAction(ContextualMenuType.StoppedTimeEntry, trackThenRunAsync(analyticsEvent, CalendarContextualMenuActionType.Edit, editTimeEntry)),
                createCalendarMenuSaveAction(ContextualMenuType.StoppedTimeEntry, trackThenRunAsync(analyticsEvent, CalendarContextualMenuActionType.Save, saveTimeEntry)),
                createCalendarMenuActionFor(ContextualMenuType.StoppedTimeEntry, CalendarMenuActionKind.Continue, Resources.Continue, trackThenRunAsync(analyticsEvent, CalendarContextualMenuActionType.Continue, continueTimeEntry))
            );

            return new CalendarContextualMenu(ContextualMenuType.StoppedTimeEntry, actions, trackThenDismiss(analyticsEvent));
        }

        private async Task continueTimeEntry(CalendarItem calendarItem)
        {
            if (!calendarItem.TimeEntryId.HasValue)
                return;

            await interactorFactory
                .ContinueTimeEntry(calendarItem.TimeEntryId.Value, ContinueTimeEntryMode.CalendarContextualMenu)
                .Execute();

            closeMenuWithCommittedChanges();
        }

        private CalendarContextualMenu setupNewTimeEntryContextualActions()
        {
            var analyticsEvent = analyticsService.CalendarNewTimeEntryContextualMenu;
            var actions = ImmutableList.Create(
                createCalendarMenuDiscardAction(ContextualMenuType.NewTimeEntry, trackThenRun(analyticsEvent, CalendarContextualMenuActionType.Discard, discardCurrentItemInEditMode)),
                createCalendarMenuEditAction(ContextualMenuType.NewTimeEntry, trackThenRunAsync(analyticsEvent, CalendarContextualMenuActionType.Edit, startTimeEntryFrom)),
                createCalendarMenuSaveAction(ContextualMenuType.NewTimeEntry, trackThenRunManualCreationAsync(CalendarTimeEntryCreatedType.LongPress, CalendarContextualMenuActionType.Save, createTimeEntryFromCalendarItem))
            );

            return new CalendarContextualMenu(ContextualMenuType.NewTimeEntry, actions, trackThenDismiss(analyticsEvent));
        }

        private async Task startTimeEntryFrom(CalendarItem calendarItem)
        {
            var workspace = await interactorFactory.GetDefaultWorkspace()
                .TrackException<InvalidOperationException, IThreadSafeWorkspace>("CalendarContextualMenuViewModel.startTimeEntryFromCalendarItem")
                .Execute();

            var timeEntryParams = new StartTimeEntryParameters(
                calendarItem.StartTime,
                string.Empty,
                calendarItem.Duration,
                workspace.Id);

            await Navigate<StartTimeEntryViewModel, StartTimeEntryParameters>(timeEntryParams);
            closeMenuWithCommittedChanges();
        }

        private void discardCurrentItemInEditMode(CalendarItem calendarItem)
        {
            closeMenuDismissingUncommittedChanges();
        }

        private ContextualMenuType selectContextualMenuTypeFrom(CalendarItem calendarItem)
        {
            if (calendarItemIsANewTimeEntry(calendarItem))
                return ContextualMenuType.NewTimeEntry;

            if (calendarItemIsFromAnExistingTimeEntry(calendarItem))
            {
                return calendarItem.Duration.HasValue
                    ? ContextualMenuType.StoppedTimeEntry
                    : ContextualMenuType.RunningTimeEntry;
            }

            return ContextualMenuType.CalendarEvent;
        }

        private bool calendarItemIsFromAnExistingTimeEntry(CalendarItem calendarItem)
            => calendarItem.Source == CalendarItemSource.TimeEntry
               && !string.IsNullOrEmpty(calendarItem.Id)
               && calendarItem.TimeEntryId.HasValue;

        private bool calendarItemIsANewTimeEntry(CalendarItem calendarItem)
            => string.IsNullOrEmpty(calendarItem.Id);

        private bool hasSameTimeEntryDisplayInfoAs(CalendarItem calendarItem)
            => calendarItem.Description == currentTimeEntryDisplayInfo.Description
               && calendarItem.Project == currentTimeEntryDisplayInfo.Project
               && calendarItem.Task == currentTimeEntryDisplayInfo.Task
               && calendarItem.Color == currentTimeEntryDisplayInfo.ProjectTaskColor;

        private ViewAction trackThenRun(IAnalyticsEvent<CalendarContextualMenuActionType> analyticsEvent, CalendarContextualMenuActionType eventValue, Action<CalendarItem> action)
            => rxActionFactory.FromAction(() =>
            {
                analyticsEvent.Track(eventValue);
                action(currentCalendarItem);
            });

        private ViewAction trackThenRunAsync(IAnalyticsEvent<CalendarContextualMenuActionType> analyticsEvent, CalendarContextualMenuActionType eventValue, Func<CalendarItem, Task> action)
            => rxActionFactory.FromAsync(() =>
            {
                analyticsEvent.Track(eventValue);
                return action(currentCalendarItem);
            });

        private ViewAction trackThenRunCalendarEventCreationAsync(CalendarTimeEntryCreatedType eventCreationType, CalendarContextualMenuActionType menuType, Func<CalendarItem, Task> action)
            => rxActionFactory.FromAsync(() =>
            {
                trackCalendarEventCreation(currentCalendarItem, eventCreationType, menuType);
                return action(currentCalendarItem);
            });

        private ViewAction trackThenRunManualCreationAsync(CalendarTimeEntryCreatedType eventCreationType, CalendarContextualMenuActionType menuType, Func<CalendarItem, Task> action)
            => rxActionFactory.FromAsync(() =>
            {
                trackLongPressCreation(currentCalendarItem, eventCreationType, menuType);
                return action(currentCalendarItem);
            });

        private ViewAction trackThenDismiss(IAnalyticsEvent<CalendarContextualMenuActionType> analyticsEvent)
            => rxActionFactory.FromAsync(() =>
            {
                analyticsEvent.Track(CalendarContextualMenuActionType.Dismiss);
                return confirmThenCloseMenuDismissingUncommittedChanges();
            });

        private async Task confirmThenCloseMenuDismissingUncommittedChanges()
        {
            if (!changesWereMadeToTheCurrentItem() || await View.ConfirmDestructiveAction(ActionType.DiscardEditingChanges))
                closeMenuDismissingUncommittedChanges();
        }

        private CalendarMenuAction createCalendarMenuActionFor(ContextualMenuType sourceMenuType, CalendarMenuActionKind calendarMenuActionKind, string title, ViewAction action)
            => new CalendarMenuAction(sourceMenuType, calendarMenuActionKind, title, action);

        private CalendarMenuAction createCalendarMenuSaveAction(ContextualMenuType sourceMenuType, ViewAction action)
            => createCalendarMenuActionFor(sourceMenuType, CalendarMenuActionKind.Save, Resources.Save, action);

        private CalendarMenuAction createCalendarMenuEditAction(ContextualMenuType sourceMenuType, ViewAction action)
            => createCalendarMenuActionFor(sourceMenuType, CalendarMenuActionKind.Edit, Resources.Edit, action);

        private CalendarMenuAction createCalendarMenuDiscardAction(ContextualMenuType sourceMenuType, ViewAction action)
            => createCalendarMenuActionFor(sourceMenuType, CalendarMenuActionKind.Discard, Resources.Discard, action);
    }
}
