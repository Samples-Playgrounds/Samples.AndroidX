using System;
using System.Collections.Generic;

namespace Toggl.Core.Analytics
{
    public class EditDurationEvent : ITrackableEvent
    {
        public enum NavigationOrigin
        {
            Start,
            Edit
        }

        public enum Result
        {
            Cancel,
            Save
        }

        public string EventName => "EditDuration";

        private bool changedStartDateWithBarrel = false;
        private bool changedStartTimeWithBarrel = false;
        private bool changedEndDateWithBarrel = false;
        private bool changedEndTimeWithBarrel = false;
        private bool changedStartTimeWithWheel = false;
        private bool changedEndTimeWithWheel = false;
        private bool changedBothTimesWithWheel = false;
        private bool changedDurationWithNumPad = false;
        private bool entryWasRunningWhenOpened;
        private bool stoppedRunningEntry = false;
        private NavigationOrigin navigationOrigin;
        private Result result;

        private EditDurationEvent(bool changedStartDateWithBarrel, bool changedStartTimeWithBarrel,
            bool changedEndDateWithBarrel, bool changedEndTimeWithBarrel, bool changedStartTimeWithWheel,
            bool changedEndTimeWithWheel, bool changedBothTimesWithWheel, bool changedDurationWithNumPad,
            bool entryWasRunningWhenOpened, bool stoppedRunningEntry, NavigationOrigin navigationOrigin, Result result)
        {
            this.changedStartDateWithBarrel = changedStartDateWithBarrel;
            this.changedStartTimeWithBarrel = changedStartTimeWithBarrel;
            this.changedEndDateWithBarrel = changedEndDateWithBarrel;
            this.changedEndTimeWithBarrel = changedEndTimeWithBarrel;
            this.changedStartTimeWithWheel = changedStartTimeWithWheel;
            this.changedEndTimeWithWheel = changedEndTimeWithWheel;
            this.changedBothTimesWithWheel = changedBothTimesWithWheel;
            this.changedDurationWithNumPad = changedDurationWithNumPad;
            this.entryWasRunningWhenOpened = entryWasRunningWhenOpened;
            this.stoppedRunningEntry = stoppedRunningEntry;
            this.navigationOrigin = navigationOrigin;
            this.result = result;
        }

        public EditDurationEvent(bool entryWasRunningWhenOpened, NavigationOrigin navigationOrigin)
        {
            this.entryWasRunningWhenOpened = entryWasRunningWhenOpened;
            this.navigationOrigin = navigationOrigin;
        }

        public EditDurationEvent With(bool? changedStartDateWithBarrel = null, bool? changedStartTimeWithBarrel = null,
            bool? changedEndDateWithBarrel = null, bool? changedEndTimeWithBarrel = null,
            bool? changedStartTimeWithWheel = null, bool? changedEndTimeWithWheel = null,
            bool? changedBothTimesWithWheel = null, bool? changedDurationWithNumPad = null,
            bool? stoppedRunningEntry = null, Result? result = null)
        {
            return new EditDurationEvent(
                changedStartDateWithBarrel ?? this.changedStartDateWithBarrel,
                changedStartTimeWithBarrel ?? this.changedStartTimeWithBarrel,
                changedEndDateWithBarrel ?? this.changedEndDateWithBarrel,
                changedEndTimeWithBarrel ?? this.changedEndTimeWithBarrel,
                changedStartTimeWithWheel ?? this.changedStartTimeWithWheel,
                changedEndTimeWithWheel ?? this.changedEndTimeWithWheel,
                changedBothTimesWithWheel ?? this.changedBothTimesWithWheel,
                changedDurationWithNumPad ?? this.changedDurationWithNumPad,
                entryWasRunningWhenOpened,
                stoppedRunningEntry ?? this.stoppedRunningEntry,
                navigationOrigin,
                result ?? this.result
            );
        }

        public EditDurationEvent UpdateWith(EditTimeSource source)
        {
            switch (source)
            {
                case EditTimeSource.WheelStartTime:
                    return With(changedStartTimeWithWheel: true);
                case EditTimeSource.WheelEndTime:
                    return With(changedEndTimeWithWheel: true);
                case EditTimeSource.WheelBothTimes:
                    return With(changedBothTimesWithWheel: true);
                case EditTimeSource.BarrelStartTime:
                    return With(changedStartTimeWithBarrel: true);
                case EditTimeSource.BarrelStopTime:
                    return With(changedEndTimeWithBarrel: true);
                case EditTimeSource.BarrelStartDate:
                    return With(changedStartDateWithBarrel: true);
                case EditTimeSource.BarrelStopDate:
                    return With(changedEndDateWithBarrel: true);
                case EditTimeSource.NumpadDuration:
                    return With(changedDurationWithNumPad: true);
                default:
                    throw new ArgumentOutOfRangeException(nameof(source), source, null);
            }
        }

        public Dictionary<string, string> ToDictionary()
        {
            return new Dictionary<string, string>
            {
                [nameof(changedStartDateWithBarrel)] = changedStartDateWithBarrel.ToString(),
                [nameof(changedStartTimeWithBarrel)] = changedStartTimeWithBarrel.ToString(),
                [nameof(changedEndDateWithBarrel)] = changedEndDateWithBarrel.ToString(),
                [nameof(changedEndTimeWithBarrel)] = changedEndTimeWithBarrel.ToString(),
                [nameof(changedStartTimeWithWheel)] = changedStartTimeWithWheel.ToString(),
                [nameof(changedEndTimeWithWheel)] = changedEndTimeWithWheel.ToString(),
                [nameof(changedBothTimesWithWheel)] = changedBothTimesWithWheel.ToString(),
                [nameof(changedDurationWithNumPad)] = changedDurationWithNumPad.ToString(),
                [nameof(entryWasRunningWhenOpened)] = entryWasRunningWhenOpened.ToString(),
                [nameof(stoppedRunningEntry)] = stoppedRunningEntry.ToString(),
                [nameof(navigationOrigin)] = navigationOrigin.ToString(),
                [nameof(result)] = result.ToString()
            };
        }
    }
}
