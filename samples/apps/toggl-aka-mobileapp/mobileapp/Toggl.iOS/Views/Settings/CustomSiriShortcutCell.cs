using Foundation;
using System;
using Toggl.Core.UI.Helper;
using Toggl.iOS.Cells;
using Toggl.iOS.Extensions;
using Toggl.iOS.Transformations;
using Toggl.iOS.ViewControllers.Settings;
using Toggl.Shared;
using UIKit;

namespace Toggl.iOS.Views.Settings
{
    public partial class CustomSiriShortcutCell : BaseTableViewCell<SiriShortcutViewModel>
    {
        public static readonly string Identifier = nameof(CustomSiriShortcutCell);
        public static readonly UINib Nib;

        private ProjectTaskClientToAttributedString projectTaskClientToAttributedString;

        static CustomSiriShortcutCell()
        {
            Nib = UINib.FromName("CustomSiriShortcutCell", NSBundle.MainBundle);
        }

        protected CustomSiriShortcutCell(IntPtr handle) : base(handle)
        {
            // Note: this .ctor should not contain any initialization logic.
        }

        public override void AwakeFromNib()
        {
            base.AwakeFromNib();

            projectTaskClientToAttributedString = new ProjectTaskClientToAttributedString(
                DetailsLabel.Font.CapHeight,
                Colors.TimeEntriesLog.ClientColor.ToNativeColor()
            );

            ContentView.InsertSeparator();
        }

        protected override void UpdateView()
        {
            var projectColor = new Color(Item.ProjectColor).ToNativeColor();

            TitleLabel.Text = Item.Title;
            DetailsLabel.TextColor = ColorAssets.Text2;

            if (Item.ProjectName == null)
                DetailsLabel.Text = string.Format(Resources.CustomSiriShortcutCellIn, Item.WorkspaceName);
            else
                DetailsLabel.AttributedText = projectTaskClientToAttributedString.Convert(Item.ProjectName, null, Item.ClientName, projectColor);

            InvocationLabel.Text = $"\"{Item.InvocationPhrase}\"";
            InvocationLabel.TextColor = ColorAssets.Text2;

            BillableIcon.Hidden = !Item.IsBillable;
            TagsIcon.Hidden = !Item.HasTags;
        }
    }
}

