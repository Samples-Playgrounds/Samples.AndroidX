using Foundation;
using System;
using System.Diagnostics;
using UIKit;
using ObjCRuntime;
using Toggl.Core.Suggestions;
using Toggl.iOS.Extensions;
using Toggl.Shared;
using CoreGraphics;
using Toggl.Core.UI.Helper;
using Toggl.iOS.Transformations;

namespace Toggl.iOS
{
    public sealed partial class SuggestionView : UIView
    {
        ProjectTaskClientToAttributedString projectTaskClientToAttributedString;

        public SuggestionView(IntPtr handle) : base(handle)
        {
        }

        public static SuggestionView Create()
        {
            var arr = NSBundle.MainBundle.LoadNib(nameof(SuggestionView), null, null);
            return Runtime.GetNSObject<SuggestionView>(arr.ValueAt(0));
        }

        private Suggestion suggestion;
        public Suggestion Suggestion
        {
            get => suggestion;
            set
            {
                if (suggestion == value) return;
                suggestion = value;
                onSuggestionChanged();
            }
        }

        private void onSuggestionChanged()
        {
            if (Suggestion == null) return;

            updateAccessibilityProperties();

            Hidden = false;

            DescriptionLabel.Text = Suggestion.Description;
            DescriptionLabel.Hidden = Suggestion.Description == string.Empty;
            NoDescriptionLabel.Hidden = Suggestion.Description != string.Empty;

            prefixWithProviderNameInDebug();

            var hasProject = Suggestion.ProjectId != null;
            ProjectFadeView.Hidden = !hasProject;

            ProjectLabel.AttributedText = projectTaskClientToAttributedString.Convert(Suggestion);
        }

        public override void AwakeFromNib()
        {
            base.AwakeFromNib();

            IsAccessibilityElement = true;
            AccessibilityHint = Resources.SuggestionAccessibilityHint;
            AccessibilityTraits = UIAccessibilityTrait.Button;
            NoDescriptionLabel.Text = Resources.NoDescription;

            DescriptionFadeView.FadeRight = true;
            ProjectFadeView.FadeRight = true;

            if (Suggestion == null)
                Hidden = true;

            projectTaskClientToAttributedString = new ProjectTaskClientToAttributedString(
                ProjectLabel.Font.CapHeight,
                Colors.TimeEntriesLog.ClientColor.ToNativeColor()
            );

            ArrowImage.SetTemplateColor(ColorAssets.Text4);
        }

        public override void LayoutSubviews()
        {
            base.LayoutSubviews();

            var shadowPath = UIBezierPath.FromRect(Bounds);
            Layer.ShadowPath?.Dispose();
            Layer.ShadowPath = shadowPath.CGPath;

            Layer.CornerRadius = 8;
            Layer.ShadowRadius = 4;
            Layer.ShadowOpacity = 0.1f;
            Layer.MasksToBounds = false;
            Layer.ShadowOffset = new CGSize(0, 2);
            Layer.ShadowColor = UIColor.Black.CGColor;
        }

        private void updateAccessibilityProperties()
        {
            AccessibilityLabel = $"{Resources.Suggestion}, ";
            if (!string.IsNullOrEmpty(suggestion.Description))
                AccessibilityLabel += $"{suggestion.Description}, ";
            if (suggestion.HasProject)
                AccessibilityLabel += $"{Resources.Project}: {suggestion.ProjectName }, ";
            if (suggestion.HasTask)
                AccessibilityLabel += $"{Resources.Task}: {suggestion.TaskName}, ";
            if (suggestion.HasClient)
                AccessibilityLabel += $"{Resources.Client}: {suggestion.ClientName}";
        }

        [Conditional("DEBUG")]
        private void prefixWithProviderNameInDebug()
        {
            var prefix = Suggestion.ProviderType.ToString().Substring(0, 4);
            DescriptionLabel.Text = $"{prefix} {Suggestion.Description}";
        }
    }
}
