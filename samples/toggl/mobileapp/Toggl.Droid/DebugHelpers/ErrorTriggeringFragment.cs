#if !USE_PRODUCTION_API
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using System;
using System.Reactive.Disposables;
using Toggl.Core.UI.Extensions;
using Toggl.Core.UI.Navigation;
using Toggl.Core.UI.ViewModels;
using Toggl.Droid.Extensions;
using Toggl.Droid.Extensions.Reactive;
using Toggl.Droid.Fragments;
using Toggl.Shared.Extensions;

namespace Toggl.Droid.Debug
{
    public sealed class ErrorTriggeringFragment : ReactiveDialogFragment<ViewModel>
    {
        private readonly CompositeDisposable disposeBag = new CompositeDisposable();

        public ErrorTriggeringFragment()
        {
        }

        public ErrorTriggeringFragment(IntPtr javaReference, JniHandleOwnership transfer)
            : base(javaReference, transfer)
        {
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var matchParent = ViewGroup.LayoutParams.MatchParent;

            var view = new LinearLayout(Context)
            {
                Orientation = Orientation.Vertical,
                LayoutParameters = new ViewGroup.LayoutParams(matchParent, matchParent),
            };

            var tokenReset = createTextView("Token reset error");
            var noWorkspace = createTextView("No workspace error");
            var noDefaultWorkspace = createTextView("No default workspace error");
            var outdatedApp = createTextView("Outdated client error");
            var outdatedApi = createTextView("Outdated API error");
            var outdatedAppPermanently = createTextView("Permanent outdated client error");
            var outdatedApiPermanently = createTextView("Permanent outdated API error");

            tokenReset.Rx().Tap()
                .Subscribe(dismissAndThenRun(tokenResetErrorTriggered))
                .DisposedBy(disposeBag);

            noWorkspace.Rx().Tap()
                .Subscribe(dismissAndThenRun(noWorkspaceErrorTriggered))
                .DisposedBy(disposeBag);

            noDefaultWorkspace.Rx().Tap()
                .Subscribe(dismissAndThenRun(noDefaultWorkspaceErrorTriggered))
                .DisposedBy(disposeBag);

            outdatedApp.Rx().Tap()
                .Subscribe(dismissAndThenRun(outdatedAppErrorTriggered))
                .DisposedBy(disposeBag);

            outdatedApi.Rx().Tap()
                .Subscribe(dismissAndThenRun(outdatedApiErrorTriggered))
                .DisposedBy(disposeBag);

            outdatedAppPermanently.Rx().Tap()
                .Subscribe(dismissAndThenRun(outdatedAppPermanentlyErrorTriggered))
                .DisposedBy(disposeBag);

            outdatedApiPermanently.Rx().Tap()
                .Subscribe(dismissAndThenRun(outdatedApiPermanentlyErrorTriggered))
                .DisposedBy(disposeBag);

            view.AddView(tokenReset);
            view.AddView(noWorkspace);
            view.AddView(noDefaultWorkspace);
            view.AddView(outdatedApp);
            view.AddView(outdatedApi);
            view.AddView(outdatedAppPermanently);
            view.AddView(outdatedApiPermanently);

            return view;
        }

        public override void OnCancel(IDialogInterface dialog)
        {
            Dismiss();
        }

        private TextView createTextView(string text)
        {
            var layoutParameters = new LinearLayout.LayoutParams(
                ViewGroup.LayoutParams.MatchParent,
                ViewGroup.LayoutParams.WrapContent);

            var margin = 20.DpToPixels(Context);
            layoutParameters.TopMargin = margin;
            layoutParameters.LeftMargin = margin;
            layoutParameters.RightMargin = margin;
            layoutParameters.BottomMargin = margin;

            var textView = new TextView(Context)
            {
                Text = text,
                LayoutParameters = layoutParameters
            };

            var outValue = new TypedValue();
            Context.Theme.ResolveAttribute(
                Android.Resource.Attribute.SelectableItemBackground, outValue, true);
            textView.SetBackgroundResource(outValue.ResourceId);

            return textView;
        }

        private Action dismissAndThenRun(Action callback)
            => () =>
            {
                Dismiss();
                callback?.Invoke();
            };

        private void tokenResetErrorTriggered()
        {
            var container = AndroidDependencyContainer.Instance;
            container.NavigationService.Navigate<TokenResetViewModel>(null);
        }

        private void noWorkspaceErrorTriggered()
        {
            var container = AndroidDependencyContainer.Instance;
            var noWorkspaceViewModel = container.ViewModelLoader.Load<NoWorkspaceViewModel>();
            var noWorkspaceFragment = new NoWorkspaceFragment();
            container.ViewModelCache.Cache(noWorkspaceViewModel);
            noWorkspaceFragment.Show(FragmentManager, nameof(NoWorkspaceFragment));
        }

        private async void noDefaultWorkspaceErrorTriggered()
        {
            var container = AndroidDependencyContainer.Instance;
            var noWorkspaceViewModel = container.ViewModelLoader.Load<SelectDefaultWorkspaceViewModel>();
            await noWorkspaceViewModel.Initialize();
            var noWorkspaceFragment = new SelectDefaultWorkspaceFragment();
            container.ViewModelCache.Cache(noWorkspaceViewModel);
            noWorkspaceFragment.Show(FragmentManager, nameof(SelectDefaultWorkspaceFragment));
        }

        private void outdatedAppErrorTriggered()
        {
            var container = AndroidDependencyContainer.Instance;
            container.NavigationService.Navigate<OutdatedAppViewModel>(null);
        }

        private void outdatedApiErrorTriggered()
        {
            var container = AndroidDependencyContainer.Instance;
            container.NavigationService.Navigate<OutdatedAppViewModel>(null);
        }

        private void outdatedAppPermanentlyErrorTriggered()
        {
            var container = AndroidDependencyContainer.Instance;
            container.AccessRestrictionStorage.SetApiOutdated();
            outdatedAppErrorTriggered();
        }

        private void outdatedApiPermanentlyErrorTriggered()
        {
            var container = AndroidDependencyContainer.Instance;
            container.AccessRestrictionStorage.SetClientOutdated();
            outdatedApiErrorTriggered();
        }

        protected override void InitializeViews(View view)
        {
        }
    }
}
#endif
