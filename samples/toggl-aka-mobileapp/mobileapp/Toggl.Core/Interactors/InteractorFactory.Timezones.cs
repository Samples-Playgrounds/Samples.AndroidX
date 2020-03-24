using System;
using System.Collections.Generic;
using Toggl.Core.Interactors.Timezones;

namespace Toggl.Core.Interactors
{
    public sealed partial class InteractorFactory
    {
        public IInteractor<IObservable<IEnumerable<string>>> GetSupportedTimezones() =>
            new GetSupportedTimezonesInteractor();
    }
}
