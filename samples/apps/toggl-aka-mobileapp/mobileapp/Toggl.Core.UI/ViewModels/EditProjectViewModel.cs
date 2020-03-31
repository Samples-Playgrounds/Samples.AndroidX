using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Threading.Tasks;
using Toggl.Core.DataSources;
using Toggl.Core.DTOs;
using Toggl.Core.Helper;
using Toggl.Core.Interactors;
using Toggl.Core.Models.Interfaces;
using Toggl.Core.Services;
using Toggl.Core.UI.Extensions;
using Toggl.Core.UI.Navigation;
using Toggl.Core.UI.Parameters;
using Toggl.Core.UI.Views;
using Toggl.Shared;
using Toggl.Shared.Extensions;
using Toggl.Shared.Extensions.Reactive;
using static Toggl.Core.Helper.Colors;

namespace Toggl.Core.UI.ViewModels
{
    [Preserve(AllMembers = true)]
    public sealed class EditProjectViewModel : ViewModel<string, long?>
    {
        private const long noClientId = 0;

        private readonly Random random = new Random();
        private readonly IInteractorFactory interactorFactory;
        private readonly ITogglDataSource dataSource;

        private long initialWorkspaceId;
        private readonly IObservable<IThreadSafeClient> currentClient;
        private readonly IObservable<IThreadSafeWorkspace> currentWorkspace;

        public string Title { get; } = Resources.NewProject;
        public string DoneButtonText { get; } = Resources.Create;

        public BehaviorRelay<string> Name { get; }
        public BehaviorRelay<bool> IsPrivate { get; }
        public IObservable<Color> Color { get; }
        public IObservable<string> ClientName { get; }
        public IObservable<string> WorkspaceName { get; }
        public IObservable<bool> CanCreatePublicProjects { get; }
        public ViewAction ToggleIsPrivate { get; }
        public ViewAction Save { get; }
        public OutputAction<Color> PickColor { get; }
        public OutputAction<IThreadSafeClient> PickClient { get; }
        public OutputAction<IThreadSafeWorkspace> PickWorkspace { get; }
        public IObservable<string> Error { get; }

        public EditProjectViewModel(
            ITogglDataSource dataSource,
            IRxActionFactory rxActionFactory,
            IInteractorFactory interactorFactory,
            ISchedulerProvider schedulerProvider,
            INavigationService navigationService)
            : base(navigationService)
        {
            Ensure.Argument.IsNotNull(dataSource, nameof(dataSource));
            Ensure.Argument.IsNotNull(rxActionFactory, nameof(rxActionFactory));
            Ensure.Argument.IsNotNull(interactorFactory, nameof(interactorFactory));
            Ensure.Argument.IsNotNull(schedulerProvider, nameof(schedulerProvider));

            this.dataSource = dataSource;
            this.interactorFactory = interactorFactory;

            Name = new BehaviorRelay<string>("");
            IsPrivate = new BehaviorRelay<bool>(true);

            PickColor = rxActionFactory.FromObservable<Color>(pickColor);
            PickClient = rxActionFactory.FromObservable<IThreadSafeClient>(pickClient);
            PickWorkspace = rxActionFactory.FromObservable<IThreadSafeWorkspace>(pickWorkspace);

            var initialWorkspaceObservable = interactorFactory
                .GetDefaultWorkspace()
                .TrackException<InvalidOperationException, IThreadSafeWorkspace>("EditProjectViewModel.Initialize")
                .Execute()
                .SelectMany(defaultWorkspaceOrWorkspaceEligibleForProjectCreation)
                .Do(initialWorkspace => initialWorkspaceId = initialWorkspace.Id);

            currentWorkspace = initialWorkspaceObservable
                .Merge(PickWorkspace.Elements)
                .ShareReplay(1);

            currentClient = currentWorkspace
                .SelectValue((IThreadSafeClient)null)
                .Merge(PickClient.Elements)
                .ShareReplay(1);

            WorkspaceName = currentWorkspace
                .Select(w => w.Name)
                .DistinctUntilChanged()
                .AsDriver(schedulerProvider);

            CanCreatePublicProjects = currentWorkspace
                .Select(w => w.Admin)
                .DoIf(isAdmin => !isAdmin, _ => IsPrivate.Accept(true))
                .DistinctUntilChanged()
                .AsDriver(schedulerProvider);

            var clientName = currentClient
                .Select(client => client?.Name ?? "")
                .DistinctUntilChanged();

            ClientName = clientName
                .AsDriver(schedulerProvider);

            Color = PickColor.Elements
                .StartWith(getRandomColor())
                .Merge(currentWorkspace
                    .SelectMany(customColorIsEnabled)
                    .SelectMany(customColorsAreAvailable => customColorsAreAvailable
                        ? Observable.Empty<Color>()
                        : Color.FirstAsync().Select(randomColorIfNotDefault)))
                .DistinctUntilChanged()
                .AsDriver(schedulerProvider);

            var saveEnabledObservable = Name.Select(checkNameValidity);

            var projectOrClientNameChanged = Observable
                .Merge(clientName.SelectUnit(), Name.SelectUnit());

            Save = rxActionFactory.FromObservable(done, saveEnabledObservable);
            ToggleIsPrivate = rxActionFactory.FromAction(toggleIsPrivate);

            Error = Save.Errors
                .Select(e => e.Message)
                .Merge(projectOrClientNameChanged.SelectValue(string.Empty))
                .AsDriver(schedulerProvider);

            IObservable<IThreadSafeWorkspace> defaultWorkspaceOrWorkspaceEligibleForProjectCreation(IThreadSafeWorkspace defaultWorkspace)
                => defaultWorkspace.IsEligibleForProjectCreation()
                    ? Observable.Return(defaultWorkspace)
                    : interactorFactory.GetAllWorkspaces().Execute()
                        .Select(allWorkspaces => allWorkspaces.First(ws => ws.IsEligibleForProjectCreation()));

            IObservable<bool> customColorIsEnabled(IThreadSafeWorkspace workspace)
                => interactorFactory
                    .AreCustomColorsEnabledForWorkspace(workspace.Id)
                    .Execute();

            Color getRandomColor()
            {
                var randomColorIndex = random.Next(0, Helper.Colors.DefaultProjectColors.Length);
                return Helper.Colors.DefaultProjectColors[randomColorIndex];
            }

            Color randomColorIfNotDefault(Color lastColor)
            {
                var hex = lastColor.ToHexString();
                if (DefaultProjectColors.Any(defaultColor => defaultColor == hex))
                    return lastColor;

                return getRandomColor();
            }

            bool checkNameValidity(string name)
                => !string.IsNullOrWhiteSpace(name)
                    && name.LengthInBytes() <= Constants.MaxProjectNameLengthInBytes;
        }

        public override Task Initialize(string name)
        {
            Name.Accept(name);

            return base.Initialize(name);
        }

        private void toggleIsPrivate()
        {
            IsPrivate.Accept(!IsPrivate.Value);
        }

        private IObservable<IThreadSafeWorkspace> pickWorkspace()
        {
            return currentWorkspace.FirstAsync().SelectMany(workspaceFromViewModel);

            IObservable<IThreadSafeWorkspace> workspaceFromViewModel(IThreadSafeWorkspace currentWorkspace)
                => interactorFactory.GetAllWorkspaces().Execute()
                .SelectMany(allWorkspaces =>
                {
                    var eligibleWorkspaces = allWorkspaces.Where(ws => ws.IsEligibleForProjectCreation()).ToList();
                    var selectWorkspaces = eligibleWorkspaces.Select(ws => new SelectOption<IThreadSafeWorkspace>(ws, ws.Name));
                    var selectedWorkspaceIndex = eligibleWorkspaces.IndexOf(ws => ws.Id == currentWorkspace.Id);

                    return View.Select(Resources.Workspace, selectWorkspaces, selectedWorkspaceIndex);
                })
                .SelectMany(selectedWorkspace => workspaceFromId(selectedWorkspace.Id, currentWorkspace));

            IObservable<IThreadSafeWorkspace> workspaceFromId(long selectedWorkspaceId, IThreadSafeWorkspace currentWorkspace)
                => selectedWorkspaceId == currentWorkspace.Id
                    ? Observable.Empty<IThreadSafeWorkspace>()
                    : interactorFactory
                        .GetWorkspaceById(selectedWorkspaceId)
                        .Execute();
        }

        private IObservable<IThreadSafeClient> pickClient()
        {
            return currentWorkspace.FirstAsync()
                .SelectMany(currentWorkspace => currentClient.FirstAsync()
                    .SelectMany(currentClient => clientFromViewModel(currentClient, currentWorkspace)));

            IObservable<IThreadSafeClient> clientFromViewModel(IThreadSafeClient currentClient, IThreadSafeWorkspace currentWorkspace)
                => Navigate<SelectClientViewModel, SelectClientParameters, long?>(
                        SelectClientParameters.WithIds(currentWorkspace.Id, currentClient?.Id))
                    .ToObservable()
                    .SelectMany(clientFromId);

            IObservable<IThreadSafeClient> clientFromId(long? selectedClientId)
            {
                if (selectedClientId == null)
                    return Observable.Empty<IThreadSafeClient>();

                if (selectedClientId.Value == 0)
                    return Observable.Return<IThreadSafeClient>(null);

                return interactorFactory.GetClientById(selectedClientId.Value).Execute();
            }
        }

        private IObservable<Color> pickColor()
        {
            return currentWorkspace.FirstAsync()
                .SelectMany(currentWorkspace => interactorFactory
                    .AreCustomColorsEnabledForWorkspace(currentWorkspace.Id).Execute()
                    .SelectMany(areCustomColorsEnabled => Color.FirstAsync()
                        .SelectMany(currentColor =>
                            colorFromViewmodel(currentColor, areCustomColorsEnabled))));

            IObservable<Color> colorFromViewmodel(Color currentColor, bool areCustomColorsEnabled)
                => Navigate<SelectColorViewModel, ColorParameters, Color>(
                        ColorParameters.Create(currentColor, areCustomColorsEnabled))
                    .ToObservable();
        }

        private IObservable<Unit> done()
        {
            var nameIsAlreadyTaken = currentWorkspace
                .SelectMany(workspace => dataSource.Projects.GetAll(project => project.WorkspaceId == workspace.Id))
                .Select(existingProjectsDictionary)
                .CombineLatest(currentClient, Name, checkNameIsTaken);

            var projectCreation = currentWorkspace.FirstAsync()
                .SelectMany(workspace => checkIfCanContinue(workspace)
                    .SelectMany(shouldContinue => !shouldContinue
                        ? Observable.Empty<Unit>()
                        : getDto(workspace)
                            .SelectMany(dto => interactorFactory.CreateProject(dto).Execute())
                            .Do(createdProject => Close(createdProject.Id))
                            .SelectUnit()
                    )
                );

            return nameIsAlreadyTaken.SelectMany(taken =>
            {
                if (taken)
                {
                    throw new Exception(Resources.ProjectNameTakenError);
                }

                return projectCreation;
            });

            IObservable<bool> checkIfCanContinue(IThreadSafeWorkspace workspace)
            {
                if (initialWorkspaceId == workspace.Id)
                    return Observable.Return(true);

                return View.Confirm(
                    Resources.WorkspaceChangedAlertTitle,
                    Resources.WorkspaceChangedAlertMessage,
                    Resources.Ok,
                    Resources.Cancel
                );
            }

            IObservable<CreateProjectDTO> getDto(IThreadSafeWorkspace workspace)
                => Observable.CombineLatest(
                    Name.FirstAsync(),
                    Color.FirstAsync(),
                    currentClient.FirstAsync(),
                    interactorFactory.AreProjectsBillableByDefault(workspace.Id).Execute(),
                    IsPrivate.FirstAsync(),
                    (name, color, client, billable, isPrivate) => new CreateProjectDTO
                    {
                        Name = name.Trim(),
                        Color = color.ToHexString(),
                        IsPrivate = isPrivate,
                        ClientId = client?.Id,
                        Billable = billable,
                        WorkspaceId = workspace.Id
                    }
                );

            Dictionary<long, HashSet<string>> existingProjectsDictionary(IEnumerable<IThreadSafeProject> projectsInWorkspace)
                => projectsInWorkspace.Aggregate(new Dictionary<long, HashSet<string>>(), (dict, project) =>
                {
                    var key = project.ClientId ?? noClientId;
                    if (dict.ContainsKey(key))
                    {
                        dict[key].Add(project.Name);
                        return dict;
                    }

                    dict[key] = new HashSet<string> { project.Name };
                    return dict;
                });

            bool checkNameIsTaken(Dictionary<long, HashSet<string>> projectNameDictionary, IThreadSafeClient client, string name)
            {
                var key = client?.Id ?? noClientId;
                if (projectNameDictionary.TryGetValue(key, out var projectNames))
                    return projectNames.Contains(name.Trim());

                return false;
            }
        }
    }
}
