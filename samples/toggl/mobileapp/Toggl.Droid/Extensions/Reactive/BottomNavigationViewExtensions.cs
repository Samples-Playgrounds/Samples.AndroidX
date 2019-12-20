using Android.Views;
using System;
using System.Reactive.Linq;
using Google.Android.Material.BottomNavigation;
using Toggl.Core.UI.Reactive;

namespace Toggl.Droid.Extensions.Reactive
{
    public static class BottomNavigationViewExtensions
    {
        public static IObservable<IMenuItem> ItemSelected(this IReactive<BottomNavigationView> reactive)
            => Observable
                .FromEventPattern<BottomNavigationView.NavigationItemSelectedEventArgs>(
                    e => reactive.Base.NavigationItemSelected += e,
                    e => reactive.Base.NavigationItemSelected -= e)
                .Select(e => e.EventArgs.Item);
    }
}
