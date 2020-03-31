using System;
using Toggl.Core.UI.Helper;
using Toggl.Core.UI.ViewModels.Reports;
using Toggl.iOS.Extensions;
using UIKit;

namespace Toggl.iOS.Views.Reports
{
    internal sealed class BarView : UIView
    {
        private readonly nfloat minimumHeight = 1;

        private readonly BarViewModel bar;

        private readonly UIView billableView;

        private readonly UIView nonBillableView;

        private readonly UIView minimumLevelView;

        private bool shouldSetupConstraints = true;

        public BarView(BarViewModel bar)
        {
            this.bar = bar;

            nonBillableView = new BarSegmentView(Colors.Reports.BarChart.NonBillable.ToNativeColor());
            billableView = new BarSegmentView(Colors.Reports.BarChart.Billable.ToNativeColor());
            minimumLevelView = new BarSegmentView(
                bar.BillablePercent > bar.NonBillablePercent
                    ? Colors.Reports.BarChart.Billable.ToNativeColor()
                    : Colors.Reports.BarChart.NonBillable.ToNativeColor());

            AddSubview(minimumLevelView);
            AddSubview(nonBillableView);
            AddSubview(billableView);
        }

        public override void UpdateConstraints()
        {
            base.UpdateConstraints();

            if (!shouldSetupConstraints) return;

            minimumLevelView.BottomAnchor.ConstraintEqualTo(BottomAnchor).Active = true;
            minimumLevelView.LeftAnchor.ConstraintEqualTo(LeftAnchor).Active = true;
            minimumLevelView.RightAnchor.ConstraintEqualTo(RightAnchor).Active = true;
            minimumLevelView.HeightAnchor.ConstraintEqualTo(minimumHeight).Active = true;

            billableView.BottomAnchor.ConstraintEqualTo(BottomAnchor).Active = true;
            billableView.LeftAnchor.ConstraintEqualTo(LeftAnchor).Active = true;
            billableView.RightAnchor.ConstraintEqualTo(RightAnchor).Active = true;

            nonBillableView.BottomAnchor.ConstraintEqualTo(billableView.TopAnchor).Active = true;
            nonBillableView.LeftAnchor.ConstraintEqualTo(LeftAnchor).Active = true;
            nonBillableView.RightAnchor.ConstraintEqualTo(RightAnchor).Active = true;

            billableView.HeightAnchor.ConstraintEqualTo(HeightAnchor, (nfloat)bar.BillablePercent).Active = true;
            nonBillableView.HeightAnchor.ConstraintEqualTo(HeightAnchor, (nfloat)bar.NonBillablePercent).Active = true;

            shouldSetupConstraints = false;
        }
    }
}
