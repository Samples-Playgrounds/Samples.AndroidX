using System.Reactive.Linq;
using System;
using CoreGraphics;
using Toggl.Core;
using Toggl.Core.Calendar;
using Toggl.Core.UI.ViewModels.Calendar;
using Toggl.iOS.Presentation;
using Toggl.iOS.Views.Calendar;
using Toggl.iOS.ViewSources;
using Toggl.Shared.Extensions;
using UIKit;
using System;
using System.Collections.Immutable;
using System.Linq;
using System.Reactive;
using Toggl.Core.Extensions;
using Toggl.Core.Services;
using Toggl.Core.UI.Helper;
using Toggl.Core.UI.ViewModels.Calendar.ContextualMenu;
using Toggl.iOS.Extensions;
using Toggl.iOS.Extensions.Reactive;
using Toggl.Shared;
using Toggl.Shared.Extensions.Reactive;

namespace Toggl.iOS.ViewControllers
{
    public sealed partial class CalendarDayViewController : ReactiveViewController<CalendarDayViewModel>, IScrollableToTop
    {
        private const double minimumOffsetOfCurrentTimeIndicatorFromScreenEdge = 0.2;
        private const double middleOfTheDay = 12;
        private const float collectionViewDefaultInset = 20;
        private const float maxWidth = 834;
        private const float additionalContentOffsetWhenContextualMenuIsVisible = 128;

        private readonly ITimeService timeService;
        private readonly IRxActionFactory rxActionFactory;

        private bool contextualMenuInitialised;

        private CalendarCollectionViewLayout layout;
        private CalendarCollectionViewSource dataSource;
        private CalendarCollectionViewEditItemHelper editItemHelper;
        private CalendarCollectionViewCreateFromSpanHelper createFromSpanHelper;
        private CalendarCollectionViewZoomHelper zoomHelper;
        private CalendarCollectionViewContextualMenuDismissHelper tapToDismissHelper;

        public float ScrollOffset => (float)CalendarCollectionView.ContentOffset.Y;

        private readonly BehaviorRelay<bool> contextualMenuVisible;
        private readonly BehaviorRelay<string> timeTrackedOnDay;
        private readonly BehaviorRelay<int> currentPageRelay;

        public CalendarDayViewController(
            CalendarDayViewModel viewModel,
            BehaviorRelay<int> currentPageRelay,
            BehaviorRelay<string> timeTrackedOnDay,
            BehaviorRelay<bool> contextualMenuVisible)
            : base(viewModel, nameof(CalendarDayViewController))
        {
            Ensure.Argument.IsNotNull(ViewModel, nameof(ViewModel));
            Ensure.Argument.IsNotNull(currentPageRelay, nameof(currentPageRelay));
            Ensure.Argument.IsNotNull(timeTrackedOnDay, nameof(timeTrackedOnDay));
            Ensure.Argument.IsNotNull(contextualMenuVisible, nameof(contextualMenuVisible));

            timeService = IosDependencyContainer.Instance.TimeService;
            rxActionFactory = IosDependencyContainer.Instance.RxActionFactory;

            this.currentPageRelay = currentPageRelay;
            this.timeTrackedOnDay = timeTrackedOnDay;
            this.contextualMenuVisible = contextualMenuVisible;
        }

        public void SetScrollOffset(float scrollOffset)
        {
            CalendarCollectionView?.SetContentOffset(new CGPoint(0, scrollOffset), false);
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            ViewModel.ContextualMenuViewModel.AttachView(this);

            ContextualMenu.Layer.CornerRadius = 8;
            ContextualMenu.Layer.ShadowColor = UIColor.Black.CGColor;
            ContextualMenu.Layer.ShadowOpacity = 0.1f;
            ContextualMenu.Layer.ShadowOffset = new CGSize(0, -2);

            ContextualMenuBottonConstraint.Constant = -ContextualMenu.Frame.Height;

            ContextualMenuFadeView.FadeLeft = true;
            ContextualMenuFadeView.FadeRight = true;

            dataSource = new CalendarCollectionViewSource(
                timeService,
                CalendarCollectionView,
                ViewModel.TimeOfDayFormat,
                ViewModel.CalendarItems);

            layout = new CalendarCollectionViewLayout(ViewModel.Date.ToLocalTime().Date, timeService, dataSource);

            editItemHelper = new CalendarCollectionViewEditItemHelper(CalendarCollectionView, timeService, rxActionFactory, dataSource, layout);
            createFromSpanHelper = new CalendarCollectionViewCreateFromSpanHelper(CalendarCollectionView, dataSource, layout);
            zoomHelper = new CalendarCollectionViewZoomHelper(CalendarCollectionView, layout);
            tapToDismissHelper = new CalendarCollectionViewContextualMenuDismissHelper(CalendarCollectionView, dataSource);

            CalendarCollectionView.SetCollectionViewLayout(layout, false);
            CalendarCollectionView.Delegate = dataSource;
            CalendarCollectionView.DataSource = dataSource;

            //Editing items
            dataSource.ItemTapped
                .Select(item => (CalendarItem?)item)
                .Subscribe(ViewModel.ContextualMenuViewModel.OnCalendarItemUpdated.Inputs)
                .DisposedBy(DisposeBag);

            ViewModel.ContextualMenuViewModel.CalendarItemInEditMode
                .Subscribe(editItemHelper.StartEditingItem.Inputs)
                .DisposedBy(DisposeBag);

            editItemHelper.ItemUpdated
                .Subscribe(dataSource.UpdateItemView)
                .DisposedBy(DisposeBag);

            editItemHelper.ItemUpdated
                .Select(item => (CalendarItem?)item)
                .Subscribe(ViewModel.ContextualMenuViewModel.OnCalendarItemUpdated.Inputs)
                .DisposedBy(DisposeBag);

            ViewModel.ContextualMenuViewModel.DiscardChanges
                .Subscribe(_ => editItemHelper.DiscardChanges())
                .DisposedBy(DisposeBag);

            //Contextual menu
            ViewModel.ContextualMenuViewModel.CurrentMenu
                .Select(menu => menu.Actions)
                .Subscribe(replaceContextualMenuActions)
                .DisposedBy(DisposeBag);

            ViewModel.ContextualMenuViewModel.MenuVisible
                .Where(isVisible => isVisible)
                .Subscribe(_ => showContextualMenu())
                .DisposedBy(DisposeBag);

            ViewModel.ContextualMenuViewModel.MenuVisible
                .Where(isVisible => !isVisible)
                .Subscribe(_ => dismissContextualMenu())
                .DisposedBy(DisposeBag);

            ViewModel.ContextualMenuViewModel.CalendarItemRemoved
                .Subscribe(dataSource.RemoveItemView)
                .DisposedBy(DisposeBag);

            ContextualMenuCloseButton.Rx().Tap()
                .Subscribe(_ => ViewModel.ContextualMenuViewModel.OnCalendarItemUpdated.Execute(null))
                .DisposedBy(DisposeBag);

            tapToDismissHelper.DidTapOnEmptySpace
                .Subscribe(_ => ViewModel.ContextualMenuViewModel.OnCalendarItemUpdated.Execute(null))
                .DisposedBy(DisposeBag);

            ViewModel.ContextualMenuViewModel.TimeEntryPeriod
                .Subscribe(ContextualMenuTimeEntryPeriodLabel.Rx().Text())
                .DisposedBy(DisposeBag);

            ViewModel.ContextualMenuViewModel.TimeEntryInfo
                .Select(timeEntryInfo => timeEntryInfo.ToAttributedString(ContextualMenuTimeEntryDescriptionProjectTaskClientLabel.Font.CapHeight))
                .Subscribe(ContextualMenuTimeEntryDescriptionProjectTaskClientLabel.Rx().AttributedText())
                .DisposedBy(DisposeBag);

            ViewModel.TimeTrackedOnDay
                .ReemitWhen(currentPageRelay.SelectUnit())
                .Subscribe(notifyTotalDurationIfCurrentPage)
                .DisposedBy(DisposeBag);

            CalendarCollectionView.LayoutIfNeeded();
        }

        private void notifyTotalDurationIfCurrentPage(string durationString)
        {
            if (currentPageRelay.Value == View.Tag)
            {
                timeTrackedOnDay.Accept(durationString);
            }
        }

        public override void ViewDidLayoutSubviews()
        {
            base.ViewDidLayoutSubviews();

            updateContentInset();

            if (contextualMenuInitialised) return;
            contextualMenuInitialised = true;
            ContextualMenuBottonConstraint.Constant = -ContextualMenu.Frame.Height;
            View.LayoutIfNeeded();
        }

        private void replaceContextualMenuActions(IImmutableList<CalendarMenuAction> actions)
        {
            if (actions == null || actions.Count == 0) return;

            ContextualMenuStackView.ArrangedSubviews.ForEach(view => view.RemoveFromSuperview());

            actions.Select(action => new CalendarContextualMenuActionView(action)
                {
                    TranslatesAutoresizingMaskIntoConstraints = false
                })
                .Do(ContextualMenuStackView.AddArrangedSubview);
        }

        private void showContextualMenu()
        {
            if (!contextualMenuInitialised) return;

            contextualMenuVisible?.Accept(true);
            View.LayoutIfNeeded();
            ContextualMenuBottonConstraint.Constant = 0;
            AnimationExtensions.Animate(
                Animation.Timings.EnterTiming,
                Animation.Curves.EaseOut,
                () => View.LayoutIfNeeded(),
                scrollUpIfEditingItemIsCoveredByContextualMenu);
        }

        private void dismissContextualMenu()
        {
            if (!contextualMenuInitialised) return;

            ContextualMenuBottonConstraint.Constant = -ContextualMenu.Frame.Height;
            AnimationExtensions.Animate(
                Animation.Timings.EnterTiming,
                Animation.Curves.EaseOut,
                () => View.LayoutIfNeeded(),
                () =>
                {
                    contextualMenuVisible?.Accept(false);
                    updateContentInset(true);
                });
        }


        private void scrollUpIfEditingItemIsCoveredByContextualMenu()
        {
            var editingItemFrame = dataSource.FrameOfEditingItem();

            if (editingItemFrame == null) return;
            var editingItemTop = editingItemFrame.Value.Top - CalendarCollectionView.ContentOffset.Y;

            var shouldScrollUp = ContextualMenu.Frame.Top <= editingItemTop + additionalContentOffsetWhenContextualMenuIsVisible;
            if (!shouldScrollUp) return;

            var scrollDelta = editingItemTop - ContextualMenu.Frame.Top - additionalContentOffsetWhenContextualMenuIsVisible;

            var newContentOffset = new CGPoint(
                CalendarCollectionView.ContentOffset.X,
                CalendarCollectionView.ContentOffset.Y - scrollDelta);
            CalendarCollectionView.SetContentOffset(newContentOffset, true);
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);

            updateContentInset();
            layout.InvalidateCurrentTimeLayout();
        }

        private void updateContentInset(bool animate = false)
        {
            var topInset = collectionViewDefaultInset;

            var leftInset = CalendarCollectionView.Frame.Width <= maxWidth
                ? collectionViewDefaultInset
                : (CalendarCollectionView.Frame.Width - maxWidth) / 2;

            var rightInset = leftInset;

            var bottomInset = contextualMenuVisible.Value
                ? collectionViewDefaultInset * 2 + ContextualMenu.Frame.Height
                : collectionViewDefaultInset * 2;

            if (animate)
            {
                AnimationExtensions.Animate(
                    Animation.Timings.EnterTiming,
                    Animation.Curves.EaseOut,
                    () => CalendarCollectionView.ContentInset = new UIEdgeInsets(
                        topInset, leftInset, bottomInset, rightInset)
                    );
            }
            else
            {
                CalendarCollectionView.ContentInset = new UIEdgeInsets(
                    topInset, leftInset, bottomInset, rightInset);
            }
        }

        public override void ViewWillTransitionToSize(CGSize toSize, IUIViewControllerTransitionCoordinator coordinator)
        {
            base.ViewWillTransitionToSize(toSize, coordinator);

            updateContentInset();
        }

        public void ScrollToTop()
        {
            CalendarCollectionView?.SetContentOffset(CGPoint.Empty, true);
        }

        public void SetGoodScrollPoint()
        {
            var frameHeight =
                CalendarCollectionView.Frame.Height
                - CalendarCollectionView.ContentInset.Top
                - CalendarCollectionView.ContentInset.Bottom;
            var hoursOnScreen = frameHeight / (CalendarCollectionView.ContentSize.Height / 24);
            var centeredHour = calculateCenteredHour(timeService.CurrentDateTime.ToLocalTime().TimeOfDay.TotalHours, hoursOnScreen);

            var offsetY = (centeredHour / 24) * CalendarCollectionView.ContentSize.Height - (frameHeight / 2);
            var scrollPointY = offsetY.Clamp(0, CalendarCollectionView.ContentSize.Height - frameHeight);
            var offset = new CGPoint(0, scrollPointY);
            CalendarCollectionView.SetContentOffset(offset, false);
        }

        private static double calculateCenteredHour(double currentHour, double hoursOnScreen)
        {
            var hoursPerHalfOfScreen = hoursOnScreen / 2;
            var minimumOffset = hoursOnScreen * minimumOffsetOfCurrentTimeIndicatorFromScreenEdge;

            var center = (currentHour + middleOfTheDay) / 2;

            if (currentHour < center - hoursPerHalfOfScreen + minimumOffset)
            {
                // the current time indicator would be too close to the top edge of the screen
                return currentHour - minimumOffset + hoursPerHalfOfScreen;
            }

            if (currentHour > center + hoursPerHalfOfScreen - minimumOffset)
            {
                // the current time indicator would be too close to the bottom edge of the screen
                return currentHour + minimumOffset - hoursPerHalfOfScreen;
            }

            return center;
        }
    }
}
