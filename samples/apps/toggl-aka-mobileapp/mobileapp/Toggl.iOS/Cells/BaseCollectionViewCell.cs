using System;
using UIKit;

namespace Toggl.iOS.Cells
{
    public abstract class BaseCollectionViewCell<T> : UICollectionViewCell
    {
        private T item;
        public T Item
        {
            get => item;
            set
            {
                item = value;
                UpdateView();
            }
        }

        protected BaseCollectionViewCell()
        {
        }

        protected BaseCollectionViewCell(IntPtr handle)
            : base(handle)
        {
        }

        protected abstract void UpdateView();
    }
}
