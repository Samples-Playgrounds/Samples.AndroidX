using Intents;
using Toggl.iOS.Intents;
using Toggl.Shared;
using System;

namespace Toggl.iOS.Models
{
    public enum SiriShortcutType
    {
        Start,
        StartFromClipboard,
        Continue,
        Stop,
        CustomStart,
        ShowReport,
        CustomReport
    }

    static class SiriShortcutTypeExtensions
    {
        public static string Title(this SiriShortcutType shortcutType)
        {
            switch (shortcutType)
            {
                case SiriShortcutType.Start:
                    return Resources.SiriShortcutsStartTimer;
                case SiriShortcutType.StartFromClipboard:
                    return Resources.SiriShortcutsStartFromClipboard;
                case SiriShortcutType.Continue:
                    return Resources.SiriShortcutsContinueTracking;
                case SiriShortcutType.Stop:
                    return Resources.SiriShortcutsStopRunningEntry;
                case SiriShortcutType.CustomStart:
                    return Resources.SiriShortcutsStartTimerWithCustomDetails;
                case SiriShortcutType.ShowReport:
                    return Resources.SiriShortcutsShowReport;
                case SiriShortcutType.CustomReport:
                    return Resources.SiriShortcutsShowCustomReport;
                default:
                    throw new ArgumentOutOfRangeException(nameof(shortcutType), shortcutType, null);
            }
        }
    }
    static class INIntentExtensions
    {
        public static SiriShortcutType ShortcutType(this INIntent intent)
        {
            if (intent is StartTimerIntent startTimerIntent)
            {
                if (startTimerIntent.EntryDescription != null ||
                    startTimerIntent.Billable != null ||
                    startTimerIntent.Tags != null ||
                    startTimerIntent.ProjectId != null)
                    return SiriShortcutType.CustomStart;

                return SiriShortcutType.Start;
            }

            if (intent is StartTimerFromClipboardIntent startFromClipboardTimerIntent)
            {
                if (startFromClipboardTimerIntent.Billable != null ||
                    startFromClipboardTimerIntent.Tags != null ||
                    startFromClipboardTimerIntent.ProjectId != null)
                    return SiriShortcutType.CustomStart;

                return SiriShortcutType.StartFromClipboard;
            }

            if (intent is StopTimerIntent)
                return SiriShortcutType.Stop;

            if (intent is ContinueTimerIntent)
                return SiriShortcutType.Continue;

            if (intent is ShowReportIntent)
                return SiriShortcutType.ShowReport;

            if (intent is ShowReportPeriodIntent)
                return SiriShortcutType.CustomReport;

            throw new ArgumentOutOfRangeException(nameof(intent));
        }
    }
}
