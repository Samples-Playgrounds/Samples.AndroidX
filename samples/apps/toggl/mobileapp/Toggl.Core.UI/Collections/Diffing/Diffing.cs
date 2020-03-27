using System;
using System.Collections.Generic;
using System.Linq;

namespace Toggl.Core.UI.Collections.Diffing
{
    public sealed class Diffing<TSection, THeader, TElement, TKey>
        where TKey : IEquatable<TKey>
        where TSection : IAnimatableSectionModel<THeader, TElement, TKey>, new()
        where TElement : IDiffable<TKey>, IEquatable<TElement>
        where THeader : IDiffable<TKey>
    {
        public class Changeset
        {
            public List<TSection> OriginalSections { get; }
            public List<TSection> FinalSections { get; }

            public List<int> InsertedSections { get; }
            public List<int> DeletedSections { get; }
            public List<(int, int)> MovedSections { get; }
            public List<int> UpdatedSections { get; }
            public List<ItemPath> InsertedItems { get; }
            public List<ItemPath> DeletedItems { get; }
            public List<(ItemPath, ItemPath)> MovedItems { get; }
            public List<ItemPath> UpdatedItems { get; }

            public Changeset(
                List<TSection> originalSections = null,
                List<TSection> finalSections = null,
                List<int> insertedSections = null,
                List<int> deletedSections = null,
                List<(int, int)> movedSections = null,
                List<int> updatedSections = null,
                List<ItemPath> insertedItems = null,
                List<ItemPath> deletedItems = null,
                List<(ItemPath, ItemPath)> movedItems = null,
                List<ItemPath> updatedItems = null)
            {
                OriginalSections = originalSections ?? new List<TSection>();
                FinalSections = finalSections ?? new List<TSection>();

                InsertedSections = insertedSections ?? new List<int>();
                DeletedSections = deletedSections ?? new List<int>();
                MovedSections = movedSections ?? new List<(int, int)>();
                UpdatedSections = updatedSections ?? new List<int>();

                InsertedItems = insertedItems ?? new List<ItemPath>();
                DeletedItems = deletedItems ?? new List<ItemPath>();
                MovedItems = movedItems ?? new List<(ItemPath, ItemPath)>();
                UpdatedItems = updatedItems ?? new List<ItemPath>();
            }
        }

        private readonly List<TSection> initialSections;
        private readonly List<TSection> finalSections;

        private List<List<TElement>> initialItemCache;
        private List<List<TElement>> finalItemCache;

        private List<SectionAssociatedData> initialSectionData;
        private List<SectionAssociatedData> finalSectionData;

        private List<List<ItemAssociatedData>> initialItemData;
        private List<List<ItemAssociatedData>> finalItemData;

        public Diffing(IEnumerable<TSection> initialSections, IEnumerable<TSection> finalSections)
        {
            this.initialSections = initialSections.ToList();
            this.finalSections = finalSections.ToList();
        }

        public List<Changeset> ComputeDifferences()
        {
            (initialSectionData, finalSectionData) = calculateSectionMovements(initialSections.ToList(), finalSections.ToList());

            initialItemCache = initialSections.Select(collection => collection.Items.ToList()).ToList();
            finalItemCache = finalSections.Select(collection => collection.Items.ToList()).ToList();

            var result = Enumerable.Empty<Changeset>().ToList();

            (initialItemData, finalItemData) = calculateItemMovements(
                initialItemCache,
                finalItemCache,
                initialSectionData,
                finalSectionData
            );

            result.AddRange(generateDeleteSectionsDeletedItemsAndUpdatedItems());
            result.AddRange(generateInsertAndMoveSections());
            result.AddRange(generateInsertAndMovedItems());

            return result;
        }

        private static (List<SectionAssociatedData>, List<SectionAssociatedData>) calculateSectionMovements(
            List<TSection> initialSections, List<TSection> finalSections)
        {
            var initialSectionIndexes = indexSections(initialSections);

            var initialSectionData = Enumerable.Range(0, initialSections.Count)
                .Select(_ => SectionAssociatedData.Initial())
                .ToList();

            var finalSectionData = Enumerable.Range(0, finalSections.Count)
                .Select(_ => SectionAssociatedData.Initial())
                .ToList();

            for (var i = 0; i < finalSections.Count; i++)
            {
                var section = finalSections[i];

                finalSectionData[i].ItemCount = finalSections[i].Items.Count;

                if (!initialSectionIndexes.ContainsKey(section.Identity))
                {
                    continue;
                }
                var initialSectionIndex = initialSectionIndexes[section.Identity];

                if (initialSectionData[initialSectionIndex].MoveIndex.HasValue)
                {
                    throw new DuplicateSectionException<TKey>(section.Identity);
                }

                initialSectionData[initialSectionIndex].MoveIndex = i;
                finalSectionData[i].MoveIndex = initialSectionIndex;
            }

            var sectionIndexAfterDelete = 0;

            // deleted sections

            for (var i = 0; i < initialSectionData.Count; i++)
            {
                initialSectionData[i].ItemCount = initialSections[i].Items.Count;
                if (initialSectionData[i].MoveIndex == null)
                {
                    initialSectionData[i].EditEvent = EditEvent.Deleted;
                    continue;
                }

                initialSectionData[i].IndexAfterDelete = sectionIndexAfterDelete;
                sectionIndexAfterDelete += 1;
            }

            // moved sections

            int? untouchedOldIndex = 0;
            int? findNextUntouchedOldIndex(int? initialSearchIndex)
            {
                if (!initialSearchIndex.HasValue)
                {
                    return null;
                }

                var i = initialSearchIndex.Value;
                while (i < initialSections.Count)
                {
                    if (initialSectionData[i].EditEvent == EditEvent.Untouched)
                    {
                        return i;
                    }

                    i++;
                }

                return null;
            }

            // inserted and moved sections
            // this should fix all sections and move them into correct places
            // 2nd stage
            for (var i = 0; i < finalSections.Count; i++)
            {
                untouchedOldIndex = findNextUntouchedOldIndex(untouchedOldIndex);

                // oh, it did exist
                var oldSectionIndex = finalSectionData[i].MoveIndex;
                if (oldSectionIndex.HasValue)
                {
                    var moveType = oldSectionIndex != untouchedOldIndex
                        ? EditEvent.Moved
                        : EditEvent.MovedAutomatically;

                    finalSectionData[i].EditEvent = moveType;
                    initialSectionData[oldSectionIndex.Value].EditEvent = moveType;
                }
                else
                {
                    finalSectionData[i].EditEvent = EditEvent.Inserted;
                }
            }

            // inserted sections
            foreach (var section in finalSectionData)
            {
                if (!section.MoveIndex.HasValue)
                {
                    section.EditEvent = EditEvent.Inserted;
                }
            }

            return (initialSectionData, finalSectionData);
        }

        private static (List<List<ItemAssociatedData>>, List<List<ItemAssociatedData>>)
            calculateItemMovements(
                IReadOnlyList<List<TElement>> initialItemCache,
                IReadOnlyList<List<TElement>> finalItemCache,
                IReadOnlyList<SectionAssociatedData> initialSectionData,
                IReadOnlyList<SectionAssociatedData> finalSectionData)
        {
            var (initialItemData, finalItemData) = calculateAssociatedData(
                initialItemCache.Select(items => items.ToList()).ToList(),
                finalItemCache.Select(items => items.ToList()).ToList()
            );

            int? findNextUntouchedOldIndex(int initialSectionIndex, int? initialSearchIndex)
            {
                if (!initialSearchIndex.HasValue)
                {
                    return null;
                }

                var i2 = initialSearchIndex.Value;
                while (i2 < initialSectionData[initialSectionIndex].ItemCount)
                {
                    if (initialItemData[initialSectionIndex][i2].EditEvent == EditEvent.Untouched)
                    {
                        return i2;
                    }

                    i2++;
                }

                return null;
            }

            // first mark deleted items
            for (int i = 0; i < initialItemCache.Count; i++)
            {
                if (!initialSectionData[i].MoveIndex.HasValue)
                {
                    continue;
                }

                var indexAfterDelete = 0;
                for (int j = 0; j < initialItemCache[i].Count; j++)
                {
                    if (initialItemData[i][j].MoveIndex == null)
                    {
                        initialItemData[i][j].EditEvent = EditEvent.Deleted;
                        continue;
                    }

                    var finalIndexPath = initialItemData[i][j].MoveIndex;
                    // from this point below, section has to be move type because it's initial and not deleted

                    // because there is no move to inserted section
                    if (finalSectionData[finalIndexPath.sectionIndex].EditEvent == EditEvent.Inserted)
                    {
                        initialItemData[i][j].EditEvent = EditEvent.Deleted;
                        continue;
                    }

                    initialItemData[i][j].IndexAfterDelete = indexAfterDelete;
                    indexAfterDelete += 1;
                }
            }

            // mark moved or moved automatically
            for (int i = 0; i < finalItemCache.Count; i++)
            {
                if (!finalSectionData[i].MoveIndex.HasValue)
                {
                    continue;
                }

                var originalSectionIndex = finalSectionData[i].MoveIndex.Value;

                int? untouchedIndex = 0;
                for (int j = 0; j < finalItemCache[i].Count; j++)
                {
                    untouchedIndex = findNextUntouchedOldIndex(originalSectionIndex, untouchedIndex);

                    if (finalItemData[i][j].MoveIndex == null)
                    {
                        finalItemData[i][j].EditEvent = EditEvent.Inserted;
                        continue;
                    }

                    var originalIndex = finalItemData[i][j].MoveIndex;

                    // In case trying to move from deleted section, abort, otherwise it will crash table view
                    if (initialSectionData[originalIndex.sectionIndex].EditEvent == EditEvent.Deleted)
                    {
                        finalItemData[i][j].EditEvent = EditEvent.Inserted;
                        continue;
                    }

                    // original section can't be inserted
                    if (initialSectionData[originalIndex.sectionIndex].EditEvent == EditEvent.Inserted)
                    {
                        throw new Exception("New section in initial sections, that is wrong");
                    }

                    var initialSectionEvent = initialSectionData[originalIndex.sectionIndex].EditEvent;
                    if (initialSectionEvent != EditEvent.Moved && initialSectionEvent != EditEvent.MovedAutomatically)
                    {
                        throw new Exception("Section not moved");
                    }

                    var eventType = (originalIndex.sectionIndex == originalSectionIndex && originalIndex.itemIndex == (untouchedIndex ?? -1))
                        ? EditEvent.MovedAutomatically
                        : EditEvent.Moved;

                    initialItemData[originalIndex.sectionIndex][originalIndex.itemIndex].EditEvent = eventType;
                    finalItemData[i][j].EditEvent = eventType;
                }
            }

            return (initialItemData, finalItemData);
        }

        private List<Changeset> generateDeleteSectionsDeletedItemsAndUpdatedItems()
        {
            var deletedSections = new List<int>();
            var updatedSections = new List<int>();
            var deletedItems = new List<ItemPath>();
            var updatedItems = new List<ItemPath>();
            var afterDeleteState = new List<TSection>();

            // mark deleted items
            // 1rst stage again (I know, I know ...)
            for (var i = 0; i < initialItemCache.Count; i++)
            {
                var initialItems = initialItemCache[i];
                var editEvent = initialSectionData[i].EditEvent;

                // Deleted section will take care of deleting child items.
                // In case of moving an item from deleted section, tableview will
                // crash anyway, so this is not limiting anything.
                if (editEvent == EditEvent.Deleted)
                {
                    deletedSections.Add(i);
                    continue;
                }

                var afterDeleteItems = new List<TElement>();
                for (int j = 0; j < initialItems.Count; j++)
                {
                    editEvent = initialItemData[i][j].EditEvent;
                    switch (editEvent)
                    {
                        case EditEvent.Deleted:
                            deletedItems.Add(new ItemPath(i, j));
                            break;

                        case EditEvent.Moved:
                        case EditEvent.MovedAutomatically:
                            var finalItemIndex = initialItemData[i][j].MoveIndex;
                            var finalItem = finalItemCache[finalItemIndex.sectionIndex][finalItemIndex.itemIndex];
                            if (!finalItem.Equals(initialSections[i].Items[j]))
                            {
                                updatedItems.Add(new ItemPath(sectionIndex: i, itemIndex: j));
                            }

                            afterDeleteItems.Add(finalItem);
                            break;

                        default:
                            throw new Exception("Unhandled case");
                    }
                }

                var sectionData = initialSectionData[i];
                TSection section;
                switch (sectionData.EditEvent)
                {
                    case EditEvent.Moved:
                    case EditEvent.MovedAutomatically:
                        section = finalSections[sectionData.MoveIndex ?? i];
                        break;
                    default:
                        section = finalSections[i];
                        break;
                }


                var newSection = new TSection();
                newSection.Initialize(section.Header, afterDeleteItems);
                afterDeleteState.Add(newSection);
            }

            if (deletedItems.Count == 0 && deletedSections.Count == 0 && updatedItems.Count == 0)
            {
                return new List<Changeset>();
            }

            updatedSections = deletedItems
                .Concat(updatedItems)
                .Select(item => item.sectionIndex)
                .Distinct()
                .ToList();

            var changeSet = new Changeset(
                finalSections: afterDeleteState,
                deletedSections: deletedSections,
                updatedSections: updatedSections,
                deletedItems: deletedItems,
                updatedItems: updatedItems
            );

            return new List<Changeset>(new[] { changeSet });
        }

        private IEnumerable<Changeset> generateInsertAndMoveSections()
        {
            var movedSections = new List<(int, int)>();
            var insertedSections = new List<int>();

            for (int i = 0; i < initialSections.Count; i++)
            {
                switch (initialSectionData[i].EditEvent)
                {
                    case EditEvent.Deleted:
                        break;

                    case EditEvent.Moved:
                        movedSections.Add((initialSectionData[i].IndexAfterDelete.Value,
                            initialSectionData[i].MoveIndex.Value));
                        break;

                    case EditEvent.MovedAutomatically:
                        break;

                    default:
                        throw new Exception("Unhandled case in initial sections");
                }
            }

            for (int i = 0; i < finalSections.Count; i++)
            {
                if (finalSectionData[i].EditEvent == EditEvent.Inserted)
                {
                    insertedSections.Add(i);
                }
            }

            if (insertedSections.Count == 0 && movedSections.Count == 0)
            {
                return new List<Changeset>();
            }

            // sections should be in place, but items should be original without deleted ones
            var sectionsAfterChange = Enumerable.Range(0, finalSections.Count).Select(i =>
            {
                var section = finalSections[i];
                var editEvent = finalSectionData[i].EditEvent;

                if (editEvent == EditEvent.Inserted)
                {
                    // it's already set up
                    return section;
                }

                if (editEvent == EditEvent.Moved || editEvent == EditEvent.MovedAutomatically)
                {
                    var originalSectionIndex = finalSectionData[i].MoveIndex.Value;
                    var originalSection = initialSections[originalSectionIndex];

                    var items = new List<TElement>();
                    //items.reserveCapacity(originalSection.items.count)
                    var itemAssociatedData = initialItemData[originalSectionIndex];
                    for (int j = 0; j < originalSection.Items.Count; j++)
                    {
                        var initialData = itemAssociatedData[j];

                        if (initialData.EditEvent == EditEvent.Deleted)
                        {
                            continue;
                        }

                        if (initialData.MoveIndex == null)
                        {
                            throw new Exception("Item was moved, but no final location.");
                        }

                        var finalIndex = initialData.MoveIndex;

                        items.Add(finalItemCache[finalIndex.sectionIndex][finalIndex.itemIndex]);
                    }

                    var newSection = new TSection();
                    newSection.Initialize(section.Header, items);
                    var modifiedSection = newSection;

                    return modifiedSection;
                }

                throw new Exception("This is weird, this shouldn't happen");
            });

            var changeSet = new Changeset(
                finalSections: sectionsAfterChange.ToList(),
                insertedSections: insertedSections,
                movedSections: movedSections);

            return new List<Changeset>(new[] { changeSet });
        }

        private IEnumerable<Changeset> generateInsertAndMovedItems()
        {
            var updatedSections = new List<int>();
            var insertedItems = new List<ItemPath>();
            var movedItems = new List<(ItemPath, ItemPath)>();

            // mark new and moved items
            // 3rd stage
            for (int i = 0; i < finalSections.Count; i++)
            {
                var finalSection = finalSections[i];

                var sectionEvent = finalSectionData[i].EditEvent;
                // new and deleted sections cause reload automatically
                if (sectionEvent != EditEvent.Moved && sectionEvent != EditEvent.MovedAutomatically)
                {
                    continue;
                }

                for (int j = 0; j < finalSection.Items.Count; j++)
                {
                    var currentItemEvent = finalItemData[i][j].EditEvent;

                    if (currentItemEvent == EditEvent.Untouched)
                    {
                        throw new Exception("Current event is not untouched");
                    }

                    var editEvent = finalItemData[i][j].EditEvent;

                    switch (editEvent)
                    {
                        case EditEvent.Inserted:
                            insertedItems.Add(new ItemPath(i, j));
                            break;

                        case EditEvent.Moved:
                            var originalIndex = finalItemData[i][j].MoveIndex;
                            var finalSectionIndex = initialSectionData[originalIndex.sectionIndex].MoveIndex.Value;
                            var moveFromItemWithIndex = initialItemData[originalIndex.sectionIndex][originalIndex.itemIndex].IndexAfterDelete.Value;

                            var moveCommand = (
                                new ItemPath(finalSectionIndex, moveFromItemWithIndex),
                                new ItemPath(i, j)
                            );

                            movedItems.Add(moveCommand);
                            break;
                    }
                }
            }

            if (insertedItems.Count == 0 && movedItems.Count == 0)
            {
                return new List<Changeset>();
            }

            updatedSections = insertedItems
                .Select(item => item.sectionIndex)
                .Concat(movedItems.SelectMany(movedItem => new[] { movedItem.Item1.sectionIndex, movedItem.Item2.sectionIndex }))
                .Distinct()
                .ToList();

            var changeset = new Changeset(
                finalSections: finalSections,
                updatedSections: updatedSections,
                insertedItems: insertedItems,
                movedItems: movedItems
            );

            return new List<Changeset>(new[] { changeset });
        }

        private static Dictionary<TKey, int> indexSections(List<TSection> sections)
        {
            Dictionary<TKey, int> indexedSections = new Dictionary<TKey, int>();

            for (int i = 0; i < sections.Count; i++)
            {
                var section = sections[i];

                if (indexedSections.ContainsKey(section.Identity))
                {
                    throw new DuplicateSectionException<TKey>(section.Identity);
                }

                indexedSections[section.Identity] = i;
            }

            return indexedSections;
        }

        private static (List<List<ItemAssociatedData>>, List<List<ItemAssociatedData>>) calculateAssociatedData(
            IReadOnlyList<List<TElement>> initialItemCache,
            IReadOnlyList<List<TElement>> finalItemCache)
        {
            var initialIdentities = new List<TKey>();
            var initialItemPaths = new List<ItemPath>();

            for (int i = 0; i < initialItemCache.Count; i++)
            {
                var items = initialItemCache[i];
                for (int j = 0; j < items.Count; j++)
                {
                    var item = items[j];

                    initialIdentities.Add(item.Identity);
                    initialItemPaths.Add(new ItemPath(i, j));
                }
            }

            var initialItemData = initialItemCache
                .Select(items => Enumerable.Range(0, items.Count).Select(_ => ItemAssociatedData.Initial()).ToList())
                .ToList();

            var finalItemData = finalItemCache
                .Select(items => Enumerable.Range(0, items.Count).Select(_ => ItemAssociatedData.Initial()).ToList())
                .ToList();

            var dictionary = new Dictionary<TKey, int>();

            for (int i = 0; i < initialIdentities.Count; i++)
            {
                var identity = initialIdentities[i];

                if (dictionary.ContainsKey(identity))
                {
                    var existingValueItemPathIndex = dictionary[identity];
                    var itemPath = initialItemPaths[existingValueItemPathIndex];
                    var item = initialItemCache[itemPath.sectionIndex][itemPath.itemIndex];
                    throw new DuplicateItemException<TKey>(item.Identity);
                }

                dictionary[identity] = i;
            }

            for (int i = 0; i < finalItemCache.Count; i++)
            {
                var items = finalItemCache[i];

                for (int j = 0; j < items.Count; j++)
                {
                    var item = items[j];

                    var identity = item.Identity;
                    if (!dictionary.ContainsKey(identity))
                    {
                        continue;
                    }

                    var initialItemPathIndex = dictionary[identity];
                    var itemPath = initialItemPaths[initialItemPathIndex];
                    if (initialItemData[itemPath.sectionIndex][itemPath.itemIndex].MoveIndex != null)
                    {
                        throw new DuplicateItemException<TKey>(item.Identity);
                    }

                    initialItemData[itemPath.sectionIndex][itemPath.itemIndex].MoveIndex = new ItemPath(i, j);
                    finalItemData[i][j].MoveIndex = itemPath;
                }
            }

            return (initialItemData, finalItemData);
        }
    }
}
