using CoreAnimation;
using CoreGraphics;
using Foundation;
using System;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Runtime.InteropServices;
using Toggl.Core.UI.ViewModels.Reports;
using Toggl.iOS.Cells;
using Toggl.iOS.Extensions;
using Toggl.iOS.Extensions.Reactive;
using Toggl.Shared;
using Toggl.Shared.Extensions;
using UIKit;

namespace Toggl.iOS.Views.Reports
{
    public partial class ReportsHeaderView : BaseTableHeaderFooterView<ReportsViewModel>
    {
        public static readonly string Identifier = nameof(ReportsHeaderView);
        public static readonly NSString Key = new NSString(nameof(ReportsHeaderView));
        public static readonly UINib Nib;

        private readonly CompositeDisposable disposeBag = new CompositeDisposable();

        private ReportsOverviewCardView overview = ReportsOverviewCardView.CreateFromNib();
        private ReportsBarChartCardView barChart = ReportsBarChartCardView.CreateFromNib();

        private CAShapeLayer mask = new CAShapeLayer();

        static ReportsHeaderView()
        {
            Nib = UINib.FromName(nameof(ReportsHeaderView), NSBundle.MainBundle);
        }

        public ReportsHeaderView(IntPtr handle) : base(handle)
        {
            // Note: this .ctor should not contain any initialization logic.
        }

        public override void AwakeFromNib()
        {
            base.AwakeFromNib();

            OverviewContainerView.AddSubview(overview);
            BarChartContainerView.AddSubview(barChart);
            overview.Frame = OverviewContainerView.Bounds;
            barChart.Frame = BarChartContainerView.Bounds;

            EmptyStateTitleLabel.Text = Resources.ReportsEmptyStateTitle;
            EmptyStateDescriptionLabel.Text = Resources.ReportsEmptyStateDescription;

            PieChartBackground.BackgroundColor = TraitCollection.HorizontalSizeClass == UIUserInterfaceSizeClass.Regular
                ? ColorAssets.CellBackground
                : UIColor.Clear;
        }

        public override void LayoutSubviews()
        {
            base.LayoutSubviews();

            if (BarChartContainerView != null)
            {
                BarChartContainerView.Hidden = TraitCollection.HorizontalSizeClass == UIUserInterfaceSizeClass.Regular;
            }

            if (TraitCollection.HorizontalSizeClass == UIUserInterfaceSizeClass.Regular)
            {
                // iPad -> turn this into a card
                // A card has rounded corners and a shadow
                var cornerRadius = 8;

                // when there are some projects, the bottom part of the card
                // is the reports "legend" table view
                var cornersToRound = PieChartView.Segments.Count() > 0
                    ? UIRectCorner.TopLeft | UIRectCorner.TopRight
                    : UIRectCorner.TopLeft | UIRectCorner.TopRight | UIRectCorner.BottomLeft | UIRectCorner.BottomRight;

                mask.Path = UIBezierPath.FromRoundedRect(Bounds, cornersToRound, new CGSize(cornerRadius, cornerRadius)).CGPath;
                Layer.Mask = mask;

                Layer.ShadowColor = UIColor.Black.CGColor;
                Layer.ShadowRadius = 8;
                Layer.ShadowOffset = new CGSize(0, 2);
                Layer.ShadowOpacity = 0.1f;
            }
            else
            {
                Layer.Mask = null;
            }
        }

        protected override void UpdateView()
        {
            overview.Item = Item;
            barChart.Item = Item;

            //Loading chart
            Item.GroupedSegmentsObservable
                .Select(segments => segments == null)
                .Subscribe(LoadingPieChartView.Rx().IsVisibleWithFade())
                .DisposedBy(disposeBag);

            //Pretty stuff
            Item.GroupedSegmentsObservable
                .Subscribe(groupedSegments => PieChartView.Segments = groupedSegments)
                .DisposedBy(disposeBag);

            Item.IsLoadingObservable
                .Select(CommonFunctions.Invert)
                .Subscribe(BarChartContainerView.Rx().IsVisible())
                .DisposedBy(disposeBag);

            //Visibility
            Item.ShowEmptyStateObservable
                .Subscribe(EmptyStateView.Rx().IsVisible())
                .DisposedBy(disposeBag);
        }
    }
}
