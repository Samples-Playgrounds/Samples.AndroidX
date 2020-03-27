using FluentAssertions;
using NSubstitute;
using System;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Toggl.Core.Tests.Generators;
using Toggl.Core.UI.Navigation;
using Toggl.Core.UI.ViewModels.Settings;
using Xunit;

namespace Toggl.Core.Tests.UI.ViewModels
{
    public sealed class NotificationSettingsViewModelTests
    {
        public abstract class NotificationSettingsViewModelTest : BaseViewModelTests<NotificationSettingsViewModel>
        {
            protected override NotificationSettingsViewModel CreateViewModel()
                => new NotificationSettingsViewModel(NavigationService, BackgroundService, PermissionsChecker, UserPreferences, SchedulerProvider, RxActionFactory);
        }

        public sealed class TheConstructor : NotificationSettingsViewModelTest
        {
            [Theory, LogIfTooSlow]
            [ConstructorData]
            public void ThrowsIfAnyOfTheArgumentsIsNull(
                bool useNavigationService,
                bool useBackgroundService,
                bool usePermissionsChecker,
                bool useUserPreferences,
                bool useSchedulerProvider,
                bool useRxActionFactory)
            {
                Action tryingToConstructWithEmptyParameters =
                    () => new NotificationSettingsViewModel(
                        useNavigationService ? NavigationService : null,
                        useBackgroundService ? BackgroundService : null,
                        usePermissionsChecker ? PermissionsChecker : null,
                        useUserPreferences ? UserPreferences : null,
                        useSchedulerProvider ? SchedulerProvider : null,
                        useRxActionFactory ? RxActionFactory : null
                    );

                tryingToConstructWithEmptyParameters.Should().Throw<ArgumentNullException>();
            }
        }

        public sealed class ThePermissionGrantedProperty : NotificationSettingsViewModelTest
        {
            [Theory, LogIfTooSlow]
            [InlineData(true)]
            [InlineData(false)]
            public async Task GetsInitialisedToTheProperValue(bool permissionGranted)
            {
                var observer = TestScheduler.CreateObserver<bool>();
                PermissionsChecker.NotificationPermissionGranted.Returns(Observable.Return(permissionGranted));

                var viewModel = new NotificationSettingsViewModel(NavigationService, BackgroundService, PermissionsChecker, UserPreferences, SchedulerProvider, RxActionFactory);
                viewModel.PermissionGranted.Subscribe(observer);

                await viewModel.Initialize();
                TestScheduler.Start();

                observer.Messages.First().Value.Value.Should().Be(permissionGranted);
            }
        }

        public sealed class TheRequestAccessAction : NotificationSettingsViewModelTest
        {
            [Fact, LogIfTooSlow]
            public async Task OpensAppSettings()
            {
                ViewModel.RequestAccess.Execute(Unit.Default);
                TestScheduler.Start();

                View.Received().OpenAppSettings();
            }
        }

        public sealed class TheOpenUpcomingEventsAction : NotificationSettingsViewModelTest
        {
            [Fact, LogIfTooSlow]
            public async Task NavigatesToTheUpcomingEvents()
            {
                ViewModel.OpenUpcomingEvents.Execute();
                TestScheduler.Start();

                NavigationService.Received().Navigate<UpcomingEventsNotificationSettingsViewModel, Unit>(View);
            }
        }
    }
}
