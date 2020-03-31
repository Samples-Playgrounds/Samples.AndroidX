using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Toggl.Core.Extensions;
using Toggl.Core.Models.Interfaces;
using Toggl.Shared.Extensions;

namespace Toggl.Core.Search
{
    public sealed class DefaultTimeEntrySearchEngine : SearchEngine<IThreadSafeTimeEntry>
    {
        public DefaultTimeEntrySearchEngine() : base(filterTimeEntryByWord)
        {
        }

        private static bool filterTimeEntryByWord(string word, IThreadSafeTimeEntry te)
            => isProjectNullOrActive(te)
            && (te.Description.ContainsIgnoringCase(word)
            || (te.Project?.Name.ContainsIgnoringCase(word) ?? false)
            || (te.Project?.Client != null && te.Project.Client.Name.ContainsIgnoringCase(word))
            || (te.Task != null && te.Task.Name.ContainsIgnoringCase(word)));

        private static bool isProjectNullOrActive(IThreadSafeTimeEntry te)
            => te.Project?.Active ?? true;
    }
}
