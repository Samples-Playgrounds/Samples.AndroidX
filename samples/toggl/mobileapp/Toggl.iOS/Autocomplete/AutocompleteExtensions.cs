using Foundation;
using System;
using System.Collections.Generic;
using Toggl.Core.Autocomplete;
using Toggl.Core.Autocomplete.Span;
using Toggl.Core.UI.Helper;
using Toggl.iOS.Extensions;
using Toggl.Shared;
using Toggl.Shared.Extensions;
using UIKit;
using static Toggl.iOS.Autocomplete.Constants;

namespace Toggl.iOS.Autocomplete
{
    using ProjectInformationTuple = ValueTuple<long, string, string, long?, string>;
    using TagInformationTuple = ValueTuple<long, string>;

    public static class AutocompleteExtensions
    {
        private const int lineHeight = 24;
        private const int maxTextLength = 50;

        private static readonly NSParagraphStyle paragraphStyle;
        private static readonly UIFont tokenFont = UIFont.SystemFontOfSize(12, UIFontWeight.Regular);
        private static readonly UIFont regularFont = UIFont.SystemFontOfSize(16, UIFontWeight.Regular);

        static AutocompleteExtensions()
        {
            paragraphStyle = new NSMutableParagraphStyle
            {
                MinimumLineHeight = lineHeight,
                MaximumLineHeight = lineHeight
            };
        }

        public static ProjectInformationTuple GetProjectInformation(this NSDictionary dictionary)
        {
            long? taskId = null;
            string taskName = null;

            var projectId = (long)(dictionary[ProjectId] as NSNumber);
            var projectName = (dictionary[ProjectName] as NSString).ToString();
            var projectColor = (dictionary[ProjectColor] as NSString).ToString();

            if (dictionary.ContainsKey(TaskId))
            {
                taskId = (long)(dictionary[TaskId] as NSNumber);
                taskName = (dictionary[TaskName] as NSString)?.ToString();
            }

            return (projectId, projectName, projectColor, taskId, taskName);
        }

        public static TagInformationTuple GetTagInformation(this NSDictionary dictionary)
        {
            var tagId = (long)(dictionary[TagId] as NSNumber);
            var tagName = (dictionary[TagName] as NSString).ToString();

            return (tagId, tagName);
        }

        public static TokenTextAttachment GetTagToken(this string tag)
            => new TagTextAttachment(
                tag.TruncatedAt(maxTextLength),
                ColorAssets.Text2,
                tokenFont);

        public static IEnumerable<ISpan> AsSpans(this NSAttributedString text, int cursorPosition)
        {
            var start = 0;
            bool queryTextSpanAlreadyUsed = false;

            List<ISpan> spans = new List<ISpan>();

            while (start != text.Length)
            {
                var attributes = text.GetAttributes(start, out var longestEffectiveRange, new NSRange(start, text.Length - start));
                var length = (int)longestEffectiveRange.Length;
                var end = start + length;

                if (attributes.ContainsKey(ProjectId))
                {
                    var (projectId, projectName, projectColor, taskId, taskName) = attributes.GetProjectInformation();

                    spans.Add(new ProjectSpan(projectId, projectName, projectColor, taskId, taskName));
                }
                else if (attributes.ContainsKey(TagId))
                {
                    var (tagId, tagName) = attributes.GetTagInformation();
                    spans.Add(new TagSpan(tagId, tagName));
                }
                else if (length > 0)
                {
                    var subText = text.Substring(start, length).ToString().Substring(0, length);
                    if (queryTextSpanAlreadyUsed == false && cursorPosition.IsInRange(start, end))
                    {
                        queryTextSpanAlreadyUsed = true;
                        spans.Add(new QueryTextSpan(subText, cursorPosition - start));
                    }
                    else
                    {
                        spans.Add(new TextSpan(subText));
                    }
                }

                start = end;
            }

            for(var i = 1; i < spans.Count; i++)
            {
                var previousSpan = spans[i - 1];
                if (!previousSpan.IsTextSpan())
                    continue;

                var previousTextSpan = (TextSpan)previousSpan;
                var previousTextSpanContainsQuerySymbol =
                    previousTextSpan.Text.Contains(QuerySymbols.ProjectsString)
                    || previousTextSpan.Text.Contains(QuerySymbols.TagsString);

                if (previousTextSpanContainsQuerySymbol
                    && spans[i] is QueryTextSpan currentSpan)
                {
                    var newText = previousTextSpan.Text + currentSpan.Text;
                    spans[i] = new QueryTextSpan(newText, newText.Length);
                    spans.Remove(spans[i - 1]);
                }
            }

            return spans;
        }

        public static NSAttributedString AsAttributedText(this TextFieldInfo self)
        {
            var attributedText = new NSMutableAttributedString("", createBasicAttributes());

            foreach (var span in self.Spans)
            {
                var spanString = span.AsAttributedString();
                attributedText.Append(spanString);
            }

            return attributedText;
        }

        public static NSRange CursorPosition(this TextFieldInfo self)
        {
            nint location = 0;

            foreach (var span in self.Spans)
            {
                if (span is QueryTextSpan querySpan)
                {
                    location += querySpan.CursorPosition;
                    break;
                }

                var spanString = span.AsAttributedString();
                location += spanString.Length;
            }

            return new NSRange(location, 0);
        }

        private static NSMutableAttributedString AsAttributedString(this ISpan span)
        {
            switch (span)
            {
                case QueryTextSpan querySpan:
                    return querySpan.AsAttributedString();
                case TagSpan tagSpan:
                    return tagSpan.AsAttributedString();
                case ProjectSpan projectSpan:
                    return projectSpan.AsAttributedString();
                case TextSpan textSpan:
                    return textSpan.AsAttributedString();
            }

            throw new ArgumentOutOfRangeException(nameof(span));
        }

        private static NSMutableAttributedString AsAttributedString(this TextSpan textSpan)
            => new NSMutableAttributedString(textSpan.Text, createBasicAttributes());

        private static NSMutableAttributedString AsAttributedString(this ProjectSpan projectSpan)
        {
            var projectColor = new Color(projectSpan.ProjectColor).ToNativeColor();

            var projectNameString = projectSpan.ProjectName;
            if (!string.IsNullOrEmpty(projectSpan.TaskName))
                projectNameString = $"{projectNameString}: {projectSpan.TaskName}";

            var textAttachment = new ProjectTextAttachment(
                projectNameString, projectColor, tokenFont);

            var tokenString = new NSMutableAttributedString(NSAttributedString.FromAttachment(textAttachment));
            var attributes = createBasicAttributes();
            attributes.Dictionary[ProjectId] = new NSNumber(projectSpan.ProjectId);
            attributes.Dictionary[ProjectName] = new NSString(projectSpan.ProjectName);
            attributes.Dictionary[ProjectColor] = new NSString(projectSpan.ProjectColor);
            if (projectSpan.TaskId.HasValue)
            {
                attributes.Dictionary[TaskId] = new NSNumber(projectSpan.TaskId.Value);
                attributes.Dictionary[TaskName] = new NSString(projectSpan.TaskName);
            }

            tokenString.AddAttributes(attributes, new NSRange(0, tokenString.Length));

            return tokenString;
        }

        private static NSMutableAttributedString AsAttributedString(this TagSpan tagSpan)
        {
            var tagName = tagSpan.TagName.TruncatedAt(maxTextLength);
            var textAttachment = tagName.GetTagToken();
            var tokenString = new NSMutableAttributedString(NSAttributedString.FromAttachment(textAttachment));

            var attributes = createBasicAttributes();
            attributes.Dictionary[TagId] = new NSNumber(tagSpan.TagId);
            attributes.Dictionary[TagName] = new NSString(tagSpan.TagName);
            tokenString.AddAttributes(attributes, new NSRange(0, tokenString.Length));

            return tokenString;
        }

        private static UIStringAttributes createBasicAttributes()
            => new UIStringAttributes
            {
                ForegroundColor = ColorAssets.Text,
                Font = regularFont,
                ParagraphStyle = paragraphStyle
            };
    }
}
