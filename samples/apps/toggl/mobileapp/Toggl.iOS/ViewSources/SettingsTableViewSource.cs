using System;
using Foundation;
using Toggl.Core.Sync;
using Toggl.Core.UI.Collections;
using Toggl.Core.UI.Helper;
using Toggl.iOS.Cells.Settings;
using Toggl.iOS.Extensions;
using Toggl.iOS.ViewControllers.Settings.Models;
using UIKit;

namespace Toggl.iOS.ViewSources
{
    using SettingsSection = SectionModel<string, ISettingRow>;

    public class SettingsTableViewSource: BaseTableViewSource<SettingsSection, string, ISettingRow>
    {
        private readonly float headerHeight = 48;
        private readonly float emptyHeaderHeight = 24;

        public SettingsTableViewSource(UITableView tableView)
        {
            tableView.SeparatorStyle = UITableViewCellSeparatorStyle.None;
            tableView.RegisterNibForCellReuse(SettingCell.Nib, SettingCell.Identifier);
            tableView.RegisterNibForCellReuse(SettingsAnnotationCell.Nib, SettingsAnnotationCell.Identifier);
            tableView.RegisterNibForCellReuse(SettingsSyncCell.Nib, SettingsSyncCell.Identifier);
            tableView.RegisterNibForCellReuse(SettingsButtonCell.Nib, SettingsButtonCell.Identifier);
            tableView.RowHeight = UITableView.AutomaticDimension;
            tableView.RegisterNibForHeaderFooterViewReuse(SettingsSectionHeader.Nib, SettingsSectionHeader.Identifier);
        }

        public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            var model = ModelAt(indexPath);

            switch (model)
            {
                case AnnotationRow annotationRow:
                {
                    var cell = (SettingsAnnotationCell) tableView.DequeueReusableCell(SettingsAnnotationCell.Identifier,
                        indexPath);
                    cell.Item = annotationRow;
                    return cell;
                }

                case CustomRow<PresentableSyncStatus> customRow:
                {
                    var cell = (SettingsSyncCell) tableView.DequeueReusableCell(
                        SettingsSyncCell.Identifier,
                        indexPath);
                    cell.Item = customRow;
                    return cell;
                }

                case ButtonRow buttonRow:
                {
                    var cell = (SettingsButtonCell) tableView.DequeueReusableCell(
                        SettingsButtonCell.Identifier,
                        indexPath);
                    cell.Item = buttonRow;
                    return cell;
                }

                default:
                {
                    var cell = (SettingCell)tableView.DequeueReusableCell(SettingCell.Identifier, indexPath);
                    cell.Item = model;
                    return cell;
                }
            }
        }

        public override nfloat GetHeightForHeader(UITableView tableView, nint section)
        {
            var title = HeaderOf(section);

            if (string.IsNullOrEmpty(title))
            {
                return emptyHeaderHeight;
            }

            return headerHeight;
        }

        public override UIView GetViewForHeader(UITableView tableView, nint section)
        {
            var title = HeaderOf(section);

            if (string.IsNullOrEmpty(title))
            {
                var view = new UIView();
                view.BackgroundColor = ColorAssets.TableBackground;
                return view;
            }

            var header = (SettingsSectionHeader)tableView.DequeueReusableHeaderFooterView(SettingsSectionHeader.Identifier);
            header.Item = title;
            return header;
        }

        public override NSIndexPath WillSelectRow(UITableView tableView, NSIndexPath indexPath)
        {
            var settingsRow = Sections[indexPath.Section].Items[indexPath.Row];

            if (settingsRow is NavigationRow || settingsRow is ButtonRow)
            {
                return indexPath;
            }

            return null;
        }
    }
}
