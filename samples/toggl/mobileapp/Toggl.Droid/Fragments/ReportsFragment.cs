using Android.OS;
using Android.Views;
using System;
using System.Reactive.Linq;
using Toggl.Core.UI.Extensions;
using Toggl.Core.UI.ViewModels.Reports;
using Toggl.Droid.Adapters;
using Toggl.Droid.Extensions;
using Toggl.Droid.Extensions.Reactive;
using Toggl.Droid.LayoutManagers;
using Toggl.Droid.Presentation;
using Toggl.Droid.ViewHelpers;
using Toggl.Shared.Extensions;

namespace Toggl.Droid.Fragments
{
    public sealed partial class ReportsFragment : ReactiveTabFragment<ReportsViewModel>, IScrollableToStart
    {
        private static readonly TimeSpan toggleCalendarThrottleDuration = TimeSpan.FromMilliseconds(300);
        private ReportsRecyclerAdapter reportsRecyclerAdapter;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var view = inflater.Inflate(Resource.Layout.ReportsFragment, container, false);
            InitializeViews(view);
            SetupToolbar(view);
            reportsRecyclerView.AttachMaterialScrollBehaviour(appBarLayout);

            return view;
        }

        public override void OnViewCreated(View view, Bundle savedInstanceState)
        {
            base.OnViewCreated(view, savedInstanceState);
            ViewModel?.CalendarViewModel.AttachView(this);

            selectWorkspaceFab.Rx().Tap()
                .Subscribe(ViewModel.SelectWorkspace.Inputs)
                .DisposedBy(DisposeBag);

            setupReportsRecyclerView();
            ViewModel.StartDate.CombineLatest(
                    ViewModel.EndDate,
                    ViewModel.WorkspaceHasBillableFeatureEnabled,
                    ViewModel.BarChartViewModel.DateFormat,
                    ViewModel.BarChartViewModel.Bars,
                    ViewModel.BarChartViewModel.MaximumHoursPerBar,
                    ViewModel.BarChartViewModel.HorizontalLegend,
                    BarChartData.Create)
                .Subscribe(reportsRecyclerAdapter.UpdateBarChart)
                .DisposedBy(DisposeBag);

            ViewModel.WorkspaceNameObservable
                .Subscribe(reportsRecyclerAdapter.UpdateWorkspaceName)
                .DisposedBy(DisposeBag);

            ViewModel.SegmentsObservable.CombineLatest(
                    ViewModel.ShowEmptyStateObservable,
                    ViewModel.TotalTimeObservable,
                    ViewModel.TotalTimeIsZeroObservable,
                    ViewModel.BillablePercentageObservable,
                    ViewModel.DurationFormatObservable,
                    ReportsSummaryData.Create)
                .Subscribe(reportsRecyclerAdapter.UpdateReportsSummary)
                .DisposedBy(DisposeBag);

            toolbarCurrentDateRangeText.Rx().Tap()
                .Subscribe(showCalendar)
                .DisposedBy(DisposeBag);

            ViewModel.CurrentDateRange
                .Subscribe(toolbarCurrentDateRangeText.Rx().TextObserver())
                .DisposedBy(DisposeBag);
        }

        public override void OnStart()
        {
            base.OnStart();
            ViewModel?.CalendarViewModel.ViewAppearing();
        }

        public override void OnResume()
        {
            base.OnResume();

            if (IsHidden) return;

            ViewModel?.CalendarViewModel.ViewAppeared();
        }

        public override void OnStop()
        {
            base.OnStop();
            ViewModel?.CalendarViewModel.ViewDisappeared();
        }

        public override void OnDestroy()
        {
            ViewModel?.CalendarViewModel.DetachView();
            base.OnDestroy();
        }

        public override void OnHiddenChanged(bool hidden)
        {
            base.OnHiddenChanged(hidden);
            if (hidden)
                ViewModel.CalendarViewModel.ViewDisappeared();
            else
                ViewModel.CalendarViewModel.ViewAppeared();
        }

        public void ScrollToStart()
        {
            reportsRecyclerView?.SmoothScrollToPosition(0);
        }

        private void setupReportsRecyclerView()
        {
            reportsRecyclerAdapter = new ReportsRecyclerAdapter(Context);
            reportsRecyclerView.SetLayoutManager(new UnpredictiveLinearLayoutManager(Context));
            reportsRecyclerView.SetAdapter(reportsRecyclerAdapter);
        }

        private void showCalendar()
        {
            AndroidDependencyContainer
                .Instance
                .ViewModelCache
                .Cache(ViewModel.CalendarViewModel);

            new ReportsCalendarFragment()
                .Show(ChildFragmentManager, nameof(ReportsCalendarFragment));
        }
    }
}
