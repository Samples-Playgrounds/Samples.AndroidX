using System;
using FluentAssertions;
using NSubstitute;
using Toggl.Core.Models;
using Toggl.Storage.Models;
using Xunit;

namespace Toggl.Core.Tests
{
    public class TimeEntryTests
    {
        public class TheFromFactoryMethod
        {
            [Fact]
            public void SetsDefaultValueForDescriptionIfDatabaseStoresNullValue()
            {
                var databaseEntry = Substitute.For<IDatabaseTimeEntry>();
                databaseEntry.Description.Returns(null as string);

                var timeEntry = TimeEntry.From(databaseEntry);

                timeEntry.Description.Should().NotBeNull();
                timeEntry.Description.Should().BeEmpty();
            }
        }

        public class TheCleanFactoryMethod
        {
            [Fact]
            public void SetsDefaultValueForDescriptionIfDatabaseStoresNullValue()
            {
                var databaseEntry = Substitute.For<IDatabaseTimeEntry>();
                databaseEntry.Description.Returns(null as string);

                var timeEntry = TimeEntry.Clean(databaseEntry);

                timeEntry.Description.Should().NotBeNull();
                timeEntry.Description.Should().BeEmpty();
            }
        }

        public class TheDirtyFactoryMethod
        {
            [Fact]
            public void SetsDefaultValueForDescriptionIfDatabaseStoresNullValue()
            {
                var databaseEntry = Substitute.For<IDatabaseTimeEntry>();
                databaseEntry.Description.Returns(null as string);

                var timeEntry = TimeEntry.Dirty(databaseEntry);

                timeEntry.Description.Should().NotBeNull();
                timeEntry.Description.Should().BeEmpty();
            }
        }

        public class TheUnsyncableFactoryMethod
        {
            [Fact]
            public void SetsDefaultValueForDescriptionIfDatabaseStoresNullValue()
            {
                var databaseEntry = Substitute.For<IDatabaseTimeEntry>();
                databaseEntry.Description.Returns(null as string);

                var timeEntry = TimeEntry.Unsyncable(databaseEntry, "some error");

                timeEntry.Description.Should().NotBeNull();
                timeEntry.Description.Should().BeEmpty();
            }
        }

        public class TheCleanDeletedFactoryMethod
        {
            [Fact]
            public void SetsDefaultValueForDescriptionIfDatabaseStoresNullValue()
            {
                var databaseEntry = Substitute.For<IDatabaseTimeEntry>();
                databaseEntry.Description.Returns(null as string);

                var timeEntry = TimeEntry.CleanDeleted(databaseEntry);

                timeEntry.Description.Should().NotBeNull();
                timeEntry.Description.Should().BeEmpty();
            }
        }

        public class TheDirtyDeletedFactoryMethod
        {
            [Fact]
            public void SetsDefaultValueForDescriptionIfDatabaseStoresNullValue()
            {
                var databaseEntry = Substitute.For<IDatabaseTimeEntry>();
                databaseEntry.Description.Returns(null as string);

                var timeEntry = TimeEntry.DirtyDeleted(databaseEntry);

                timeEntry.Description.Should().NotBeNull();
                timeEntry.Description.Should().BeEmpty();
            }
        }

        public class TheUnsyncableDeletedFactoryMethod
        {
            [Fact]
            public void SetsDefaultValueForDescriptionIfDatabaseStoresNullValue()
            {
                var databaseEntry = Substitute.For<IDatabaseTimeEntry>();
                databaseEntry.Description.Returns(null as string);

                var timeEntry = TimeEntry.UnsyncableDeleted(databaseEntry, "some error");

                timeEntry.Description.Should().NotBeNull();
                timeEntry.Description.Should().BeEmpty();
            }
        }

        public class TheWithDurationFactoryMethod
        {
            [Fact]
            public void SetsDefaultValueForDescriptionIfDatabaseStoresNullValue()
            {
                var databaseEntry = Substitute.For<IDatabaseTimeEntry>();
                databaseEntry.Description.Returns(null as string);

                var timeEntry = databaseEntry.With(duration: 123);

                timeEntry.Description.Should().NotBeNull();
                timeEntry.Description.Should().BeEmpty();
            }
        }

        public class TheUpdatedAtFactoryMethod
        {
            [Fact]
            public void SetsDefaultValueForDescriptionIfDatabaseStoresNullValue()
            {
                var databaseEntry = Substitute.For<IDatabaseTimeEntry>();
                databaseEntry.Description.Returns(null as string);

                var timeEntry = databaseEntry.UpdatedAt(DateTimeOffset.Now);

                timeEntry.Description.Should().NotBeNull();
                timeEntry.Description.Should().BeEmpty();
            }
        }

        public class TheBuilderFactory
        {
            [Fact]
            public void SetsDefaultValueForDescriptionIfDatabaseStoresNullValue()
            {
                var builder = TimeEntry.Builder.Create(1)
                    .SetDescription(null)
                    .SetStart(DateTimeOffset.Now)
                    .SetWorkspaceId(2)
                    .SetUserId(3)
                    .SetAt(DateTimeOffset.Now)
                    .SetDuration(123);

                Action building = () => builder.Build();

                building.Should().Throw<InvalidOperationException>();
            }
        }
    }
}
