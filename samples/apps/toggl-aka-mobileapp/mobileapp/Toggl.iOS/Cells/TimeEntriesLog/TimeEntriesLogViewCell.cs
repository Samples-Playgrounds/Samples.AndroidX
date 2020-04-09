using Foundation;
using System;
using System.Reactive;
using System.Reactive.Disposables;
using Toggl.iOS.Extensions;
using Toggl.iOS.Extensions.Reactive;
using Toggl.Core.UI.ViewModels.TimeEntriesLog;
using Toggl.iOS.Cells;
using Toggl.iOS.Transformations;
using Toggl.Shared;
using UIKit;
using System.Reactive.Subjects;
using System.Reactive.Linq;

namespace Toggl.iOS.Views
{
    public partial class TimeEntriesLogViewCell : BaseTableViewCell<LogItemViewModel>
    {
        public static readonly string Identifier = "timeEntryCell";

        private ProjectTaskClientToAttributedString projectTaskClientToAttributedString;

        public static readonly NSString Key = new NSString(nameof(TimeEntriesLogViewCell));
        public static readonly UINib Nib;

        public CompositeDisposable DisposeBag = new CompositeDisposable();

        public IObservable<Unit> ContinueButtonTap
            => ContinueButton.Rx().Tap();

        private ISubject<Unit> accessibilityToggleGroupSubject = new Subject<Unit>();

        public IObservable<Unit> ToggleGroup
            => accessibilityToggleGroupSubject.AsObservable()
                .Merge(GroupSizeContainer.Rx().Tap());

        static TimeEntriesLogViewCell()
        {
            Nib = UINib.FromName(nameof(TimeEntriesLogViewCell), NSBundle.MainBundle);
        }

        protected TimeEntriesLogViewCell(IntPtr handle)
            : base(handle)
        {
        }

        public override void AwakeFromNib()
        {
            base.AwakeFromNib();

            IsAccessibilityElement = true;
            AccessibilityTraits = UIAccessibilityTrait.Button;
            AccessibilityHint = Resources.TimeEntryRowAccessibilityHint;

            DescriptionFadeView.FadeRight = true;
            ProjectTaskClientFadeView.FadeRight = true;

            TimeLabel.Font = TimeLabel.Font.GetMonospacedDigitFont();

            projectTaskClientToAttributedString = new ProjectTaskClientToAttributedString(
                ProjectTaskClientLabel.Font.CapHeight,
                ColorAssets.Text3
            );

            GroupSizeBackground.Layer.CornerRadius = 14;

            ContinueImageView.SetTemplateColor(ColorAssets.Text4);

            this.InsertSeparator();
        }

        public override void PrepareForReuse()
        {
            base.PrepareForReuse();
            DisposeBag.Dispose();
            DisposeBag = new CompositeDisposable();
        }

        protected override void UpdateView()
        {
            updateAccessibilityProperties();

            // Text
            DescriptionLabel.Text = Item.HasDescription ? Item.Description : Resources.AddDescription;
            ProjectTaskClientLabel.AttributedText = projectTaskClientToAttributedString.Convert(Item);
            TimeLabel.Text = Item.Duration;

            // Colors
            DescriptionLabel.TextColor = Item.HasDescription
                ? ColorAssets.Text
                : ColorAssets.Text3;

            ContentView.BackgroundColor = ColorAssets.CellBackground;

            // Visibility
            ProjectTaskClientFadeView.Hidden = !Item.HasProject;
            SyncErrorImageView.Hidden = Item.CanContinue;
            UnsyncedImageView.Hidden = !Item.NeedsSync;
            ContinueButton.Hidden = !Item.CanContinue;
            ContinueImageView.Hidden = !Item.CanContinue;
            BillableIcon.Hidden = !Item.IsBillable;
            TagIcon.Hidden = !Item.HasTags;

            switch (Item.VisualizationIntent)
            {
                case LogItemVisualizationIntent.SingleItem:
                    presentAsSingleTimeEntry();
                    break;

                case LogItemVisualizationIntent.GroupItem:
                    presentAsTimeEntryInAGroup();
                    break;

                case LogItemVisualizationIntent.CollapsedGroupHeader:
                    presentAsCollapsedGroupHeader(Item.RepresentedTimeEntriesIds.Length);
                    break;

                case LogItemVisualizationIntent.ExpandedGroupHeader:
                    presentAsExpandedGroupHeader(Item.RepresentedTimeEntriesIds.Length);
                    break;

                default:
                    throw new ArgumentOutOfRangeException($"Cannot visualize {Item.VisualizationIntent} in the time entries log table.");
            }
        }

        protected override void Dispose(bool disposing)
        {
            DisposeBag.Dispose();
            base.Dispose(disposing);
        }

        private void presentAsCollapsedGroupHeader(int groupSize)
        {
            TimeEntryContentLeadingConstraint.Constant = 0;
            GroupSizeLabel.Text = groupSize.ToString();
            GroupSizeContainer.Hidden = false;
            GroupSizeContainer.UserInteractionEnabled = true;
            GroupSizeBackground.Hidden = false;
            GroupSizeBackground.Layer.BorderWidth = 1;
            GroupSizeBackground.Layer.BorderColor = ColorAssets.Separator.CGColor;
            GroupSizeBackground.BackgroundColor = ColorAssets.CellBackground;
            GroupSizeLabel.TextColor = ColorAssets.Text2;
        }

        private void presentAsExpandedGroupHeader(int groupSize)
        {
            TimeEntryContentLeadingConstraint.Constant = 0;
            GroupSizeLabel.Text = groupSize.ToString();
            GroupSizeContainer.Hidden = false;
            GroupSizeContainer.UserInteractionEnabled = true;
            GroupSizeBackground.Hidden = false;
            GroupSizeBackground.Layer.BorderWidth = 0;
            GroupSizeBackground.BackgroundColor = ColorAssets.CustomGray5;
            GroupSizeLabel.TextColor = ColorAssets.LightishGreen;
        }

        private void presentAsSingleTimeEntry()
        {
            GroupSizeContainer.Hidden = true;
            TimeEntryContentLeadingConstraint.Constant = 16;
        }

        private void presentAsTimeEntryInAGroup()
        {
            TimeEntryContentLeadingConstraint.Constant = 0;
            GroupSizeContainer.Hidden = false;
            GroupSizeContainer.UserInteractionEnabled = false;
            GroupSizeBackground.Hidden = true;
            ContentView.BackgroundColor = ColorAssets.CustomGray6;
        }

        private void updateAccessibilityProperties()
        {
            updateAccessibilityActionToExpandGroup();
            AccessibilityLabel = "";

            if (Item.IsTimeEntryGroupHeader)
                AccessibilityLabel += $"{Item.RepresentedTimeEntriesIds.Length} {Resources.TimeEntries}, ";

            if (Item.HasDescription)
                AccessibilityLabel += $"{Item.Description}, ";

            if (Item.HasProject)
                AccessibilityLabel += $"{Resources.Project}: {Item.ProjectName }, ";

            if (string.IsNullOrEmpty(AccessibilityLabel))
                AccessibilityLabel += $"{Resources.TimeEntry}, ";

            AccessibilityLabel += $"{Item.Duration}, ";

            if (Item.HasTags)
                AccessibilityLabel += $"{Resources.HasTags}, ";

            if (Item.IsBillable)
                AccessibilityLabel += $"{Resources.IsBillable}, ";
        }

        private void updateAccessibilityActionToExpandGroup()
        {
            if (Item.VisualizationIntent == LogItemVisualizationIntent.GroupItem
                || Item.VisualizationIntent == LogItemVisualizationIntent.SingleItem)
            {
                AccessibilityCustomActions = null;
                return;
            }

            var actionName = Item.VisualizationIntent == LogItemVisualizationIntent.CollapsedGroupHeader
                ? Resources.ExpandTimeEntryGroup
                : Resources.CollapseTimeEntryGroup;

            var action = new UIAccessibilityCustomAction(actionName, probe: (x) =>
            {
                accessibilityToggleGroupSubject.OnNext(Unit.Default);
                return true;
            });
            AccessibilityCustomActions = new[] { action };
        }
    }
}
