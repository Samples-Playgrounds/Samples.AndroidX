using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using Toggl.Core.Interactors;
using Toggl.Core.Models.Interfaces;
using Toggl.Core.Services;
using Toggl.Core.UI.Navigation;
using Toggl.Core.UI.Parameters;
using Toggl.Shared;
using Toggl.Shared.Extensions;
using static Toggl.Core.Helper.Constants;

namespace Toggl.Core.UI.ViewModels
{
    [Preserve(AllMembers = true)]
    public sealed class SelectClientViewModel : ViewModel<SelectClientParameters, long?>
    {
        private readonly IRxActionFactory rxActionFactory;
        private readonly IInteractorFactory interactorFactory;
        private readonly ISchedulerProvider schedulerProvider;

        private long workspaceId;
        private long selectedClientId;
        private SelectableClientViewModel noClient;

        public InputAction<SelectableClientBaseViewModel> SelectClient { get; }
        public ISubject<string> FilterText { get; } = new BehaviorSubject<string>(string.Empty);
        public IObservable<IImmutableList<SelectableClientBaseViewModel>> Clients { get; private set; }

        public SelectClientViewModel(
            IInteractorFactory interactorFactory,
            INavigationService navigationService,
            ISchedulerProvider schedulerProvider,
            IRxActionFactory rxActionFactory)
            : base(navigationService)
        {
            Ensure.Argument.IsNotNull(interactorFactory, nameof(interactorFactory));
            Ensure.Argument.IsNotNull(schedulerProvider, nameof(schedulerProvider));
            Ensure.Argument.IsNotNull(rxActionFactory, nameof(rxActionFactory));

            this.interactorFactory = interactorFactory;
            this.rxActionFactory = rxActionFactory;
            this.schedulerProvider = schedulerProvider;

            SelectClient = rxActionFactory.FromAsync<SelectableClientBaseViewModel>(selectClient);
        }

        public override async Task Initialize(SelectClientParameters parameter)
        {
            await base.Initialize(parameter);

            workspaceId = parameter.WorkspaceId;
            selectedClientId = parameter.SelectedClientId;
            noClient = new SelectableClientViewModel(0, Resources.NoClient, selectedClientId == 0);

            var allClients = await interactorFactory
                .GetAllClientsInWorkspace(workspaceId)
                .Execute();

            Clients = FilterText
                .Select(text => text?.Trim() ?? string.Empty)
                .DistinctUntilChanged()
                .Select(trimmedText => filterClientsByText(trimmedText, allClients))
                .AsDriver(ImmutableList<SelectableClientBaseViewModel>.Empty, schedulerProvider);
        }

        private IImmutableList<SelectableClientBaseViewModel> filterClientsByText(string trimmedText, IEnumerable<IThreadSafeClient> allClients)
        {
            var selectableViewModels = allClients
                .Where(c => c.Name.ContainsIgnoringCase(trimmedText))
                .OrderBy(client => client.Name)
                .Select(toSelectableViewModel);

            var isClientFilterEmpty = string.IsNullOrEmpty(trimmedText);
            var suggestCreation = !isClientFilterEmpty
                && allClients.None(c => c.Name == trimmedText)
                && trimmedText.LengthInBytes() <= MaxClientNameLengthInBytes;

            if (suggestCreation)
            {
                var creationSelectableViewModel = new SelectableClientCreationViewModel(trimmedText);
                selectableViewModels = selectableViewModels.Prepend(creationSelectableViewModel);
            }
            else if (isClientFilterEmpty)
            {
                selectableViewModels = selectableViewModels.Prepend(noClient);
            }

            return selectableViewModels.ToImmutableList();
        }

        private SelectableClientBaseViewModel toSelectableViewModel(IThreadSafeClient client)
            => new SelectableClientViewModel(client.Id, client.Name, client.Id == selectedClientId);

        private async Task selectClient(SelectableClientBaseViewModel client)
        {
            switch (client)
            {
                case SelectableClientCreationViewModel c:
                    var newClient = await interactorFactory.CreateClient(c.Name.Trim(), workspaceId).Execute();
                    Close(newClient.Id);
                    break;
                case SelectableClientViewModel c:
                    Close(c.Id);
                    break;
            }
        }
    }
}
