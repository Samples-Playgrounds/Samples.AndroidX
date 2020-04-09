namespace Toggl.Core.UI.Helper
{
    public enum TemporalInconsistency
    {
        StartTimeAfterCurrentTime,
        StartTimeAfterStopTime,
        StopTimeBeforeStartTime,
        DurationTooLong
    }
}
