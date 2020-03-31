using System;
using System.Linq;
using Foundation;
using Toggl.Core.Analytics;
using CoreGraphics;
using System.Collections.Immutable;
using System.Reactive.Linq;
using Toggl.Core.UI.Helper;
using Toggl.Core.UI.ViewModels.Calendar;
using Toggl.iOS.Extensions;
using Toggl.iOS.Extensions.Reactive;
using Toggl.iOS.ViewSources;
using Toggl.Shared.Extensions;
using Toggl.Shared.Extensions.Reactive;
using UIKit;

namespace Toggl.iOS.ViewControllers
{
    public sealed partial class CalendarViewController : ReactiveViewController<CalendarViewModel>, IUIPageViewControllerDataSource, IUIPageViewControllerDelegate
    {
        private const int weekViewHeight = 44;
        private const int maxAllowedPageIndex = 0;
        private const int minAllowedPageIndex = -13;
        private const int weekViewHeaderFontSize = 12;

        private readonly BehaviorRelay<bool> contextualMenuVisible = new BehaviorRelay<bool>(false);
        private readonly BehaviorRelay<string> timeTrackedOnDay = new BehaviorRelay<string>("");
        private readonly BehaviorRelay<int> currentPageRelay = new BehaviorRelay<int>(0);
        private readonly UIPageViewController pageViewController;
        private readonly UILabel[] weekViewHeaderLabels;
        private readonly UICollectionViewFlowLayout weekViewCollectionViewLayout;

        private CalendarWeeklyViewDayCollectionViewSource weekViewCollectionViewSource;
        private DateTime currentlyShownDate;

        public CalendarViewController(CalendarViewModel calendarViewModel)
            : base(calendarViewModel, nameof(CalendarViewController))
        {
            pageViewController = new UIPageViewController(UIPageViewControllerTransitionStyle.Scroll, UIPageViewControllerNavigationOrientation.Horizontal);
            weekViewHeaderLabels = Enumerable.Range(0, 7)
                .Select(_ => new UILabel
                {
                    TextAlignment = UITextAlignment.Center,
                    TranslatesAutoresizingMaskIntoConstraints = false,
                    Font = UIFont.SystemFontOfSize(weekViewHeaderFontSize, UIFontWeight.Medium),
                    TextColor = ColorAssets.CalendarHeaderLabel
                })
                .ToArray();

            weekViewCollectionViewLayout= new UICollectionViewFlowLayout
            {
                ScrollDirection = UICollectionViewScrollDirection.Horizontal,
                MinimumLineSpacing = 0
            };
        }

        public override void LoadView()
        {
            base.LoadView();
            setupWeekViewHeaderLabels();
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            ExtendedLayoutIncludesOpaqueBars = true;

            weekViewCollectionViewSource = new CalendarWeeklyViewDayCollectionViewSource(WeekViewCollectionView);
            currentlyShownDate = ViewModel.CurrentlyShownDate.Value;

            setupViews();

            ViewModel.WeekViewDays
                .Subscribe(weekViewCollectionViewSource.UpdateItems)
                .DisposedBy(DisposeBag);

            ViewModel.WeekViewHeaders
                .Subscribe(updateWeeklyViewHeaderLabelTexts)
                .DisposedBy(DisposeBag);

            weekViewCollectionViewSource.DaySelected
                .Subscribe(ViewModel.SelectDayFromWeekView.Inputs)
                .DisposedBy(DisposeBag);

            ViewModel.CurrentlyShownDate
                .Subscribe(weekViewCollectionViewSource.UpdateCurrentlySelectedDate)
                .DisposedBy(DisposeBag);

            ViewModel.CurrentlyShownDate
                .Subscribe(updateCurrentlyShownViewController)
                .DisposedBy(DisposeBag);

            ViewModel.CurrentlyShownDateString
                .Subscribe(SelectedDateLabel.Rx().Text())
                .DisposedBy(DisposeBag);

            SettingsButton.Rx()
                .BindAction(ViewModel.OpenSettings)
                .DisposedBy(DisposeBag);

            contextualMenuVisible
                .Select(CommonFunctions.Invert)
                .Subscribe(setPageViewControllerEnabled)
                .DisposedBy(DisposeBag);

            contextualMenuVisible
                .Subscribe(contextualMenuVisible => WeekViewCollectionView.UserInteractionEnabled = !contextualMenuVisible)
                .DisposedBy(DisposeBag);

            contextualMenuVisible
                .Subscribe(toggleTabBar)
                .DisposedBy(DisposeBag);

            timeTrackedOnDay
                .Subscribe(DailyTrackedTimeLabel.Rx().Text())
                .DisposedBy(DisposeBag);
        }

        private void toggleTabBar(bool hidden)
        {
            TabBarController.TabBar.Hidden = hidden;
        }

        private void setupViews()
        {
            DailyTrackedTimeLabel.Font = DailyTrackedTimeLabel.Font.GetMonospacedDigitFont();

            pageViewController.DataSource = this;
            pageViewController.Delegate = this;
            pageViewController.View.Frame = DayViewContainer.Bounds;
            DayViewContainer.AddSubview(pageViewController.View);
            pageViewController.DidMoveToParentViewController(this);

            var viewControllers = new[] { viewControllerAtIndex(0) };
            viewControllers[0].SetGoodScrollPoint();
            pageViewController.SetViewControllers(viewControllers, UIPageViewControllerNavigationDirection.Forward, false, null);

            WeekViewCollectionView.Source = weekViewCollectionViewSource;
            WeekViewCollectionView.ShowsHorizontalScrollIndicator = false;
            WeekViewCollectionView.CollectionViewLayout = weekViewCollectionViewLayout;
            WeekViewCollectionView.DecelerationRate = UIScrollView.DecelerationRateFast;
        }

        private void setPageViewControllerEnabled(bool enabled)
        {
            pageViewController.DataSource = enabled ? this : null;
            pageViewController.Delegate = enabled ? this : null;
        }

        private void updateCurrentlyShownViewController(DateTime newDate)
        {
            if (newDate == currentlyShownDate)
                return;

            var direction = newDate > currentlyShownDate
                ? UIPageViewControllerNavigationDirection.Forward
                : UIPageViewControllerNavigationDirection.Reverse;
            currentlyShownDate = newDate;

            var today = IosDependencyContainer.Instance.TimeService.CurrentDateTime.ToLocalTime().Date;
            var index = (newDate - today).Days;

            var currentViewController = pageViewController.ViewControllers[0] as CalendarDayViewController;
            var newViewController = viewControllerAtIndex(index);
            if (currentViewController != null)
                newViewController.SetScrollOffset(currentViewController.ScrollOffset);
            pageViewController.SetViewControllers(new[] {newViewController},  direction, true, null);
            currentPageRelay.Accept(index);
        }

        public override void ViewDidLayoutSubviews()
        {
            base.ViewDidLayoutSubviews();

            updateWeekViewHeaderWidthConstraints();
            weekViewCollectionViewSource.UpdateCurrentlySelectedDate(ViewModel.CurrentlyShownDate.Value);
            ViewModel.RealoadWeekView();
        }

        public UIViewController GetPreviousViewController(UIPageViewController pageViewController, UIViewController referenceViewController)
        {
            var referenceTag = referenceViewController.View.Tag;
            if (referenceTag == minAllowedPageIndex)
                return null;

            return viewControllerAtIndex(referenceTag - 1);
        }

        public UIViewController GetNextViewController(UIPageViewController pageViewController, UIViewController referenceViewController)
        {
            var referenceTag = referenceViewController.View.Tag;
            if (referenceTag == maxAllowedPageIndex)
                return null;

            return viewControllerAtIndex(referenceTag + 1);
        }

        public override void ViewWillTransitionToSize(CGSize toSize, IUIViewControllerTransitionCoordinator coordinator)
        {
            pageViewController.ViewControllers.ForEach(viewController => viewController.ViewWillTransitionToSize(toSize, coordinator));
        }

        [Export("pageViewController:willTransitionToViewControllers:")]
        public void WillTransition(UIPageViewController pageViewController, UIViewController[] pendingViewControllers)
        {
            var pendingCalendarDayViewController = pendingViewControllers.FirstOrDefault() as CalendarDayViewController;
            if (pendingCalendarDayViewController == null)
                return;

            var currentCalendarDayViewController = pageViewController.ViewControllers.FirstOrDefault() as CalendarDayViewController;
            if (currentCalendarDayViewController == null) return;

            pendingCalendarDayViewController.SetScrollOffset(currentCalendarDayViewController.ScrollOffset);
        }

        private CalendarDayViewController viewControllerAtIndex(nint index)
        {
            var viewModel = ViewModel.DayViewModelAt((int) index);
            var viewController = new CalendarDayViewController(viewModel, currentPageRelay, timeTrackedOnDay, contextualMenuVisible);
            viewController.View.Tag = index;
            return viewController;
        }

        [Export("pageViewController:didFinishAnimating:previousViewControllers:transitionCompleted:")]
        public void DidFinishAnimating(UIPageViewController pageViewController, bool finished, UIViewController[] previousViewControllers, bool completed)
        {
            if (!completed) return;

            var newIndex = pageViewController.ViewControllers.FirstOrDefault()?.View?.Tag;
            if (newIndex == null) return;

            var newDate = ViewModel.IndexToDate((int)newIndex.Value);

            currentlyShownDate = newDate;
            currentPageRelay.Accept((int)newIndex);
            ViewModel.CurrentlyShownDate.Accept(newDate);

            var previousIndex = previousViewControllers.FirstOrDefault()?.View?.Tag;
            if (previousIndex == null) return;
            var swipeDirection = previousIndex > newIndex
                ? CalendarSwipeDirection.Left
                : CalendarSwipeDirection.Rignt;
            var daysSinceToday = (int)newIndex.Value;
            var dayOfWeek = ViewModel.IndexToDate((int)newIndex.Value).DayOfWeek.ToString();
            IosDependencyContainer.Instance.AnalyticsService.CalendarSingleSwipe.Track(swipeDirection, daysSinceToday, dayOfWeek);
        }

        private void setupWeekViewHeaderLabels()
        {
            foreach (var dayHeader in weekViewHeaderLabels)
            {
                WeekViewDayHeaderContainer.AddSubview(dayHeader);
                dayHeader.TopAnchor.ConstraintEqualTo(WeekViewDayHeaderContainer.TopAnchor).Active = true;
                dayHeader.BottomAnchor.ConstraintEqualTo(WeekViewDayHeaderContainer.BottomAnchor).Active = true;
            }

            weekViewHeaderLabels.First().LeadingAnchor.ConstraintEqualTo(WeekViewDayHeaderContainer.LeadingAnchor).Active = true;
            weekViewHeaderLabels.Last().TrailingAnchor.ConstraintEqualTo(WeekViewDayHeaderContainer.TrailingAnchor).Active = true;

            for (int i = 1; i < 7; i++)
            {
                var previousDayHeader = weekViewHeaderLabels[i - 1];
                var currentDayHeader = weekViewHeaderLabels[i];
                previousDayHeader.TrailingAnchor.ConstraintEqualTo(currentDayHeader.LeadingAnchor).Active = true;
            }
        }

        private void updateWeekViewHeaderWidthConstraints()
        {
            var targetWidth = WeekViewContainer.Frame.Width / 7;
            weekViewCollectionViewLayout.ItemSize = new CGSize(targetWidth, weekViewHeight);
            foreach (var label in weekViewHeaderLabels)
            {
                var widthConstraint = label.Constraints.FirstOrDefault(constraint => constraint.FirstAttribute == NSLayoutAttribute.Width);
                if (widthConstraint == null)
                {
                    label.WidthAnchor.ConstraintEqualTo(targetWidth).Active = true;
                    continue;
                }

                widthConstraint.Constant = targetWidth;
                label.SetNeedsLayout();
            }
        }

        private void updateWeeklyViewHeaderLabelTexts(IImmutableList<DayOfWeek> headers)
        {
            if (headers.Count != 7)
                throw new ArgumentException($"The count {nameof(headers)} must be 7 (it was {headers.Count})");

            for (int i = 0; i < 7; i++)
                weekViewHeaderLabels[i].Text = textForDayHeader(headers[i]);
        }

        private string textForDayHeader(DayOfWeek dayOfWeek)
            => TraitCollection.HorizontalSizeClass == UIUserInterfaceSizeClass.Regular
                ? dayOfWeek.FullName()
                : dayOfWeek.Initial();

    }
}
