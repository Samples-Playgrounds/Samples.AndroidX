using System;

namespace Toggl.Core.UI.Views
{
    public interface IGoogleTokenProvider
    {
        IObservable<string> GetGoogleToken();
    }
}
