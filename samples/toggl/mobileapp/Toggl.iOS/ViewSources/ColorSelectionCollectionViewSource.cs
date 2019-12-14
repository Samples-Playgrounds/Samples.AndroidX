using CoreGraphics;
using Foundation;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Reactive.Linq;
using Toggl.Core.UI.ViewModels;
using Toggl.iOS.Views;
using Toggl.Shared;
using UIKit;
using static System.Math;

namespace Toggl.iOS.ViewSources
{
    public sealed class ColorSelectionCollectionViewSource : ReloadCollectionViewSource<SelectableColorViewModel>, IUICollectionViewDelegateFlowLayout
    {
        public IObservable<Color> ColorSelected
            => Observable
                .FromEventPattern<SelectableColorViewModel>(e => OnItemTapped += e, e => OnItemTapped -= e)
                .Select(e => e.EventArgs.Color);

        public ColorSelectionCollectionViewSource(IObservable<IEnumerable<SelectableColorViewModel>> colors)
            : base(ImmutableList<SelectableColorViewModel>.Empty, configureCell)
        {
        }

        public void SetNewColors(IEnumerable<SelectableColorViewModel> colors)
        {
            items = colors.ToImmutableList();
        }

        [Export("collectionView:layout:sizeForItemAtIndexPath:")]
        public CGSize GetSizeForItem(
            UICollectionView collectionView, UICollectionViewLayout layout, NSIndexPath indexPath)
            => new CGSize(Floor(collectionView.Frame.Width / 5), 36);

        [Export("collectionView:layout:minimumLineSpacingForSectionAtIndex:")]
        public nfloat GetMinimumLineSpacingForSection(
            UICollectionView collectionView, UICollectionViewLayout layout, nint section)
            => 12;

        [Export("collectionView:layout:minimumInteritemSpacingForSectionAtIndex:")]
        public nfloat GetMinimumInteritemSpacingForSection(
            UICollectionView collectionView, UICollectionViewLayout layout, nint section)
            => 0;

        [Export("collectionView:layout:insetForSectionAtIndex:")]
        public UIEdgeInsets GetInsetForSection(
            UICollectionView collectionView, UICollectionViewLayout layout, nint section)
            => new UIEdgeInsets(0, 0, 0, 0);

        private static UICollectionViewCell configureCell(ReloadCollectionViewSource<SelectableColorViewModel> source,
            UICollectionView collectionView, NSIndexPath indexPath, SelectableColorViewModel colorViewModel)
        {
            var cell = collectionView.DequeueReusableCell(ColorSelectionViewCell.Identifier, indexPath) as ColorSelectionViewCell;
            cell.Item = colorViewModel;
            return cell;
        }
    }
}
