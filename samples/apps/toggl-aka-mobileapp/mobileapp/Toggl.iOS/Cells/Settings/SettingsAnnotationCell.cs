using System;
using Foundation;
using Toggl.iOS.Extensions;
using Toggl.iOS.ViewControllers.Settings.Models;
using UIKit;

namespace Toggl.iOS.Cells.Settings
{
    public partial class SettingsAnnotationCell : BaseTableViewCell<ISettingRow>
    {
        public static readonly string Identifier = nameof(SettingsAnnotationCell);
        public static readonly UINib Nib;

        static SettingsAnnotationCell()
        {
            Nib = UINib.FromName("SettingsAnnotationCell", NSBundle.MainBundle);
        }

        protected SettingsAnnotationCell(IntPtr handle) : base(handle)
        {
            // Note: this .ctor should not contain any initialization logic.
        }

        public override void AwakeFromNib()
        {
            TextLabel.TextColor = ColorAssets.Text2;
            ContentView.BackgroundColor = ColorAssets.TableBackground;
        }

        protected override void UpdateView()
        {
            TextLabel.Text = Item.Title;
        }
    }
}

