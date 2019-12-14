using System;
using System.Reactive.Linq;
using Toggl.Core.Calendar;
using Toggl.Core.Services;
using Toggl.Shared;

namespace Toggl.Core.Interactors.Calendar
{
    internal sealed class GetCalendarItemWithIdInteractor : IInteractor<IObservable<CalendarItem>>
    {
        private readonly string id;
        private readonly ICalendarService calendarService;

        public GetCalendarItemWithIdInteractor(ICalendarService calendarService, string id)
        {
            Ensure.Argument.IsNotNull(id, nameof(id));
            Ensure.Argument.IsNotNull(calendarService, nameof(calendarService));

            this.id = id;
            this.calendarService = calendarService;
        }

        public IObservable<CalendarItem> Execute()
            => calendarService
                .GetEventWithId(id)
                .Catch<CalendarItem, Exception>(
                    ex => Observable.Empty<CalendarItem>()
                );
    }
}
