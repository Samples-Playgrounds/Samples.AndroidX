using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Reactive.Threading.Tasks;
using System.Threading.Tasks;
using Toggl.Core.Analytics;
using Toggl.Core.Autocomplete;
using Toggl.Core.Autocomplete.Span;
using Toggl.Core.Autocomplete.Suggestions;
using Toggl.Core.DataSources;
using Toggl.Core.Extensions;
using Toggl.Core.Interactors;
using Toggl.Core.Models.Interfaces;
using Toggl.Core.Search;
using Toggl.Core.Services;
using Toggl.Core.UI.Collections;
using Toggl.Core.UI.Extensions;
using Toggl.Core.UI.Navigation;
using Toggl.Core.UI.Parameters;
using Toggl.Core.UI.Views;
using Toggl.Shared;
using Toggl.Shared.Extensions;
using Toggl.Shared.Extensions.Reactive;
using Toggl.Storage.Settings;
using static Toggl.Core.Helper.Constants;
using static Toggl.Shared.Extensions.CommonFunctions;

namespace Toggl.Core.UI.ViewModels
{
    [Preserve(AllMembers = true)]
    public sealed class StartTimeEntryViewModel : ViewModelWithInput<StartTimeEntryParameters>
    {
        private readonly ITimeService timeService;
        private readonly IUserPreferences userPreferences;
        private readonly IInteractorFactory interactorFactory;
        private readonly IAnalyticsService analyticsService;
        private readonly ISchedulerProvider schedulerProvider;

        private readonly CompositeDisposable disposeBag = new CompositeDisposable();
        private readonly BehaviorRelay<TextFieldInfo> textFieldInfo = new BehaviorRelay<TextFieldInfo>(Autocomplete.TextFieldInfo.Empty(0));
        private readonly ISubject<AutocompleteSuggestionType> queryByTypeSubject = new Subject<AutocompleteSuggestionType>();
        private readonly BehaviorRelay<TimeSpan> displayedTime = new BehaviorRelay<TimeSpan>(TimeSpan.Zero);
        private readonly BehaviorRelay<bool> isBillable = new BehaviorRelay<bool>(false);
        private readonly BehaviorRelay<bool> isSuggestingTags = new BehaviorRelay<bool>(false);
        private readonly BehaviorRelay<bool> isSuggestingProjects = new BehaviorRelay<bool>(false);
        private readonly BehaviorRelay<bool> isBillableAvailable = new BehaviorRelay<bool>(false);

        private bool isDirty => !string.IsNullOrEmpty(textFieldInfo.Value.Description)
                                || textFieldInfo.Value.Spans.Any(s => s is ProjectSpan || s is TagSpan)
                                || isBillable.Value
                                || startTime != parameter?.StartTime
                                || duration != parameter?.Duration;

        private bool hasAnyTags;
        private bool hasAnyProjects;
        private bool canCreateProjectsInWorkspace;
        private IThreadSafeWorkspace defaultWorkspace;
        private StartTimeEntryParameters parameter;
        private StartTimeEntryParameters initialParameters;

        private DateTimeOffset startTime;
        private TimeSpan? duration;

        private bool isRunning => !duration.HasValue;

        private string currentQuery;

        private BehaviorSubject<HashSet<long>> expandedProjects =
            new BehaviorSubject<HashSet<long>>(new HashSet<long>());

        public IObservable<TextFieldInfo> TextFieldInfo { get; }
        public IObservable<bool> IsBillable { get; }
        public IObservable<bool> IsSuggestingTags { get; }
        public IObservable<bool> IsSuggestingProjects { get; }
        public IObservable<bool> IsBillableAvailable { get; }

        public string PlaceholderText { get; private set; }

        public ITogglDataSource DataSource { get; }

        public IOnboardingStorage OnboardingStorage { get; }

        public OutputAction<IThreadSafeTimeEntry> Done { get; }
        public ViewAction DurationTapped { get; }
        public ViewAction ToggleBillable { get; }
        public ViewAction SetStartDate { get; }
        public ViewAction ChangeTime { get; }
        public ViewAction ToggleTagSuggestions { get; }
        public ViewAction ToggleProjectSuggestions { get; }
        public InputAction<AutocompleteSuggestion> SelectSuggestion { get; }
        public InputAction<TimeSpan> SetRunningTime { get; }
        public InputAction<ProjectSuggestion> ToggleTasks { get; }

        public IObservable<IImmutableList<SectionModel<string, AutocompleteSuggestion>>> Suggestions { get; }
        public IObservable<string> DisplayedTime { get; }

        private DefaultTimeEntrySearchEngine searchEngine = new DefaultTimeEntrySearchEngine();

        public StartTimeEntryViewModel(
            ITimeService timeService,
            ITogglDataSource dataSource,
            IUserPreferences userPreferences,
            IOnboardingStorage onboardingStorage,
            IInteractorFactory interactorFactory,
            INavigationService navigationService,
            IAnalyticsService analyticsService,
            ISchedulerProvider schedulerProvider,
            IRxActionFactory rxActionFactory)
            : base(navigationService)
        {
            Ensure.Argument.IsNotNull(dataSource, nameof(dataSource));
            Ensure.Argument.IsNotNull(timeService, nameof(timeService));
            Ensure.Argument.IsNotNull(userPreferences, nameof(userPreferences));
            Ensure.Argument.IsNotNull(interactorFactory, nameof(interactorFactory));
            Ensure.Argument.IsNotNull(onboardingStorage, nameof(onboardingStorage));
            Ensure.Argument.IsNotNull(analyticsService, nameof(analyticsService));
            Ensure.Argument.IsNotNull(schedulerProvider, nameof(schedulerProvider));
            Ensure.Argument.IsNotNull(rxActionFactory, nameof(rxActionFactory));

            this.timeService = timeService;
            this.userPreferences = userPreferences;
            this.interactorFactory = interactorFactory;
            this.analyticsService = analyticsService;
            this.schedulerProvider = schedulerProvider;

            DataSource = dataSource;
            OnboardingStorage = onboardingStorage;

            TextFieldInfo = textFieldInfo.AsDriver(schedulerProvider);
            IsBillable = isBillable.AsDriver(schedulerProvider);
            IsSuggestingTags = isSuggestingTags.AsDriver(schedulerProvider);
            IsSuggestingProjects = isSuggestingProjects.AsDriver(schedulerProvider);
            IsBillableAvailable = isBillableAvailable.AsDriver(schedulerProvider);
            DisplayedTime = displayedTime
                .Select(time => time.ToFormattedString(DurationFormat.Improved))
                .AsDriver(schedulerProvider);

            Done = rxActionFactory.FromObservable<IThreadSafeTimeEntry>(done);
            DurationTapped = rxActionFactory.FromAction(durationTapped);
            ToggleBillable = rxActionFactory.FromAction(toggleBillable);
            SetStartDate = rxActionFactory.FromAsync(setStartDate);
            ChangeTime = rxActionFactory.FromAsync(changeTime);
            ToggleTagSuggestions = rxActionFactory.FromAction(toggleTagSuggestions);
            ToggleProjectSuggestions = rxActionFactory.FromAction(toggleProjectSuggestions);
            SelectSuggestion = rxActionFactory.FromAsync<AutocompleteSuggestion>(selectSuggestion);
            SetRunningTime = rxActionFactory.FromAction<TimeSpan>(setRunningTime);
            ToggleTasks = rxActionFactory.FromAction<ProjectSuggestion>(toggleTasks);

            var queryByType = queryByTypeSubject
                .AsObservable()
                .Select(type => new QueryInfo("", type));

            var queryByText = textFieldInfo
                .SelectMany(setBillableValues)
                .Select(QueryInfo.ParseFieldInfo)
                .Do(onParsedQuery)
                .ObserveOn(schedulerProvider.BackgroundScheduler);

            Suggestions = Observable.Merge(queryByText, queryByType)
                .SelectMany(query => interactorFactory.GetAutocompleteSuggestions(query, searchEngine).Execute())
                .Select(items => items.ToList()) // This is line is needed for now to read objects from realm .ObserveOn(schedulerProvider.BackgroundScheduler)
                .Select(filter)
                .Select(group)
                .CombineLatest(expandedProjects, (groups, _) => groups)
                .Select(toCollections)
                .Select(addStaticElements)
                .AsDriver(schedulerProvider);
        }

        public override async Task Initialize(StartTimeEntryParameters parameter)
        {
            await base.Initialize(parameter);

            interactorFactory.GetAllTimeEntriesVisibleToTheUser()
                .Execute()
                .SubscribeOn(schedulerProvider.BackgroundScheduler)
                .Subscribe(entries => searchEngine.SetInitialData(entries.ToImmutableList()))
                .DisposedBy(disposeBag);

            this.parameter = parameter;
            startTime = parameter.StartTime;
            duration = parameter.Duration;

            PlaceholderText = parameter.PlaceholderText;
            if (!string.IsNullOrEmpty(parameter.EntryDescription))
            {
                initialParameters = parameter;
            }

            displayedTime.Accept(duration ?? TimeSpan.Zero);

            timeService.CurrentDateTimeObservable
                .Where(_ => isRunning)
                .Subscribe(currentTime => displayedTime.Accept(currentTime - startTime))
                .DisposedBy(disposeBag);

            defaultWorkspace = await interactorFactory.GetDefaultWorkspace()
                .TrackException<InvalidOperationException, IThreadSafeWorkspace>("StartTimeEntryViewModel.Initialize")
                .Execute();

            canCreateProjectsInWorkspace =
                await interactorFactory.GetAllWorkspaces().Execute().Select(allWorkspaces =>
                    allWorkspaces.Any(ws => ws.IsEligibleForProjectCreation()));

            if (initialParameters != null)
            {
                var spans = new List<ISpan>();
                spans.Add(new TextSpan(initialParameters.EntryDescription));
                if (initialParameters.ProjectId != null)
                {
                    try
                    {
                        var project = await interactorFactory.GetProjectById((long)initialParameters.ProjectId).Execute();
                        spans.Add(new ProjectSpan(project));
                    }
                    catch
                    {
                        // Intentionally left blank
                    }
                }
                if (initialParameters.TagIds != null)
                {
                    try
                    {
                        var tags = initialParameters.TagIds.ToObservable()
                            .SelectMany<long, IThreadSafeTag>(tagId => interactorFactory.GetTagById(tagId).Execute())
                            .ToEnumerable();
                        spans.AddRange(tags.Select(tag => new TagSpan(tag)));
                    }
                    catch
                    {
                        // Intentionally left blank
                    }
                }

                textFieldInfo.Accept(textFieldInfo.Value.ReplaceSpans(spans.ToImmutableList()));
            }
            else
            {
                textFieldInfo.Accept(Autocomplete.TextFieldInfo.Empty(parameter?.WorkspaceId ?? defaultWorkspace.Id));
            }

            hasAnyTags = (await DataSource.Tags.GetAll()).Any();
            hasAnyProjects = (await DataSource.Projects.GetAll()).Any();
        }

        public override void ViewDestroyed()
        {
            base.ViewDestroyed();
            disposeBag?.Dispose();
        }

        public override async Task<bool> ConfirmCloseRequest()
        {
            if (isDirty)
            {
                var view = View;
                if (view == null)
                    return true;

                return await view
                    .ConfirmDestructiveAction(ActionType.DiscardNewTimeEntry);
            }

            return true;
        }

        public void SetTextSpans(IImmutableList<ISpan> spans)
        {
            textFieldInfo.Accept(textFieldInfo.Value.ReplaceSpans(spans));
        }

        private void setRunningTime(TimeSpan runningTime)
        {
            if (isRunning)
            {
                startTime = timeService.CurrentDateTime - runningTime;
            }
            else
            {
                duration = runningTime;
            }

            displayedTime.Accept(runningTime);
        }

        private async Task selectSuggestion(AutocompleteSuggestion suggestion)
        {
            switch (suggestion)
            {
                case QuerySymbolSuggestion querySymbolSuggestion:

                    if (querySymbolSuggestion.Symbol == QuerySymbols.ProjectsString)
                    {
                        analyticsService.StartViewTapped.Track(StartViewTapSource.PickEmptyStateProjectSuggestion);
                        analyticsService.StartEntrySelectProject.Track(ProjectTagSuggestionSource.TableCellButton);
                    }
                    else if (querySymbolSuggestion.Symbol == QuerySymbols.TagsString)
                    {
                        analyticsService.StartViewTapped.Track(StartViewTapSource.PickEmptyStateTagSuggestion);
                        analyticsService.StartEntrySelectTag.Track(ProjectTagSuggestionSource.TableCellButton);
                    }

                    textFieldInfo.Accept(textFieldInfo.Value.FromQuerySymbolSuggestion(querySymbolSuggestion));
                    break;

                case TimeEntrySuggestion timeEntrySuggestion:
                    analyticsService.StartViewTapped.Track(StartViewTapSource.PickTimeEntrySuggestion);
                    textFieldInfo.Accept(textFieldInfo.Value.FromTimeEntrySuggestion(timeEntrySuggestion));
                    break;

                case ProjectSuggestion projectSuggestion:
                    analyticsService.StartViewTapped.Track(StartViewTapSource.PickProjectSuggestion);

                    if (textFieldInfo.Value.WorkspaceId != projectSuggestion.WorkspaceId
                        && await workspaceChangeDenied())
                        return;

                    isSuggestingProjects.Accept(false);
                    textFieldInfo.Accept(textFieldInfo.Value.FromProjectSuggestion(projectSuggestion));
                    queryByTypeSubject.OnNext(AutocompleteSuggestionType.None);

                    break;

                case TaskSuggestion taskSuggestion:
                    analyticsService.StartViewTapped.Track(StartViewTapSource.PickTaskSuggestion);

                    if (textFieldInfo.Value.WorkspaceId != taskSuggestion.WorkspaceId
                        && await workspaceChangeDenied())
                        return;

                    isSuggestingProjects.Accept(false);
                    textFieldInfo.Accept(textFieldInfo.Value.FromTaskSuggestion(taskSuggestion));
                    queryByTypeSubject.OnNext(AutocompleteSuggestionType.None);
                    break;

                case TagSuggestion tagSuggestion:
                    analyticsService.StartViewTapped.Track(StartViewTapSource.PickTagSuggestion);
                    textFieldInfo.Accept(textFieldInfo.Value.FromTagSuggestion(tagSuggestion));
                    break;

                case CreateEntitySuggestion createEntitySuggestion:
                    if (isSuggestingProjects.Value)
                    {
                        await createProject();
                    }
                    else
                    {
                        await createTag();
                    }
                    break;

                default:
                    return;
            }

            IObservable<bool> workspaceChangeDenied()
                => View.Confirm(
                    Resources.DifferentWorkspaceAlertTitle,
                    Resources.DifferentWorkspaceAlertMessage,
                    Resources.Ok,
                    Resources.Cancel
                ).Select(Invert);
        }

        private async Task createProject()
        {
            var projectId = await Navigate<EditProjectViewModel, string, long?>(currentQuery);
            if (projectId == null)
                return;

            var project = await interactorFactory.GetProjectById(projectId.Value).Execute();
            var projectSuggestion = new ProjectSuggestion(project);

            textFieldInfo.Accept(textFieldInfo.Value.FromProjectSuggestion(projectSuggestion));
            isSuggestingProjects.Accept(false);
            queryByTypeSubject.OnNext(AutocompleteSuggestionType.None);
            hasAnyProjects = true;
        }

        private async Task createTag()
        {
            var createdTag = await interactorFactory.CreateTag(currentQuery, textFieldInfo.Value.WorkspaceId).Execute();
            var tagSuggestion = new TagSuggestion(createdTag);
            await selectSuggestion(tagSuggestion);
            hasAnyTags = true;
        }

        private void durationTapped()
        {
            analyticsService.StartViewTapped.Track(StartViewTapSource.Duration);
        }

        private void toggleTagSuggestions()
        {
            if (isSuggestingTags.Value)
            {
                isSuggestingTags.Accept(false);
                textFieldInfo.Accept(textFieldInfo.Value.RemoveTagQueryIfNeeded());
                queryByTypeSubject.OnNext(AutocompleteSuggestionType.None);
                return;
            }

            analyticsService.StartViewTapped.Track(StartViewTapSource.Tags);
            analyticsService.StartEntrySelectTag.Track(ProjectTagSuggestionSource.ButtonOverKeyboard);
            OnboardingStorage.ProjectOrTagWasAdded();

            textFieldInfo.Accept(textFieldInfo.Value.AddQuerySymbol(QuerySymbols.TagsString));
        }

        private void toggleProjectSuggestions()
        {
            if (isSuggestingProjects.Value)
            {
                isSuggestingProjects.Accept(false);
                textFieldInfo.Accept(textFieldInfo.Value.RemoveProjectQueryIfNeeded());
                queryByTypeSubject.OnNext(AutocompleteSuggestionType.None);
                return;
            }

            analyticsService.StartViewTapped.Track(StartViewTapSource.Project);
            analyticsService.StartEntrySelectProject.Track(ProjectTagSuggestionSource.ButtonOverKeyboard);
            OnboardingStorage.ProjectOrTagWasAdded();

            if (textFieldInfo.Value.HasProject)
            {
                isSuggestingProjects.Accept(true);
                queryByTypeSubject.OnNext(AutocompleteSuggestionType.Projects);
                return;
            }

            textFieldInfo.Accept(textFieldInfo.Value.AddQuerySymbol(QuerySymbols.ProjectsString));
        }

        private void toggleTasks(ProjectSuggestion projectSuggestion)
        {
            var currentExpandedProjects = expandedProjects.Value;
            if (currentExpandedProjects.Contains(projectSuggestion.ProjectId))
            {
                currentExpandedProjects.Remove(projectSuggestion.ProjectId);
            }
            else
            {
                currentExpandedProjects.Add(projectSuggestion.ProjectId);
            }
            expandedProjects.OnNext(currentExpandedProjects);
        }

        private void toggleBillable()
        {
            analyticsService.StartViewTapped.Track(StartViewTapSource.Billable);
            isBillable.Accept(!isBillable.Value);
        }

        private async Task changeTime()
        {
            analyticsService.StartViewTapped.Track(StartViewTapSource.StartTime);

            var currentDuration = DurationParameter.WithStartAndDuration(startTime, duration);

            var selectedDuration = await Navigate<EditDurationViewModel, EditDurationParameters, DurationParameter>(new EditDurationParameters(currentDuration, isStartingNewEntry: true))
                .ConfigureAwait(false);

            startTime = selectedDuration.Start;
            duration = selectedDuration.Duration ?? duration;
            displayedTime.Accept(duration ?? TimeSpan.Zero);
        }

        private async Task setStartDate()
        {
            analyticsService.StartViewTapped.Track(StartViewTapSource.StartDate);

            var parameters = isRunning
                ? DateTimePickerParameters.ForStartDateOfRunningTimeEntry(startTime, timeService.CurrentDateTime)
                : DateTimePickerParameters.ForStartDateOfStoppedTimeEntry(startTime);

            var duration = this.duration;

            startTime = await Navigate<SelectDateTimeViewModel, DateTimePickerParameters, DateTimeOffset>(parameters)
                .ConfigureAwait(false);

            if (isRunning == false)
            {
                this.duration = duration;
            }
        }

        private IObservable<IThreadSafeTimeEntry> done()
        {
            var timeEntry = textFieldInfo.Value.AsTimeEntryPrototype(startTime, duration, isBillable.Value);
            var origin = duration.HasValue ? TimeEntryStartOrigin.Manual : TimeEntryStartOrigin.Timer;
            if (parameter.Origin is TimeEntryStartOrigin paramOrigin)
            {
                origin = paramOrigin;
            }

            Close();

            return interactorFactory.CreateTimeEntry(timeEntry, origin)
                .Execute()
                .ToObservable();
        }

        private void onParsedQuery(QueryInfo parsedQuery)
        {
            var newQuery = parsedQuery.Text?.Trim() ?? "";
            if (currentQuery != newQuery)
            {
                currentQuery = newQuery;
            }

            bool suggestsTags = parsedQuery.SuggestionType == AutocompleteSuggestionType.Tags;
            bool suggestsProjects = parsedQuery.SuggestionType == AutocompleteSuggestionType.Projects;

            if (!isSuggestingTags.Value && suggestsTags)
            {
                analyticsService.StartEntrySelectTag.Track(ProjectTagSuggestionSource.TextField);
            }

            if (!isSuggestingProjects.Value && suggestsProjects)
            {
                analyticsService.StartEntrySelectProject.Track(ProjectTagSuggestionSource.TextField);
            }

            isSuggestingTags.Accept(suggestsTags);
            isSuggestingProjects.Accept(suggestsProjects);
        }

        private IEnumerable<AutocompleteSuggestion> filter(IEnumerable<AutocompleteSuggestion> suggestions)
        {
            if (textFieldInfo.Value.HasProject && !isSuggestingProjects.Value && !isSuggestingTags.Value)
            {
                var projectId = textFieldInfo.Value.Spans.OfType<ProjectSpan>().Single().ProjectId;

                return suggestions.OfType<TimeEntrySuggestion>()
                    .Where(suggestion => suggestion.ProjectId == projectId);
            }

            return suggestions;
        }

        private IEnumerable<IGrouping<long, AutocompleteSuggestion>> group(IEnumerable<AutocompleteSuggestion> suggestions)
        {
            var firstSuggestion = suggestions.FirstOrDefault();
            if (firstSuggestion is ProjectSuggestion)
            {
                return suggestions
                    .Cast<ProjectSuggestion>()
                    .OrderBy(ps => ps.ProjectName)
                    .Where(suggestion => !string.IsNullOrEmpty(suggestion.WorkspaceName))
                    .GroupBy(suggestion => suggestion.WorkspaceId)
                    .OrderByDescending(group => group.First().WorkspaceId == (defaultWorkspace?.Id ?? 0))
                    .ThenBy(group => group.First().WorkspaceName);
            }

            if (isSuggestingTags.Value)
                suggestions = suggestions.Where(suggestion => suggestion.WorkspaceId == textFieldInfo.Value.WorkspaceId);

            return suggestions
                .GroupBy(suggestion => suggestion.WorkspaceId);
        }

        private IEnumerable<SectionModel<string, AutocompleteSuggestion>> toCollections(IEnumerable<IGrouping<long, AutocompleteSuggestion>> suggestions)
        {
            var sections = suggestions.Select(group =>
                {
                    var header = "";
                    var items = group
                        .Distinct(AutocompleteSuggestionComparer.Instance)
                        .SelectMany(includeTasksIfExpanded);

                    if (group.First() is ProjectSuggestion projectSuggestion)
                    {
                        header = projectSuggestion.WorkspaceName;
                        items = items.Prepend(ProjectSuggestion.NoProject(projectSuggestion.WorkspaceId,
                            projectSuggestion.WorkspaceName));
                    }
                    else if (group.First() is TimeEntrySuggestion timeEntrySuggestion)
                    {
                        header = timeEntrySuggestion.WorkspaceName;
                    }

                    return new SectionModel<string, AutocompleteSuggestion>(header, items);
                }
            );

            return sections;
        }

        private IEnumerable<AutocompleteSuggestion> includeTasksIfExpanded(AutocompleteSuggestion suggestion)
        {
            yield return suggestion;

            if (suggestion is ProjectSuggestion projectSuggestion && expandedProjects.Value.Contains(projectSuggestion.ProjectId))
            {
                var orderedTasks = projectSuggestion.Tasks
                    .OrderBy(t => t.Name);

                foreach (var taskSuggestion in orderedTasks)
                    yield return taskSuggestion;
            }
        }

        private IImmutableList<SectionModel<string, AutocompleteSuggestion>> addStaticElements(IEnumerable<SectionModel<string, AutocompleteSuggestion>> sections)
        {
            var suggestions = sections.SelectMany(section => section.Items);

            if (isSuggestingProjects.Value)
            {
                if (shouldAddProjectCreationSuggestion())
                {
                    sections = sections
                        .Prepend(
                            SectionModel<string, AutocompleteSuggestion>.SingleElement(
                                new CreateEntitySuggestion(Resources.CreateProject, currentQuery)
                            )
                        );
                }

                if (!hasAnyProjects)
                {
                    sections = sections.Append(SectionModel<string, AutocompleteSuggestion>.SingleElement(
                        NoEntityInfoMessage.CreateProject())
                    );
                }
            }

            if (isSuggestingTags.Value)
            {
                if (shouldAddTagCreationSuggestion())
                {
                    sections = sections
                        .Prepend(
                            SectionModel<string, AutocompleteSuggestion>.SingleElement(
                                new CreateEntitySuggestion(Resources.CreateTag, currentQuery)
                            )
                        );
                }

                if (!hasAnyTags)
                {
                    sections = sections.Append(SectionModel<string, AutocompleteSuggestion>.SingleElement(
                        NoEntityInfoMessage.CreateTag())
                    );
                }
            }

            return sections.ToImmutableList();

            bool shouldAddProjectCreationSuggestion()
                => canCreateProjectsInWorkspace && !textFieldInfo.Value.HasProject &&
                   currentQuery.LengthInBytes() <= MaxProjectNameLengthInBytes &&
                   !string.IsNullOrEmpty(currentQuery);

            bool shouldAddTagCreationSuggestion()
                => !string.IsNullOrEmpty(currentQuery) && currentQuery.IsAllowedTagByteSize() &&
                   suggestions.None(item =>
                       item is TagSuggestion tagSuggestion &&
                       tagSuggestion.Name.IsSameCaseInsensitiveTrimedTextAs(currentQuery));
        }

        private IObservable<TextFieldInfo> setBillableValues(TextFieldInfo textFieldInfo)
        {
            long? projectId = textFieldInfo.ProjectId;
            var hasProject = projectId.HasValue && projectId.Value != ProjectSuggestion.NoProjectId;
            if (hasProject)
            {
                var defaultsToBillable = interactorFactory.ProjectDefaultsToBillable(projectId.Value).Execute();
                var billableAvailable =
                    interactorFactory.IsBillableAvailableForProject(projectId.Value).Execute()
                        .Do(isBillableAvailable.Accept);

                return Observable.CombineLatest(billableAvailable, defaultsToBillable, And)
                    .Do(isBillable.Accept)
                    .SelectValue(textFieldInfo);
            }

            isBillable.Accept(false);
            return interactorFactory.IsBillableAvailableForWorkspace(textFieldInfo.WorkspaceId).Execute()
                .Do(isBillableAvailable.Accept)
                .SelectValue(textFieldInfo);
        }
    }
}
