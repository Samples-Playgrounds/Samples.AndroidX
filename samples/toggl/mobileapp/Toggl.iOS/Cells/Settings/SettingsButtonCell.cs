using System;
using Foundation;
using Toggl.Core.UI.Helper;
using Toggl.iOS.Extensions;
using Toggl.iOS.ViewControllers.Settings.Models;
using UIKit;

namespace Toggl.iOS.Cells.Settings
{
    public partial class SettingsButtonCell : BaseTableViewCell<ISettingRow>
    {
        public static readonly string Identifier = nameof(SettingsButtonCell);
        public static readonly UINib Nib;

        static SettingsButtonCell()
        {
            Nib = UINib.FromName("SettingsButtonCell", NSBundle.MainBundle);
        }

        protected SettingsButtonCell(IntPtr handle) : base(handle)
        {
            // Note: this .ctor should not contain any initialization logic.
        }

        public override void AwakeFromNib()
        {
            TitleLabel.TextColor = UIColor.Red;
            ContentView.InsertSeparator();
        }

        protected override void UpdateView()
        {
            TitleLabel.Text = Item.Title;
        }
    }
}

