using System;
using System.Reactive.Linq;
using AndroidX.ViewPager.Widget;
using Toggl.Core.UI.Reactive;

namespace Toggl.Droid.Extensions.Reactive
{
    public static class PagerExtensions
    {
        public static IObservable<int> CurrentItem(this IReactive<ViewPager> reactive)
            => Observable
                .FromEventPattern<ViewPager.PageScrolledEventArgs>(e => reactive.Base.PageScrolled += e, e => reactive.Base.PageScrolled -= e)
                .Select(_ => reactive.Base.CurrentItem);
    }
}
