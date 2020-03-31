using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Toggl.Core.Interactors;
using Toggl.Core.Models.Interfaces;
using Toggl.Core.Services;
using Toggl.Core.UI.Navigation;
using Toggl.Core.UI.ViewModels.Settings;
using Toggl.Shared;
using Toggl.Shared.Extensions;

namespace Toggl.Core.UI.ViewModels
{
    [Preserve(AllMembers = true)]
    public sealed class SiriShortcutsViewModel : ViewModel
    {
        private readonly IInteractorFactory interactorFactory;
        private readonly ISchedulerProvider schedulerProvider;

        public ViewAction NavigateToCustomReportShortcut { get; }
        public ViewAction NavigateToCustomTimeEntryShortcut { get; }

        public SiriShortcutsViewModel(
            IInteractorFactory interactorFactory,
            IRxActionFactory rxActionFactory,
            ISchedulerProvider schedulerProvider,
            INavigationService navigationService) : base(navigationService)
        {
            Ensure.Argument.IsNotNull(interactorFactory, nameof(interactorFactory));
            Ensure.Argument.IsNotNull(rxActionFactory, nameof(rxActionFactory));
            Ensure.Argument.IsNotNull(schedulerProvider, nameof(schedulerProvider));

            this.interactorFactory = interactorFactory;
            this.schedulerProvider = schedulerProvider;

            NavigateToCustomReportShortcut = rxActionFactory.FromAsync(navigateToCustomReportShortcut);
            NavigateToCustomTimeEntryShortcut = rxActionFactory.FromAsync(navigateToCustomTimeEntryShortcut);
        }

        private Task navigateToCustomReportShortcut()
            => Navigate<SiriShortcutsSelectReportPeriodViewModel>();

        private Task navigateToCustomTimeEntryShortcut()
            => Navigate<SiriShortcutsCustomTimeEntryViewModel>();

        public IObservable<IThreadSafeProject> GetProject(long projectId)
            => interactorFactory.GetProjectById(projectId).Execute()
                .ObserveOn(schedulerProvider.MainScheduler);

        public IObservable<IEnumerable<IThreadSafeWorkspace>> GetUserWorkspaces()
            => interactorFactory.GetAllWorkspaces().Execute()
                .Select(ws => ws.ToList()) // <- This is to avoid Realm threading issues
                .ObserveOn(schedulerProvider.MainScheduler);
    }
}
