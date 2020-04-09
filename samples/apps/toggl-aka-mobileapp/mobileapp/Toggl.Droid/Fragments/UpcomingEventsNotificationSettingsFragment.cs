using Android.OS;
using Android.Views;
using Toggl.Core.UI.ViewModels.Settings;
using Toggl.Droid.Adapters;
using Toggl.Shared.Extensions;

namespace Toggl.Droid.Fragments
{
    public sealed partial class UpcomingEventsNotificationSettingsFragment : ReactiveDialogFragment<UpcomingEventsNotificationSettingsViewModel>
    {
        private SelectCalendarNotificationsOptionAdapter adapter;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var view = inflater.Inflate(Resource.Layout.UpcomingEventsNotificationSettingsFragment, container, false);
            InitializeViews(view);

            return view;
        }

        public override void OnViewCreated(View view, Bundle savedInstanceState)
        {
            base.OnViewCreated(view, savedInstanceState);
            
            adapter.Items = ViewModel.AvailableOptions;
            adapter
                .ItemTapObservable
                .Subscribe(ViewModel.SelectOption.Inputs)
                .DisposedBy(DisposeBag);
        }

        public override void OnResume()
        {
            base.OnResume();
            var layoutParams = Dialog.Window.Attributes;
            layoutParams.Width = ViewGroup.LayoutParams.MatchParent;
            layoutParams.Height = ViewGroup.LayoutParams.WrapContent;
            Dialog.Window.Attributes = layoutParams;
        }
    }
}
