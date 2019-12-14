using Android.Runtime;
using Android.Views;
using Android.Widget;
using System;
using Toggl.Core.UI.ViewModels.Selectable;

namespace Toggl.Droid.ViewHolders
{
    public sealed class UserCalendarViewHolder : BaseRecyclerViewHolder<SelectableUserCalendarViewModel>
    {
        private CheckBox checkbox;
        private TextView calendarName;

        public static UserCalendarViewHolder Create(View itemView)
            => new UserCalendarViewHolder(itemView);

        public UserCalendarViewHolder(View itemView)
            : base(itemView)
        {
        }

        public UserCalendarViewHolder(IntPtr handle, JniHandleOwnership ownership)
            : base(handle, ownership)
        {
        }

        protected override void InitializeViews()
        {
            checkbox = ItemView.FindViewById<CheckBox>(Resource.Id.Checkbox);
            calendarName = ItemView.FindViewById<TextView>(Resource.Id.CalendarName);
        }

        protected override void UpdateView()
        {
            checkbox.Checked = Item.Selected;
            calendarName.Text = Item.Name;
        }

        protected override void OnItemViewClick(object sender, EventArgs args)
        {
            base.OnItemViewClick(sender, args);
            checkbox.Checked = !checkbox.Checked;
        }
    }
}
