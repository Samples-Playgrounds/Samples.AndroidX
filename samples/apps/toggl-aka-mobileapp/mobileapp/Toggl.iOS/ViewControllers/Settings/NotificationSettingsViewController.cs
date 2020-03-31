using System;
using Toggl.Core.UI.ViewModels.Settings;
using Toggl.iOS.Extensions;
using Toggl.iOS.Extensions.Reactive;
using Toggl.Shared;
using Toggl.Shared.Extensions;
using UIKit;

namespace Toggl.iOS.ViewControllers.Settings
{
    public sealed partial class NotificationSettingsViewController : ReactiveViewController<NotificationSettingsViewModel>
    {
        public NotificationSettingsViewController(NotificationSettingsViewModel viewModel)
            : base(viewModel, nameof(NotificationSettingsViewController))
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            View.BackgroundColor = ColorAssets.TableBackground;

            NavigationItem.Title = Resources.SmartReminders;
            NotificationDisabledLabel.Text = Resources.NotificationDisabledNotice;
            OpenSettingsButton.SetTitle(Resources.OpenSettingsApp, UIControlState.Normal);
            RowLabel.Text = Resources.UpcomingEvent;
            ExplainationLabel.Text = Resources.NotificationSettingExplaination;

            ViewModel.PermissionGranted
                .Invert()
                .Subscribe(OpenSettingsContainer.Rx().IsVisible())
                .DisposedBy(DisposeBag);

            ViewModel.PermissionGranted
                .Subscribe(CalendarNotificationsContainer.Rx().IsVisible())
                .DisposedBy(DisposeBag);

            OpenSettingsButton.Rx()
                .BindAction(ViewModel.RequestAccess)
                .DisposedBy(DisposeBag);

            CalendarNotificationsRow.Rx().Tap()
                .Subscribe(ViewModel.OpenUpcomingEvents.Inputs)
                .DisposedBy(DisposeBag);

            ViewModel.UpcomingEvents
                .Subscribe(CalendarNotificationsValue.Rx().Text())
                .DisposedBy(DisposeBag);
        }
    }
}
