using Toggl.Shared.Extensions;

namespace Toggl.iOS.ViewControllers.Settings.Models
{
    public class InfoRow : ISettingRow
    {
        public string Title { get; }
        public ViewAction Action { get; }

        public string Detail { get; }

        public InfoRow(string title, string detail)
        {
            Title = title;
            Detail = detail;
        }
    }
}
