using Foundation;
using ObjCRuntime;
using System;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using Toggl.Core.Extensions;
using Toggl.Core.UI.Extensions;
using Toggl.Core.UI.Helper;
using Toggl.Core.UI.ViewModels.Reports;
using Toggl.iOS.Cells;
using Toggl.iOS.Extensions;
using Toggl.iOS.Extensions.Reactive;
using Toggl.Shared;
using Toggl.Shared.Extensions;
using UIKit;

namespace Toggl.iOS.Views.Reports
{
    [Register(nameof(ReportsOverviewCardView))]
    public sealed partial class ReportsOverviewCardView : BaseReportsCardView<ReportsViewModel>
    {
        private const int fontSize = 24;

        private readonly CompositeDisposable disposeBag = new CompositeDisposable();

        private static readonly UIColor normalColor = Colors.Reports.PercentageActivated.ToNativeColor();
        private static readonly UIColor disabledColor = Colors.Reports.Disabled.ToNativeColor();

        private readonly UIStringAttributes normalAttributes = new UIStringAttributes
        {
            Font = UIFont.SystemFontOfSize(fontSize, UIFontWeight.Medium),
            ForegroundColor = normalColor
        };

        private readonly UIStringAttributes disabledAttributes = new UIStringAttributes
        {
            Font = UIFont.SystemFontOfSize(fontSize, UIFontWeight.Medium),
            ForegroundColor = disabledColor
        };

        public ReportsOverviewCardView(IntPtr handle) : base(handle)
        {
            // Note: this .ctor should not contain any initialization logic.
        }

        public static ReportsOverviewCardView CreateFromNib()
        {
            var arr = NSBundle.MainBundle.LoadNib(nameof(ReportsOverviewCardView), null, null);
            return Runtime.GetNSObject<ReportsOverviewCardView>(arr.ValueAt(0));
        }

        public override void AwakeFromNib()
        {
            base.AwakeFromNib();

            TotalTitleLabel.Text = Resources.Total.ToUpper();
            BillableTitleLabel.Text = Resources.Billable.ToUpper();

            var templateImage = TotalDurationGraph.Image.ImageWithRenderingMode(UIImageRenderingMode.AlwaysTemplate);
            TotalDurationGraph.Image = templateImage;

            prepareViews();
        }

        protected override void UpdateViewBinding()
        {
            //Text
            Item.BillablePercentageObservable
                .Select(billableFormattedString)
                .Subscribe(BillablePercentageLabel.Rx().AttributedText())
                .DisposedBy(disposeBag);

            Item.TotalTimeObservable
                .CombineLatest(Item.DurationFormatObservable,
                    (totalTime, durationFormat) => totalTime.ToFormattedString(durationFormat))
                .Subscribe(TotalDurationLabel.Rx().Text())
                .DisposedBy(disposeBag);

            //Loading chart
            Item.BillablePercentageObservable
                .CombineLatest(Item.TotalTimeObservable,
                    (totalTime, percentage) => totalTime == null || percentage == null)
                .Subscribe(LoadingOverviewView.Rx().IsVisibleWithFade())
                .DisposedBy(disposeBag);

            //Pretty stuff
            Item.BillablePercentageObservable
                .Subscribe(percentage => BillablePercentageView.Percentage = percentage)
                .DisposedBy(disposeBag);

            var totalDurationColorObservable = Item.TotalTimeIsZeroObservable
                .Select(isZero => isZero
                    ? Colors.Reports.Disabled.ToNativeColor()
                    : Colors.Reports.TotalTimeActivated.ToNativeColor());

            totalDurationColorObservable
                .Subscribe(TotalDurationGraph.Rx().TintColor())
                .DisposedBy(disposeBag);

            totalDurationColorObservable
                .Subscribe(TotalDurationLabel.Rx().TextColor())
                .DisposedBy(disposeBag);

            NSAttributedString billableFormattedString(float? value)
            {
                var isDisabled = value == null;
                var actualValue = isDisabled ? 0 : value.Value;

                var percentage = $"{actualValue.ToString("0.00")}%";

                var attributes = isDisabled ? disabledAttributes : normalAttributes;
                return new NSAttributedString(percentage, attributes);
            }

            LayoutIfNeeded();
        }

        private void prepareViews()
        {
            PrepareCard(OverviewCardView);

            TotalTitleLabel.SetKerning(-0.2);
            TotalDurationLabel.SetKerning(-0.2);
            BillableTitleLabel.SetKerning(-0.2);
            BillablePercentageLabel.SetKerning(-0.2);
        }
    }
}
