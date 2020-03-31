using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using Toggl.Core.Autocomplete.Suggestions;
using Toggl.Core.DataSources;
using Toggl.Core.Extensions;
using Toggl.Core.Interactors;
using Toggl.Core.Services;
using Toggl.Core.UI.Collections;
using Toggl.Core.UI.Extensions;
using Toggl.Core.UI.Navigation;
using Toggl.Core.UI.Parameters;
using Toggl.Shared;
using Toggl.Shared.Extensions;
using static Toggl.Core.Helper.Constants;

namespace Toggl.Core.UI.ViewModels
{
    [Preserve(AllMembers = true)]
    public sealed class SelectProjectViewModel
        : ViewModel<SelectProjectParameter, SelectProjectParameter>
    {
        private readonly ITogglDataSource dataSource;
        private readonly IInteractorFactory interactorFactory;
        private readonly ISchedulerProvider schedulerProvider;

        private long? taskId;
        private long? projectId;
        private long workspaceId;

        private bool creationEnabled = true;
        private bool projectCreationSuggestionsAreEnabled;

        public bool UseGrouping { get; private set; }

        private BehaviorSubject<IImmutableList<SectionModel<string, AutocompleteSuggestion>>> suggestionsSubject
            = new BehaviorSubject<IImmutableList<SectionModel<string, AutocompleteSuggestion>>>(ImmutableList<SectionModel<string, AutocompleteSuggestion>>.Empty);
        public IObservable<IImmutableList<SectionModel<string, AutocompleteSuggestion>>> Suggestions => suggestionsSubject.AsObservable();

        public ISubject<string> FilterText { get; } = new BehaviorSubject<string>(string.Empty);

        public IObservable<bool> IsEmpty { get; }

        public IObservable<string> PlaceholderText { get; }

        public InputAction<ProjectSuggestion> ToggleTaskSuggestions { get; }

        public InputAction<AutocompleteSuggestion> SelectProject { get; }

        public SelectProjectViewModel(
            ITogglDataSource dataSource,
            IRxActionFactory rxActionFactory,
            IInteractorFactory interactorFactory,
            INavigationService navigationService,
            ISchedulerProvider schedulerProvider)
            : base(navigationService)
        {
            Ensure.Argument.IsNotNull(dataSource, nameof(dataSource));
            Ensure.Argument.IsNotNull(rxActionFactory, nameof(rxActionFactory));
            Ensure.Argument.IsNotNull(interactorFactory, nameof(interactorFactory));
            Ensure.Argument.IsNotNull(schedulerProvider, nameof(schedulerProvider));

            this.dataSource = dataSource;
            this.interactorFactory = interactorFactory;
            this.schedulerProvider = schedulerProvider;

            ToggleTaskSuggestions = rxActionFactory.FromAction<ProjectSuggestion>(toggleTaskSuggestions);
            SelectProject = rxActionFactory.FromAsync<AutocompleteSuggestion>(selectProject);

            IsEmpty = dataSource.Projects.GetAll().Select(projects => projects.None());
            PlaceholderText = IsEmpty.Select(isEmpty => isEmpty ? Resources.EnterProject : Resources.AddFilterProjects);
        }

        private bool shouldSuggestCreation(string text)
        {
            if (!projectCreationSuggestionsAreEnabled)
                return false;

            text = text.Trim();

            if (string.IsNullOrEmpty(text))
                return false;

            var isOfAllowedLength = text.LengthInBytes() <= MaxProjectNameLengthInBytes;
            if (!isOfAllowedLength)
                return false;

            return true;
        }

        public override async Task Initialize(SelectProjectParameter parameter)
        {
            await base.Initialize(parameter);
            creationEnabled = parameter.CreationEnabled;
            taskId = parameter.TaskId;
            projectId = parameter.ProjectId;
            workspaceId = parameter.WorkspaceId;

            var workspaces = await interactorFactory.GetAllWorkspaces().Execute();

            projectCreationSuggestionsAreEnabled = workspaces.Any(ws => ws.IsEligibleForProjectCreation());
            UseGrouping = workspaces.Count() > 1;

            FilterText.Subscribe(async text =>
            {
                var suggestions = interactorFactory.GetProjectsAutocompleteSuggestions(text.SplitToQueryWords()).Execute().SelectMany(x => x).ToEnumerable()
                    .Cast<ProjectSuggestion>()
                    .Select(setSelectedProject);

                var collectionSections = suggestions
                    .GroupBy(project => project.WorkspaceId)
                    .Select(grouping => grouping.OrderBy(projectSuggestion => projectSuggestion.ProjectName))
                    .OrderBy(grouping => grouping.First().WorkspaceName)
                    .Select(grouping => collectionSection(grouping, prependNoProject: string.IsNullOrEmpty(text)))
                    .ToList();

                if (creationEnabled && shouldSuggestCreation(text))
                {
                    var createEntitySuggestion = new CreateEntitySuggestion(Resources.CreateProject, text);
                    var section = new SectionModel<string, AutocompleteSuggestion>(null, new[] { createEntitySuggestion });
                    collectionSections.Insert(0, section);
                }

                if (collectionSections.None())
                {
                    var workspace = await interactorFactory.GetWorkspaceById(workspaceId).Execute();
                    var noProjectSuggestion = ProjectSuggestion.NoProject(workspace.Id, workspace.Name);
                    collectionSections.Add(
                        new SectionModel<string, AutocompleteSuggestion>(null, new[] { noProjectSuggestion })
                    );
                }

                suggestionsSubject.OnNext(collectionSections.ToImmutableList());
            });
        }

        public override Task<bool> CloseWithDefaultResult()
        {
            Close(new SelectProjectParameter(projectId, taskId, workspaceId));
            return Task.FromResult(true);
        }

        private SectionModel<string, AutocompleteSuggestion> collectionSection(IEnumerable<ProjectSuggestion> suggestions, bool prependNoProject)
        {
            var workspaceName = suggestions.First().WorkspaceName;
            var sectionItems = suggestions.ToList();

            if (prependNoProject)
            {
                var workspaceIdForNoProject = suggestions.First().WorkspaceId;
                var noProjectSuggestion = ProjectSuggestion.NoProject(workspaceIdForNoProject, workspaceName);
                sectionItems.Insert(0, noProjectSuggestion);
            }

            return new SectionModel<string, AutocompleteSuggestion>(workspaceName, sectionItems);
        }

        private ProjectSuggestion setSelectedProject(ProjectSuggestion suggestion)
        {
            suggestion.Selected = suggestion.ProjectId == projectId;
            return suggestion;
        }

        private async Task createProject(string name)
        {
            var createdProjectId = await Navigate<EditProjectViewModel, string, long?>(name);
            if (createdProjectId == null) return;

            var project = await interactorFactory.GetProjectById(createdProjectId.Value).Execute();
            var parameter = new SelectProjectParameter(project.Id, null, project.WorkspaceId);
            Close(parameter);
        }

        private async Task selectProject(AutocompleteSuggestion suggestion)
        {
            if (suggestion is CreateEntitySuggestion createEntitySuggestion)
            {
                await createProject(createEntitySuggestion.EntityName);
                return;
            }

            if (suggestion.WorkspaceId == workspaceId || suggestion.WorkspaceId == 0)
            {
                setProject(suggestion);
                return;
            }

            var shouldSetProject = await View.Confirm(
                Resources.DifferentWorkspaceAlertTitle,
                Resources.DifferentWorkspaceAlertMessage,
                Resources.Ok,
                Resources.Cancel
            );

            if (!shouldSetProject) return;

            setProject(suggestion);
        }

        private void setProject(AutocompleteSuggestion suggestion)
        {
            workspaceId = suggestion.WorkspaceId;
            switch (suggestion)
            {
                case ProjectSuggestion projectSuggestion:
                    projectId = projectSuggestion
                        .ProjectId == 0 ? null : (long?)projectSuggestion.ProjectId;
                    taskId = null;
                    break;

                case TaskSuggestion taskSuggestion:
                    projectId = taskSuggestion.ProjectId;
                    taskId = taskSuggestion.TaskId;
                    break;

                default:
                    throw new ArgumentException($"{nameof(suggestion)} must be either of type {nameof(ProjectSuggestion)} or {nameof(TaskSuggestion)}.");
            }

            Close(new SelectProjectParameter(projectId, taskId, workspaceId));
        }

        private void toggleTaskSuggestions(ProjectSuggestion projectSuggestion)
        {
            if (projectSuggestion.TasksVisible)
                removeTasksFor(projectSuggestion);
            else
                insertTasksFor(projectSuggestion);

            projectSuggestion.TasksVisible = !projectSuggestion.TasksVisible;
        }

        private void insertTasksFor(ProjectSuggestion projectSuggestion)
        {
            var indexOfTargetSection = suggestionsSubject.Value.IndexOf(section => section.Header == projectSuggestion.WorkspaceName);
            if (indexOfTargetSection < 0)
                return;

            var targetSection = suggestionsSubject.Value.ElementAt(indexOfTargetSection);
            var indexOfSuggestion = targetSection.Items.IndexOf(project => project == projectSuggestion);
            if (indexOfSuggestion < 0)
                return;

            var newItemsInSection = targetSection.Items.InsertRange(indexOfSuggestion + 1, projectSuggestion.Tasks.OrderBy(task => task.Name));

            var newSection = new SectionModel<string, AutocompleteSuggestion>(targetSection.Header, newItemsInSection);
            var newSuggestions = suggestionsSubject.Value.ToList();
            newSuggestions[indexOfTargetSection] = newSection;

            suggestionsSubject.OnNext(newSuggestions.ToImmutableList());
        }

        private void removeTasksFor(ProjectSuggestion projectSuggestion)
        {
            var indexOfTargetSection = suggestionsSubject.Value.IndexOf(section => section.Items.Contains(projectSuggestion));
            if (indexOfTargetSection < 0)
                return;

            var targetSection = suggestionsSubject.Value.ElementAt(indexOfTargetSection);
            var newItemsInSection = targetSection.Items.ToList();
            foreach (var task in projectSuggestion.Tasks)
                newItemsInSection.Remove(task);

            var newSection = new SectionModel<string, AutocompleteSuggestion>(targetSection.Header, newItemsInSection);
            var newSuggestions = suggestionsSubject.Value.ToList();
            newSuggestions[indexOfTargetSection] = newSection;

            suggestionsSubject.OnNext(newSuggestions.ToImmutableList());
        }
    }
}
