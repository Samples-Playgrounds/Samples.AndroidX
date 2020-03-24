using Android.App;
using Android.Content.PM;
using Android.Runtime;
using System;
using Android.OS;
using Toggl.Core.UI.Extensions;
using Toggl.Core.UI.ViewModels;
using Toggl.Droid.Extensions.Reactive;
using Toggl.Droid.Presentation;
using Toggl.Shared.Extensions;

namespace Toggl.Droid.Activities
{
    [Activity(Theme = "@style/Theme.Splash",
        ScreenOrientation = ScreenOrientation.Portrait,
        ConfigurationChanges = ConfigChanges.Orientation | ConfigChanges.ScreenSize)]
    public sealed partial class SelectProjectActivity : ReactiveActivity<SelectProjectViewModel>
    {
        private bool hasToggledTasks = false;
        public SelectProjectActivity() : base(
            Resource.Layout.SelectProjectActivity,
            Resource.Style.AppTheme,
            Transitions.SlideInFromBottom)
        {
        }

        public SelectProjectActivity(IntPtr javaReference, JniHandleOwnership transfer)
            : base(javaReference, transfer)
        {
        }

        protected override void OnStart()
        {
            base.OnStart();
            searchField.RequestFocus();
        }

        protected override void InitializeBindings()
        {
            ViewModel.Suggestions
                .Subscribe(adapter.Rx().Items())
                .DisposedBy(DisposeBag);

            adapter.ItemsUpdateCompleted
                .Subscribe(scrollToTopIfHasntToggledTasks)
                .DisposedBy(DisposeBag);

            adapter.ItemTapObservable
                .Subscribe(ViewModel.SelectProject.Inputs)
                .DisposedBy(DisposeBag);

            adapter.ToggleTasks
                .Do(() => hasToggledTasks = true)
                .Subscribe(ViewModel.ToggleTaskSuggestions.Inputs)
                .DisposedBy(DisposeBag);

            ViewModel.PlaceholderText
                .Subscribe(searchField.Rx().Hint())
                .DisposedBy(DisposeBag);

            searchField.Rx().Text()
                .Subscribe(ViewModel.FilterText)
                .DisposedBy(DisposeBag);

            void scrollToTopIfHasntToggledTasks()
            {
                if (!hasToggledTasks)
                {
                    recyclerView.GetLayoutManager().ScrollToPosition(0);
                }

                hasToggledTasks = false;
            }
        }
    }
}