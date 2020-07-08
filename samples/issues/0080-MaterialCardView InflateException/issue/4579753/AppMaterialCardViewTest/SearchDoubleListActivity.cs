using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Content.Res;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using AndroidX.AppCompat.App;
using AndroidX.RecyclerView.Widget;
using AndroidX.SwipeRefreshLayout.Widget;
using AndroidX.ViewPager.Widget;
using Google.Android.Material.BottomNavigation;

namespace AppMaterialCardViewTest
{
    [Activity(Label = "Выберите", Theme = "@style/AppTheme.NoActionBar", ConfigurationChanges = Android.Content.PM.ConfigChanges.Orientation | Android.Content.PM.ConfigChanges.ScreenSize)]
    public class SearchDoubleListActivity : AppCompatActivity
    {
        LessonsAdapter mAdapter;
        string parent;
        ViewPager vp;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.search_list_layout);

            var toolbar = FindViewById<AndroidX.AppCompat.Widget.Toolbar>(Resource.Id.toolbar);
            SetSupportActionBar(toolbar);
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            // Create your application here

            vp = FindViewById<ViewPager>(Resource.Id.viewpager);

            List<DayRasp> daysRasp = new List<DayRasp>();
            var lesson = new LessonRasp();
            lesson.SetDemo();
            var list = new List<LessonRasp>
                {
                    lesson,
                    lesson,
                    lesson,
                };

            DayRasp day1 = new DayRasp();
            day1.date = DateTime.Today;
            day1.lessons = new List<LessonRasp>(list);
            daysRasp.Add(day1);
            DayRasp day2 = new DayRasp();
            day2.date = DateTime.Today.AddDays(1);
            day2.lessons = new List<LessonRasp>(list);
            daysRasp.Add(day2);


            BuildRasp(daysRasp);



        }

        private void BuildRasp(List<DayRasp> daysRasp)
        {
            List<View> pages = new List<View>();

            int todayInd = 0;
            for (int i = 0; i < daysRasp.Count; i++)
            {
                DayRasp day = daysRasp[i];
                var pageLayout = BuildPage(day);
                pages.Add(pageLayout);

                if (day.date == DateTime.Today)
                    todayInd = i;
            }

            CustomPagerAdapter adapter1 = new CustomPagerAdapter(Application.Context, pages);
            this.RunOnUiThread(() =>
            {
                if (vp.ChildCount > 0)
                    todayInd = vp.CurrentItem; //вместо сегодня исп. последний

                vp.Adapter = adapter1;
                vp.SetCurrentItem(todayInd, false);
            });

            //vp.Adapter = adapter1;
        }

        private LinearLayout BuildPage(DayRasp day)
        {
            RecyclerView listview1 = new RecyclerView(Application.Context);
            listview1.SetLayoutManager(new LinearLayoutManager(Application.Context));
            var co1 = listview1.ItemDecorationCount;
            var mAdapter = new LessonsAdapter(day.lessons.ToArray(), this);
            mAdapter.ItemWasClicked += RaspItemWasClicked;
            //listview1.SetAdapter(new LessonsAdapter(LessonsList[i].ToArray()));
            listview1.SetAdapter(mAdapter);

            LinearLayout linLayout = new LinearLayout(Application.Context);
            linLayout.Orientation = Android.Widget.Orientation.Vertical;
            TextView tv = new TextView(Application.Context);
            //tv.Text = "сегодня, среда, 25 марта";
            tv.Text = day.dateStr;
            tv.Typeface = Typeface.Create("sans-serif-medium", TypefaceStyle.Normal);
            tv.Alpha = 0.54f;
            tv.TextSize = 14;
            tv.SetTextColor(Color.ParseColor("#000000"));
            int dp = 10;
            int px = (int)TypedValue.ApplyDimension(ComplexUnitType.Dip, dp, Application.Context.Resources.DisplayMetrics);
            int dpLeft = 4;
            int pxLeft = (int)TypedValue.ApplyDimension(ComplexUnitType.Dip, dpLeft, Application.Context.Resources.DisplayMetrics);
            LinearLayout.LayoutParams lp = new LinearLayout.LayoutParams(LinearLayout.LayoutParams.WrapContent, LinearLayout.LayoutParams.WrapContent);
            lp.SetMargins(pxLeft, 0, 0, px);
            tv.LayoutParameters = lp;

            var swipeRefreshLayout = new SwipeRefreshLayout(Application.Context);
            swipeRefreshLayout.Refresh += SwipeRefreshLayout_Refresh;
            swipeRefreshLayout.AddView(listview1);

            linLayout.AddView(tv);
            linLayout.AddView(swipeRefreshLayout);

            return linLayout;
        }

        private void SwipeRefreshLayout_Refresh(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private void RaspItemWasClicked(object sender, EventArgs e)
        {
            
        }
    }
}