using CoreGraphics;
using System;
using System.Reactive.Disposables;
using UIKit;

namespace Toggl.iOS.Views
{
    public abstract class ReactiveCollectionViewCell<TViewModel> : UICollectionViewCell
    {
        public CompositeDisposable DisposeBag { get; private set; } = new CompositeDisposable();

        private TViewModel item;
        public TViewModel Item
        {
            get => item;
            set
            {
                item = value;
                UpdateView();
            }
        }

        protected internal ReactiveCollectionViewCell(IntPtr handle) : base(handle)
        {
        }

        public ReactiveCollectionViewCell(CGRect frame) : base(frame)
        {
        }

        protected abstract void UpdateView();

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (!disposing) return;
            DisposeBag?.Dispose();
        }
    }
}
