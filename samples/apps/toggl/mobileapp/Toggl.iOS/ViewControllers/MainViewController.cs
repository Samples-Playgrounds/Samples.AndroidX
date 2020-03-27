using CoreGraphics;
using Foundation;
using System;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading;
using CoreAnimation;
using Toggl.Core.Analytics;
using Toggl.Core.Extensions;
using Toggl.Core.Models.Interfaces;
using Toggl.Core.UI.Collections;
using Toggl.Core.UI.Extensions;
using Toggl.Core.UI.Helper;
using Toggl.Core.UI.Onboarding.MainView;
using Toggl.Core.UI.ViewModels;
using Toggl.Core.UI.ViewModels.TimeEntriesLog;
using Toggl.Core.UI.ViewModels.TimeEntriesLog.Identity;
using Toggl.iOS.Shared;
using Toggl.iOS.Extensions;
using Toggl.iOS.Extensions.Reactive;
using Toggl.iOS.Helper;
using Toggl.iOS.Presentation;
using Toggl.iOS.Suggestions;
using Toggl.iOS.Views;
using Toggl.iOS.ViewSources;
using Toggl.Shared;
using Toggl.Shared.Extensions;
using Toggl.Storage.Extensions;
using Toggl.Storage.Onboarding;
using Toggl.Storage.Settings;
using UIKit;
using static Toggl.Core.Analytics.EditTimeEntryOrigin;
using static Toggl.Core.UI.Helper.Animation;

namespace Toggl.iOS.ViewControllers
{
    using MainLogSection = AnimatableSectionModel<DaySummaryViewModel, LogItemViewModel, IMainLogKey>;

    public partial class MainViewController : ReactiveViewController<MainViewModel>, IScrollableToTop
    {
        private const float showCardDelay = 0.1f;

        private const float spiderHingeCornerRadius = 0.8f;
        private const float spiderHingeWidth = 16;
        private const float spiderHingeHeight = 2;
        private const float welcomeViewTopDistance = 239;
        private const float welcomeViewSideMargin = 16;

        private const float tooltipOffset = 7;

        private readonly UIView spiderContainerView = new UIView();
        private readonly SpiderOnARopeView spiderBroView = new SpiderOnARopeView();
        private readonly UIButton settingsButton = new UIButton(new CGRect(0, 0, 40, 50));
        private readonly UIButton syncFailuresButton = new UIButton(new CGRect(0, 0, 30, 40));
        private readonly UIImageView titleImage = new UIImageView(UIImage.FromBundle("togglLogo"))
        {
            AccessibilityLabel = Resources.AppTitle,
            AccessibilityTraits = UIAccessibilityTrait.Header
        };
        private readonly TimeEntriesEmptyLogView emptyStateView = TimeEntriesEmptyLogView.Create();

        private TimeEntriesLogViewCell firstTimeEntryCell;

        private bool viewInitialized;
        private CancellationTokenSource cardAnimationCancellation;

        private DismissableOnboardingStep tapToEditStep;

        private CompositeDisposable disposeBag = new CompositeDisposable();

        private Subject<Unit> traitCollectionSubject = new Subject<Unit>();

        private readonly UIView tableHeader = new UIView();
        private readonly UIView suggestionsContaier = new UIView { TranslatesAutoresizingMaskIntoConstraints = false };
        private readonly UIView ratingViewContainer = new UIView { TranslatesAutoresizingMaskIntoConstraints = false };
        private readonly SuggestionsView suggestionsView = new SuggestionsView { TranslatesAutoresizingMaskIntoConstraints = false };

        private TimeEntriesLogViewSource tableViewSource;

        private SnackBar snackBar;
        private RatingView ratingView;

        public MainViewController(MainViewModel viewModel)
            : base(viewModel, nameof(MainViewController))
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            TableShadow.Layer.MasksToBounds = false;
            TableShadow.Layer.ShadowColor = UIColor.Black.CGColor;
            TableShadow.Layer.ShadowOffset = new CGSize(0, 0);
            TableShadow.Layer.ShadowOpacity = 0.1f;
            TableShadow.Layer.ShadowRadius = 4;
            TableShadow.Hidden = TraitCollection.HorizontalSizeClass != UIUserInterfaceSizeClass.Regular;

            var separator = NavigationController.NavigationBar.InsertSeparator();
            separator.BackgroundColor = ColorAssets.OpaqueSeparator;

            WelcomeBackLabel.Text = Resources.LogEmptyStateTitle;
            WelcomeBackDescriptionLabel.Text = Resources.LogEmptyStateText;
            CreatedFirstTimeEntryLabel.Text = Resources.YouHaveCreatedYourFirstTimeEntry;
            TapToEditItLabel.Text = Resources.TapToEditIt;
            StartTimerBubbleLabel.Text = Resources.TapToStartTimer;
            TapToStopTimerLabel.Text = Resources.TapToStopTimer;
            FeedbackSentSuccessTitleLabel.Text = Resources.DoneWithExclamationMark.ToUpper();
            FeedbackSentDescriptionLabel.Text = Resources.ThankYouForTheFeedback;

            StartTimeEntryButton.AccessibilityLabel = Resources.StartTimeEntry;
            StopTimeEntryButton.AccessibilityLabel = Resources.StopCurrentlyRunningTimeEntry;

            tableViewSource = new TimeEntriesLogViewSource();

            prepareViews();
            prepareOnboarding();
            setupTableViewHeader();

            ViewModel.SwipeActionsEnabled
                .Subscribe(tableViewSource.SetSwipeActionsEnabled)
                .DisposedBy(disposeBag);

            TimeEntriesLogTableView.Source = tableViewSource;
            TimeEntriesLogTableView.BackgroundColor = ColorAssets.TableBackground;

            ViewModel.TimeEntries
                .Subscribe(TimeEntriesLogTableView.Rx().AnimateSections<MainLogSection, DaySummaryViewModel, LogItemViewModel, IMainLogKey>(tableViewSource))
                .DisposedBy(disposeBag);

            ViewModel.ShouldReloadTimeEntryLog
                .WithLatestFrom(ViewModel.TimeEntries, (_, timeEntries) => timeEntries)
                .Subscribe(TimeEntriesLogTableView.Rx().ReloadSections(tableViewSource))
                .DisposedBy(disposeBag);

            tableViewSource.ToggleGroupExpansion
                .Subscribe(ViewModel.TimeEntriesViewModel.ToggleGroupExpansion.Inputs)
                .DisposedBy(disposeBag);

            tableViewSource.FirstCell
                .Subscribe(onFirstTimeEntryChanged)
                .DisposedBy(DisposeBag);

            tableViewSource.Rx().Scrolled()
                .Subscribe(onTableScroll)
                .DisposedBy(DisposeBag);

            tableViewSource.ContinueTap
                .Select(item => timeEntryContinuation(item, false))
                .Subscribe(ViewModel.ContinueTimeEntry.Inputs)
                .DisposedBy(DisposeBag);

            tableViewSource.SwipeToContinue
                .Select(item => timeEntryContinuation(item, true))
                .Subscribe(ViewModel.ContinueTimeEntry.Inputs)
                .DisposedBy(DisposeBag);

            tableViewSource.SwipeToDelete
                .Select(logItem => logItem.RepresentedTimeEntriesIds)
                .Subscribe(ViewModel.TimeEntriesViewModel.DelayDeleteTimeEntries.Inputs)
                .DisposedBy(DisposeBag);

            tableViewSource.Rx().ModelSelected()
                .Select(editEventInfo)
                .Subscribe(ViewModel.SelectTimeEntry.Inputs)
                .DisposedBy(DisposeBag);

            tableViewSource.Rx().ItemsChanged()
                .Subscribe(updateTooltipPositions)
                .DisposedBy(DisposeBag);

            ViewModel.TimeEntriesViewModel.TimeEntriesPendingDeletion
                .Subscribe(toggleUndoDeletion)
                .DisposedBy(DisposeBag);

            // Refresh Control
            var refreshControl = new RefreshControl(
                ViewModel.SyncProgressState,
                tableViewSource.Rx().Scrolled(),
                tableViewSource.IsDragging);
            refreshControl.Refresh
                .Subscribe(ViewModel.Refresh.Inputs)
                .DisposedBy(DisposeBag);
            TimeEntriesLogTableView.CustomRefreshControl = refreshControl;

            //Actions
            settingsButton.Rx().BindAction(ViewModel.OpenSettings).DisposedBy(DisposeBag);
            syncFailuresButton.Rx().BindAction(ViewModel.OpenSyncFailures).DisposedBy(DisposeBag);
            StopTimeEntryButton.Rx().BindAction(ViewModel.StopTimeEntry, _ => TimeEntryStopOrigin.Manual).DisposedBy(DisposeBag);

            StartTimeEntryButton.Rx().BindAction(ViewModel.StartTimeEntry, _ => true).DisposedBy(DisposeBag);
            StartTimeEntryButton.Rx().BindAction(ViewModel.StartTimeEntry, _ => false, ButtonEventType.LongPress, useFeedback: true).DisposedBy(DisposeBag);

            CurrentTimeEntryCard.Rx().Tap()
                .WithLatestFrom(ViewModel.CurrentRunningTimeEntry, (_, te) => te)
                .Where(te => te != null)
                .Select(te => new EditTimeEntryInfo(EditTimeEntryOrigin.RunningTimeEntryCard, te.Id))
                .Subscribe(ViewModel.SelectTimeEntry.Inputs)
                .DisposedBy(DisposeBag);

            //Visibility
            var shouldWelcomeBack = ViewModel.ShouldShowWelcomeBack;

            ViewModel.ShouldShowEmptyState
                .Subscribe(visible => emptyStateView.Hidden = !visible)
                .DisposedBy(DisposeBag);

            shouldWelcomeBack
                .Subscribe(WelcomeBackView.Rx().IsVisible())
                .DisposedBy(DisposeBag);

            shouldWelcomeBack
                .Subscribe(spiderContainerView.Rx().IsVisible())
                .DisposedBy(DisposeBag);

            shouldWelcomeBack
                .Subscribe(visible =>
                {
                    if (visible)
                        spiderBroView.Show();
                    else
                        spiderBroView.Hide();
                })
                .DisposedBy(DisposeBag);

            //Text
            ViewModel.CurrentRunningTimeEntry
                .Select(te => te?.Description)
                .Subscribe(CurrentTimeEntryDescriptionLabel.Rx().Text())
                .DisposedBy(DisposeBag);

            ViewModel.ElapsedTime
                .Subscribe(CurrentTimeEntryElapsedTimeLabel.Rx().Text())
                .DisposedBy(DisposeBag);

            var capHeight = CurrentTimeEntryProjectTaskClientLabel.Font.CapHeight;
            var clientColor = ColorAssets.Text3;
            ViewModel.CurrentRunningTimeEntry
                .Select(te => te?.ToFormattedTimeEntryString(capHeight, clientColor, shouldColorProject: true))
                .Subscribe(CurrentTimeEntryProjectTaskClientLabel.Rx().AttributedText())
                .DisposedBy(DisposeBag);

            //Accessibility
            CurrentTimeEntryCard.IsAccessibilityElementFocused
                .CombineLatest(ViewModel.CurrentRunningTimeEntry,
                    (_, runningEntry) => createAccessibilityLabelForRunningEntryCard(runningEntry))
                .Subscribe(CurrentTimeEntryCard.Rx().AccessibilityLabel())
                .DisposedBy(disposeBag);

            //The start button
            var trackModeImage = UIImage.FromBundle("playIcon");
            var manualModeImage = UIImage.FromBundle("manualIcon");
            ViewModel.IsInManualMode
                .Select(isInManualMode => isInManualMode ? manualModeImage : trackModeImage)
                .Subscribe(image => StartTimeEntryButton.SetImage(image, UIControlState.Normal))
                .DisposedBy(DisposeBag);

            //The sync failures button
            ViewModel.NumberOfSyncFailures
                .Select(numberOfSyncFailures => numberOfSyncFailures > 0)
                .Subscribe(syncFailuresButton.Rx().IsVisible())
                .DisposedBy(DisposeBag);

            ViewModel.RatingViewModel.IsFeedbackSuccessViewShowing
                .Subscribe(SendFeedbackSuccessView.Rx().AnimatedIsVisible())
                .DisposedBy(DisposeBag);

            SendFeedbackSuccessView.Rx().Tap()
                .Subscribe(ViewModel.RatingViewModel.CloseFeedbackSuccessView)
                .DisposedBy(DisposeBag);

            ViewModel.ShouldShowRatingView
                .Subscribe(showHideRatingView)
                .DisposedBy(disposeBag);

            // Suggestion View
            suggestionsView.SuggestionTapped
                .Subscribe(ViewModel.SuggestionsViewModel.StartTimeEntry.Inputs)
                .DisposedBy(DisposeBag);

            ViewModel.SuggestionsViewModel.IsEmpty.Invert()
                .Subscribe(suggestionsView.Rx().IsVisible())
                .DisposedBy(DisposeBag);

            ViewModel.SuggestionsViewModel.Suggestions
                .ReemitWhen(traitCollectionSubject)
                .Subscribe(suggestions =>
                {
                    suggestionsView.OnSuggestions(suggestions);
                    layoutTableHeader();
                })
                .DisposedBy(DisposeBag);

            // Intent Donation
            IosDependencyContainer.Instance.IntentDonationService.SetDefaultShortcutSuggestions();

            Observable.Merge(
                    ViewModel.ContinueTimeEntry.Elements,
                    ViewModel.SuggestionsViewModel.StartTimeEntry.Elements
                )
                .Subscribe(IosDependencyContainer.Instance.IntentDonationService.DonateStartTimeEntry)
                .DisposedBy(DisposeBag);

            ViewModel.StopTimeEntry.Elements
                .Subscribe(IosDependencyContainer.Instance.IntentDonationService.DonateStopCurrentTimeEntry)
                .DisposedBy(DisposeBag);

            View.SetNeedsLayout();
            View.LayoutIfNeeded();

            NSNotificationCenter.DefaultCenter.AddObserver(UIApplication.DidBecomeActiveNotification, onApplicationDidBecomeActive);
        }

        public override void ViewDidAppear(bool animated)
        {
            base.ViewDidAppear(animated);

            var activity = new NSUserActivity(Handoff.Action.Log);
            activity.EligibleForHandoff = true;
            activity.WebPageUrl = Handoff.Url.Log;
            UserActivity = activity;
            activity.BecomeCurrent();
        }

        public override void ViewDidDisappear(bool animated)
        {
            base.ViewDidDisappear(animated);

            if (TapToEditBubbleView != null && !TapToEditBubbleView.Hidden)
            {
                tapToEditStep?.Dismiss();
            }
        }

        private string createAccessibilityLabelForRunningEntryCard(IThreadSafeTimeEntry timeEntry)
        {
            if (timeEntry == null)
                return null;

            var accessibilityLabel = Resources.CurrentlyRunningTimeEntry;

            var duration = IosDependencyContainer.Instance.TimeService.CurrentDateTime - timeEntry.Start;
            accessibilityLabel += $", {duration}";

            if (!string.IsNullOrEmpty(timeEntry.Description))
                accessibilityLabel += $", {timeEntry.Description}";

            var projectName = timeEntry.Project?.Name ?? "";
            if (!string.IsNullOrEmpty(projectName))
                accessibilityLabel += $", {Resources.Project}: {projectName}";

            var taskName = timeEntry.Task?.Name ?? "";
            if (!string.IsNullOrEmpty(taskName))
                accessibilityLabel += $", {Resources.Task}: {taskName}";

            var clientName = timeEntry.Project?.Client?.Name ?? "";
            if (!string.IsNullOrEmpty(clientName))
                accessibilityLabel += $", {Resources.Client}: {clientName}";

            return accessibilityLabel;
        }

        private void setupTableViewHeader()
        {
            TimeEntriesLogTableView.TableHeaderView = tableHeader;

            tableHeader.TranslatesAutoresizingMaskIntoConstraints = false;
            tableHeader.WidthAnchor.ConstraintEqualTo(TimeEntriesLogTableView.WidthAnchor).Active = true;

            tableHeader.AddSubview(suggestionsContaier);
            tableHeader.AddSubview(ratingViewContainer);

            suggestionsContaier.ConstrainToViewSides(tableHeader);
            ratingViewContainer.ConstrainToViewSides(tableHeader);

            suggestionsContaier.TopAnchor.ConstraintEqualTo(tableHeader.TopAnchor, TimeEntriesLogViewSource.SpaceBetweenSections).Active = true;
            suggestionsContaier.BottomAnchor.ConstraintEqualTo(ratingViewContainer.TopAnchor, TimeEntriesLogViewSource.SpaceBetweenSections).Active = true;
            ratingViewContainer.BottomAnchor.ConstraintEqualTo(tableHeader.BottomAnchor).Active = true;

            suggestionsContaier.AddSubview(suggestionsView);
            suggestionsView.ConstrainInView(suggestionsContaier);

            layoutTableHeader();
            updateTooltipPositions();
        }

        private void layoutTableHeader()
        {
            // This method makes little to no sense, but it works, and it comes from this accepted StackOverflow answer:
            // https://stackoverflow.com/questions/16471846/is-it-possible-to-use-autolayout-with-uitableviews-tableheaderview
            TimeEntriesLogTableView.TableHeaderView = tableHeader;
            tableHeader.SetNeedsLayout();
            tableHeader.LayoutIfNeeded();
            var frame = tableHeader.Frame;
            frame.Size = tableHeader.SystemLayoutSizeFittingSize(UIView.UILayoutFittingCompressedSize);
            tableHeader.Frame = frame;
            TimeEntriesLogTableView.TableHeaderView = tableHeader;
        }

        private EditTimeEntryInfo editEventInfo(LogItemViewModel item)
        {
            var origin = item.IsTimeEntryGroupHeader
                ? GroupHeader
                : item.BelongsToGroup
                    ? GroupTimeEntry
                    : SingleTimeEntry;

            return new EditTimeEntryInfo(origin, item.RepresentedTimeEntriesIds);
        }

        private ContinueTimeEntryInfo timeEntryContinuation(LogItemViewModel itemViewModel, bool isSwipe)
        {
            var continueMode = default(ContinueTimeEntryMode);

            if (isSwipe)
            {
                continueMode = itemViewModel.IsTimeEntryGroupHeader
                    ? ContinueTimeEntryMode.TimeEntriesGroupSwipe
                    : ContinueTimeEntryMode.SingleTimeEntrySwipe;
            }
            else
            {
                continueMode = itemViewModel.IsTimeEntryGroupHeader
                    ? ContinueTimeEntryMode.TimeEntriesGroupContinueButton
                    : ContinueTimeEntryMode.SingleTimeEntryContinueButton;
            }

            return new ContinueTimeEntryInfo(itemViewModel, continueMode);
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);

            NavigationItem.TitleView = titleImage;
            NavigationItem.RightBarButtonItems = new[]
            {
                new UIBarButtonItem(settingsButton)
                {
                    AccessibilityLabel = Resources.Settings
                }
            };

#if DEBUG
            NavigationItem.LeftBarButtonItems = new[]
            {
                new UIKit.UIBarButtonItem(syncFailuresButton)
            };
#endif
        }

        public override void TraitCollectionDidChange(UITraitCollection previousTraitCollection)
        {
            base.TraitCollectionDidChange(previousTraitCollection);
            traitCollectionSubject.OnNext(Unit.Default);
            TimeEntriesLogTableView.ReloadData();

            TableShadow.Hidden = TraitCollection.HorizontalSizeClass != UIUserInterfaceSizeClass.Regular;
        }

        private void trackSiriEvents()
        {
            var events = SharedStorage.Instance.PopSiriTrackableEvents();

            events?
                .Select(e => e?.ToTrackableEvent())
                .Where(e => e != null)
                .Do(ViewModel.Track);
        }

        private void trackWidgetEvents()
        {
            var events = SharedStorage.Instance.PopWidgetTrackableEvents();

            events?
                .Select(e => e?.ToTrackableEvent())
                .Where(e => e != null)
                .Do(ViewModel.Track);
        }

        private void onApplicationDidBecomeActive(NSNotification notification)
        {
            if (SharedStorage.Instance.GetNeedsSync())
            {
                SharedStorage.Instance.SetNeedsSync(false);
                ViewModel.Refresh.Execute();
            }
            trackSiriEvents();
            trackWidgetEvents();
        }

        private void toggleUndoDeletion(int? numberOfTimeEntriesPendingDeletion)
        {
            if (snackBar != null)
            {
                snackBar.Hide();
                snackBar = null;
            }

            if (!numberOfTimeEntriesPendingDeletion.HasValue)
                return;

            var undoText = numberOfTimeEntriesPendingDeletion > 1
                ? String.Format(Resources.MultipleEntriesDeleted, numberOfTimeEntriesPendingDeletion)
                : Resources.EntryDeleted;

            snackBar = SnackBar.Factory.CreateUndoSnackBar(
                onUndo: () => ViewModel.TimeEntriesViewModel.CancelDeleteTimeEntry.Execute(Unit.Default),
                text: undoText);

            snackBar.SnackBottomAnchor = StartTimeEntryButton.TopAnchor;
            snackBar.Show(View);
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (!disposing) return;

            spiderBroView.Dispose();

            disposeBag?.Dispose();
            disposeBag = null;
        }

        public override void ViewDidLayoutSubviews()
        {
            base.ViewDidLayoutSubviews();

            TimeEntriesLogTableView.ContentInset = new UIEdgeInsets(
                top: -TimeEntriesLogViewSource.SpaceBetweenSections,
                left: 0,
                bottom: (nfloat)System.Math.Max(CurrentTimeEntryCard.Frame.Height, StartTimeEntryButton.Frame.Height),
                right: 0);

            TimeEntriesLogTableView.BringSubviewToFront(TimeEntriesLogTableView.TableHeaderView);

            if (viewInitialized) return;

            viewInitialized = true;

            ViewModel.IsTimeEntryRunning
                .Where(visible => visible)
                .Subscribe(_ => showTimeEntryCard())
                .DisposedBy(disposeBag);

            ViewModel.IsTimeEntryRunning
                .Where(visible => !visible)
                .Subscribe(_ => hideTimeEntryCard())
                .DisposedBy(disposeBag);
        }

        public void ScrollToTop()
        {
            TimeEntriesLogTableView.SetContentOffset(CGPoint.Empty, true);
        }

        private void showHideRatingView(bool shouldShow)
        {
            if (shouldShow)
            {
                showRatingView();
                return;
            }

            hideRatingView();
            layoutTableHeader();
        }

        private void showRatingView()
        {
            ratingView = RatingView.Create();
            ratingView.TranslatesAutoresizingMaskIntoConstraints = false;
            ratingView.ViewModel = ViewModel.RatingViewModel;
            ratingViewContainer.AddSubview(ratingView);
            ratingView.ConstrainInView(ratingViewContainer);
            View.SetNeedsLayout();
        }

        private void hideRatingView()
        {
            if (ratingView == null) return;

            ratingView.RemoveFromSuperview();
            ratingView.Dispose();
            ratingView = null;

            View.SetNeedsLayout();
        }

        private void prepareViews()
        {
            //Prevent bounces in UIScrollView
            AutomaticallyAdjustsScrollViewInsets = false;

            //Card border
            CurrentTimeEntryCard.Opaque = false;
            CurrentTimeEntryCard.Layer.CornerRadius = 8;
            CurrentTimeEntryCard.Layer.MaskedCorners = (CACornerMask)3;
            CurrentTimeEntryCard.Layer.ShadowColor = UIColor.Black.CGColor;
            CurrentTimeEntryCard.Layer.ShadowOffset = new CGSize(0, -2);
            CurrentTimeEntryCard.Layer.ShadowOpacity = 0.1f;
            CurrentTimeEntryElapsedTimeLabel.Font = CurrentTimeEntryElapsedTimeLabel.Font.GetMonospacedDigitFont();

            // Card animations
            StopTimeEntryButton.Hidden = true;
            CurrentTimeEntryCard.Hidden = true;

            //Hide play button for later animating it
            StartTimeEntryButton.Transform = CGAffineTransform.MakeScale(0.01f, 0.01f);
            StartTimeEntryButton.AdjustsImageWhenHighlighted = false;

            //Prepare Navigation bar images
            settingsButton.SetImage(UIImage.FromBundle("icSettings"), UIControlState.Normal);
            syncFailuresButton.SetImage(UIImage.FromBundle("icWarning"), UIControlState.Normal);

            RunningEntryDescriptionFadeView.FadeLeft = true;
            RunningEntryDescriptionFadeView.FadeRight = true;

            // Send Feedback Success View Setup
            SendFeedbackSuccessView.Hidden = true;

            prepareWelcomeBackViews();
            prepareEmptyStateView();
            prepareStartButtonLongPressAnimation();

            // Open edit view for the currently running time entry by swiping up
            var swipeUpRunningCardGesture = new UISwipeGestureRecognizer(async () =>
            {
                var currentlyRunningTimeEntry = await ViewModel.CurrentRunningTimeEntry.FirstAsync();
                if (currentlyRunningTimeEntry == null)
                    return;

                var selectTimeEntryData = new EditTimeEntryInfo(RunningTimeEntryCard, currentlyRunningTimeEntry.Id);
                await ViewModel.SelectTimeEntry.ExecuteWithCompletion(selectTimeEntryData);
            });
            swipeUpRunningCardGesture.Direction = UISwipeGestureRecognizerDirection.Up;
            CurrentTimeEntryCard.AddGestureRecognizer(swipeUpRunningCardGesture);
        }

        private void prepareStartButtonLongPressAnimation()
        {
            const double longPressMinimumPressDuration = 0.5; // default OS long press duration is 0.5s
            const double startAnimatingAfter = 0.1;
            const double bounceAnimationDuration = 0.1f;
            const double shrinkingAnimationDuration = longPressMinimumPressDuration - startAnimatingAfter - bounceAnimationDuration;
            nfloat noDelay = 0.0f;

            var shrunk = CGAffineTransform.MakeScale(0.9f, 0.9f);
            var bigger = CGAffineTransform.MakeScale(1.05f, 1.05f);
            var normalScale = CGAffineTransform.MakeScale(1f, 1f);

            var cts = new CancellationTokenSource();
            var press = new UILongPressGestureRecognizer(startButtonAnimation);
            press.MinimumPressDuration = startAnimatingAfter;
            press.ShouldRecognizeSimultaneously = (_, __) => true;

            StartTimeEntryButton.AddGestureRecognizer(press);

            void startButtonAnimation(UIGestureRecognizer recognizer)
            {
                switch (recognizer.State)
                {
                    case UIGestureRecognizerState.Began:
                        startShrinkingAnimation();
                        break;

                    case UIGestureRecognizerState.Cancelled:
                    case UIGestureRecognizerState.Failed:
                        cts?.Cancel();
                        cts = new CancellationTokenSource();
                        backToNormal();
                        break;
                }
            }

            void startShrinkingAnimation()
            {
                AnimationExtensions.Animate(
                    shrinkingAnimationDuration,
                    noDelay,
                    Curves.Bounce,
                    () => StartTimeEntryButton.Transform = shrunk,
                    expand,
                    cts.Token);
            }

            void expand()
            {
                AnimationExtensions.Animate(
                    bounceAnimationDuration / 2,
                    noDelay,
                    Curves.Bounce,
                    () => StartTimeEntryButton.Transform = bigger,
                    backToNormal,
                    cts.Token);
            }

            void backToNormal()
            {
                AnimationExtensions.Animate(
                    bounceAnimationDuration / 2,
                    noDelay,
                    Curves.Bounce,
                    () => StartTimeEntryButton.Transform = normalScale,
                    cancellationToken: cts.Token);
            }
        }

        private void showTimeEntryCard()
        {
            StopTimeEntryButton.Hidden = false;
            CurrentTimeEntryCard.Hidden = false;

            cardAnimationCancellation?.Cancel();
            cardAnimationCancellation = new CancellationTokenSource();

            AnimationExtensions.Animate(Timings.EnterTiming, showCardDelay, Curves.EaseOut,
                () => StartTimeEntryButton.Transform = CGAffineTransform.MakeScale(0.01f, 0.01f),
                () =>
                {
                    AnimationExtensions.Animate(Timings.LeaveTimingFaster, Curves.EaseIn,
                        () => StopTimeEntryButton.Transform = CGAffineTransform.MakeScale(1.0f, 1.0f),
                        cancellationToken: cardAnimationCancellation.Token);

                    AnimationExtensions.Animate(Timings.LeaveTiming, Curves.CardOutCurve,
                        () => CurrentTimeEntryCard.Transform = CGAffineTransform.MakeTranslation(0, 0),
                        cancellationToken: cardAnimationCancellation.Token);
                },
                cancellationToken: cardAnimationCancellation.Token);
        }

        private void hideTimeEntryCard()
        {
            cardAnimationCancellation?.Cancel();
            cardAnimationCancellation = new CancellationTokenSource();

            AnimationExtensions.Animate(Timings.LeaveTimingFaster, Curves.EaseIn,
                () => StopTimeEntryButton.Transform = CGAffineTransform.MakeScale(0.01f, 0.01f),
                () => StopTimeEntryButton.Hidden = true,
                cancellationToken: cardAnimationCancellation.Token);

            AnimationExtensions.Animate(Timings.LeaveTiming, Curves.CardOutCurve,
                () => CurrentTimeEntryCard.Transform = CGAffineTransform.MakeTranslation(0, CurrentTimeEntryCard.Frame.Height),
                () =>
                {
                    CurrentTimeEntryCard.Hidden = true;

                    AnimationExtensions.Animate(Timings.EnterTiming, Curves.EaseOut,
                        () => StartTimeEntryButton.Transform = CGAffineTransform.MakeScale(1f, 1f),
                        cancellationToken: cardAnimationCancellation.Token);
                },
                cancellationToken: cardAnimationCancellation.Token);
        }

        //Spider is added in code, because IB doesn't allow adding subviews
        //to a UITableView and the spider needs to be a subview of the table
        //view so it reacts to pulling down to refresh
        private void prepareWelcomeBackViews()
        {
            // Welcome back view must be placed inside of the time entries
            // log table view below the spider so that it does not overlay
            // the spider at any time.
            WelcomeBackView.RemoveFromSuperview();
            TimeEntriesLogTableView.AddSubview(WelcomeBackView);
            NSLayoutConstraint.ActivateConstraints(new[]
            {
                WelcomeBackView.CenterXAnchor.ConstraintEqualTo(TimeEntriesLogTableView.CenterXAnchor),
                WelcomeBackView.TopAnchor.ConstraintEqualTo(TimeEntriesLogTableView.TopAnchor, welcomeViewTopDistance),
                WelcomeBackView.LeadingAnchor.ConstraintEqualTo(TimeEntriesLogTableView.LeadingAnchor, welcomeViewSideMargin),
                WelcomeBackView.TrailingAnchor.ConstraintEqualTo(TimeEntriesLogTableView.TrailingAnchor, welcomeViewSideMargin)
            });

            var spiderHinge = new UIView();

            spiderHinge.Layer.CornerRadius = spiderHingeCornerRadius;
            spiderHinge.TranslatesAutoresizingMaskIntoConstraints = false;
            spiderHinge.BackgroundColor = ColorAssets.Spider;
            spiderContainerView.TranslatesAutoresizingMaskIntoConstraints = false;
            spiderBroView.TranslatesAutoresizingMaskIntoConstraints = false;
            spiderContainerView.BackgroundColor = UIColor.Clear;

            spiderContainerView.AddSubview(spiderHinge);
            spiderContainerView.AddSubview(spiderBroView);
            TimeEntriesLogTableView.AddSubview(spiderContainerView);

            //Container constraints
            spiderContainerView.WidthAnchor.ConstraintEqualTo(TimeEntriesLogTableView.WidthAnchor).Active = true;
            spiderContainerView.HeightAnchor.ConstraintEqualTo(TimeEntriesLogTableView.HeightAnchor).Active = true;
            spiderContainerView.CenterYAnchor.ConstraintEqualTo(TimeEntriesLogTableView.CenterYAnchor).Active = true;
            spiderContainerView.CenterXAnchor.ConstraintEqualTo(TimeEntriesLogTableView.CenterXAnchor).Active = true;

            //Hinge constraints
            spiderHinge.WidthAnchor.ConstraintEqualTo(spiderHingeWidth).Active = true;
            spiderHinge.HeightAnchor.ConstraintEqualTo(spiderHingeHeight).Active = true;
            spiderHinge.TopAnchor.ConstraintEqualTo(spiderContainerView.TopAnchor).Active = true;
            spiderHinge.CenterXAnchor.ConstraintEqualTo(spiderContainerView.CenterXAnchor).Active = true;

            //Spider constraints
            spiderBroView.TopAnchor.ConstraintEqualTo(spiderContainerView.TopAnchor).Active = true;
            spiderBroView.WidthAnchor.ConstraintEqualTo(spiderContainerView.WidthAnchor).Active = true;
            spiderBroView.BottomAnchor.ConstraintEqualTo(spiderContainerView.BottomAnchor).Active = true;
            spiderBroView.CenterXAnchor.ConstraintEqualTo(spiderContainerView.CenterXAnchor).Active = true;
        }

        private void prepareEmptyStateView()
        {
            emptyStateView.BackgroundColor = UIColor.Clear;
            emptyStateView.TranslatesAutoresizingMaskIntoConstraints = false;

            TimeEntriesLogTableView.AddSubview(emptyStateView);

            emptyStateView.WidthAnchor.ConstraintEqualTo(TimeEntriesLogTableView.WidthAnchor).Active = true;
            emptyStateView.HeightAnchor.ConstraintEqualTo(TimeEntriesLogTableView.HeightAnchor).Active = true;
            emptyStateView.CenterYAnchor.ConstraintEqualTo(TimeEntriesLogTableView.CenterYAnchor).Active = true;
            emptyStateView.TopAnchor.ConstraintEqualTo(TimeEntriesLogTableView.TopAnchor).Active = true;
        }

        private void prepareOnboarding()
        {
            var storage = ViewModel.OnboardingStorage;

            var timelineIsEmpty = Observable.CombineLatest(
                tableViewSource.FirstCell.Select(cell => cell == null),
                ViewModel.LogEmpty,
                CommonFunctions.Or);

            new StartTimeEntryOnboardingStep(storage)
                .ManageDismissableTooltip(StartTimeEntryOnboardingBubbleView, storage)
                .DisposedBy(disposeBag);

            new StopTimeEntryOnboardingStep(storage, ViewModel.IsTimeEntryRunning)
                .ManageDismissableTooltip(StopTimeEntryOnboardingBubbleView, storage)
                .DisposedBy(disposeBag);

            tapToEditStep = new EditTimeEntryOnboardingStep(storage, timelineIsEmpty)
                .ToDismissable(nameof(EditTimeEntryOnboardingStep), storage);

            tapToEditStep.DismissByTapping(TapToEditBubbleView);
            tapToEditStep.ManageVisibilityOf(TapToEditBubbleView).DisposedBy(disposeBag);
        }

        private void onTableScroll(CGPoint offset)
        {
            updateTooltipPositions();
        }

        private void onFirstTimeEntryChanged(TimeEntriesLogViewCell nextFirstTimeEntry)
        {
            firstTimeEntryCell = nextFirstTimeEntry;
            updateTooltipPositions();
        }

        private void updateTooltipPositions()
        {
            if (TapToEditBubbleView.Hidden) return;
            if (firstTimeEntryCell == null) return;

            var position = TimeEntriesLogTableView.ConvertRectToView(
                firstTimeEntryCell.Frame, TimeEntriesLogTableView.Superview);

            TapToEditBubbleViewTopConstraint.Constant = position.Bottom + tooltipOffset;
        }
    }
}
