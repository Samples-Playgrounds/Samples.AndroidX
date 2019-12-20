using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using FsCheck;
using NSubstitute;
using Toggl.Core.DataSources;
using Toggl.Core.Models.Interfaces;
using Toggl.Core.Suggestions;
using Toggl.Core.Tests.Generators;
using Toggl.Core.Tests.Mocks;
using Xunit;

namespace Toggl.Core.Tests.Suggestions
{
    public sealed class RandomForestSuggestionProviderTests
    {
        public abstract class RandomForestSuggestionProviderTest
        {
            protected RandomForestSuggestionProvider Provider { get; }
            protected ITimeService TimeService { get; } = Substitute.For<ITimeService>();
            protected ITogglDataSource DataSource { get; } = Substitute.For<ITogglDataSource>();

            protected DateTimeOffset Now { get; } = new DateTimeOffset(2017, 03, 24, 12, 34, 56, TimeSpan.Zero);

            protected RandomForestSuggestionProviderTest()
            {
                Provider = new RandomForestSuggestionProvider(DataSource, TimeService);

                TimeService.CurrentDateTime.Returns(_ => Now);
            }
        }

        public sealed class TheConstructor : RandomForestSuggestionProviderTest
        {
            [Theory, LogIfTooSlow]
            [ConstructorData]
            public void ThrowsIfAnyOfTheArgumentsIsNull(bool useDataSource, bool useTimeService)
            {
                var dataSource = useDataSource ? DataSource : null;
                var timeService = useTimeService ? TimeService : null;

                Action tryingToConstructWithEmptyParameters =
                    () => new RandomForestSuggestionProvider(dataSource, timeService);

                tryingToConstructWithEmptyParameters
                    .Should().Throw<ArgumentNullException>();
            }
        }

        public sealed class TheGetSuggestionsMethod : RandomForestSuggestionProviderTest
        {
            private IEnumerable<IThreadSafeTimeEntry> getTimeEntries(
                int numberOfTimeEntries,
                bool withProject,
                int initalId = 0,
                bool activeProject = true,
                long projectId = 4)
            {
                if (numberOfTimeEntries == 0)
                {
                    return new List<IThreadSafeTimeEntry>();
                }

                var workspace = new MockWorkspace { Id = 12 };
                var project = new MockProject { Id = projectId, Name = $"{projectId}", Active = activeProject };

                return Enumerable.Range(initalId, numberOfTimeEntries)
                    .Select(index =>
                    {
                        var entry = new MockTimeEntry
                        {
                            Id = index,
                            UserId = 10,
                            WorkspaceId = workspace.Id,
                            Workspace = workspace,
                            At = Now,
                            Start = Now.AddHours(index % 23),
                            Description = $"te{index}"
                        };

                        if (withProject)
                        {
                            entry.ProjectId = project.Id;
                            entry.Project = project;
                        }

                        return entry;
                    });
            }

            [Fact, LogIfTooSlow]
            public async Task ReturnsEmptyObservableIfThereAreNoTimeEntries()
            {
                DataSource.TimeEntries
                        .GetAll()
                        .Returns(Observable.Empty<IEnumerable<IThreadSafeTimeEntry>>());

                var suggestions = await Provider.GetSuggestions().ToList();

                suggestions.Should().HaveCount(0);
            }

            [Theory]
            [InlineData(50, 120, 1, false)]
            [InlineData(120, 40, 1, true)]
            [InlineData(50, 20, 0, false)]
            [InlineData(120, 0, 1, true)]
            [InlineData(0, 120, 1, false)]
            public async Task ReturnsTheCorrectAmountOfSuggestions(
                int numberOfTimeEntrieWithProject,
                int numberOfTimeEntrieWithoutProject,
                int numberOfExpectedResults,
                bool expectedToHaveProject)
            {
                var provider = new RandomForestSuggestionProvider(DataSource, TimeService);

                var timeEntries = getTimeEntries(numberOfTimeEntrieWithProject, true)
                    .Concat(getTimeEntries(numberOfTimeEntrieWithoutProject, false, numberOfTimeEntrieWithProject + 1));

                DataSource.TimeEntries
                        .GetAll()
                        .Returns(Observable.Return(timeEntries));

                var suggestions = await provider.GetSuggestions().ToList();

                suggestions.Should().HaveCount(numberOfExpectedResults);

                if (numberOfExpectedResults == 0)
                    return;

                if (expectedToHaveProject)
                {
                    suggestions.First().ProjectId.Should().NotBe(null);
                    suggestions.First().ProjectName.Should().NotBe("");
                }
                else if (numberOfTimeEntrieWithoutProject == 0)
                {
                    suggestions.First().ProjectId.Should().Be(null);
                    suggestions.First().ProjectName.Should().Be("");
                }
            }

            [Theory]
            [InlineData(120, 120, 1)]
            [InlineData(0, 120, 0)]
            public async Task DoesNotReturnSuggestionsWithArchivedProjects(
                int numberOfTimeEntrieWithActiveProject,
                int numberOfTimeEntrieWithoutActiveProject,
                int numberOfExpectedResults)
            {
                var provider = new RandomForestSuggestionProvider(DataSource, TimeService);

                var timeEntries = getTimeEntries(numberOfTimeEntrieWithActiveProject, true, projectId: 2)
                    .Concat(getTimeEntries(numberOfTimeEntrieWithoutActiveProject, true, numberOfTimeEntrieWithActiveProject + 1, false, projectId: 5));

                DataSource.TimeEntries
                    .GetAll()
                    .Returns(Observable.Return(timeEntries));

                var suggestions = await provider.GetSuggestions().ToList();

                suggestions.Should().HaveCount(numberOfExpectedResults);

                if (numberOfExpectedResults == 0)
                    return;

                suggestions.First().ProjectId.Should().Be(2);
                suggestions.First().ProjectName.Should().Be("2");
            }

            [Fact]
            public void NeverThrows()
            {
                var exception = new Exception();
                DataSource.TimeEntries.GetAll().Returns(Observable.Throw<IEnumerable<IThreadSafeTimeEntry>>(exception));
                var provider = new RandomForestSuggestionProvider(DataSource, TimeService);

                Action getSuggestions = () => provider.GetSuggestions().Subscribe();
                getSuggestions.Should().NotThrow();
            }

            [Fact]
            public void ReturnsNoSuggestionsInCaseOfError()
            {
                var exception = new Exception();
                DataSource.TimeEntries.GetAll().Returns(Observable.Throw<IEnumerable<IThreadSafeTimeEntry>>(exception));
                var provider = new RandomForestSuggestionProvider(DataSource, TimeService);

                provider.GetSuggestions().Count().Wait().Should().Be(0);
            }
        }
    }
}
