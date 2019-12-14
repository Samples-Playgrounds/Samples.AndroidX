using Foundation;
using System;
using Toggl.Core.Models;
using Toggl.iOS.Cells;
using Toggl.iOS.Extensions;
using Toggl.Shared;
using UIKit;

namespace Toggl.iOS.Views.Settings
{
    public partial class SiriWorkflowCell : BaseTableViewCell<SiriWorkflow>
    {
        public static readonly string Identifier = nameof(SiriWorkflowCell);
        public static readonly UINib Nib;

        static SiriWorkflowCell()
        {
            Nib = UINib.FromName("SiriWorkflowCell", NSBundle.MainBundle);
        }

        protected SiriWorkflowCell(IntPtr handle) : base(handle)
        {
            // Note: this .ctor should not contain any initialization logic.
        }

        public override void AwakeFromNib()
        {
            base.AwakeFromNib();

            SelectionStyle = UITableViewCellSelectionStyle.None;
            BackgroundView.Layer.CornerRadius = 8;
        }

        protected override void UpdateView()
        {
            TitleLabel.Text = Item.Title;
            DescriptionLabel.Text = Item.Description;
            BackgroundView.BackgroundColor = new Color(Item.Color).ToNativeColor();
            IconImage.TintColor = UIColor.White;
            IconImage.Image = UIImage.FromBundle(Item.Icon).ImageWithRenderingMode(UIImageRenderingMode.AlwaysTemplate);
        }
    }
}

