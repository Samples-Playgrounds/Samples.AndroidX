using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using Toggl.Core.Analytics;
using Toggl.Core.Models;
using Toggl.Core.Reports;
using Toggl.Networking;
using Toggl.Networking.ApiClients;
using Toggl.Networking.ApiClients.Interfaces;
using Toggl.Shared;
using Toggl.Shared.Extensions;
using Toggl.Shared.Models;
using Toggl.Shared.Models.Reports;
using Toggl.Storage;
using Toggl.Storage.Models;

namespace Toggl.Core.Interactors
{
    public sealed class GetProjectSummaryInteractor
        : TrackableInteractor,
          IInteractor<IObservable<ProjectSummaryReport>>
    {
        private readonly long workspaceId;
        private readonly DateTimeOffset startDate;
        private readonly DateTimeOffset? endDate;

        private readonly IProjectsApi projectsApi;
        private readonly ReportsMemoryCache memoryCache;
        private readonly IProjectsSummaryApi projectSummaryApi;
        private readonly ITimeEntriesReportsApi timeEntriesReportsApi;
        private readonly IRepository<IDatabaseProject> projectsRepository;
        private readonly IRepository<IDatabaseClient> clientsRepository;

        public GetProjectSummaryInteractor(
            ITogglApi api,
            ITogglDatabase database,
            IAnalyticsService analyticsService,
            ReportsMemoryCache memoryCache,
            long workspaceId,
            DateTimeOffset startDate,
            DateTimeOffset? endDate)
            : base(analyticsService)
        {
            Ensure.Argument.IsNotNull(api, nameof(api));
            Ensure.Argument.IsNotNull(database, nameof(database));

            projectsApi = api.Projects;
            projectsRepository = database.Projects;
            clientsRepository = database.Clients;
            projectSummaryApi = api.ProjectsSummary;
            timeEntriesReportsApi = api.TimeEntriesReports;

            this.endDate = endDate;
            this.startDate = startDate;
            this.workspaceId = workspaceId;
            this.memoryCache = memoryCache;
        }

        public IObservable<ProjectSummaryReport> Execute()
            => projectSummaryApi
                .GetByWorkspace(workspaceId, startDate, endDate)
                .ToObservable()
                .SelectMany(response => summaryReportFromResponse(response, workspaceId));

        private IObservable<ProjectSummaryReport> summaryReportFromResponse(IProjectsSummary response, long workspaceId)
        {
            var summarySegments = getChartSegmentsForWorkspace(response, workspaceId);
            var projectIdsNotSynced = getProjectIdsNotSynced(response);

            var result = Observable.CombineLatest(
                summarySegments,
                projectIdsNotSynced,
                (segments, ids) => new ProjectSummaryReport(segments, ids.Length)
            );

            return result;
        }

        private IObservable<ChartSegment[]> getChartSegmentsForWorkspace(IProjectsSummary projectsSummary, long workspaceId)
            => projectsSummary.ProjectsSummaries
                .Select(s => s.ProjectId)
                .SelectNonNulls()
                .Apply(ids => searchProjects(workspaceId, ids.ToArray()))
                .Select(projectsInReport => getChartSegmentsForProjectsInReport(projectsSummary, projectsInReport))
                .SelectMany(segmentsObservable => segmentsObservable.Merge())
                .ToArray()
                .Select(s => s.OrderByDescending(c => c.Percentage).ToArray());

        private IEnumerable<IObservable<ChartSegment>> getChartSegmentsForProjectsInReport(IProjectsSummary projectsSummary, IList<IProject> projectsInReport)
        {
            var totalSeconds = projectsSummary.ProjectsSummaries.Select(s => s.TrackedSeconds).Sum();
            return projectsSummary.ProjectsSummaries
                .Select(summary =>
                {
                    var project = projectsInReport.FirstOrDefault(p => p.Id == summary.ProjectId);
                    return findClient(project)
                        .Select(client => chartFromSummary(summary, project, client, totalSeconds));
                });
        }

        private IObservable<long[]> getProjectIdsNotSynced(IProjectsSummary response)
        {
            var summaryProjectIds = response.ProjectsSummaries
                .Select(s => s.ProjectId)
                .SelectNonNulls()
                .ToArray();

            return projectsIdsNotInDatabase(summaryProjectIds);
        }

        private IObservable<IClient> findClient(IProject project)
            => project != null && project.ClientId.HasValue
                ? clientsRepository.GetAll()
                    .Flatten()
                    .Where(c => c.Id == project.ClientId.Value)
                    .FirstOrDefaultAsync()
                : Observable.Return<IClient>(null);

        private ChartSegment chartFromSummary(
            IProjectSummary summary,
            IProject project,
            IClient client,
            long totalSeconds)
        {
            var percentage = totalSeconds == 0 ? 0 : (summary.TrackedSeconds / (float)totalSeconds) * 100;
            var billableSeconds = summary.BillableSeconds ?? 0;

            return project == null
                ? new ChartSegment(Resources.NoProject, null, percentage, summary.TrackedSeconds, billableSeconds, Helper.Colors.NoProject)
                : new ChartSegment(project.Name, client?.Name, percentage, summary.TrackedSeconds, billableSeconds, project.Color);
        }

        private IObservable<IList<IProject>> searchProjects(long workspaceId, long[] projectIds) =>
            Observable.DeferAsync(async cancellationToken =>
            {
                if (projectIds.Length == 0)
                    return Observable.Return(new List<IProject>());

                var projectsInDatabase = await projectIds
                    .Select(tryGetProjectFromDatabase)
                    .Aggregate(Observable.Merge)
                    .ToList();

                var notInDatabaseIds = projectsInDatabase.SelectAllLeft().ToArray();
                var databaseProjectsObservable =
                    Observable.Return(projectsInDatabase.SelectAllRight().ToList());

                return notInDatabaseIds.Length == 0
                    ? databaseProjectsObservable
                    : databaseProjectsObservable
                        .Merge(searchMemoryAndApi(workspaceId, notInDatabaseIds))
                        .Flatten()
                        .ToList();
            });

        private IObservable<Either<long, IProject>> tryGetProjectFromDatabase(long id)
            => projectsRepository.GetById(id)
                .Select(Project.From)
                .Select(Either<long, IProject>.WithRight)
                .Catch(Observable.Return(Either<long, IProject>.WithLeft(id)));

        private IObservable<IList<IProject>> searchMemoryAndApi(long workspaceId, long[] projectIds) =>
            Observable.Defer(() =>
            {
                var projectsInMemory = memoryCache.TryGetProjects(projectIds);

                var notInMemoryIds = projectsInMemory.SelectAllLeft().ToArray();
                var memoryProjectsObservable =
                    Observable.Return(projectsInMemory.SelectAllRight().ToList());

                return notInMemoryIds.Length == 0
                    ? memoryProjectsObservable
                    : memoryProjectsObservable
                        .Merge(searchApi(workspaceId, notInMemoryIds))
                        .Flatten()
                        .ToList();
            });

        private IObservable<List<IProject>> searchApi(long workspaceId, long[] projectIds)
            => projectsApi.Search(workspaceId, projectIds)
                .ToObservable()
                .Do(memoryCache.PersistInCache);

        private IObservable<long[]> projectsIdsNotInDatabase(long[] projectIds)
            => projectIds
                .Select(id => tryGetProjectFromDatabase(id))
                .Merge()
                .ToList()
                .Select(ids => ids.SelectAllLeft().ToArray())
                .DefaultIfEmpty(Array.Empty<long>());
    }
}
