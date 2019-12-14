namespace Toggl.Core.UI.Parameters
{
    public sealed class EditDurationParameters
    {
        public enum InitialFocus
        {
            Duration,
            None
        }

        public DurationParameter DurationParam { get; }
        public bool IsStartingNewEntry { get; }
        public bool IsDurationInitiallyFocused { get; }

        public EditDurationParameters(DurationParameter durationParam,
            bool isStartingNewEntry = false,
            bool isDurationInitiallyFocused = false)
        {
            DurationParam = durationParam;
            IsStartingNewEntry = isStartingNewEntry;
            IsDurationInitiallyFocused = isDurationInitiallyFocused;
        }
    }
}
