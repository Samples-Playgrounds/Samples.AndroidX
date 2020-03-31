using FluentAssertions;
using Microsoft.Reactive.Testing;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using Toggl.Core.UI.Collections;
using Toggl.Core.UI.Collections.Changes;
using Xunit;

namespace Toggl.Core.Tests.UI.Collections
{
    public sealed class MockItem : IEquatable<MockItem>
    {
        public int Id { get; set; }
        public string Description { get; set; }

        public override string ToString()
        {
            return $"{Id}: {Description}";
        }

        public bool Equals(MockItem other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Id == other.Id && string.Equals(Description, other.Description);
        }
    }

    public class ObservableGroupedOrderedListTests
    {
        public sealed class TheUpdateMethod : ReactiveTest
        {
            [Fact, LogIfTooSlow]
            public void CanCreateNewSectionWhenMovingItem()
            {
                List<MockItem> list = new List<MockItem>
                {
                    new MockItem { Id = 0, Description = "A" },
                    new MockItem { Id = 1, Description = "B" },
                    new MockItem { Id = 3, Description = "D" },
                    new MockItem { Id = 8, Description = "DFE" }
                };
                var collection = new ObservableGroupedOrderedCollection<MockItem>(i => i.Id, i => i.Description, i => i.Description.Length);
                collection.ReplaceWith(list);

                var scheduler = new TestScheduler();
                var observer = scheduler.CreateObserver<ICollectionChange>();

                collection.CollectionChange.Subscribe(observer);

                var updated = new MockItem { Id = 1, Description = "ED" };
                collection.UpdateItem(updated.Id, updated);

                List<List<MockItem>> expected = new List<List<MockItem>>
                {
                    new List<MockItem>
                    {
                        new MockItem { Id = 0, Description = "A" },
                        new MockItem { Id = 3, Description = "D" },
                    },
                    new List<MockItem>
                    {
                        new MockItem { Id = 1, Description = "ED" }
                    },
                    new List<MockItem>
                    {
                        new MockItem { Id = 8, Description = "DFE" }
                    }
                };
                CollectionAssert.AreEqual(collection, expected);
            }

            [Fact, LogIfTooSlow]
            public void CanRemoveASectionWhenMovingItem()
            {
                List<MockItem> list = new List<MockItem>
                {
                    new MockItem { Id = 0, Description = "A" },
                    new MockItem { Id = 1, Description = "B" },
                    new MockItem { Id = 3, Description = "D" },
                    new MockItem { Id = 8, Description = "DFE" }
                };
                var collection = new ObservableGroupedOrderedCollection<MockItem>(i => i.Id, i => i.Description, i => i.Description.Length);
                collection.ReplaceWith(list);

                var scheduler = new TestScheduler();
                var observer = scheduler.CreateObserver<ICollectionChange>();

                collection.CollectionChange.Subscribe(observer);

                var updated = new MockItem { Id = 8, Description = "C" };
                collection.UpdateItem(updated.Id, updated);

                List<List<MockItem>> expected = new List<List<MockItem>>
                {
                    new List<MockItem>
                    {
                        new MockItem { Id = 0, Description = "A" },
                        new MockItem { Id = 1, Description = "B" },
                        new MockItem { Id = 8, Description = "C" },
                        new MockItem { Id = 3, Description = "D" }
                    }
                };
                CollectionAssert.AreEqual(collection, expected);
            }
        }

        public sealed class TheCollectionChangesProperty : ReactiveTest
        {
            [Fact, LogIfTooSlow]
            public void SendsEventWhenItemRemoved()
            {
                List<int> list = new List<int> { 40, 70, 8, 3, 1, 2 };
                var collection = new ObservableGroupedOrderedCollection<int>(i => i, i => i, i => i.ToString().Length);
                collection.ReplaceWith(list);

                var scheduler = new TestScheduler();
                var observer = scheduler.CreateObserver<ICollectionChange>();

                collection.CollectionChange.Subscribe(observer);

                collection.RemoveItemAt(0, 2);

                ICollectionChange change = new RemoveRowCollectionChange(new SectionedIndex(0, 2));

                observer.Messages.AssertEqual(
                    OnNext(0, change)
                );
            }

            [Fact, LogIfTooSlow]
            public void SendsEventWhenItemAdded()
            {
                List<int> list = new List<int> { 40, 70, 8, 3, 1, 2 };
                var collection = new ObservableGroupedOrderedCollection<int>(i => i, i => i, i => i.ToString().Length);
                collection.ReplaceWith(list);

                var scheduler = new TestScheduler();
                var observer = scheduler.CreateObserver<ICollectionChange>();

                collection.CollectionChange.Subscribe(observer);

                collection.InsertItem(20);

                ICollectionChange change = new AddRowCollectionChange<int>(new SectionedIndex(1, 0), 20);

                observer.Messages.AssertEqual(
                    OnNext(0, change)
                );
            }

            [Fact, LogIfTooSlow]
            public void SendsEventWhenReplaced()
            {
                List<int> list = new List<int> { 40, 70, 8, 3, 1, 2 };
                var collection = new ObservableGroupedOrderedCollection<int>(i => i, i => i, i => i.ToString().Length);
                collection.ReplaceWith(list);

                var scheduler = new TestScheduler();
                var observer = scheduler.CreateObserver<ICollectionChange>();

                collection.CollectionChange.Subscribe(observer);

                int[] newItems = { 0, 10, 100, 1000 };
                collection.ReplaceWith(newItems);

                ICollectionChange change = new ReloadCollectionChange();

                observer.Messages.AssertEqual(
                    OnNext(0, change)
                );
            }

            [Fact, LogIfTooSlow]
            public void SendsEventWhenUpdated()
            {
                List<MockItem> list = new List<MockItem>
                {
                    new MockItem { Id = 0, Description = "A" },
                    new MockItem { Id = 1, Description = "B" },
                    new MockItem { Id = 3, Description = "D" }
                };
                var collection = new ObservableGroupedOrderedCollection<MockItem>(i => i.Id, i => i.Description, i => i.Description.Length);
                collection.ReplaceWith(list);

                var scheduler = new TestScheduler();
                var observer = scheduler.CreateObserver<ICollectionChange>();

                collection.CollectionChange.Subscribe(observer);

                var updated = new MockItem { Id = 1, Description = "C" };
                collection.UpdateItem(updated.Id, updated);

                ICollectionChange change = new UpdateRowCollectionChange<MockItem>(new SectionedIndex(0, 1), updated);

                observer.Messages.AssertEqual(
                    OnNext(0, change)
                );
            }

            [Fact, LogIfTooSlow]
            public void SendsCreationEventIfUpdateCantFindItem()
            {
                List<MockItem> list = new List<MockItem>
                {
                    new MockItem { Id = 0, Description = "A" },
                    new MockItem { Id = 1, Description = "B" },
                    new MockItem { Id = 2, Description = "C" },
                    new MockItem { Id = 3, Description = "D" }
                };
                var collection = new ObservableGroupedOrderedCollection<MockItem>(i => i.Id, i => i.Description, i => i.Description.Length);
                collection.ReplaceWith(list);

                var scheduler = new TestScheduler();
                var observer = scheduler.CreateObserver<ICollectionChange>();

                collection.CollectionChange.Subscribe(observer);

                var updated = new MockItem { Id = 5, Description = "E" };
                collection.UpdateItem(updated.Id, updated);

                ICollectionChange change = new AddRowCollectionChange<MockItem>(new SectionedIndex(0, 4), updated);

                observer.Messages.AssertEqual(
                    OnNext(0, change)
                );
            }
        }

        public sealed class TheUpdateItemMethod : ReactiveTest
        {
            [Fact, LogIfTooSlow]
            public void ReturnsTheNewIndex()
            {
                List<MockItem> list = new List<MockItem>
                {
                    new MockItem { Id = 0, Description = "A" },
                    new MockItem { Id = 1, Description = "B" },
                    new MockItem { Id = 2, Description = "C" },
                    new MockItem { Id = 3, Description = "D" }
                };
                var collection = new ObservableGroupedOrderedCollection<MockItem>(i => i.Id, i => i.Description, i => i.Description.Length);
                collection.ReplaceWith(list);

                var scheduler = new TestScheduler();
                var observer = scheduler.CreateObserver<ICollectionChange>();

                collection.CollectionChange.Subscribe(observer);

                var updated = new MockItem { Id = 4, Description = "B2" };
                var index = collection.UpdateItem(1, updated);

                index.HasValue.Should().BeTrue();
                index.Value.Section.Should().Be(1);
                index.Value.Row.Should().Be(0);
            }
        }

        public sealed class TheEmptyObservableProperty : ReactiveTest
        {
            [Fact, LogIfTooSlow]
            public void UpdatesAccordingly()
            {
                List<int> list = new List<int>();
                var collection = new ObservableGroupedOrderedCollection<int>(i => i, i => i, i => i.ToString().Length);
                collection.ReplaceWith(list);

                var scheduler = new TestScheduler();
                var observer = scheduler.CreateObserver<bool>();

                collection.Empty.Subscribe(observer);

                collection.InsertItem(20);
                collection.InsertItem(2);
                collection.RemoveItemAt(0, 0);
                collection.RemoveItemAt(0, 0);

                observer.Messages.AssertEqual(
                    OnNext(0, true),
                    OnNext(0, false),
                    OnNext(0, true)
                );
            }
        }

        public sealed class TheTotalCountObservableProperty : ReactiveTest
        {
            [Fact, LogIfTooSlow]
            public void UpdatesAccordingly()
            {
                List<int> list = new List<int>();
                var collection = new ObservableGroupedOrderedCollection<int>(i => i, i => i, i => i.ToString().Length);
                collection.ReplaceWith(list);

                var scheduler = new TestScheduler();
                var observer = scheduler.CreateObserver<int>();

                collection.TotalCount.Subscribe(observer);

                collection.InsertItem(20);
                collection.InsertItem(2);
                collection.RemoveItemAt(0, 0);
                collection.RemoveItemAt(0, 0);

                observer.Messages.AssertEqual(
                    OnNext(0, 0),
                    OnNext(0, 1),
                    OnNext(0, 2),
                    OnNext(0, 1),
                    OnNext(0, 0)
                );
            }
        }
    }
}
