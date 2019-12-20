using Foundation;
using System;
using System.Collections.Immutable;
using UIKit;

namespace Toggl.iOS.ViewSources
{
    public class ReloadCollectionViewSource<TModel> : UICollectionViewSource
    {
        public delegate UICollectionViewCell CellConfiguration(ReloadCollectionViewSource<TModel> source, UICollectionView tableView, NSIndexPath indexPath, TModel model);

        private readonly CellConfiguration configureCell;

        internal IImmutableList<TModel> items;

        public EventHandler<TModel> OnItemTapped { get; set; }

        public ReloadCollectionViewSource(IImmutableList<TModel> items, CellConfiguration configureCell)
        {
            this.items = items;
            this.configureCell = configureCell;
        }

        public override UICollectionViewCell GetCell(UICollectionView collectionView, NSIndexPath indexPath)
        {
            return configureCell(this, collectionView, indexPath, items[indexPath.Row]);
        }

        public override void ItemSelected(UICollectionView collectionView, NSIndexPath indexPath)
        {
            collectionView.DeselectItem(indexPath, true);
            OnItemTapped.Invoke(this, items[indexPath.Row]);
        }

        public override nint GetItemsCount(UICollectionView collectionView, nint section)
            => items.Count;
    }
}
