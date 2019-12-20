using Foundation;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using Toggl.Core.UI.Collections;
using Toggl.Shared;
using UIKit;

namespace Toggl.iOS.ViewSources.Generic.TableView
{
    public delegate UITableViewCell CellConfiguration<TModel>(UITableView tableView, NSIndexPath indexPath, TModel model);
    public delegate UIView HeaderConfiguration<THeader>(UITableView tableView, nint section, THeader header);

    public sealed class CustomTableViewSource<TSection, THeader, TItem>
        : BaseTableViewSource<TSection, THeader, TItem>
        where TSection : ISectionModel<THeader, TItem>, new()
    {
        private readonly CellConfiguration<TItem> configureCell;
        private readonly HeaderConfiguration<THeader> configureHeader;

        public CustomTableViewSource(
            CellConfiguration<TItem> configureCell,
            IImmutableList<TItem> items = null)
            : this(configureCell, null, items)
        {
        }

        public CustomTableViewSource(
            CellConfiguration<TItem> configureCell,
            IImmutableList<TSection> sections)
            : this(configureCell, null, sections)
        {
        }

        public CustomTableViewSource(
            CellConfiguration<TItem> configureCell,
            HeaderConfiguration<THeader> configureHeader,
            IImmutableList<TItem> items = null)
            : base(items)
        {
            Ensure.Argument.IsNotNull(configureCell, nameof(configureCell));

            this.configureCell = configureCell;
            this.configureHeader = configureHeader;
        }

        public CustomTableViewSource(
            CellConfiguration<TItem> configureCell,
            HeaderConfiguration<THeader> configureHeader,
            IImmutableList<TSection> sections = null)
            : base(sections)
        {
            Ensure.Argument.IsNotNull(configureCell, nameof(configureCell));

            this.configureCell = configureCell;
            this.configureHeader = configureHeader;
        }

        public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
            => configureCell(tableView, indexPath, ModelAt(indexPath));

        public override UIView GetViewForHeader(UITableView tableView, nint section)
            => configureHeader?.Invoke(tableView, section, HeaderOf(section));

        public override nfloat GetHeightForHeader(UITableView tableView, nint section)
            => configureHeader == null ? 0 : tableView.SectionHeaderHeight;
    }
}
