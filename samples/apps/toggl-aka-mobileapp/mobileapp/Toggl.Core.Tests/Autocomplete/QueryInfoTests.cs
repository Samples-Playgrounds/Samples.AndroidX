using FluentAssertions;
using FsCheck;
using FsCheck.Xunit;
using System;
using Toggl.Core.Autocomplete;
using Toggl.Core.Autocomplete.Span;
using Toggl.Core.Autocomplete.Suggestions;
using Xunit;

namespace Toggl.Core.Tests.Autocomplete
{
    public sealed class QueryInfoTests
    {
        public sealed class TheParseQueryMethod
        {
            public sealed class None
            {
                [Theory, LogIfTooSlow]
                [InlineData("")]
                public void DoesNotSuggestAnythingWhenTheTextIsEmpty(string text)
                {
                    var textFieldInfo = TextFieldInfo.Empty(1)
                        .ReplaceSpans(new QueryTextSpan(text, 0));

                    var parsed = QueryInfo.ParseFieldInfo(textFieldInfo);

                    parsed.SuggestionType.Should().Be(AutocompleteSuggestionType.None);
                    parsed.Text.Should().Be(string.Empty);
                }

                [Property]
                public void DoesNotSuggestAnythingWhenThereIsOnlyOneCharacter(char letter)
                {
                    if (letter == '#' || letter == '@')
                        return;

                    var textFieldInfo = TextFieldInfo.Empty(1)
                        .ReplaceSpans(new QueryTextSpan(letter.ToString(), 1));

                    var parsed = QueryInfo.ParseFieldInfo(textFieldInfo);

                    parsed.SuggestionType.Should().Be(AutocompleteSuggestionType.None);
                    parsed.Text.Should().Be(string.Empty);
                }
            }

            public sealed class TimeEntries
            {
                private const long projectId = 10;
                private const string projectName = "Toggl";
                private const string projectColor = "#F41F19";

                [Property]
                public void ExtractsTheProjectNameWhileTyping(NonEmptyString nonEmptyString)
                {
                    var text = nonEmptyString.Get;
                    if (text.Length < 2 || text.Contains("#") || text.Contains("@"))
                        return;

                    var textFieldInfo = TextFieldInfo.Empty(1)
                        .ReplaceSpans(new QueryTextSpan(text, text.Length));

                    var parsed = QueryInfo.ParseFieldInfo(textFieldInfo);

                    parsed.SuggestionType.Should().Be(AutocompleteSuggestionType.TimeEntries);
                    parsed.Text.Should().Be(text);
                }

                [Property]
                public void DoesNotSuggestAnyMoreProjectsWhenSomeProjectIsAlreadySelected(NonEmptyString nonEmptyString)
                {
                    var text = nonEmptyString.Get;
                    if (text.Length < 2 || text.Contains("#"))
                        return;

                    var textFieldInfo = TextFieldInfo.Empty(1).ReplaceSpans(
                        new QueryTextSpan(text, text.Length),
                        new ProjectSpan(projectId, projectName, projectColor)
                    );

                    var parsed = QueryInfo.ParseFieldInfo(textFieldInfo);

                    parsed.SuggestionType.Should().Be(AutocompleteSuggestionType.TimeEntries);
                    parsed.Text.Should().Be(text);
                }
            }

            public sealed class Projects
            {
                [Theory, LogIfTooSlow]
                [InlineData("@", "")]
                [InlineData("abcde @", "")]
                [InlineData("@abcde", "abcde")]
                [InlineData("@abcde fgh ijk", "abcde fgh ijk")]
                [InlineData("abcde @fgh ijk", "fgh ijk")]
                [InlineData("abcde @fgh #ijk", "fgh #ijk")]
                [InlineData("meeting with someone@gmail.com @meetings", "meetings")]
                public void ExtractsTheProjectNameWhileTyping(string text, string expectedProjectName)
                {
                    var textFieldInfo = TextFieldInfo.Empty(1)
                        .ReplaceSpans(new QueryTextSpan(text, text.Length));

                    var parsed = QueryInfo.ParseFieldInfo(textFieldInfo);

                    parsed.SuggestionType.Should().Be(AutocompleteSuggestionType.Projects);
                    parsed.Text.Should().Be(expectedProjectName);
                }

                [Theory, LogIfTooSlow]
                [InlineData("@", 0)]
                [InlineData("abcde @", 3)]
                [InlineData("@abcde", 0)]
                [InlineData("@abcde fgh ijk", 0)]
                [InlineData("abcde @fgh ijk", 5)]
                [InlineData("abcde #fgh @ijk", 8)]
                [InlineData("meeting with someone@gmail.com @meetings", 10)]
                public void DoesNotExtractTheProjectNameWhenCursorIsMovedBeforeTheAtSymbol(string text, int cursorPosition)
                {
                    var textFieldInfo = TextFieldInfo.Empty(1)
                        .ReplaceSpans(new QueryTextSpan(text, cursorPosition));

                    var parsed = QueryInfo.ParseFieldInfo(textFieldInfo);

                    parsed.SuggestionType.Should().NotBe(AutocompleteSuggestionType.Projects);
                }

                [Theory, LogIfTooSlow]
                [InlineData("@@@", 1, "@@")]
                [InlineData("abcde @fgh @ijk", 8, "fgh @ijk")]
                [InlineData("meeting with @meetings with someone@gmail.com", 20, "meetings with someone@gmail.com")]
                public void ExtractTheProjectNameFromTheFirstAtSymbolPrecedingTheCursor(string text, int cursorPosition, string expectedProjectName)
                {
                    var textFieldInfo = TextFieldInfo.Empty(1).ReplaceSpans(new QueryTextSpan(text, cursorPosition));

                    var parsed = QueryInfo.ParseFieldInfo(textFieldInfo);

                    parsed.SuggestionType.Should().Be(AutocompleteSuggestionType.Projects);
                    parsed.Text.Should().Be(expectedProjectName);
                }

                [Theory, LogIfTooSlow]
                [InlineData("@", "")]
                [InlineData("@@@", "@@")]
                [InlineData("@abc@def", "abc@def")]
                [InlineData("abcde @@fgh", "@fgh")]
                [InlineData("abcde @fgh @ijk", "fgh @ijk")]
                [InlineData("abcde @fgh#ijk", "fgh#ijk")]
                [InlineData("meeting with @meetings with someone@gmail.com", "meetings with someone@gmail.com")]
                public void ExtractTheProjectNameFromTheFirstAtSymbolWithPreceededWithAWhiteSpaceOrAtTheVeryBeginningOfTheText(string text, string expectedProjectName)
                {
                    var textFieldInfo = TextFieldInfo.Empty(1)
                        .ReplaceSpans(new QueryTextSpan(text, text.Length));

                    var parsed = QueryInfo.ParseFieldInfo(textFieldInfo);

                    parsed.SuggestionType.Should().Be(AutocompleteSuggestionType.Projects);
                    parsed.Text.Should().Be(expectedProjectName);
                }
            }

            public sealed class Tags
            {
                protected const long ProjectId = 10;
                protected const string ProjectName = "Toggl";
                protected const string ProjectColor = "#F41F19";

                [Theory, LogIfTooSlow]
                [InlineData("#", "")]
                [InlineData("abcde #", "")]
                [InlineData("#abcde", "abcde")]
                [InlineData("#abcde fgh ijk", "abcde fgh ijk")]
                [InlineData("abcde #fgh ijk", "fgh ijk")]
                [InlineData("abcde #fgh @ijk", "fgh @ijk")]
                [InlineData("meeting with someone@gmail.com #meetings", "meetings")]
                public void ExtractsTheTagNameWhileTyping(string text, string expectedTagName)
                {
                    var textFieldInfo = TextFieldInfo.Empty(1)
                        .ReplaceSpans(new QueryTextSpan(text, text.Length));

                    var parsed = QueryInfo.ParseFieldInfo(textFieldInfo);

                    parsed.SuggestionType.Should().Be(AutocompleteSuggestionType.Tags);
                    parsed.Text.Should().Be(expectedTagName);
                }

                [Theory, LogIfTooSlow]
                [InlineData("#@", "@")]
                [InlineData("#abcde@fgh.ijk", "abcde@fgh.ijk")]
                [InlineData("abcde #fgh @ijk", "fgh @ijk")]
                public void ExtractsTheTagNameIncludingTheAtSymbolsWhenAProjectIsSelected(string text, string expectedTagName)
                {
                    var textFieldInfo = TextFieldInfo.Empty(1).ReplaceSpans(
                        new QueryTextSpan(text, text.Length),
                        new ProjectSpan(ProjectId, ProjectName, ProjectColor)
                    );

                    var parsed = QueryInfo.ParseFieldInfo(textFieldInfo);

                    parsed.SuggestionType.Should().Be(AutocompleteSuggestionType.Tags);
                    parsed.Text.Should().Be(expectedTagName);
                }

                [Theory, LogIfTooSlow]
                [InlineData("#", "")]
                [InlineData("###", "##")]
                [InlineData("#abc#def", "abc#def")]
                [InlineData("abcde ##fgh", "#fgh")]
                [InlineData("abcde #fgh #ijk", "fgh #ijk")]
                [InlineData("meeting with #meetings with someone@gmail.com", "meetings with someone@gmail.com")]
                public void ExtractTheTagtNameFromTheFirstHashSymbolWithPreceededWithAWhiteSpaceOrAtTheVeryBeginningOfTheText(string text, string expectedTagName)
                {
                    var textFieldInfo = TextFieldInfo.Empty(1)
                        .ReplaceSpans(new QueryTextSpan(text, text.Length));

                    var parsed = QueryInfo.ParseFieldInfo(textFieldInfo);

                    parsed.SuggestionType.Should().Be(AutocompleteSuggestionType.Tags);
                    parsed.Text.Should().Be(expectedTagName);
                }
            }
        }
    }
}
