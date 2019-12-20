using Foundation;
using System;
using System.Text;
using CoreGraphics;
using Toggl.Core.Extensions;
using Toggl.Core.Models.Interfaces;
using Toggl.Core.Suggestions;
using Toggl.Core.UI.ViewModels.TimeEntriesLog;
using Toggl.iOS.Extensions;
using Toggl.Shared;
using UIKit;

namespace Toggl.iOS.Transformations
{
    public class ProjectTaskClientToAttributedString
    {
        private const string projectDotImageResource = "icProjectDot";
        private readonly UIColor clientColor;
        private readonly nfloat fontHeight;

        public ProjectTaskClientToAttributedString(nfloat fontHeight, UIColor clientColor)
        {
            Ensure.Argument.IsNotNull(clientColor, nameof(clientColor));

            this.clientColor = clientColor;
            this.fontHeight = fontHeight;
        }

        public NSAttributedString Convert(LogItemViewModel logItemViewModel)
        {
            var projectColor = new Color(logItemViewModel.ProjectColor).ToNativeColor();
            return Convert(
                logItemViewModel.ProjectName,
                logItemViewModel.TaskName,
                logItemViewModel.ClientName, projectColor,
                logItemViewModel.ProjectIsPlaceholder,
                logItemViewModel.TaskIsPlaceholder);
        }

        public NSAttributedString Convert(IThreadSafeTimeEntry timeEntry)
        {
            var projectColor = new Color(timeEntry.Project?.Color ?? string.Empty).ToNativeColor();
            return Convert(
                timeEntry.Project?.Name ?? "",
                timeEntry.Task?.Name ?? "",
                timeEntry.Project?.Client?.Name ?? "",
                projectColor,
                timeEntry.Project?.IsPlaceholder() ?? false,
                timeEntry.Task?.IsPlaceholder() ?? false);
        }

        public NSAttributedString Convert(Suggestion suggestion)
        {
            var projectColor = new Color(suggestion.ProjectColor ?? string.Empty).ToNativeColor();
            return Convert(
                suggestion.ProjectName ?? "",
                suggestion.TaskName ?? "",
                suggestion.ClientName ?? "",
                projectColor);
        }

        public NSAttributedString Convert(string project, string task, string client, UIColor color, bool isProjectPlaceholder = false, bool isTaskPlaceholder = false)
        {
            if (isProjectPlaceholder)
            {
                var attachment = new NSTextAttachment();
                attachment.Image = placeHolderImage();
                var placeholderImage = NSAttributedString.FromAttachment(attachment);
                return placeholderImage;
            }

            var dotString = projectDotImageResource.GetAttachmentString(fontHeight);
            var clone = new NSMutableAttributedString(dotString);
            var dottedString = tryAddColorToDot(clone, color);
            var projectInfo = buildString(project, task, client, color, isTaskPlaceholder);
            dottedString.Append(projectInfo);

            return dottedString;
        }

        private static NSMutableAttributedString tryAddColorToDot(NSMutableAttributedString dotString, UIColor color)
        {
            if (color == null) return dotString;

            var range = new NSRange(0, 1);
            var attributes = new UIStringAttributes { ForegroundColor = color };
            dotString.AddAttributes(attributes, range);

            return dotString;
        }

        private NSAttributedString buildString(string project, string task, string client, UIColor color, bool isTaskPlaceholder)
        {
            var builder = new StringBuilder();
            var hasClient = !string.IsNullOrEmpty(client);

            if (!string.IsNullOrEmpty(project))
                builder.Append($" {project}");

            if (!string.IsNullOrEmpty(task) && !isTaskPlaceholder)
                builder.Append($": {task}");

            if (hasClient)
                builder.Append($" {client}");

            var text = builder.ToString();

            var result = new NSMutableAttributedString(text);
            var clientIndex = text.Length - (client?.Length ?? 0);

            var projectNameRange = new NSRange(0, clientIndex);
            var projectNameAttributes = new UIStringAttributes { ForegroundColor = color };
            result.AddAttributes(projectNameAttributes, projectNameRange);

            if (hasClient)
            {
                var clientNameRange = new NSRange(clientIndex, client.Length);
                var clientNameAttributes = new UIStringAttributes {ForegroundColor = clientColor};
                result.AddAttributes(clientNameAttributes, clientNameRange);
            }

            if (isTaskPlaceholder)
            {
                var attachment = new NSTextAttachment();
                attachment.Image = placeHolderImage();
                var placeholderImage = new NSMutableAttributedString(" ");
                placeholderImage.Append(NSAttributedString.FromAttachment(attachment));
                result.Append(placeholderImage);
                return result;
            }

            return result;
        }

        private UIImage placeHolderImage()
        {
            var path = UIBezierPath.FromRoundedRect(new CGRect(0, 0, 100, 8), 4);
            UIGraphics.BeginImageContextWithOptions(path.Bounds.Size, false, 0);
            UIColor.LightGray.ColorWithAlpha(0.4f).SetFill();
            path.Fill();
            var image = UIGraphics.GetImageFromCurrentImageContext();
            UIGraphics.EndImageContext();

            return image;
        }
    }
}
