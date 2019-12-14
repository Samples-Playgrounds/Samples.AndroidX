using System;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using Foundation;
using Toggl.iOS.ViewSources;
using Toggl.Shared;
using UIKit;

namespace Toggl.iOS.Views.Calendar
{
    public sealed class CalendarCollectionViewContextualMenuDismissHelper : NSObject, IUIGestureRecognizerDelegate
    {
        private readonly UITapGestureRecognizer tapGestureRecognizer;
        private readonly CalendarCollectionViewSource dataSource;
        private readonly UICollectionView collectionView;

        private readonly ISubject<Unit> didTapOnEmptySpaaceSubject = new Subject<Unit>();
        public IObservable<Unit> DidTapOnEmptySpace => didTapOnEmptySpaaceSubject.AsObservable();

        public CalendarCollectionViewContextualMenuDismissHelper(UICollectionView collectionView, CalendarCollectionViewSource dataSource)
        {
            Ensure.Argument.IsNotNull(collectionView, nameof(collectionView));
            Ensure.Argument.IsNotNull(dataSource, nameof(dataSource));

            this.collectionView = collectionView;
            this.dataSource = dataSource;

            tapGestureRecognizer = new UITapGestureRecognizer(onTap);
            tapGestureRecognizer.Delegate = this;
            collectionView.AddGestureRecognizer(tapGestureRecognizer);
        }

        [Export("gestureRecognizer:shouldReceiveTouch:")]
        public bool ShouldReceiveTouch(UIGestureRecognizer gestureRecognizer, UITouch touch)
        {
            if (gestureRecognizer == tapGestureRecognizer)
            {
                var point = touch.LocationInView(collectionView);
                var thereIsNoItemAtPoint = dataSource.CalendarItemAtPoint(point) == null;
                return thereIsNoItemAtPoint;
            }

            return false;
        }

        void onTap(UITapGestureRecognizer tap)
        {
            didTapOnEmptySpaaceSubject.OnNext(Unit.Default);
        }
    }
}
