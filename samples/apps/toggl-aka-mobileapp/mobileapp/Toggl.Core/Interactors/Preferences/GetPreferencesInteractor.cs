using System;
using Toggl.Core.DataSources;
using Toggl.Core.Models.Interfaces;

namespace Toggl.Core.Interactors
{
    internal sealed class GetPreferencesInteractor : IInteractor<IObservable<IThreadSafePreferences>>
    {
        private readonly ITogglDataSource dataSource;

        public GetPreferencesInteractor(ITogglDataSource dataSource)
        {
            this.dataSource = dataSource;
        }

        public IObservable<IThreadSafePreferences> Execute()
            => dataSource.Preferences.Current;
    }
}
