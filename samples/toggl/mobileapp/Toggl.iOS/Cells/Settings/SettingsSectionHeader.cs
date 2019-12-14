using System;
using Foundation;
using Toggl.iOS.Extensions;
using UIKit;

namespace Toggl.iOS.Cells.Settings
{
    public partial class SettingsSectionHeader : BaseTableHeaderFooterView<string>
    {
        public static readonly string Identifier = new NSString("SettingsSectionHeader");
        public static readonly UINib Nib;

        static SettingsSectionHeader()
        {
            Nib = UINib.FromName("SettingsSectionHeader", NSBundle.MainBundle);
        }

        protected SettingsSectionHeader(IntPtr handle) : base(handle)
        {
            // Note: this .ctor should not contain any initialization logic.
        }

        public override void AwakeFromNib()
        {
            TitleLabel.TextColor = ColorAssets.Text2;
            ContentView.BackgroundColor = ColorAssets.TableBackground;
            ContentView.InsertSeparator();
        }

        protected override void UpdateView()
        {
            TitleLabel.Text = Item;
        }
    }
}

