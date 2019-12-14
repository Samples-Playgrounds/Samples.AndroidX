using Foundation;
using System;
using Toggl.Core.UI.ViewModels;
using Toggl.iOS.Cells;
using Toggl.iOS.Extensions;
using UIKit;

namespace Toggl.iOS.Views.Client
{
    public partial class ClientViewCell : BaseTableViewCell<SelectableClientBaseViewModel>
    {
        public static readonly string Identifier = nameof(ClientViewCell);
        public static readonly UINib Nib;

        static ClientViewCell()
        {
            Nib = UINib.FromName(nameof(ClientViewCell), NSBundle.MainBundle);
        }

        protected ClientViewCell(IntPtr handle) : base(handle)
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
            NameLabel.Text = Item.Name;
        }
    }
}
