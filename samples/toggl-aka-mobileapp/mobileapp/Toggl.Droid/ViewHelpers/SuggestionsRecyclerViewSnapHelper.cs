using Android.Views;
using System;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using AndroidX.RecyclerView.Widget;

namespace Toggl.Droid.ViewHelpers
{     public sealed class SuggestionsRecyclerViewSnapHelper : LinearSnapHelper
    {
        private OrientationHelper horizontalHelper;
        private readonly int startMargin;
        private readonly BehaviorSubject<int> subject = new BehaviorSubject<int>(1);

        public IObservable<int> CurrentIndexObservable
            => subject.AsObservable()
                .Where(index => index > 0)
                .DistinctUntilChanged();

        public SuggestionsRecyclerViewSnapHelper(int startMargin)
        {
            this.startMargin = startMargin;
        }

        public override View FindSnapView(RecyclerView.LayoutManager layoutManager)
            => getStartView(layoutManager, getHorizontalHelper(layoutManager));

        public override int[] CalculateDistanceToFinalSnap(RecyclerView.LayoutManager layoutManager, View targetView)
            => new[] { distanceToStart(targetView, getHorizontalHelper(layoutManager)), 0 };

        private int distanceToStart(View targetView, OrientationHelper helper)
            => helper.GetDecoratedStart(targetView) - helper.StartAfterPadding - startMargin;

        private OrientationHelper getHorizontalHelper(RecyclerView.LayoutManager layoutManager)
            => horizontalHelper ?? (horizontalHelper = OrientationHelper.CreateHorizontalHelper(layoutManager));

        private View getStartView(RecyclerView.LayoutManager layoutManager, OrientationHelper helper)
        {
            var linearLayoutManager = (LinearLayoutManager)layoutManager;

            var firstChildIndex = linearLayoutManager.FindFirstVisibleItemPosition();
            if (firstChildIndex == RecyclerView.NoPosition)
                return null;

            subject.OnNext(linearLayoutManager.FindFirstCompletelyVisibleItemPosition() + 1);
            var lastItemIndex = linearLayoutManager.ItemCount - 1;
            var isLastItem = linearLayoutManager.FindLastCompletelyVisibleItemPosition() == lastItemIndex;
            if (isLastItem)
                return null;

            var firstView = layoutManager.FindViewByPosition(firstChildIndex);
            var decoratedEnd = helper.GetDecoratedEnd(firstView);
            var threshold = helper.GetDecoratedMeasurement(firstView) / 2;
            if (decoratedEnd >= threshold && decoratedEnd > 0)
                return firstView;

            return layoutManager.FindViewByPosition(firstChildIndex + 1);
        }
    }
}
