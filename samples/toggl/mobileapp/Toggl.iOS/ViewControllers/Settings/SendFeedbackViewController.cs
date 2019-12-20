using System;
using System.Reactive.Linq;
using Toggl.Core.UI.Extensions;
using Toggl.Core.UI.Helper;
using Toggl.Core.UI.ViewModels;
using Toggl.iOS.Extensions;
using Toggl.iOS.Extensions.Reactive;
using Toggl.Shared;
using Toggl.Shared.Extensions;
using UIKit;
using static Toggl.Shared.Extensions.CommonFunctions;

namespace Toggl.iOS.ViewControllers.Settings
{
    public sealed partial class SendFeedbackViewController : KeyboardAwareViewController<SendFeedbackViewModel>
    {

        public SendFeedbackViewController(SendFeedbackViewModel viewModel)
            : base(viewModel, nameof(SendFeedbackViewController))
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            TitleLabel.Text = Resources.SubmitFeedback;
            FeedbackPlaceholderTextView.Text = Resources.FeedbackFieldPlaceholder;
            ErrorTitleLabel.Text = Resources.SubmitFeedback.ToUpper();
            ErrorMessageLabel.Text = Resources.ContactUsSomethingWentWrongTryAgain;
            SendButton.SetTitle(Resources.ContactUsSend, UIControlState.Normal);

            prepareViews();
            prepareIndicatorView();

            CloseButton.Rx().Tap()
                .Subscribe(() => ViewModel.CloseWithDefaultResult())
                .DisposedBy(DisposeBag);

            FeedbackTextView.Rx().Text()
                .Subscribe(ViewModel.FeedbackText)
                .DisposedBy(DisposeBag);

            ErrorView.Rx()
                .BindAction(ViewModel.DismissError)
                .DisposedBy(DisposeBag);

            SendButton.Rx()
                .BindAction(ViewModel.Send)
                .DisposedBy(DisposeBag);
            SendButton.TouchUpInside += (sender, args) => { FeedbackTextView.ResignFirstResponder(); };

            ViewModel.IsFeedbackEmpty
                .Subscribe(FeedbackPlaceholderTextView.Rx().IsVisible())
                .DisposedBy(DisposeBag);

            ViewModel.Error
                .Select(NotNull)
                .Subscribe(ErrorView.Rx().AnimatedIsVisible())
                .DisposedBy(DisposeBag);

            ViewModel.SendEnabled
                .Subscribe(SendButton.Rx().Enabled())
                .DisposedBy(DisposeBag);

            ViewModel.IsLoading
                .Invert()
                .Subscribe(SendButton.Rx().IsVisible())
                .DisposedBy(DisposeBag);

            ViewModel.IsLoading
                .Invert()
                .Subscribe(CloseButton.Rx().IsVisible())
                .DisposedBy(DisposeBag);

            ViewModel.IsLoading
                .Subscribe(IndicatorView.Rx().IsVisible())
                .DisposedBy(DisposeBag);

            ViewModel.IsLoading
                .Subscribe(UIApplication.SharedApplication.Rx().NetworkActivityIndicatorVisible())
                .DisposedBy(DisposeBag);
        }

        protected override void KeyboardWillShow(object sender, UIKeyboardEventArgs e)
        {
            UIEdgeInsets contentInsets = new UIEdgeInsets(0, 0, e.FrameEnd.Height, 0);
            FeedbackTextView.ContentInset = contentInsets;
            FeedbackTextView.ScrollIndicatorInsets = contentInsets;
        }

        protected override void KeyboardWillHide(object sender, UIKeyboardEventArgs e)
        {
            FeedbackTextView.ContentInset = UIEdgeInsets.Zero;
            FeedbackTextView.ScrollIndicatorInsets = UIEdgeInsets.Zero;
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
            IndicatorView.StartSpinning();
        }

        private void prepareViews()
        {
            ErrorView.Hidden = true;
            FeedbackTextView.TintColor = Colors.Feedback.Cursor.ToNativeColor();
            FeedbackPlaceholderTextView.TintColor = Colors.Feedback.Cursor.ToNativeColor();
        }

        private void prepareIndicatorView()
        {
            IndicatorView.IndicatorColor = Colors.Feedback.ActivityIndicator.ToNativeColor();
            IndicatorView.Hidden = true;
        }
    }
}

