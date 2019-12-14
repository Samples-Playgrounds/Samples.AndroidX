using Foundation;
using System;
using Toggl.iOS.Extensions;
using Toggl.Shared;
using UIKit;

namespace Toggl.iOS.Cells.Settings
{
    public sealed partial class LicensesViewCell : BaseTableViewCell<License>
    {
        public static readonly string Identifier = nameof(LicensesViewCell);
        public static readonly NSString Key = new NSString(nameof(LicensesViewCell));
        public static readonly UINib Nib;

        public string Text
        {
            get => LicenseLabel.Text;
            set => LicenseLabel.Text = value;
        }

        static LicensesViewCell()
        {
            Nib = UINib.FromName(nameof(LicensesViewCell), NSBundle.MainBundle);
        }

        public LicensesViewCell(IntPtr handle) : base(handle)
        {
        }

        public override void AwakeFromNib()
        {
            base.AwakeFromNib();

            GrayBackground.Layer.BorderWidth = 1;
            GrayBackground.Layer.BorderColor = ColorAssets.Separator.CGColor;
        }

        protected override void UpdateView()
        {
            Text = Item.Text;
        }
    }
}
