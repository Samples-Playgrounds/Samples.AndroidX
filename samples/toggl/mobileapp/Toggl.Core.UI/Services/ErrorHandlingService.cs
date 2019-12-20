using System;
using Toggl.Core.Exceptions;
using Toggl.Core.Services;
using Toggl.Core.UI.Navigation;
using Toggl.Core.UI.ViewModels;
using Toggl.Networking.Exceptions;
using Toggl.Shared;
using Toggl.Storage.Settings;

namespace Toggl.Core.UI.Services
{
    public sealed class ErrorHandlingService : IErrorHandlingService
    {
        private readonly INavigationService navigationService;
        private readonly IAccessRestrictionStorage accessRestrictionStorage;

        public ErrorHandlingService(
            INavigationService navigationService,
            IAccessRestrictionStorage accessRestrictionStorage)
        {
            Ensure.Argument.IsNotNull(navigationService, nameof(navigationService));
            Ensure.Argument.IsNotNull(accessRestrictionStorage, nameof(accessRestrictionStorage));

            this.navigationService = navigationService;
            this.accessRestrictionStorage = accessRestrictionStorage;
        }

        public bool TryHandleDeprecationError(Exception error)
        {
            switch (error)
            {
                case ApiDeprecatedException _:
                    accessRestrictionStorage.SetApiOutdated();
                    navigationService.Navigate<OutdatedAppViewModel>(null);
                    return true;
                case ClientDeprecatedException _:
                    accessRestrictionStorage.SetClientOutdated();
                    navigationService.Navigate<OutdatedAppViewModel>(null);
                    return true;
            }

            return false;
        }

        public bool TryHandleUnauthorizedError(Exception error)
        {
            if (error is UnauthorizedException unauthorized)
            {
                var token = unauthorized.ApiToken;
                if (token != null)
                {
                    accessRestrictionStorage.SetUnauthorizedAccess(token);
                    navigationService.Navigate<TokenResetViewModel>(null);
                }

                return true;
            }

            return false;
        }

        public bool TryHandleNoWorkspaceError(Exception error)
        {
            if (error is NoWorkspaceException)
            {
                accessRestrictionStorage.SetNoWorkspaceStateReached(true);
                return true;
            }

            return false;
        }

        public bool TryHandleNoDefaultWorkspaceError(Exception error)
        {
            if (error is NoDefaultWorkspaceException)
            {
                accessRestrictionStorage.SetNoDefaultWorkspaceStateReached(true);
                return true;
            }

            return false;
        }
    }
}
