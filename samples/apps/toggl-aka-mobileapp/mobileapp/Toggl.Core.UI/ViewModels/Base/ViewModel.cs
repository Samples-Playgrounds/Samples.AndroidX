using System.Reactive;
using System.Threading.Tasks;
using Toggl.Core.UI.Navigation;
using Toggl.Core.UI.Views;
using Toggl.Shared;

namespace Toggl.Core.UI.ViewModels
{
    public abstract class ViewModel<TInput, TOutput> : IViewModel
    {
        protected INavigationService NavigationService { get; }

        private readonly TaskCompletionSource<TOutput> resultCompletionSource =
            new TaskCompletionSource<TOutput>();

        public IView View { get; private set; }

        public Task<TOutput> Result => resultCompletionSource.Task;

        protected ViewModel(INavigationService navigationService)
        {
            Ensure.Argument.IsNotNull(navigationService, nameof(navigationService));

            this.NavigationService = navigationService;
        }

        public virtual Task Initialize(TInput payload)
            => Task.CompletedTask;

        public virtual Task<bool> ConfirmCloseRequest()
            => Task.FromResult(true);

        public virtual async Task<bool> CloseWithDefaultResult()
        {
            var shouldClose = await ConfirmCloseRequest();
            if (!shouldClose)
                return false;

            Close(default);
            return true;
        }

        public virtual void Close(TOutput output)
        {
            View?.Close();
            resultCompletionSource.TrySetResult(output);
        }

        public void AttachView(IView viewToAttach)
        {
            View = viewToAttach;
        }

        public void DetachView()
        {
            View = null;
        }

        public virtual void ViewAppeared()
        {
        }

        public virtual void ViewAppearing()
        {
        }

        public virtual void ViewDisappearing()
        {
        }

        public virtual void ViewDisappeared()
        {
        }

        public virtual void ViewDestroyed()
        {
        }

        public void ViewWasClosed()
        {
            resultCompletionSource.TrySetResult(default);
            View = null;
        }

        public Task<TNavigationOutput> Navigate<TViewModel, TNavigationInput, TNavigationOutput>(TNavigationInput payload)
            where TViewModel : ViewModel<TNavigationInput, TNavigationOutput>
            => NavigationService.Navigate<TViewModel, TNavigationInput, TNavigationOutput>(payload, View);

        public Task Navigate<TViewModel>()
            where TViewModel : ViewModel<Unit, Unit>
            => Navigate<TViewModel, Unit, Unit>(Unit.Default);

        public Task<TNavigationOutput> Navigate<TViewModel, TNavigationOutput>()
            where TViewModel : ViewModel<Unit, TNavigationOutput>
            => Navigate<TViewModel, Unit, TNavigationOutput>(Unit.Default);

        public Task Navigate<TViewModel, TNavigationInput>(TNavigationInput payload)
            where TViewModel : ViewModel<TNavigationInput, Unit>
            => Navigate<TViewModel, TNavigationInput, Unit>(payload);
    }

    public abstract class ViewModel : ViewModel<Unit, Unit>
    {
        protected ViewModel(INavigationService navigationService) : base(navigationService)
        {
        }

        public virtual void Close()
        {
            base.Close(Unit.Default);
        }

        public virtual Task Initialize()
            => Task.CompletedTask;

        public sealed override Task Initialize(Unit payload)
            => Initialize();

        public sealed override void Close(Unit output)
        {
            Close();
        }
    }
}
