using Intents;
using Toggl.Core.Models.Interfaces;
using Toggl.iOS.Models;
using Toggl.Shared;

namespace Toggl.iOS.ViewControllers.Settings
{
    public class SiriShortcutViewModel
    {
        public string Title { get; }
        public string InvocationPhrase { get; }
        public string Description { get; }
        public string WorkspaceName { get; }
        public string ProjectName { get; }
        public string ClientName { get; }
        public string ProjectColor { get; }
        public bool HasTags { get; }
        public bool IsBillable { get; }
        public bool IsCustomStart { get; }
        public bool IsActive { get; }
        public SiriShortcutType Type { get; }
        public INVoiceShortcut VoiceShortcut { get; }

        public SiriShortcutViewModel(SiriShortcut siriShortcut, IThreadSafeProject project = null)
        {
            if (siriShortcut.VoiceShortcut != null)
            {
                VoiceShortcut = siriShortcut.VoiceShortcut;
                Type = VoiceShortcut.Shortcut.Intent.ShortcutType();
                Title = Type.Title();
                InvocationPhrase = VoiceShortcut.InvocationPhrase;
                IsActive = true;

                Description = siriShortcut.Parameters.Description;
                WorkspaceName = siriShortcut.Parameters.WorkspaceName;
                HasTags = siriShortcut.Parameters.Tags != null;
                IsBillable = siriShortcut.Parameters.Billable;

                ProjectName = project?.Name;
                ClientName = project?.Client?.Name;
                ProjectColor = project?.Color;

                if (Type == SiriShortcutType.CustomStart)
                {
                    Title = Description == null
                        ? Type.Title()
                        : string.Format(Resources.SiriShortcutsStartTimerWithName, Description);
                    IsCustomStart = true;
                }
            }
            else
            {
                Type = siriShortcut.Type;
                Title = Type.Title();
                IsActive = false;
            }
        }

        public bool IsTimerShortcut()
        {
            return Type == SiriShortcutType.Stop || Type == SiriShortcutType.Start ||
                   Type == SiriShortcutType.Continue || Type == SiriShortcutType.CustomStart ||
                   Type == SiriShortcutType.StartFromClipboard;
        }

        public bool IsReportsShortcut()
        {
            return Type == SiriShortcutType.ShowReport || Type == SiriShortcutType.CustomReport;
        }
    }
}
