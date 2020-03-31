using CoreAnimation;
using Foundation;
using System;
using Toggl.iOS.Extensions;
using UIKit;
using static Toggl.Core.UI.Helper.Animation;
using static Toggl.Shared.Math;

namespace Toggl.iOS.Views
{
    [Register(nameof(ActivityIndicatorView))]
    public sealed class ActivityIndicatorView : UIImageView
    {
        private const float animationDuration = 1f;

        private string imageResource = "icLoader";

        public ActivityIndicatorView(UIColor color = null)
        {
            init(color);
        }

        public ActivityIndicatorView(IntPtr handle)
            : base(handle)
        {
        }

        public UIColor IndicatorColor
        {
            set
            {
                Image = Image.ImageWithRenderingMode(UIImageRenderingMode.AlwaysTemplate);
                TintColor = value;
            }
        }

        public override void AwakeFromNib()
        {
            init(null);
        }

        /// <summary>
        /// Starts the spinning animation of the activity indicator.
        /// NOTE: may not work when called from ViewDidLoad, use ViewWillAppear or ViewDidAppear.
        /// </summary>
        public void StartSpinning()
        {
            var animation = createAnimation();
            Layer.RemoveAllAnimations();
            Layer.AddAnimation(animation, "spinning");
        }

        public void StopSpinning()
        {
            Layer.RemoveAllAnimations();
        }

        private void init(UIColor color)
        {
            Image = UIImage.FromBundle(imageResource);
            ContentMode = UIViewContentMode.Center;
            if (color != null)
            {
                IndicatorColor = color;
            }
        }

        private CAAnimation createAnimation()
        {
            var animation = CABasicAnimation.FromKeyPath("transform.rotation.z");
            animation.Duration = animationDuration;
            animation.TimingFunction = Curves.Linear.ToMediaTimingFunction();
            animation.Cumulative = true;
            animation.From = NSNumber.FromNFloat(0);
            animation.To = NSNumber.FromNFloat((nfloat)FullCircle);
            animation.RepeatCount = float.PositiveInfinity;
            return animation;
        }
    }
}
