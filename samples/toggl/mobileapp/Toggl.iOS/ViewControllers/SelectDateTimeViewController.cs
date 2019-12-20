using CoreGraphics;
using Foundation;
using System;
using Toggl.Core.UI.Extensions;
using Toggl.Core.UI.ViewModels;
using Toggl.iOS.Extensions;
using Toggl.iOS.Extensions.Reactive;
using Toggl.Shared;
using Toggl.Shared.Extensions;
using UIKit;

namespace Toggl.iOS.ViewControllers
{
    public partial class SelectDateTimeViewController : ReactiveViewController<SelectDateTimeViewModel>
    {
        public SelectDateTimeViewController(SelectDateTimeViewModel viewModel)
            : base(viewModel, nameof(SelectDateTimeViewController))
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            TitleLabel.Text = Resources.Startdate;
            SaveButton.SetTitle(Resources.Save, UIControlState.Normal);

            prepareDatePicker();

            DatePicker.MinimumDate = ViewModel.MinDate.ToNSDate();
            DatePicker.MaximumDate = ViewModel.MaxDate.ToNSDate();

            DatePicker.Rx().Date()
                .Subscribe(ViewModel.CurrentDateTime.Accept)
                .DisposedBy(DisposeBag);

            ViewModel.CurrentDateTime
                .Subscribe(DatePicker.Rx().DateTimeObserver())
                .DisposedBy(DisposeBag);

            SaveButton.Rx()
                .BindAction(ViewModel.Save)
                .DisposedBy(DisposeBag);

            CloseButton.Rx().Tap()
                .Subscribe(() => ViewModel.CloseWithDefaultResult())
                .DisposedBy(DisposeBag);
        }

        private void prepareDatePicker()
        {
            var screenWidth = UIScreen.MainScreen.Bounds.Width;
            PreferredContentSize = new CGSize
            {
                //ScreenWidth - 32 for 16pt margins on both sides
                Width = screenWidth > 320 ? screenWidth - 32 : 312,
                Height = View.Frame.Height
            };

            DatePicker.Locale = NSLocale.CurrentLocale;
            DatePicker.Mode = UIDatePickerMode.Date;
        }
    }
}
