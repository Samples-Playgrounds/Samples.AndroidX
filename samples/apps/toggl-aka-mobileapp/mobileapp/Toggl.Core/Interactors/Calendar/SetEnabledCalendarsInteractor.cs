using System.Reactive;
using Toggl.Shared;
using Toggl.Storage.Settings;

namespace Toggl.Core.Interactors.Calendar
{
    public sealed class SetEnabledCalendarsInteractor : IInteractor<Unit>
    {
        private readonly IUserPreferences userPreferences;

        private readonly string[] selectedCalendarIds;

        public SetEnabledCalendarsInteractor(IUserPreferences userPreferences, params string[] selectedCalendarIds)
        {
            Ensure.Argument.IsNotNull(userPreferences, nameof(userPreferences));
            Ensure.Argument.IsNotNull(selectedCalendarIds, nameof(selectedCalendarIds));

            this.userPreferences = userPreferences;
            this.selectedCalendarIds = selectedCalendarIds;
        }

        public Unit Execute()
        {
            userPreferences.SetEnabledCalendars(selectedCalendarIds);
            return Unit.Default;
        }
    }
}
