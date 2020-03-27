using System;

namespace Toggl.Core.Services
{
    public interface IBackgroundService
    {
        void EnterBackground();
        void EnterForeground();
        void EnterBackgroundFetch();

        bool AppIsInBackground { get; }
        IObservable<TimeSpan> AppResumedFromBackground { get; }
    }
}
