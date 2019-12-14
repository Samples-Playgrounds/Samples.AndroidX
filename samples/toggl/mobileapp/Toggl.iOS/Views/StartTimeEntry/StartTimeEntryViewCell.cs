using Foundation;
using System;
using Toggl.Core.Autocomplete.Suggestions;
using Toggl.Core.UI.Helper;
using Toggl.iOS.Cells;
using Toggl.iOS.Extensions;
using Toggl.iOS.Transformations;
using Toggl.Shared;
using UIKit;

namespace Toggl.iOS.Views
{
    public partial class StartTimeEntryViewCell : BaseTableViewCell<TimeEntrySuggestion>
    {
        private const float NoProjectDistance = 16;
        private const float HasProjectDistance = 8;

        private ProjectTaskClientToAttributedString projectTaskClientToAttributedString;

        public static readonly string Identifier = nameof(StartTimeEntryViewCell);
        public static readonly UINib Nib;

        static StartTimeEntryViewCell()
        {
            Nib = UINib.FromName(nameof(StartTimeEntryViewCell), NSBundle.MainBundle);
        }

        protected StartTimeEntryViewCell(IntPtr handle) : base(handle)
        {
            // Note: this .ctor should not contain any initialization logic.
        }

        public override void AwakeFromNib()
        {
            base.AwakeFromNib();

            projectTaskClientToAttributedString = new ProjectTaskClientToAttributedString(
                ProjectLabel.Font.CapHeight,
                Colors.Suggestions.ClientColor.ToNativeColor());

            this.InsertSeparator();
        }

        protected override void UpdateView()
        {
            //Text
            DescriptionLabel.Text = Item.Description;
            ProjectLabel.AttributedText = projectTaskClientToAttributedString.Convert(
                Item.ProjectName,
                Item.TaskName,
                Item.ClientName,
                new Color(Item.ProjectColor).ToNativeColor());

            //Visibility
            DescriptionTopDistanceConstraint.Constant = Item.HasProject ? HasProjectDistance : NoProjectDistance;
            ProjectLabel.Hidden = !Item.HasProject;
        }
    }
}
