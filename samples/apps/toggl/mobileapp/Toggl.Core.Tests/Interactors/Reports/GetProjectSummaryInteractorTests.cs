using FluentAssertions;
using FsCheck;
using FsCheck.Xunit;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using Toggl.Core.Analytics;
using Toggl.Core.Helper;
using Toggl.Core.Interactors;
using Toggl.Core.Reports;
using Toggl.Core.Tests.Generators;
using Toggl.Core.Tests.TestExtensions;
using Toggl.Networking;
using Toggl.Networking.ApiClients;
using Toggl.Networking.Models.Reports;
using Toggl.Shared;
using Toggl.Shared.Extensions;
using Toggl.Shared.Models;
using Toggl.Shared.Models.Reports;
using Toggl.Storage;
using Toggl.Storage.Models;
using Xunit;
using Math = System.Math;

namespace Toggl.Core.Tests.Interactors
{
    public sealed class GetProjectSummaryInteractorTests
    {
        public abstract class GetProjectSummaryInteractorTest
        {
            protected ITogglApi Api { get; } = Substitute.For<ITogglApi>();
            protected ITogglDatabase Database { get; } = Substitute.For<ITogglDatabase>();
            protected ReportsMemoryCache ReportsMemoryCache { get; set; } = new ReportsMemoryCache();

            protected IProjectsApi ProjectsApi { get; } = Substitute.For<IProjectsApi>();
            protected IProjectsSummaryApi ProjectsSummaryApi { get; } = Substitute.For<IProjectsSummaryApi>();
            protected IRepository<IDatabaseProject> ProjectsRepository { get; } =
                Substitute.For<IRepository<IDatabaseProject>>();
            protected IRepository<IDatabaseClient> ClientsRepository { get; } =
                Substitute.For<IRepository<IDatabaseClient>>();
            protected IAnalyticsService AnalyticsService { get; } = Substitute.For<IAnalyticsService>();

            protected GetProjectSummaryInteractorTest()
            {
                Api.Projects.Returns(ProjectsApi);
                Api.ProjectsSummary.Returns(ProjectsSummaryApi);

                Database.Projects.Returns(ProjectsRepository);
                Database.Clients.Returns(ClientsRepository);
            }

            protected GetProjectSummaryInteractor GetInteractor(
                long workspaceId, DateTimeOffset startDate, DateTimeOffset? endDate)
                => new GetProjectSummaryInteractor(
                    Api,
                    Database,
                    AnalyticsService,
                    ReportsMemoryCache,
                    workspaceId,
                    startDate,
                    endDate
                );
        }

        public sealed class Constructor : GetProjectSummaryInteractorTest
        {
            [Theory, LogIfTooSlow]
            [ConstructorData]
            public void ThrowsIfAnyOfTheArgumentsIsNull(bool useApi, bool useDatabase, bool useAnalyticsService)
            {
                var api = useApi ? Api : null;
                var database = useDatabase ? Database : null;
                var analyticsService = useAnalyticsService ? AnalyticsService : null;

                Action tryingToConstructWithEmptyParameters =
                    () => new GetProjectSummaryInteractor(
                        api,
                        database,
                        analyticsService,
                        ReportsMemoryCache, 0, DateTimeOffset.Now, null
                    );

                tryingToConstructWithEmptyParameters
                    .Should().Throw<ArgumentNullException>();
            }
        }

        public sealed class TheGetProjectSummaryMethod : GetProjectSummaryInteractorTest
        {
            private const long workspaceId = 10;

            private readonly IProjectsSummary apiProjectsSummary = Substitute.For<IProjectsSummary>();

            public TheGetProjectSummaryMethod()
            {
                ProjectsSummaryApi
                    .GetByWorkspace(Arg.Any<long>(), Arg.Any<DateTimeOffset>(), Arg.Any<DateTimeOffset>())
                    .ReturnsTaskOf(apiProjectsSummary);
            }

            [Property]
            public void QueriesTheDatabaseForFindingProjects(NonEmptyArray<NonNegativeInt> projectIds)
            {
                var actualProjectIds = projectIds.Get.Select(i => (long)i.Get).Distinct().ToArray();
                var summaries = getSummaryList(actualProjectIds);
                apiProjectsSummary.ProjectsSummaries.Returns(summaries);
                configureRepositoryToReturn(actualProjectIds);

                GetInteractor(workspaceId, DateTimeOffset.Now.AddDays(-7), DateTimeOffset.Now)
                    .Execute()
                    .Wait();

                ProjectsRepository.Received()
                    .GetById(Arg.Is<long>(id => Array.IndexOf(actualProjectIds, id) >= 0));
            }

            [Property(MaxTest = 1)]
            public void QueriesTheApiIfAnyProjectIsNotFoundInTheDatabase(NonEmptyArray<NonNegativeInt> projectIds)
            {
                var actualProjectIds = projectIds.Get.Select(i => (long)i.Get).Distinct().ToArray();
                if (actualProjectIds.Length < 2) return;

                var projectsInDb = actualProjectIds.Where((id, i) => i % 2 == 0).ToArray();
                var projectsInApi = actualProjectIds.Where((id, i) => i % 2 != 0).ToArray();
                var summaries = getSummaryList(actualProjectIds);
                apiProjectsSummary.ProjectsSummaries.Returns(summaries);
                configureRepositoryToReturn(projectsInDb, projectsInApi);
                configureApiToReturn(projectsInApi);

                GetInteractor(workspaceId, DateTimeOffset.Now.AddDays(-7), DateTimeOffset.Now)
                    .Execute()
                    .Wait();

                ProjectsApi.Received()
                    .Search(workspaceId, Arg.Is<long[]>(
                        calledIds => ensureExpectedIdsAreReturned(calledIds, projectsInApi)));
            }

            [Property(MaxTest = 10, StartSize = 10, EndSize = 20)]
            public void ReturnsOnlyOneListIfItQueriesTheApi(NonEmptyArray<NonNegativeInt> projectIds)
            {
                var actualProjectIds = projectIds.Get.Select(i => (long)i.Get).Distinct().ToArray();
                if (actualProjectIds.Length < 2) return;

                var projectsInDb = actualProjectIds.Where((i, id) => i % 2 == 0).ToArray();
                var projectsInApi = actualProjectIds.Where((i, id) => i % 2 != 0).ToArray();
                var summaries = getSummaryList(actualProjectIds);
                apiProjectsSummary.ProjectsSummaries.Returns(summaries);
                configureRepositoryToReturn(projectsInDb, projectsInApi);
                configureApiToReturn(projectsInApi);

                var lists = GetInteractor(workspaceId, DateTimeOffset.Now.AddDays(-7), DateTimeOffset.Now)
                    .Execute()
                    .ToList()
                    .Wait();

                lists.Should().HaveCount(1);
            }

            [Property(MaxTest = 10, StartSize = 10, EndSize = 20)]
            public void ReturnsOnlyOneListIfItUsesTheMemoryCache(NonEmptyArray<NonNegativeInt> projectIds)
            {
                var actualProjectIds = projectIds.Get.Select(i => (long)i.Get).Distinct().ToArray();
                if (actualProjectIds.Length < 2) return;

                ReportsMemoryCache = new ReportsMemoryCache();

                var projectsInDb = actualProjectIds.Where((i, id) => i % 2 == 0).ToArray();
                var projectsInApi = actualProjectIds.Where((i, id) => i % 2 != 0).ToArray();
                var summaries = getSummaryList(actualProjectIds);
                apiProjectsSummary.ProjectsSummaries.Returns(summaries);
                configureRepositoryToReturn(projectsInDb, projectsInApi);
                configureApiToReturn(projectsInApi);

                GetInteractor(workspaceId, DateTimeOffset.Now.AddDays(-7), DateTimeOffset.Now).Execute().Wait();
                var lists = GetInteractor(workspaceId, DateTimeOffset.Now.AddDays(-7), DateTimeOffset.Now)
                    .Execute()
                    .ToList()
                    .Wait();

                lists.Should().HaveCount(1);
            }

            [Property(MaxTest = 1)]
            public void CachesTheApiResultsInMemorySoTheApiIsNotCalledTwiceForTheSameProjects(
                NonEmptyArray<NonNegativeInt> projectIds)
            {
                var actualProjectIds = projectIds.Get.Select(i => (long)i.Get).Distinct().ToArray();
                var idCount = actualProjectIds.Length;
                if (idCount < 2) return;
                var dbProjectCount = (int)Math.Floor((float)idCount / 2);
                var apiProjectCount = idCount - dbProjectCount;
                var projectsInDb = actualProjectIds.Take(dbProjectCount).ToArray();
                var projectsInApi = actualProjectIds.TakeLast(apiProjectCount).ToArray();
                var summaries = getSummaryList(actualProjectIds);
                apiProjectsSummary.ProjectsSummaries.Returns(summaries);
                configureRepositoryToReturn(projectsInDb, projectsInApi);
                configureApiToReturn(projectsInApi);

                GetInteractor(workspaceId, DateTimeOffset.Now.AddDays(-7), DateTimeOffset.Now).Execute().Wait();
                GetInteractor(workspaceId, DateTimeOffset.Now.AddDays(-7), DateTimeOffset.Now).Execute().Wait();

                ProjectsApi.Received(1)
                    .Search(workspaceId, Arg.Is<long[]>(
                        calledIds => ensureExpectedIdsAreReturned(calledIds, projectsInApi)));
            }

            [Property]
            public void DoesNotCallTheApiIfAllProjectsAreInTheDatabase(NonEmptyArray<NonNegativeInt> projectIds)
            {
                var actualProjectIds = projectIds.Get.Select(i => (long)i.Get).Distinct().ToArray();
                var summaries = getSummaryList(actualProjectIds);
                apiProjectsSummary.ProjectsSummaries.Returns(summaries);
                configureRepositoryToReturn(actualProjectIds);

                GetInteractor(workspaceId, DateTimeOffset.Now.AddDays(-7), DateTimeOffset.Now)
                    .Execute()
                    .Wait();

                ProjectsApi.DidNotReceive().Search(Arg.Any<long>(), Arg.Any<long[]>());
            }

            [Property]
            public void CreatesAChartSegmentWithNoProjectIfThereAreNullProjectIdsAmongTheApiResults(
                NonNegativeInt[] projectIds)
            {
                var actualProjectIds = projectIds.Select(i => (long)i.Get).Distinct().ToArray();
                var summaries = getSummaryList(actualProjectIds);
                summaries.Add(new ProjectSummary());
                apiProjectsSummary.ProjectsSummaries.Returns(summaries);
                configureRepositoryToReturn(actualProjectIds);

                var report = GetInteractor(workspaceId, DateTimeOffset.Now.AddDays(-7), DateTimeOffset.Now)
                    .Execute()
                    .Wait();

                report.Segments.Single(s => s.Color == Colors.NoProject).ProjectName.Should().Be(Resources.NoProject);
            }

            [Property]
            public void ReturnsTheProjectOrderedByTotalTimeTracked(NonNegativeInt[] projectIds)
            {
                var actualProjectIds = projectIds.Select(i => (long)i.Get).Distinct().ToArray();
                var summaries = getSummaryList(actualProjectIds);
                summaries.Add(new ProjectSummary());
                var summaryCount = summaries.Count;
                for (int i = 0; i < summaryCount; i++)
                {
                    var summary = (ProjectSummary)summaries[i];
                    summary.TrackedSeconds = i;
                }
                apiProjectsSummary.ProjectsSummaries.Returns(summaries);
                configureRepositoryToReturn(actualProjectIds);

                var report = GetInteractor(workspaceId, DateTimeOffset.Now.AddDays(-7), DateTimeOffset.Now)
                    .Execute()
                    .Wait();

                report.Segments.Should().BeInDescendingOrder(s => s.Percentage);
            }

            private IList<IProjectSummary> getSummaryList(long[] projectIds)
                => projectIds
                    .Select(id => new ProjectSummary { ProjectId = id })
                    .Cast<IProjectSummary>()
                    .ToList();

            private void configureRepositoryToReturn(long[] actualProjectIds, long[] throwIds = null)
            {
                foreach (var projectId in actualProjectIds)
                {
                    var project = Substitute.For<IDatabaseProject>();
                    project.Id.Returns(projectId);
                    project.Name.Returns(projectId.ToString());

                    ProjectsRepository
                            .GetById(Arg.Is(projectId))
                            .Returns(Observable.Return(project));
                }

                if (throwIds == null) return;

                foreach (var id in throwIds)
                {
                    ProjectsRepository
                        .GetById(id)
                        .Returns(Observable.Throw<IDatabaseProject>(new Exception()));
                }
            }

            private void configureApiToReturn(long[] projectIds)
            {
                var projects = projectIds.Select(projectId =>
                {
                    var project = Substitute.For<IProject>();
                    project.Id.Returns(projectId);
                    project.Name.Returns(projectId.ToString());
                    return project;
                }).ToList();

                ProjectsApi
                    .Search(Arg.Any<long>(), Arg.Any<long[]>())
                    .ReturnsTaskOf(projects);
            }

            private bool ensureExpectedIdsAreReturned(long[] actual, long[] expected)
            {
                if (actual.Length != expected.Length) return false;

                foreach (var actualTag in actual)
                {
                    if (!expected.Contains(actualTag))
                        return false;
                }
                return true;
            }
        }
    }
}
