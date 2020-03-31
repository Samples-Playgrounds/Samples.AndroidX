using System;
using System.Collections.Immutable;
using System.Linq;
using System.Reactive;
using Foundation;
using Toggl.Core.UI.Collections;
using Toggl.Core.UI.Helper;
using Toggl.Core.UI.Views;
using Toggl.iOS.Cells.Common;
using Toggl.iOS.Extensions;
using UIKit;

namespace Toggl.iOS.ViewSources.Common
{
    public sealed class SelectorTableViewSource : BaseTableViewSource<SectionModel<Unit, string>, Unit, string>
    {
        private readonly int initialSelection;
        private readonly Action<int> selectItem;

        public SelectorTableViewSource(UITableView tableView, int initialSelection, Action<int> selectItem)
        {
            this.initialSelection = initialSelection;
            this.selectItem = selectItem;

            tableView.RegisterNibForCellReuse(SelectorCell.Nib, SelectorCell.Identifier);
        }

        public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            var model = ModelAt(indexPath);
            var cell = (SelectorCell)tableView.DequeueReusableCell(SelectorCell.Identifier);
            cell.Item = model;
            cell.OptionSelected = indexPath.Row == initialSelection;
            if (indexPath.Row == Sections[indexPath.Section].Items.Count - 1)
            {
                cell.SeparatorInset = UIEdgeInsets.Zero;
            }

            return cell;
        }

        public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
        {
            base.RowSelected(tableView, indexPath);
            selectItem(indexPath.Row);
        }

        public override nfloat GetHeightForHeader(UITableView tableView, nint section)
        {
            return 1 / UIScreen.MainScreen.Scale;
        }

        public override UIView GetViewForHeader(UITableView tableView, nint section)
        {
            var headerView = new UIView();
            headerView.BackgroundColor = ColorAssets.Separator;
            return headerView;
        }
    }
}
