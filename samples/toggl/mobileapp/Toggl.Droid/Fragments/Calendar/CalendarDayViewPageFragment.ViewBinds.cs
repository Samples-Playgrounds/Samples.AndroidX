using Android.Views;
using Android.Widget;
using AndroidX.RecyclerView.Widget;
using Toggl.Droid.Extensions;
using Toggl.Droid.Views.Calendar;

namespace Toggl.Droid.Fragments.Calendar
{
    public partial class CalendarDayViewPageFragment
    {
        private CalendarDayView calendarDayView;
        private View contextualMenuContainer;
        private View dismissButton;
        private TextView periodText;
        private TextView timeEntryDetails;
        private RecyclerView actionsRecyclerView;

        private void initializeViews(View view)
        { 
            calendarDayView = view.FindViewById<CalendarDayView>(Resource.Id.CalendarDayView);
            contextualMenuContainer = view.FindViewById(Resource.Id.ContextualMenu);
            dismissButton = view.FindViewById<ImageView>(Resource.Id.DismissButton); 
            periodText = view.FindViewById<TextView>(Resource.Id.PeriodText); 
            timeEntryDetails = view.FindViewById<TextView>(Resource.Id.TimeEntryDetails); 
            actionsRecyclerView = view.FindViewById<RecyclerView>(Resource.Id.ActionsRecyclerView);
           
            actionsRecyclerView.SetLayoutManager(new LinearLayoutManager(view.Context, LinearLayoutManager.Horizontal, false));
            contextualMenuContainer.DoOnApplyWindowInsets((v, insets, initialPadding) =>
                contextualMenuContainer.SetPadding(0, 0, 0, insets.SystemWindowInsetBottom));
        }
    }
}
