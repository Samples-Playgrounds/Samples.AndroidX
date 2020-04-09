using Toggl.Shared.Extensions;

namespace Toggl.iOS.ViewControllers.Settings.Models
{
    public class ButtonRow : ISettingRow
    {
        public string Title { get; }
        public ViewAction Action { get; }

        public ButtonRow(string title, ViewAction action)
        {
            Title = title;
            Action = action;
        }
    }
}
