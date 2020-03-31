using FluentAssertions;
using System;
using Toggl.Core.Tests.Generators;
using Toggl.Core.UI.ViewModels;
using Xunit;

namespace Toggl.Core.Tests.UI.ViewModels
{
    public class MainTabViewModelTests
    {
        public abstract class MainTabViewModelTest : BaseViewModelTests<MainTabBarViewModel>
        {
            protected override MainTabBarViewModel CreateViewModel()
                => new MainTabBarViewModel(
                    TimeService,
                    DataSource,
                    SyncManager,
                    RatingService,
                    UserPreferences,
                    AnalyticsService,
                    BackgroundService,
                    InteractorFactory,
                    OnboardingStorage,
                    SchedulerProvider,
                    PermissionsChecker,
                    NavigationService,
                    RemoteConfigService,
                    AccessibilityService,
                    UpdateRemoteConfigCacheService,
                    AccessRestrictionStorage,
                    RxActionFactory,
                    UserAccessManager,
                    PrivateSharedStorageService,
                    PlatformInfo,
                    WidgetsService,
                    LastTimeUsageStorage
                );
        }

        public sealed class TheConstructor : MainTabViewModelTest
        {
            [Theory, LogIfTooSlow]
            [ConstructorData]
            public void ThrowsIfAnyOfTheArgumentsIsNull(
                    bool useTimeService,
                    bool useDataSource,
                    bool useSyncManager,
                    bool useRatingService,
                    bool useUserPreferences,
                    bool useAnalyticsService,
                    bool useBackgroundService,
                    bool useInteractorFactory,
                    bool useOnboardingStorage,
                    bool useSchedulerProvider,
                    bool usePermissionsChecker,
                    bool useNavigationService,
                    bool useRemoteConfigService,
                    bool useAccessibilityService,
                    bool useRemoteConfigUpdateService,
                    bool useAccessRestrictionStorage,
                    bool useRxActionFactory,
                    bool useUserAccessManager,
                    bool usePrivateSharedStorageService,
                    bool usePlatformInfo,
                    bool useWidgetsService,
                    bool useLastTimeUsageStorage)
            {
                var timeService = useTimeService ? TimeService : null;
                var dataSource = useDataSource ? DataSource : null;
                var syncManager = useSyncManager ? SyncManager : null;
                var ratingService = useRatingService ? RatingService : null;
                var userPreferences = useUserPreferences ? UserPreferences : null;
                var analyticsService = useAnalyticsService ? AnalyticsService : null;
                var interactorFactory = useInteractorFactory ? InteractorFactory : null;
                var onboardingStorage = useOnboardingStorage ? OnboardingStorage : null;
                var backgroundService = useBackgroundService ? BackgroundService : null;
                var schedulerProvider = useSchedulerProvider ? SchedulerProvider : null;
                var permissionsService = usePermissionsChecker ? PermissionsChecker : null;
                var navigationService = useNavigationService ? NavigationService : null;
                var remoteConfigService = useRemoteConfigService ? RemoteConfigService : null;
                var accessibilityService = useAccessibilityService ? AccessibilityService : null;
                var remoteConfigUpdateService = useRemoteConfigUpdateService ? UpdateRemoteConfigCacheService : null;
                var accessRestrictionStorage = useAccessRestrictionStorage ? AccessRestrictionStorage : null;
                var rxActionFactory = useRxActionFactory ? RxActionFactory : null;
                var userAccessManager = useUserAccessManager ? UserAccessManager : null;
                var privateSharedStorageService = usePrivateSharedStorageService ? PrivateSharedStorageService : null;
                var platformInfo = usePlatformInfo ? PlatformInfo : null;
                var widgetsService = useWidgetsService ? WidgetsService : null;
                var lastTimeUsageStorage = useLastTimeUsageStorage ? LastTimeUsageStorage : null;

                Action tryingToConstructWithEmptyParameters =
                    () => new MainTabBarViewModel(
                        timeService,
                        dataSource,
                        syncManager,
                        ratingService,
                        userPreferences,
                        analyticsService,
                        backgroundService,
                        interactorFactory,
                        onboardingStorage,
                        schedulerProvider,
                        permissionsService,
                        navigationService,
                        remoteConfigService,
                        accessibilityService,
                        remoteConfigUpdateService,
                        accessRestrictionStorage,
                        rxActionFactory,
                        userAccessManager,
                        privateSharedStorageService,
                        platformInfo,
                        widgetsService,
                        lastTimeUsageStorage
                    );

                tryingToConstructWithEmptyParameters
                    .Should().Throw<ArgumentNullException>();
            }
        }
    }
}
