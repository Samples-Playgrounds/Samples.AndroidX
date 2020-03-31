using CoreAnimation;
using CoreGraphics;
using Foundation;
using System;
using Toggl.Core.UI.Helper;
using Toggl.iOS.Extensions;
using UIKit;

namespace Toggl.iOS.Views.Reports
{
    [Register(nameof(PercentageView))]
    public sealed class PercentageView : UIView
    {
        private static readonly UIColor disabledColor = Colors.Reports.PercentageDisabled.ToNativeColor();
        private static readonly UIColor activeColor = Colors.Reports.PercentageActivatedBackground.ToNativeColor();

        private readonly CALayer percentageFilledLayer = new CALayer();

        private float? percentage = null;
        public float? Percentage
        {
            get => percentage;
            set
            {
                if (percentage == value) return;
                percentage = value;

                BackgroundColor = percentage.HasValue ? activeColor : disabledColor;

                var width = value == 0 ? 0 : (Frame.Width / 100) * (percentage ?? 0);
                percentageFilledLayer.Frame = new CGRect(0, 0, width, Frame.Height);
            }
        }

        public PercentageView(IntPtr handle) : base(handle)
        {
        }

        public override void AwakeFromNib()
        {
            base.AwakeFromNib();

            Layer.AddSublayer(percentageFilledLayer);
            percentageFilledLayer.BackgroundColor = Colors.Reports.PercentageActivated.ToNativeColor().CGColor;
            percentageFilledLayer.Frame = new CGRect(0, 0, 0, Frame.Height);
            percentageFilledLayer.CornerRadius = Layer.CornerRadius;
        }
    }
}
