﻿using Foundation;
using Toggl.Shared;
using UIKit;

namespace Toggl.iOS.Views.Calendar
{
    public sealed class CalendarCollectionViewZoomHelper : NSObject, IUIGestureRecognizerDelegate
    {
        private UIPinchGestureRecognizer pinchGestureRecognizer;
        private CalendarCollectionViewLayout layout;

        public CalendarCollectionViewZoomHelper(UICollectionView collectionView, CalendarCollectionViewLayout layout)
        {
            Ensure.Argument.IsNotNull(collectionView, nameof(collectionView));
            Ensure.Argument.IsNotNull(layout, nameof(layout));
            this.layout = layout;

            pinchGestureRecognizer = new UIPinchGestureRecognizer(onPinchUpdated);
            pinchGestureRecognizer.Delegate = this;
            collectionView.AddGestureRecognizer(pinchGestureRecognizer);
        }

        void onPinchUpdated(UIPinchGestureRecognizer gesture)
        {
            var pinchCenter = gesture.LocationInView(gesture.View);

            switch (gesture.State)
            {
                case UIGestureRecognizerState.Began:
                    layout.ScaleHourHeight(gesture.Scale, pinchCenter);
                    break;

                case UIGestureRecognizerState.Changed:
                    layout.ScaleHourHeight(gesture.Scale, pinchCenter);
                    gesture.Scale = 1;
                    break;

                case UIGestureRecognizerState.Ended:
                    layout.ScaleHourHeight(gesture.Scale, pinchCenter);
                    layout.OnScalingEnded();
                    break;

                case UIGestureRecognizerState.Cancelled:
                case UIGestureRecognizerState.Failed:
                    break;
            }
        }
    }
}
