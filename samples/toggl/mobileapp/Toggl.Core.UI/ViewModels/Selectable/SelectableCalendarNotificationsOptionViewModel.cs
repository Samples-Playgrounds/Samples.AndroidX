using Toggl.Core.UI.Interfaces;
using Toggl.Shared;

namespace Toggl.Core.UI.ViewModels.Selectable
{
    public sealed class SelectableCalendarNotificationsOptionViewModel : IDiffableByIdentifier<SelectableCalendarNotificationsOptionViewModel>
    {
        public CalendarNotificationsOption Option { get; }

        public bool Selected { get; set; }

        public SelectableCalendarNotificationsOptionViewModel(CalendarNotificationsOption option, bool selected)
        {
            Option = option;
            Selected = selected;
        }

        public long Identifier => Option.GetHashCode();

        public bool Equals(SelectableCalendarNotificationsOptionViewModel other) => Option == other.Option;
    }
}
