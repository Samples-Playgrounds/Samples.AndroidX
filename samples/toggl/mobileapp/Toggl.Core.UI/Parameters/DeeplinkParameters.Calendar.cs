namespace Toggl.Core.UI.Parameters
{
    public sealed class DeeplinkShowCalendarParameters : DeeplinkParameters
    {
        public string EventId { get; }

        internal DeeplinkShowCalendarParameters(string eventId)
        {
            EventId = eventId;
        }
    }
}
