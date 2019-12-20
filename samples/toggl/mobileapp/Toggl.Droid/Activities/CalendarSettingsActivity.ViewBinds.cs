using Android.Views;
using Android.Widget;
using AndroidX.RecyclerView.Widget;
using Toggl.Droid.Adapters;

namespace Toggl.Droid.Activities
{
    public sealed partial class CalendarSettingsActivity
    {
        private View toggleCalendarsView;
        private Switch toggleCalendarsSwitch;
        private View calendarsContainer;
        private RecyclerView calendarsRecyclerView;
        private TextView linkCalendarsTitle;
        private TextView linkCalendarsMessage;
        private TextView selectCalendarsTitle;
        private TextView selectCalendarsMessage;
        private UserCalendarsRecyclerAdapter userCalendarsAdapter;

        protected override void InitializeViews()
        {
            linkCalendarsTitle = FindViewById<TextView>(Resource.Id.LinkCalendarsTitle);
            linkCalendarsMessage = FindViewById<TextView>(Resource.Id.LinkCalendarsMessage);
            selectCalendarsTitle = FindViewById<TextView>(Resource.Id.SelectCalendarsTitle);
            selectCalendarsMessage = FindViewById<TextView>(Resource.Id.SelectCalendarsMessage);
            toggleCalendarsView = FindViewById(Resource.Id.ToggleCalendarsView);
            toggleCalendarsSwitch = FindViewById<Switch>(Resource.Id.ToggleCalendarsSwitch);
            calendarsContainer = FindViewById(Resource.Id.CalendarsContainer);
            calendarsRecyclerView = FindViewById<RecyclerView>(Resource.Id.CalendarsRecyclerView);

            userCalendarsAdapter = new UserCalendarsRecyclerAdapter();
            calendarsRecyclerView.SetAdapter(userCalendarsAdapter);
            calendarsRecyclerView.SetLayoutManager(new LinearLayoutManager(this));
            
            linkCalendarsTitle.Text = Shared.Resources.LinkCalendars;
            linkCalendarsMessage.Text = Shared.Resources.LinkCalendarsMessage;
            selectCalendarsTitle.Text = Shared.Resources.SelectCalendars;
            selectCalendarsMessage.Text = Shared.Resources.SelectCalendarsMessage;
            
            SetupToolbar(Shared.Resources.CalendarSettingsTitle);
        }
    }
}
