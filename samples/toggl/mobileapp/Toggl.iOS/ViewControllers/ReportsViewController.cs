using CoreGraphics;
using Foundation;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using Toggl.Core.Models.Interfaces;
using Toggl.Core.Reports;
using Toggl.Core.UI.Extensions;
using Toggl.Core.UI.Helper;
using Toggl.Core.UI.ViewModels.Reports;
using Toggl.Core.UI.Views;
using Toggl.iOS.Extensions;
using Toggl.iOS.Extensions.Reactive;
using Toggl.iOS.Helper;
using Toggl.iOS.Presentation;
using Toggl.iOS.Views.Reports;
using Toggl.iOS.ViewSources;
using Toggl.Shared.Extensions;
using UIKit;
using static Toggl.iOS.Extensions.AnimationExtensions;

namespace Toggl.iOS.ViewControllers
{
    public sealed partial class ReportsViewController : ReactiveViewController<ReportsViewModel>, IScrollableToTop
    {
        private const string boundsKey = "bounds";

        private const double maximumWorkspaceNameLabelWidthCompact = 144;
        private const double maximumWorkspaceNameLabelWidthRegular = 288;

        private const double maxWidth = 834;

        private CGRect activityIndicatorCenteredFrame = new CGRect(80, 0.5, 40, 40);
        private CGRect activityIndicatorLeftAlignedFrame = new CGRect(-35, -5, 40, 40);

        private UIButton titleButton;
        private UIActivityIndicatorView activityIndicator;
        private bool calendarIsVisible;
        private bool alreadyLoadedCalendar;
        private ReportsTableViewSource source;
        private IDisposable calendarSizeDisposable;
        private ReportsCalendarViewController calendarViewController;
        private nfloat calendarHeight => CalendarContainer.Bounds.Height;
        private ISubject<Unit> viewDidAppearSubject = new Subject<Unit>();
        private ReportsOverviewCardView overview = ReportsOverviewCardView.CreateFromNib();
        private ReportsBarChartCardView barChart = ReportsBarChartCardView.CreateFromNib();

        public ReportsViewController(ReportsViewModel viewModel) : base(viewModel, nameof(ReportsViewController))
        {
        }

        public override void TraitCollectionDidChange(UITraitCollection previousTraitCollection)
        {
            base.TraitCollectionDidChange(previousTraitCollection);

            ReportsTableView.ReloadData();
        }

        public override void DidRotate(UIInterfaceOrientation fromInterfaceOrientation)
        {
            base.DidRotate(fromInterfaceOrientation);

            HideCalendar();
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            var separator = NavigationController.NavigationBar.InsertSeparator();
            separator.BackgroundColor = ColorAssets.OpaqueSeparator;

            calendarViewController = ViewControllerLocator.GetViewController(ViewModel.CalendarViewModel) as ReportsCalendarViewController;
            prepareViews();

            OverviewContainerView.AddSubview(overview);
            overview.Frame = OverviewContainerView.Bounds;
            overview.Item = ViewModel;
            BarChartsContainerView.AddSubview(barChart);
            barChart.Frame = BarChartsContainerView.Bounds;
            barChart.Item = ViewModel;

            calendarSizeDisposable = CalendarContainer.AddObserver(boundsKey, NSKeyValueObservingOptions.New, onCalendarSizeChanged);

            source = new ReportsTableViewSource(ReportsTableView, ViewModel);
            source.SetItems(ImmutableList<ChartSegment>.Empty);
            ReportsTableView.ReloadData();

            ViewModel.SegmentsObservable
                .Subscribe(ReportsTableView.Rx().ReloadItems(source))
                .DisposedBy(DisposeBag);

            source.ScrolledWithHeaderOffset
                .Subscribe(onReportsTableScrolled)
                .DisposedBy(DisposeBag);

            ReportsTableView.Source = source;

            bool areThereEnoughWorkspaces(ICollection<SelectOption<IThreadSafeWorkspace>> workspaces) => workspaces.Count > 1;

            bool isWorkspaceNameTooLong(string workspaceName)
            {
                var attributes = new UIStringAttributes { Font = WorkspaceLabel.Font };
                var size = new NSString(workspaceName).GetSizeUsingAttributes(attributes);
                var maxWidth = TraitCollection.HorizontalSizeClass == UIUserInterfaceSizeClass.Regular
                    ? maximumWorkspaceNameLabelWidthRegular
                    : maximumWorkspaceNameLabelWidthCompact;
                return size.Width >= maxWidth;
            };

            //Text
            ViewModel.WorkspaceNameObservable
                .Subscribe(WorkspaceLabel.Rx().Text())
                .DisposedBy(DisposeBag);

            ViewModel.CurrentDateRange
                .Subscribe(titleButton.Rx().TitleAdaptive())
                .DisposedBy(DisposeBag);

            ViewModel.CurrentDateRange
                .Select(range => range == null)
                .DistinctUntilChanged()
                .Subscribe(shouldCenter =>
                {
                    if (shouldCenter)
                        activityIndicator.Frame = activityIndicatorCenteredFrame;
                    else
                        activityIndicator.Frame = activityIndicatorLeftAlignedFrame;
                })
                .DisposedBy(DisposeBag);

            //Visibility
            ViewModel.WorkspacesObservable
                .Select(areThereEnoughWorkspaces)
                .Do(updateWorkspaceButtonInsets)
                .Subscribe(WorkspaceButton.Rx().IsVisible())
                .DisposedBy(DisposeBag);

            ViewModel.WorkspaceNameObservable
                .Select(isWorkspaceNameTooLong)
                .Subscribe(WorkspaceFadeView.Rx().FadeRight())
                .DisposedBy(DisposeBag);

            ViewModel.IsLoadingObservable
                .Subscribe(activityIndicator.Rx().IsAnimating())
                .DisposedBy(DisposeBag);

            //Commands
            titleButton.Rx().Tap()
                .Subscribe(toggleCalendar)
                .DisposedBy(DisposeBag);

            ReportsTableView.Rx().Tap()
                .Subscribe(HideCalendar)
                .DisposedBy(DisposeBag);

            WorkspaceButton.Rx()
                .BindAction(ViewModel.SelectWorkspace)
                .DisposedBy(DisposeBag);

            //Handoff
            viewDidAppearSubject.AsObservable()
                .CombineLatest(
                    ViewModel.WorkspaceId,
                    ViewModel.StartDate,
                    ViewModel.EndDate,
                    (_, workspaceId, start, end) => createUserActivity(workspaceId, start, end))
                .Subscribe(updateUserActivity);

            NSUserActivity createUserActivity(long workspaceId, DateTimeOffset start, DateTimeOffset end)
            {
                var userActivity = new NSUserActivity(Handoff.Action.Reports);
                userActivity.EligibleForHandoff = true;
                userActivity.WebPageUrl = Handoff.Url.Reports(workspaceId, start, end);
                return userActivity;
            }

            void updateUserActivity(NSUserActivity userActivity)
            {
                UserActivity = userActivity;
                UserActivity.BecomeCurrent();
            }

            void toggleCalendar()
            {
                if (calendarIsVisible)
                {
                    HideCalendar();
                    return;
                }

                if (TraitCollection.HorizontalSizeClass == UIUserInterfaceSizeClass.Compact)
                {
                    ShowCalendar();
                    return;
                }

                ShowPopoverCalendar();
            }
        }

        public override void ViewDidAppear(bool animated)
        {
            base.ViewDidAppear(animated);

            IosDependencyContainer.Instance.IntentDonationService.DonateShowReport();

            if (alreadyLoadedCalendar)
                return;

            // Calendar
            if (TraitCollection.HorizontalSizeClass == UIUserInterfaceSizeClass.Compact)
            {
                AddChildViewController(calendarViewController);
                CalendarContainer.AddSubview(calendarViewController.View);

                calendarViewController.View.TopAnchor.ConstraintEqualTo(CalendarContainer.TopAnchor).Active = true;
                calendarViewController.View.BottomAnchor.ConstraintEqualTo(CalendarContainer.BottomAnchor).Active = true;
                calendarViewController.View.LeftAnchor.ConstraintEqualTo(CalendarContainer.LeftAnchor).Active = true;
                calendarViewController.View.RightAnchor.ConstraintEqualTo(CalendarContainer.RightAnchor).Active = true;
                calendarViewController.View.TranslatesAutoresizingMaskIntoConstraints = false;
                calendarViewController.DidMoveToParentViewController(this);
            }
            else
            {
                ViewModel.CalendarViewModel.SelectInitialShortcut();
            }

            alreadyLoadedCalendar = true;

            viewDidAppearSubject.OnNext(Unit.Default);
        }

        public void ScrollToTop()
        {
            if (ReportsTableView == null)
                return;

            var point = new CGPoint(0, -ReportsTableView.ContentInset.Top);
            ReportsTableView.SetContentOffset(point, true);
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (!disposing) return;

            calendarSizeDisposable?.Dispose();
            calendarSizeDisposable = null;
        }

        private void onReportsTableScrolled(CGPoint offset)
        {
            if (calendarIsVisible)
            {
                var topConstant = (TopCalendarConstraint.Constant + offset.Y).Clamp(0, calendarHeight);
                TopCalendarConstraint.Constant = topConstant;

                if (topConstant == 0) return;

                // we need to adjust the offset of the scroll view so that it doesn't fold
                // under the calendar while scrolling up
                var adjustedOffset = new CGPoint(offset.X, ReportsTableView.ContentOffset.Y - offset.Y);
                ReportsTableView.SetContentOffset(adjustedOffset, false);
                View.LayoutIfNeeded();

                if (topConstant == calendarHeight)
                {
                    HideCalendar();
                }
            }
        }

        internal void ShowCalendar()
        {
            TopCalendarConstraint.Constant = 0;
            Animate(
                Animation.Timings.EnterTiming,
                Animation.Curves.SharpCurve,
                () => View.LayoutIfNeeded(),
                () =>
                {
                    calendarIsVisible = true;
                    ViewModel.CalendarViewModel.Reload();
                });
        }

        internal void HideCalendar()
        {
            calendarViewController.DismissViewController(false, null);

            TopCalendarConstraint.Constant = calendarHeight;
            Animate(
                Animation.Timings.EnterTiming,
                Animation.Curves.SharpCurve,
                () => View.LayoutIfNeeded(),
                () => calendarIsVisible = false);
        }

        internal void ShowPopoverCalendar()
        {
            HideCalendar();

            calendarViewController.ModalPresentationStyle = UIModalPresentationStyle.Popover;
            UIPopoverPresentationController presentationPopover = calendarViewController.PopoverPresentationController;
            if (presentationPopover != null)
            {
                presentationPopover.SourceView = titleButton;
                presentationPopover.PermittedArrowDirections = UIPopoverArrowDirection.Up;
                presentationPopover.SourceRect = titleButton.Frame;
            }

            PresentViewController(calendarViewController, true, null);
            ViewModel.CalendarViewModel.Reload();
        }

        public override void ViewDidLayoutSubviews()
        {
            base.ViewDidLayoutSubviews();

            source.UpdateContentInset();
        }

        public override void ViewWillLayoutSubviews()
        {
            base.ViewWillLayoutSubviews();
            ContentWidthConstraint.Constant = (nfloat)Math.Min(View.Bounds.Width, maxWidth);
        }

        private void prepareViews()
        {
            // Title view
            NavigationItem.TitleView = titleButton = new UIButton(new CGRect(0, 0, 200, 40));
            titleButton.Font = UIFont.SystemFontOfSize(14, UIFontWeight.Medium);
            titleButton.SetTitleColor(ColorAssets.Text, UIControlState.Normal);
            activityIndicator = new UIActivityIndicatorView();
            activityIndicator.Color = ColorAssets.Text;
            activityIndicator.StartAnimating();
            activityIndicator.Frame = activityIndicatorCenteredFrame;
            titleButton.AddSubview(activityIndicator);

            // Calendar configuration
            TopCalendarConstraint.Constant = calendarHeight;

            // Workspace button settings
            WorkspaceFadeView.FadeWidth = 32;
            WorkspaceButton.Layer.ShadowColor = UIColor.Black.CGColor;
            WorkspaceButton.Layer.ShadowRadius = 10;
            WorkspaceButton.Layer.ShadowOffset = new CGSize(0, 2);
            WorkspaceButton.Layer.ShadowOpacity = 0.10f;
            WorkspaceButton.Layer.BorderColor = ColorAssets.Separator.CGColor;
            WorkspaceButton.Layer.BorderWidth = 0.35f;

            View.BackgroundColor = ColorAssets.TableBackground;
        }

        private void onCalendarSizeChanged(NSObservedChange change)
        {
            TopCalendarConstraint.Constant = calendarIsVisible ? 0 : calendarHeight;
        }

        private void updateWorkspaceButtonInsets(bool workspacesButtonIsShown)
        {
            source.UpdateContentInset(workspacesButtonIsShown);
        }
    }
}

