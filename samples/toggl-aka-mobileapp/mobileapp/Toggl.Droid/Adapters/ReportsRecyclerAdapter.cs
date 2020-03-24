﻿using Android.Content;
using Android.Runtime;
using Android.Views;
using System;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using AndroidX.ConstraintLayout.Widget;
using AndroidX.RecyclerView.Widget;
using Toggl.Droid.Extensions;
using Toggl.Droid.ViewHelpers;
using Toggl.Droid.ViewHolders;
using Toggl.Droid.Views;

namespace Toggl.Droid.Adapters
{
    public sealed class ReportsRecyclerAdapter : RecyclerView.Adapter
    {
        private readonly int lastItemCellHeight;
        private readonly int normalItemCellHeight;

        private const int WorkspaceName = 0;
        private const int Header = 1;
        private const int Item = 2;

        private const int workspaceNameCellIndex = 0;
        private const int summaryCardCellIndex = 1;
        private const int headerItemsCount = 2;

        private BarChartData? currentBarChartData;
        private string currentWorkspaceName = String.Empty;

        private readonly ISubject<Unit> summaryCardClicks = new Subject<Unit>();

        private ReportsSummaryData currentReportsSummaryData = ReportsSummaryData.Empty();
        public IObservable<Unit> SummaryCardClicks => summaryCardClicks.AsObservable();

        public ReportsRecyclerAdapter(Context context)
        {
            lastItemCellHeight = 72.DpToPixels(context);
            normalItemCellHeight = 48.DpToPixels(context);
        }

        public ReportsRecyclerAdapter(IntPtr javaReference, JniHandleOwnership transfer)
            : base(javaReference, transfer)
        {
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            var layoutInflater = LayoutInflater.From(parent.Context);
            switch (viewType)
            {
                case WorkspaceName:
                    var workpaceNameCell = layoutInflater.Inflate(Resource.Layout.ReportsFragmentWorkspaceName, parent, false);
                    return new ReportsWorkspaceNameViewHolder(workpaceNameCell);

                case Header:
                    var headerCellView = layoutInflater.Inflate(Resource.Layout.ReportsFragmentHeader, parent, false);
                    var reportsHeaderCellViewHolder = new ReportsHeaderCellViewHolder(headerCellView);
                    reportsHeaderCellViewHolder.SummaryCardClicksSubject = summaryCardClicks;
                    return reportsHeaderCellViewHolder;

                case Item:
                    var itemCellView = layoutInflater.Inflate(Resource.Layout.ReportsFragmentItem, parent, false);
                    return new ReportsItemCellViewHolder(itemCellView, lastItemCellHeight, normalItemCellHeight);

                default:
                    throw new InvalidOperationException($"Invalid view type {viewType}");
            }
        }

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            switch (holder)
            {
                case ReportsItemCellViewHolder reportsViewHolder:
                    reportsViewHolder.Item = currentReportsSummaryData.Segments[position - headerItemsCount];
                    reportsViewHolder.IsLastItem = position == ItemCount - 1;
                    reportsViewHolder.RecalculateSize();
                    break;

                case ReportsWorkspaceNameViewHolder reportsWorkspaceHolder:
                    reportsWorkspaceHolder.Item = currentWorkspaceName;
                    break;

                case ReportsHeaderCellViewHolder reportsSummaryHolder:
                    reportsSummaryHolder.Item = currentReportsSummaryData;
                    if (currentBarChartData.HasValue)
                    {
                        var barChartView = holder.ItemView.FindViewById<BarChartView>(Resource.Id.BarChartView);
                        var barChartTopLegendGroup = holder.ItemView.FindViewById<Group>(Resource.Id.WorkspaceBillableGroup);
                        var barChartData = currentBarChartData.GetValueOrDefault();
                        barChartView.BarChartData = barChartData;
                        barChartTopLegendGroup.Visibility = barChartData.WorkspaceIsBillable ? ViewStates.Visible : ViewStates.Gone;
                    }
                    break;

                default:
                    throw new InvalidOperationException($"Tried to bind unexpected viewholder {holder?.GetType().Name ?? "null"}");
            }
        }

        public override int ItemCount
            => headerItemsCount + currentReportsSummaryData.Segments.Count;

        public override int GetItemViewType(int position)
        {
            switch (position)
            {
                case workspaceNameCellIndex:
                    return WorkspaceName;

                case summaryCardCellIndex:
                    return Header;

                default:
                    return Item;
            }
        }

        public void UpdateBarChart(BarChartData barChartData)
        {
            currentBarChartData = barChartData;
            NotifyItemChanged(summaryCardCellIndex);
        }

        public void UpdateWorkspaceName(string workspaceName)
        {
            currentWorkspaceName = workspaceName;
            NotifyItemChanged(workspaceNameCellIndex);
        }

        public void UpdateReportsSummary(ReportsSummaryData reportsSummaryData)
        {
            currentReportsSummaryData = reportsSummaryData;
            NotifyDataSetChanged();
        }
    }
}
