using System.Reactive;
using System.Threading.Tasks;
using Toggl.Core.UI.ViewModels;
using Toggl.Core.UI.Views;

namespace Toggl.Core.UI.Navigation
{
    public interface INavigationService
    {
        Task<TOutput> Navigate<TViewModel, TInput, TOutput>(TInput payload, IView sourceView)
               where TViewModel : ViewModel<TInput, TOutput>;
    }

    public static class NavigationServiceExtensions
    {
        public static Task Navigate<TViewModel>(this INavigationService navigationService, IView sourceView)
            where TViewModel : ViewModel<Unit, Unit>
            => navigationService.Navigate<TViewModel, Unit, Unit>(Unit.Default, sourceView);

        public static Task<TOutput> Navigate<TViewModel, TOutput>(this INavigationService navigationService, IView sourceView)
            where TViewModel : ViewModel<Unit, TOutput>
            => navigationService.Navigate<TViewModel, Unit, TOutput>(Unit.Default, sourceView);

        public static Task Navigate<TViewModel, TInput>(this INavigationService navigationService, TInput payload, IView sourceView)
            where TViewModel : ViewModel<TInput, Unit>
            => navigationService.Navigate<TViewModel, TInput, Unit>(payload, sourceView);
    }
}
