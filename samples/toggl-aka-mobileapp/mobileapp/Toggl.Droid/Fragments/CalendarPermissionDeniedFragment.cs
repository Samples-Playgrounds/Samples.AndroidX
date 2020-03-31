using Android.OS;
using Android.Views;
using Toggl.Core.UI.Extensions;
using Toggl.Core.UI.ViewModels.Calendar;
using Toggl.Droid.Extensions.Reactive;
using Toggl.Shared.Extensions;

namespace Toggl.Droid.Fragments
{
    public sealed partial class CalendarPermissionDeniedFragment : ReactiveDialogFragment<CalendarPermissionDeniedViewModel>
    {
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var view = inflater.Inflate(Resource.Layout.CalendarPermissionDeniedFragment, container, false);
            InitializeViews(view);
            return view;
        }

        public override void OnViewCreated(View view, Bundle savedInstanceState)
        {
            base.OnViewCreated(view, savedInstanceState);

            continueButton.Rx().Tap()
                .Subscribe(() => ViewModel.CloseWithDefaultResult())
                .DisposedBy(DisposeBag);

            allowAccessButton.Rx().Tap()
                .Subscribe(ViewModel.EnableAccess.Inputs)
                .DisposedBy(DisposeBag);
        }
    }
}
