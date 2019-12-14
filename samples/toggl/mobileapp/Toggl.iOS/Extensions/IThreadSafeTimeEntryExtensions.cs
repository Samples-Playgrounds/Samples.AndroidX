using Foundation;
using System;
using System.Text;
using Toggl.Core.Extensions;
using Toggl.Core.Models.Interfaces;
using Toggl.Shared;
using UIKit;

namespace Toggl.iOS.Extensions
{
    public static class IThreadSafeTimeEntryExtensions
    {
        private const string projectDotImageResource = "icProjectDot";

        public static NSAttributedString ToFormattedTimeEntryString(
            this IThreadSafeTimeEntry timeEntry,
            nfloat fontHeight,
            UIColor clientColor,
            bool shouldColorProject)
        {
            if (string.IsNullOrEmpty(timeEntry.Project?.Name))
                return new NSAttributedString("");

            var displayColor = timeEntry.Project.DisplayColor();
            var color = string.IsNullOrEmpty(displayColor) ? null
                      : new Color(displayColor).ToNativeColor();

            var projectColor = shouldColorProject ? color : null;

            var result = getColoredDot(color, fontHeight);
            var projectInfo = getFormattedProjectInfo(timeEntry, projectColor, clientColor);
            result.Append(projectInfo);

            return result;
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
            IThreadSafeTimeEntry timeEntry,
            UIColor projectColor,
            UIColor clientColor)
        {
            var builder = new StringBuilder();
            var hasClient = !string.IsNullOrEmpty(timeEntry.Project.Client?.Name);

            if (!string.IsNullOrEmpty(timeEntry.Project.Name))
                builder.Append($" {timeEntry.Project.Name}");

            if (!string.IsNullOrEmpty(timeEntry.Task?.Name))
                builder.Append($": {timeEntry.Task?.Name}");

            if (hasClient)
                builder.Append($" {timeEntry.Project.Client?.Name}");

            var text = builder.ToString();

            var result = new NSMutableAttributedString(text);
            var clientIndex = text.Length - (timeEntry.Project?.Client?.Name.Length ?? 0);
            if (projectColor != null)
            {
                var projectNameRange = new NSRange(0, clientIndex);
                var projectNameAttributes = new UIStringAttributes { ForegroundColor = projectColor };
                result.AddAttributes(projectNameAttributes, projectNameRange);
            }

            if (!hasClient) return result;

            var clientNameRange = new NSRange(clientIndex, timeEntry.Project.Client.Name.Length);
            var clientNameAttributes = new UIStringAttributes { ForegroundColor = clientColor };
            result.AddAttributes(clientNameAttributes, clientNameRange);

            return result;
        }
    }
}
