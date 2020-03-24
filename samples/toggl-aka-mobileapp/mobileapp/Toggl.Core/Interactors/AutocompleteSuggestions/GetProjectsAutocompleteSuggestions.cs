using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using Toggl.Core.Autocomplete.Suggestions;
using Toggl.Core.DataSources.Interfaces;
using Toggl.Core.Models.Interfaces;
using Toggl.Shared;
using Toggl.Shared.Extensions;
using Toggl.Storage.Models;

namespace Toggl.Core.Interactors.AutocompleteSuggestions
{
    internal sealed class GetProjectsAutocompleteSuggestions : IInteractor<IObservable<IEnumerable<AutocompleteSuggestion>>>
    {
        private readonly IDataSource<IThreadSafeProject, IDatabaseProject> dataSource;

        private readonly IList<string> wordsToQuery;

        public GetProjectsAutocompleteSuggestions(IDataSource<IThreadSafeProject, IDatabaseProject> dataSource, IList<string> wordsToQuery)
        {
            Ensure.Argument.IsNotNull(dataSource, nameof(dataSource));
            Ensure.Argument.IsNotNull(wordsToQuery, nameof(wordsToQuery));

            this.dataSource = dataSource;
            this.wordsToQuery = wordsToQuery;
        }

        public IObservable<IEnumerable<AutocompleteSuggestion>> Execute()
            => getProjectsForSuggestions().Select(ProjectSuggestion.FromProjects);

        private IObservable<IEnumerable<IThreadSafeProject>> getProjectsForSuggestions()
            => wordsToQuery.Count == 0
                ? getAllProjects()
                : getAllProjectsFiltered();

        private IObservable<IEnumerable<IThreadSafeProject>> getAllProjects()
            => dataSource.GetAll(project => project.Active);

        private IObservable<IEnumerable<IThreadSafeProject>> getAllProjectsFiltered()
            => wordsToQuery.Aggregate(getAllProjects(), (obs, word) => obs.Select(filterProjectsByWord(word)));

        private Func<IEnumerable<IThreadSafeProject>, IEnumerable<IThreadSafeProject>> filterProjectsByWord(string word)
            => projects =>
                projects.Where(
                    p => p.Name.ContainsIgnoringCase(word)
                         || (p.Client != null && p.Client.Name.ContainsIgnoringCase(word))
                         || (p.Tasks != null && p.Tasks.Any(task => task.Name.ContainsIgnoringCase(word))));
    }
}
