using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using AndroidX.RecyclerView.Widget;
using AndroidX.ViewPager.Widget;
using Toggl.Core.UI.Parameters;
using Toggl.Core.UI.ViewModels.ReportsCalendar;
using Toggl.Droid.Views;
using Toggl.Shared.Extensions;
using Object = Java.Lang.Object;

namespace Toggl.Droid.Adapters
{
    public sealed class ReportsCalendarPagerAdapter : PagerAdapter
    {
        private Dictionary<int, CompositeDisposable> disposableBags = new Dictionary<int, CompositeDisposable>();

        private readonly Context context;
        private readonly RecyclerView.RecycledViewPool recyclerviewPool = new RecyclerView.RecycledViewPool();
        private IImmutableList<ReportsCalendarPageViewModel> currentMonths = ImmutableList<ReportsCalendarPageViewModel>.Empty;
        private Subject<ReportsCalendarDayViewModel> dayTaps = new Subject<ReportsCalendarDayViewModel>();
        private Subject<ReportsDateRangeParameter> selectionChanges = new Subject<ReportsDateRangeParameter>();
        private ReportsDateRangeParameter currentDateRange;
        private Handler mainHandler;

        public IObservable<ReportsCalendarDayViewModel> DayTaps => dayTaps.AsObservable();

        public ReportsCalendarPagerAdapter(Context context)
        {
            this.context = context;
            mainHandler = new Handler(Looper.MainLooper);
        }

        public ReportsCalendarPagerAdapter(IntPtr javaReference, JniHandleOwnership transfer)
            : base(javaReference, transfer)
        {
        }

        private Dictionary<int, ReportsCalendarRecyclerView> recyclerViews = new Dictionary<int, ReportsCalendarRecyclerView>();

        public override int Count => currentMonths.Count;

        public override Object InstantiateItem(ViewGroup container, int position)
        {
            var inflater = LayoutInflater.FromContext(context);
            var inflatedView = inflater.Inflate(Resource.Layout.ReportsCalendarFragmentPage, container, false);

            var calendarRecyclerView = (ReportsCalendarRecyclerView)inflatedView;
            calendarRecyclerView.SetRecycledViewPool(recyclerviewPool);
            calendarRecyclerView.SetLayoutManager(new ReportsCalendarLayoutManager(context));

            setupAdapter(calendarRecyclerView, position);

            recyclerViews[position] = calendarRecyclerView;

            container.AddView(inflatedView);

            return inflatedView;
        }

        private void setupAdapter(ReportsCalendarRecyclerView calendarRecyclerView, int position)
        {
            var adapter = new ReportsCalendarRecyclerAdapter(currentDateRange)
            {
                Items = currentMonths[position].Days
            };

            var disposeBag = new CompositeDisposable();
            disposableBags[position] = disposeBag;

            calendarRecyclerView.SetAdapter(adapter);

            adapter.ItemTapObservable
                .Subscribe(dayTaps.OnNext)
                .DisposedBy(disposeBag);

            selectionChanges
                .ObserveOn(AndroidDependencyContainer.Instance.SchedulerProvider.MainScheduler)
                .Subscribe(adapter.UpdateDateRangeParameter)
                .DisposedBy(disposeBag);
        }

        private void notifyPageContentAdapters()
        {
            foreach (var recyclerViewInfo in recyclerViews)
            {
                var position = recyclerViewInfo.Key;
                var recyclerView = recyclerViewInfo.Value;

                var adapter = recyclerView.GetAdapter() as ReportsCalendarRecyclerAdapter;
                adapter.Items = currentMonths[position].Days;
            }
        }

        private void disposeOfAdapterSubscriptions(int position)
        {
            if (disposableBags.TryGetValue(position, out var disposableBag))
            {
                disposableBag.Dispose();
                disposableBags.Remove(position);
            }
        }

        public override void DestroyItem(ViewGroup container, int position, Object @object)
        {
            disposeOfAdapterSubscriptions(position);

            recyclerViews.Remove(position);
            container.RemoveView(@object as View);
        }

        public override bool IsViewFromObject(View view, Object @object)
            => view == @object;

        public void UpdateMonths(IImmutableList<ReportsCalendarPageViewModel> newMonths)
        {
            currentMonths = newMonths;
            NotifyDataSetChanged();
            notifyPageContentAdapters();
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (!disposing) return;

            disposableBags.Values.ForEach(bag => bag.Dispose());
            disposableBags.Clear();

            recyclerViews.Clear();
        }

        public void UpdateSelectedRange(ReportsDateRangeParameter newDateRange)
        {
            currentDateRange = newDateRange;
            selectionChanges.OnNext(currentDateRange);
            NotifyDataSetChanged();
        }
    }
}
