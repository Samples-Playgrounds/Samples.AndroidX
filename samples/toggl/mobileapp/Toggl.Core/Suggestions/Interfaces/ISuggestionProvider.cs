using System;

namespace Toggl.Core.Suggestions
{
    public interface ISuggestionProvider
    {
        IObservable<Suggestion> GetSuggestions();
    }
}
