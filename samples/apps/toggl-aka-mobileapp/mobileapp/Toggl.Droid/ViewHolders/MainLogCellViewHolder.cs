using Android.Graphics;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System;
using System.Reactive.Subjects;
using AndroidX.ConstraintLayout.Widget;
using Toggl.Core.Analytics;
using Toggl.Core.UI.Helper;
using Toggl.Core.UI.ViewModels.TimeEntriesLog;
using Toggl.Droid.Extensions;
using Toggl.Droid.ViewHelpers;
using static Toggl.Droid.Resource.Id;

namespace Toggl.Droid.ViewHolders
{
    public class MainLogCellViewHolder : BaseRecyclerViewHolder<TimeEntryViewData>
    {
        private Color cardColor;
        private Color backgroundColor;

        private GroupId groupId;
        private TextView timeEntriesLogCellDescription;
        private TextView addDescriptionLabel;
        private TextView timeEntriesLogCellProjectLabel;
        private TextView timeEntriesLogCellDuration;
        private ImageView timeEntriesLogCellProjectArchivedIcon;
        private View groupItemBackground;
        private View timeEntriesLogCellContinueImage;
        private View errorImageView;
        private View errorNeedsSync;
        private View timeEntriesLogCellContinueButton;
        private TextView mainLogBackgroundContinue;
        private TextView mainLogBackgroundDelete;
        private View billableIcon;
        private View hasTagsIcon;
        private View durationPadding;
        private View durationFadeGradient;
        private TextView groupCountTextView;
        private View groupExpansionButton;

        public bool CanSync => Item.ViewModel.CanContinue;
        public View MainLogContentView { get; private set; }
        public Subject<ContinueTimeEntryInfo> ContinueButtonTappedSubject { get; set; }
        public Subject<GroupId> ToggleGroupExpansionSubject { get; set; }

        public MainLogCellViewHolder(View itemView)
            : base(itemView)
        {
        }

        public MainLogCellViewHolder(IntPtr handle, JniHandleOwnership ownership)
            : base(handle, ownership)
        {
        }

        protected override void InitializeViews()
        {
            cardColor = ItemView.Context.SafeGetColor(Resource.Color.cardBackground);
            backgroundColor = ItemView.Context.SafeGetColor(Resource.Color.background);

            groupItemBackground = ItemView.FindViewById<View>(MainLogGroupBackground);
            groupCountTextView = ItemView.FindViewById<TextView>(TimeEntriesLogCellGroupCount);

            timeEntriesLogCellDescription = ItemView.FindViewById<TextView>(TimeEntriesLogCellDescription);
            addDescriptionLabel = ItemView.FindViewById<TextView>(AddDescriptionLabel);
            timeEntriesLogCellProjectLabel = ItemView.FindViewById<TextView>(TimeEntriesLogCellProjectLabel);
            timeEntriesLogCellDuration = ItemView.FindViewById<TextView>(TimeEntriesLogCellDuration);
            timeEntriesLogCellProjectArchivedIcon = ItemView.FindViewById<ImageView>(TimeEntriesLogCellProjectArchivedIcon);
            timeEntriesLogCellContinueImage = ItemView.FindViewById(TimeEntriesLogCellContinueImage);
            errorImageView = ItemView.FindViewById(ErrorImageView);
            errorNeedsSync = ItemView.FindViewById(ErrorNeedsSync);
            timeEntriesLogCellContinueButton = ItemView.FindViewById(TimeEntriesLogCellContinueButton);
            mainLogBackgroundContinue = ItemView.FindViewById<TextView>(MainLogBackgroundContinue);
            mainLogBackgroundDelete = ItemView.FindViewById<TextView>(MainLogBackgroundDelete);
            billableIcon = ItemView.FindViewById(TimeEntriesLogCellBillable);
            hasTagsIcon = ItemView.FindViewById(TimeEntriesLogCellTags);
            durationPadding = ItemView.FindViewById(TimeEntriesLogCellDurationPaddingArea);
            durationFadeGradient = ItemView.FindViewById(TimeEntriesLogCellDurationGradient);
            MainLogContentView = ItemView.FindViewById(Resource.Id.MainLogContentView);

            groupExpansionButton = ItemView.FindViewById(TimeEntriesLogCellToggleExpansionButton);
            timeEntriesLogCellContinueButton.Click += onContinueClick;
            groupExpansionButton.Click += onExpansionClick;

            mainLogBackgroundContinue.Text = Shared.Resources.Continue;
            mainLogBackgroundDelete.Text = Shared.Resources.Delete;
            addDescriptionLabel.Text = Shared.Resources.AddDescription;
        }

        private void onExpansionClick(object sender, EventArgs e)
        {
            ToggleGroupExpansionSubject.OnNext(groupId);
        }

        public void ShowSwipeToContinueBackground()
        {
            mainLogBackgroundContinue.Visibility = ViewStates.Visible;
            mainLogBackgroundDelete.Visibility = ViewStates.Invisible;
        }

        public void ShowSwipeToDeleteBackground()
        {
            mainLogBackgroundContinue.Visibility = ViewStates.Invisible;
            mainLogBackgroundDelete.Visibility = ViewStates.Visible;
        }

        public void HideSwipeBackgrounds()
        {
            mainLogBackgroundContinue.Visibility = ViewStates.Invisible;
            mainLogBackgroundDelete.Visibility = ViewStates.Invisible;
        }

        private void onContinueClick(object sender, EventArgs e)
        {
            var continueMode = Item.ViewModel.IsTimeEntryGroupHeader
                ? ContinueTimeEntryMode.TimeEntriesGroupContinueButton
                : ContinueTimeEntryMode.SingleTimeEntryContinueButton;

            ContinueButtonTappedSubject?.OnNext(new ContinueTimeEntryInfo(Item.ViewModel, continueMode));
        }

        private ConstraintLayout.LayoutParams getDurationPaddingWidthDependentOnIcons()
        {
            var whitePaddingWidth =
                72
                + (Item.ViewModel.IsBillable ? 22 : 0)
                + (Item.ViewModel.HasTags ? 22 : 0);

            var layoutParameters = (ConstraintLayout.LayoutParams)durationPadding.LayoutParameters;
            layoutParameters.Width = whitePaddingWidth.DpToPixels(ItemView.Context);
            return layoutParameters;
        }

        protected override void UpdateView()
        {
            groupId = Item.ViewModel.GroupId;

            timeEntriesLogCellDescription.Text = Item.ViewModel.Description;
            timeEntriesLogCellDescription.Visibility = Item.DescriptionVisibility;
            addDescriptionLabel.Visibility = Item.AddDescriptionLabelVisibility;

            timeEntriesLogCellProjectLabel.TextFormatted = Item.ProjectTaskClientText;
            timeEntriesLogCellProjectLabel.Visibility = Item.ProjectTaskClientVisibility;

            timeEntriesLogCellProjectArchivedIcon.Visibility = Item.ProjectArchivedIconVisibility;
            timeEntriesLogCellProjectArchivedIcon.SetColorFilter(Item.ProjectArchivedIconTintColor);

            timeEntriesLogCellDuration.Text = Item.ViewModel.Duration;

            timeEntriesLogCellContinueImage.Visibility = Item.ContinueImageVisibility;
            errorImageView.Visibility = Item.ErrorImageViewVisibility;
            errorNeedsSync.Visibility = Item.ErrorNeedsSyncVisibility;
            timeEntriesLogCellContinueButton.Visibility = Item.ContinueButtonVisibility;
            billableIcon.Visibility = Item.BillableIconVisibility;
            hasTagsIcon.Visibility = Item.HasTagsIconVisibility;

            durationPadding.LayoutParameters = getDurationPaddingWidthDependentOnIcons();

            switch (Item.ViewModel.VisualizationIntent)
            {
                case LogItemVisualizationIntent.SingleItem:
                    presentAsSingleTimeEntry();
                    break;

                case LogItemVisualizationIntent.GroupItem:
                    presentAsTimeEntryInAGroup();
                    break;

                case LogItemVisualizationIntent.CollapsedGroupHeader:
                    presentAsCollapsedGroupHeader(Item.ViewModel.RepresentedTimeEntriesIds.Length);
                    break;

                case LogItemVisualizationIntent.ExpandedGroupHeader:
                    presentAsExpandedGroupHeader(Item.ViewModel.RepresentedTimeEntriesIds.Length);
                    break;

                default:
                    throw new ArgumentOutOfRangeException($"Cannot visualize {Item.ViewModel.VisualizationIntent} in the time entries log table.");
            }
        }

        private void presentAsCollapsedGroupHeader(int timeEntriesCount)
        {
            groupExpansionButton.Enabled = true;
            groupCountTextView.SetBackgroundResource(Resource.Drawable.GrayBorderRoundedRectangle);
            groupCountTextView.Enabled = true;
            groupCountTextView.Text = timeEntriesCount.ToString();
            groupCountTextView.Visibility = ViewStates.Visible;
            groupCountTextView.SetTextColor(ItemView.Context.SafeGetColor(Resource.Color.primaryText));
            groupItemBackground.Visibility = ViewStates.Gone;
            durationPadding.SetBackgroundColor(cardColor);
            durationFadeGradient.SetBackgroundResource(Resource.Drawable.TransparentToCardColorGradient);
        }

        private void presentAsExpandedGroupHeader(int timeEntriesCount)
        {
            groupExpansionButton.Enabled = true;
            groupCountTextView.SetBackgroundResource(Resource.Drawable.TagsBackground);
            groupCountTextView.Enabled = true;
            groupCountTextView.Text = timeEntriesCount.ToString();
            groupCountTextView.Visibility = ViewStates.Visible;
            groupCountTextView.SetTextColor(ItemView.Context.SafeGetColor(Resource.Color.accent));
            groupItemBackground.Visibility = ViewStates.Gone;
            durationPadding.SetBackgroundColor(cardColor);
            durationFadeGradient.SetBackgroundResource(Resource.Drawable.TransparentToCardColorGradient);
        }

        private void presentAsSingleTimeEntry()
        {
            groupExpansionButton.Enabled = false;
            groupCountTextView.Visibility = ViewStates.Gone;
            groupItemBackground.Visibility = ViewStates.Gone;
            durationPadding.SetBackgroundColor(cardColor);
            durationFadeGradient.SetBackgroundResource(Resource.Drawable.TransparentToCardColorGradient);
        }

        private void presentAsTimeEntryInAGroup()
        {
            groupExpansionButton.Enabled = false;
            groupCountTextView.Visibility = ViewStates.Invisible;
            groupItemBackground.Visibility = ViewStates.Visible;
            durationPadding.SetBackgroundColor(backgroundColor);
            durationFadeGradient.SetBackgroundResource(Resource.Drawable.TransparentToBackgroundGradient);
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (!disposing)
                return;

            timeEntriesLogCellContinueButton.Click -= onContinueClick;
            groupExpansionButton.Click -= onExpansionClick;
        }

        public enum AnimationSide
        {
            Left,
            Right
        }
    }
}
