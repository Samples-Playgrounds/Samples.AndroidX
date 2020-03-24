using Toggl.Shared.Extensions;

namespace Toggl.iOS.ViewControllers.Settings.Models
{
    public class CustomRow<TCustomValue> : ISettingRow
    {
        public string Title { get; }
        public ViewAction Action { get; }
        public TCustomValue CustomValue { get; }

        public CustomRow(TCustomValue customValue, ViewAction action = null)
        {
            CustomValue = customValue;
            Action = action;
        }
    }
}
