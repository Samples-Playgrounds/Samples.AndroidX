using Android.OS;
using Android.Runtime;
using Android.Views;
using System;
using Toggl.Core.UI.ViewModels;
using Toggl.Core.UI.Extensions;
using Toggl.Droid.Extensions;
using Toggl.Droid.Extensions.Reactive;
using Toggl.Shared.Extensions;
using Android.Text.Style;
using Android.Text;
using Android.Text.Method;

namespace Toggl.Droid.Fragments
{
    public sealed partial class TermsOfServiceFragment : ReactiveDialogFragment<TermsOfServiceViewModel>
    {
        public TermsOfServiceFragment() { }

        public TermsOfServiceFragment(IntPtr javaReference, JniHandleOwnership transfer)
            : base(javaReference, transfer) { }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            base.OnCreateView(inflater, container, savedInstanceState);
            var view = inflater.Inflate(Resource.Layout.TermsOfServiceFragment, null);

            InitializeViews(view);

            return view;
        }

        public override void OnViewCreated(View view, Bundle savedInstanceState)
        {
            base.OnViewCreated(view, savedInstanceState);

            termsMessageTextView.TextFormatted = new SpannableString(ViewModel.FormattedDialogText)
                .SetClickableSpan(
                    ViewModel.IndexOfPrivacyPolicy,
                    Shared.Resources.PrivacyPolicy.Length,
                    ViewModel.ViewPrivacyPolicy)
                .SetClickableSpan(
                    ViewModel.IndexOfTermsOfService,
                    Shared.Resources.TermsOfService.Length,
                    ViewModel.ViewTermsOfService);
            termsMessageTextView.MovementMethod = LinkMovementMethod.Instance;

            acceptButton.Rx().Tap()
                .Subscribe(() => ViewModel.Close(true))
                .DisposedBy(DisposeBag);
        }

        public override void OnResume()
        {
            base.OnResume();

            Dialog.Window.SetDefaultDialogLayout(Activity, Context, heightDp: 350);
        }
    }
}
