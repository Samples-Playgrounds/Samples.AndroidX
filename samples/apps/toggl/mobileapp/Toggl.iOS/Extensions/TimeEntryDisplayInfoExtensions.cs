using System;
using System.Text;
using Foundation;
using Toggl.Core.UI.ViewModels.Calendar.ContextualMenu;
using Toggl.Shared;
using UIKit;

namespace Toggl.iOS.Extensions
{
    public static class TimeEntryDisplayInfoExtensions
    {
        private const string projectDotImageResource = "icProjectDot";

        public static NSAttributedString ToAttributedString(
            this TimeEntryDisplayInfo timeEntryDisplayInfo,
            nfloat fontHeight)
        {
            var attributedString = new NSMutableAttributedString();

            if (!string.IsNullOrEmpty(timeEntryDisplayInfo.Description))
                attributedString.Append(new NSAttributedString(timeEntryDisplayInfo.Description));

            if (string.IsNullOrEmpty(timeEntryDisplayInfo.Project))
                return attributedString;

            var color = new Color(timeEntryDisplayInfo.ProjectTaskColor).ToNativeColor();
            attributedString.Append(new NSAttributedString(" "));
            attributedString.Append(getColoredDot(color, fontHeight));

            attributedString.Append(getFormattedProjectInfo(timeEntryDisplayInfo, color, UIColor.LightGray));

            return attributedString;
        }

        private static NSMutableAttributedString getColoredDot(UIColor color, nfloat fontHeight)
        {
            var dot = projectDotImageResource.GetAttachmentString(fontHeight);

            if (color == null)
                return dot;

            var range = new NSRange(0, 1);
            var attributes = new UIStringAttributes { ForegroundColor = color };
            dot.AddAttributes(attributes, range);

            return dot;
        }

        private static NSAttributedString getFormattedProjectInfo(
            TimeEntryDisplayInfo timeEntry,
            UIColor projectColor,
            UIColor clientColor)
        {
            var builder = new StringBuilder();
            var hasProject =  !string.IsNullOrEmpty(timeEntry.Project);
            var hasTask = !string.IsNullOrEmpty(timeEntry.Task);
            var hasClient = !string.IsNullOrEmpty(timeEntry.Client);

            if (hasProject)
                builder.Append($" {timeEntry.Project}");

            if (hasTask)
                builder.Append($": {timeEntry.Task}");

            if (hasClient)
                builder.Append($" {timeEntry.Client}");

            var text = builder.ToString();

            var result = new NSMutableAttributedString(text);
            var clientIndex = text.Length - (timeEntry.Client?.Length ?? 0);
            if (projectColor != null)
            {
                var projectNameRange = new NSRange(0, clientIndex);
                var projectNameAttributes = new UIStringAttributes { ForegroundColor = projectColor };
                result.AddAttributes(projectNameAttributes, projectNameRange);
            }

            if (!hasClient) return result;

            var clientNameRange = new NSRange(clientIndex, timeEntry.Client.Length);
            var clientNameAttributes = new UIStringAttributes { ForegroundColor = clientColor };
            result.AddAttributes(clientNameAttributes, clientNameRange);

            return result;
        }
    }
}
