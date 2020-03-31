using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using System;
using Toggl.Core.UI.ViewModels.Settings;
using Toggl.Droid.Extensions.Reactive;
using Toggl.Droid.Presentation;
using Toggl.Shared.Extensions;

namespace Toggl.Droid.Activities
{
    [Activity(Theme = "@style/Theme.Splash",
              WindowSoftInputMode = SoftInput.AdjustResize,
              ScreenOrientation = ScreenOrientation.Portrait,
              ConfigurationChanges = ConfigChanges.Orientation | ConfigChanges.ScreenSize)]
    public sealed partial class CalendarSettingsActivity : ReactiveActivity<CalendarSettingsViewModel>
    {
        public CalendarSettingsActivity() : base(
            Resource.Layout.CalendarSettingsActivity,
            Resource.Style.AppTheme,
            Transitions.SlideInFromRight)
        { }

        public CalendarSettingsActivity(IntPtr javaReference, JniHandleOwnership transfer)
            : base(javaReference, transfer)
        {
        }

        protected override void InitializeBindings()
        {
            toggleCalendarsView.Rx()
                .BindAction(ViewModel.TogglCalendarIntegration)
                .DisposedBy(DisposeBag);

            toggleCalendarsSwitch.Rx()
                .BindAction(ViewModel.TogglCalendarIntegration)
                .DisposedBy(DisposeBag);

            ViewModel.CalendarListVisible
                .Subscribe(toggleCalendarsSwitch.Rx().CheckedObserver(ignoreUnchanged: true))
                .DisposedBy(DisposeBag);

            ViewModel.CalendarListVisible
                .Subscribe(calendarsContainer.Rx().IsVisible())
                .DisposedBy(DisposeBag);

            ViewModel
                .Calendars
                .Subscribe(userCalendarsAdapter.Rx().Items())
                .DisposedBy(DisposeBag);

            userCalendarsAdapter
                .ItemTapObservable
                .Subscribe(ViewModel.SelectCalendar.Inputs)
                .DisposedBy(DisposeBag);
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.CalendarSettingsMenu, menu);
            var doneMenuItem = menu.FindItem(Resource.Id.Done);
            doneMenuItem.SetTitle(Shared.Resources.Done);
            return true;
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            if (item.ItemId == Resource.Id.Done)
            {
                ViewModel.Save.Execute();
                return true;
            }

            return base.OnOptionsItemSelected(item);
        }
    }
}
