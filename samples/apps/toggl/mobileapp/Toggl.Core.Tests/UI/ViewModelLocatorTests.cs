using FluentAssertions;
using NSubstitute;
using System;
using System.Collections.Generic;
using Toggl.Core.Analytics;
using Toggl.Core.DataSources;
using Toggl.Core.Interactors;
using Toggl.Core.Login;
using Toggl.Core.Services;
using Toggl.Core.Shortcuts;
using Toggl.Core.Sync;
using Toggl.Core.Tests.Generators;
using Toggl.Core.UI;
using Toggl.Core.UI.Navigation;
using Toggl.Core.UI.Services;
using Toggl.Shared;
using Toggl.Storage;
using Toggl.Storage.Queries;
using Toggl.Storage.Settings;
using Xunit;

namespace Toggl.Core.Tests.UI
{
    public class ViewModelLocatorTests : BaseTest
    {
        [Theory, LogIfTooSlow]
        [ViewModelTypeData]
        public void IsAbleToCreateEveryViewModel(Type viewModelType)
        {
            var container = createContainer();
            var loader = new ViewModelLoader(container);

            var loadMethod = typeof(ViewModelLoader)
                .GetMethod(nameof(ViewModelLoader.Load));

            var genericLoadMethod = loadMethod.MakeGenericMethod(viewModelType);

            Action tryingToFindAViewModel = () => genericLoadMethod.Invoke(loader, new object[0]);
            tryingToFindAViewModel.Should().NotThrow();
        }

        private TestDependencyContainer createContainer()
        {
            var container = new TestDependencyContainer
            {
                MockUserAccessManager = Substitute.For<IUserAccessManager>(),
                MockAccessRestrictionStorage = Substitute.For<IAccessRestrictionStorage>(),
                MockAnalyticsService = Substitute.For<IAnalyticsService>(),
                MockBackgroundSyncService = Substitute.For<IBackgroundSyncService>(),
                MockCalendarService = Substitute.For<ICalendarService>(),
                MockDatabase = Substitute.For<ITogglDatabase>(),
                MockDataSource = Substitute.For<ITogglDataSource>(),
                MockKeyValueStorage = Substitute.For<IKeyValueStorage>(),
                MockLastTimeUsageStorage = Substitute.For<ILastTimeUsageStorage>(),
                MockLicenseProvider = Substitute.For<ILicenseProvider>(),
                MockQueryFactory = Substitute.For<IQueryFactory>(),
                MockNavigationService = Substitute.For<INavigationService>(),
                MockNotificationService = Substitute.For<INotificationService>(),
                MockAccessibilityService = Substitute.For<IAccessibilityService>(),
                MockOnboardingStorage = Substitute.For<IOnboardingStorage>(),
                MockPermissionsChecker = Substitute.For<IPermissionsChecker>(),
                MockPlatformInfo = Substitute.For<IPlatformInfo>(),
                MockPrivateSharedStorageService = Substitute.For<IPrivateSharedStorageService>(),
                MockRatingService = Substitute.For<IRatingService>(),
                MockRemoteConfigService = Substitute.For<IRemoteConfigService>(),
                MockSchedulerProvider = Substitute.For<ISchedulerProvider>(),
                MockShortcutCreator = Substitute.For<IApplicationShortcutCreator>(),
                MockUserPreferences = Substitute.For<IUserPreferences>(),
                MockInteractorFactory = Substitute.For<IInteractorFactory>(),
                MockTimeService = Substitute.For<ITimeService>(),
                MockSyncManager = Substitute.For<ISyncManager>(),
                MockPushNotificationsTokenService = Substitute.For<IPushNotificationsTokenService>(),
                MockUpdateRemoteConfigCacheService = Substitute.For<IUpdateRemoteConfigCacheService>(),
                MockWidgetsService = Substitute.For<IWidgetsService>()
            };

            container.MockLicenseProvider.GetAppLicenses().Returns(new Dictionary<string, string>());

            return container;
        }
    }
}
