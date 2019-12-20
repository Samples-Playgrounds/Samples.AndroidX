using Toggl.Core.UI.Interfaces;
using Toggl.Shared;

namespace Toggl.Core.UI.ViewModels.Selectable
{
    public sealed class SelectableUserCalendarViewModel : IDiffableByIdentifier<SelectableUserCalendarViewModel>
    {
        public string Id { get; }

        public string Name { get; }

        public string SourceName { get; }

        public bool Selected { get; set; }

        public long Identifier => Id.GetHashCode();

        public SelectableUserCalendarViewModel(UserCalendar calendar, bool selected)
        {
            Ensure.Argument.IsNotNull(calendar, nameof(calendar));

            Id = calendar.Id;
            Name = calendar.Name;
            SourceName = calendar.SourceName;
            Selected = selected;
        }

        public bool Equals(SelectableUserCalendarViewModel other)
        {
            return Id == other.Id;
        }
    }
}
