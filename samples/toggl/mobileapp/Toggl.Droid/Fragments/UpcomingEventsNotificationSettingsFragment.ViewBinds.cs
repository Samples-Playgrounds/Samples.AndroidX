using Android.Views;
using Android.Widget;
using AndroidX.RecyclerView.Widget;
using Toggl.Droid.Adapters;

namespace Toggl.Droid.Fragments
{
    public sealed partial class UpcomingEventsNotificationSettingsFragment
    {
        private TextView setSmartRemindersTitle;
        private TextView setSmartRemindersMessage;
        private RecyclerView recyclerView;

        protected override void InitializeViews(View view)
        {
            setSmartRemindersTitle = view.FindViewById<TextView>(Resource.Id.SetSmartRemindersTitle);
            setSmartRemindersMessage = view.FindViewById<TextView>(Resource.Id.SetSmartRemindersMessage);
            recyclerView = view.FindViewById<RecyclerView>(Resource.Id.CalendarsRecyclerView);
            
            setSmartRemindersTitle.Text = Shared.Resources.SetSmartReminders;
            setSmartRemindersMessage.Text = Shared.Resources.SetSmartRemindersMessage;

            adapter = new SelectCalendarNotificationsOptionAdapter();
            recyclerView.SetAdapter(adapter);
            recyclerView.SetLayoutManager(new LinearLayoutManager(Context));
        }
    }
}
