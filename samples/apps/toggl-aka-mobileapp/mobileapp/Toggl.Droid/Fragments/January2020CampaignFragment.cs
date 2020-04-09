using System;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Toggl.Core.UI.ViewModels;
using Toggl.Droid.Extensions.Reactive;
using Toggl.Shared.Extensions;

namespace Toggl.Droid.Fragments
{
    public class January2020CampaignFragment : ReactiveDialogFragment<January2020CampaignViewModel>
    {
        private TextView campaignTitle;
        private TextView campaignMessage;
        private Button mainActionButton;
        private Button dismissButton;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            base.OnCreateView(inflater, container, savedInstanceState);
            Cancelable = false;
            var view = inflater.Inflate(Resource.Layout.January2020CampaignFragment, null);

            InitializeViews(view);

            return view;
        }

        public override void OnViewCreated(View view, Bundle savedInstanceState)
        {
            base.OnViewCreated(view, savedInstanceState);

            campaignTitle.Text = Shared.Resources.January2020CampaignTitle;
            mainActionButton.Text = Shared.Resources.January2020CampaignPositiveButtonText;
            dismissButton.Text = Shared.Resources.January2020CampaignNegativeButtonText;
            campaignMessage.Text = ViewModel.CampaignMessage;
            
            mainActionButton.Rx().Tap()
                .Subscribe(ViewModel.OpenBrowser.Inputs)
                .DisposedBy(DisposeBag);
            
            dismissButton.Rx().Tap()
                .Subscribe(ViewModel.Dismiss.Inputs)
                .DisposedBy(DisposeBag);
        }

        protected override void InitializeViews(View view)
        {
            campaignTitle = view.FindViewById<TextView>(Resource.Id.CampaignTitle);
            campaignMessage = view.FindViewById<TextView>(Resource.Id.CampaignMessage);
            mainActionButton = view.FindViewById<Button>(Resource.Id.MainActionButton);
            dismissButton = view.FindViewById<Button>(Resource.Id.DismissButton);
        }
    }
}
