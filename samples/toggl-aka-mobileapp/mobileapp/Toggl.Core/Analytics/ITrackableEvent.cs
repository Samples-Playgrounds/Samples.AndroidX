using System.Collections.Generic;

namespace Toggl.Core.Analytics
{
    public interface ITrackableEvent
    {
        string EventName { get; }
        Dictionary<string, string> ToDictionary();
    }
}
