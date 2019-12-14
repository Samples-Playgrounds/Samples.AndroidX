using System;
using System.Collections.Immutable;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using CoreGraphics;
using Foundation;
using Toggl.Core.UI.ViewModels.Calendar;
using Toggl.iOS.Cells.Calendar;
using Toggl.Shared;
using Toggl.Shared.Extensions;
using UIKit;
using Math = System.Math;

namespace Toggl.iOS.ViewSources
{
    public sealed class CalendarWeeklyViewDayCollectionViewSource : UICollectionViewSource
    {
        private readonly UICollectionView collectionView;
        private readonly ISubject<CalendarWeeklyViewDayViewModel> daySelectedSubject = new Subject<CalendarWeeklyViewDayViewModel>();

        private int currentPage;
        private DateTime currentlySelectedDate;

        private IImmutableList<CalendarWeeklyViewDayViewModel> weekDays =
            ImmutableList<CalendarWeeklyViewDayViewModel>.Empty;

        private nfloat pageWidth
            => ((UICollectionViewFlowLayout) collectionView.CollectionViewLayout).ItemSize.Width * 7;

        private int pageCount => weekDays.Count / 7;

        public IObservable<CalendarWeeklyViewDayViewModel> DaySelected { get; }

        public CalendarWeeklyViewDayCollectionViewSource(UICollectionView collectionView)
        {
            Ensure.Argument.IsNotNull(collectionView, nameof(collectionView));

            this.collectionView = collectionView;

            DaySelected = daySelectedSubject.AsObservable();

            collectionView.RegisterNibForCell(CalendarWeeklyViewDayCollectionViewCell.Nib, CalendarWeeklyViewDayCollectionViewCell.Key);
        }

        public override UICollectionViewCell GetCell(UICollectionView collectionView, NSIndexPath indexPath)
        {
            var day = weekDays[indexPath.Row];
            var cell = (CalendarWeeklyViewDayCollectionViewCell)collectionView.DequeueReusableCell(CalendarWeeklyViewDayCollectionViewCell.Key, indexPath);
            cell.UpdateSelectedDate(currentlySelectedDate);
            cell.Item = day;
            return cell;
        }

        public override void ItemSelected(UICollectionView collectionView, NSIndexPath indexPath)
        {
            collectionView.DeselectItem(indexPath, true);
            var item = weekDays[indexPath.Row];
            if (!item.Enabled)
                return;

            daySelectedSubject.OnNext(item);
            currentlySelectedDate = item.Date;
        }

        public override void DraggingStarted(UIScrollView scrollView)
        {
            currentPage = (int) Math.Floor((collectionView.ContentOffset.X - pageWidth / 2) / pageWidth) + 1;
        }

        public override void WillEndDragging(UIScrollView scrollView, CGPoint velocity, ref CGPoint targetContentOffset)
        {
            var newPage = currentPage;

            //Slow drag & release
            if (velocity.X == 0)
            {
                newPage = (int) Math.Floor((targetContentOffset.X - pageWidth / 2) / pageWidth) + 1;
            }
            //Swipe
            else
            {
                newPage = velocity.X > 0 ? currentPage + 1 : currentPage - 1;

                if (newPage < 0)
                    newPage = 0;
                if (newPage > collectionView.ContentSize.Width / pageWidth)
                    newPage = (int) (Math.Ceiling(collectionView.ContentSize.Width / pageWidth) - 1.0);
            }

            if (newPage >= pageCount)
                return;

            var goingForward = newPage > currentPage;
            currentPage = newPage;

            var modOfCurrentlySelectedItem = weekDays.IndexOf(d => d.Date == currentlySelectedDate) % 7;

            var newSelectedItemIndex = 7 * currentPage + modOfCurrentlySelectedItem;
            var newSelectedItem = weekDays[newSelectedItemIndex];
            while (!newSelectedItem.Enabled)
            {
                newSelectedItemIndex = goingForward ? newSelectedItemIndex - 1 : newSelectedItemIndex + 1;
                newSelectedItem = weekDays[newSelectedItemIndex];
            }

            daySelectedSubject.OnNext(newSelectedItem);

            targetContentOffset = contentOffsetFor(newPage);
        }

        public override CGPoint GetTargetContentOffset(UICollectionView collectionView, CGPoint proposedContentOffset)
            => contentOffsetFor(currentPage);

        private CGPoint contentOffsetFor(int page)
            => new CGPoint(page * pageWidth, 0);

        public override nint NumberOfSections(UICollectionView collectionView) => 1;

        public override nint GetItemsCount(UICollectionView collectionView, nint section)
            => weekDays.Count;

        public void UpdateItems(IImmutableList<CalendarWeeklyViewDayViewModel> weekDays)
        {
            if (weekDays.Count % 7 != 0)
                throw new ArgumentException($"{nameof(weekDays)} must have a number of days that is divisible by 7");

            this.weekDays = weekDays;
            collectionView.ReloadData();
            var page = pageFor(currentlySelectedDate);
            if (page != null)
                setCurrentPage(page.Value, false);
        }

        private void setCurrentPage(int page, bool animated)
        {
            var contentOffset = contentOffsetFor(page);
            collectionView.SetContentOffset(contentOffset, animated);
            currentPage = page;
        }

        public void UpdateCurrentlySelectedDate(DateTime date)
        {
            currentlySelectedDate = date;
            collectionView.CollectionViewLayout.InvalidateLayout();
            collectionView.ReloadData();
            var page = pageFor(date);
            if (page != null)
                setCurrentPage(page.Value, true);
        }

        private int? pageFor(DateTime date)
        {
            var indexOfDayViewModel = weekDays.IndexOf(vm => vm.Date == date);
            if (indexOfDayViewModel == -1)
                return null;
            return indexOfDayViewModel / 7;
        }
    }
}
