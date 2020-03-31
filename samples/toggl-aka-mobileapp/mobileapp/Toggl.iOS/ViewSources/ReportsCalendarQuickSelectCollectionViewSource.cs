using CoreGraphics;
using Foundation;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using Toggl.Core.UI.Parameters;
using Toggl.Core.UI.ViewModels.ReportsCalendar.QuickSelectShortcuts;
using Toggl.iOS.Views.Reports;
using Toggl.Shared;
using UIKit;

namespace Toggl.iOS.ViewSources
{
    public sealed class ReportsCalendarQuickSelectCollectionViewSource
        : UICollectionViewSource, IUICollectionViewDelegateFlowLayout
    {
        private const int cellWidth = 96;
        private const int cellHeight = 32;
        private const string cellIdentifier = nameof(ReportsCalendarQuickSelectViewCell);

        private ISubject<ReportsCalendarBaseQuickSelectShortcut> shortcutTaps = new Subject<ReportsCalendarBaseQuickSelectShortcut>();
        private readonly UICollectionView collectionView;
        private ReportsDateRangeParameter currentDateRange;
        private IImmutableList<ReportsCalendarBaseQuickSelectShortcut> shortcuts = ImmutableList<ReportsCalendarBaseQuickSelectShortcut>.Empty;

        public IObservable<ReportsCalendarBaseQuickSelectShortcut> ShortcutTaps { get; }

        public ReportsCalendarQuickSelectCollectionViewSource(UICollectionView collectionView)
        {
            Ensure.Argument.IsNotNull(collectionView, nameof(collectionView));
            collectionView.RegisterNibForCell(ReportsCalendarQuickSelectViewCell.Nib, cellIdentifier);
            ShortcutTaps = shortcutTaps.AsObservable();
            this.collectionView = collectionView;
        }

        public override UICollectionViewCell GetCell(UICollectionView collectionView, NSIndexPath indexPath)
        {
            var item = shortcuts[indexPath.Row] as ReportsCalendarBaseQuickSelectShortcut;
            var cell = collectionView.DequeueReusableCell(cellIdentifier, indexPath) as ReportsCalendarQuickSelectViewCell;

            cell.UpdateSelectedDateRange(currentDateRange);
            cell.Item = item;

            return cell;
        }

        public override nint NumberOfSections(UICollectionView collectionView) => 1;

        public override nint GetItemsCount(UICollectionView collectionView, nint section) => shortcuts.Count;

        [Export("collectionView:layout:sizeForItemAtIndexPath:")]
        public CGSize GetSizeForItem(UICollectionView collectionView, UICollectionViewLayout layout, NSIndexPath indexPath)
        {
            return new CGSize(cellWidth, cellHeight);
        }

        public override void ItemSelected(UICollectionView collectionView, NSIndexPath indexPath)
        {
            shortcutTaps.OnNext(shortcuts[indexPath.Row]);
        }

        public void UpdateShortcuts(IImmutableList<ReportsCalendarBaseQuickSelectShortcut> newShortcuts)
        {
            shortcuts = newShortcuts;
            collectionView.ReloadData();
        }

        public void UpdateSelection(ReportsDateRangeParameter dateRange)
        {
            currentDateRange = dateRange;
            collectionView.ReloadData();
        }
    }
}
