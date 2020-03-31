using Android.Graphics;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System;
using Toggl.Core.Reports;
using Toggl.Core.UI.Transformations;

namespace Toggl.Droid.ViewHolders
{
    public sealed class ReportsItemCellViewHolder : BaseRecyclerViewHolder<ChartSegment>
    {
        private readonly int lastContainerHeight;
        private readonly int normalContainerHeight;

        private TextView projectName;
        private TextView clientName;
        private TextView duration;
        private TextView percentage;

        public bool IsLastItem { get; set; }

        public ReportsItemCellViewHolder(View itemView, int lastContainerHeight, int normalContainerHeight) : base(itemView)
        {
            this.lastContainerHeight = lastContainerHeight;
            this.normalContainerHeight = normalContainerHeight;
        }

        public ReportsItemCellViewHolder(IntPtr handle, JniHandleOwnership ownership) : base(handle, ownership)
        {
        }

        public void RecalculateSize()
        {
            var height = IsLastItem ? lastContainerHeight : normalContainerHeight;
            var layoutParameters = ItemView.LayoutParameters;
            layoutParameters.Height = height;
            ItemView.LayoutParameters = layoutParameters;
        }

        protected override void InitializeViews()
        {
            projectName = ItemView.FindViewById<TextView>(Resource.Id.ReportsFragmentItemProjectName);
            clientName = ItemView.FindViewById<TextView>(Resource.Id.ReportsFragmentItemClientName);
            duration = ItemView.FindViewById<TextView>(Resource.Id.ReportsFragmentItemDuration);
            percentage = ItemView.FindViewById<TextView>(Resource.Id.ReportsFragmentItemPercentage);
        }

        protected override void UpdateView()
        {
            projectName.Text = Item.ProjectName;
            projectName.SetTextColor(Color.ParseColor(Item.Color));

            duration.Text = DurationAndFormatToString.Convert(Item.TrackedTime, Item.DurationFormat);

            percentage.Text = $"{Item.Percentage:0.00}%";
        }
    }
}
