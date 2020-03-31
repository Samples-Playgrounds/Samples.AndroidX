using CoreGraphics;
using System;
using UIKit;

namespace Toggl.iOS.Cells
{
    public abstract class BaseReportsCardView<T> : UIView
    {
        private T item;
        public T Item
        {
            get => item;
            set
            {
                item = value;
                UpdateViewBinding();
            }
        }

        protected BaseReportsCardView()
        {
        }

        protected BaseReportsCardView(IntPtr handle)
            : base(handle)
        {
        }

        protected abstract void UpdateViewBinding();

        protected void PrepareCard(UIView view)
        {
            view.Layer.CornerRadius = 8;
            view.Layer.ShadowColor = UIColor.Black.CGColor;
            view.Layer.ShadowRadius = 8;
            view.Layer.ShadowOffset = new CGSize(0, 2);
            view.Layer.ShadowOpacity = 0.1f;
        }
    }
}
