using CoreGraphics;
using Foundation;
using System;
using System.Collections.Generic;
using System.Linq;
using Toggl.Core.Reports;
using Toggl.iOS.Extensions;
using UIKit;
using static Toggl.Shared.Math;
using Color = Toggl.Shared.Color;
using Math = System.Math;
using Point = Toggl.Shared.Point;

namespace Toggl.iOS.Views.Reports
{
    [Register(nameof(PieChartView))]
    public sealed class PieChartView : UIView
    {
        private const float padding = 8.0f;
        private const float linesSeparatorHeight = 0.5f * padding;
        private const int minVisibleLetters = 8;
        private const float paddingMultiplicatorForProjectName = 3.5f;
        
        private Point nameCoordinates = new Point();
        private Point percentageCoordinates = new Point();
        
        private static readonly UIStringAttributes attributes = new UIStringAttributes
        {
            Font = UIFont.SystemFontOfSize(10, UIFontWeight.Semibold),
            ForegroundColor = UIColor.White
        };

        private IEnumerable<ChartSegment> segments = new ChartSegment[0];
        public IEnumerable<ChartSegment> Segments
        {
            get => segments;
            set
            {
                segments = value;
                SetNeedsDisplay();
            }
        }

        public PieChartView(IntPtr handle) : base(handle)
        {
        }

        private static bool isSegmentOnTheRight(float endAngle) =>
            endAngle > (float) -Math.PI / 2.0f && endAngle < 0
            || (endAngle >= 0 && endAngle <= (float) Math.PI / 2.0f);

        public override void Draw(CGRect rect)
        {
            var ctx = UIGraphics.GetCurrentContext();
            if (ctx == null) return;

            var viewCenterX = Bounds.Size.Width * 0.5f;
            var viewCenterY = Bounds.Size.Height * 0.5f;
            var radius = viewCenterX;
            var totalSeconds = (float) Segments.Select(x => x.TrackedTime.TotalSeconds).Sum();

            var startAngle = (float) Math.PI * -0.5f;

            foreach (var segment in Segments)
            {
                ctx.SetFillColor(new Color(segment.Color).ToNativeColor().CGColor);

                var percent = (float) segment.TrackedTime.TotalSeconds / totalSeconds;
                var endAngle = startAngle + (float) FullCircle * percent;

                // Draw arc
                ctx.MoveTo(viewCenterX, viewCenterY);
                ctx.AddArc(viewCenterX, viewCenterY, radius, startAngle, endAngle, clockwise: false);
                ctx.FillPath();

                // Disable drawing on segments that are too small
                if (ProjectSummaryReport.ShouldDraw(percent))
                {
                    // Save state for restoring later.
                    ctx.SaveState();

                    var isOnTheRight = isSegmentOnTheRight(endAngle);
                    var integerPercentage = (int) (percent * 100);
                    var nameToDraw = new NSAttributedString(segment.FormattedName(), attributes);
                    var percentageToDraw = new NSAttributedString($"{integerPercentage}%", attributes);

                    var textWidth = nameToDraw.Size.Width;
                    var textHeight = nameToDraw.Size.Height;
                    var initialTextLength = nameToDraw.Length;
                    
                    var shortenBy = 1;
                    while(shortenBy < initialTextLength - minVisibleLetters
                          && textWidth >= radius - paddingMultiplicatorForProjectName * padding)
                    {
                        nameToDraw = new NSAttributedString(segment.FormattedName(shortenBy), attributes);
                        textWidth = nameToDraw.Size.Width;
                        textHeight = nameToDraw.Size.Height;
                        ++shortenBy;
                    }

                    var percentWidth = percentageToDraw.Size.Width;
                    var percentHeight = percentageToDraw.Size.Height;

                    // Translate to draw the text
                    ctx.TranslateCTM(viewCenterX, viewCenterY);
                    if (isOnTheRight)
                    {
                        ctx.RotateCTM(endAngle);

                        nameCoordinates.X = radius - padding - textWidth;
                        nameCoordinates.Y = -padding - textHeight;

                        percentageCoordinates.X = radius - padding - percentWidth;
                        percentageCoordinates.Y = -(padding + linesSeparatorHeight) - textHeight - percentHeight;
                    }
                    else
                    {
                        ctx.RotateCTM(endAngle + (float) Math.PI);
                        
                        nameCoordinates.X = -radius + padding;
                        nameCoordinates.Y = padding;

                        percentageCoordinates.X = -radius + padding;
                        percentageCoordinates.Y = textHeight + padding + linesSeparatorHeight;
                    }

                    nameToDraw.DrawString(new CGPoint(x: nameCoordinates.X, y: nameCoordinates.Y));
                    percentageToDraw.DrawString(new CGPoint(x: percentageCoordinates.X, y: percentageCoordinates.Y));

                    // Restore the original coordinate system.
                    ctx.RestoreState();
                }

                startAngle = endAngle;
            }
        }
    }
}