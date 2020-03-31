using System.Collections.Immutable;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Toggl.Core.Interactors;
using Toggl.Core.Models;
using Toggl.Core.UI.Navigation;
using Toggl.Shared;

namespace Toggl.Core.UI.ViewModels
{
    [Preserve(AllMembers = true)]
    public class SyncFailuresViewModel : ViewModel
    {
        public IImmutableList<SyncFailureItem> SyncFailures { get; private set; }

        private readonly IInteractorFactory interactorFactory;

        public SyncFailuresViewModel(IInteractorFactory interactorFactory, INavigationService navigationService)
            : base(navigationService)
        {
            Ensure.Argument.IsNotNull(interactorFactory, nameof(interactorFactory));

            this.interactorFactory = interactorFactory;
        }

        public override async Task Initialize()
        {
            await base.Initialize();

            var syncFailures = await interactorFactory
                .GetItemsThatFailedToSync()
                .Execute()
                .FirstAsync();

            SyncFailures = syncFailures.ToImmutableList();
        }
    }
}
