using System.Reactive.Linq;
using System.Threading.Tasks;
using Toggl.Core.Services;
using Toggl.Core.UI.Navigation;
using Toggl.Core.UI.Services;
using Toggl.Shared;
using Toggl.Shared.Extensions;

namespace Toggl.Core.UI.ViewModels.Calendar
{
    [Preserve(AllMembers = true)]
    public sealed class CalendarPermissionDeniedViewModel : ViewModel
    {
        private readonly IPermissionsChecker permissionsChecker;
        private readonly IRxActionFactory rxActionFactory;

        public ViewAction EnableAccess { get; }

        public CalendarPermissionDeniedViewModel(INavigationService navigationService, IPermissionsChecker permissionsChecker, IRxActionFactory rxActionFactory)
            : base(navigationService)
        {
            Ensure.Argument.IsNotNull(permissionsChecker, nameof(permissionsChecker));
            Ensure.Argument.IsNotNull(rxActionFactory, nameof(rxActionFactory));

            this.permissionsChecker = permissionsChecker;

            EnableAccess = rxActionFactory.FromAction(enableAccess);
        }

        public override void ViewAppeared()
        {
            base.ViewAppeared();
            closeIfPermissionIsGranted();
        }

        private void enableAccess()
        {
            View.OpenAppSettings();
        }

        private async Task closeIfPermissionIsGranted()
        {
            var authorized = await permissionsChecker.CalendarPermissionGranted;
            if (authorized)
                Close();
        }
    }
}
