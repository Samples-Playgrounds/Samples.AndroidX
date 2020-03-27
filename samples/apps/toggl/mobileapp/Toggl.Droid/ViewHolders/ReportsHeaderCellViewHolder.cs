using Android.App;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.Runtime;
using Android.Text;
using Android.Text.Style;
using Android.Views;
using Android.Widget;
using System;
using System.Reactive;
using System.Reactive.Subjects;
using AndroidX.CardView.Widget;
using Toggl.Droid.Extensions;
using Toggl.Droid.ViewHelpers;
using Toggl.Droid.Views;
using Toggl.Shared;
using Toggl.Shared.Extensions;
using Color = Android.Graphics.Color;

namespace Toggl.Droid.ViewHolders
{
    public class ReportsHeaderCellViewHolder : BaseRecyclerViewHolder<ReportsSummaryData>
    {
        private static readonly Color totalTimeText;
        private static readonly Color disabledReportFeature;
        private static readonly Color billablePercentageText;

        private CardView summaryCard;
        private TextView reportsSummaryTotal;
        private TextView reportsSummaryTotalLabel;
        private ImageView reportsTotalChartImageView;
        private TextView reportsSummaryBillable;
        private TextView reportsSummaryBillableLabel;
        private View billablePercentageView;

        private TextView clockedHoursLabel;
        private TextView billableTextLabel;
        private TextView nonBillableTextLabel;

        private CardView pieChartCard;
        private PieChartView pieChartView;

        private LinearLayout emptyStateView;
        private TextView emptyStateTitle;
        private TextView emptyStateMessage;

        private Drawable reportsTotalChartImageDrawable;

        public ISubject<Unit> SummaryCardClicksSubject { get; set; }

        static ReportsHeaderCellViewHolder()
        {
            totalTimeText = Application.Context.SafeGetColor(Resource.Color.totalTimeText);
            disabledReportFeature = Application.Context.SafeGetColor(Resource.Color.disabledReportFeature);
            billablePercentageText = Application.Context.SafeGetColor(Resource.Color.billablePercentageText);
        }

        public ReportsHeaderCellViewHolder(View itemView)
            : base(itemView)
        {
        }

        public ReportsHeaderCellViewHolder(IntPtr handle, JniHandleOwnership ownership)
            : base(handle, ownership)
        {

        }

        protected override void InitializeViews()
        {
            summaryCard = ItemView.FindViewById<CardView>(Resource.Id.SummaryCard);
            reportsSummaryTotal = ItemView.FindViewById<TextView>(Resource.Id.ReportsSummaryTotal);
            reportsSummaryTotalLabel = ItemView.FindViewById<TextView>(Resource.Id.ReportsSummaryTotalLabel);
            reportsTotalChartImageView = ItemView.FindViewById<ImageView>(Resource.Id.ReportsTotalChartImageView);
            reportsTotalChartImageDrawable = reportsTotalChartImageView.Drawable;
            reportsSummaryBillable = ItemView.FindViewById<TextView>(Resource.Id.ReportsSummaryBillable);
            reportsSummaryBillableLabel = ItemView.FindViewById<TextView>(Resource.Id.ReportsSummaryBillableLabel);
            billablePercentageView = ItemView.FindViewById(Resource.Id.BillablePercentageView);
            clockedHoursLabel = ItemView.FindViewById<TextView>(Resource.Id.ClockedHours);
            billableTextLabel = ItemView.FindViewById<TextView>(Resource.Id.BillableText);
            nonBillableTextLabel = ItemView.FindViewById<TextView>(Resource.Id.NonBillableText);
            pieChartCard = ItemView.FindViewById<CardView>(Resource.Id.PieChartCard);
            pieChartView = ItemView.FindViewById<PieChartView>(Resource.Id.PieChartView);
            emptyStateView = ItemView.FindViewById<LinearLayout>(Resource.Id.EmptyStateView);
            emptyStateTitle = ItemView.FindViewById<TextView>(Resource.Id.EmptyStateTitle);
            emptyStateMessage = ItemView.FindViewById<TextView>(Resource.Id.EmptyStateMessage);

            summaryCard.Click += hideCalendar;

            reportsSummaryTotalLabel.Text = Resources.Total;
            reportsSummaryBillableLabel.Text = Resources.Billable;
            clockedHoursLabel.Text = Resources.ClockedHours;
            billableTextLabel.Text = Resources.Billable;
            nonBillableTextLabel.Text = Resources.NonBillable;
            emptyStateTitle.Text = Resources.ReportsEmptyStateTitle;
            emptyStateMessage.Text = Resources.ReportsEmptyStateDescription;
        }

        protected override void UpdateView()
        {
            reportsSummaryTotal.TextFormatted = convertReportTimeSpanToDurationString(Item.TotalTime, Item.DurationFormat);
            reportsTotalChartImageDrawable.SetColorFilter(Item.TotalTimeIsZero ? disabledReportFeature : totalTimeText, PorterDuff.Mode.SrcIn);
            reportsSummaryBillable.TextFormatted = convertBillablePercentageToSpannable(Item.BillablePercentage);
            updateBillablePercentageViewWidth();
            pieChartCard.Visibility = (!Item.ShowEmptyState).ToVisibility();
            pieChartView.Segments = Item.Segments;
            emptyStateView.Visibility = Item.ShowEmptyState.ToVisibility();
        }

        private void updateBillablePercentageViewWidth()
        {
            billablePercentageView.Post(() =>
            {
                if (billablePercentageView.Parent == null) return;

                var percentage = Item.BillablePercentage;
                var availableWidth = ((View)billablePercentageView.Parent).Width;
                var targetWidth = (availableWidth / 100.0f) * percentage;

                var layoutParams = billablePercentageView.LayoutParameters;
                layoutParams.Width = (int)targetWidth;
                billablePercentageView.LayoutParameters = layoutParams;
            });
        }

        public ISpannable convertReportTimeSpanToDurationString(TimeSpan timeSpan, DurationFormat durationFormat)
        {
            var timeString = timeSpan.ToFormattedString(durationFormat);

            var emphasizedPartLength = durationFormat == DurationFormat.Improved
                ? timeString.Length - 3
                : timeString.Length;

            var isDisabled = timeSpan.Ticks == 0;

            return selectTimeSpanEnabledStateSpan(timeString, emphasizedPartLength, isDisabled);
        }

        private ISpannable selectTimeSpanEnabledStateSpan(string timeString, int emphasizedPartLength, bool isDisabled)
        {
            var color = isDisabled ? disabledReportFeature : totalTimeText;

            var spannable = new SpannableString(timeString);
            spannable.SetSpan(new TypefaceSpan("sans-serif"), 0, emphasizedPartLength, SpanTypes.InclusiveInclusive);
            if (emphasizedPartLength < timeString.Length)
            {
                spannable.SetSpan(new TypefaceSpan("sans-serif-light"), emphasizedPartLength, timeString.Length, SpanTypes.InclusiveInclusive);
            }
            spannable.SetSpan(new ForegroundColorSpan(color), 0, spannable.Length(), SpanTypes.InclusiveInclusive);

            return spannable;
        }

        public ISpannable convertBillablePercentageToSpannable(float? value)
        {
            var isDisabled = value == null;
            var actualValue = isDisabled ? 0 : value.Value;

            var percentage = $"{actualValue:0.00}%";

            return selectBillablePercentageEnabledStateSpan(percentage, isDisabled);
        }

        private ISpannable selectBillablePercentageEnabledStateSpan(string percentage, bool isDisabled)
        {
            var color = isDisabled ? disabledReportFeature : billablePercentageText;
            var spannable = new SpannableString(percentage);
            spannable.SetSpan(new ForegroundColorSpan(color), 0, spannable.Length(), SpanTypes.ExclusiveExclusive);

            return spannable;
        }

        private void hideCalendar(object sender, EventArgs args)
        {
            SummaryCardClicksSubject?.OnNext(Unit.Default);
        }
    }
}
