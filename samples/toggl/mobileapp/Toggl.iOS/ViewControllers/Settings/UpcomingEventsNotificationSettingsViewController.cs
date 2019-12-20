using CoreGraphics;
using System.Reactive;
using Toggl.Core.UI.Collections;
using Toggl.Core.UI.Extensions;
using Toggl.Core.UI.ViewModels.Selectable;
using Toggl.Core.UI.ViewModels.Settings;
using Toggl.iOS.Cells.Settings;
using Toggl.iOS.Extensions;
using Toggl.iOS.Extensions.Reactive;
using Toggl.iOS.ViewSources.Generic.TableView;
using Toggl.Shared;
using Toggl.Shared.Extensions;
using UIKit;

namespace Toggl.iOS.ViewControllers.Settings
{
    using CalendarSectionModel = SectionModel<Unit, SelectableCalendarNotificationsOptionViewModel>;

    public sealed partial class UpcomingEventsNotificationSettingsViewController : ReactiveViewController<UpcomingEventsNotificationSettingsViewModel>
    {

        private const int rowHeight = 44;

        public UpcomingEventsNotificationSettingsViewController(UpcomingEventsNotificationSettingsViewModel viewModel)
            : base(viewModel, nameof(UpcomingEventsNotificationSettingsViewController))
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            TitleLabel.Text = Resources.UpcomingEvent;

            TableView.ScrollEnabled = false;
            TableView.TableFooterView = new UIView(CGRect.Empty);
            TableView.RegisterNibForCellReuse(UpcomingEventsOptionCell.Nib, UpcomingEventsOptionCell.Identifier);
            TableView.RowHeight = rowHeight;
            TableView.SeparatorStyle = UITableViewCellSeparatorStyle.None;

            var source = new CustomTableViewSource<CalendarSectionModel, Unit, SelectableCalendarNotificationsOptionViewModel>(
                UpcomingEventsOptionCell.CellConfiguration(UpcomingEventsOptionCell.Identifier),
                ViewModel.AvailableOptions
            );

            source.Rx().ModelSelected()
                .Subscribe(ViewModel.SelectOption.Inputs)
                .DisposedBy(DisposeBag);

            TableView.Source = source;

            CloseButton.Rx().Tap()
                .Subscribe(() => ViewModel.CloseWithDefaultResult())
                .DisposedBy(DisposeBag);
        }
    }
}
