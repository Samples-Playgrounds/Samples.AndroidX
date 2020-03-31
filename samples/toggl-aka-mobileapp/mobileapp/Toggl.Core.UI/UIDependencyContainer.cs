using System;
using System.Transactions;
using Toggl.Core.Services;
using Toggl.Core.UI.Navigation;
using Toggl.Core.UI.Services;
using Toggl.Networking;
using Toggl.Networking.Network;

namespace Toggl.Core.UI
{
    public abstract class UIDependencyContainer : DependencyContainer
    {
        private readonly Lazy<IDeeplinkParser> deeplinkParser;
        private readonly Lazy<ViewModelLoader> viewModelLoader;
        private readonly Lazy<INavigationService> navigationService;
        private readonly Lazy<IPermissionsChecker> permissionsService;
        private Lazy<IWidgetsService> widgetsService;

        public IDeeplinkParser DeeplinkParser => deeplinkParser.Value;
        public ViewModelLoader ViewModelLoader => viewModelLoader.Value;
        public INavigationService NavigationService => navigationService.Value;
        public IPermissionsChecker PermissionsChecker => permissionsService.Value;
        public IWidgetsService WidgetsService => widgetsService.Value;

        public static UIDependencyContainer Instance { get; protected set; }

        protected UIDependencyContainer(ApiEnvironment apiEnvironment, UserAgent userAgent)
            : base(apiEnvironment, userAgent)
        {
            deeplinkParser = new Lazy<IDeeplinkParser>(createDeeplinkParser);
            viewModelLoader = new Lazy<ViewModelLoader>(CreateViewModelLoader);
            navigationService = new Lazy<INavigationService>(CreateNavigationService);
            permissionsService = new Lazy<IPermissionsChecker>(CreatePermissionsChecker);
            widgetsService = new Lazy<IWidgetsService>(CreateWidgetsService);
        }

        private IDeeplinkParser createDeeplinkParser()
            => new DeeplinkParser();

        protected abstract INavigationService CreateNavigationService();
        protected abstract IPermissionsChecker CreatePermissionsChecker();
        protected abstract IWidgetsService CreateWidgetsService();

        protected virtual ViewModelLoader CreateViewModelLoader() => new ViewModelLoader(this);
        protected override IErrorHandlingService CreateErrorHandlingService()
            => new ErrorHandlingService(NavigationService, AccessRestrictionStorage);

        protected override IRemoteConfigService CreateRemoteConfigService()
            => new RemoteConfigService(KeyValueStorage);

        protected override void RecreateLazyDependenciesForLogin(ITogglApi togglApi)
        {
            base.RecreateLazyDependenciesForLogin(togglApi);

            if (widgetsService.IsValueCreated)
            {
                WidgetsService?.Dispose();
                widgetsService = new Lazy<IWidgetsService>(CreateWidgetsService);
            }
        }

        protected override void RecreateLazyDependenciesForLogout()
        {
            base.RecreateLazyDependenciesForLogout();
            WidgetsService?.Dispose();
            widgetsService = new Lazy<IWidgetsService>(CreateWidgetsService);
        }
    }
}
