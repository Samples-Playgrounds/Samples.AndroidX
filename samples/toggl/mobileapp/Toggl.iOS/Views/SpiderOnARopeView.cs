using CoreGraphics;
using CoreMotion;
using Foundation;
using System;
using System.Collections.Generic;
using System.Linq;
using Toggl.Core.UI.Helper;
using Toggl.iOS.Extensions;
using UIKit;

namespace Toggl.iOS.Views
{
    [Register(nameof(SpiderOnARopeView))]
    public class SpiderOnARopeView : UIView
    {
        private const int chainLength = 8;
        private const double chainWidth = 2;
        private const float spiderResistance = 0.85f;
        private readonly CGColor ropeColor = ColorAssets.Spider.CGColor;

        private double spiderRadius;

        private UIDynamicAnimator spiderAnimator;
        private UIGravityBehavior gravity;
        private UISnapBehavior dragging;
        private CMMotionManager motionManager;
        private UIAttachmentBehavior spiderAttachment;

        private UIImage spiderImage;
        private UIView spiderView;
        private UIView[] links;
        private CGPoint anchorPoint;

        public bool IsVisible { get; private set; }

        public SpiderOnARopeView()
        {
            init();
        }

        public SpiderOnARopeView(IntPtr handle) : base(handle)
        {
        }

        public override void AwakeFromNib()
        {
            base.AwakeFromNib();
            init();
        }

        private void init()
        {
            spiderImage = UIImage.FromBundle("icJustSpider");
            BackgroundColor = UIColor.Clear;
        }

        public override void LayoutSubviews()
        {
            base.LayoutSubviews();

            if (IsVisible && anchorPoint.X != Center.X)
            {
                Show();
            }
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            reset();
        }

        public override void Draw(CGRect rect)
        {
            base.Draw(rect);

            var ctx = UIGraphics.GetCurrentContext();
            if (ctx == null) return;

            if (links != null && IsVisible == true)
            {
                var points = links.Select(links => links.Center).ToArray();
                var path = createCurvedPath(anchorPoint, points);
                ctx.SetStrokeColor(ropeColor);
                ctx.SetLineWidth((nfloat)chainWidth);
                ctx.AddPath(path);
                ctx.DrawPath(CGPathDrawingMode.Stroke);
            }
        }

        public void Show()
        {
            reset();
            anchorPoint = new CGPoint(Center.X, 0);

            spiderView = new UIImageView(spiderImage);
            AddSubview(spiderView);

            preparePhysics();

            IsVisible = true;
        }

        public void Hide()
        {
            reset();
        }

        public override void TouchesBegan(NSSet touches, UIEvent evt)
        {
            base.TouchesBegan(touches, evt);
            snapTo(touches);
        }

        public override void TouchesMoved(NSSet touches, UIEvent evt)
        {
            base.TouchesMoved(touches, evt);
            snapTo(touches);
        }

        public override void TouchesEnded(NSSet touches, UIEvent evt)
        {
            base.TouchesEnded(touches, evt);
            releaseSnap();
        }

        public override void TouchesCancelled(NSSet touches, UIEvent evt)
        {
            base.TouchesCancelled(touches, evt);
            releaseSnap();
        }

        private void snapTo(NSSet touches)
        {
            if (spiderView == null) return;

            var touch = (UITouch)touches.First();

            lock (spiderAnimator)
            {
                if (dragging != null)
                {
                    spiderAnimator.RemoveBehavior(dragging);
                }

                var position = touch.LocationInView(this);
                dragging = new UISnapBehavior(spiderView, position);

                spiderAnimator.RemoveBehavior(gravity);
                spiderAnimator.AddBehavior(dragging);
            }
        }

        private void releaseSnap()
        {
            lock (spiderAnimator)
            {
                spiderAnimator.RemoveBehavior(dragging);
                spiderAnimator.AddBehavior(gravity);

                dragging = null;
            }
        }

        private void reset()
        {
            foreach (var subview in Subviews)
            {
                subview.RemoveFromSuperview();
            }

            motionManager?.Dispose();
            gravity?.Dispose();
            spiderAnimator?.Dispose();
            spiderView?.Dispose();

            spiderAnimator = null;
            motionManager = null;
            gravity = null;
            spiderView = null;

            IsVisible = false;
        }

        private void preparePhysics()
        {
            spiderRadius = Math.Sqrt(Math.Pow(spiderImage.Size.Width / 2, 2) + Math.Pow(spiderImage.Size.Height / 2, 2));
            double height = (UIScreen.MainScreen.ApplicationFrame.Size.Width / 2) - 1.5 * spiderRadius;

            spiderAnimator = new UIDynamicAnimator(this);

            var spider = new UIDynamicItemBehavior(spiderView);
            spider.Action = () => SetNeedsDisplay();
            spider.Resistance = spiderResistance;
            spider.AllowsRotation = true;
            spiderAnimator.AddBehavior(spider);

            links = createRope(height);

            gravity = new UIGravityBehavior(links);
            spiderAnimator.AddBehavior(gravity);

            motionManager?.Dispose();
            motionManager = new CMMotionManager();
            motionManager.StartAccelerometerUpdates(NSOperationQueue.CurrentQueue, processAccelerometerData);
        }

        private UIView[] createRope(double length)
        {
            double chainLinkHeight = length / chainLength;
            var chain = new List<UIView>();
            UIView lastLink = null;

            for (int i = 0; i < chainLength; i++)
            {
                var chainLink = createChainLink(i, chainLinkHeight, lastLink);
                chain.Add(chainLink);
                lastLink = chainLink;   
            }

            spiderView.Center = new CGPoint(Center.X, -length + spiderImage.Size.Height / 2);
            spiderAttachment = new UIAttachmentBehavior(lastLink, UIOffset.Zero, spiderView, new UIOffset(0, -spiderImage.Size.Height / 2));
            spiderAttachment.Length = 0;
            spiderAnimator.AddBehavior(spiderAttachment);

            chain.Add(spiderView);

            return chain.ToArray();
        }

        private UIView createChainLink(int n, double chainLinkHeight, UIView lastLink)
        {
            double y = -n * chainLinkHeight;

            var chainLink = new UIView();
            chainLink.BackgroundColor = UIColor.Clear;
            chainLink.Frame = new CGRect(Center.X, y, chainWidth, chainLinkHeight);

            AddSubview(chainLink);

            var chainDynamics = new UIDynamicItemBehavior(chainLink);
            spiderAnimator.AddBehavior(chainDynamics);

            var attachment = lastLink == null
                ? new UIAttachmentBehavior(chainLink, anchorPoint)
                : new UIAttachmentBehavior(chainLink, lastLink);
            attachment.Length = (nfloat)chainLinkHeight;

            spiderAnimator.AddBehavior(attachment);

            return chainLink;
        }

        private CGPath createCurvedPath(CGPoint anchor, CGPoint[] points)
        {
            var path = new UIBezierPath();

            if (points.Length > 1)
            {
                var previousPoint = points[0];
                var startOfCurve = previousPoint;
                path.MoveTo(anchor);

                for (int i = 1; i < points.Length; i++)
                {
                    var endOfCurve = points[i];
                    var nextPoint = i < points.Length - 1 ? points[i + 1] : points[i];
                    var (controlPointB, controlPointC) = calculateControlPoints(previousPoint, startOfCurve, endOfCurve, nextPoint);
                    path.AddCurveToPoint(endOfCurve, controlPointB, controlPointC);

                    previousPoint = startOfCurve;
                    startOfCurve = endOfCurve;
                }
            }

            return path.CGPath;
        }

        private (double, double) convertXYCoordinate(double originX, double originY,
            UIInterfaceOrientation orientation)
        {
            switch (orientation)
            {
                case UIInterfaceOrientation.LandscapeLeft:
                    return (originY, -originX);
                case UIInterfaceOrientation.LandscapeRight:
                    return (-originY, originX);
                case UIInterfaceOrientation.PortraitUpsideDown:
                    return (-originX, -originY);
                default:
                    return (originX, originY);
            }
        }
        private void processAccelerometerData(CMAccelerometerData data, NSError error)
        {
            if (spiderView == null) return;

            var (ax, ay) = convertXYCoordinate(data.Acceleration.X, data.Acceleration.Y,
                UIApplication.SharedApplication.StatusBarOrientation);

            var angle = -(nfloat)Math.Atan2(ay, ax);
            gravity.Angle = angle;
        }

        // Catmull-Rom to Cubic Bezier conversion matrix:
        // |   0       1       0       0  |
        // | -1/6      1      1/6      0  |
        // |   0      1/6      1     -1/6 |
        // |   0       0       1       0  |
        private (CGPoint, CGPoint) calculateControlPoints(CGPoint a, CGPoint b, CGPoint c, CGPoint d)
            => (new CGPoint((-1.0 / 6.0 * a.X) + b.X + (1.0 / 6.0 * c.X), (-1.0 / 6.0 * a.Y) + b.Y + (1.0 / 6.0 * c.Y)),
                new CGPoint((1.0 / 6.0 * b.X) + c.X + (-1.0 / 6.0 * d.X), (1.0 / 6.0 * b.Y) + c.Y + (-1.0 / 6.0 * d.Y)));
    }
}
