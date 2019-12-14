using System;
using System.Linq;
using Android.Views;
using Android.Widget;
using AndroidX.ConstraintLayout.Widget;
using AndroidX.ViewPager.Widget;
using Google.Android.Material.AppBar;
using Toggl.Droid.Extensions;
using Toolbar = AndroidX.AppCompat.Widget.Toolbar;

namespace Toggl.Droid.Fragments
{
    public partial class CalendarFragment
    {
        private TextView headerTimeEntriesDurationTextView;
        private TextView headerDateTextView;
        private LockableViewPager calendarViewPager;
        private ViewPager calendarWeekStripePager;
        private ConstraintLayout calendarWeekStripeLabelsContainer;
        private TextView[] calendarWeekStripeHeaders;
        private AppBarLayout appBarLayout;
        private Toolbar toolbar;

        protected override void InitializeViews(View view)
        {
            headerDateTextView = view.FindViewById<TextView>(Resource.Id.HeaderDateTextView);
            headerTimeEntriesDurationTextView = view.FindViewById<TextView>(Resource.Id.HeaderTimeEntriesDurationTextView);
            appBarLayout = view.FindViewById<AppBarLayout>(Resource.Id.HeaderView);
            calendarViewPager = view.FindViewById<LockableViewPager>(Resource.Id.Pager);
            calendarWeekStripePager = view.FindViewById<ViewPager>(Resource.Id.WeekStripePager);
            calendarWeekStripeLabelsContainer = view.FindViewById<ConstraintLayout>(Resource.Id.CalendarWeekStripeLabels);
            calendarWeekStripeHeaders = calendarWeekStripeLabelsContainer.GetChildren().Cast<TextView>().ToArray();
                
            if (calendarWeekStripeHeaders.Length != NumberOfDaysInTheWeek) {
                throw new ArgumentOutOfRangeException($"Week headers should contain exactly {NumberOfDaysInTheWeek} text views");
            }
            
            toolbar = view.FindViewById<Toolbar>(Resource.Id.Toolbar);
        }
    }
}
