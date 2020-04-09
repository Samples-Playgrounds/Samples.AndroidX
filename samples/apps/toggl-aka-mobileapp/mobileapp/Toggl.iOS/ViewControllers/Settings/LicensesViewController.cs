using System.Collections.Immutable;
using System.Linq;
using Toggl.Core.UI.Collections;
using Toggl.Core.UI.ViewModels;
using Toggl.iOS.Cells.Settings;
using Toggl.iOS.Extensions;
using Toggl.iOS.ViewSources.Generic.TableView;
using Toggl.Shared;
using UIKit;

namespace Toggl.iOS.ViewControllers
{
    public sealed class LicensesViewController : ReactiveTableViewController<LicensesViewModel>
    {
        public LicensesViewController(LicensesViewModel viewModel)
            : base(viewModel)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            Title = Resources.Licenses;

            TableView.EstimatedRowHeight = 396;
            TableView.SectionHeaderHeight = 44;
            TableView.RowHeight = UITableView.AutomaticDimension;

            TableView.RegisterNibForCellReuse(LicensesViewCell.Nib, LicensesViewCell.Identifier);
            TableView.RegisterNibForHeaderFooterViewReuse(LicensesHeaderViewCell.Nib,
                LicensesHeaderViewCell.Identifier);

            var sectionedLicenses = ViewModel.Licenses
                .Select(license => new SectionModel<License, License>(license, new[] { license }))
                .ToImmutableList();

            var source = new CustomTableViewSource<SectionModel<License, License>, License, License>(
                LicensesViewCell.CellConfiguration(LicensesViewCell.Identifier),
                LicensesHeaderViewCell.HeaderConfiguration,
                sectionedLicenses
            );

            TableView.Source = source;
            TableView.BackgroundColor = ColorAssets.TableBackground;
        }
    }
}
