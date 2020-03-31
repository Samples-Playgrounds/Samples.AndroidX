using Android.Graphics;
using Android.Graphics.Drawables;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System;
using Toggl.Core.UI.Parameters;
using Toggl.Core.UI.ViewModels.ReportsCalendar.QuickSelectShortcuts;

namespace Toggl.Droid.ViewHolders
{
    public class ReportsCalendarShortcutCellViewHolder : BaseRecyclerViewHolder<ReportsCalendarBaseQuickSelectShortcut>
    {
        public static ReportsCalendarShortcutCellViewHolder Create(View itemView)
            => new ReportsCalendarShortcutCellViewHolder(itemView);

        private TextView shortcutText;
        private GradientDrawable backgroundDrawable;

        public ReportsCalendarShortcutCellViewHolder(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
        {
        }

        public ReportsCalendarShortcutCellViewHolder(View itemView) : base(itemView)
        {
        }

        protected override void InitializeViews()
        {
            shortcutText = ItemView as TextView;
            backgroundDrawable = shortcutText?.Background as GradientDrawable;
        }

        protected override void UpdateView()
        {
            shortcutText.Text = Item.Title;
        }

        public void UpdateSelectionState(ReportsDateRangeParameter currentDateRange)
        {
            backgroundDrawable.SetColor(Item.IsSelected(currentDateRange) ? Color.ParseColor("#328fff") : Color.ParseColor("#5E5B5B"));
            backgroundDrawable.InvalidateSelf();
        }
    }
}
