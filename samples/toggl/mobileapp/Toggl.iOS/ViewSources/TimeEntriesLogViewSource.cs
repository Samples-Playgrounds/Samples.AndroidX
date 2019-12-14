using Foundation;
using System;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using Toggl.Core.UI.Collections;
using Toggl.Core.UI.Extensions;
using Toggl.Core.UI.ViewModels.TimeEntriesLog;
using Toggl.Core.UI.ViewModels.TimeEntriesLog.Identity;
using Toggl.iOS.Extensions;
using Toggl.iOS.Views;
using Toggl.Shared;
using Toggl.Shared.Extensions;
using Toggl.Shared.Models;
using UIKit;

namespace Toggl.iOS.ViewSources
{
    using MainLogSection = AnimatableSectionModel<DaySummaryViewModel, LogItemViewModel, IMainLogKey>;

    public sealed class TimeEntriesLogViewSource
        : BaseTableViewSource<MainLogSection, DaySummaryViewModel, LogItemViewModel>
    {
        private const int rowHeightCompact = 64;
        private const int rowHeightRegular = 48;
        private const int headerHeight = 48;

        public delegate IObservable<DaySummaryViewModel> ObservableHeaderForSection(int section);

        private readonly Subject<LogItemViewModel> continueTapSubject = new Subject<LogItemViewModel>();
        private readonly Subject<LogItemViewModel> continueSwipeSubject = new Subject<LogItemViewModel>();
        private readonly Subject<LogItemViewModel> deleteSwipeSubject = new Subject<LogItemViewModel>();
        private readonly Subject<GroupId> toggleGroupExpansionSubject = new Subject<GroupId>();

        private readonly ReplaySubject<TimeEntriesLogViewCell> firstCellSubject = new ReplaySubject<TimeEntriesLogViewCell>(1);
        private readonly Subject<bool> isDraggingSubject = new Subject<bool>();

        private bool swipeActionsEnabled = true;

        public const int SpaceBetweenSections = 20;

        public IObservable<LogItemViewModel> ContinueTap { get; }
        public IObservable<LogItemViewModel> SwipeToContinue { get; }
        public IObservable<LogItemViewModel> SwipeToDelete { get; }
        public IObservable<GroupId> ToggleGroupExpansion { get; }

        public IObservable<TimeEntriesLogViewCell> FirstCell { get; }
        public IObservable<bool> IsDragging { get; }

        public TimeEntriesLogViewSource()
        {
            if (!NSThread.Current.IsMainThread)
            {
                throw new InvalidOperationException($"{nameof(TimeEntriesLogViewSource)} must be created on the main thread");
            }

            ContinueTap = continueTapSubject.AsObservable();
            SwipeToContinue = continueSwipeSubject.AsObservable();
            SwipeToDelete = deleteSwipeSubject.AsObservable();
            ToggleGroupExpansion = toggleGroupExpansionSubject.AsObservable();

            FirstCell = firstCellSubject.AsObservable();
            IsDragging = isDraggingSubject.AsObservable();
        }

        public void SetSwipeActionsEnabled(bool enabled)
        {
            swipeActionsEnabled = enabled;
        }

        public override nfloat GetHeightForHeader(UITableView tableView, nint section) => headerHeight + SpaceBetweenSections;

        public override nfloat GetHeightForRow(UITableView tableView, NSIndexPath indexPath) =>
            tableView.TraitCollection.HorizontalSizeClass == UIUserInterfaceSizeClass.Regular
            ? rowHeightRegular
            : rowHeightCompact;

        // It needs this method, otherwise the ContentOffset will reset to 0 everytime the table reloads. ¯\_(ツ)_/¯
        public override nfloat EstimatedHeight(UITableView tableView, NSIndexPath indexPath) =>
            tableView.TraitCollection.HorizontalSizeClass == UIUserInterfaceSizeClass.Regular
            ? rowHeightRegular
            : rowHeightCompact;

        public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            var cell = (TimeEntriesLogViewCell)tableView.DequeueReusableCell(TimeEntriesLogViewCell.Identifier);

            var model = ModelAt(indexPath);

            cell.ContinueButtonTap
                .Subscribe(() => continueTapSubject.OnNext(model))
                .DisposedBy(cell.DisposeBag);

            cell.ToggleGroup
                .Subscribe(() => toggleGroupExpansionSubject.OnNext(model.GroupId))
                .DisposedBy(cell.DisposeBag);

            if (indexPath.Row == 0 && indexPath.Section == 0)
                firstCellSubject.OnNext(cell);

            cell.Item = model;

            return cell;
        }

        public override UIView GetViewForHeader(UITableView tableView, nint section)
        {
            var header = (TimeEntriesLogHeaderView)tableView.DequeueReusableHeaderFooterView(TimeEntriesLogHeaderView.Identifier);
            header.Item = HeaderOf((int)section);
            return header;
        }

        public override UISwipeActionsConfiguration GetLeadingSwipeActionsConfiguration(UITableView tableView,
            NSIndexPath indexPath)
        {
            if (!swipeActionsEnabled)
            {
                return createDisabledActionConfiguration();
            }

            return createSwipeActionConfiguration(continueSwipeActionFor, indexPath);
        }

        public override UISwipeActionsConfiguration GetTrailingSwipeActionsConfiguration(UITableView tableView, NSIndexPath indexPath)
        {
            if (!swipeActionsEnabled)
            {
                return createDisabledActionConfiguration();
            }

            return createSwipeActionConfiguration(deleteSwipeActionFor, indexPath);
        }

        public override void DraggingStarted(UIScrollView scrollView)
        {
            isDraggingSubject.OnNext(true);
        }

        public override void DraggingEnded(UIScrollView scrollView, bool willDecelerate)
        {
            isDraggingSubject.OnNext(false);
        }

        private UISwipeActionsConfiguration createSwipeActionConfiguration(
            Func<LogItemViewModel, UIContextualAction> factory, NSIndexPath indexPath)
        {
            var item = ModelAt(indexPath);
            if (item == null)
                return null;

            return UISwipeActionsConfiguration.FromActions(new[] { factory(item) });
        }

        private UIContextualAction continueSwipeActionFor(LogItemViewModel viewModel)
        {
            var continueAction = UIContextualAction.FromContextualActionStyle(
                UIContextualActionStyle.Normal,
                Resources.Continue,
                (action, sourceView, completionHandler) =>
                {
                    continueSwipeSubject.OnNext(viewModel);
                    completionHandler.Invoke(finished: true);
                }
            );
            continueAction.BackgroundColor = Core.UI.Helper.Colors.TimeEntriesLog.ContinueSwipeActionBackground.ToNativeColor();
            return continueAction;
        }

        private UIContextualAction deleteSwipeActionFor(LogItemViewModel viewModel)
        {
            var deleteAction = UIContextualAction.FromContextualActionStyle(
                UIContextualActionStyle.Destructive,
                Resources.Delete,
                (action, sourceView, completionHandler) =>
                {
                    deleteSwipeSubject.OnNext(viewModel);
                    completionHandler.Invoke(finished: true);
                }
            );
            deleteAction.BackgroundColor = Core.UI.Helper.Colors.TimeEntriesLog.DeleteSwipeActionBackground.ToNativeColor();
            return deleteAction;
        }

        private UISwipeActionsConfiguration createDisabledActionConfiguration()
        {
            var swipeAction = UISwipeActionsConfiguration.FromActions(new UIContextualAction[]{});
            swipeAction.PerformsFirstActionWithFullSwipe = false;
            return swipeAction;
        }
    }
}
