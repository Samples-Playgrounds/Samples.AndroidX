using System;
using System.Reactive.Disposables;
using System.Threading.Tasks;
using CoreFoundation;
using Toggl.Core.UI.ViewModels;
using Toggl.Core.UI.Views;
using Toggl.iOS.Extensions;
using UIKit;

namespace Toggl.iOS.ViewControllers
{
    public abstract partial class ReactiveTableViewController<TViewModel> : UITableViewController, IReactiveViewController, IView
        where TViewModel : IViewModel
    {
        public CompositeDisposable DisposeBag { get; private set; } = new CompositeDisposable();

        public TViewModel ViewModel { get; }

        protected ReactiveTableViewController(TViewModel viewModel) : base(null, null)
        {
            ViewModel = viewModel;
        }

        protected ReactiveTableViewController(IntPtr handle)
            : base(handle) { }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            ViewModel?.AttachView(this);
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
            ViewModel?.ViewAppearing();
        }

        public override void ViewDidAppear(bool animated)
        {
            base.ViewDidAppear(animated);
            ViewModel?.ViewAppeared();
        }

        public override void ViewWillDisappear(bool animated)
        {
            base.ViewWillDisappear(animated);
            ViewModel?.ViewDisappearing();
        }

        public override void ViewDidDisappear(bool animated)
        {
            base.ViewDidDisappear(animated);
            ViewModel?.ViewDisappeared();
        }

        public override void DidMoveToParentViewController(UIViewController parent)
        {
            base.DidMoveToParentViewController(parent);

            if (parent == null)
            {
                ViewModel?.DetachView();
                ViewModel?.ViewDestroyed();
            }
        }

        public override void DismissViewController(bool animated, Action completionHandler)
        {
            base.DismissViewController(animated, completionHandler);

            ViewModel?.DetachView();
            ViewModel?.ViewDestroyed();
        }

        public Task<bool> DismissFromNavigationController()
            => ViewModel.CloseWithDefaultResult();

        public void ViewcontrollerWasPopped()
        {
            ViewModel.ViewWasClosed();
        }

        public void Close()
        {
            DispatchQueue.MainQueue.DispatchAsync(this.Dismiss);
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (!disposing) return;
            DisposeBag?.Dispose();
        }

        public IObservable<string> GetGoogleToken()
            => throw new InvalidOperationException();
    }
}
