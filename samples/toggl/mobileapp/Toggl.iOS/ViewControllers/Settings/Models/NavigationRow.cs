using Toggl.Shared.Extensions;

namespace Toggl.iOS.ViewControllers.Settings.Models
{
    public class NavigationRow : ISettingRow
    {
        public string Title { get; }
        public string Detail { get; }
        public ViewAction Action { get; }

        public NavigationRow(string title, string detail, ViewAction action = null)
        {
            Title = title;
            Action = action;
            Detail = detail;
        }

        public NavigationRow(string title, ViewAction action = null)
            : this(title, null, action)
        {
        }
    }
}
