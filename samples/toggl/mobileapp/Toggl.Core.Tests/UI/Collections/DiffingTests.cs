using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Toggl.Core.Tests.UI.Collections.Extensions;
using Toggl.Core.UI.Collections;
using Toggl.Core.UI.Collections.Diffing;
using Xunit;

namespace Toggl.Core.Tests.UI.Collections
{
    public class DiffableInt : IDiffable<long>
    {
        public int Value { get; set; }
        public long Identity { get; }

        public DiffableInt(int value)
        {
            this.Value = value;
            Identity = value;
        }
    }

    public class TestItem : IDiffable<long>, IEquatable<TestItem>
    {
        public int Id { get; set; }
        public string Value { get; set; }
        public long Identity { get; }

        public TestItem(int id, string value)
        {
            Id = id;
            Value = value;
            Identity = id;
        }

        public bool Equals(TestItem other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return string.Equals(Value, other.Value);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((TestItem)obj);
        }

        public override int GetHashCode()
        {
            return (Value != null ? Value.GetHashCode() : 0);
        }
    }

    public class TestSectionModel : AnimatableSectionModel<DiffableInt, TestItem, long>
    {
        public TestSectionModel()
        {
        }

        public TestSectionModel(DiffableInt header, IEnumerable<TestItem> items) : base(header, items)
        {
        }

        public static TestSectionModel Create(int header, (int, string)[] items)
        {
            return new TestSectionModel(
                new DiffableInt(header),
                items.Select(tuple => new TestItem(tuple.Item1, tuple.Item2))
            );
        }
    }

    public sealed class ItemTests
    {
        [Fact, LogIfTooSlow]
        public void TestItemInsert()
        {
            var initial = new List<TestSectionModel>()
            {
                TestSectionModel.Create(1, new[]
                {
                    (0, ""),
                    (1, ""),
                    (2, "")
                })
            };

            var final = new List<TestSectionModel>()
            {
                TestSectionModel.Create(1, new[]
                {
                    (0, ""),
                    (1, ""),
                    (2, ""),
                    (3, "")
                })
            };

            var diff = new Diffing<TestSectionModel, DiffableInt, TestItem, long>(initial, final);
            var differences = diff.ComputeDifferences();

            differences.Count.Should().Be(1);
            differences.First().OnlyContains(insertedItems: 1, updatedSections: 1).Should().BeTrue();
            initial.Apply(differences).Should().BeEquivalentTo(final);
        }

        [Fact, LogIfTooSlow]
        public void TestItemDelete()
        {
            var initial = new List<TestSectionModel>()
            {
                TestSectionModel.Create(1, new[]
                {
                    (0, ""),
                    (1, ""),
                    (2, "")
                })
            };


            var final = new List<TestSectionModel>()
            {
                TestSectionModel.Create(1, new[]
                {
                    (0, ""),
                    (2, "")
                })
            };

            var diff = new Diffing<TestSectionModel, DiffableInt, TestItem, long>(initial, final);
            var differences = diff.ComputeDifferences();

            differences.Count.Should().Be(1);
            differences.First().OnlyContains(deletedItems: 1, updatedSections: 1).Should().BeTrue();
            initial.Apply(differences).Should().BeEquivalentTo(final);
        }

        [Fact, LogIfTooSlow]
        public void TestItemMove1()
        {
            var initial = new List<TestSectionModel>()
            {
                TestSectionModel.Create(1, new[]
                {
                    (0, ""),
                    (1, ""),
                    (2, "")
                })
            };


            var final = new List<TestSectionModel>()
            {
                TestSectionModel.Create(1, new[]
                {
                    (1, ""),
                    (2, ""),
                    (0, "")
                })
            };

            var diff = new Diffing<TestSectionModel, DiffableInt, TestItem, long>(initial, final);
            var differences = diff.ComputeDifferences();

            differences.Count.Should().Be(1);
            differences.First().OnlyContains(movedItems: 2, updatedSections: 1).Should().BeTrue();
            initial.Apply(differences).Should().BeEquivalentTo(final);
        }

        [Fact, LogIfTooSlow]
        public void TestItemMove2()
        {
            var initial = new List<TestSectionModel>()
            {
                TestSectionModel.Create(1, new[]
                {
                    (0, ""),
                    (1, ""),
                    (2, "")
                })
            };


            var final = new List<TestSectionModel>()
            {
                TestSectionModel.Create(1, new[]
                {
                    (2, ""),
                    (0, ""),
                    (1, "")
                })
            };

            var diff = new Diffing<TestSectionModel, DiffableInt, TestItem, long>(initial, final);
            var differences = diff.ComputeDifferences();

            differences.Count.Should().Be(1);
            differences.First().OnlyContains(movedItems: 1, updatedSections: 1).Should().BeTrue();
            initial.Apply(differences).Should().BeEquivalentTo(final);
        }

        [Fact, LogIfTooSlow]
        public void TestItemUpdated()
        {
            var initial = new List<TestSectionModel>()
            {
                TestSectionModel.Create(1, new[]
                {
                    (0, ""),
                    (1, ""),
                    (2, "")
                })
            };


            var final = new List<TestSectionModel>()
            {
                TestSectionModel.Create(1, new[]
                {
                    (0, ""),
                    (1, "u"),
                    (2, "")
                })
            };

            var diff = new Diffing<TestSectionModel, DiffableInt, TestItem, long>(initial, final);
            var differences = diff.ComputeDifferences();

            differences.Count.Should().Be(1);
            differences.First().OnlyContains(updatedItems: 1, updatedSections: 1).Should().BeTrue();
            initial.Apply(differences).Should().BeEquivalentTo(final);
        }
    }

    public sealed class SectionTests
    {
        [Fact, LogIfTooSlow]
        public void TestInsertSection()
        {

            var initial = new List<TestSectionModel>()
            {
                TestSectionModel.Create(1, new[]
                {
                    (0, ""),
                    (1, ""),
                    (2, "")
                })
            };


            var final = new List<TestSectionModel>()
            {
                TestSectionModel.Create(1, new[]
                {
                    (0, ""),
                    (1, ""),
                    (2, "")
                }),
                TestSectionModel.Create(2, new[]
                {
                    (3, ""),
                    (4, ""),
                    (5, "")
                })
            };

            var diff = new Diffing<TestSectionModel, DiffableInt, TestItem, long>(initial, final);
            var differences = diff.ComputeDifferences();

            differences.Count.Should().Be(1);
            differences.First().OnlyContains(insertedSections: 1).Should().BeTrue();
            initial.Apply(differences).Should().BeEquivalentTo(final);
        }

        [Fact, LogIfTooSlow]
        public void TestDeleteSection()
        {

            var initial = new List<TestSectionModel>()
            {
                TestSectionModel.Create(1, new[]
                {
                    (0, ""),
                    (1, ""),
                    (2, "")
                })
            };


            var final = new List<TestSectionModel>();

            var diff = new Diffing<TestSectionModel, DiffableInt, TestItem, long>(initial, final);
            var differences = diff.ComputeDifferences();

            differences.Count.Should().Be(1);
            differences.First().OnlyContains(deletedSections: 1).Should().BeTrue();
            initial.Apply(differences).Should().BeEquivalentTo(final);
        }

        [Fact, LogIfTooSlow]
        public void TestMovedSection1()
        {
            var initial = new List<TestSectionModel>()
            {
                TestSectionModel.Create(1, new[]
                {
                    (0, ""),
                    (1, ""),
                    (2, "")
                }),
                TestSectionModel.Create(2, new[]
                {
                    (3, ""),
                    (4, ""),
                    (5, "")
                }),
                TestSectionModel.Create(3, new[]
                {
                    (6, ""),
                    (7, ""),
                    (8, "")
                })
            };

            var final = new List<TestSectionModel>()
            {
                TestSectionModel.Create(2, new[]
                {
                    (3, ""),
                    (4, ""),
                    (5, "")
                }),
                TestSectionModel.Create(3, new[]
                {
                    (6, ""),
                    (7, ""),
                    (8, "")
                }),
                TestSectionModel.Create(1, new[]
                {
                    (0, ""),
                    (1, ""),
                    (2, "")
                }),
            };

            var diff = new Diffing<TestSectionModel, DiffableInt, TestItem, long>(initial, final);
            var differences = diff.ComputeDifferences();

            differences.Count.Should().Be(1);
            differences.First().OnlyContains(movedSections: 2).Should().BeTrue();
            initial.Apply(differences).Should().BeEquivalentTo(final);
        }

        [Fact, LogIfTooSlow]
        public void TestMovedSection2()
        {
            var initial = new List<TestSectionModel>()
            {
                TestSectionModel.Create(1, new[]
                {
                    (0, ""),
                    (1, ""),
                    (2, "")
                }),
                TestSectionModel.Create(2, new[]
                {
                    (3, ""),
                    (4, ""),
                    (5, "")
                }),
                TestSectionModel.Create(3, new[]
                {
                    (6, ""),
                    (7, ""),
                    (8, "")
                })
            };

            var final = new List<TestSectionModel>()
            {
                TestSectionModel.Create(3, new[]
                {
                    (6, ""),
                    (7, ""),
                    (8, "")
                }),
                TestSectionModel.Create(1, new[]
                {
                    (0, ""),
                    (1, ""),
                    (2, "")
                }),
                TestSectionModel.Create(2, new[]
                {
                    (3, ""),
                    (4, ""),
                    (5, "")
                })
            };

            var diff = new Diffing<TestSectionModel, DiffableInt, TestItem, long>(initial, final);
            var differences = diff.ComputeDifferences();

            differences.Count.Should().Be(1);
            differences.First().OnlyContains(movedSections: 1).Should().BeTrue();
            initial.Apply(differences).Should().BeEquivalentTo(final);
        }
    }

    public sealed class ExceptionTests
    {
        [Fact, LogIfTooSlow]
        public void TestThrowsErrorOnDuplicateItem()
        {
            var initial = new List<TestSectionModel>()
            {
                TestSectionModel.Create(1, new[]
                {
                    (1111, "")
                }),
                TestSectionModel.Create(2, new[]
                {
                    (1111, "")
                })
            };

            var diff = new Diffing<TestSectionModel, DiffableInt, TestItem, long>(initial, initial);
            Action tryingToComputeDifferences =
                () => diff.ComputeDifferences();

            tryingToComputeDifferences.Should()
                .Throw<DuplicateItemException<long>>()
                .Where(exception => exception.DuplicatedIdentity == 1111);
        }

        [Fact, LogIfTooSlow]
        public void TestThrowsErrorOnDuplicateSection()
        {
            var initial = new List<TestSectionModel>()
            {
                TestSectionModel.Create(1, new[]
                {
                    (1111, "")
                }),
                TestSectionModel.Create(1, new[]
                {
                    (1112, "")
                })
            };

            var diff = new Diffing<TestSectionModel, DiffableInt, TestItem, long>(initial, initial);
            Action tryingToComputeDifferences =
                () => diff.ComputeDifferences();

            tryingToComputeDifferences.Should()
                .Throw<DuplicateSectionException<long>>()
                .Where(exception => exception.DuplicatedIdentity == 1);
        }
    }

    public sealed class EdgeCaseTests
    {
        [Fact, LogIfTooSlow]
        public void TestCase1()
        {
            var initial = new List<TestSectionModel>()
            {
                TestSectionModel.Create(1, new[]
                {
                    (1111, "")
                }),
                TestSectionModel.Create(2, new[]
                {
                    (2222, "")
                })
            };

            var final = new List<TestSectionModel>()
            {
                TestSectionModel.Create(2, new[]
                {
                    (0, "1")
                }),
                TestSectionModel.Create(1, new(int, string)[] { })
            };

            var diff = new Diffing<TestSectionModel, DiffableInt, TestItem, long>(initial, final);
            var differences = diff.ComputeDifferences();

            initial.Apply(differences).Should().BeEquivalentTo(final);
        }

        [Fact, LogIfTooSlow]
        public void TestCase2()
        {
            var initial = new List<TestSectionModel>()
            {
                TestSectionModel.Create(4, new[]
                {
                    (10, ""),
                    (11, ""),
                    (12, "")
                }),
                TestSectionModel.Create(9, new[]
                {
                    (25, ""),
                    (26, ""),
                    (27, "")
                })
            };

            var final = new List<TestSectionModel>()
            {
                TestSectionModel.Create(9, new[]
                {
                    (11, ""),
                    (26, ""),
                    (27, "u")
                }),
                TestSectionModel.Create(4, new[]
                {
                    (10, ""),
                    (12, "")
                })
            };

            var diff = new Diffing<TestSectionModel, DiffableInt, TestItem, long>(initial, final);
            var differences = diff.ComputeDifferences();

            initial.Apply(differences).Should().BeEquivalentTo(final);
        }

        [Fact, LogIfTooSlow]
        public void TestCase3()
        {
            var initial = new List<TestSectionModel>()
            {
                TestSectionModel.Create(4, new[]
                {
                    (5, "")
                }),
                TestSectionModel.Create(6, new[]
                {
                    (20, ""),
                    (14, "")
                }),
                TestSectionModel.Create(9, new(int, string)[] { }),
                TestSectionModel.Create(2, new[]
                {
                    (2, ""),
                    (26, "")
                }),
                TestSectionModel.Create(8, new[]
                {
                    (23, "")
                }),
                TestSectionModel.Create(10, new[]
                {
                    (8, ""),
                    (18, ""),
                    (13, "")
                }),
                TestSectionModel.Create(1, new[]
                {
                    (28, ""),
                    (25, ""),
                    (6, ""),
                    (11, ""),
                    (10, ""),
                    (29, ""),
                    (24, ""),
                    (27, ""),
                    (19, "")
                })
            };

            var final = new List<TestSectionModel>()
            {
                TestSectionModel.Create(4, new[]
                {
                    (5, "")
                }),
                TestSectionModel.Create(6, new[]
                {
                    (20, "u"),
                    (14, "")
                }),
                TestSectionModel.Create(9, new[]
                {
                    (16, "u"),
                }),
                TestSectionModel.Create(7, new[]
                {
                    (17, ""),
                    (15, ""),
                    (4, "u")
                }),
                TestSectionModel.Create(2, new[]
                {
                    (2, ""),
                    (26, "u"),
                    (23, "u")
                }),
                TestSectionModel.Create(8, new(int, string)[] { }),
                TestSectionModel.Create(10, new[]
                {
                    (8, "u"),
                    (18, "u"),
                    (13, "u")
                }),
                TestSectionModel.Create(1, new[]
                {
                    (28, "u"),
                    (25, "u"),
                    (6, "u"),
                    (11, "u"),
                    (10, "u"),
                    (29, "u"),
                    (24, "u"),
                    (7, "u"),
                    (19, "u")
                })
            };

            var diff = new Diffing<TestSectionModel, DiffableInt, TestItem, long>(initial, final);
            var differences = diff.ComputeDifferences();

            initial.Apply(differences).Should().BeEquivalentTo(final);
        }

        [Fact, LogIfTooSlow]
        public void TestPerformance()
        {

            List<TestSectionModel> initialValue()
            {
                var nSections = 500;
                var nItems = 500;

                return Enumerable.Range(0, nSections)
                    .Select(s =>
                    {
                        var items = Enumerable.Range(0, nItems)
                            .Select(i => new TestItem(s * nSections + i, $"{s}:{i}"));
                        var newSection = new TestSectionModel();
                        newSection.Initialize(new DiffableInt(s), items);
                        return newSection;
                    })
                    .ToList();
            }

            var initial = initialValue();
            var final = randomize(initial.Select(section =>
            {
                var newSection = new TestSectionModel();
                newSection.Initialize(section.Header, randomize(section.Items));
                return newSection;
            })).ToList();

            var stopwatch = new Stopwatch();
            stopwatch.Start();

            var diffing = new Diffing<TestSectionModel, DiffableInt, TestItem, long>(initial, final);
            var differences = diffing.ComputeDifferences();

            stopwatch.Stop();

            initial.Apply(differences).Should().BeEquivalentTo(final);
            stopwatch.ElapsedMilliseconds.Should().BeLessThan(3000);
        }

        private static IEnumerable<T> randomize<T>(IEnumerable<T> source)
        {
            Random rnd = new Random();
            return source.OrderBy((item) => rnd.Next());
        }
    }
}

