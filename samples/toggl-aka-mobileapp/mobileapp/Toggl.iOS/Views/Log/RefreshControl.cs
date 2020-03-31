using CoreGraphics;
using Foundation;
using System;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using Toggl.Core.Sync;
using Toggl.Core.UI.Helper;
using Toggl.iOS.Extensions;
using Toggl.iOS.Views;
using Toggl.Shared;
using Toggl.Shared.Extensions;
using UIKit;
using static Toggl.iOS.Extensions.TextExtensions;

namespace Toggl.iOS.ViewSources
{
    public sealed class RefreshControl
    {
        private CompositeDisposable disposeBag = new CompositeDisposable();

        private const float syncBarHeight = 26;
        private const float activityIndicatorSize = 14;
        private const float syncLabelFontSize = 12;

        private static readonly float scrollThreshold = 3 * syncBarHeight;

        private readonly UIColor pullToRefreshColor = ColorAssets.CustomGray2;
        private readonly UIColor syncingColor = ColorAssets.CustomGray2;
        private readonly UIColor syncFailedColor = ColorAssets.CustomGray2;
        private readonly UIColor offlineColor = ColorAssets.CustomGray2;
        private readonly UIColor syncCompletedColor = Colors.Main.SyncCompleted.ToNativeColor();

        private bool wasReleased;
        private bool isSyncing = false;
        private bool needsRefresh;
        private bool shouldCalculateOnDeceleration;
        private bool shouldRefreshOnTap;

        private readonly UIView syncStateView = new UIView();
        private readonly UILabel syncStateLabel = new UILabel();
        private readonly UIButton dismissSyncBarButton = new UIButton();
        private readonly ActivityIndicatorView activityIndicatorView = new ActivityIndicatorView();

        private NSLayoutConstraint heightConstraint;

        private Subject<Unit> refreshSubject = new Subject<Unit>();
        public IObservable<Unit> Refresh
            => refreshSubject.AsObservable();

        private UIScrollView scrollView;

        public RefreshControl(
            IObservable<SyncProgress> syncProgress,
            IObservable<CGPoint> scrollOffset,
            IObservable<bool> isDragging)
        {
            syncProgress
                .Subscribe(syncProgressChanged)
                .DisposedBy(disposeBag);

            scrollOffset
                .Subscribe(didScroll)
                .DisposedBy(disposeBag);

            isDragging
                .Subscribe(draggingChanged)
                .DisposedBy(disposeBag);
        }

        public void Configure(UIScrollView scrollView)
        {
            if (scrollView.Superview == null)
            {
                throw new ArgumentException("RefreshControl cannot be added to UISCrollView not in view hierarchy.");
            }

            this.scrollView = scrollView;

            syncStateView.AddSubview(syncStateLabel);
            syncStateView.AddSubview(activityIndicatorView);
            syncStateView.AddSubview(dismissSyncBarButton);
            scrollView.Superview.AddSubview(syncStateView);

            prepareSyncStateView();
            prepareSyncStateLabel();
            prepareActivityIndicatorView();
            prepareDismissSyncBarImageView();
        }

        private void prepareSyncStateView()
        {
            syncStateView.BackgroundColor = pullToRefreshColor;
            syncStateView.TranslatesAutoresizingMaskIntoConstraints = false;
            syncStateView.TopAnchor.ConstraintEqualTo(scrollView.Superview.TopAnchor).Active = true;
            syncStateView.WidthAnchor.ConstraintEqualTo(scrollView.WidthAnchor).Active = true;
            syncStateView.CenterXAnchor.ConstraintEqualTo(scrollView.CenterXAnchor).Active = true;
            heightConstraint = syncStateView.HeightAnchor.ConstraintEqualTo(0);
            heightConstraint.Active = true;
        }

        private void prepareSyncStateLabel()
        {
            syncStateLabel.TextColor = UIColor.White;
            syncStateLabel.TranslatesAutoresizingMaskIntoConstraints = false;
            syncStateLabel.Font = syncStateLabel.Font.WithSize(syncLabelFontSize);
            syncStateLabel.CenterXAnchor.ConstraintEqualTo(syncStateView.CenterXAnchor).Active = true;
            syncStateLabel.BottomAnchor.ConstraintEqualTo(syncStateView.BottomAnchor, -6).Active = true;

            var tapGestureRecognizer = new UITapGestureRecognizer(onStatusLabelTap);
            syncStateLabel.UserInteractionEnabled = true;
            syncStateLabel.AddGestureRecognizer(tapGestureRecognizer);
        }

        private void prepareActivityIndicatorView()
        {
            activityIndicatorView.TranslatesAutoresizingMaskIntoConstraints = false;
            activityIndicatorView.WidthAnchor.ConstraintEqualTo(activityIndicatorSize).Active = true;
            activityIndicatorView.HeightAnchor.ConstraintEqualTo(activityIndicatorSize).Active = true;
            activityIndicatorView.CenterYAnchor.ConstraintEqualTo(syncStateLabel.CenterYAnchor).Active = true;
            activityIndicatorView.LeadingAnchor.ConstraintEqualTo(syncStateLabel.TrailingAnchor, 6).Active = true;
            activityIndicatorView.StartSpinning();
        }

        private void prepareDismissSyncBarImageView()
        {
            dismissSyncBarButton.Hidden = true;
            dismissSyncBarButton.SetImage(UIImage.FromBundle("icClose").ImageWithRenderingMode(UIImageRenderingMode.AlwaysTemplate), UIControlState.Normal);
            dismissSyncBarButton.TintColor = UIColor.White;
            dismissSyncBarButton.TranslatesAutoresizingMaskIntoConstraints = false;
            dismissSyncBarButton.CenterYAnchor.ConstraintEqualTo(syncStateLabel.CenterYAnchor).Active = true;
            dismissSyncBarButton.TrailingAnchor.ConstraintEqualTo(syncStateView.TrailingAnchor, -16).Active = true;
            dismissSyncBarButton.TouchUpInside += onDismissSyncBarButtonTap;
        }

        private async void syncProgressChanged(SyncProgress syncProgress)
        {
            bool hideIndicator = false;
            shouldRefreshOnTap = false;
            isSyncing = false;

            switch (syncProgress)
            {
                case SyncProgress.Unknown:
                    return;

                case SyncProgress.Syncing:
                    setSyncIndicatorTextAndBackground(
                        new NSAttributedString(Resources.Syncing),
                        syncingColor);
                    setActivityIndicatorVisible(true);
                    isSyncing = true;
                    break;

                case SyncProgress.OfflineModeDetected:
                    setSyncIndicatorTextAndBackground(
                        Resources.Offline.EndingWithRefreshIcon(syncStateLabel.Font.CapHeight),
                        offlineColor);
                    dismissSyncBarButton.Hidden = false;
                    setActivityIndicatorVisible(false);
                    shouldRefreshOnTap = true;
                    break;

                case SyncProgress.Synced:
                    setSyncIndicatorTextAndBackground(
                        Resources.SyncCompleted.EndingWithTick(syncStateLabel.Font.CapHeight),
                        syncCompletedColor);
                    hideIndicator = true;
                    setActivityIndicatorVisible(false);
                    break;

                case SyncProgress.Failed:
                    setSyncIndicatorTextAndBackground(
                        Resources.SyncFailed.EndingWithRefreshIcon(syncStateLabel.Font.CapHeight),
                        syncFailedColor);
                    dismissSyncBarButton.Hidden = false;
                    setActivityIndicatorVisible(false);
                    break;

                default:
                    throw new ArgumentException(nameof(SyncProgress));
            }

            showSyncBar();

            if (!hideIndicator) return;

            await hideSyncBar();
        }

        private void didScroll(CGPoint offset)
        {
            if (!scrollView.Dragging || wasReleased) return;

            if (offset.Y >= 0)
            {
                heightConstraint.Constant = 0;
                return;
            }

            heightConstraint.Constant = -offset.Y;
            syncStateView.SetNeedsLayout();

            var needsMorePulling = System.Math.Abs(offset.Y) < scrollThreshold;
            needsRefresh = !needsMorePulling;

            if (isSyncing) return;

            dismissSyncBarButton.Hidden = true;
            setSyncIndicatorTextAndBackground(
                new NSAttributedString(needsMorePulling ? Resources.PullDownToRefresh : Resources.ReleaseToRefresh),
                pullToRefreshColor);
        }

        private void draggingChanged(bool isDragging)
        {
            if (isDragging)
            {
                wasReleased = false;
            }
            else
            {
                var offset = scrollView.ContentOffset.Y;
                if (offset >= 0) return;

                wasReleased = true;

                if (shouldCalculateOnDeceleration) return;
                refreshIfNeeded();
            }
        }

        private void refreshIfNeeded()
        {
            if (!needsRefresh)
            {
                hideSyncBar(false);
                return;
            }

            needsRefresh = false;
            shouldCalculateOnDeceleration = false;

            if (isSyncing)
            {
                showSyncBar();
                return;
            }

            refreshSubject.OnNext(Unit.Default);
        }

        private void setSyncIndicatorTextAndBackground(NSAttributedString text, UIColor backgroundColor)
        {
            UIView.Animate(Animation.Timings.EnterTiming, () =>
            {
                syncStateLabel.AttributedText = text;
                syncStateView.BackgroundColor = backgroundColor;
            });
        }

        private void showSyncBar()
        {
            if (scrollView.Dragging) return;

            heightConstraint.Constant = syncBarHeight;
            UIView.Animate(Animation.Timings.EnterTiming, () =>
            {
                syncStateView.LayoutIfNeeded();
            });
        }

        private async Task hideSyncBar(bool withDelay = true)
        {
            if (withDelay)
                await Task.Delay(Animation.Timings.HideSyncStateViewDelay);

            heightConstraint.Constant = 0;
            UIView.Animate(Animation.Timings.EnterTiming, () =>
            {
                syncStateView.LayoutIfNeeded();
            });
        }

        private void setActivityIndicatorVisible(bool visible)
        {
            activityIndicatorView.Hidden = !visible;

            if (visible)
                activityIndicatorView.StartSpinning();
            else
                activityIndicatorView.StopSpinning();
        }

        private void onStatusLabelTap()
        {
            if (shouldRefreshOnTap == false) return;

            refreshSubject.OnNext(Unit.Default);
        }

        private void onDismissSyncBarButtonTap(object sender, EventArgs e)
        {
            hideSyncBar(false);
        }
    }
}
