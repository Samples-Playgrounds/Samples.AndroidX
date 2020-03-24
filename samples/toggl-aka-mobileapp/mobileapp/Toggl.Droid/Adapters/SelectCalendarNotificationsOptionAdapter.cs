using Android.Views;
using Toggl.Core.UI.ViewModels.Selectable;
using Toggl.Droid.ViewHolders;

namespace Toggl.Droid.Adapters
{
    public sealed class SelectCalendarNotificationsOptionAdapter : BaseRecyclerAdapter<SelectableCalendarNotificationsOptionViewModel>
    {
        protected override BaseRecyclerViewHolder<SelectableCalendarNotificationsOptionViewModel> CreateViewHolder(ViewGroup parent, LayoutInflater inflater, int viewType)
        {
            var inflatedView = inflater.Inflate(Resource.Layout.SelectCalendarNotificationsOptionItem, parent, false);
            return new SelectCalendarNotificationsOptionViewHolder(inflatedView);
        }
    }
}
