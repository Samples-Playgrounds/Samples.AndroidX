using Android.Content;
using Android.Runtime;
using Android.Util;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using AndroidX.RecyclerView.Widget;
using AndroidX.ViewPager.Widget;
using Toggl.Core.UI.Helper;
using Toggl.Core.UI.Parameters;
using Toggl.Core.UI.ViewModels;
using Toggl.Core.UI.ViewModels.ReportsCalendar;
using Toggl.Core.UI.ViewModels.ReportsCalendar.QuickSelectShortcuts;
using Toggl.Droid.Adapters;
using Toggl.Droid.Extensions;
using Toggl.Droid.Extensions.Reactive;
using Toggl.Shared.Extensions;

namespace Toggl.Droid.Views
{
    [Register("toggl.droid.views.ReportsCalendarView")]
    public sealed class ReportsCalendarView : LinearLayout
    {
        private TextView monthYear;
        private ViewPager monthsPager;
        private LinearLayout daysHeader;
        private RecyclerView shortcutsRecyclerView;

        private int rowHeight;
        private int currentRowCount;

        private ReportsCalendarViewModel viewModel;

        private readonly CompositeDisposable disposeBag = new CompositeDisposable();

        public ReportsCalendarView(IntPtr javaReference, JniHandleOwnership transfer)
            : base(javaReference, transfer)
        {
        }

        public ReportsCalendarView(Context context)
            : base(context)
        {
            Init(Context);
        }

        public ReportsCalendarView(Context context, IAttributeSet attrs)
            : base(context, attrs)
        {
            Init(Context);
        }

        public ReportsCalendarView(Context context, IAttributeSet attrs, int defStyleAttr)
            : base(context, attrs, defStyleAttr)
        {
            Init(Context);
        }

        public ReportsCalendarView(Context context, IAttributeSet attrs, int defStyleAttr, int defStyleRes)
            : base(context, attrs, defStyleAttr, defStyleRes)
        {
            Init(Context);
        }

        private void Init(Context context)
        {
            Inflate(Context, Resource.Layout.ReportsCalendarView, this);
            SetBackgroundColor(Context.SafeGetColor(Resource.Color.background));

            rowHeight = context.Resources.DisplayMetrics.WidthPixels / 7;

            monthYear = FindViewById<TextView>(Resource.Id.ReportsCalendarMonthYear);
            daysHeader = FindViewById<LinearLayout>(Resource.Id.ReportsCalendarFragmentHeader);
            monthsPager = FindViewById<ViewPager>(Resource.Id.ReportsCalendarFragmentViewPager);
            shortcutsRecyclerView = FindViewById<RecyclerView>(Resource.Id.ReportsCalendarFragmentShortcuts);
            shortcutsRecyclerView.SetLayoutManager(new LinearLayoutManager(context, LinearLayoutManager.Horizontal, false));
        }

        public void SetupWith(ReportsCalendarViewModel reportsCalendarViewModel)
        {
            viewModel = reportsCalendarViewModel;

            viewModel.DayHeadersObservable
                .Subscribe(setupWeekdaysLabels)
                .DisposedBy(disposeBag);

            var calendarPagesAdapter = new ReportsCalendarPagerAdapter(Context);
            monthsPager.Adapter = calendarPagesAdapter;

            viewModel.MonthsObservable
                .Subscribe(calendarPagesAdapter.UpdateMonths)
                .DisposedBy(disposeBag);

            monthsPager.Rx().CurrentItem()
                .Subscribe(viewModel.SetCurrentPage)
                .DisposedBy(disposeBag);

            monthsPager.Rx().CurrentItem()
                .Subscribe(viewModel.UpdateMonth)
                .DisposedBy(disposeBag);

            calendarPagesAdapter.DayTaps
                .Subscribe(viewModel.SelectDay.Inputs)
                .DisposedBy(disposeBag);

            viewModel.CurrentPageObservable
                .Subscribe(newPage => monthsPager.SetCurrentItem(newPage, false))
                .DisposedBy(disposeBag);

            viewModel.RowsInCurrentMonthObservable
                .Subscribe(onRowCountChanged)
                .DisposedBy(disposeBag);

            viewModel.SelectedDateRangeObservable
                .CombineLatest(viewModel.QuickSelectShortcutsObservable,
                    viewModel.MonthsObservable,
                    (selectedDateRange, shortcuts, months) => (selectedDateRange, shortcuts, months))
                .Subscribe(tuple => onDateRangeChanged(tuple.Item1, tuple.Item2, tuple.Item3))
                .DisposedBy(disposeBag);

            viewModel.CurrentMonthObservable
                .Select(calendarMonth => calendarMonth.ToDateTime())
                .Select(dateTime => dateTime.ToString(DateFormatCultureInfo.CurrentCulture.DateTimeFormat.YearMonthPattern))
                .Subscribe(monthYear.Rx().TextObserver())
                .DisposedBy(disposeBag);

            var shortcutsAdapter = new ReportsCalendarShortcutAdapter();

            shortcutsAdapter.ItemTapObservable
                .Subscribe(viewModel.SelectShortcut.Inputs)
                .DisposedBy(disposeBag);

            shortcutsRecyclerView.SetAdapter(shortcutsAdapter);

            viewModel.QuickSelectShortcutsObservable
                .Subscribe(shortcutsAdapter.Rx().Items())
                .DisposedBy(disposeBag);

            viewModel.SelectedDateRangeObservable
                .Subscribe(shortcutsAdapter.UpdateSelectedShortcut)
                .DisposedBy(disposeBag);

            viewModel.HighlightedDateRangeObservable
                .Subscribe(calendarPagesAdapter.UpdateSelectedRange)
                .DisposedBy(disposeBag);
        }

        private void setupWeekdaysLabels(IReadOnlyList<string> dayHeaders)
        {
            daysHeader
                .GetChildren<TextView>()
                .Indexed()
                .ForEach((textView, index)
                    => textView.Text = dayHeaders[index]);
        }

        private void onDateRangeChanged(ReportsDateRangeParameter dateRange, IImmutableList<ReportsCalendarBaseQuickSelectShortcut> shortcuts, IImmutableList<ReportsCalendarPageViewModel> months)
        {
            var dateRangeStartDate = dateRange.StartDate;
            var monthToScroll = months.IndexOf(month =>
                month.CalendarMonth.Month == dateRangeStartDate.Month
                && month.CalendarMonth.Year == dateRangeStartDate.Year);
            if (monthToScroll == monthsPager.CurrentItem) return;

            var dateRangeStartDateIsContainedInCurrentMonthView =
                months[monthsPager.CurrentItem]
                .Days.Any(day => day.DateTimeOffset == dateRangeStartDate);

            if (dateRangeStartDateIsContainedInCurrentMonthView
            && dateRangeStartDate.Month != dateRange.EndDate.Month)
                return;

            monthsPager.Post(() => monthsPager.SetCurrentItem(monthToScroll, true));
        }

        private void onRowCountChanged(int rowsInCurrentMonth)
        {
            if (currentRowCount == rowsInCurrentMonth)
                return;

            currentRowCount = rowsInCurrentMonth;
            recalculatePagerHeight();
        }

        private void recalculatePagerHeight()
        {
            var layoutParams = monthsPager.LayoutParameters;
            layoutParams.Height = rowHeight * currentRowCount;
            monthsPager.LayoutParameters = layoutParams;
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (!disposing || disposeBag == null) return;

            disposeBag.Dispose();
        }
    }
}
