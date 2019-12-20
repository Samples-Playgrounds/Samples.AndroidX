using CoreGraphics;
using Foundation;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reactive.Linq;
using Toggl.Core.UI.Extensions;
using Toggl.Core.UI.Helper;
using Toggl.Core.UI.ViewModels;
using Toggl.Core.UI.ViewModels.ReportsCalendar;
using Toggl.iOS.Extensions;
using Toggl.iOS.Extensions.Reactive;
using Toggl.iOS.ViewSources;
using Toggl.Shared.Extensions;
using UIKit;

namespace Toggl.iOS.ViewControllers
{
    public partial class ReportsCalendarViewController : ReactiveViewController<ReportsCalendarViewModel>, IUICollectionViewDelegate
    {
        private CGSize popoverPreferedSize = new CGSize(319, 355);
        private bool calendarInitialized;
        private List<ReportsCalendarPageViewModel> pendingMonthsUpdate;
        private ReportsCalendarCollectionViewSource calendarCollectionViewSource;

        public ReportsCalendarViewController(ReportsCalendarViewModel viewModel)
            : base(viewModel, nameof(ReportsCalendarViewController))
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            PreferredContentSize = popoverPreferedSize;

            calendarCollectionViewSource = new ReportsCalendarCollectionViewSource(CalendarCollectionView);
            var calendarCollectionViewLayout = new ReportsCalendarCollectionViewLayout();
            CalendarCollectionView.Delegate = calendarCollectionViewSource;
            CalendarCollectionView.DataSource = calendarCollectionViewSource;
            CalendarCollectionView.CollectionViewLayout = calendarCollectionViewLayout;

            var quickSelectCollectionViewSource = new ReportsCalendarQuickSelectCollectionViewSource(QuickSelectCollectionView);
            QuickSelectCollectionView.Source = quickSelectCollectionViewSource;

            ViewModel.DayHeadersObservable
                .Subscribe(setupDayHeaders)
                .DisposedBy(DisposeBag);

            ViewModel.MonthsObservable
                .Subscribe(calendarCollectionViewSource.UpdateMonths)
                .DisposedBy(DisposeBag);

            calendarCollectionViewSource.DayTaps
                .Subscribe(ViewModel.SelectDay.Inputs)
                .DisposedBy(DisposeBag);

            ViewModel.HighlightedDateRangeObservable
                .Subscribe(calendarCollectionViewSource.UpdateSelection)
                .DisposedBy(DisposeBag);

            ViewModel.CurrentMonthObservable
                .Select(month =>
                {
                    var dateTime = month.ToDateTime();
                    var pattern = DateFormatCultureInfo.CurrentCulture.DateTimeFormat.YearMonthPattern;

                    var yearMonthString = dateTime.ToString(pattern);

                    var year = month.Year.ToString();
                    var rangeStart = yearMonthString.IndexOf(year);
                    var rangeEnd = year.Length;
                    var range = new NSRange(rangeStart, rangeEnd);

                    var attributedString = new NSMutableAttributedString(
                        yearMonthString,
                        new UIStringAttributes { ForegroundColor = ColorAssets.Text });
                    attributedString.AddAttributes(
                        new UIStringAttributes { ForegroundColor = ColorAssets.Text2 },
                        range);

                    return attributedString;
                })
                .Subscribe(CurrentMonthLabel.Rx().AttributedText())
                .DisposedBy(DisposeBag);

            ViewModel.SelectedDateRangeObservable
                .Subscribe(quickSelectCollectionViewSource.UpdateSelection)
                .DisposedBy(DisposeBag);

            quickSelectCollectionViewSource.ShortcutTaps
                .Do(shortcut =>
                {
                    IosDependencyContainer.Instance.IntentDonationService.DonateShowReport(shortcut.Period);
                })
                .Subscribe(ViewModel.SelectShortcut.Inputs)
                .DisposedBy(DisposeBag);

            ViewModel.QuickSelectShortcutsObservable
                .Subscribe(quickSelectCollectionViewSource.UpdateShortcuts)
                .DisposedBy(DisposeBag);

            ViewModel.ReloadObservable
                .Select(_ => ViewModel.CurrentPage)
                .Subscribe(calendarCollectionViewSource.RefreshUIAtPage)
                .DisposedBy(DisposeBag);
        }

        public override void DidMoveToParentViewController(UIViewController parent)
        {
            base.DidMoveToParentViewController(parent);

            //The constraint isn't available before DidMoveToParentViewController

            var rowHeight = ReportsCalendarCollectionViewLayout.CellHeight;
            var additionalHeight = View.Bounds.Height - CalendarCollectionView.Bounds.Height;

            var heightConstraint = View
                .Superview
                .Constraints
                .Single(c => c.FirstAttribute == NSLayoutAttribute.Height);

            ViewModel.RowsInCurrentMonthObservable
                .Select(rows => rows * rowHeight + additionalHeight)
                .Subscribe(heightConstraint.Rx().ConstantAnimated())
                .DisposedBy(DisposeBag);
        }

        public override void ViewDidLayoutSubviews()
        {
            base.ViewDidLayoutSubviews();

            if (calendarInitialized) return;

            calendarCollectionViewSource.CurrentPageNotScrollingObservable
                .Subscribe(ViewModel.SetCurrentPage)
                .DisposedBy(DisposeBag);

            calendarCollectionViewSource.CurrentPageWhileScrollingObservable
                .Subscribe(ViewModel.UpdateMonth)
                .DisposedBy(DisposeBag);

            ViewModel.CurrentPageObservable
                .ObserveOn(IosDependencyContainer.Instance.SchedulerProvider.MainScheduler)
                .Subscribe(CalendarCollectionView.Rx().CurrentPageObserver())
                .DisposedBy(DisposeBag);

            calendarInitialized = true;
        }

        private void setupDayHeaders(IReadOnlyList<string> dayHeaders)
        {
            DayHeader0.Text = dayHeaders[0];
            DayHeader1.Text = dayHeaders[1];
            DayHeader2.Text = dayHeaders[2];
            DayHeader3.Text = dayHeaders[3];
            DayHeader4.Text = dayHeaders[4];
            DayHeader5.Text = dayHeaders[5];
            DayHeader6.Text = dayHeaders[6];
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (!disposing)
            {
                DisposeBag.Dispose();
            }
        }
    }
}

