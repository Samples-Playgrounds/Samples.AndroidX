using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using System;
using System.Reactive.Linq;
using Toggl.Core.UI.ViewModels;
using Toggl.Droid.Extensions.Reactive;
using Toggl.Droid.Presentation;
using Toggl.Networking.Exceptions;
using Toggl.Shared.Extensions;

namespace Toggl.Droid.Activities
{
    [Activity(Theme = "@style/Theme.Splash",
        ScreenOrientation = ScreenOrientation.Portrait,
        WindowSoftInputMode = SoftInput.StateVisible,
        ConfigurationChanges = ConfigChanges.Orientation | ConfigChanges.ScreenSize)]
    public partial class SendFeedbackActivity : ReactiveActivity<SendFeedbackViewModel>
    {
        private bool sendEnabled;

        public SendFeedbackActivity() : base(
            Resource.Layout.SendFeedbackActivity,
            Resource.Style.AppTheme,
            Transitions.SlideInFromRight)
        { }

        public SendFeedbackActivity(IntPtr javaReference, JniHandleOwnership transfer)
            : base(javaReference, transfer)
        { }

        protected override void InitializeBindings()
        {
            feedbackEditText.Rx().Text()
                .Subscribe(ViewModel.FeedbackText)
                .DisposedBy(DisposeBag);

            errorCard.Rx()
                .BindAction(ViewModel.DismissError)
                .DisposedBy(DisposeBag);

            var sendButtonEnabled = ViewModel.SendEnabled.CombineLatest(ViewModel.IsLoading,
                (sendIsEnabled, isLoading) => sendIsEnabled && !isLoading);
            sendButtonEnabled
                 .Subscribe(onSendEnabled)
                 .DisposedBy(DisposeBag);

            ViewModel.Error
                .Select(selectErrorMessage)
                .Subscribe(errorInfoText.Rx().TextObserver())
                .DisposedBy(DisposeBag);

            ViewModel.Error
                .Select(error => error != null)
                .Subscribe(errorCard.Rx().IsVisible())
                .DisposedBy(DisposeBag);

            ViewModel.IsLoading
                .Subscribe(progressBar.Rx().IsVisible())
                .DisposedBy(DisposeBag);

            ViewModel.IsLoading
                .Invert()
                .Subscribe(feedbackEditText.Rx().Enabled())
                .DisposedBy(DisposeBag);

            void onSendEnabled(bool enabled)
            {
                sendEnabled = enabled;
                InvalidateOptionsMenu();
            }

            string selectErrorMessage(Exception exception)
                => exception is OfflineException
                    ? Shared.Resources.GenericInternetConnectionErrorMessage
                    : Shared.Resources.SomethingWentWrongTryAgain;
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.SendFeedbackMenu, menu);
            return true;
        }

        public override bool OnPrepareOptionsMenu(IMenu menu)
        {
            var sendMenuItem = menu.FindItem(Resource.Id.SendMenuItem);
            sendMenuItem.SetEnabled(sendEnabled);
            sendMenuItem.SetTitle(Shared.Resources.Send);
            return true;
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Resource.Id.SendMenuItem:
                    ViewModel.Send.Execute();
                    return true;

                default:
                    return base.OnOptionsItemSelected(item);
            }
        }
    }
}
