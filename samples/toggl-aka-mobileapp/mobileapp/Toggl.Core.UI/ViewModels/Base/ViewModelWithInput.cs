using System.Reactive;
using System.Threading.Tasks;
using Toggl.Core.UI.Navigation;

namespace Toggl.Core.UI.ViewModels
{
    public abstract class ViewModelWithInput<TInput> : ViewModel<TInput, Unit>
    {
        protected ViewModelWithInput(INavigationService navigationService) : base(navigationService)
        {
        }

        public virtual void Close()
        {
            base.Close(Unit.Default);
        }

        public sealed override void Close(Unit output)
            => Close();
    }
}
