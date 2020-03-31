using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using Toggl.Core.Analytics;
using Toggl.Core.DataSources;
using Toggl.Core.Interactors;
using Toggl.Core.Models;
using Toggl.Core.Models.Interfaces;
using Toggl.Core.Reports;
using Toggl.Core.Services;
using Toggl.Core.UI.Extensions;
using Toggl.Core.UI.Helper;
using Toggl.Core.UI.Navigation;
using Toggl.Core.UI.Parameters;
using Toggl.Core.UI.Views;
using Toggl.Networking.Exceptions;
using Toggl.Shared;
using Toggl.Shared.Extensions;
using Toggl.Shared.Models.Reports;
using Colors = Toggl.Core.UI.Helper.Colors;

namespace Toggl.Core.UI.ViewModels.Reports
{
    [Preserve(AllMembers = true)]
    public sealed class ReportsViewModel : ViewModel
    {
        private const float minimumSegmentPercentageToBeOnItsOwn = 5f;
        private const float maximumSegmentPercentageToEndUpInOther = 1f;
        private const float minimumOtherSegmentDisplayPercentage = 1f;
        private const float maximumOtherProjectPercentageWithSegmentsBetweenOneAndFivePercent = 5f;

        private readonly CompositeDisposable disposeBag = new CompositeDisposable();

        private readonly ITimeService timeService;
        private readonly ITogglDataSource dataSource;
        private readonly IInteractorFactory interactorFactory;
        private readonly IAnalyticsService analyticsService;

        private readonly Subject<Unit> reportSubject = new Subject<Unit>();
        private readonly BehaviorSubject<bool> isLoading = new BehaviorSubject<bool>(true);
        private readonly BehaviorSubject<IThreadSafeWorkspace> workspaceSubject = new BehaviorSubject<IThreadSafeWorkspace>(null);
        private readonly BehaviorSubject<string> currentDateRangeStringSubject = new BehaviorSubject<string>(string.Empty);
        private readonly Subject<DateTimeOffset> startDateSubject = new Subject<DateTimeOffset>();
        private readonly Subject<DateTimeOffset> endDateSubject = new Subject<DateTimeOffset>();
        private readonly ISubject<TimeSpan> totalTimeSubject = new BehaviorSubject<TimeSpan>(TimeSpan.Zero);
        private readonly ISubject<float?> billablePercentageSubject = new Subject<float?>();
        private readonly ISubject<IReadOnlyList<ChartSegment>> segmentsSubject = new Subject<IReadOnlyList<ChartSegment>>();
        private readonly TimeSpan reloadInterval = TimeSpan.FromSeconds(5);

        private DateTimeOffset startDate;
        private DateTimeOffset endDate;
        private int totalDays => (endDate - startDate).Days + 1;
        private ReportsSource source;

        [Obsolete("This should be removed, replaced by something that is actually used or turned into a constant.")]
        private int projectsNotSyncedCount = 0;

        private DateTime reportSubjectStartTime;
        private long workspaceId;
        private IThreadSafeWorkspace workspace;
        private long userId;
        private DateFormat dateFormat;
        private ReportParameter parameter;
        private BeginningOfWeek beginningOfWeek;
        private DateTimeOffset viewDisappearedAtTime;

        public IObservable<bool> IsLoadingObservable { get; }

        public IObservable<TimeSpan> TotalTimeObservable { get; }

        public IObservable<bool> TotalTimeIsZeroObservable
            => TotalTimeObservable.Select(time => time.Ticks == 0);

        public IObservable<DurationFormat> DurationFormatObservable { get; }

        public IObservable<float?> BillablePercentageObservable { get; }

        public ReportsBarChartViewModel BarChartViewModel { get; }

        public ReportsCalendarViewModel CalendarViewModel { get; }

        public IObservable<IImmutableList<ChartSegment>> SegmentsObservable { get; }

        public IObservable<IImmutableList<ChartSegment>> GroupedSegmentsObservable { get; }

        public IObservable<bool> ShowEmptyStateObservable { get; }

        public IObservable<string> CurrentDateRange { get; }

        public IObservable<long> WorkspaceId { get; }
        public IObservable<string> WorkspaceNameObservable { get; }
        public ICollection<SelectOption<IThreadSafeWorkspace>> Workspaces { get; private set; }
        public IObservable<ICollection<SelectOption<IThreadSafeWorkspace>>> WorkspacesObservable { get; }
        public IObservable<DateTimeOffset> StartDate { get; }
        public IObservable<DateTimeOffset> EndDate { get; }
        public IObservable<bool> WorkspaceHasBillableFeatureEnabled { get; }

        public ViewAction SelectWorkspace { get; }

        public ReportsViewModel(ITogglDataSource dataSource,
            ITimeService timeService,
            INavigationService navigationService,
            IInteractorFactory interactorFactory,
            IAnalyticsService analyticsService,
            ISchedulerProvider schedulerProvider,
            IRxActionFactory rxActionFactory)
            : base(navigationService)
        {
            Ensure.Argument.IsNotNull(dataSource, nameof(dataSource));
            Ensure.Argument.IsNotNull(timeService, nameof(timeService));
            Ensure.Argument.IsNotNull(rxActionFactory, nameof(rxActionFactory));
            Ensure.Argument.IsNotNull(analyticsService, nameof(analyticsService));
            Ensure.Argument.IsNotNull(interactorFactory, nameof(interactorFactory));
            Ensure.Argument.IsNotNull(schedulerProvider, nameof(schedulerProvider));
            Ensure.Argument.IsNotNull(navigationService, nameof(navigationService));

            this.dataSource = dataSource;
            this.timeService = timeService;
            this.analyticsService = analyticsService;
            this.interactorFactory = interactorFactory;

            CalendarViewModel = new ReportsCalendarViewModel(timeService, dataSource, rxActionFactory, navigationService, schedulerProvider);

            var totalsObservable = reportSubject
                .SelectMany(_ => interactorFactory.GetReportsTotals(userId, workspaceId, startDate, endDate).Execute())
                .Catch<ITimeEntriesTotals, OfflineException>(_ => Observable.Return<ITimeEntriesTotals>(null))
                .Where(report => report != null);
            BarChartViewModel = new ReportsBarChartViewModel(schedulerProvider, dataSource.Preferences, totalsObservable, navigationService);

            IsLoadingObservable = isLoading.AsObservable().AsDriver(schedulerProvider);
            StartDate = startDateSubject.AsObservable().AsDriver(schedulerProvider);
            EndDate = endDateSubject.AsObservable().AsDriver(schedulerProvider);
            TotalTimeObservable = totalTimeSubject.AsObservable().AsDriver(schedulerProvider);
            BillablePercentageObservable = billablePercentageSubject.AsObservable().AsDriver(schedulerProvider);

            SelectWorkspace = rxActionFactory.FromAsync(selectWorkspace);

            WorkspaceId = workspaceSubject
                .Select(workspace => workspace.Id)
                .DistinctUntilChanged()
                .AsDriver(schedulerProvider);

            WorkspaceNameObservable = workspaceSubject
                .Select(workspace => workspace?.Name ?? string.Empty)
                .DistinctUntilChanged()
                .AsDriver(schedulerProvider);

            WorkspaceHasBillableFeatureEnabled = workspaceSubject
                .Where(workspace => workspace != null)
                .SelectMany(workspace => interactorFactory.GetWorkspaceFeaturesById(workspace.Id).Execute())
                .Select(workspaceFeatures => workspaceFeatures.IsEnabled(WorkspaceFeatureId.Pro))
                .StartWith(false)
                .DistinctUntilChanged()
                .AsDriver(schedulerProvider);

            CurrentDateRange = currentDateRangeStringSubject
                .StartWith(Resources.ThisWeek)
                .Where(text => !string.IsNullOrEmpty(text))
                .Select(text => $"{text} ▾")
                .DistinctUntilChanged()
                .AsDriver(schedulerProvider);

            WorkspacesObservable = interactorFactory.ObserveAllWorkspaces().Execute()
                .Select(list => list.Where(w => !w.IsInaccessible))
                .Select(readOnlyWorkspaceSelectOptions)
                .AsDriver(schedulerProvider);

            DurationFormatObservable = dataSource.Preferences.Current
                .Select(prefs => prefs.DurationFormat)
                .AsDriver(schedulerProvider);

            var segmentsObservable = segmentsSubject.CombineLatest(DurationFormatObservable, applyDurationFormat);

            SegmentsObservable = segmentsObservable.AsDriver(schedulerProvider);

            GroupedSegmentsObservable = segmentsObservable
                .CombineLatest(DurationFormatObservable, groupSegments)
                .AsDriver(schedulerProvider);

            ShowEmptyStateObservable = segmentsObservable
                .CombineLatest(IsLoadingObservable, shouldShowEmptyState)
                .AsDriver(schedulerProvider);
        }

        public override async Task Initialize()
        {
            await base.Initialize();

            await CalendarViewModel.Initialize();

            WorkspacesObservable
                .Subscribe(data => Workspaces = data)
                .DisposedBy(disposeBag);

            var user = await dataSource.User.Get();
            userId = user.Id;

            IInteractor<IObservable<IThreadSafeWorkspace>> workspaceInteractor;

            workspaceInteractor = interactorFactory.GetDefaultWorkspace();

            var workspace = await workspaceInteractor
                .TrackException<InvalidOperationException, IThreadSafeWorkspace>("ReportsViewModel.Initialize")
                .Execute();

            workspaceId = workspace.Id;
            workspaceSubject.OnNext(workspace);

            CalendarViewModel.SelectedDateRangeObservable
                .Subscribe(changeDateRange)
                .DisposedBy(disposeBag);

            reportSubject
                .AsObservable()
                .Do(setLoadingState)
                .SelectMany(_ =>
                   startDate == default(DateTimeOffset) || endDate == default(DateTimeOffset)
                       ? Observable.Empty<ProjectSummaryReport>()
                       : interactorFactory.GetProjectSummary(workspaceId, startDate, endDate).Execute())
                .Catch(Observable.Return<ProjectSummaryReport>(null))
                .Subscribe(onReport)
                .DisposedBy(disposeBag);

            dataSource.Preferences.Current
                .Subscribe(onPreferencesChanged)
                .DisposedBy(disposeBag);

            dataSource.User.Current
                .Select(currentUser => currentUser.BeginningOfWeek)
                .Subscribe(onBeginningOfWeekChanged)
                .DisposedBy(disposeBag);

            interactorFactory.ObserveDefaultWorkspaceId()
                .Execute()
                .Where(newWorkspaceId => newWorkspaceId != workspaceId)
                .SelectMany(id => interactorFactory.GetWorkspaceById(id).Execute())
                .Where(ws => !ws.IsInaccessible)
                .Subscribe(updateWorkspace)
                .DisposedBy(disposeBag);
        }

        private void updateWorkspace(IThreadSafeWorkspace newWorkspace)
        {
            if (viewAppearedForTheFirstTime()) return;

            loadReport(newWorkspace, startDate, endDate, source);
        }

        public override void ViewAppeared()
        {
            base.ViewAppeared();

            if (viewAppearedForTheFirstTime())
                CalendarViewModel.ViewAppeared();
            else if (timeService.CurrentDateTime - viewDisappearedAtTime >= reloadInterval)
                reportSubject.OnNext(Unit.Default);
        }

        public override void ViewDisappeared()
        {
            base.ViewDisappeared();
            viewDisappearedAtTime = timeService.CurrentDateTime;
        }

        private bool viewAppearedForTheFirstTime()
            => startDate == default(DateTimeOffset);

        private bool isCurrentWeek()
        {
            var firstDayOfCurrentWeek = timeService.CurrentDateTime.BeginningOfWeek(beginningOfWeek);
            var lastDayOfCurrentWeek = firstDayOfCurrentWeek.AddDays(6);

            return startDate.Date == firstDayOfCurrentWeek
                   && endDate.Date == lastDayOfCurrentWeek;
        }

        private static ReadOnlyCollection<SelectOption<IThreadSafeWorkspace>> readOnlyWorkspaceSelectOptions(IEnumerable<IThreadSafeWorkspace> workspaces)
            => workspaces
                .Select(ws => new SelectOption<IThreadSafeWorkspace>(ws, ws.Name))
                .ToList()
                .AsReadOnly();

        private void setLoadingState(Unit obj)
        {
            reportSubjectStartTime = timeService.CurrentDateTime.UtcDateTime;
            isLoading.OnNext(true);
            segmentsSubject.OnNext(new ChartSegment[0]);
        }

        private void onReport(ProjectSummaryReport report)
        {
            if (report == null)
            {
                isLoading.OnNext(false);
                trackReportsEvent(false);
                return;
            }

            totalTimeSubject.OnNext(TimeSpan.FromSeconds(report.TotalSeconds));
            billablePercentageSubject.OnNext(report.TotalSeconds is 0 ? null : (float?)report.BillablePercentage);
            segmentsSubject.OnNext(report.Segments);
            isLoading.OnNext(false);

            trackReportsEvent(true);
        }

        private void trackReportsEvent(bool success)
        {
            var loadingTime = timeService.CurrentDateTime.UtcDateTime - reportSubjectStartTime;

            if (success)
            {
                analyticsService.ReportsSuccess.Track(source, totalDays, projectsNotSyncedCount, loadingTime.TotalMilliseconds);
            }
            else
            {
                analyticsService.ReportsFailure.Track(source, totalDays, loadingTime.TotalMilliseconds);
            }
        }

        private void changeDateRange(ReportsDateRangeParameter dateRange)
        {
            LoadReport(workspaceId, dateRange.StartDate, dateRange.EndDate, source);
        }

        private void updateCurrentDateRangeString()
        {
            if (startDate == default(DateTimeOffset) || endDate == default(DateTimeOffset))
                return;

            var currentTime = timeService.CurrentDateTime;

            if (startDate == endDate && startDate == currentTime.RoundDownToLocalDate())
            {
                currentDateRangeStringSubject.OnNext(Resources.Today);
            }
            else if (startDate == endDate && startDate == currentTime.RoundDownToLocalDate().AddDays(-1))
            {
                currentDateRangeStringSubject.OnNext(Resources.Yesterday);
                return;
            }
            else if ((startDate, endDate).IsCurrentWeek(currentTime, beginningOfWeek))
            {
                currentDateRangeStringSubject.OnNext(Resources.ThisWeek);
            }
            else if ((startDate, endDate).IsLastWeek(currentTime, beginningOfWeek))
            {
                currentDateRangeStringSubject.OnNext(Resources.LastWeek);
            }
            else if ((startDate, endDate).IsCurrentMonth(currentTime))
            {
                currentDateRangeStringSubject.OnNext(Resources.ThisMonth);
            }
            else if ((startDate, endDate).IsLastMonth(currentTime))
            {
                currentDateRangeStringSubject.OnNext(Resources.LastMonth);
            }
            else if ((startDate, endDate).IsCurrentYear(currentTime))
            {
                currentDateRangeStringSubject.OnNext(Resources.ThisYear);
            }
            else if ((startDate, endDate).IsLastYear(currentTime))
            {
                currentDateRangeStringSubject.OnNext(Resources.LastYear);
            }
            else
            {
                var startDateText = startDate.ToString(dateFormat.Short, DateFormatCultureInfo.CurrentCulture);
                var endDateText = endDate.ToString(dateFormat.Short, DateFormatCultureInfo.CurrentCulture);
                var dateRangeText = $"{startDateText} - {endDateText}";
                currentDateRangeStringSubject.OnNext(dateRangeText);
            }
        }

        private void onPreferencesChanged(IThreadSafePreferences preferences)
        {
            dateFormat = preferences.DateFormat;
            updateCurrentDateRangeString();
        }

        private void onBeginningOfWeekChanged(BeginningOfWeek beginningOfWeek)
        {
            this.beginningOfWeek = beginningOfWeek;
            updateCurrentDateRangeString();
        }

        private IImmutableList<ChartSegment> applyDurationFormat(IReadOnlyList<ChartSegment> chartSegments, DurationFormat durationFormat)
        {
            return chartSegments.Select(segment => segment.WithDurationFormat(durationFormat))
                .ToImmutableList();
        }

        private bool shouldShowEmptyState(IReadOnlyList<ChartSegment> chartSegments, bool isLoading)
            => chartSegments.None() && !isLoading;

        private IImmutableList<ChartSegment> groupSegments(IReadOnlyList<ChartSegment> segments, DurationFormat durationFormat)
        {
            var groupedData = segments.GroupBy(segment => segment.Percentage >= minimumSegmentPercentageToBeOnItsOwn).ToList();

            var aboveStandAloneThresholdSegments = groupedData
                .Where(group => group.Key)
                .Flatten()
                .ToList();

            var otherProjectsCandidates = groupedData
                .Where(group => !group.Key)
                .Flatten()
                .ToList();

            var finalOtherProjects = otherProjectsCandidates
                .Where(segment => segment.Percentage < maximumSegmentPercentageToEndUpInOther)
                .ToList();

            var remainingOtherProjectCandidates = otherProjectsCandidates
                .Except(finalOtherProjects)
                .OrderBy(segment => segment.Percentage)
                .ToList();

            foreach (var segment in remainingOtherProjectCandidates)
            {
                finalOtherProjects.Add(segment);

                if (percentageOf(finalOtherProjects) + segment.Percentage > maximumOtherProjectPercentageWithSegmentsBetweenOneAndFivePercent)
                {
                    break;
                }
            }

            if (!finalOtherProjects.Any())
                return segments.ToImmutableList();

            var leftOutOfOther = remainingOtherProjectCandidates.Except(finalOtherProjects).ToList();
            aboveStandAloneThresholdSegments.AddRange(leftOutOfOther);
            var onTheirOwnSegments = aboveStandAloneThresholdSegments.OrderBy(segment => segment.Percentage).ToList();

            ChartSegment lastSegment;

            if (finalOtherProjects.Count == 1)
            {
                var singleSmallSegment = finalOtherProjects.First();
                lastSegment = new ChartSegment(
                    singleSmallSegment.ProjectName,
                    string.Empty,
                    singleSmallSegment.Percentage >= minimumOtherSegmentDisplayPercentage ? singleSmallSegment.Percentage : minimumOtherSegmentDisplayPercentage,
                    finalOtherProjects.Sum(segment => (float)segment.TrackedTime.TotalSeconds),
                    finalOtherProjects.Sum(segment => segment.BillableSeconds),
                    singleSmallSegment.Color,
                    durationFormat);
            }
            else
            {
                var otherPercentage = percentageOf(finalOtherProjects);
                lastSegment = new ChartSegment(
                    Resources.Other,
                    string.Empty,
                    otherPercentage >= minimumOtherSegmentDisplayPercentage ? otherPercentage : minimumOtherSegmentDisplayPercentage,
                    finalOtherProjects.Sum(segment => (float)segment.TrackedTime.TotalSeconds),
                    finalOtherProjects.Sum(segment => segment.BillableSeconds),
                    Colors.Reports.OtherProjectsSegmentBackground.ToHexString(),
                    durationFormat);
            }

            return onTheirOwnSegments
                .Append(lastSegment)
                .ToImmutableList();
        }

        private async Task selectWorkspace()
        {
            var currentWorkspaceIndex = Workspaces.IndexOf(w => w.Item.Id == workspaceId);

            var workspace = await View.Select(Resources.SelectWorkspace, Workspaces, currentWorkspaceIndex);

            if (workspace == null || workspace.Id == workspaceId) return;

            loadReport(workspace, startDate, endDate, source);
        }

        private float percentageOf(List<ChartSegment> list)
            => list.Sum(segment => segment.Percentage);

        private void loadReport(IThreadSafeWorkspace workspace, DateTimeOffset startDate, DateTimeOffset endDate, ReportsSource source)
        {
            if (this.startDate == startDate && this.endDate == endDate && workspaceId == workspace.Id)
                return;

            workspaceId = workspace.Id;
            this.workspace = workspace;
            this.startDate = startDate;
            this.endDate = endDate;
            this.source = source;

            workspaceSubject.OnNext(workspace);
            startDateSubject.OnNext(startDate);
            endDateSubject.OnNext(endDate);

            updateCurrentDateRangeString();

            reportSubject.OnNext(Unit.Default);
        }

        public async Task LoadReport(long? workspaceId, DateTimeOffset startDate, DateTimeOffset endDate, ReportsSource source)
        {
            var getWorkspaceInteractor = workspaceId.HasValue
                ? interactorFactory.GetWorkspaceById(this.workspaceId)
                : interactorFactory.GetDefaultWorkspace();

            var workspace = await getWorkspaceInteractor.Execute();

            loadReport(workspace, startDate, endDate, source);
        }

        public async Task LoadReport(long? workspaceId, ReportPeriod period)
        {
            var getWorkspaceInteractor = workspaceId.HasValue
                ? interactorFactory.GetWorkspaceById(this.workspaceId)
                : interactorFactory.GetDefaultWorkspace();

            workspace = await getWorkspaceInteractor.Execute();
            workspaceId = workspace.Id;

            CalendarViewModel.SelectPeriod(period);
        }
    }
}
