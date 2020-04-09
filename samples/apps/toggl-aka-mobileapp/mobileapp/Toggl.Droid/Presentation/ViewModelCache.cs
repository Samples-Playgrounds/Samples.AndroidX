using System;
using System.Collections.Generic;
using System.Linq;
using Toggl.Core.UI.ViewModels;

namespace Toggl.Droid.Presentation
{
    public sealed class ViewModelCache
    {
        private readonly Dictionary<Type, IViewModel> cache = new Dictionary<Type, IViewModel>();

        public TViewModel Get<TViewModel>()
            where TViewModel : IViewModel
        {
            cache.TryGetValue(typeof(TViewModel), out var cachedViewModel);
            return (TViewModel)cachedViewModel;
        }

        public void Cache<TViewModel>(TViewModel viewModel)
            where TViewModel : IViewModel
        {
            cache[viewModel.GetType()] = viewModel;
        }

        public void Clear<TViewModel>()
            where TViewModel : IViewModel
        {
            cache.Remove(typeof(TViewModel));
        }

        public void ClearAll()
        {
            foreach (var cacheValue in cache.Values.ToList())
            {
                cacheValue?.DetachView();
                cacheValue?.CloseWithDefaultResult();
            }

            cache.Clear();
        }
    }
}
