using FluentAssertions;
using Microsoft.Reactive.Testing;
using System;
using System.Linq;
using Toggl.Core.Autocomplete.Span;
using Toggl.Core.UI.Extensions;
using Xunit;

namespace Toggl.Core.Tests.Autocomplete
{
    public sealed class AutocompleteExtensionsTests
    {
        public sealed class TheCollapseTextSpansMethod
        {
            [Fact]
            public void PreservesEmptySequence()
            {
                var emptyList = Array.Empty<ISpan>();

                var collapsedList = emptyList.CollapseTextSpans();

                collapsedList.Should().BeEmpty();
            }

            [Fact]
            public void DoesNotCollapseTagSpans()
            {
                var list = new ISpan[] { new TagSpan(1, "tag1"), new TagSpan(2, "tag2") };

                var collapsedList = list.CollapseTextSpans();

                collapsedList.AssertEqual(list);
            }

            [Fact]
            public void DoesNotCollapseProjectSpans()
            {
                var list = new ISpan[] { new ProjectSpan(1, "project1", "red"), new ProjectSpan(2, "project2", "green") };

                var collapsedList = list.CollapseTextSpans();

                collapsedList.AssertEqual(list);
            }

            [Fact]
            public void CollapsesTwoTextSpansIntoOneTextSpan()
            {
                var list = new ISpan[] { new TextSpan("a"), new TextSpan("b") };

                var collapsedList = list.CollapseTextSpans().ToList();

                collapsedList.Should().HaveCount(1);
                collapsedList.First().IsTextSpan().Should().BeTrue();
                ((TextSpan)collapsedList.First()).Text.Should().Be("ab");
            }

            [Fact]
            public void CollapsesThreeTextSpansIntoOneTextSpan()
            {
                var list = new ISpan[] { new TextSpan("a"), new TextSpan("b"), new TextSpan("c") };

                var collapsedList = list.CollapseTextSpans().ToList();

                collapsedList.Should().HaveCount(1);
                collapsedList.First().IsTextSpan().Should().BeTrue();
                ((TextSpan)collapsedList.First()).Text.Should().Be("abc");
            }

            [Fact]
            public void CollapsesTextSpanAndQueryTextSpanIntoQueryTextSpan()
            {
                var list = new ISpan[] { new TextSpan("aa"), new QueryTextSpan("bb", 1) };

                var collapsedList = list.CollapseTextSpans().ToList();

                collapsedList.Should().HaveCount(1);
                collapsedList.First().IsTextSpan().Should().BeTrue();
                var queryTextSpan = (QueryTextSpan)collapsedList.First();
                queryTextSpan.Text.Should().Be("aabb");
                queryTextSpan.CursorPosition.Should().Be(3);
            }

            [Fact]
            public void CollapsesQueryTextSpanAndTextSpanIntoQueryTextSpan()
            {
                var list = new ISpan[] { new QueryTextSpan("aa", 1), new TextSpan("bb") };

                var collapsedList = list.CollapseTextSpans().ToList();

                collapsedList.Should().HaveCount(1);
                collapsedList.First().IsTextSpan().Should().BeTrue();
                var queryTextSpan = (QueryTextSpan)collapsedList.First();
                queryTextSpan.Text.Should().Be("aabb");
                queryTextSpan.CursorPosition.Should().Be(1);
            }

            [Fact]
            public void CollapsesQueryTextSpanAndMultipleTextSpanIntoQueryTextSpan()
            {
                var list = new ISpan[] { new TextSpan("aa"), new QueryTextSpan("bb", 1), new TextSpan("cc"), new TextSpan("dd") };

                var collapsedList = list.CollapseTextSpans().ToList();

                collapsedList.Should().HaveCount(1);
                collapsedList.First().IsTextSpan().Should().BeTrue();
                var queryTextSpan = (QueryTextSpan)collapsedList.First();
                queryTextSpan.Text.Should().Be("aabbccdd");
                queryTextSpan.CursorPosition.Should().Be(3);
            }

            [Fact]
            public void CollapsesNotSeparatedByTagOrProjectSpans()
            {
                var list = new ISpan[]
                {
                    new TextSpan("aa"),
                    new QueryTextSpan("bb", 1),
                    new ProjectSpan(1, "proj1", "red"),
                    new TextSpan("cc"),
                    new TagSpan(1, "tag1"),
                    new TextSpan("dd"),
                    new TextSpan("ee")
                };

                var collapsedList = list.CollapseTextSpans().ToList();

                collapsedList.Should().HaveCount(5);
                collapsedList[0].Should().BeOfType<QueryTextSpan>();
                ((QueryTextSpan)collapsedList[0]).Text.Should().Be("aabb");
                ((QueryTextSpan)collapsedList[0]).CursorPosition.Should().Be(3);
                collapsedList[1].Should().BeOfType<ProjectSpan>();
                collapsedList[2].Should().BeOfType<TextSpan>();
                ((TextSpan)collapsedList[2]).Text.Should().Be("cc");
                collapsedList[3].Should().BeOfType<TagSpan>();
                collapsedList[4].Should().BeOfType<TextSpan>();
                ((TextSpan)collapsedList[4]).Text.Should().Be("ddee");
            }
        }
    }
}
