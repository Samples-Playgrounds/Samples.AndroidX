using Foundation;
using System;
using System.Collections.Immutable;
using Toggl.Core.UI.Collections;
using Toggl.Core.UI.ViewModels.Calendar;
using Toggl.Core.UI.ViewModels.Selectable;
using Toggl.iOS.Cells;
using Toggl.iOS.Cells.Calendar;
using Toggl.Shared;
using Toggl.Shared.Extensions;
using UIKit;

namespace Toggl.iOS.ViewSources
{
    using CalendarSectionModel = SectionModel<UserCalendarSourceViewModel, SelectableUserCalendarViewModel>;
    public sealed class SelectUserCalendarsTableViewSource : BaseTableViewSource<CalendarSectionModel, UserCalendarSourceViewModel, SelectableUserCalendarViewModel>
    {
        private const int rowHeight = 48;
        private const int headerHeight = 48;
        private readonly InputAction<SelectableUserCalendarViewModel> selectCalendar;

        public SelectUserCalendarsTableViewSource(UITableView tableView, InputAction<SelectableUserCalendarViewModel> selectCalendar)
            : base(ImmutableList<SectionModel<UserCalendarSourceViewModel, SelectableUserCalendarViewModel>>.Empty)
        {
            Ensure.Argument.IsNotNull(selectCalendar, nameof(selectCalendar));

            this.selectCalendar = selectCalendar;

            tableView.RegisterNibForCellReuse(SelectableUserCalendarViewCell.Nib, SelectableUserCalendarViewCell.Identifier);
            tableView.RegisterNibForHeaderFooterViewReuse(UserCalendarListHeaderViewCell.Nib, UserCalendarListHeaderViewCell.Identifier);
            tableView.SectionHeaderHeight = headerHeight;
            tableView.RowHeight = rowHeight;
        }

        public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            var cell = (BaseTableViewCell<SelectableUserCalendarViewModel>)tableView.DequeueReusableCell(
                SelectableUserCalendarViewCell.Identifier, indexPath);

            if (cell is SelectableUserCalendarViewCell selectableUserCalendarViewCell)
                selectableUserCalendarViewCell.SelectCalendar = selectCalendar;

            cell.Item = ModelAt(indexPath);
            return cell;
        }

        public override UIView GetViewForHeader(UITableView tableView, nint section)
        {
            var header = tableView.DequeueReusableHeaderFooterView(UserCalendarListHeaderViewCell.Identifier) as UserCalendarListHeaderViewCell;
            header.Item = HeaderOf(section);
            return header;
        }
    }
}
