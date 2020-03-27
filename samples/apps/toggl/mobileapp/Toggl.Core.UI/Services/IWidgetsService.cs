using System;
using System.Collections.Immutable;
using Toggl.Core.Models.Interfaces;
using Toggl.Core.Suggestions;

namespace Toggl.Core.UI.Services
{
    public interface IWidgetsService : IDisposable
    {
        void Start();
        void OnSuggestionsUpdated(IImmutableList<Suggestion> suggestions);
    }
}
