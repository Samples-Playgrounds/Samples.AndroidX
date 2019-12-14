using CoreGraphics;
using Foundation;
using System;
using System.Collections.Immutable;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using Toggl.Core.Reports;
using Toggl.Core.UI.Collections;
using Toggl.Core.UI.Helper;
using Toggl.Core.UI.ViewModels.Reports;
using Toggl.iOS.Extensions;
using Toggl.iOS.Extensions.Reactive;
using Toggl.iOS.Views.Reports;
using Toggl.iOS.ViewSources.Generic.TableView;
using Toggl.Shared.Extensions;
using UIKit;

namespace Toggl.iOS.ViewSources
{
    using ReportsSection = SectionModel<ReportsViewModel, ChartSegment>;
    public sealed class ReportsTableViewSource : BaseTableViewSource<ReportsSection, ReportsViewModel, ChartSegment>
    {
        private const int summaryHeightRegular = 308;
        private const int summaryHeightCompact = 768;
        private const int topAndBottomPaddingRegular = 12;
        private const int bottomWorkspaceSelectionButtonInset = 76;

        private readonly CompositeDisposable disposeBag = new CompositeDisposable();
        private readonly ReportsViewModel viewModel;
        private readonly UITableView tableView;

        public IObservable<CGPoint> ScrolledWithHeaderOffset { get; }

        private readonly nfloat bottomHeight = 78;
        private readonly nfloat rowHeight = 56;

        private readonly CellConfiguration<ChartSegment> cellConfiguration =
            ReportsLegendViewCell.CellConfiguration(ReportsLegendViewCell.Identifier);

        private nfloat headerHeight
            => tableView.TraitCollection.HorizontalSizeClass == UIUserInterfaceSizeClass.Regular
                ? summaryHeightRegular
                : summaryHeightCompact;

        private nfloat topPadding
            => tableView.TraitCollection.HorizontalSizeClass == UIUserInterfaceSizeClass.Regular
                ? topAndBottomPaddingRegular
                : 0;

        private nfloat bottomPadding
            => topPadding + (showWorkspaceButton ? bottomWorkspaceSelectionButtonInset : 0);

        private UIView headerViewSpacer = new UIView();

        private bool showWorkspaceButton = false;

        public ReportsTableViewSource(UITableView tableView, ReportsViewModel viewModel)
        {
            this.viewModel = viewModel;
            this.tableView = tableView;

            headerViewSpacer.Frame = new CGRect(0, 0, tableView.Bounds.Width, headerHeight);
            tableView.TableHeaderView = headerViewSpacer;
            tableView.ContentInset = new UIEdgeInsets(-headerHeight + topPadding, 0, bottomPadding, 0);
            tableView.SeparatorStyle = UITableViewCellSeparatorStyle.None;
            tableView.RowHeight = rowHeight;
            tableView.SectionHeaderHeight = headerHeight;
            tableView.RegisterNibForCellReuse(ReportsLegendViewCell.Nib, ReportsLegendViewCell.Identifier);
            tableView.RegisterNibForHeaderFooterViewReuse(ReportsHeaderView.Nib, ReportsHeaderView.Identifier);
            tableView.BackgroundColor = ColorAssets.TableBackground;

            this.viewModel.WorkspacesObservable
                .Select(workspaces => workspaces.Count)
                .Subscribe(updateWorkspaceCount)
                .DisposedBy(disposeBag);

            ScrolledWithHeaderOffset = this.Rx().Scrolled()
                .Select(offset => new CGPoint(offset.X, offset.Y - headerHeight));
        }

        public void UpdateContentInset()
        {
            tableView.ContentInset = new UIEdgeInsets(-headerHeight + topPadding, 0, bottomPadding, 0);
            headerViewSpacer.Frame = new CGRect(0, 0, tableView.Bounds.Width, headerHeight);
        }

        public void UpdateContentInset(bool workspacesButtonIsShown)
        {
            showWorkspaceButton = workspacesButtonIsShown;
            UpdateContentInset();
        }

        private void updateWorkspaceCount(int workspaceCount)
        {
            tableView.ContentInset = new UIEdgeInsets(-headerHeight, 0, workspaceCount > 1 ? bottomHeight : 0, 0);
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (!disposing) return;

            disposeBag.Dispose();
        }

        public override void SetSections(IImmutableList<ReportsSection> sections)
        {
            if (sections.Count == 0)
            {
                var newSection = new ReportsSection();
                newSection.Initialize(default, ImmutableList<ChartSegment>.Empty);
                sections = ImmutableList.Create(newSection);
            }
            base.SetSections(sections);
        }

        public override UIView GetViewForHeader(UITableView tableView, nint section)
        {
            var header = tableView.DequeueReusableHeaderFooterView(ReportsHeaderView.Identifier) as ReportsHeaderView;
            header.Item = viewModel;
            return header;
        }

        public override nfloat GetHeightForHeader(UITableView tableView, nint section)
        {
            return headerHeight;
        }

        public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            var cell = (ReportsLegendViewCell)cellConfiguration(tableView, indexPath, ModelAt(indexPath));
            cell.SetIsLast(indexPath.Row == Sections[(int)indexPath.Section].Items.Count - 1);
            return cell;
        }
    }
}
