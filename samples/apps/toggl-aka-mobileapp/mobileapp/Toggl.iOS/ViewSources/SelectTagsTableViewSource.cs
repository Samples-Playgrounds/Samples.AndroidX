using Foundation;
using Toggl.Core.UI.Collections;
using Toggl.Core.UI.ViewModels;
using Toggl.iOS.Cells;
using Toggl.iOS.Views.Tag;
using UIKit;

namespace Toggl.iOS.ViewSources
{
    using TagsSection = SectionModel<string, SelectableTagBaseViewModel>;

    public sealed class SelectTagsTableViewSource : BaseTableViewSource<TagsSection, string, SelectableTagBaseViewModel>
    {
        public const int RowHeight = 48;

        public SelectTagsTableViewSource(UITableView tableView)
        {
            tableView.RowHeight = RowHeight;
            tableView.RegisterNibForCellReuse(NewTagViewCell.Nib, NewTagViewCell.Identifier);
            tableView.RegisterNibForCellReuse(CreateTagViewCell.Nib, CreateTagViewCell.Identifier);
            tableView.SeparatorStyle = UITableViewCellSeparatorStyle.None;
            tableView.Source = this;
        }

        public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            var model = ModelAt(indexPath);
            var identifier = model is SelectableTagCreationViewModel ? CreateTagViewCell.Identifier : NewTagViewCell.Identifier;
            var cell = (BaseTableViewCell<SelectableTagBaseViewModel>)tableView.DequeueReusableCell(identifier);
            cell.Item = model;
            return cell;
        }
    }
}
