using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toggl.Core.Search;
using Toggl.Shared.Extensions;
using Xunit;

namespace Toggl.Core.Tests.SearchEngine
{
    public sealed class SearchEngineTests
    {
        private class MockDataItem
        {
            public string Text { get; private set; }

            public MockDataItem(string text)
            {
                Text = text;
            }
        }

        private bool contains(string query, MockDataItem item) => item.Text.Contains(query);
        private bool pass(string query, MockDataItem item) => true;

        private ImmutableList<MockDataItem> getItems() => Enumerable
            .Range(10, 21)
            .Select(i => new MockDataItem(i.ToString()))
            .ToImmutableList();

        private ImmutableList<MockDataItem> getTextItems() =>
            new[] { "ice", "nice", "cerebrum", "drummer" }
            .Select(i => new MockDataItem(i.ToString()))
            .ToImmutableList();

        [Fact, LogIfTooSlow]
        public async Task SearchWithEmptyStringReturnsAllItems()
        {
            var list = getItems();
            var searchEngine = new SearchEngine<MockDataItem>(contains);
            searchEngine.SetInitialData(list);

            var data = await searchEngine.Get("");

            data.SetEquals(list).Should().BeTrue();
        }

        [Theory, LogIfTooSlow]
        [InlineData("0", 3)]
        [InlineData("1", 11)]
        [InlineData("2", 11)]
        [InlineData("3", 3)]
        [InlineData("5", 2)]
        public async Task SearchCorrectlyFiltersElements(string query, int expectedCount)
        {
            var list = getItems();
            var searchEngine = new SearchEngine<MockDataItem>(contains);
            searchEngine.SetInitialData(list);

            var data = await searchEngine.Get(query);

            data.Should().HaveCount(expectedCount);
        }

        [Fact, LogIfTooSlow]
        public async Task SearchFiltersAllElementsForNonTrivialChangesToQuery()
        {
            var counter = 0;
            Func<string, MockDataItem, bool> countingFilter = (query, item) =>
            {
                counter++;
                return contains(query, item);
            };
            var list = getTextItems();
            var searchEngine = new SearchEngine<MockDataItem>(countingFilter);
            searchEngine.SetInitialData(list);

            var data = await searchEngine.Get("ice");
            data = await searchEngine.Get("drum");

            counter.Should().Be(list.Count * 2);
        }

        [Fact, LogIfTooSlow]
        public async Task SearchUsesFilteredCachedItemForSubqueryChanges()
        {
            var counter = 0;
            Func<string, MockDataItem, bool> countingFilter = (query, item) =>
            {
                counter++;
                return contains(query, item);
            };
            var list = getTextItems();
            var searchEngine = new SearchEngine<MockDataItem>(countingFilter);
            searchEngine.SetInitialData(list);
            var iceItems = list.Where(item => item.Text.Contains("ice")).ToList();

            var data = await searchEngine.Get("ice");
            data = await searchEngine.Get("nice");

            counter.Should().Be(list.Count + iceItems.Count);
        }

        [Fact, LogIfTooSlow]
        public async Task SearchUtilizesCachedItemForRepeatedQueries()
        {
            var counter = 0;
            Func<string, MockDataItem, bool> countingFilter = (query, item) =>
            {
                counter++;
                return contains(query, item);
            };
            var list = getTextItems();
            var searchEngine = new SearchEngine<MockDataItem>(countingFilter);
            searchEngine.SetInitialData(list);

            var data = await searchEngine.Get("ice");
            data = await searchEngine.Get("ice");

            counter.Should().Be(list.Count);
        }

        [Fact, LogIfTooSlow]
        public async Task SearchUsesFilteredCachedItemForCachedSubqueryChanges()
        {
            var counter = 0;
            Func<string, MockDataItem, bool> countingFilter = (query, item) =>
            {
                counter++;
                return contains(query, item);
            };
            var list = getTextItems();
            var searchEngine = new SearchEngine<MockDataItem>(countingFilter);
            searchEngine.SetInitialData(list);
            var drumItems = list.Where(item => item.Text.Contains("rum")).ToList();

            var data = await searchEngine.Get("rum");
            data = await searchEngine.Get("ice");
            data = await searchEngine.Get("drum");

            counter.Should().Be(list.Count + list.Count + drumItems.Count);
        }
    }
}
