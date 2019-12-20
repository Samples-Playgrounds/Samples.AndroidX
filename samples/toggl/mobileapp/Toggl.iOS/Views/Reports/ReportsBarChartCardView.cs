using Foundation;
using ObjCRuntime;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using Toggl.Core.Conversions;
using Toggl.Core.UI.Helper;
using Toggl.Core.UI.ViewModels.Reports;
using Toggl.iOS.Cells;
using Toggl.iOS.Extensions;
using Toggl.iOS.Extensions.Reactive;
using Toggl.Shared;
using Toggl.Shared.Extensions;
using UIKit;

namespace Toggl.iOS.Views.Reports
{
    [Register(nameof(ReportsBarChartCardView))]
    public partial class ReportsBarChartCardView : BaseReportsCardView<ReportsViewModel>
    {
        private const float barChartSpacingProportion = 0.3f;

        private readonly CompositeDisposable disposeBag = new CompositeDisposable();
        private readonly ISubject<Unit> updateLayout = new BehaviorSubject<Unit>(Unit.Default);

        public ReportsBarChartCardView(IntPtr handle) : base(handle)
        {
            // Note: this .ctor should not contain any initialization logic.
        }

        public static ReportsBarChartCardView CreateFromNib()
        {
            var arr = NSBundle.MainBundle.LoadNib(nameof(ReportsBarChartCardView), null, null);
            return Runtime.GetNSObject<ReportsBarChartCardView>(arr.ValueAt(0));
        }

        public override void AwakeFromNib()
        {
            base.AwakeFromNib();

            ClockedHoursTitleLabel.Text = Resources.ClockedHours.ToUpper();
            BillableLegendLabel.Text = Resources.Billable.ToUpper();
            NonBillableLegendLabel.Text = Resources.NonBillable.ToUpper();
            ZeroHoursLabel.Text = $"0 {Resources.UnitHour}";

            prepareViews();
        }

        protected override void UpdateViewBinding()
        {
            // Bar chart
            Item.WorkspaceHasBillableFeatureEnabled
                .Subscribe(ColorsLegendContainerView.Rx().IsVisible())
                .DisposedBy(disposeBag);

            Item.StartDate
                .CombineLatest(
                    Item.BarChartViewModel.DateFormat,
                    (startDate, format) => startDate.ToString(format.Short, DateFormatCultureInfo.CurrentCulture))
                .Subscribe(StartDateLabel.Rx().Text())
                .DisposedBy(disposeBag);

            Item.EndDate
                .CombineLatest(
                    Item.BarChartViewModel.DateFormat,
                    (endDate, format) => endDate.ToString(format.Short, DateFormatCultureInfo.CurrentCulture))
                .Subscribe(EndDateLabel.Rx().Text())
                .DisposedBy(disposeBag);

            Item.BarChartViewModel.MaximumHoursPerBar
                .Select(hours => $"{hours} {Resources.UnitHour}")
                .Subscribe(MaximumHoursLabel.Rx().Text())
                .DisposedBy(disposeBag);

            Item.BarChartViewModel.MaximumHoursPerBar
                .Select(hours => $"{hours / 2} {Resources.UnitHour}")
                .Subscribe(HalfHoursLabel.Rx().Text())
                .DisposedBy(disposeBag);

            Item.BarChartViewModel.HorizontalLegend
                .Where(legend => legend == null)
                .Subscribe(_ =>
                {
                    HorizontalLegendStackView.Subviews.ForEach(subview => subview.RemoveFromSuperview());
                    StartDateLabel.Hidden = false;
                    EndDateLabel.Hidden = false;
                })
                .DisposedBy(disposeBag);

            Item.BarChartViewModel.HorizontalLegend
                .Where(legend => legend != null)
                .CombineLatest(Item.BarChartViewModel.DateFormat, createHorizontalLegendLabels)
                .Do(_ =>
                {
                    StartDateLabel.Hidden = true;
                    EndDateLabel.Hidden = true;
                })
                .Subscribe(HorizontalLegendStackView.Rx().ArrangedViews())
                .DisposedBy(disposeBag);

            Item.BarChartViewModel.Bars
                .Select(createBarViews)
                .Subscribe(BarsStackView.Rx().ArrangedViews())
                .DisposedBy(disposeBag);

            var spacingObservable = Item.BarChartViewModel.Bars
                .CombineLatest(updateLayout, (bars, _) => bars)
                .Select(bars => BarsStackView.Frame.Width / bars.Count * barChartSpacingProportion);

            spacingObservable
                .Subscribe(BarsStackView.Rx().Spacing())
                .DisposedBy(disposeBag);

            spacingObservable
                .Subscribe(HorizontalLegendStackView.Rx().Spacing())
                .DisposedBy(disposeBag);
        }

        public override void LayoutSubviews()
        {
            base.LayoutSubviews();

            if (Hidden)
            {
                return;
            }

            updateLayout.OnNext(Unit.Default);
        }

        private void prepareViews()
        {
            PrepareCard(BarChartCardView);

            ClockedHoursTitleLabel.SetKerning(-0.2);
            BillableLegendLabel.SetKerning(-0.2);
            NonBillableLegendLabel.SetKerning(-0.2);
        }

        private IEnumerable<UIView> createBarViews(IEnumerable<BarViewModel> bars)
            => bars.Select<BarViewModel, UIView>(bar =>
            {
                if (bar.NonBillablePercent == 0 && bar.BillablePercent == 0)
                {
                    return new EmptyBarView();
                }

                return new BarView(bar);
            });

        private IEnumerable<UILabel> createHorizontalLegendLabels(IEnumerable<DateTimeOffset> dates, DateFormat format)
            => dates.Select(date =>
                new BarLegendLabel(
                    DateTimeOffsetConversion.ToDayOfWeekInitial(date),
                    date.ToString(format.Short, DateFormatCultureInfo.CurrentCulture)));
    }
}
