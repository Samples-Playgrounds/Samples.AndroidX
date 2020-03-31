using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using Toggl.Core.DataSources;
using Toggl.Core.Extensions;
using Toggl.Core.Interactors;
using Toggl.Core.Models.Interfaces;
using Toggl.Core.Services;
using Toggl.Core.UI.Navigation;
using Toggl.Core.UI.Parameters;
using Toggl.Core.UI.ViewModels.Settings.Siri;
using Toggl.Shared;
using Toggl.Shared.Extensions;
using Toggl.Shared.Extensions.Reactive;
using Toggl.Storage.Settings;

namespace Toggl.Core.UI.ViewModels.Settings
{
    public class SiriShortcutsCustomTimeEntryViewModel : ViewModel
    {
        private readonly INavigationService navigationService;
        private readonly IInteractorFactory interactorFactory;
        private readonly IOnboardingStorage onboardingStorage;
        private CompositeDisposable disposeBag = new CompositeDisposable();

        private BehaviorSubject<EditTimeEntryViewModel.ProjectClientTaskInfo> projectClientTaskInfo =
            new BehaviorSubject<EditTimeEntryViewModel.ProjectClientTaskInfo>(EditTimeEntryViewModel
                .ProjectClientTaskInfo.Empty);

        public BehaviorRelay<bool> IsBillable { get; } = new BehaviorRelay<bool>(false);
        public BehaviorRelay<string> Description { get; } = new BehaviorRelay<string>(string.Empty);
        public BehaviorRelay<IThreadSafeWorkspace> Workspace { get; } = new BehaviorRelay<IThreadSafeWorkspace>(null);
        public BehaviorRelay<IThreadSafeProject> Project { get; } = new BehaviorRelay<IThreadSafeProject>(null);
        public BehaviorRelay<long?> TaskId { get; } = new BehaviorRelay<long?>(null);
        public BehaviorRelay<IEnumerable<IThreadSafeTag>> Tags { get; } =
            new BehaviorRelay<IEnumerable<IThreadSafeTag>>(Enumerable.Empty<IThreadSafeTag>());
        public BehaviorRelay<bool> PasteFromClipboard { get; } = new BehaviorRelay<bool>(false);
        public IObservable<bool> IsBillableAvailable { get; }
        public IObservable<IEnumerable<string>> TagNames { get; }
        public IObservable<bool> HasTags { get; }
        public IObservable<EditTimeEntryViewModel.ProjectClientTaskInfo> ProjectClientTask { get; }

        public ViewAction SelectTags { get; }
        public ViewAction SelectProject { get; }
        public ViewAction SelectClipboard { get; }

        public SiriShortcutsCustomTimeEntryViewModel(
            ITogglDataSource dataSource,
            IInteractorFactory interactorFactory,
            IRxActionFactory rxActionFactory,
            IOnboardingStorage onboardingStorage,
            ISchedulerProvider schedulerProvider,
            INavigationService navigationService) : base(navigationService)
        {
            Ensure.Argument.IsNotNull(dataSource, nameof(dataSource));
            Ensure.Argument.IsNotNull(interactorFactory, nameof(interactorFactory));
            Ensure.Argument.IsNotNull(navigationService, nameof(navigationService));
            Ensure.Argument.IsNotNull(rxActionFactory, nameof(rxActionFactory));
            Ensure.Argument.IsNotNull(onboardingStorage, nameof(onboardingStorage));
            Ensure.Argument.IsNotNull(schedulerProvider, nameof(schedulerProvider));

            this.navigationService = navigationService;
            this.interactorFactory = interactorFactory;
            this.onboardingStorage = onboardingStorage;

            SelectTags = rxActionFactory.FromAsync(selectTags);
            SelectProject = rxActionFactory.FromAsync(selectProject);
            SelectClipboard = rxActionFactory.FromAsync(selectClipboard);

            IsBillableAvailable = Workspace
                .Where(ws => ws?.Id != null)
                .SelectMany(ws => interactorFactory.IsBillableAvailableForWorkspace(ws.Id).Execute())
                .DistinctUntilChanged()
                .AsDriver(false, schedulerProvider);

            HasTags = Tags.Select(tags => tags.Any()).AsDriver(false, schedulerProvider);
            TagNames = Tags
                .Select(tags => tags.Select(t => t.Name))
                .AsDriver(Enumerable.Empty<string>(), schedulerProvider);
            ProjectClientTask =
                projectClientTaskInfo.AsDriver(EditTimeEntryViewModel.ProjectClientTaskInfo.Empty, schedulerProvider);
        }

        public override async Task Initialize()
        {
            await base.Initialize();
            var defaultWorkspace = await interactorFactory.GetDefaultWorkspace().Execute();
            Workspace.Accept(defaultWorkspace);
        }

        public override void ViewDestroyed()
        {
            base.ViewDestroyed();
            disposeBag?.Dispose();
        }

        private async Task selectTags()
        {
            if (!(Workspace.Value?.Id is long workspaceId))
            {
                return;
            }

            var currentTags = Tags.Value.Select(t => t.Id).OrderBy(CommonFunctions.Identity).ToArray();

            var chosenTags = await Navigate<SelectTagsViewModel, SelectTagsParameter, long[]>(
                    new SelectTagsParameter(currentTags, workspaceId, false));

            if (chosenTags.OrderBy(CommonFunctions.Identity).SequenceEqual(currentTags))
            {
                return;
            }

            var selectedTags = await interactorFactory.GetMultipleTagsById(chosenTags).Execute();

            Tags.Accept(selectedTags);
        }

        private async Task selectProject()
        {
            if (!(Workspace.Value?.Id is long workspaceId))
            {
                return;
            }

            var chosenProjectParams = await Navigate<SelectProjectViewModel, SelectProjectParameter, SelectProjectParameter>(
                    new SelectProjectParameter(Project.Value?.Id, TaskId.Value, workspaceId, false));

            if (chosenProjectParams.WorkspaceId == workspaceId
                && chosenProjectParams.ProjectId == Project.Value?.Id
                && chosenProjectParams.TaskId == TaskId.Value)
            {
                return;
            }

            TaskId.Accept(chosenProjectParams.TaskId);

            var chosenWorkspace = await interactorFactory.GetWorkspaceById(chosenProjectParams.WorkspaceId).Execute();

            if (!(chosenProjectParams.ProjectId is long chosenProjectProjectId))
            {
                projectClientTaskInfo.OnNext(EditTimeEntryViewModel.ProjectClientTaskInfo.Empty);
                clearTagsIfNeeded(workspaceId, chosenProjectParams.WorkspaceId);
                Workspace.Accept(chosenWorkspace);
                Project.Accept(null);
                return;
            }

            var project = await interactorFactory.GetProjectById(chosenProjectProjectId).Execute();
            clearTagsIfNeeded(workspaceId, project.WorkspaceId);

            var task = chosenProjectParams.TaskId.HasValue
                ? await interactorFactory.GetTaskById(chosenProjectParams.TaskId.Value).Execute()
                : null;

            var taskName = task?.Name ?? string.Empty;

            projectClientTaskInfo.OnNext(new EditTimeEntryViewModel.ProjectClientTaskInfo(
                project.DisplayName(),
                project.DisplayColor(),
                project.Client?.Name,
                taskName,
                project.IsPlaceholder(),
                task?.IsPlaceholder() ?? false));

            Workspace.Accept(chosenWorkspace);
            Project.Accept(project);
        }

        private async Task selectClipboard()
        {
            if (!onboardingStorage.DidShowSiriClipboardInstruction())
            {
                await Navigate<PasteFromClipboardViewModel>();
            }

            PasteFromClipboard.Accept(!PasteFromClipboard.Value);
        }

        private void clearTagsIfNeeded(long currentWorkspaceId, long newWorkspaceId)
        {
            if (currentWorkspaceId == newWorkspaceId)
                return;

            Tags.Accept(Enumerable.Empty<IThreadSafeTag>());
        }
    }
}
