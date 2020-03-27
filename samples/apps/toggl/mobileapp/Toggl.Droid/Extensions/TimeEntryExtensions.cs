using Android.Content;
using Android.Text;
using Android.Text.Style;
using Toggl.Droid.Views;
using static Toggl.Core.Helper.Colors;
using Color = Android.Graphics.Color;

namespace Toggl.Droid.Extensions
{
    public static class TimeEntryExtensions
    {
        private static string placeholderSeparator = " ";
        private static string taskSeparator = ": ";

        public static ISpannable ToProjectTaskClient(Context context,
            bool hasProject,
            string project,
            string projectColor,
            string task,
            string client,
            bool projectIsPlaceholder,
            bool taskIsPlaceholder,
            bool displayPlaceholders = false)
        {
            if (!hasProject)
                return new SpannableString(string.Empty);

            var spannableString = new SpannableStringBuilder();

            appendProjectTextAndSpans(context, project, projectColor, projectIsPlaceholder, displayPlaceholders, spannableString);
            appendTaskTextAndSpans(context, task, projectIsPlaceholder, taskIsPlaceholder, displayPlaceholders, spannableString);

            if (!string.IsNullOrEmpty(client))
                spannableString.Append($" {client}", new ForegroundColorSpan(Color.ParseColor(ClientNameColor)), SpanTypes.ExclusiveExclusive);

            return spannableString;
        }

        private static void appendTaskTextAndSpans(Context context, string taskName, bool projectIsPlaceholder, bool taskIsPlaceholder, bool displayPlaceholders, SpannableStringBuilder spannableString)
        {
            if (string.IsNullOrEmpty(taskName)) return;
            if (displayPlaceholders && taskIsPlaceholder)
            {
                spannableString.Append(placeholderSeparator);
                spannableString.Append(taskName, new VerticallyCenteredImageSpan(context, Resource.Drawable.text_placeholder), SpanTypes.ExclusiveExclusive);
            }
            else
            {
                var projectTaskSeparator = projectIsPlaceholder ? placeholderSeparator : taskSeparator;
                spannableString.Append($"{projectTaskSeparator}{taskName}");
            }
        }

        private static void appendProjectTextAndSpans(Context context, string projectName, string projectColor, bool projectIsPlaceholder, bool displayPlaceholders, SpannableStringBuilder spannableString)
        {
            if (displayPlaceholders && projectIsPlaceholder)
            {
                spannableString.Append(projectName, new VerticallyCenteredImageSpan(context, Resource.Drawable.text_placeholder), SpanTypes.ExclusiveExclusive);
                spannableString.Append(placeholderSeparator);
            }
            else
                spannableString.Append(projectName, new ForegroundColorSpan(Color.ParseColor(projectColor)), SpanTypes.ExclusiveInclusive);
        }
    }
}
