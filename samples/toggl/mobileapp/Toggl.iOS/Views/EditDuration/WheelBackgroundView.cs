using CoreAnimation;
using CoreGraphics;
using Foundation;
using System;
using Toggl.iOS.Extensions;
using Toggl.iOS.Views.EditDuration.Shapes;
using UIKit;
using static Toggl.Shared.Math;

namespace Toggl.iOS.Views.EditDuration
{
    [Register(nameof(WheelBackgroundView))]
    public sealed class WheelBackgroundView : BaseWheelView
    {
        private readonly CGColor wheelBackgroundColor = ColorAssets.CustomGray6.CGColor;

        private readonly CGColor thickSegmentColor = ColorAssets.CustomGray3.CGColor;

        private readonly CGColor thinSegmentColor = ColorAssets.CustomGray5.CGColor;

        // The sizes are relative to the radius of the wheel.
        // The radius of the wheel in the design document is 128 points.
        private readonly CGSize thickSegmentDimensions = new CGSize(1.7f / 128f, 5f / 128f);

        private readonly CGSize thinSegmentDimensions = new CGSize(0.8f / 128f, 5f / 128f);

        private readonly CGSize letterFrameDimensions = new CGSize(17f / 128f, 17f / 128f);

        private readonly nfloat letterFontSize = 13.2f / 128f;

        private readonly nfloat smallRadiusDifference = 5f / 128f;

        private readonly nfloat numbersRadiusDifference = 20f / 128f;

        private nfloat segmentsRadius;

        private nfloat numbersRadius;

        private CALayer backgroundLayer;

        public WheelBackgroundView(IntPtr handle) : base(handle)
        {
        }

        public override void LayoutSubviews()
        {
            base.LayoutSubviews();

            RemoveSublayers();

            segmentsRadius = SmallRadius - Resize(smallRadiusDifference);
            numbersRadius = SmallRadius - Resize(numbersRadiusDifference);

            backgroundLayer = createBackgroundLayer();
            Layer.AddSublayer(backgroundLayer);
        }

        private CALayer createBackgroundLayer()
        {
            var layer = new CALayer();

            var wheel = new Wheel(Center, Radius, SmallRadius, wheelBackgroundColor);
            var dial = createClockDial();

            layer.AddSublayer(wheel);
            layer.AddSublayer(dial);

            return layer;
        }

        private CALayer createClockDial()
        {
            var dial = new CALayer();
            var minuteSegmentsPerHourMark = MinutesInAnHour / HoursOnTheClock;

            var scaledThickSegment = scaleRect(thickSegmentDimensions);
            var scaledThinSegment = scaleRect(thinSegmentDimensions);

            for (int i = 1; i <= MinutesInAnHour; ++i)
            {
                var angle = (nfloat)FullCircle * (i / (nfloat)MinutesInAnHour);
                var correspondsToHourMark = i % minuteSegmentsPerHourMark == 0;
                var (rect, color) = correspondsToHourMark
                    ? (scaledThickSegment, thickSegmentColor)
                    : (scaledThinSegment, thinSegmentColor);
                var layer = createMinuteSegment(rect, color, segmentsRadius, angle);
                dial.AddSublayer(layer);

                if (correspondsToHourMark)
                {
                    var number = i / minuteSegmentsPerHourMark;
                    var numberView = createMinuteNumber(i, color, numbersRadius, angle);
                    AddSubview(numberView);
                }
            }

            return dial;
        }

        private CALayer createMinuteSegment(CGRect rect, CGColor color, nfloat distanceFromcenter, nfloat angle)
        {
            var layer = new CAShapeLayer();

            var rotation = CGAffineTransform.MakeRotation(angle);
            var translation = CreateTranslationTransform(distanceFromcenter, angle);
            var transform = rotation * translation;

            var path = CGPath.FromRect(rect, transform);
            layer.Path = path;
            layer.FillColor = color;

            return layer;
        }

        private UIView createMinuteNumber(int number, CGColor color, nfloat distanceFromcenter, nfloat angle)
        {
            var view = new UILabel();

            view.Font = UIFont.SystemFontOfSize(Resize(letterFontSize), UIFontWeight.Regular);
            view.Text = number.ToString();
            view.TextColor = new UIColor(color);
            view.Frame = scaleRect(letterFrameDimensions);
            view.TextAlignment = UITextAlignment.Center;

            var translation = CreateTranslationTransform(distanceFromcenter, angle);
            view.Transform = translation;

            return view;
        }

        private CGRect scaleRect(CGSize size)
            => new CGRect(-Resize(size.Width) / 2f, -Resize(size.Height) / 2f, Resize(size.Width), Resize(size.Height));
    }
}
