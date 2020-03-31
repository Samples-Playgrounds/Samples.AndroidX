using FluentAssertions;
using System;
using System.Collections.Generic;
using Toggl.Core.Models.Interfaces;
using Toggl.Core.Tests.Mocks;
using Toggl.Core.UI.ViewModels.TimeEntriesLog;
using Xunit;

namespace Toggl.Core.Tests.UI.ViewModels
{
    public sealed class GroupIdTests
    {
        public sealed class TheEqualsMethod
        {
            [Theory]
            [MemberData(nameof(MatchingPairs))]
            public void CorrectlyIdentifiesMatchingIds(IThreadSafeTimeEntry timeEntryA, IThreadSafeTimeEntry timeEntryB)
            {
                var groupA = new GroupId(timeEntryA);
                var groupB = new GroupId(timeEntryB);

                groupA.Equals(groupB).Should().BeTrue();
            }

            [Theory]
            [MemberData(nameof(NonMatchingPairs))]
            public void CorrectlyIdentifiesNonMatchingIds(IThreadSafeTimeEntry timeEntryA, IThreadSafeTimeEntry timeEntryB)
            {
                var groupA = new GroupId(timeEntryA);
                var groupB = new GroupId(timeEntryB);

                groupA.Equals(groupB).Should().BeFalse();
            }

            public static IEnumerable<object[]> MatchingPairs()
                => new[]
                {
                    new object[]
                    {
                        new MockTimeEntry { Description = "abc" },
                        new MockTimeEntry { Description = "abc" }
                    },
                    new object[]
                    {
                        new MockTimeEntry { Start = new DateTimeOffset(2019, 03, 18, 12, 34, 56, TimeSpan.FromHours(7)) },
                        new MockTimeEntry { Start = new DateTimeOffset(2019, 03, 18, 12, 34, 56, TimeSpan.FromHours(7)) }
                    },
                    new object[]
                    {
                        new MockTimeEntry { WorkspaceId = 1 },
                        new MockTimeEntry { WorkspaceId = 1 }
                    },
                    new object[]
                    {
                        new MockTimeEntry { Project = new MockProject { Id = 1 } },
                        new MockTimeEntry { Project = new MockProject { Id = 1 } }
                    },
                    new object[]
                    {
                        new MockTimeEntry { Task = new MockTask { Id = 1 } },
                        new MockTimeEntry { Task = new MockTask { Id = 1 } }
                    },
                    new object[]
                    {
                        new MockTimeEntry { Billable = true },
                        new MockTimeEntry { Billable = true }
                    },
                    new object[]
                    {
                        new MockTimeEntry { TagIds = new long[] { 1, 2, 3 } },
                        new MockTimeEntry { TagIds = new long[] { 1, 3, 2 } }
                    }
                };

            public static IEnumerable<object[]> NonMatchingPairs()
                => new[]
                {
                    new object[]
                    {
                        new MockTimeEntry { Description = "abc" },
                        new MockTimeEntry { Description = "abcd" }
                    },
                    new object[]
                    {
                        new MockTimeEntry { Start = new DateTimeOffset(2019, 03, 18, 12, 34, 56, TimeSpan.FromHours(7)) },
                        new MockTimeEntry { Start = new DateTimeOffset(2019, 03, 19, 12, 34, 56, TimeSpan.FromHours(7)) }
                    },
                    new object[]
                    {
                        new MockTimeEntry { WorkspaceId = 1 },
                        new MockTimeEntry { WorkspaceId = 2 }
                    },
                    new object[]
                    {
                        new MockTimeEntry { Project = new MockProject { Id = 1 } },
                        new MockTimeEntry { Project = new MockProject { Id = 2 } }
                    },
                    new object[]
                    {
                        new MockTimeEntry { Task = new MockTask { Id = 1 } },
                        new MockTimeEntry { Task = new MockTask { Id = 2 } }
                    },
                    new object[]
                    {
                        new MockTimeEntry { Billable = true },
                        new MockTimeEntry { Billable = false }
                    },
                    new object[]
                    {
                        new MockTimeEntry { TagIds = new long[] { 1, 2, 3 } },
                        new MockTimeEntry { TagIds = new long[] { 1, 3, 2, 4 } }
                    }
                };
        }
    }
}
