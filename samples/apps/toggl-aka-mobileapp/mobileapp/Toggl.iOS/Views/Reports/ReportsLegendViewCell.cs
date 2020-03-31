using CoreAnimation;
using CoreGraphics;
using Foundation;
using System;
using Toggl.Core.Reports;
using Toggl.iOS.Cells;
using Toggl.iOS.Extensions;
using Toggl.Shared;
using Toggl.Shared.Extensions;
using UIKit;

namespace Toggl.iOS.Views.Reports
{
    public partial class ReportsLegendViewCell : BaseTableViewCell<ChartSegment>
    {
        public static readonly string Identifier = nameof(ReportsLegendViewCell);
        public static readonly NSString Key = new NSString(nameof(ReportsLegendViewCell));
        public static readonly UINib Nib;

        private CAShapeLayer mask = new CAShapeLayer();
        private UIView bottomSeparator;
        private bool isLast = false;

        static ReportsLegendViewCell()
        {
            Nib = UINib.FromName(nameof(ReportsLegendViewCell), NSBundle.MainBundle);
        }

        protected ReportsLegendViewCell(IntPtr handle) : base(handle)
        {
            // Note: this .ctor should not contain any initialization logic.
        }

        public override void AwakeFromNib()
        {
            base.AwakeFromNib();

            FadeView.FadeRight = true;
            bottomSeparator = ContentView.InsertSeparator();
            ContentView.InsertSeparator(UIRectEdge.Top);
        }

        public void SetIsLast(bool last)
        {
            isLast = last;
        }

        protected override void UpdateView()
        {
            ProjectLabel.SetKerning(-0.2);
            ClientLabel.SetKerning(-0.2);
            TotalTimeLabel.SetKerning(-0.2);
            PercentageLabel.SetKerning(-0.2);

            //Text
            ProjectLabel.Text = Item.ProjectName;
            ClientLabel.Text = Item.ClientName;
            PercentageLabel.Text = $"{Item.Percentage:F2}%";
            TotalTimeLabel.Text = Item.TrackedTime.ToFormattedString(Item.DurationFormat);

            ClientLabel.Hidden = !Item.HasClient;

            // Color
            var color = new Color(Item.Color).ToNativeColor();
            ProjectLabel.TextColor = color;
            CircleView.BackgroundColor = color;
        }

        public override void LayoutSubviews()
        {
            base.LayoutSubviews();

            Layer.Mask = null;
            bottomSeparator.Hidden = true;

            if (TraitCollection.HorizontalSizeClass == UIUserInterfaceSizeClass.Regular)
            {
                Layer.ShadowColor = UIColor.Black.CGColor;
                Layer.ShadowRadius = 8;
                Layer.ShadowOffset = new CGSize(0, 2);
                Layer.ShadowOpacity = 0.1f;

                if (isLast)
                {
                    var cornerRadius = 8;
                    var cornersToRound = UIRectCorner.BottomLeft | UIRectCorner.BottomRight;

                    mask.Path = UIBezierPath.FromRoundedRect(Bounds, cornersToRound, new CGSize(cornerRadius, cornerRadius)).CGPath;
                    Layer.Mask = mask;
                }
            }
            else if (isLast)
            {
                bottomSeparator.Hidden = false;
            }
        }
    }
}
