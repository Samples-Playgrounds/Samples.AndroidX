using System.Reactive;
using Toggl.Core.Models;
using Toggl.Core.UI.Collections;
using Toggl.Core.UI.ViewModels;
using Toggl.iOS.Cells;
using Toggl.iOS.ViewSources.Generic.TableView;
using UIKit;

namespace Toggl.iOS.ViewControllers
{
    public sealed class SyncFailuresViewController : ReactiveTableViewController<SyncFailuresViewModel>
    {
        public SyncFailuresViewController(SyncFailuresViewModel viewModel) : base(viewModel)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            TableView.RowHeight = UITableView.AutomaticDimension;
            TableView.RegisterNibForCellReuse(SyncFailureCell.Nib, SyncFailureCell.Identifier);
            var tableViewSource = new CustomTableViewSource<SectionModel<Unit, SyncFailureItem>, Unit, SyncFailureItem>(
                SyncFailureCell.CellConfiguration(SyncFailureCell.Identifier),
                ViewModel.SyncFailures
            );
            TableView.Source = tableViewSource;
        }
    }
}
