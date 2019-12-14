using FluentAssertions;
using System.Collections.Immutable;
using System.Linq;
using Toggl.Core.Autocomplete;
using Toggl.Core.Autocomplete.Span;
using Xunit;

namespace Toggl.Core.Tests.Autocomplete
{
    public sealed class TextFieldInfoTests
    {
        public abstract class TextFieldInfoTest
        {
            protected const long WorkspaceId = 9;

            protected const string Description = "Testing Toggl mobile apps";

            protected const long ProjectId = 10;
            protected const string ProjectName = "Toggl";
            protected const string ProjectColor = "#F41F19";

            protected const long TaskId = 13;
            protected const string TaskName = "Test Toggl apps";

            protected TextFieldInfo CreateDefaultTextFieldInfo()
            {
                var spans = new ISpan[]
                {
                    new QueryTextSpan(Description, Description.Length),
                    new ProjectSpan(ProjectId, ProjectName, ProjectColor, TaskId, TaskName)
                };

                return TextFieldInfo.Empty(WorkspaceId).ReplaceSpans(spans.ToImmutableList());
            }
        }

        public sealed class TheWithProjectInfoMethod : TextFieldInfoTest
        {
            private const long newWorkspaceId = 100;

            private const long newProjectId = 200;
            private const string newProjectName = "Some other project";
            private const string newProjectColor = "Some other project";

            private const long newTaskId = 300;
            private const string newTaskName = "New task";

            [Fact, LogIfTooSlow]
            public void ReplacesTheExistingProjectSpanIfItExists()
            {
                var oldTextFieldInfo = CreateDefaultTextFieldInfo();
                var textFieldInfo = oldTextFieldInfo
                    .WithProject(WorkspaceId, newProjectId, newProjectName, newProjectColor, null, null);

                var projectSpan = textFieldInfo.Spans.OfType<ProjectSpan>().Single();
                projectSpan.ProjectId.Should().Be(newProjectId);
                projectSpan.ProjectName.Should().Be(newProjectName);
                projectSpan.ProjectColor.Should().Be(newProjectColor);
            }

            [Fact, LogIfTooSlow]
            public void RemovesAllTagsIfTheWorkspaceChanges()
            {
                var oldTextFieldInfo = CreateDefaultTextFieldInfo()
                    .AddTag(1, "1").AddTag(2, "2").AddTag(3, "3");

                var textFieldInfo = oldTextFieldInfo
                    .WithProject(newWorkspaceId, newProjectId, newProjectName, newProjectColor, newTaskId, newTaskName);

                textFieldInfo.Spans.Should().NotContain(span => span is TagSpan);
            }

            [Fact, LogIfTooSlow]
            public void SetsTheTaskInfo()
            {
                var textFieldInfo = CreateDefaultTextFieldInfo()
                    .WithProject(WorkspaceId, newProjectId, newProjectName, newProjectColor, newTaskId, newTaskName);

                var projectSpan = textFieldInfo.Spans.OfType<ProjectSpan>().Single();
                projectSpan.TaskId.Should().Be(newTaskId);
                projectSpan.TaskName.Should().Be(newTaskName);
            }

            [Fact, LogIfTooSlow]
            public void DoesNotAllowDuplicatesEvenIfNoQuerySpanExists()
            {
                var textFieldInfo = TextFieldInfo
                    .Empty(WorkspaceId)
                    .WithProject(WorkspaceId, ProjectId, ProjectName, ProjectColor, TaskId, TaskName);

                var newTextFieldinfo = textFieldInfo
                    .WithProject(WorkspaceId, newProjectId, newProjectName, newProjectColor, newTaskId, newTaskName);

                var projectSpan = newTextFieldinfo.Spans.OfType<ProjectSpan>().Single();
                projectSpan.ProjectId.Should().Be(newProjectId);
            }
        }

        public sealed class TheRemoveProjectQueryIfNeededMethod : TextFieldInfoTest
        {

            [Fact, LogIfTooSlow]
            public void RemovesTheProjectQueryIfAnyAtSymbolIsPresent()
            {
                var newDescription = $"{Description}@something";

                var textFieldInfo = TextFieldInfo.Empty(WorkspaceId).ReplaceSpans(
                    new QueryTextSpan(newDescription, newDescription.Length),
                    new ProjectSpan(ProjectId, ProjectName, ProjectColor)
                ).RemoveProjectQueryIfNeeded();

                textFieldInfo.GetQuerySpan().Text.Should().Be(Description);
            }

            [Fact, LogIfTooSlow]
            public void RemovesTheProjectQueryFromTheFirstAtSymbolIsPresent()
            {
                var newDescription = $"{Description}@something";
                var longDescription = $"{newDescription}@else";

                var textFieldInfo = TextFieldInfo.Empty(WorkspaceId).ReplaceSpans(
                    new QueryTextSpan(longDescription, longDescription.Length),
                    new ProjectSpan(ProjectId, ProjectName, ProjectColor)
                ).RemoveProjectQueryIfNeeded();

                textFieldInfo.GetQuerySpan().Text.Should().Be(Description);
            }

            [Fact, LogIfTooSlow]
            public void DoesNotChangeAnyPropertyIfThereIsNoProjectQueryInTheDescription()
            {
                var textFieldInfo = CreateDefaultTextFieldInfo();

                var newTextFieldInfo =
                    textFieldInfo.RemoveProjectQueryIfNeeded();

                textFieldInfo.Description.Should().Be(newTextFieldInfo.Description);
            }
        }

        public sealed class RemoveTagQueryIfNeeded : TextFieldInfoTest
        {
            [Fact, LogIfTooSlow]
            public void RemovesTheTagQueryIfAnyHashtagSymbolIsPresent()
            {
                var newDescription = $"{Description}#something";

                var textFieldInfo = TextFieldInfo.Empty(WorkspaceId)
                    .ReplaceSpans(new QueryTextSpan(newDescription, newDescription.Length))
                    .RemoveTagQueryIfNeeded();

                textFieldInfo.GetQuerySpan().Text.Should().Be(Description);
            }

            [Fact, LogIfTooSlow]
            public void RemovesTheTagQueryFromTheFirstAtSymbolPresent()
            {
                var newDescription = $"{Description}#something";
                var longDescription = $"{newDescription}#else";

                var textFieldInfo = TextFieldInfo.Empty(WorkspaceId)
                    .ReplaceSpans(new QueryTextSpan(longDescription, longDescription.Length))
                    .RemoveTagQueryIfNeeded();

                textFieldInfo.GetQuerySpan().Text.Should().Be(Description);
            }

            [Fact, LogIfTooSlow]
            public void DoesNotChangeAnyPropertyIfThereIsNoTagQueryInTheDescription()
            {
                var textFieldInfo = CreateDefaultTextFieldInfo();

                var newTextFieldInfo =
                    textFieldInfo.RemoveTagQueryIfNeeded();

                textFieldInfo.Description.Should().Be(newTextFieldInfo.Description);
            }
        }

        public sealed class TheAddTagsMethod : TextFieldInfoTest
        {
            [Fact, LogIfTooSlow]
            public void AddsTagsCorrectly()
            {
                var textFieldInfo =
                    TextFieldInfo.Empty(WorkspaceId)
                        .AddTag(1, "1")
                        .AddTag(2, "2");

                var tags = textFieldInfo.Spans.OfType<TagSpan>().ToList();

                tags.Count.Should().Be(2);
                tags[0].TagId.Should().Be(1);
                tags[1].TagId.Should().Be(2);
            }

            [Fact, LogIfTooSlow]
            public void DoesNotAddTagIfAlreadyAdded()
            {
                var textFieldInfo =
                    TextFieldInfo.Empty(WorkspaceId)
                        .AddTag(1, "1")
                        .AddTag(1, "1");

                textFieldInfo.Spans.OfType<TagSpan>().Should().HaveCount(1);
            }
        }
    }

    public static class TestExtensions
    {
        public static QueryTextSpan GetQuerySpan(this TextFieldInfo textFieldInfo)
            => textFieldInfo.Spans.OfType<QueryTextSpan>().Single();

        public static TextFieldInfo ReplaceSpans(this TextFieldInfo textFieldInfo, params ISpan[] spans)
            => textFieldInfo.ReplaceSpans(spans.ToImmutableList());
    }
}
