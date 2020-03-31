using Android.Views;
using Toggl.Core.UI.ViewModels.Calendar;
using Toggl.Core.UI.ViewModels.Selectable;
using Toggl.Droid.ViewHolders;

namespace Toggl.Droid.Adapters
{
    public sealed class UserCalendarsRecyclerAdapter
        : BaseSectionedRecyclerAdapter<UserCalendarSourceViewModel, SelectableUserCalendarViewModel>
    {
        protected override BaseRecyclerViewHolder<UserCalendarSourceViewModel> CreateHeaderViewHolder(LayoutInflater inflater, ViewGroup parent, int viewType)
        {
            var view = inflater.Inflate(Resource.Layout.UserCalendarHeader, parent, false);
            return new UserCalendarHeaderViewHolder(view);
        }

        protected override BaseRecyclerViewHolder<SelectableUserCalendarViewModel> CreateItemViewHolder(LayoutInflater inflater, ViewGroup parent, int viewType)
        {
            var view = inflater.Inflate(Resource.Layout.UserCalendarItem, parent, false);
            return new UserCalendarViewHolder(view);
        }
    }
}
