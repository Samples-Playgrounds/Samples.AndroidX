using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using System;
using System.Collections.Generic;
using System.Reactive;
using System.Reactive.Disposables;
using AndroidX.Fragment.App;
using Toggl.Core.UI.ViewModels;
using Toggl.Core.UI.Views;

namespace Toggl.Droid.Fragments
{
    public abstract class ReactiveDialogFragment<TViewModel> : DialogFragment, IView
        where TViewModel : class, IViewModel
    {
        protected CompositeDisposable DisposeBag = new CompositeDisposable();

        protected abstract void InitializeViews(View view);

        public TViewModel ViewModel { get; private set; }

        protected ReactiveDialogFragment()
        {
        }

        protected ReactiveDialogFragment(IntPtr javaReference, JniHandleOwnership transfer)
            : base(javaReference, transfer)
        {
        }

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetStyle(StyleNoTitle, Theme);
        }

        public override void OnViewCreated(View view, Bundle savedInstanceState)
        {
            base.OnViewCreated(view, savedInstanceState);
            ViewModel = AndroidDependencyContainer
                .Instance
                .ViewModelCache
                .Get<TViewModel>();

            ViewModel?.AttachView(this);
        }

        public override void OnStart()
        {
            base.OnStart();
            ViewModel?.ViewAppearing();
        }

        public override void OnResume()
        {
            base.OnResume();
            ViewModel?.ViewAppeared();
        }

        public override void OnPause()
        {
            base.OnPause();
            ViewModel?.ViewDisappearing();
        }

        public override void OnStop()
        {
            base.OnStop();
            ViewModel?.ViewDisappeared();
        }

        public override void OnDestroyView()
        {
            ViewModel?.DetachView();
            base.OnDestroyView();
            DisposeBag.Dispose();
            DisposeBag = new CompositeDisposable();
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (!disposing) return;
            DisposeBag?.Dispose();
        }

        public void Close()
        {
            AndroidDependencyContainer
                .Instance
                .ViewModelCache
                .Clear<TViewModel>();

            Dismiss();
        }

        public override void OnCancel(IDialogInterface dialog)
        {
            ViewModel.CloseWithDefaultResult();
        }

        public void OpenAppSettings()
        {
            if (IsDetached
                || Activity == null
                || Activity.IsFinishing
                || !(Activity is IPermissionRequester permissionRequester))
                return;

            permissionRequester.OpenAppSettings();
        }

        public IObservable<bool> Confirm(string title, string message, string confirmButtonText, string dismissButtonText)
            => throw new InvalidOperationException("You shouldn't be doing this from the Dialog. Use the parent activity/fragment");

        public IObservable<Unit> Alert(string title, string message, string buttonTitle)
            => throw new InvalidOperationException("You shouldn't be doing this from the Dialog. Use the parent activity/fragment");

        public IObservable<bool> ConfirmDestructiveAction(ActionType type, params object[] formatArguments)
            => throw new InvalidOperationException("You shouldn't be doing this from the Dialog. Use the parent activity/fragment");

        public IObservable<T> Select<T>(string title, IEnumerable<SelectOption<T>> options, int initialSelectionIndex)
            => throw new InvalidOperationException("You shouldn't be doing this from the Dialog. Use the parent activity/fragment");
        
        public IObservable<T> SelectAction<T>(string title, IEnumerable<SelectOption<T>> options)
            => throw new InvalidOperationException("This is not implemented for Android");

        public IObservable<bool> RequestCalendarAuthorization(bool force = false)
            => throw new InvalidOperationException("You shouldn't be doing this from the Dialog. Use the parent activity/fragment");

        public IObservable<bool> RequestNotificationAuthorization(bool force = false)
            => throw new InvalidOperationException("You shouldn't be doing this from the Dialog. Use the parent activity/fragment");

        public IObservable<string> GetGoogleToken()
            => throw new InvalidOperationException("You shouldn't be doing this from the Dialog. Use the parent activity/fragment");
    }
}
