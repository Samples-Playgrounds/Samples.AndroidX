using Android.App;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Toggl.Core.UI.Navigation;
using Toggl.Core.UI.ViewModels;
using Toggl.Core.UI.Views;

namespace Toggl.Droid.Presentation
{
    public abstract class AndroidPresenter : IPresenter
    {
        protected abstract HashSet<Type> AcceptedViewModels { get; }

        protected abstract void PresentOnMainThread<TInput, TOutput>(ViewModel<TInput, TOutput> viewModel, IView sourceView);

        public virtual bool CanPresent<TInput, TOutput>(ViewModel<TInput, TOutput> viewModel)
            => AcceptedViewModels.Contains(viewModel.GetType());

        public Task Present<TInput, TOutput>(ViewModel<TInput, TOutput> viewModel, IView sourceView)
        {
            var tcs = new TaskCompletionSource<object>();
            Application.SynchronizationContext.Post(_ =>
            {
                PresentOnMainThread(viewModel, sourceView);
                tcs.SetResult(true);
            }, null);

            return tcs.Task;
        }

        public virtual bool ChangePresentation(IPresentationChange presentationChange)
            => false;
    }
}
