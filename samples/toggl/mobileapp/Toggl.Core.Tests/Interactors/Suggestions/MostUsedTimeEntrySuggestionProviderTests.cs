using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using FsCheck;
using FsCheck.Xunit;
using NSubstitute;
using Toggl.Core.DataSources;
using Toggl.Core.Models.Interfaces;
using Toggl.Core.Suggestions;
using Toggl.Core.Tests.Generators;
using Toggl.Core.Tests.Mocks;
using Toggl.Storage;
using Toggl.Storage.Models;
using Xunit;

namespace Toggl.Core.Tests.Suggestions
{
    public sealed class MostUsedTimeEntrySuggestionProviderTests
    {
        public abstract class MostUsedTimeEntrySuggestionProviderTest
        {
            protected const int NumberOfSuggestions = 7;

            protected MostUsedTimeEntrySuggestionProvider Provider { get; }
            protected ITimeService TimeService { get; } = Substitute.For<ITimeService>();
            protected ITogglDataSource DataSource { get; } = Substitute.For<ITogglDataSource>();

            protected DateTimeOffset Now { get; } = new DateTimeOffset(2017, 03, 24, 12, 34, 56, TimeSpan.Zero);

            protected MostUsedTimeEntrySuggestionProviderTest()
            {
                Provider = new MostUsedTimeEntrySuggestionProvider(TimeService, DataSource, NumberOfSuggestions);

                TimeService.CurrentDateTime.Returns(_ => Now);
            }
        }

        public sealed class TheConstructor : MostUsedTimeEntrySuggestionProviderTest
        {
            [Theory, LogIfTooSlow]
            [ConstructorData]
            public void ThrowsIfAnyOfTheArgumentsIsNull(bool useDataSource, bool useTimeService)
            {
                var dataSource = useDataSource ? DataSource : null;
                var timeService = useTimeService ? TimeService : null;

                Action tryingToConstructWithEmptyParameters =
                    () => new MostUsedTimeEntrySuggestionProvider(timeService, dataSource, NumberOfSuggestions);

                tryingToConstructWithEmptyParameters
                    .Should().Throw<ArgumentNullException>();
            }
        }

        public sealed class TheGetSuggestionsMethod : MostUsedTimeEntrySuggestionProviderTest
        {
            private IEnumerable<IThreadSafeTimeEntry> getCustomTimeEntries(int count, Func<MockTimeEntry, int, MockTimeEntry> transform)
                => getRepeatingTimeEntries(count)
                    .OfType<MockTimeEntry>()
                    .Select((te, index) => transform(te, index));

            private IEnumerable<IThreadSafeTimeEntry> getRepeatingTimeEntries(params int[] numberOfRepetitions)
            {
                var workspace = new MockWorkspace(12);

                for (int i = 0; i < numberOfRepetitions.Length; i++)
                {
                    var project = new MockProject(i + 1000, workspace) { Active = true };

                    for (int j = 0; j < numberOfRepetitions[i]; j++)
                    {
                        yield return new MockTimeEntry(12, workspace, Now, 30, project) { Description = $"te{i}" };
                    }
                }
            }

            [Fact, LogIfTooSlow]
            public async Task ReturnsEmptyObservableIfThereAreNoTimeEntries()
            {
                DataSource.TimeEntries
                          .GetAll(Arg.Any<Func<IDatabaseTimeEntry, bool>>())
                          .Returns(Observable.Empty<IEnumerable<IThreadSafeTimeEntry>>());

                var suggestions = await Provider.GetSuggestions().ToList();

                suggestions.Should().HaveCount(0);
            }

            [Property(StartSize = 1, EndSize = 10, MaxTest = 10)]
            public void ReturnsUpToNSuggestionsWhereNIsTheNumberUsedWhenConstructingTheProvider(
                NonNegativeInt numberOfSuggestions)
            {
                var provider = new MostUsedTimeEntrySuggestionProvider(TimeService, DataSource, numberOfSuggestions.Get);

                var timeEntries = getRepeatingTimeEntries(2, 2, 2, 3, 3, 4, 5, 5, 6, 6, 7, 7, 7, 8, 8, 9);

                DataSource.TimeEntries
                          .GetAll(Arg.Any<Func<IDatabaseTimeEntry, bool>>())
                          .Returns(Observable.Return(timeEntries));

                var suggestions = provider.GetSuggestions().ToList().Wait();

                suggestions.Should().HaveCount(numberOfSuggestions.Get);
            }

            [Fact, LogIfTooSlow]
            public async Task SortsTheSuggestionsByUsage()
            {
                var timeEntries = getRepeatingTimeEntries(5, 3, 2, 5, 4, 4, 5, 4, 3).ToArray();
                var expectedDescriptions = new[] { 0, 3, 6, 4, 5, 7, 1 }.Select(i => $"te{i}");

                DataSource.TimeEntries
                          .GetAll(Arg.Any<Func<IDatabaseTimeEntry, bool>>())
                          .Returns(Observable.Return(timeEntries));

                var suggestions = await Provider.GetSuggestions().ToList();

                suggestions.Should().OnlyContain(suggestion => expectedDescriptions.Contains(suggestion.Description));
            }

            [Fact, LogIfTooSlow]
            public async Task DoesNotReturnTimeEntriesWithoutDescriptionAndProject()
            {
                var timeEntries = getCustomTimeEntries(10, (te, index) =>
                {
                    te.Project = index % 2 == 0 ? te.Project : null;
                    te.ProjectId = index % 2 == 0 ? te.ProjectId : null;
                    te.Description = index % 3 == 0 ? te.Description : "";
                    return te;
                });

                timeEntries = timeEntries
                    .Concat(getRepeatingTimeEntries(1, 2, 3, 4, 5))
                    .ToList();

                DataSource.TimeEntries
                          .GetAll(Arg.Any<Func<IDatabaseTimeEntry, bool>>())
                          .Returns(c => Observable.Return(
                                timeEntries.Where(c.Arg<Func<IDatabaseTimeEntry, bool>>()).OfType<IThreadSafeTimeEntry>()));

                var suggestions = await Provider.GetSuggestions().ToList();

                suggestions.Should().OnlyContain(
                    suggestion => !string.IsNullOrWhiteSpace(suggestion.Description) || suggestion.ProjectId.HasValue
                );
            }

            [Fact, LogIfTooSlow]
            public async Task DoesNotReturnUnsyncedEntries()
            {
                const string synced = "SYNCED";
                var syncStatusesCount = Enum.GetValues(typeof(SyncStatus)).Length;
                var timeEntries = getCustomTimeEntries(10, (te, index) =>
                {
                    te.SyncStatus = (SyncStatus)(index % syncStatusesCount);
                    te.Description = te.SyncStatus == SyncStatus.InSync ? synced : te.Description;
                    return te;
                });

                DataSource.TimeEntries
                          .GetAll(Arg.Any<Func<IDatabaseTimeEntry, bool>>())
                          .Returns(c => Observable.Return(
                                timeEntries.Where(c.Arg<Func<IDatabaseTimeEntry, bool>>()).OfType<IThreadSafeTimeEntry>()));

                var suggestions = await Provider.GetSuggestions().ToList();

                suggestions.Should().OnlyContain(suggestion =>
                    suggestion.Description == synced
                    && !string.IsNullOrWhiteSpace(suggestion.Description)
                    || suggestion.ProjectId.HasValue
                );
            }

            [Fact]
            public void NeverThrows()
            {
                var exception = new Exception();
                DataSource.TimeEntries.GetAll(Arg.Any<Func<IDatabaseTimeEntry, bool>>()).Returns(Observable.Throw<IEnumerable<IThreadSafeTimeEntry>>(exception));

                Action getSuggestions = () => Provider.GetSuggestions().Subscribe();
                getSuggestions.Should().NotThrow();
            }

            [Fact]
            public void ReturnsNoSuggestionsInCaseOfError()
            {
                var exception = new Exception();
                DataSource.TimeEntries.GetAll(Arg.Any<Func<IDatabaseTimeEntry, bool>>()).Returns(Observable.Throw<IEnumerable<IThreadSafeTimeEntry>>(exception));

                Provider.GetSuggestions().Count().Wait().Should().Be(0);
            }
        }
    }
}
