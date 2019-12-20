using System.Threading.Tasks;
using Toggl.Core.Services;
using Toggl.Core.UI.Navigation;
using Toggl.Shared;
using Toggl.Shared.Extensions;
using Toggl.Storage.Settings;

namespace Toggl.Core.UI.ViewModels.Settings.Siri
{
    public class PasteFromClipboardViewModel : ViewModel
    {
        private readonly IOnboardingStorage onboardingStorage;

        public ViewAction DoNotShowAgain { get; }

        public PasteFromClipboardViewModel(
            IRxActionFactory rxActionFactory,
            IOnboardingStorage onboardingStorage,
            INavigationService navigationService) : base(navigationService)
        {
            Ensure.Argument.IsNotNull(onboardingStorage, nameof(onboardingStorage));
            Ensure.Argument.IsNotNull(rxActionFactory, nameof(rxActionFactory));

            this.onboardingStorage = onboardingStorage;

            DoNotShowAgain = rxActionFactory.FromAction(doNotShowAgain);
        }

        private void doNotShowAgain()
        {
            onboardingStorage.SetDidShowSiriClipboardInstruction(true);
            Close();
        }
    }
}
