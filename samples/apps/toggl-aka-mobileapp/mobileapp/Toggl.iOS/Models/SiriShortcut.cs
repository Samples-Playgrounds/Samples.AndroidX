using Intents;
using System;
using System.Collections.Generic;
using System.Linq;
using Toggl.Core.Models;
using Toggl.iOS.Extensions;
using Toggl.iOS.Intents;

namespace Toggl.iOS.Models
{
    public struct SiriShortcutParameters
    {
        public string Description;
        public long? WorkspaceId;
        public string WorkspaceName;
        public bool Billable;
        public IEnumerable<long> Tags;
        public long? ProjectId;
        public ReportPeriod ReportPeriod;
    }

    public class SiriShortcut
    {
        public SiriShortcutType Type { get; }
        public string Identifier { get; }
        public INVoiceShortcut VoiceShortcut { get; }
        public SiriShortcutParameters Parameters = new SiriShortcutParameters();

        private INIntent Intent { get; }

        public SiriShortcut(INVoiceShortcut voiceShortcut)
        {
            VoiceShortcut = voiceShortcut;
            Identifier = voiceShortcut.Identifier.AsString();
            Intent = voiceShortcut.Shortcut.Intent;
            Type = Intent.ShortcutType();

            if (Intent is ShowReportPeriodIntent showReportPeriodIntent)
            {
                Parameters.WorkspaceId = stringToLong(showReportPeriodIntent.Workspace?.Identifier);
                Parameters.WorkspaceName = showReportPeriodIntent.Workspace?.DisplayString;
                Parameters.ReportPeriod = showReportPeriodIntent.Period.ToReportPeriod();
            }

            if (Intent is StartTimerIntent startTimerIntent)
            {
                Parameters.Description = startTimerIntent.EntryDescription;
                Parameters.WorkspaceId = stringToLong(startTimerIntent.Workspace?.Identifier);
                Parameters.WorkspaceName = startTimerIntent.Workspace?.DisplayString;
                Parameters.Billable = startTimerIntent.Billable?.Identifier == "True";
                Parameters.Tags = startTimerIntent.Tags == null
                    ? null
                    : stringToLongCollection(startTimerIntent.Tags.Select(tag => tag.Identifier));
                Parameters.ProjectId = stringToLong(startTimerIntent.ProjectId?.Identifier);
            }

            if (Intent is StartTimerFromClipboardIntent startTimerFromClipboardIntent)
            {
                Parameters.WorkspaceId = stringToLong(startTimerFromClipboardIntent.Workspace?.Identifier);
                Parameters.WorkspaceName = startTimerFromClipboardIntent.Workspace?.DisplayString;
                Parameters.Billable = startTimerFromClipboardIntent.Billable?.Identifier == "True";
                Parameters.Tags = startTimerFromClipboardIntent.Tags == null
                    ? null
                    : stringToLongCollection(startTimerFromClipboardIntent.Tags.Select(tag => tag.Identifier));
                Parameters.ProjectId = stringToLong(startTimerFromClipboardIntent.ProjectId?.Identifier);
            }
        }

        public SiriShortcut(SiriShortcutType type)
        {
            Type = type;
        }

        public static SiriShortcut[] DefaultShortcuts = new[]
        {
            new SiriShortcut(
                SiriShortcutType.Start
            ),
            new SiriShortcut(
                SiriShortcutType.Stop
            ),
            new SiriShortcut(
                SiriShortcutType.Continue
            ),
            new SiriShortcut(
                SiriShortcutType.StartFromClipboard
            ),
            new SiriShortcut(
                SiriShortcutType.CustomStart
            ),
            new SiriShortcut(
                SiriShortcutType.ShowReport
            ),
            new SiriShortcut(
                SiriShortcutType.CustomReport
            )
        };

        private long? stringToLong(string str)
        {
            if (string.IsNullOrEmpty(str))
                return null;

            return (long)Convert.ToDouble(str);
        }

        private IEnumerable<long> stringToLongCollection(IEnumerable<string> strings)
        {
            if (strings.Count() == 0)
                return new long[0];

            return strings.Select(stringToLong).Cast<long>();
        }
    }
}
