using CoreAnimation;
using CoreGraphics;
using Foundation;
using System;
using Toggl.Core.UI.Helper;
using Toggl.iOS.Extensions;
using UIKit;
using static Toggl.Core.UI.Helper.Animation;

namespace Toggl.iOS.Views.Reports
{
    [Register(nameof(LoadingPieChartView))]
    public sealed class LoadingPieChartView : UIView
    {
        private static readonly nfloat pi = (nfloat)(Math.PI);
        private static readonly nfloat fullCircle = 2 * pi;
        private static readonly nfloat startAngle = -0.5f * pi;
        private const double fps = 60.0;

        private readonly CGColor lightColor = ColorAssets.CustomGray2.CGColor;
        private readonly CGColor darkColor = ColorAssets.CustomGray4.CGColor;

        private CGPoint start;
        private nfloat radius;

        private CAShapeLayer foreground;
        private CAShapeLayer background;
        private CAKeyFrameAnimation animation;
        private bool animationIsRunning;
        private object animationLock = new object();

        public override bool Hidden
        {
            get => base.Hidden;
            set
            {
                if (base.Hidden == true && value == false)
                {
                    startAnimation();
                }
                else if (base.Hidden == false && value == true)
                {
                    stopAnimation();
                }

                base.Hidden = value;
            }
        }

        public LoadingPieChartView(IntPtr handle) : base(handle)
        {
        }

        public override void LayoutSubviews()
        {
            base.LayoutSubviews();

            // change the layout only if the size of the view changes
            if (Center.X != radius)
            {
                radius = Center.X;
                start = new CGPoint(Center.X, Center.Y + radius);

                foreground = new CAShapeLayer();
                foreground.FillColor = darkColor;

                background = new CAShapeLayer();
                background.FillColor = lightColor;

                background.RemoveFromSuperLayer();
                foreground.RemoveFromSuperLayer();

                Layer.AddSublayer(background);
                Layer.AddSublayer(foreground);

                animation = createAnimation();
                startAnimation();
            }
        }

        private void startAnimation()
        {
            if (animation == null) return;

            lock (animationLock)
            {
                animationIsRunning = true;
                resetShapes();
                foreground.AddAnimation(animation, null);
            }
        }

        private void stopAnimation()
        {
            lock (animationLock)
            {
                animationIsRunning = false;
                foreground.RemoveAllAnimations();
            }
        }

        private CGPath createArc(nfloat percent)
        {
            var angle = startAngle + fullCircle * percent;
            var path = new UIBezierPath();
            path.MoveTo(Center);
            path.AddLineTo(start);
            path.AddArc(Center, radius, startAngle, angle, true);
            path.ClosePath();
            return path.CGPath;
        }

        private CAKeyFrameAnimation createAnimation()
        {
            var animation = CAKeyFrameAnimation.FromKeyPath("path");
            var frames = createKeyFrames();
            animation.SetValues(frames);
            animation.Duration = Timings.ReportsLoading;
            animation.TimingFunction = Curves.Linear.ToMediaTimingFunction();
            animation.CalculationMode = CAAnimation.AnimationLinear;
            animation.FillMode = CAFillMode.Forwards;
            animation.RemovedOnCompletion = false;
            animation.AnimationStopped += animationStopped;
            return animation;
        }

        private void animationStopped(object sender, EventArgs e)
        {
            lock (animationLock)
            {
                if (shouldRepeatAnimation == false) return;

                swapLayers();
                foreground.AddAnimation(animation, null);
            }
        }

        private void swapLayers()
        {
            (foreground, background) = (background, foreground);
            foreground.RemoveFromSuperLayer();
            Layer.AddSublayer(foreground);
        }

        private CGPath[] createKeyFrames()
        {
            int framesCount = (int)(Timings.ReportsLoading * fps);
            var frames = new CGPath[framesCount];
            for (int i = 0; i < framesCount; ++i)
            {
                nfloat percent = (nfloat)(i + 1) / framesCount;
                var path = createArc(percent);
                frames[i] = path;
            }

            return frames;
        }

        private void resetShapes()
        {
            foreground.Path = createArc(0f);
            background.Path = createArc(1f);
        }

        private bool shouldRepeatAnimation
            => animationIsRunning && Hidden == false && Window != null;
    }
}
