using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using AndroidX.AppCompat.App;
using AndroidX.AppCompat.Widget;
using Android.Views;
using System;
using System.Reactive.Disposables;
using Toggl.Core.UI.ViewModels;
using Toggl.Core.UI.Views;
using Toggl.Droid.Extensions;
using Toggl.Droid.Extensions;
using Toggl.Droid.Presentation;

namespace Toggl.Droid.Activities
{
    public abstract partial class ReactiveActivity<TViewModel> : AppCompatActivity, IView
        where TViewModel : class, IViewModel
    {
        private readonly int layoutResId;
        private readonly int correctTheme;
        private readonly ActivityTransitionSet transitions;

        protected abstract void InitializeViews();
        protected abstract void InitializeBindings();
        protected CompositeDisposable DisposeBag { get; private set; } = new CompositeDisposable();

        public TViewModel ViewModel { get; private set; }

        protected ReactiveActivity(
            int layoutResId,
            int correctTheme,
            ActivityTransitionSet transitions)
        {
            this.layoutResId = layoutResId;
            this.transitions = transitions;
            this.correctTheme = correctTheme;
        }

        protected ReactiveActivity(IntPtr javaReference, JniHandleOwnership transfer)
            : base(javaReference, transfer)
        {
        }

        protected sealed override void OnCreate(Bundle bundle)
        {
            SetTheme(correctTheme);
            base.OnCreate(bundle);

            var cache = AndroidDependencyContainer.Instance.ViewModelCache;
            ViewModel = cache.Get<TViewModel>();

            if (ViewModel == null)
            {
                bailOutToSplashScreen();
                return;
            }

            ViewModel?.AttachView(this);
            SetContentView(layoutResId);
            setupRootViewInsets();
            OverridePendingTransition(transitions.SelfIn, transitions.OtherOut);
            InitializeViews();
            RestoreViewModelStateFromBundle(bundle);
            InitializeBindings();
            this.SetQFlags();
        }

        /// <summary>
        /// Use this to rehydrate the ViewModel after tombstoning
        /// </summary>
        /// <param name="bundle"></param>
        protected virtual void RestoreViewModelStateFromBundle(Bundle bundle)
        {
        }

        protected override void OnStart()
        {
            base.OnStart();
            ViewModel?.ViewAppearing();
        }

        protected override void OnResume()
        {
            base.OnResume();
            ViewModel?.ViewAppeared();
        }

        protected override void OnPause()
        {
            base.OnPause();
            ViewModel?.ViewDisappearing();
        }

        protected override void OnStop()
        {
            base.OnStop();
            ViewModel?.ViewDisappeared();
        }

        protected override void OnDestroy()
        {
            ViewModel?.DetachView();
            base.OnDestroy();
            ViewModel?.ViewDestroyed();
        }

        public override void Finish()
        {
            base.Finish();
            OverridePendingTransition(transitions.OtherIn, transitions.SelfOut);
        }

        public override void OnBackPressed()
        {
            if (ViewModel == null)
            {
                base.OnBackPressed();
                return;
            }

            ViewModel.CloseWithDefaultResult();
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            if (item.ItemId == Android.Resource.Id.Home)
            {
                ViewModel.CloseWithDefaultResult();
                return true;
            }

            return base.OnOptionsItemSelected(item);
        }

        public void Close()
        {
            AndroidDependencyContainer.Instance
                .ViewModelCache
                .Clear<TViewModel>();

            Finish();
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (!disposing) return;
            DisposeBag?.Dispose();
        }

        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);

            switch (requestCode)
            {
                case googleSignInResult:
                    onGoogleSignInResult(data);
                    break;
            }
        }

        protected void SetupToolbar(string title = "", bool showHomeAsUp = true)
        {
            var toolbar = FindViewById<Toolbar>(Resource.Id.Toolbar);
            toolbar.Title = title;
            SetSupportActionBar(toolbar);

            SupportActionBar.SetDisplayHomeAsUpEnabled(showHomeAsUp);
            SupportActionBar.SetDisplayShowHomeEnabled(showHomeAsUp);
        }

        private void setupRootViewInsets()
        {
            var rootContentView = (ViewGroup) FindViewById(Android.Resource.Id.Content);
            if (rootContentView != null && rootContentView.ChildCount > 0)
            {
                rootContentView.GetChildAt(0).FitTopInset();
            }
        }

        private void bailOutToSplashScreen()
        {
            StartActivity(new Intent(this, typeof(SplashScreen)).AddFlags(ActivityFlags.TaskOnHome));
            Finish();
        }
    }
}
