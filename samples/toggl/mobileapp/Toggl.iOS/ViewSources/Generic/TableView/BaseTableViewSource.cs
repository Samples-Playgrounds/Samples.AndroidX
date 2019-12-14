using CoreGraphics;
using Foundation;
using System;
using System.Collections.Immutable;
using System.Linq;
using Toggl.Core.UI.Collections;
using UIKit;

namespace Toggl.iOS.ViewSources
{
    public abstract class BaseTableViewSource<TSection, THeader, TModel> : UITableViewSource
    where TSection : ISectionModel<THeader, TModel>, new()
    {
        public IImmutableList<TSection> Sections { get; private set; }

        public EventHandler<TModel> OnItemTapped { get; set; }
        public EventHandler<CGPoint> OnScrolled { get; set; }
        public EventHandler OnDragStarted { get; set; }

        // This one is emitted from UITableViewExtensions whenever the datasource changes are applied
        public EventHandler OnItemsChanged { get; set; }

        public BaseTableViewSource()
            : this(ImmutableList<TModel>.Empty)
        {
        }

        public BaseTableViewSource(IImmutableList<TModel> items)
        {
            SetItems(items);
        }

        public BaseTableViewSource(IImmutableList<TSection> sections)
        {
            SetSections(sections);
        }

        public void SetItems(IImmutableList<TModel> items)
        {
            var sections = ImmutableList<TSection>.Empty;
            if (items != null && items.Any())
            {
                var newSection = new TSection();
                newSection.Initialize(default, items);
                sections = ImmutableList.Create(newSection);
            }

            SetSections(sections);
        }

        public virtual void SetSections(IImmutableList<TSection> sections)
        {
            Sections = sections ?? ImmutableList<TSection>.Empty;
        }

        public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
        {
            tableView.DeselectRow(indexPath, true);
            OnItemTapped?.Invoke(this, ModelAt(indexPath));
        }

        public override void Scrolled(UIScrollView scrollView)
        {
            OnScrolled?.Invoke(this, scrollView.ContentOffset);
        }

        public override void DraggingStarted(UIScrollView scrollView)
        {
            OnDragStarted?.Invoke(this, new EventArgs());
        }

        public override nint NumberOfSections(UITableView tableView)
            => Sections.Count;

        public override nint RowsInSection(UITableView tableview, nint section)
            => Sections[(int)section].Items.Count;

        protected THeader HeaderOf(nint section)
            => Sections[(int)section].Header;

        protected TModel ModelAt(NSIndexPath path)
            => Sections[path.Section].Items[path.Row];

        protected TModel ModelAtOrDefault(NSIndexPath path)
        {
            var section = Sections.ElementAtOrDefault(path.Section);
            if (section == null)
                return default;

            return section.Items.ElementAtOrDefault(path.Row);
        }
    }
}
