using CoreGraphics;
using Toggl.Core.UI.Extensions;
using Toggl.Core.UI.ViewModels.Calendar;
using Toggl.iOS.Extensions;
using Toggl.iOS.Extensions.Reactive;
using Toggl.Shared;
using Toggl.Shared.Extensions;
using Foundation;
using UIKit;
using Toggl.Core.UI.Helper;

namespace Toggl.iOS.ViewControllers.Calendar
{
    public sealed partial class CalendarPermissionDeniedViewController
        : ReactiveViewController<CalendarPermissionDeniedViewModel>
    {
        private const float cardHeight = 342;

        public CalendarPermissionDeniedViewController(CalendarPermissionDeniedViewModel viewModel) : base(viewModel)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            HeadingLabel.Text = Resources.NoWorries;
            MessageLabel.Text = Resources.EnableAccessLater;
            ContinueWithoutAccessButton.SetTitle(Resources.ContinueWithoutAccess, UIControlState.Normal);

            var enableAccessString = string.Format(Resources.CalendarPermissionDeniedOr, Resources.CalendarPermissionDeniedEnableButton);

            var rangeStart = enableAccessString.IndexOf(Resources.CalendarPermissionDeniedEnableButton, System.StringComparison.CurrentCulture);
            var rangeEnd = Resources.CalendarPermissionDeniedEnableButton.Length;
            var range = new NSRange(rangeStart, rangeEnd);

            var attributedString = new NSMutableAttributedString(
                enableAccessString,
                new UIStringAttributes { ForegroundColor = ColorAssets.Text });
            attributedString.AddAttributes(
                new UIStringAttributes { ForegroundColor = Colors.Calendar.EnableCalendarAction.ToNativeColor() },
                range);

            EnableAccessButton.SetAttributedTitle(attributedString, UIControlState.Normal);

            var screenWidth = UIScreen.MainScreen.Bounds.Width;
            PreferredContentSize = new CGSize
            {
                // ScreenWidth - 32 for 16pt margins on both sides
                Width = screenWidth > 320 ? screenWidth - 32 : 312,
                Height = cardHeight
            };

            EnableAccessButton.Rx()
                .BindAction(ViewModel.EnableAccess)
                .DisposedBy(DisposeBag);

            ContinueWithoutAccessButton.Rx().Tap()
                .Subscribe(() => { ViewModel.CloseWithDefaultResult(); })
                .DisposedBy(DisposeBag);
        }
    }
}

