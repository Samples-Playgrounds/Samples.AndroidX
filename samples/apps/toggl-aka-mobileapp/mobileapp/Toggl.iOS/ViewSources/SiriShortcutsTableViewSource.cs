using Foundation;
using System;
using Toggl.Core.UI.Collections;
using Toggl.iOS.Cells;
using Toggl.iOS.ViewControllers.Settings;
using Toggl.iOS.Views.Settings;
using UIKit;

namespace Toggl.iOS.ViewSources
{
    using SiriShortcutsSection = SectionModel<string, SiriShortcutViewModel>;

    public class SiriShortcutsTableViewSource : BaseTableViewSource<SiriShortcutsSection, string, SiriShortcutViewModel>
    {
        public SiriShortcutsTableViewSource(UITableView tableView)
        {
            tableView.SeparatorStyle = UITableViewCellSeparatorStyle.None;
            tableView.RegisterNibForCellReuse(SiriShortcutCell.Nib, SiriShortcutCell.Identifier);
            tableView.RegisterNibForCellReuse(CustomSiriShortcutCell.Nib, CustomSiriShortcutCell.Identifier);
            tableView.RegisterNibForHeaderFooterViewReuse(SiriShortcutTableViewHeader.Nib, SiriShortcutTableViewHeader.Identifier);
            tableView.SectionHeaderHeight = 48;
        }

        public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            var model = ModelAt(indexPath);

            var identifier = model.IsCustomStart
                ? CustomSiriShortcutCell.Identifier
                : SiriShortcutCell.Identifier;

            var cell = (BaseTableViewCell<SiriShortcutViewModel>)tableView.DequeueReusableCell(identifier, indexPath);
            cell.Item = model;
            return cell;
        }

        public override UIView GetViewForHeader(UITableView tableView, nint section)
        {
            var header = (SiriShortcutTableViewHeader)tableView.DequeueReusableHeaderFooterView(SiriShortcutTableViewHeader.Identifier);
            header.Item = HeaderOf(section);
            return header;
        }

        public override nfloat GetHeightForRow(UITableView tableView, NSIndexPath indexPath)
        {
            var model = ModelAt(indexPath);
            return model.IsCustomStart ? 64 : 44;
        }
    }
}
