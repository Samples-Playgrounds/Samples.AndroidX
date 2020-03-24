using Microsoft.Reactive.Testing;
using NSubstitute;
using Toggl.Core.Analytics;
using Toggl.Core.DataSources;
using Toggl.Core.Interactors;
using Toggl.Core.Services;
using Toggl.Core.Shortcuts;
using Toggl.Core.UI.Navigation;
using Toggl.Core.UI.Services;
using Toggl.Storage;
using Toggl.Storage.Settings;

namespace Toggl.Core.Tests.UI
{
    public abstract class BaseTest : ReactiveTest
    {
        protected IIdProvider IdProvider { get; } = Substitute.For<IIdProvider>();
        protected ITimeService TimeService { get; } = Substitute.For<ITimeService>();
        protected ITogglDataSource DataSource { get; } = Substitute.For<ITogglDataSource>();
        protected IUserPreferences UserPreferences { get; } = Substitute.For<IUserPreferences>();
        protected ICalendarService CalendarService { get; } = Substitute.For<ICalendarService>();
        protected IAnalyticsService AnalyticsService { get; } = Substitute.For<IAnalyticsService>();
        protected IInteractorFactory InteractorFactory { get; } = Substitute.For<IInteractorFactory>();
        protected IPermissionsChecker PermissionsChecker { get; } = Substitute.For<IPermissionsChecker>();
        protected IApplicationShortcutCreator ApplicationShortcutCreator { get; }
            = Substitute.For<IApplicationShortcutCreator>();

        protected INavigationService NavigationService { get; } = Substitute.For<INavigationService>();
        protected IPresenter ViewPresenter { get; } = Substitute.For<IPresenter>();
        protected TestSchedulerProvider SchedulerProvider { get; } = new TestSchedulerProvider();
        protected IPrivateSharedStorageService PrivateSharedStorageService { get; } = Substitute.For<IPrivateSharedStorageService>();
    }
}
