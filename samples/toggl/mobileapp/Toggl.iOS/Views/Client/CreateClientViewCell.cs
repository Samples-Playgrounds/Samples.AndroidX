using Foundation;
using System;
using Toggl.Core.UI.ViewModels;
using Toggl.iOS.Cells;
using Toggl.iOS.Extensions;
using Toggl.Shared;
using UIKit;

namespace Toggl.iOS.Views.Client
{
    public partial class CreateClientViewCell : BaseTableViewCell<SelectableClientBaseViewModel>
    {
        public static readonly string Identifier = nameof(CreateClientViewCell);
        public static readonly UINib Nib;

        static CreateClientViewCell()
        {
            Nib = UINib.FromName("CreateClientViewCell", NSBundle.MainBundle);
        }

        protected CreateClientViewCell(IntPtr handle) : base(handle)
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
            TextLabel.Text = $"{Resources.CreateClient} \"{Item.Name.Trim()}\"";
        }
    }
}

