using Foundation;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reactive;
using Toggl.Core.UI.Collections;
using Toggl.Core.UI.Collections.Diffing;
using Toggl.Core.UI.Reactive;
using Toggl.iOS.Cells;
using Toggl.iOS.ViewSources;
using UIKit;

namespace Toggl.iOS.Extensions.Reactive
{
    public static class UITableViewExtensions
    {
        public static IObserver<IImmutableList<TSection>> ReloadSections<TSection, THeader, TModel>(
            this IReactive<UITableView> reactive, BaseTableViewSource<TSection, THeader, TModel> dataSource)
        where TSection : ISectionModel<THeader, TModel>, new()
        {
            return Observer.Create<IImmutableList<TSection>>(list =>
            {
                dataSource.SetSections(list);
                reactive.Base.ReloadData();
                dataSource.OnItemsChanged?.Invoke(dataSource, new EventArgs());
            });
        }

        public static IObserver<IImmutableList<TSection>> AnimateSections<TSection, THeader, TModel, TKey>(
            this IReactive<UITableView> reactive,
            BaseTableViewSource<TSection, THeader, TModel> dataSource)
            where TKey : IEquatable<TKey>
            where TSection : IAnimatableSectionModel<THeader, TModel, TKey>, new()
            where TModel : IDiffable<TKey>, IEquatable<TModel>
            where THeader : IDiffable<TKey>
        {
            return Observer.Create<IImmutableList<TSection>>(finalSections =>
            {
                var initialSections = dataSource.Sections;
                if (initialSections == null || initialSections.Count == 0)
                {
                    dataSource.SetSections(finalSections);
                    reactive.Base.ReloadData();
                    dataSource.OnItemsChanged?.Invoke(dataSource, new EventArgs());
                    return;
                }

                // if view is not in view hierarchy, performing batch updates will crash the app
                if (reactive.Base.Window == null)
                {
                    dataSource.SetSections(finalSections);
                    reactive.Base.ReloadData();
                    dataSource.OnItemsChanged?.Invoke(dataSource, new EventArgs());
                    return;
                }

                var diff = new Diffing<TSection, THeader, TModel, TKey>(initialSections, finalSections);
                var changeset = diff.ComputeDifferences();

                // The changesets have to be applied one after another. Not in one transaction.
                // iOS is picky about the changes which can happen in a single transaction.
                // Don't put BeginUpdates() ... EndUpdates() around the foreach, it has to stay this way,
                // otherwise the app might crash from time to time.

                foreach (var difference in changeset)
                {
                    reactive.Base.BeginUpdates();
                    dataSource.SetSections(difference.FinalSections.ToImmutableList());
                    reactive.Base.performChangesetUpdates(difference);
                    reactive.Base.EndUpdates();

                    foreach (var section in difference.UpdatedSections)
                    {
                        if (reactive.Base.GetHeaderView(section) is BaseTableHeaderFooterView<THeader> headerView)
                        {
                            headerView.Item = difference.FinalSections[section].Header;
                        }
                    }

                    dataSource.OnItemsChanged?.Invoke(dataSource, new EventArgs());
                }
            });
        }

        public static IObserver<IImmutableList<TModel>> ReloadItems<TSection, THeader, TModel>(
            this IReactive<UITableView> reactive, BaseTableViewSource<TSection, THeader, TModel> dataSource)
        where TSection : SectionModel<THeader, TModel>, new()
        {
            return Observer.Create<IImmutableList<TModel>>(list =>
            {
                dataSource.SetItems(list);
                reactive.Base.ReloadData();
                dataSource.OnItemsChanged?.Invoke(dataSource, new EventArgs());
            });
        }

        private static void performChangesetUpdates<TSection, THeader, TModel, TKey>(
            this UITableView tableView,
            Diffing<TSection, THeader, TModel, TKey>.Changeset changes)
            where TKey : IEquatable<TKey>
            where TSection : IAnimatableSectionModel<THeader, TModel, TKey>, new()
            where TModel : IDiffable<TKey>, IEquatable<TModel>
            where THeader : IDiffable<TKey>

        {
            NSIndexSet newIndexSet(List<int> indexes)
            {
                var indexSet = new NSMutableIndexSet();
                foreach (var i in indexes)
                {
                    indexSet.Add((nuint)i);
                }

                return indexSet as NSIndexSet;
            }

            tableView.DeleteSections(newIndexSet(changes.DeletedSections), UITableViewRowAnimation.Fade);
            // Updated sections doesn't mean reload entire section, somebody needs to update the section view manually
            // otherwise all cells will be reloaded for nothing.
            tableView.InsertSections(newIndexSet(changes.InsertedSections), UITableViewRowAnimation.Fade);

            foreach (var (from, to) in changes.MovedSections)
            {
                tableView.MoveSection(from, to);
            }
            tableView.DeleteRows(
                changes.DeletedItems.Select(item => NSIndexPath.FromRowSection(item.itemIndex, item.sectionIndex)).ToArray(),
                UITableViewRowAnimation.Top
            );

            tableView.InsertRows(
                changes.InsertedItems.Select(item =>
                    NSIndexPath.FromItemSection(item.itemIndex, item.sectionIndex)).ToArray(),
                UITableViewRowAnimation.Automatic
            );
            tableView.ReloadRows(
                changes.UpdatedItems.Select(item => NSIndexPath.FromRowSection(item.itemIndex, item.sectionIndex))
                    .ToArray(),
                // No animation so it doesn't fade showing the cells behind it
                UITableViewRowAnimation.None
            );

            foreach (var (from, to) in changes.MovedItems)
            {
                tableView.MoveRow(
                    NSIndexPath.FromRowSection(from.itemIndex, from.sectionIndex),
                    NSIndexPath.FromRowSection(to.itemIndex, to.sectionIndex)
                );
            }
        }
    }
}
