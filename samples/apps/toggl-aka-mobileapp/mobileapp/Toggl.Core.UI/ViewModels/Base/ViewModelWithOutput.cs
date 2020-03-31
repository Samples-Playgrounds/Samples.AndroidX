using System.Reactive;
using System.Threading.Tasks;
using Toggl.Core.UI.Navigation;

namespace Toggl.Core.UI.ViewModels
{
    public abstract class ViewModelWithOutput<TOutput> : ViewModel<Unit, TOutput>
    {
        protected ViewModelWithOutput(INavigationService navigationService) : base(navigationService)
        {
        }

        public virtual Task Initialize()
            => Task.CompletedTask;

        public sealed override Task Initialize(Unit payload)
            => Initialize();
    }
}
