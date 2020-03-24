using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using System;
using System.Reactive.Linq;
using Toggl.Core.UI.ViewModels;
using Toggl.Droid.Extensions;
using Toggl.Droid.Extensions.Reactive;
using Toggl.Shared.Extensions;

namespace Toggl.Droid.Fragments
{
    public sealed partial class NoWorkspaceFragment : ReactiveDialogFragment<NoWorkspaceViewModel>
    {
        public NoWorkspaceFragment()
        {
        }

        public NoWorkspaceFragment(IntPtr javaReference, JniHandleOwnership transfer)
            : base(javaReference, transfer)
        {
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            base.OnCreateView(LayoutInflater, container, savedInstanceState);
            var rootView = inflater.Inflate(Resource.Layout.NoWorkspaceFragment, container, false);
            InitializeViews(rootView);
            return rootView;
        }

        public override void OnViewCreated(View view, Bundle savedInstanceState)
        {
            base.OnViewCreated(view, savedInstanceState);
            createWorkspaceTextView.Rx()
                .BindAction(ViewModel.CreateWorkspaceWithDefaultName)
                .DisposedBy(DisposeBag);

            tryAgainTextView.Rx()
                .BindAction(ViewModel.TryAgain)
                .DisposedBy(DisposeBag);

            ViewModel.IsLoading
                .Select(CommonFunctions.Invert)
                .Subscribe(createWorkspaceTextView.Rx().Enabled())
                .DisposedBy(DisposeBag);

            ViewModel.IsLoading
                .Select(CommonFunctions.Invert)
                .Subscribe(tryAgainTextView.Rx().Enabled())
                .DisposedBy(DisposeBag);

            ViewModel.IsLoading
                .StartWith(false)
                .Subscribe(onLoadingStateChanged)
                .DisposedBy(DisposeBag);
        }

        public override void OnResume()
        {
            base.OnResume();
            updateDialogHeight();
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (!disposing) return;

            DisposeBag?.Dispose();
        }

        private void onLoadingStateChanged(bool isLoading)
        {
            progressBar.Visibility = isLoading.ToVisibility();
            updateDialogHeight();
        }

        private void updateDialogHeight()
        {
            Dialog?.Window?.SetDefaultDialogLayout(Activity, Context, heightDp: ViewGroup.LayoutParams.WrapContent);
        }
    }
}
