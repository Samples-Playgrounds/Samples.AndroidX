using Android.Widget;
using System;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using AndroidX.RecyclerView.Widget;
using Toggl.Core.Sync;
using Toggl.Core.UI.Onboarding.MainView;
using Toggl.Droid.Adapters;
using Toggl.Droid.Extensions;
using Toggl.Droid.Extensions.Reactive;
using Toggl.Droid.Helper;
using Toggl.Droid.ViewHolders;
using Toggl.Shared.Extensions;

namespace Toggl.Droid.Fragments
{
    public sealed partial class MainFragment
    {
        private PopupWindow playButtonTooltipPopupWindow;
        private PopupWindow stopButtonTooltipPopupWindow;
        private PopupWindow tapToEditPopup;

        private readonly BehaviorSubject<int> timeEntriesCountSubject = new BehaviorSubject<int>(0);
        private EditTimeEntryOnboardingStep editTimeEntryOnboardingStep;
        private IDisposable editTimeEntryOnboardingStepDisposable;

        private IObservable<Unit> mainRecyclerViewChangesObservable;
        private ISubject<Unit> mainRecyclerViewScrollChanges = new Subject<Unit>();
        private IDisposable mainRecyclerViewScrollChangesDisposable;

        public override void OnResume()
        {
            base.OnResume();
            mainRecyclerViewScrollChangesDisposable = mainRecyclerView
                .Rx()
                .OnScrolled()
                .ObserveOn(AndroidDependencyContainer.Instance.SchedulerProvider.MainScheduler)
                .Subscribe(mainRecyclerViewScrollChanges.OnNext);
        }

        public override void OnPause()
        {
            base.OnPause();
            mainRecyclerViewScrollChangesDisposable?.Dispose();
        }

        public override void OnStop()
        {
            base.OnStop();
            playButtonTooltipPopupWindow?.Dismiss();
            stopButtonTooltipPopupWindow?.Dismiss();
            tapToEditPopup?.Dismiss();
            playButtonTooltipPopupWindow = null;
            stopButtonTooltipPopupWindow = null;
            tapToEditPopup = null;
        }

        private void setupOnboardingSteps()
        {
            setupMainLogObservables();
            setupStartTimeEntryOnboardingStep();
            setupStopTimeEntryOnboardingStep();
            setupTapToEditOnboardingStep();
        }

        private void setupMainLogObservables()
        {
            var collectionChanges = ViewModel.TimeEntries.SelectUnit();
            mainRecyclerViewChangesObservable = mainRecyclerViewScrollChanges
                .Merge(collectionChanges);
        }

        private void setupStartTimeEntryOnboardingStep()
        {
            if (playButtonTooltipPopupWindow == null)
            {
                playButtonTooltipPopupWindow = PopupWindowFactory.PopupWindowWithText(
                    Context,
                    Resource.Layout.TooltipWithRightArrow,
                    Resource.Id.TooltipText,
                    Shared.Resources.TapToStartTimer);
            }

            new StartTimeEntryOnboardingStep(ViewModel.OnboardingStorage)
                .ManageDismissableTooltip(
                    visibilityChanged,
                    playButtonTooltipPopupWindow,
                    playButton,
                    (popup, anchor) => popup.LeftVerticallyCenteredOffsetsTo(anchor, dpExtraRightMargin: 8),
                    ViewModel.OnboardingStorage)
                .DisposedBy(DisposeBag);
        }

        private void setupStopTimeEntryOnboardingStep()
        {
            if (stopButtonTooltipPopupWindow == null)
            {
                stopButtonTooltipPopupWindow = PopupWindowFactory.PopupWindowWithText(
                    Context,
                    Resource.Layout.TooltipWithRightBottomArrow,
                    Resource.Id.TooltipText,
                    Shared.Resources.TapToStopTimer);
            }

            new StopTimeEntryOnboardingStep(ViewModel.OnboardingStorage, ViewModel.IsTimeEntryRunning)
                .ManageDismissableTooltip(
                    visibilityChanged,
                    stopButtonTooltipPopupWindow,
                    stopButton,
                    (popup, anchor) => popup.TopRightFrom(anchor, dpExtraBottomMargin: 8),
                    ViewModel.OnboardingStorage)
                .DisposedBy(DisposeBag);
        }

        private void setupTapToEditOnboardingStep()
        {
            tapToEditPopup = PopupWindowFactory.PopupWindowWithText(
                Context,
                Resource.Layout.TooltipWithLeftTopArrow,
                Resource.Id.TooltipText,
                Shared.Resources.TapToEditIt);

            editTimeEntryOnboardingStep = new EditTimeEntryOnboardingStep(
                ViewModel.OnboardingStorage, Observable.Return(false));

            var showTapToEditOnboardingStepObservable =
                Observable.CombineLatest(
                    editTimeEntryOnboardingStep.ShouldBeVisible,
                    mainRecyclerViewChangesObservable,
                    ViewModel.SyncProgressState,
                    (shouldShowStep, unit, syncState) => shouldShowStep && syncState == SyncProgress.Synced);

            showTapToEditOnboardingStepObservable
                .Where(shouldShowStep => shouldShowStep)
                .Select(_ => findOldestTimeEntryView())
                .ObserveOn(AndroidDependencyContainer.Instance.SchedulerProvider.MainScheduler)
                .Subscribe(updateTapToEditOnboardingStep)
                .DisposedBy(DisposeBag);
        }

        private void updateTapToEditOnboardingStep(MainLogCellViewHolder oldestVisibleTimeEntryViewHolder)
        {
            tapToEditPopup?.Dismiss();

            if (oldestVisibleTimeEntryViewHolder == null)
                return;

            if (editTimeEntryOnboardingStepDisposable != null)
            {
                editTimeEntryOnboardingStepDisposable.Dispose();
                editTimeEntryOnboardingStepDisposable = null;
            }

            editTimeEntryOnboardingStepDisposable = editTimeEntryOnboardingStep
                .ManageVisibilityOf(
                    visibilityChanged,
                    tapToEditPopup,
                    oldestVisibleTimeEntryViewHolder.ItemView,
                    (window, view) => PopupOffsets.FromDp(16, -4, Context));
        }

        private MainLogCellViewHolder findOldestTimeEntryView()
        {
            if (mainRecyclerAdapter == null)
            {
                return null;
            }

            for (var position = layoutManager.FindLastVisibleItemPosition(); position >= 0; position--)
            {
                var viewType = mainRecyclerAdapter.GetItemViewType(position);
                if (viewType != MainRecyclerAdapter.ItemViewType)
                {
                    continue;
                }

                var viewHolder = findLogCellViewHolderAtPosition(position);
                if (viewHolder == null)
                    return null;

                return isVisible(viewHolder)
                    ? viewHolder
                    : null;
            }

            return null;
        }

        private MainLogCellViewHolder findLogCellViewHolderAtPosition(int position)
        {
            var viewHolder = mainRecyclerView.FindViewHolderForLayoutPosition(position);

            if (viewHolder == null)
                return null;

            if (viewHolder is MainLogCellViewHolder logViewHolder)
                return logViewHolder;

            return null;
        }

        private bool isVisible(RecyclerView.ViewHolder view)
        {
            return layoutManager.IsViewPartiallyVisible(view.ItemView, true, true)
                   || layoutManager.IsViewPartiallyVisible(view.ItemView, false, true);
        }
    }
}
