using System;
using System.Linq;
using System.Threading.Tasks;
using Toggl.Core.UI.ViewModels;
using Toggl.Core.UI.Views;
using Toggl.Shared.Extensions;

namespace Toggl.Core.UI.Navigation
{
    public interface IPresenter
    {
        bool CanPresent<TInput, TOutput>(ViewModel<TInput, TOutput> viewModel);

        Task Present<TInput, TOutput>(ViewModel<TInput, TOutput> viewModel, IView sourceView);

        bool ChangePresentation(IPresentationChange presentationChange);
    }

    public sealed class CompositePresenter : IPresenter
    {
        private readonly IPresenter[] presenters;

        public CompositePresenter(params IPresenter[] presenters)
        {
            this.presenters = presenters;
        }

        public bool CanPresent<TInput, TOutput>(ViewModel<TInput, TOutput> viewModel)
            => presenters.Any(p => p.CanPresent(viewModel));

        public Task Present<TInput, TOutput>(ViewModel<TInput, TOutput> viewModel, IView sourceView)
        {
            var presenter = presenters.FirstOrDefault(p => p.CanPresent(viewModel));
            if (presenter == null)
                throw new InvalidOperationException($"Failed to find a presenter that could present ViewModel with type {viewModel.GetType().Name}");

            return presenter.Present(viewModel, sourceView);
        }

        public bool ChangePresentation(IPresentationChange presentationChange)
            => presenters
                .Select(presenter => presenter.ChangePresentation(presentationChange))
                .Aggregate(CommonFunctions.Or);
    }
}
