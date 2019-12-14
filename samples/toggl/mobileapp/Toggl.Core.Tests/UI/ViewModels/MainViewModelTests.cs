﻿using FluentAssertions;
using Microsoft.Reactive.Testing;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading;
using System.Threading.Tasks;
using Toggl.Core.Analytics;
using Toggl.Core.Interactors;
using Toggl.Core.Models;
using Toggl.Core.Models.Interfaces;
using Toggl.Core.Suggestions;
using Toggl.Core.Sync;
using Toggl.Core.Tests.Generators;
using Toggl.Core.Tests.Mocks;
using Toggl.Core.Tests.TestExtensions;
using Toggl.Core.UI.Navigation;
using Toggl.Core.UI.Parameters;
using Toggl.Core.UI.ViewModels;
using Toggl.Core.UI.Views;
using Toggl.Shared;
using Toggl.Shared.Extensions;
using Toggl.Storage;
using Toggl.Storage.Settings;
using Xunit;
using static Toggl.Core.Helper.Constants;
using ThreadingTask = System.Threading.Tasks.Task;

namespace Toggl.Core.Tests.UI.ViewModels
{
    public sealed class MainViewModelTests
    {
        public abstract class MainViewModelTest : BaseViewModelTests<MainViewModel>
        {
            protected ISubject<SyncProgress> ProgressSubject { get; } = new Subject<SyncProgress>();

            protected override MainViewModel CreateViewModel()
            {
                var vm = new MainViewModel(
                    DataSource,
                    SyncManager,
                    TimeService,
                    RatingService,
                    UserPreferences,
                    AnalyticsService,
                    OnboardingStorage,
                    InteractorFactory,
                    NavigationService,
                    RemoteConfigService,
                    AccessibilityService,
                    UpdateRemoteConfigCacheService,
                    AccessRestrictionStorage,
                    SchedulerProvider,
                    RxActionFactory,
                    PermissionsChecker,
                    BackgroundService,
                    PlatformInfo,
                    WidgetsService,
                    LastTimeUsageStorage);

                vm.Initialize();

                return vm;
            }

            protected override void AdditionalSetup()
            {
                base.AdditionalSetup();

                var syncManager = Substitute.For<ISyncManager>();
                syncManager.ProgressObservable.Returns(ProgressSubject.AsObservable());

                var defaultRemoteConfiguration = new RatingViewConfiguration(5, RatingViewCriterion.None);
                RemoteConfigService
                    .GetRatingViewConfiguration()
                    .Returns(defaultRemoteConfiguration);

                DataSource.Preferences.Current.Returns(Observable.Create<IThreadSafePreferences>(observer =>
                {
                    observer.OnNext(new MockPreferences
                    {
                        DateFormat = DateFormat.FromLocalizedDateFormat("dd/mm/YYYY")
                    });
                    return Disposable.Empty;
                }));
            }
        }

        public sealed class TheConstructor : MainViewModelTest
        {
            [Theory, LogIfTooSlow]
            [ConstructorData]
            public void ThrowsIfAnyOfTheArgumentsIsNull(
                bool useDataSource,
                bool useSyncManager,
                bool useTimeService,
                bool useRatingService,
                bool useUserPreferences,
                bool useAnalyticsService,
                bool useOnboardingStorage,
                bool useInteractorFactory,
                bool useNavigationService,
                bool useRemoteConfigService,
                bool useAccessibilityService,
                bool useRemoteConfigUpdateService,
                bool useAccessRestrictionStorage,
                bool useSchedulerProvider,
                bool useRxActionFactory,
                bool usePermissionsChecker,
                bool useBackgroundService,
                bool usePlatformInfo,
                bool useWidgetsService,
                bool useLastTimeUsageStorage)
            {
                var dataSource = useDataSource ? DataSource : null;
                var syncManager = useSyncManager ? SyncManager : null;
                var timeService = useTimeService ? TimeService : null;
                var ratingService = useRatingService ? RatingService : null;
                var userPreferences = useUserPreferences ? UserPreferences : null;
                var analyticsService = useAnalyticsService ? AnalyticsService : null;
                var navigationService = useNavigationService ? NavigationService : null;
                var interactorFactory = useInteractorFactory ? InteractorFactory : null;
                var onboardingStorage = useOnboardingStorage ? OnboardingStorage : null;
                var remoteConfigService = useRemoteConfigService ? RemoteConfigService : null;
                var accessibilityService = useAccessibilityService ? AccessibilityService : null;
                var remoteConfigUpdateService = useRemoteConfigUpdateService ? UpdateRemoteConfigCacheService : null;
                var schedulerProvider = useSchedulerProvider ? SchedulerProvider : null;
                var accessRestrictionStorage = useAccessRestrictionStorage ? AccessRestrictionStorage : null;
                var rxActionFactory = useRxActionFactory ? RxActionFactory : null;
                var permissionsChecker = usePermissionsChecker ? PermissionsChecker : null;
                var backgroundService = useBackgroundService ? BackgroundService : null;
                var platformInfo = usePlatformInfo ? PlatformInfo : null;
                var widgetsService = useWidgetsService ? WidgetsService : null;
                var lastTimeUsageStorage = useLastTimeUsageStorage ? LastTimeUsageStorage : null;

                Action tryingToConstructWithEmptyParameters =
                    () => new MainViewModel(
                        dataSource,
                        syncManager,
                        timeService,
                        ratingService,
                        userPreferences,
                        analyticsService,
                        onboardingStorage,
                        interactorFactory,
                        navigationService,
                        remoteConfigService,
                        accessibilityService,
                        remoteConfigUpdateService,
                        accessRestrictionStorage,
                        schedulerProvider,
                        rxActionFactory,
                        permissionsChecker,
                        backgroundService,
                        platformInfo,
                        widgetsService,
                        lastTimeUsageStorage);

                tryingToConstructWithEmptyParameters
                    .Should().Throw<ArgumentNullException>();
            }
        }

        [CollectionDefinition("global", DisableParallelization = true)]
        public sealed class TheViewAppearingMethod : MainViewModelTest, IDisposable
        {
            private CultureInfo originalCultureInfo;

            public TheViewAppearingMethod()
            {
                originalCultureInfo = Thread.CurrentThread.CurrentUICulture;
            }

            public void Dispose()
            {
                Thread.CurrentThread.CurrentUICulture = originalCultureInfo;
            }

            [Fact, LogIfTooSlow]
            public async ThreadingTask NavigatesToNoWorkspaceViewModelWhenNoWorkspaceStateIsSet()
            {
                AccessRestrictionStorage.HasNoWorkspace().Returns(true);

                ViewModel.ViewAppearing();

                await NavigationService.Received().Navigate<NoWorkspaceViewModel, Unit>(View);
            }

            [Fact, LogIfTooSlow]
            public async ThreadingTask DoesNotNavigateToNoWorkspaceViewModelWhenNoWorkspaceStateIsNotSet()
            {
                AccessRestrictionStorage.HasNoWorkspace().Returns(false);

                ViewModel.ViewAppearing();

                await NavigationService.DidNotReceive().Navigate<NoWorkspaceViewModel, Unit>(View);
            }

            [Fact, LogIfTooSlow]
            public async ThreadingTask DoesNotNavigateToNoWorkspaceViewSeveralTimes()
            {
                AccessRestrictionStorage.HasNoWorkspace().Returns(true);
                var task = new TaskCompletionSource<Unit>().Task;
                NavigationService.Navigate<NoWorkspaceViewModel, Unit>(View).Returns(task);

                ViewModel.ViewAppearing();
                ViewModel.ViewAppearing();
                ViewModel.ViewAppearing();
                ViewModel.ViewAppearing();

                await NavigationService.Received(1).Navigate<NoWorkspaceViewModel, Unit>(View);
            }

            [Fact, LogIfTooSlow]
            public async ThreadingTask NavigatesToSelectDefaultWorkspaceViewModelWhenNoDefaultWorkspaceStateIsSet()
            {
                AccessRestrictionStorage.HasNoWorkspace().Returns(false);
                AccessRestrictionStorage.HasNoDefaultWorkspace().Returns(true);

                await ViewModel.ViewAppearingAsync();

                await NavigationService.Received().Navigate<SelectDefaultWorkspaceViewModel, Unit>(View);
            }

            [Fact, LogIfTooSlow]
            public async ThreadingTask DoesNotNavigateToSelectDefaultWorkspaceViewModelWhenNoDefaultWorkspaceStateIsNotSet()
            {
                AccessRestrictionStorage.HasNoDefaultWorkspace().Returns(false);

                ViewModel.ViewAppearing();

                await NavigationService.DidNotReceive().Navigate<SelectDefaultWorkspaceViewModel, Unit>(View);
            }

            [Fact, LogIfTooSlow]
            public async ThreadingTask DoesNotNavigateToSelectDefaultWorkspaceViewSeveralTimes()
            {
                AccessRestrictionStorage.HasNoWorkspace().Returns(false);
                AccessRestrictionStorage.HasNoDefaultWorkspace().Returns(true);
                var task = new TaskCompletionSource<Unit>().Task;
                NavigationService.Navigate<SelectDefaultWorkspaceViewModel, Unit>(View).Returns(task);

                ViewModel.ViewAppearing();
                ViewModel.ViewAppearing();
                ViewModel.ViewAppearing();
                ViewModel.ViewAppearing();
                //ViewAppearing calls an async method. The delay is here to ensure that the async method completes before the assertion
                await ThreadingTask.Delay(200);

                await NavigationService.Received(1).Navigate<SelectDefaultWorkspaceViewModel, Unit>(View);
            }

            [Fact, LogIfTooSlow]
            public async ThreadingTask DoesNotNavigateToSelectDefaultWorkspaceViewModelWhenTheresNoWorkspaceAvaialable()
            {
                AccessRestrictionStorage.HasNoWorkspace().Returns(true);
                AccessRestrictionStorage.HasNoDefaultWorkspace().Returns(true);

                await ViewModel.ViewAppearingAsync();

                await NavigationService.Received().Navigate<NoWorkspaceViewModel, Unit>(View);
                await NavigationService.DidNotReceive().Navigate<SelectDefaultWorkspaceViewModel, Unit>(View);
            }

            [Theory, LogIfTooSlow]
            [InlineData("A")]
            [InlineData("B")]
            public async ThreadingTask NavigatesToJanuary2020CampaignPopup(string group)
            {
                var remoteConfig = new January2020CampaignConfiguration(group);
                var mockTimeEntry = new MockTimeEntry
                {
                    Start = DateTimeOffset.Now,
                    Duration = 1,
                    IsDeleted = false,
                    ServerDeletedAt = null,
                    TagIds = new long[0],
                    Workspace = new MockWorkspace { IsInaccessible = false }
                };
                var twoTEs = new BehaviorSubject<IEnumerable<IThreadSafeTimeEntry>>(new[] { mockTimeEntry, mockTimeEntry });
                Thread.CurrentThread.CurrentUICulture = new CultureInfo("en");
                TimeService.CurrentDateTime.Returns(DateTimeOffset.Now);
                RemoteConfigService.GetJanuary2020CampaignConfiguration().Returns(remoteConfig);
                OnboardingStorage.WasJanuary2020CampaignShown().Returns(false);
                LastTimeUsageStorage.LastLogin.Returns(DateTimeOffset.Now - TimeSpan.FromHours(49));
                InteractorFactory.ObserveAllTimeEntriesVisibleToTheUser().Execute().Returns(twoTEs);
                DataSource.Preferences.Current.Returns(
                    new BehaviorSubject<IThreadSafePreferences>(
                        new MockPreferences { CollapseTimeEntries = false }));

                var vm = CreateViewModel();
                await vm.Initialize(); // I need to initialize the VM after the arrangements are made

                var task = vm.ViewAppearingAsync();
                SchedulerProvider.TestScheduler.Start();
                await task;

                await NavigationService.Received().Navigate<January2020CampaignViewModel, Unit, Unit>(Unit.Default, Arg.Any<IView>());
            }

            [Theory, LogIfTooSlow]
            [InlineData("A")]
            [InlineData("B")]
            public async ThreadingTask DoesNotShowJanuary2020CampaignIfTheLanguageIsSetToJapanese(string group)
            {
                var remoteConfig = new January2020CampaignConfiguration(group);
                var mockTimeEntry = new MockTimeEntry
                {
                    Start = DateTimeOffset.Now,
                    Duration = 1,
                    IsDeleted = false,
                    ServerDeletedAt = null,
                    TagIds = new long[0],
                    Workspace = new MockWorkspace { IsInaccessible = false }
                };
                var twoTEs = new BehaviorSubject<IEnumerable<IThreadSafeTimeEntry>>(new[] { mockTimeEntry, mockTimeEntry });
                Thread.CurrentThread.CurrentUICulture = new CultureInfo("ja-JP");
                TimeService.CurrentDateTime.Returns(DateTimeOffset.Now);
                RemoteConfigService.GetJanuary2020CampaignConfiguration().Returns(remoteConfig);
                OnboardingStorage.WasJanuary2020CampaignShown().Returns(false);
                LastTimeUsageStorage.LastLogin.Returns(DateTimeOffset.Now - TimeSpan.FromHours(49));
                InteractorFactory.ObserveAllTimeEntriesVisibleToTheUser().Execute().Returns(twoTEs);
                DataSource.Preferences.Current.Returns(
                    new BehaviorSubject<IThreadSafePreferences>(
                        new MockPreferences { CollapseTimeEntries = false }));

                var vm = CreateViewModel();
                await vm.Initialize(); // I need to initialize the VM after the arrangements are made

                var task = vm.ViewAppearingAsync();
                SchedulerProvider.TestScheduler.Start();
                await task;

                await NavigationService.DidNotReceive().Navigate<January2020CampaignViewModel, Unit, Unit>(Unit.Default, Arg.Any<IView>());
            }

            [Theory, LogIfTooSlow]
            [InlineData("A")]
            [InlineData("B")]
            public async ThreadingTask DoesNotShowJanuary2020CampaignIfItWasShownBefore(string group)
            {
                var remoteConfig = new January2020CampaignConfiguration(group);
                var mockTimeEntry = new MockTimeEntry
                {
                    Start = DateTimeOffset.Now,
                    Duration = 1,
                    IsDeleted = false,
                    ServerDeletedAt = null,
                    TagIds = new long[0],
                    Workspace = new MockWorkspace { IsInaccessible = false }
                };
                var twoTEs = new BehaviorSubject<IEnumerable<IThreadSafeTimeEntry>>(new[] { mockTimeEntry, mockTimeEntry });
                Thread.CurrentThread.CurrentUICulture = new CultureInfo("en");
                TimeService.CurrentDateTime.Returns(DateTimeOffset.Now);
                RemoteConfigService.GetJanuary2020CampaignConfiguration().Returns(remoteConfig);
                OnboardingStorage.WasJanuary2020CampaignShown().Returns(true);
                LastTimeUsageStorage.LastLogin.Returns(DateTimeOffset.Now - TimeSpan.FromHours(49));
                InteractorFactory.ObserveAllTimeEntriesVisibleToTheUser().Execute().Returns(twoTEs);
                DataSource.Preferences.Current.Returns(
                    new BehaviorSubject<IThreadSafePreferences>(
                        new MockPreferences { CollapseTimeEntries = false }));

                var vm = CreateViewModel();
                await vm.Initialize(); // I need to initialize the VM after the arrangements are made

                var task = vm.ViewAppearingAsync();
                SchedulerProvider.TestScheduler.Start();
                await task;

                await NavigationService.DidNotReceive().Navigate<January2020CampaignViewModel, Unit, Unit>(Unit.Default, Arg.Any<IView>());
            }

            [Fact, LogIfTooSlow]
            public async ThreadingTask DoesNotShowJanuary2020CampaignIfItIsDisabled()
            {
                var remoteConfig = new January2020CampaignConfiguration("none");
                var mockTimeEntry = new MockTimeEntry
                {
                    Start = DateTimeOffset.Now,
                    Duration = 1,
                    IsDeleted = false,
                    ServerDeletedAt = null,
                    TagIds = new long[0],
                    Workspace = new MockWorkspace { IsInaccessible = false }
                };
                var twoTEs = new BehaviorSubject<IEnumerable<IThreadSafeTimeEntry>>(new[] { mockTimeEntry, mockTimeEntry });
                Thread.CurrentThread.CurrentUICulture = new CultureInfo("en");
                TimeService.CurrentDateTime.Returns(DateTimeOffset.Now);
                RemoteConfigService.GetJanuary2020CampaignConfiguration().Returns(remoteConfig);
                OnboardingStorage.WasJanuary2020CampaignShown().Returns(true);
                LastTimeUsageStorage.LastLogin.Returns(DateTimeOffset.Now - TimeSpan.FromHours(49));
                InteractorFactory.ObserveAllTimeEntriesVisibleToTheUser().Execute().Returns(twoTEs);
                DataSource.Preferences.Current.Returns(
                    new BehaviorSubject<IThreadSafePreferences>(new MockPreferences { CollapseTimeEntries = false }));

                var vm = CreateViewModel();
                await vm.Initialize(); // I need to initialize the VM after the arrangements are made

                var task = vm.ViewAppearingAsync();
                SchedulerProvider.TestScheduler.Start();
                await task;

                await NavigationService.DidNotReceive().Navigate<January2020CampaignViewModel, Unit, Unit>(Unit.Default, Arg.Any<IView>());
            }

            [Theory, LogIfTooSlow]
            [InlineData("A")]
            [InlineData("B")]
            public async ThreadingTask DoesNotShowJanuary2020CampaignIfTheUserLoggedInRecently(string group)
            {
                var remoteConfig = new January2020CampaignConfiguration(group);
                var mockTimeEntry = new MockTimeEntry
                {
                    Start = DateTimeOffset.Now,
                    Duration = 1,
                    IsDeleted = false,
                    ServerDeletedAt = null,
                    TagIds = new long[0],
                    Workspace = new MockWorkspace { IsInaccessible = false }
                };
                var twoTEs = new BehaviorSubject<IEnumerable<IThreadSafeTimeEntry>>(new[] { mockTimeEntry, mockTimeEntry });
                Thread.CurrentThread.CurrentUICulture = new CultureInfo("en");
                TimeService.CurrentDateTime.Returns(DateTimeOffset.Now);
                RemoteConfigService.GetJanuary2020CampaignConfiguration().Returns(remoteConfig);
                OnboardingStorage.WasJanuary2020CampaignShown().Returns(true);
                LastTimeUsageStorage.LastLogin.Returns(DateTimeOffset.Now - TimeSpan.FromHours(47));
                InteractorFactory.ObserveAllTimeEntriesVisibleToTheUser().Execute().Returns(twoTEs);
                DataSource.Preferences.Current.Returns(
                    new BehaviorSubject<IThreadSafePreferences>(new MockPreferences { CollapseTimeEntries = false }));

                var vm = CreateViewModel();
                await vm.Initialize(); // I need to initialize the VM after the arrangements are made

                var task = vm.ViewAppearingAsync();
                SchedulerProvider.TestScheduler.Start();
                await task;

                await NavigationService.DidNotReceive().Navigate<January2020CampaignViewModel, Unit, Unit>(Unit.Default, Arg.Any<IView>());
            }

            [Theory, LogIfTooSlow]
            [InlineData("A")]
            [InlineData("B")]
            public async ThreadingTask DoesNotShowJanuary2020CampaignIfTheUserDoesNotHaveEnoughTimeEntries(string group)
            {
                var remoteConfig = new January2020CampaignConfiguration(group);
                var mockTimeEntry = new MockTimeEntry
                {
                    Start = DateTimeOffset.Now,
                    Duration = 1,
                    IsDeleted = false,
                    ServerDeletedAt = null,
                    TagIds = new long[0],
                    Workspace = new MockWorkspace { IsInaccessible = false }
                };
                var singleTELog = new BehaviorSubject<IEnumerable<IThreadSafeTimeEntry>>(new[] { mockTimeEntry });
                Thread.CurrentThread.CurrentUICulture = new CultureInfo("en");
                TimeService.CurrentDateTime.Returns(DateTimeOffset.Now);
                RemoteConfigService.GetJanuary2020CampaignConfiguration().Returns(remoteConfig);
                OnboardingStorage.WasJanuary2020CampaignShown().Returns(true);
                LastTimeUsageStorage.LastLogin.Returns(DateTimeOffset.Now - TimeSpan.FromHours(49));
                InteractorFactory.ObserveAllTimeEntriesVisibleToTheUser().Execute().Returns(singleTELog);
                DataSource.Preferences.Current.Returns(
                    new BehaviorSubject<IThreadSafePreferences>(new MockPreferences { CollapseTimeEntries = false }));

                var vm = CreateViewModel();
                await vm.Initialize(); // I need to initialize the VM after the arrangements are made

                var task = vm.ViewAppearingAsync();
                SchedulerProvider.TestScheduler.Start();
                await task;

                await NavigationService.DidNotReceive().Navigate<January2020CampaignViewModel, Unit, Unit>(Unit.Default, Arg.Any<IView>());
            }
        }

        public sealed class TheStartTimeEntryAction : MainViewModelTest
        {
            private readonly ISubject<IThreadSafeTimeEntry> subject = new Subject<IThreadSafeTimeEntry>();

            public TheStartTimeEntryAction()
            {
                DataSource.TimeEntries.CurrentlyRunningTimeEntry.Returns(subject);
                TimeService.CurrentDateTime.Returns(DateTimeOffset.Now);
                ViewModel.Initialize().GetAwaiter().GetResult();

                subject.OnNext(null);
                TestScheduler.AdvanceBy(TimeSpan.FromMilliseconds(50).Ticks);
            }

            [Theory, LogIfTooSlow]
            [InlineData(true, true)]
            [InlineData(true, false)]
            [InlineData(false, true)]
            [InlineData(false, false)]
            public async ThreadingTask NavigatesToTheStartTimeEntryViewModel(bool isInManualMode, bool useDefaultMode)
            {
                UserPreferences.IsManualModeEnabled.Returns(isInManualMode);

                ViewModel.StartTimeEntry.Execute(useDefaultMode);

                TestScheduler.Start();
                await NavigationService.Received()
                   .Navigate<StartTimeEntryViewModel, StartTimeEntryParameters>(Arg.Any<StartTimeEntryParameters>(), ViewModel.View);
            }

            [Theory, LogIfTooSlow]
            [InlineData(true, true)]
            [InlineData(true, false)]
            [InlineData(false, true)]
            [InlineData(false, false)]
            public async ThreadingTask PassesTheAppropriatePlaceholderToTheStartTimeEntryViewModel(bool isInManualMode, bool useDefaultMode)
            {
                UserPreferences.IsManualModeEnabled.Returns(isInManualMode);

                ViewModel.StartTimeEntry.Execute(useDefaultMode);

                TestScheduler.Start();
                var expected = isInManualMode == useDefaultMode
                    ? Resources.ManualTimeEntryPlaceholder
                    : Resources.StartTimeEntryPlaceholder;
                await NavigationService.Received().Navigate<StartTimeEntryViewModel, StartTimeEntryParameters>(
                    Arg.Is<StartTimeEntryParameters>(parameter => parameter.PlaceholderText == expected),
                    ViewModel.View
                );
            }

            [Theory, LogIfTooSlow]
            [InlineData(true, true)]
            [InlineData(true, false)]
            [InlineData(false, true)]
            [InlineData(false, false)]
            public async ThreadingTask PassesTheAppropriateDurationToTheStartTimeEntryViewModel(bool isInManualMode, bool useDefaultMode)
            {
                UserPreferences.IsManualModeEnabled.Returns(isInManualMode);

                ViewModel.StartTimeEntry.Execute(useDefaultMode);

                TestScheduler.Start();
                var expected = isInManualMode == useDefaultMode
                    ? TimeSpan.FromMinutes(DefaultTimeEntryDurationForManualModeInMinutes)
                    : (TimeSpan?)null;
                await NavigationService.Received().Navigate<StartTimeEntryViewModel, StartTimeEntryParameters>(
                    Arg.Is<StartTimeEntryParameters>(parameter => parameter.Duration == expected),
                    ViewModel.View
                );
            }

            [Theory, LogIfTooSlow]
            [InlineData(true, true)]
            [InlineData(true, false)]
            [InlineData(false, true)]
            [InlineData(false, false)]
            public async ThreadingTask PassesTheAppropriateStartTimeToTheStartTimeEntryViewModel(bool isInManualMode, bool useDefaultMode)
            {
                var date = DateTimeOffset.Now;
                TimeService.CurrentDateTime.Returns(date);
                UserPreferences.IsManualModeEnabled.Returns(isInManualMode);

                ViewModel.StartTimeEntry.Execute(useDefaultMode);

                TestScheduler.Start();
                var expected = isInManualMode == useDefaultMode
                    ? date.Subtract(TimeSpan.FromMinutes(DefaultTimeEntryDurationForManualModeInMinutes))
                    : date;
                await NavigationService.Received().Navigate<StartTimeEntryViewModel, StartTimeEntryParameters>(
                    Arg.Is<StartTimeEntryParameters>(parameter => parameter.StartTime == expected),
                    ViewModel.View
                );
            }

            [Theory, LogIfTooSlow]
            [InlineData(true)]
            [InlineData(false)]
            public void CannotBeExecutedWhenThereIsARunningTimeEntry(bool useDefaultMode)
            {
                var timeEntry = new MockTimeEntry();
                subject.OnNext(timeEntry);
                TestScheduler.AdvanceBy(TimeSpan.FromMilliseconds(50).Ticks);

                var errors = TestScheduler.CreateObserver<Exception>();
                ViewModel.StartTimeEntry.Errors.Subscribe(errors);
                ViewModel.StartTimeEntry.Execute(useDefaultMode);

                TestScheduler.Start();

                errors.Messages.Count.Should().Be(1);
                errors.LastEmittedValue().Should().BeEquivalentTo(new RxActionNotEnabledException());
            }

            [Theory, LogIfTooSlow]
            [InlineData(true)]
            [InlineData(false)]
            public void MarksTheActionButtonTappedForOnboardingPurposes(bool useDefaultMode)
            {
                ViewModel.StartTimeEntry.Execute(useDefaultMode);

                TestScheduler.Start();
                OnboardingStorage.Received().StartButtonWasTapped();
            }

            [Theory, LogIfTooSlow]
            [InlineData(true)]
            [InlineData(false)]
            public async ThreadingTask MarksTheActionNavigatedAwayBeforeStopButtonForOnboardingPurposes(bool useDefaultMode)
            {
                OnboardingStorage.StopButtonWasTappedBefore.Returns(Observable.Return(false));
                await ViewModel.Initialize();
                subject.OnNext(null);
                TestScheduler.AdvanceBy(TimeSpan.FromMilliseconds(50).Ticks);

                ViewModel.StartTimeEntry.Execute(useDefaultMode);

                TestScheduler.Start();
                OnboardingStorage.DidNotReceive().SetNavigatedAwayFromMainViewAfterStopButton();
            }

            [Theory, LogIfTooSlow]
            [InlineData(true)]
            [InlineData(false)]
            public async ThreadingTask MarksTheActionNavigatedAwayAfterStopButtonForOnboardingPurposes(bool useDefaultMode)
            {
                var timeEntry = Substitute.For<IThreadSafeTimeEntry>();
                var observable = Observable.Return<IThreadSafeTimeEntry>(null);
                DataSource.TimeEntries.CurrentlyRunningTimeEntry.Returns(observable);
                OnboardingStorage.StopButtonWasTappedBefore.Returns(Observable.Return(true));
                await ViewModel.Initialize();
                subject.OnNext(null);
                TestScheduler.AdvanceBy(TimeSpan.FromMilliseconds(50).Ticks);

                ViewModel.StartTimeEntry.Execute(useDefaultMode);

                TestScheduler.Start();
                OnboardingStorage.Received().SetNavigatedAwayFromMainViewAfterStopButton();
            }
        }

        public sealed class TheOpenSettingsCommand : MainViewModelTest
        {
            [Fact, LogIfTooSlow]
            public async ThreadingTask NavigatesToTheSettingsViewModel()
            {
                ViewModel.Initialize().Wait();

                ViewModel.OpenSettings.Execute();

                TestScheduler.Start();
                await NavigationService.Received().Navigate<SettingsViewModel>(View);
            }

            [Fact, LogIfTooSlow]
            public void MarksTheActionBeforeStopButtonForOnboardingPurposes()
            {
                OnboardingStorage.StopButtonWasTappedBefore.Returns(Observable.Return(false));
                ViewModel.Initialize().Wait();

                ViewModel.OpenSettings.Execute();

                TestScheduler.Start();
                OnboardingStorage.DidNotReceive().SetNavigatedAwayFromMainViewAfterStopButton();
            }

            [Fact, LogIfTooSlow]
            public void MarksTheActionAfterStopButtonForOnboardingPurposes()
            {
                OnboardingStorage.StopButtonWasTappedBefore.Returns(Observable.Return(true));
                ViewModel.Initialize().Wait();

                ViewModel.OpenSettings.Execute();

                TestScheduler.Start();
                OnboardingStorage.Received().SetNavigatedAwayFromMainViewAfterStopButton();
            }
        }

        public sealed class TheOpenSyncFailuresCommand : MainViewModelTest
        {
            [Fact, LogIfTooSlow]
            public async ThreadingTask NavigatesToTheSyncFailuresViewModel()
            {
                ViewModel.Initialize().Wait();

                ViewModel.OpenSyncFailures.Execute();

                TestScheduler.Start();
                await NavigationService.Received().Navigate<SyncFailuresViewModel>(View);
            }
        }

        public class TheStopTimeEntryAction : MainViewModelTest
        {
            private ISubject<IThreadSafeTimeEntry> subject;

            public TheStopTimeEntryAction()
            {
                var timeEntry = Substitute.For<IThreadSafeTimeEntry>();
                subject = new BehaviorSubject<IThreadSafeTimeEntry>(timeEntry);
                DataSource.TimeEntries.CurrentlyRunningTimeEntry.Returns(subject);

                ViewModel.Initialize().Wait();
                TestScheduler.AdvanceBy(TimeSpan.FromMilliseconds(50).Ticks);
            }

            [Fact, LogIfTooSlow]
            public async ThreadingTask CallsTheStopMethodOnTheDataSource()
            {
                var date = DateTimeOffset.UtcNow;
                TimeService.CurrentDateTime.Returns(date);

                ViewModel.StopTimeEntry.Execute(TimeEntryStopOrigin.Deeplink);

                TestScheduler.Start();
                await InteractorFactory.Received().StopTimeEntry(date, TimeEntryStopOrigin.Deeplink).Execute();
            }

            [Fact, LogIfTooSlow]
            public async ThreadingTask InitiatesPushSync()
            {
                ViewModel.StopTimeEntry.Execute(Arg.Any<TimeEntryStopOrigin>());

                TestScheduler.Start();
                SyncManager.Received().PushSync();
            }

            [Fact, LogIfTooSlow]
            public void MarksTheActionForOnboardingPurposes()
            {
                ViewModel.StopTimeEntry.Execute(Arg.Any<TimeEntryStopOrigin>());

                TestScheduler.Start();
                OnboardingStorage.Received().StopButtonWasTapped();
            }

            [Fact, LogIfTooSlow]
            public async ThreadingTask DoesNotInitiatePushSyncWhenSavingFails()
            {
                InteractorFactory
                    .StopTimeEntry(Arg.Any<DateTimeOffset>(), Arg.Any<TimeEntryStopOrigin>())
                    .Execute()
                    .Returns(ThreadingTask.FromException<IThreadSafeTimeEntry>(new Exception()));

                var errors = TestScheduler.CreateObserver<Exception>();
                ViewModel.StopTimeEntry.Errors.Subscribe(errors);
                ViewModel.StopTimeEntry.Execute(Arg.Any<TimeEntryStopOrigin>());

                TestScheduler.Start();

                errors.Messages.Count().Should().Be(1);
                SyncManager.DidNotReceive().PushSync();
            }

            [Fact, LogIfTooSlow]
            public async ThreadingTask CannotBeExecutedWhenNoTimeEntryIsRunning()
            {
                subject.OnNext(null);
                TestScheduler.AdvanceBy(TimeSpan.FromMilliseconds(50).Ticks);

                var errors = TestScheduler.CreateObserver<Exception>();
                ViewModel.StopTimeEntry.Errors.Subscribe(errors);
                ViewModel.StopTimeEntry.Execute(TimeEntryStopOrigin.Manual);

                TestScheduler.Start();

                errors.Messages.Count.Should().Be(1);
                errors.LastEmittedValue().Should().BeEquivalentTo(new RxActionNotEnabledException());

                await InteractorFactory.DidNotReceive().StopTimeEntry(Arg.Any<DateTimeOffset>(), Arg.Any<TimeEntryStopOrigin>()).Execute();
            }
        }

        public sealed class TheNumberOfSyncFailuresProperty : MainViewModelTest
        {
            [Fact, LogIfTooSlow]
            public async ThreadingTask ReturnsTheCountOfInteractorResult()
            {
                var syncables = new IDatabaseSyncable[]
                {
                    new MockTag { Name = "Tag", SyncStatus = SyncStatus.SyncFailed, LastSyncErrorMessage = "Error1" },
                    new MockTag { Name = "Tag2", SyncStatus = SyncStatus.SyncFailed, LastSyncErrorMessage = "Error1" },
                    new MockProject { Name = "Project", SyncStatus = SyncStatus.SyncFailed, LastSyncErrorMessage = "Error2" }
                };
                var items = syncables.Select(i => new SyncFailureItem(i));
                var interactor = Substitute.For<IInteractor<IObservable<IEnumerable<SyncFailureItem>>>>();
                interactor.Execute().Returns(Observable.Return(items));
                InteractorFactory.GetItemsThatFailedToSync().Returns(interactor);
                await ViewModel.Initialize();

                var observer = TestScheduler.CreateObserver<int>();
                ViewModel.NumberOfSyncFailures.Subscribe(observer);
                TestScheduler.AdvanceBy(50);

                observer.Messages
                    .Last(m => m.Value.Kind == System.Reactive.NotificationKind.OnNext).Value.Value
                    .Should()
                    .Be(syncables.Length);
            }
        }

        public abstract class InitialStateTest : MainViewModelTest
        {
            protected void PrepareSuggestion()
            {
                DataSource.TimeEntries.IsEmpty.Returns(Observable.Return(false));
                var suggestionProvider = Substitute.For<ISuggestionProvider>();
                var timeEntry = Substitute.For<IThreadSafeTimeEntry>();
                timeEntry.Id.Returns(123);
                timeEntry.Start.Returns(DateTimeOffset.Now);
                timeEntry.Duration.Returns((long?)null);
                timeEntry.Description.Returns("something");
                var suggestion = new Suggestion(timeEntry, SuggestionProviderType.MostUsedTimeEntries);
                InteractorFactory.GetSuggestions(Arg.Any<int>()).Execute().Returns(Observable.Return(new[] { suggestion }));
            }

            protected void PrepareTimeEntry()
            {
                var timeEntry = Substitute.For<IThreadSafeTimeEntry>();
                timeEntry.Id.Returns(123);
                timeEntry.Start.Returns(DateTimeOffset.Now);
                timeEntry.Duration.Returns(100);
                InteractorFactory.ObserveAllTimeEntriesVisibleToTheUser().Execute()
                    .Returns(Observable.Return(new[] { timeEntry }));
            }

            protected void PrepareIsWelcome(bool isWelcome)
            {
                var subject = new BehaviorSubject<bool>(isWelcome);
                OnboardingStorage.IsNewUser.Returns(subject.AsObservable());
            }
        }

        public sealed class TheShouldShowEmptyStateProperty : InitialStateTest
        {
            [Fact, LogIfTooSlow]
            public async ThreadingTask ReturnsTrueWhenThereAreNoSuggestionsAndNoTimeEntriesAndIsWelcome()
            {
                PrepareIsWelcome(true);
                var viewModel = CreateViewModel();
                await viewModel.Initialize();
                var observer = TestScheduler.CreateObserver<bool>();

                viewModel.ShouldShowEmptyState.Subscribe(observer);

                TestScheduler.Start();
                observer.LastEmittedValue().Should().BeTrue();
            }

            [Fact, LogIfTooSlow]
            public async ThreadingTask ReturnsFalseWhenThereAreSomeSuggestions()
            {
                PrepareSuggestion();
                var viewModel = CreateViewModel();
                await viewModel.Initialize();

                var observer = TestScheduler.CreateObserver<bool>();

                viewModel.ShouldShowEmptyState.Subscribe(observer);

                TestScheduler.Start();
                observer.LastEmittedValue().Should().BeFalse();
            }

            [Fact, LogIfTooSlow]
            public async ThreadingTask ReturnsFalseWhenThereAreSomeTimeEntries()
            {
                PrepareTimeEntry();
                var viewModel = CreateViewModel();
                await viewModel.Initialize();

                var observer = TestScheduler.CreateObserver<bool>();

                viewModel.ShouldShowEmptyState.Subscribe(observer);

                TestScheduler.Start();
                observer.LastEmittedValue().Should().BeFalse();
            }

            [Fact, LogIfTooSlow]
            public async ThreadingTask ReturnsFalseWhenIsNotWelcome()
            {
                PrepareIsWelcome(false);
                var viewModel = CreateViewModel();
                await viewModel.Initialize();
                var observer = TestScheduler.CreateObserver<bool>();

                viewModel.ShouldShowEmptyState.Subscribe(observer);

                TestScheduler.Start();

                observer.LastEmittedValue().Should().BeFalse();
            }
        }

        public sealed class TheShouldShowWelcomeBackProperty : InitialStateTest
        {
            [Fact, LogIfTooSlow]
            public async ThreadingTask ReturnsTrueWhenThereAreNoSuggestionsAndNoTimeEntriesAndIsNotWelcome()
            {
                PrepareIsWelcome(false);
                var viewModel = CreateViewModel();
                await viewModel.Initialize();
                var observer = TestScheduler.CreateObserver<bool>();

                viewModel.ShouldShowWelcomeBack.Subscribe(observer);

                TestScheduler.Start();
                observer.Messages.AssertEqual(
                    ReactiveTest.OnNext(1, false),
                    ReactiveTest.OnNext(3, true)
                );
            }

            [Fact, LogIfTooSlow]
            public async ThreadingTask ReturnsFalseWhenThereAreSomeSuggestions()
            {
                PrepareSuggestion();
                var viewModel = CreateViewModel();
                await viewModel.Initialize();
                var observer = TestScheduler.CreateObserver<bool>();

                viewModel.ShouldShowWelcomeBack.Subscribe(observer);

                TestScheduler.Start();
                observer.LastEmittedValue().Should().BeFalse();
            }

            [Fact, LogIfTooSlow]
            public async ThreadingTask ReturnsFalseWhenThereAreSomeTimeEntries()
            {
                PrepareTimeEntry();
                var viewModel = CreateViewModel();
                await viewModel.Initialize();
                var observer = TestScheduler.CreateObserver<bool>();

                viewModel.ShouldShowWelcomeBack.Subscribe(observer);

                TestScheduler.Start();
                observer.LastEmittedValue().Should().BeFalse();
            }

            [Fact, LogIfTooSlow]
            public async ThreadingTask ReturnsFalseWhenIsWelcome()
            {
                PrepareIsWelcome(true);
                var viewModel = CreateViewModel();
                await viewModel.Initialize();
                var observer = TestScheduler.CreateObserver<bool>();

                viewModel.ShouldShowWelcomeBack.Subscribe(observer);

                TestScheduler.Start();
                observer.LastEmittedValue().Should().BeFalse();
            }
        }

        public sealed class TheInitializeMethod : MainViewModelTest
        {
            [Fact, LogIfTooSlow]
            public async void ReportsUserIdToAppCenter()
            {
                var userId = 1234567890L;
                var user = Substitute.For<IThreadSafeUser>();
                user.Id.Returns(userId);
                InteractorFactory.GetCurrentUser().Execute().Returns(Observable.Return(user));
                await ViewModel.Initialize();

                AnalyticsService.Received().SetAppCenterUserId(userId);
            }

            public sealed class WhenShowingTheRatingsView : MainViewModelTest
            {
                [Fact, LogIfTooSlow]
                public async void DoesNotShowTheRatingViewByDefault()
                {
                    await ViewModel.Initialize();

                    var observer = TestScheduler.CreateObserver<bool>();
                    ViewModel.ShouldShowRatingView.Subscribe(observer);

                    TestScheduler.Start();
                    observer.LastEmittedValue().Should().BeFalse();
                }

                [Fact, LogIfTooSlow]
                public async void ShowsTheRatingView()
                {
                    var defaultRemoteConfiguration = new RatingViewConfiguration(5, RatingViewCriterion.Start);
                    RemoteConfigService
                        .GetRatingViewConfiguration()
                        .Returns(defaultRemoteConfiguration);

                    var now = DateTimeOffset.Now;
                    var firstOpened = now - TimeSpan.FromDays(5);

                    TimeService.CurrentDateTime.Returns(now);
                    OnboardingStorage.GetFirstOpened().Returns(firstOpened);

                    await ViewModel.Initialize();
                    var observer = TestScheduler.CreateObserver<bool>();
                    ViewModel.ShouldShowRatingView.Subscribe(observer);

                    TestScheduler.Start();
                    observer.LastEmittedValue().Should().BeTrue();
                }

                [Fact, LogIfTooSlow]
                public async void DoesNotShowTheRatingViewIfThereWasAnInteraction()
                {
                    var defaultRemoteConfiguration = new RatingViewConfiguration(5, RatingViewCriterion.Start);
                    RemoteConfigService
                        .GetRatingViewConfiguration()
                        .Returns(defaultRemoteConfiguration);

                    var now = DateTimeOffset.Now;
                    var firstOpened = now - TimeSpan.FromDays(6);

                    TimeService.CurrentDateTime.Returns(now);
                    OnboardingStorage.GetFirstOpened().Returns(firstOpened);
                    OnboardingStorage.RatingViewOutcome().Returns(RatingViewOutcome.AppWasNotRated);

                    await ViewModel.Initialize();

                    var observer = TestScheduler.CreateObserver<bool>();
                    ViewModel.ShouldShowRatingView.Subscribe(observer);

                    TestScheduler.Start();
                    observer.LastEmittedValue().Should().BeFalse();
                }

                [Fact, LogIfTooSlow]
                public async void DoesNotShowTheRatingViewIfAfter24HourSnooze()
                {
                    var defaultRemoteConfiguration = new RatingViewConfiguration(5, RatingViewCriterion.Start);
                    RemoteConfigService
                        .GetRatingViewConfiguration()
                        .Returns(defaultRemoteConfiguration);

                    var now = DateTimeOffset.Now;
                    var firstOpened = now - TimeSpan.FromDays(6);
                    var lastInteraction = now - TimeSpan.FromDays(2);

                    TimeService.CurrentDateTime.Returns(now);
                    OnboardingStorage.GetFirstOpened().Returns(firstOpened);
                    OnboardingStorage.RatingViewOutcome().Returns(RatingViewOutcome.AppWasNotRated);
                    OnboardingStorage.RatingViewOutcomeTime().Returns(lastInteraction);

                    await ViewModel.Initialize();

                    var observer = TestScheduler.CreateObserver<bool>();
                    ViewModel.ShouldShowRatingView.Subscribe(observer);

                    TestScheduler.Start();
                    observer.LastEmittedValue().Should().BeFalse();
                }

                [Theory, LogIfTooSlow]
                [InlineData(ApplicationInstallLocation.Internal, Platform.Giskard, true)]
                [InlineData(ApplicationInstallLocation.External, Platform.Giskard, true)]
                [InlineData(ApplicationInstallLocation.Unknown, Platform.Giskard, true)]
                [InlineData(ApplicationInstallLocation.Internal, Platform.Daneel, false)]
                [InlineData(ApplicationInstallLocation.External, Platform.Daneel, false)]
                [InlineData(ApplicationInstallLocation.Unknown, Platform.Daneel, false)]
                public async void TracksApplicationInstallLocation(ApplicationInstallLocation location, Platform platform, bool shouldTrack)
                {
                    PlatformInfo.InstallLocation.Returns(location);
                    PlatformInfo.Platform.Returns(platform);

                    await ViewModel.Initialize();
                    TestScheduler.Start();

                    if (shouldTrack)
                        AnalyticsService.ApplicationInstallLocation.Received().Track(location);
                    else
                        AnalyticsService.ApplicationInstallLocation.DidNotReceive().Track(location);
                }
            }

            [Fact]
            public async void StartsTheWidgetsService()
            {
                await ViewModel.Initialize();
                WidgetsService.Received().Start();
            }
        }
    }
}
