using System;
using UIKit;

namespace Toggl.iOS.Cells
{
    public abstract class BaseTableHeaderFooterView<T> : UITableViewHeaderFooterView
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

        protected BaseTableHeaderFooterView()
        {
        }

        protected BaseTableHeaderFooterView(IntPtr handle)
            : base(handle)
        {
        }

        protected abstract void UpdateView();
    }
}
