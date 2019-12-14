using Android.App;
using Android.Content.PM;
using Android.Runtime;
using System;
using System.Reactive.Linq;
using Android.Views;
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
    public partial class SelectTagsActivity : ReactiveActivity<SelectTagsViewModel>
    {
        private IMenuItem doneMenuItem;
        private IMenuItem clearMenuItem;

        public SelectTagsActivity() : base(
            Resource.Layout.SelectTagsActivity,
            Resource.Style.AppTheme,
            Transitions.SlideInFromBottom)
        {
        }

        public SelectTagsActivity(IntPtr javaReference, JniHandleOwnership transfer)
            : base(javaReference, transfer)
        {
        }

        protected override void OnStart()
        {
            base.OnStart();
            textField.RequestFocus();
        }

        protected override void InitializeBindings()
        {
            ViewModel.Tags
                .Subscribe(selectTagsRecyclerAdapter.Rx().Items())
                .DisposedBy(DisposeBag);

            ViewModel.FilterText
                .Select(text => text == string.Empty)
                .DistinctUntilChanged()
                .Subscribe(isEmpty => doneMenuItem?.SetVisible(isEmpty))
                .DisposedBy(DisposeBag);

            ViewModel.FilterText
                .Select(text => text == string.Empty)
                .DistinctUntilChanged()
                .Invert()
                .Subscribe(isNonEmpty => clearMenuItem?.SetVisible(isNonEmpty))
                .DisposedBy(DisposeBag);

            textField.Rx().Text()
                .Subscribe(ViewModel.FilterText)
                .DisposedBy(DisposeBag);

            selectTagsRecyclerAdapter.ItemTapObservable
                .Subscribe(ViewModel.SelectTag.Inputs)
                .DisposedBy(DisposeBag);
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.DoneOrClearMenu, menu);
            
            doneMenuItem = menu.FindItem(Resource.Id.ButtonMenuItem);
            doneMenuItem.SetTitle(Shared.Resources.Done);

            clearMenuItem = menu.FindItem(Resource.Id.ClearMenuItem);

            return true;
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            if (item.ItemId == Resource.Id.ButtonMenuItem)
            {
                ViewModel.Save.Execute();
                return true;
            }
            else if (item.ItemId == Resource.Id.ClearMenuItem)
            {
                textField.Text = string.Empty;
            }

            return base.OnOptionsItemSelected(item);
        }
    }
}