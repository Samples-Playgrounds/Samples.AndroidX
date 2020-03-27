using FluentAssertions;
using FsCheck;
using FsCheck.Xunit;
using Microsoft.Reactive.Testing;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using Toggl.Core.Analytics;
using Toggl.Core.DTOs;
using Toggl.Core.Models.Interfaces;
using Toggl.Core.Sync;
using Toggl.Core.Tests.Generators;
using Toggl.Core.Tests.Mocks;
using Toggl.Core.Tests.TestExtensions;
using Toggl.Core.UI.Navigation;
using Toggl.Core.UI.Parameters;
using Toggl.Core.UI.ViewModels;
using Toggl.Core.UI.ViewModels.Settings;
using Toggl.Core.UI.Views;
using Toggl.Shared;
using Xunit;

namespace Toggl.Core.Tests.UI.ViewModels
{
    public sealed class SettingsViewModelTests
    {
        public abstract class SettingsViewModelTest : BaseViewModelTests<SettingsViewModel>
        {
            protected ISubject<IThreadSafeUser> UserSubject { get; set; }
            protected ISubject<SyncProgress> ProgressSubject { get; set; }
            protected ISubject<IThreadSafePreferences> PreferencesSubject { get; set; }

            protected override SettingsViewModel CreateViewModel()
            {
                UserSubject = new Subject<IThreadSafeUser>();
                ProgressSubject = new Subject<SyncProgress>();
                PreferencesSubject = new Subject<IThreadSafePreferences>();

                DataSource.User.Current.Returns(UserSubject.AsObservable());
                DataSource.Preferences.Current.Returns(PreferencesSubject.AsObservable());
                SyncManager.ProgressObservable.Returns(ProgressSubject.AsObservable());

                UserSubject.OnNext(new MockUser());
                PreferencesSubject.OnNext(new MockPreferences());

                SetupObservables();

                return new SettingsViewModel(
                    DataSource,
                    SyncManager,
                    PlatformInfo,
                    UserPreferences,
                    AnalyticsService,
                    InteractorFactory,
                    NavigationService,
                    RxActionFactory,
                    PermissionsChecker,
                    SchedulerProvider);
            }

            protected virtual void SetupObservables()
            {
            }
        }

        public sealed class TheConstructor : SettingsViewModelTest
        {
            [Theory, LogIfTooSlow]
            [ConstructorData]
            public void ThrowsIfAnyOfTheArgumentsIsNull(
                bool useDataSource,
                bool useSyncManager,
                bool useUserPreferences,
                bool useAnalyticsService,
                bool useInteractorFactory,
                bool useplatformInfo,
                bool useNavigationService,
                bool useRxActionFactory,
                bool usePermissionsChecker,
                bool useSchedulerProvider)
            {
                var dataSource = useDataSource ? DataSource : null;
                var syncManager = useSyncManager ? SyncManager : null;
                var platformInfo = useplatformInfo ? PlatformInfo : null;
                var userPreferences = useUserPreferences ? UserPreferences : null;
                var analyticsService = useAnalyticsService ? AnalyticsService : null;
                var navigationService = useNavigationService ? NavigationService : null;
                var interactorFactory = useInteractorFactory ? InteractorFactory : null;
                var rxActionFactory = useRxActionFactory ? RxActionFactory : null;
                var permissionsService = usePermissionsChecker ? PermissionsChecker : null;
                var schedulerProvider = useSchedulerProvider ? SchedulerProvider : null;

                Action tryingToConstructWithEmptyParameters =
                    () => new SettingsViewModel(
                        dataSource,
                        syncManager,
                        platformInfo,
                        userPreferences,
                        analyticsService,
                        interactorFactory,
                        navigationService,
                        rxActionFactory,
                        permissionsService,
                        schedulerProvider);

                tryingToConstructWithEmptyParameters
                    .Should().Throw<ArgumentNullException>();
            }
        }

        public sealed class TheFlags : SettingsViewModelTest
        {
            [Property]
            public void DoesNotEverSetBothIsRunningSyncAndIsSyncedBothToTrue(NonEmptyArray<SyncProgress> statuses)
            {
                DataSource.HasUnsyncedData().Returns(Observable.Return(false));
                var syncedObserver = TestScheduler.CreateObserver<bool>();
                var syncingObserver = TestScheduler.CreateObserver<bool>();
                var viewModel = CreateViewModel();
                viewModel.IsSynced.Subscribe(syncedObserver);
                viewModel.IsRunningSync.Subscribe(syncingObserver);

                foreach (var state in statuses.Get)
                {
                    syncedObserver.Messages.Clear();
                    syncingObserver.Messages.Clear();

                    ProgressSubject.OnNext(state);
                    TestScheduler.Start();
                    TestScheduler.Stop();

                    var isSynced = syncedObserver.SingleEmittedValue();
                    var isRunningSync = syncingObserver.SingleEmittedValue();

                    (isRunningSync && isSynced).Should().BeFalse();
                }
            }

            [Property]
            public void EmitTheAppropriateIsRunningSyncValues(NonEmptyArray<SyncProgress> statuses)
            {
                DataSource.HasUnsyncedData().Returns(Observable.Return(false));
                var observer = TestScheduler.CreateObserver<bool>();
                var viewModel = CreateViewModel();

                viewModel.IsRunningSync.Subscribe(observer);

                foreach (var state in statuses.Get)
                {
                    observer.Messages.Clear();

                    ProgressSubject.OnNext(state);
                    TestScheduler.Start();
                    TestScheduler.Stop();

                    var isRunningSync = observer.SingleEmittedValue();
                    isRunningSync.Should().Be(state == SyncProgress.Syncing);
                }
            }

            [Property]
            public void EmitTheAppropriateIsSyncedValues(NonEmptyArray<SyncProgress> statuses)
            {
                var observer = TestScheduler.CreateObserver<bool>();
                var viewModel = CreateViewModel();

                viewModel.IsSynced.Subscribe(observer);

                foreach (var state in statuses.Get)
                {
                    if (state == SyncProgress.Unknown)
                        continue;

                    observer.Messages.Clear();

                    ProgressSubject.OnNext(state);
                    TestScheduler.Start();
                    TestScheduler.Stop();

                    var isSynced = observer.SingleEmittedValue();
                    isSynced.Should().Be(state == SyncProgress.Synced);
                }
            }
        }

        public sealed class TheEmailObservable : SettingsViewModelTest
        {
            [Fact, LogIfTooSlow]
            public void EmitsWheneverTheUserEmailChanges()
            {
                var observer = TestScheduler.CreateObserver<string>();
                ViewModel.Email.Subscribe(observer);

                UserSubject.OnNext(new MockUser { Email = Email.From("newmail@mail.com") });
                UserSubject.OnNext(new MockUser { Email = Email.From("newmail@mail.com") });
                UserSubject.OnNext(new MockUser { Email = Email.From("differentmail@mail.com") });
                TestScheduler.Start();

                observer.Messages.Count.Should().Be(2);
            }
        }

        public sealed class TheTryLogoutMethod : SettingsViewModelTest
        {
            [Fact, LogIfTooSlow]
            public async Task EmitsOneIsLoggingOutEvent()
            {
                var observer = TestScheduler.CreateObserver<Unit>();
                ViewModel.LoggingOut.Subscribe(observer);

                doNotShowConfirmationDialog();

                ViewModel.TryLogout.Execute();
                TestScheduler.Start();

                observer.Messages.Single();
            }

            [Fact, LogIfTooSlow]
            public async Task ExecutesTheLogoutInteractor()
            {
                doNotShowConfirmationDialog();

                TestScheduler.Start();
                ViewModel.TryLogout.Execute();

                await InteractorFactory.Received().Logout(LogoutSource.Settings).Execute();
            }

            [Fact, LogIfTooSlow]
            public async Task NavigatesToTheLoginScreen()
            {
                doNotShowConfirmationDialog();

                TestScheduler.Start();
                ViewModel.TryLogout.Execute();

                await NavigationService.Received()
                    .Navigate<LoginViewModel, CredentialsParameter>(Arg.Any<CredentialsParameter>(), View);
            }

            [Fact, LogIfTooSlow]
            public async Task ChecksIfThereAreUnsyncedDataWhenTheSyncProcessFinishes()
            {
                ProgressSubject.OnNext(SyncProgress.Synced);

                TestScheduler.Start();
                ViewModel.TryLogout.Execute();

                await DataSource.Received().HasUnsyncedData();
            }

            [Fact, LogIfTooSlow]
            public async Task DoesNotShowConfirmationDialogWhenTheAppIsInSync()
            {
                doNotShowConfirmationDialog();

                TestScheduler.Start();
                ViewModel.TryLogout.Execute();

                await View.DidNotReceiveWithAnyArgs().Confirm("", "", "", "");
            }

            [Fact, LogIfTooSlow]
            public async Task ShowsConfirmationDialogWhenThereIsNothingToPushButSyncIsRunning()
            {
                DataSource.HasUnsyncedData().Returns(Observable.Return(false));
                ProgressSubject.OnNext(SyncProgress.Syncing);

                TestScheduler.Start();
                ViewModel.TryLogout.Execute();

                await View.ReceivedWithAnyArgs().Confirm("", "", "", "");
            }

            [Fact, LogIfTooSlow]
            public async Task ShowsConfirmationDialogWhenThereIsSomethingToPushButSyncIsNotRunning()
            {
                DataSource.HasUnsyncedData().Returns(Observable.Return(true));
                ProgressSubject.OnNext(SyncProgress.Syncing);

                TestScheduler.Start();
                ViewModel.TryLogout.Execute();

                await View.ReceivedWithAnyArgs().Confirm("", "", "", "");
            }

            [Fact, LogIfTooSlow]
            public async Task DoesNotProceedWithLogoutWhenUserClicksCancelButtonInTheDialog()
            {
                ProgressSubject.OnNext(SyncProgress.Syncing);
                View.Confirm(
                    Arg.Any<string>(),
                    Arg.Any<string>(),
                    Arg.Any<string>(),
                    Arg.Any<string>()).Returns(Observable.Return(false));

                TestScheduler.Start();
                ViewModel.TryLogout.Execute();

                InteractorFactory.DidNotReceive().Logout(Arg.Any<LogoutSource>());
                await NavigationService.DidNotReceive()
                    .Navigate<LoginViewModel, CredentialsParameter>(Arg.Any<CredentialsParameter>(), View);
            }

            [Fact, LogIfTooSlow]
            public async Task ProceedsWithLogoutWhenUserClicksSignOutButtonInTheDialog()
            {
                ProgressSubject.OnNext(SyncProgress.Syncing);
                View.Confirm(
                    Arg.Any<string>(),
                    Arg.Any<string>(),
                    Arg.Any<string>(),
                    Arg.Any<string>()).Returns(Observable.Return(true));

                TestScheduler.Start();
                ViewModel.TryLogout.Execute();

                await InteractorFactory.Received().Logout(LogoutSource.Settings).Execute();
                await NavigationService.Received()
                    .Navigate<LoginViewModel, CredentialsParameter>(Arg.Any<CredentialsParameter>(), View);
            }

            private void doNotShowConfirmationDialog()
            {
                DataSource.HasUnsyncedData().Returns(Observable.Return(false));
                ProgressSubject.OnNext(SyncProgress.Synced);
            }
        }

        public sealed class ThePickDefaultWorkspaceMethod : SettingsViewModelTest
        {
            private const long workspaceId = 10;
            private const long defaultWorkspaceId = 11;
            private const string workspaceName = "My custom workspace";
            private readonly IThreadSafeWorkspace workspace;
            private readonly IThreadSafeWorkspace defaultWorkspace = Substitute.For<IThreadSafeWorkspace>();

            protected override void SetupObservables()
            {
                DataSource.User.Current.Returns(Observable.Return(new MockUser()));
            }

            public ThePickDefaultWorkspaceMethod()
            {
                defaultWorkspace = new MockWorkspace { Id = defaultWorkspaceId };
                workspace = new MockWorkspace { Id = workspaceId, Name = workspaceName };

                UserSubject.OnNext(new MockUser());

                InteractorFactory.GetDefaultWorkspace().Execute()
                    .Returns(Observable.Return(defaultWorkspace));

                InteractorFactory.GetWorkspaceById(workspaceId).Execute()
                    .Returns(Observable.Return(workspace));

                ViewModel.Initialize();
            }

            [Fact, LogIfTooSlow]
            public async Task CallsTheSelectModal()
            {
                ViewModel.PickDefaultWorkspace.Execute();
                TestScheduler.Start();

                await View.Received().Select(
                    Arg.Any<string>(),
                    Arg.Any<IEnumerable<SelectOption<IThreadSafeWorkspace>>>(),
                    Arg.Any<int>());
            }

            [Fact, LogIfTooSlow]
            public async Task UpdatesTheUserWithTheReceivedWorspace()
            {
                View.Select(
                    Arg.Any<string>(),
                    Arg.Any<IEnumerable<SelectOption<IThreadSafeWorkspace>>>(),
                    Arg.Any<int>())
                .Returns(Observable.Return(new MockWorkspace { Id = workspaceId }));

                ViewModel.PickDefaultWorkspace.Execute();
                TestScheduler.Start();

                await InteractorFactory
                    .Received()
                    .UpdateDefaultWorkspace(Arg.Is(workspaceId))
                    .Execute();
            }

            [Fact, LogIfTooSlow]
            public async Task StartsTheSyncAlgorithm()
            {
                View.Select(
                    Arg.Any<string>(),
                    Arg.Any<IEnumerable<SelectOption<IThreadSafeWorkspace>>>(),
                    Arg.Any<int>())
                .Returns(Observable.Return(new MockWorkspace { Id = workspaceId }));

                ViewModel.PickDefaultWorkspace.Execute();
                TestScheduler.Start();

                SyncManager.Received().PushSync();
            }
        }

        public sealed class TheSwipeActionsToggle : SettingsViewModelTest
        {
            public TheSwipeActionsToggle()
            {
                PreferencesSubject.OnNext(new MockPreferences());
            }

            [Fact, LogIfTooSlow]
            public void TurnsOffSwipeActionsWhenTheyAreOn()
            {
                UserPreferences.AreSwipeActionsEnabled.Returns(true);

                ViewModel.ToggleSwipeActions.Execute();
                TestScheduler.Start();

                UserPreferences.Received().SetSwipeActionsEnabled(false);
            }

            [Fact, LogIfTooSlow]
            public void TurnsOnSwipeActionsWhenTheyAreOff()
            {
                UserPreferences.AreSwipeActionsEnabled.Returns(false);

                ViewModel.ToggleSwipeActions.Execute();
                TestScheduler.Start();

                UserPreferences.Received().SetSwipeActionsEnabled(true);
            }
        }

        public sealed class TheToggleManualModeMethod : SettingsViewModelTest
        {
            public TheToggleManualModeMethod()
            {
                PreferencesSubject.OnNext(new MockPreferences());
            }

            [Fact, LogIfTooSlow]
            public void CallsEnableTimerModeIfCurrentlyInManualMode()
            {
                UserPreferences.IsManualModeEnabled.Returns(true);

                ViewModel.ToggleManualMode.Execute();
                TestScheduler.Start();

                UserPreferences.Received().EnableTimerMode();
            }

            [Fact, LogIfTooSlow]
            public void CallsEnableManualModeIfCurrentlyInTimerMode()
            {
                UserPreferences.IsManualModeEnabled.Returns(false);

                ViewModel.ToggleManualMode.Execute();
                TestScheduler.Start();

                UserPreferences.Received().EnableManualMode();
            }
        }

        public sealed class TheVersionProperty : SettingsViewModelTest
        {
            [Fact, LogIfTooSlow]
            public void ShouldBeConstructedFromVersionAndBuildNumber()
            {
                const string version = "1.0";
                PlatformInfo.Version.Returns(version);

                ViewModel.Version.Should().Be($"{version} ({PlatformInfo.BuildNumber})");
            }
        }

        public sealed class TheToggleTimeEntriesGroupingAction : SettingsViewModelTest
        {
            [Fact, LogIfTooSlow]
            public async Task UpdatesTheStoredPreferences()
            {
                var oldValue = false;
                var newValue = true;
                var preferences = new MockPreferences { CollapseTimeEntries = oldValue };
                PreferencesSubject.OnNext(preferences);

                ViewModel.ToggleTimeEntriesGrouping.Execute();
                TestScheduler.Start();

                await InteractorFactory
                    .Received()
                    .UpdatePreferences(Arg.Is<EditPreferencesDTO>(dto => dto.CollapseTimeEntries.Equals(New<bool>.Value(newValue))))
                    .Execute();
            }

            [Fact, LogIfTooSlow]
            public async Task UpdatesTheCollapseTimeEntriesProperty()
            {
                var oldValue = false;
                var newValue = true;
                var oldPreferences = new MockPreferences { CollapseTimeEntries = oldValue };
                var newPreferences = new MockPreferences { CollapseTimeEntries = newValue };
                PreferencesSubject.OnNext(oldPreferences);
                InteractorFactory.UpdatePreferences(Arg.Any<EditPreferencesDTO>())
                    .Execute()
                    .Returns(Observable.Return(newPreferences));

                ViewModel.ToggleTimeEntriesGrouping.Execute();
                TestScheduler.Start();

                await InteractorFactory
                    .Received()
                    .UpdatePreferences(Arg.Is<EditPreferencesDTO>(dto => dto.CollapseTimeEntries.ValueOr(oldValue) == newValue))
                    .Execute();
            }

            [Fact, LogIfTooSlow]
            public async Task InitiatesPushSync()
            {
                var oldValue = false;
                var preferences = new MockPreferences { CollapseTimeEntries = oldValue };
                PreferencesSubject.OnNext(preferences);

                ViewModel.ToggleTimeEntriesGrouping.Execute();
                TestScheduler.Start();

                await SyncManager.Received().PushSync();
            }

            [Property, LogIfTooSlow]
            public void TracksEventWhenToggled(bool initialState)
            {
                var analyticsEvent = Substitute.For<IAnalyticsEvent<bool>>();
                AnalyticsService.GroupTimeEntriesSettingsChanged.Returns(analyticsEvent);
                var preferences = new MockPreferences { CollapseTimeEntries = initialState };
                var expectedState = !initialState;

                PreferencesSubject.OnNext(preferences);
                ViewModel.ToggleTimeEntriesGrouping.Execute();
                TestScheduler.Start();

                analyticsEvent.Received().Track(expectedState);
            }
        }

        public sealed class TheSelectDateFormatMethod : SettingsViewModelTest
        {
            [Fact, LogIfTooSlow]
            public async Task ShowsASelectDialogPassingTheCurrentDateFormat()
            {
                var dateFormat = DateFormat.FromLocalizedDateFormat("MM-DD-YYYY");
                var preferences = new MockPreferences { DateFormat = dateFormat };
                PreferencesSubject.OnNext(preferences);

                ViewModel.SelectDateFormat.Execute();
                TestScheduler.Start();

                await View
                    .Received()
                    .Select(
                        Arg.Any<string>(),
                        Arg.Any<IEnumerable<SelectOption<DateFormat>>>(),
                        Arg.Is(2));
            }

            [Fact, LogIfTooSlow]
            public async Task UpdatesTheStoredPreferences()
            {
                var oldDateFormat = DateFormat.FromLocalizedDateFormat("MM-DD-YYYY");
                var newDateFormat = DateFormat.FromLocalizedDateFormat("DD.MM.YYYY");
                var preferences = new MockPreferences { DateFormat = oldDateFormat };
                PreferencesSubject.OnNext(preferences);
                View.Select(
                    Arg.Any<string>(),
                    Arg.Any<IEnumerable<SelectOption<DateFormat>>>(),
                    Arg.Any<int>())
                .Returns(Observable.Return(newDateFormat));
                    
                ViewModel.SelectDateFormat.Execute();
                TestScheduler.Start();

                await InteractorFactory
                    .Received()
                    .UpdatePreferences(Arg.Is<EditPreferencesDTO>(dto => dto.DateFormat.Equals(New<DateFormat>.Value(newDateFormat))))
                    .Execute();
            }

            [Fact, LogIfTooSlow]
            public async Task UpdatesTheDateFormatProperty()
            {
                var oldDateFormat = DateFormat.FromLocalizedDateFormat("MM-DD-YYYY");
                var newDateFormat = DateFormat.FromLocalizedDateFormat("DD.MM.YYYY");
                var oldPreferences = new MockPreferences { DateFormat = oldDateFormat };
                var newPreferences = new MockPreferences { DateFormat = newDateFormat };
                PreferencesSubject.OnNext(oldPreferences);
                View.Select(
                    Arg.Any<string>(),
                    Arg.Any<IEnumerable<SelectOption<DateFormat>>>(),
                    Arg.Any<int>())
                .Returns(Observable.Return(newDateFormat));
                InteractorFactory.UpdatePreferences(Arg.Any<EditPreferencesDTO>())
                    .Execute()
                    .Returns(Observable.Return(newPreferences));

                ViewModel.SelectDateFormat.Execute();
                TestScheduler.Start();

                await InteractorFactory
                    .Received()
                    .UpdatePreferences(Arg.Is<EditPreferencesDTO>(dto => dto.DateFormat.ValueOr(oldDateFormat) == newDateFormat))
                    .Execute();
            }

            [Fact, LogIfTooSlow]
            public async Task InitiatesPushSync()
            {
                var oldDateFormat = DateFormat.FromLocalizedDateFormat("MM-DD-YYYY");
                var newDateFormat = DateFormat.FromLocalizedDateFormat("DD.MM.YYYY");
                var preferences = new MockPreferences { DateFormat = oldDateFormat };
                PreferencesSubject.OnNext(preferences);
                View.Select(
                    Arg.Any<string>(),
                    Arg.Any<IEnumerable<SelectOption<DateFormat>>>(),
                    Arg.Any<int>())
                .Returns(Observable.Return(newDateFormat));

                ViewModel.SelectDateFormat.Execute();
                TestScheduler.Start();

                SyncManager.Received().PushSync();
            }
        }

        public sealed class TheToggleUseTwentyFourHourClockMethod : SettingsViewModelTest
        {
            [Theory, LogIfTooSlow]
            [InlineData(true)]
            [InlineData(false)]
            public async Task ChangesTheValueOfTheUseTwentyFourHourHourClock(bool originalValue)
            {
                var timeFormat = originalValue ? TimeFormat.TwentyFourHoursFormat : TimeFormat.TwelveHoursFormat;
                PreferencesSubject.OnNext(new MockPreferences { TimeOfDayFormat = timeFormat });

                ViewModel.ToggleTwentyFourHourSettings.Execute();
                TestScheduler.Start();

                await InteractorFactory
                    .Received()
                    .UpdatePreferences(Arg.Is<EditPreferencesDTO>(
                        dto => dto.TimeOfDayFormat.ValueOr(default(TimeFormat)).IsTwentyFourHoursFormat != originalValue)
                    ).Execute();
            }

            [Fact, LogIfTooSlow]
            public async Task InitiatesPushSync()
            {
                var preferences = new MockPreferences();
                PreferencesSubject.OnNext(preferences);
                var observable = Observable.Return(preferences);
                InteractorFactory.UpdatePreferences(Arg.Any<EditPreferencesDTO>()).Execute().Returns(observable);

                ViewModel.ToggleTwentyFourHourSettings.Execute();
                TestScheduler.Start();

                SyncManager.Received().PushSync();
            }
        }

        public sealed class TheSelectDurationFormatMethod : SettingsViewModelTest
        {
            [Fact, LogIfTooSlow]
            public async Task NavigatesToSelectDurationFormatViewModelPassingCurrentDurationFormat()
            {
                var durationFormat = DurationFormat.Improved;
                var preferences = new MockPreferences { DurationFormat = durationFormat };
                PreferencesSubject.OnNext(preferences);

                ViewModel.SelectDurationFormat.Execute();
                TestScheduler.Start();

                await View
                    .Received()
                    .Select(
                        Arg.Any<string>(),
                        Arg.Any<IEnumerable<SelectOption<DurationFormat>>>(),
                        Arg.Is(1));
            }

            [Fact, LogIfTooSlow]
            public async Task UpdatesTheStoredPreferences()
            {
                var oldDurationFormat = DurationFormat.Decimal;
                var newDurationFormat = DurationFormat.Improved;
                var preferences = new MockPreferences { DurationFormat = oldDurationFormat };
                PreferencesSubject.OnNext(preferences);
                View.Select(
                    Arg.Any<string>(),
                    Arg.Any<IEnumerable<SelectOption<DurationFormat>>>(),
                    Arg.Any<int>())
                .Returns(Observable.Return(newDurationFormat));

                ViewModel.SelectDurationFormat.Execute();
                TestScheduler.Start();

                await InteractorFactory
                    .Received()
                    .UpdatePreferences(Arg.Is<EditPreferencesDTO>(dto => dto.DurationFormat.Equals(New<DurationFormat>.Value(newDurationFormat))))
                    .Execute();
            }

            [Fact, LogIfTooSlow]
            public async Task SelectDurationFormatCommandCallsPushSync()
            {
                var oldDurationFormat = DurationFormat.Decimal;
                var newDurationFormat = DurationFormat.Improved;
                var preferences = new MockPreferences { DurationFormat = oldDurationFormat };
                PreferencesSubject.OnNext(preferences);
                View.Select(
                    Arg.Any<string>(),
                    Arg.Any<IEnumerable<SelectOption<DurationFormat>>>(),
                    Arg.Any<int>())
                .Returns(Observable.Return(newDurationFormat));

                ViewModel.SelectDurationFormat.Execute();
                TestScheduler.Start();

                await SyncManager.Received().PushSync();
            }

            [Fact, LogIfTooSlow]
            public async Task UpdatesTheDurationFormatProperty()
            {
                var oldDurationFormat = DurationFormat.Decimal;
                var newDurationFormat = DurationFormat.Improved;
                var oldPreferences = new MockPreferences { DurationFormat = oldDurationFormat };
                var newPreferences = new MockPreferences { DurationFormat = newDurationFormat };
                PreferencesSubject.OnNext(oldPreferences);
                View.Select(
                    Arg.Any<string>(),
                    Arg.Any<IEnumerable<SelectOption<DurationFormat>>>(),
                    Arg.Any<int>())
                .Returns(Observable.Return(newDurationFormat));
                InteractorFactory
                    .UpdatePreferences(Arg.Any<EditPreferencesDTO>())
                    .Execute()
                    .Returns(Observable.Return(newPreferences));

                ViewModel.SelectDurationFormat.Execute();
                TestScheduler.Start();

                await InteractorFactory
                    .UpdatePreferences(Arg.Is<EditPreferencesDTO>(dto => dto.DurationFormat.ValueOr(oldDurationFormat) == newDurationFormat))
                    .Received()
                    .Execute();
            }
        }

        public sealed class TheSelectBeginningOfWeekMethod : SettingsViewModelTest
        {
            [Fact, LogIfTooSlow]
            public async Task ShowsTheSelectModalPassingCurrentBeginningOfWeek()
            {
                var beginningOfWeek = BeginningOfWeek.Friday;
                var user = new MockUser { BeginningOfWeek = beginningOfWeek };
                UserSubject.OnNext(user);

                ViewModel.SelectBeginningOfWeek.Execute();
                TestScheduler.Start();

                await View
                    .Received()
                    .Select(
                        Arg.Any<string>(),
                        Arg.Any<IEnumerable<SelectOption<BeginningOfWeek>>>(),
                        Arg.Is(5));
            }

            [Fact, LogIfTooSlow]
            public async Task UpdatesTheStoredPreferences()
            {
                var oldBeginningOfWeek = BeginningOfWeek.Tuesday;
                var newBeginningOfWeek = BeginningOfWeek.Sunday;

                var user = Substitute.For<IThreadSafeUser>();
                user.BeginningOfWeek.Returns(oldBeginningOfWeek);
                UserSubject.OnNext(user);
                View.Select(
                    Arg.Any<string>(),
                    Arg.Any<IEnumerable<SelectOption<BeginningOfWeek>>>(),
                    Arg.Any<int>())
                .Returns(Observable.Return(newBeginningOfWeek));

                ViewModel.SelectBeginningOfWeek.Execute();
                TestScheduler.Start();

                await InteractorFactory
                    .Received()
                    .UpdateUser(Arg.Is<EditUserDTO>(dto => dto.BeginningOfWeek == newBeginningOfWeek))
                    .Execute();
            }

            [Fact, LogIfTooSlow]
            public async Task SelectBeginningOfWeekCommandCallsPushSync()
            {
                var oldBeginningOfWeek = BeginningOfWeek.Tuesday;
                var newBeginningOfWeek = BeginningOfWeek.Sunday;
                var user = new MockUser { BeginningOfWeek = oldBeginningOfWeek };
                UserSubject.OnNext(user);
                View.Select(
                    Arg.Any<string>(),
                    Arg.Any<IEnumerable<SelectOption<BeginningOfWeek>>>(),
                    Arg.Any<int>())
                .Returns(Observable.Return(newBeginningOfWeek));

                ViewModel.SelectBeginningOfWeek.Execute();
                TestScheduler.Start();

                await SyncManager.Received().PushSync();
            }

            [Fact, LogIfTooSlow]
            public async Task UpdatesTheBeginningOfWeekProperty()
            {
                var oldBeginningOfWeek = BeginningOfWeek.Tuesday;
                var newBeginningOfWeek = BeginningOfWeek.Sunday;
                var oldUser = new MockUser { BeginningOfWeek = oldBeginningOfWeek };
                var newUser = new MockUser { BeginningOfWeek = newBeginningOfWeek };
                UserSubject.OnNext(oldUser);
                View.Select(
                    Arg.Any<string>(),
                    Arg.Any<IEnumerable<SelectOption<BeginningOfWeek>>>(),
                    Arg.Any<int>())
                .Returns(Observable.Return(newBeginningOfWeek));
                InteractorFactory
                    .UpdateUser(Arg.Any<EditUserDTO>())
                    .Execute()
                    .Returns(Observable.Return(newUser));

                ViewModel.SelectBeginningOfWeek.Execute();
                TestScheduler.Start();

                await InteractorFactory.UpdateUser(
                    Arg.Is<EditUserDTO>(dto => dto.BeginningOfWeek == newBeginningOfWeek
                )).Received().Execute();
            }
        }

        public sealed class TheOpenAboutViewMethod : SettingsViewModelTest
        {
            [Fact, LogIfTooSlow]
            public async Task NavigatesToTheAboutPage()
            {
                ViewModel.OpenAboutView.Execute();

                NavigationService.Received().Navigate<AboutViewModel>(ViewModel.View);
            }
        }

        public sealed class TheIsFeedBackSuccessViewShowingProperty : SettingsViewModelTest
        {
            [Fact, LogIfTooSlow]
            public void EmitsTrueWhenTapOnTheView()
            {
                var observer = TestScheduler.CreateObserver<bool>();
                var viewModel = CreateViewModel();

                viewModel.IsFeedbackSuccessViewShowing.StartWith(true).Subscribe(observer);
                viewModel.CloseFeedbackSuccessView();
                TestScheduler.Start();

                observer.LastEmittedValue().Should().BeFalse();
            }
        }

        public sealed class TheOpenCalendarSettingsAction : SettingsViewModelTest
        {
            [Fact, LogIfTooSlow]
            public async Task NavigatesToCalendarSettingsViewModel()
            {
                ViewModel.OpenCalendarSettings.Execute(Unit.Default);

                await NavigationService.Received()
                    .Navigate<CalendarSettingsViewModel, bool, string[]>(Arg.Any<bool>(), View);
            }
        }

        public sealed class TheIsSmartRemindersVisibleProperty : SettingsViewModelTest
        {
            [Fact, LogIfTooSlow]
            public async Task EmitsTrueWhenCalendarPermissionsAreGrantedAndCalendarsAreSelected()
            {
                PermissionsChecker.CalendarPermissionGranted.Returns(Observable.Return(true));
                UserPreferences.EnabledCalendars.Returns(Observable.Return(new List<string>() { "1" }));

                var observer = TestScheduler.CreateObserver<bool>();
                var viewModel = CreateViewModel();

                await viewModel.Initialize();
                viewModel.IsCalendarSmartRemindersVisible.Subscribe(observer);
                TestScheduler.Start();

                observer.Messages.First().Value.Value.Should().BeTrue();
            }

            [Fact, LogIfTooSlow]
            public async Task EmitsFalseWhenCalendarPermissionsAreNotGranted()
            {
                PermissionsChecker.CalendarPermissionGranted.Returns(Observable.Return(false));
                UserPreferences.EnabledCalendars.Returns(Observable.Return(new List<string>() { "1" }));

                var observer = TestScheduler.CreateObserver<bool>();
                var viewModel = CreateViewModel();

                await viewModel.Initialize();
                viewModel.IsCalendarSmartRemindersVisible.Subscribe(observer);
                TestScheduler.Start();

                observer.Messages.First().Value.Value.Should().BeFalse();
            }

            [Fact, LogIfTooSlow]
            public async Task EmitsFalseWhenNoCalendarsAreSelected()
            {
                PermissionsChecker.CalendarPermissionGranted.Returns(Observable.Return(true));
                UserPreferences.EnabledCalendars.Returns(Observable.Return(new List<string>()));

                var observer = TestScheduler.CreateObserver<bool>();
                var viewModel = CreateViewModel();

                await viewModel.Initialize();
                viewModel.IsCalendarSmartRemindersVisible.Subscribe(observer);
                TestScheduler.Start();
                
                observer.Messages.First().Value.Value.Should().BeFalse();
            }

            [Fact, LogIfTooSlow]
            public async Task EmitsAgainWhenCalendarPermissionsChangeAfterViewAppears()
            {
                PermissionsChecker.CalendarPermissionGranted.Returns(Observable.Return(false), Observable.Return(true));
                UserPreferences.EnabledCalendars.Returns(Observable.Return(new List<string>() { "1" }));

                var observer = TestScheduler.CreateObserver<bool>();
                var viewModel = CreateViewModel();

                await viewModel.Initialize();
                viewModel.IsCalendarSmartRemindersVisible.Subscribe(observer);
                TestScheduler.Start();

                viewModel.ViewAppeared();
                TestScheduler.Start();

                var messages = observer.Messages.Select(msg => msg.Value.Value);

                messages.First().Should().BeFalse();
                messages.Last().Should().BeTrue();
            }
        }
    }
}
