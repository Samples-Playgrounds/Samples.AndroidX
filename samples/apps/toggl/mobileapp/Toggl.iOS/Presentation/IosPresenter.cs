using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CoreFoundation;
using Toggl.Core.UI.Navigation;
using Toggl.Core.UI.ViewModels;
using Toggl.Core.UI.Views;
using UIKit;

namespace Toggl.iOS.Presentation
{
    public abstract class IosPresenter : IPresenter
    {
        protected UIWindow Window { get; }
        protected AppDelegate AppDelegate { get; }

        protected abstract HashSet<Type> AcceptedViewModels { get; }

        protected abstract void PresentOnMainThread<TInput, TOutput>(ViewModel<TInput, TOutput> viewModel, IView sourceView);

        public IosPresenter(UIWindow window, AppDelegate appDelegate)
        {
            Window = window;
            AppDelegate = appDelegate;
        }

        public virtual bool CanPresent<TInput, TOutput>(ViewModel<TInput, TOutput> viewModel)
            => AcceptedViewModels.Contains(viewModel.GetType());

        public Task Present<TInput, TOutput>(ViewModel<TInput, TOutput> viewModel, IView sourceView)
        {
            var tcs = new TaskCompletionSource<object>();
            performOnMainQueue(() =>
            {
                PresentOnMainThread(viewModel, sourceView);
                tcs.SetResult(true);
            });

            return tcs.Task;
        }

        public virtual bool ChangePresentation(IPresentationChange presentationChange)
            => false;

        protected UIViewController FindPresentedViewController()
            => findPresentedViewController(Window.RootViewController);

        private UIViewController findPresentedViewController(UIViewController current)
            => current.PresentedViewController == null || current.PresentedViewController.IsBeingDismissed
                ? current
                : findPresentedViewController(current.PresentedViewController);

        private void performOnMainQueue(Action action)
        {
            if (DispatchQueue.CurrentQueue == DispatchQueue.MainQueue)
            {
                action();
            }
            else
            {
                DispatchQueue.MainQueue.DispatchAsync(action);
            }
        }
    }
}
