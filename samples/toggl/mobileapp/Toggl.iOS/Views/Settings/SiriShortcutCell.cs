using Foundation;
using System;
using Toggl.Core.UI.Helper;
using Toggl.iOS.Cells;
using Toggl.iOS.Extensions;
using Toggl.iOS.ViewControllers.Settings;
using Toggl.Shared;
using UIKit;

namespace Toggl.iOS.Views.Settings
{
    public partial class SiriShortcutCell : BaseTableViewCell<SiriShortcutViewModel>
    {
        public static readonly string Identifier = nameof(SiriShortcutCell);
        public static readonly UINib Nib;

        static SiriShortcutCell()
        {
            Nib = UINib.FromName("SiriShortcutCell", NSBundle.MainBundle);
        }

        protected SiriShortcutCell(IntPtr handle) : base(handle)
        {
            // Note: this .ctor should not contain any initialization logic.
        }


        public override void AwakeFromNib()
        {
            base.AwakeFromNib();
            ContentView.InsertSeparator();
        }

        protected override void UpdateView()
        {
            TitleLabel.Text = Item.Title;

            if (Item.InvocationPhrase == null)
            {
                DetailLabel.Text = Resources.SiriShortcutCellAdd;
                DetailLabel.TextColor = Colors.Siri.AddButton.ToNativeColor();
            }
            else
            {
                DetailLabel.Text = $"\"{Item.InvocationPhrase}\"";
                DetailLabel.TextColor = ColorAssets.Text2;
            }
        }
    }
}

